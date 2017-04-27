using UnityEngine;
using System.Collections.Generic;

public class ScoreManager {

    #region Score Constants

    private const int GOALKEEPER_BALLSAVED = 500;
    private const int GOALKEEPER_BALLSTOPPED_NORMAL = 50;
    private const int GOALKEEPER_BALLSTOPPED_MEDIUM = 100;
    private const int GOALKEEPER_BALLSTOPPED_BEST = 200;
    private const int GOALKEEPER_POWERUP_BONUS = 100;
    private const int GOALKEEPER_THROWERFAIL_BONUS = 50;
    private const int SHOOTER_SCORED_VS_GOALKEEPER = 100;
    private const int SHOOTER_NOBULLSEYEHIT_GOAL = 10;
    private const int SHOOTER_YELLOWZONE_GOAL = 10;

    #endregion

    #region Score Addition

    public void AddScore (ShotResult result) {
        ServiceLocator.Request<IPlayerService>().AddPoints(
            ApplyScoreMultiplierToScore( result.ScorePoints ) + result.EffectBonusPoints );
    }

    #endregion

    #region Score Calculation

    /// <summary>
    /// Calcula la puntuacion de un disparo
    /// </summary>
    /// <param name="result"></param>
    public void CalculateScore (ref ShotResult result) {

        switch ( ServiceLocator.Request<IGameplayService>().GetGameMode() ) {
            case GameMode.GoalKeeper:
                CalculateGoalkeeperScore( ref result );
                break;
            case GameMode.Shooter:
                if ( GameplayService.networked ) {
                    CalculateShooterDuelScore( ref result );
                }
                else {
                    CalculateShooterMissionScore( ref result );
                }
                break;
        }

        // TODO: aplicar multiplicador de puntuacion
    }        

    /// <summary>
    /// Calcula la puntuacion para un disparo si estamos jugando como portero.
    /// El calculo de puntuacion es igual para duelos que para misiones.
    /// </summary>
    /// <param name="result"></param>
    private void CalculateGoalkeeperScore (ref ShotResult result) {

        int scoreResult = 0;

        switch ( result.Result ) {
            case Result.Saved: // balon atrapado
                scoreResult = GOALKEEPER_BALLSAVED;
                break;
            case Result.Stopped: {
                    // balon despejado, habra que ver si lo hemos parado nosotros, o ha ido al palo
                    switch ( result.DefenseResult ) {
                        case GKResult.Good: // hemos parado el tiro, así que merecemos puntos!
                            scoreResult = GOALKEEPER_BALLSTOPPED_NORMAL;
                            switch (result.Precision)
                            {
                                case 1:
                                    scoreResult = GOALKEEPER_BALLSTOPPED_BEST;
                                    break;
                                case 2:
                                    scoreResult = GOALKEEPER_BALLSTOPPED_MEDIUM;
                                    break;
                                case 3:
                                    scoreResult = GOALKEEPER_BALLSTOPPED_NORMAL;
                                    break;
                            }

                            // Si el portero tiene la habilidad "Heroico"
                            if ( Habilidades.IsActiveSkill( Habilidades.Skills.Heroico ) ) {
                                // Habilidad Heroico = las paradas no perfectas puntuan doble
                                scoreResult *= 2;
                            }
                            break;
                        case GKResult.ThrowerFail: // el tirador ha tirado a un poste, no mereces puntos
                            scoreResult = GOALKEEPER_THROWERFAIL_BONUS;
                            break;
                        case GKResult.Perfect: // no puede darse, porque si no el result no seria Stopped
                            scoreResult = 0;
                            break;
                    }
                }
                break;
            case Result.OutOfBounds: // El tirador ha fallado, no has hecho nada, no te mereces puntos
                scoreResult = GOALKEEPER_THROWERFAIL_BONUS;
                break;
            case Result.Goal: // Nos han metido gol, te jodes, no hay puntos
                scoreResult = 0;
                break;
            case Result.Target: // IMPOSIBLE: que coño pinta una diana si estamos jugando de porteros??
                scoreResult = 0;
                break;
        }

        if ( MissionManager.instance.HasCurrentMission() ) {
            GoalkeeperMissionRound ms = MissionManager.instance.GetMission().GetRoundInfo() as GoalkeeperMissionRound;

            Debug.Log(">>> ms: " + ms);
            Debug.Log(">>> result: " + result);

            if(ms.HasPowerUp && (result.Result !=  Result.Goal)) scoreResult += GOALKEEPER_POWERUP_BONUS;
        }
        

        result.ScorePoints = scoreResult;
    }    

    /// <summary>
    /// Calcula la puntuacion para un disparo en el modo duelo si estamos
    /// jugando como lanzador
    /// </summary>
    /// <param name="result"></param>
    private void CalculateShooterDuelScore (ref ShotResult result) {
        int puntuacionBase = CalculateShooterAgainstGoalkeeperScore( ref result );
        result.ScorePoints = puntuacionBase;
        if(result.Result == Result.Goal || result.Result == Result.Target)
        {
            result.EffectBonusPoints = GetEffectBonusPoints();
        }
    }

    /// <summary>
    /// Calcula la puntuacion para un disparo en el modo carrera si estamos
    /// jugando como lanzador
    /// </summary>
    /// <param name="result"></param>
    private void CalculateShooterMissionScore (ref ShotResult result) {

        int scoreResult = 0;
        int effectBonusScore = 0;

        if ( MissionManager.instance.HasCurrentMission() ) {
            ShooterMissionRound ms = MissionManager.instance.GetMission().GetRoundInfo() as ShooterMissionRound;
            if(ms == null)
            {
                ms = MissionManager.instance.GetMission().GetPrevRoundInfo() as ShooterMissionRound;
            }

            if ( ms.HasBullseye ) {
                switch ( result.Result ) {
                    case Result.Target: // Si ha acertado una diana
                        scoreResult = result.ScorePoints; // los puntos que nos llegaron en el ShotResult son buenos
                        //effectBonusScore = GetEffectBonusPoints();
                        if(!ms.bullseyeDesc.HasStaticSize) {
                            scoreResult *= 2;
                        }

                        // Si el tirador tiene la habilidad "Prima"
                        if ( Habilidades.IsActiveSkill( Habilidades.Skills.Prima ) ) {
                            // Habilidad Prima = las dianas puntuan doble (acumulable con bonus de balon)
                            scoreResult *= 2;
                        }
                        break;
                    case Result.Goal: // hay gol, por lo que no hemos atizado a la diana
                        if ( ms.bullseyeDesc.HasYellowZone ) {
                            // si tiene zona amarilla es que le hemos atizado a la zona amarilla
                            scoreResult = SHOOTER_NOBULLSEYEHIT_GOAL;
                        }
                        else {
                            // si no tiene zona amarilla se lo damos como gol normal
                            scoreResult = SHOOTER_NOBULLSEYEHIT_GOAL;
                        }
                        break;
                }
            }
            else if ( ms.HasGoalkeeper ) {
                scoreResult = CalculateShooterAgainstGoalkeeperScore( ref result );
            }
            else if ( ms.HasSheet ) {
                // Si ha acertado a una sabana
                if ( result.Result == Result.Target ) {
                    scoreResult = result.ScorePoints; // los puntos que nos llegan serán los buenos.
                    //effectBonusScore = GetEffectBonusPoints();
                }
            }

            if(result.Result == Result.Goal || result.Result == Result.Target)
            {
                effectBonusScore = GetEffectBonusPoints();
            }
        }

        result.ScorePoints = scoreResult;
        result.EffectBonusPoints = effectBonusScore;
    }

    private int CalculateShooterAgainstGoalkeeperScore (ref ShotResult result) {
        int scoreResult = 0;

        // si hemos marcado un gol
        if ( result.Result == Result.Goal ) {
            scoreResult = SHOOTER_SCORED_VS_GOALKEEPER;

            // Si el tirador tiene la habilidad "Goleador"                        
            if ( Habilidades.IsActiveSkill( Habilidades.Skills.Goleador ) ) {
                // Habilidad Goleador = los goles a portero puntuan doble (acumulable con bonus de balon)
                scoreResult *= 2;
            }
        }

        return scoreResult;
    }

    #endregion

    #region Score Multipliers

    public int ApplyScoreMultiplierToScore (int score) {
        return Mathf.CeilToInt( (float)score * EscudosManager.escudoEquipado.boost );
    }

    #endregion

    #region BullsEye Score

    public enum BullsEyeScore {
        Red = 100,
        Yellow = 75,
        Blue = 50,
        White = 25,
    }

    #endregion

    #region Sheet Score

    public enum SheetScore {
        S = 100,
        M = 75,
        L = 50,
    }

    #endregion

    #region Effect Bonus

    public enum EffectBonus {
        NONE = 0,
        LOW = 25,
        MEDIUM = 50,
        HIGH = 100,
    }

    private ShotInfo _lastShotInfo;

    public void SetLastShotInfo (ShotInfo shotInfo) {
        _lastShotInfo = shotInfo;
    }

    private int GetEffectBonusPoints () {
        int effectPoints = 0;

        float effect = Mathf.Abs( _lastShotInfo.Effect01 );

        if ( effect > 0.9f ) {
            effectPoints = (int)EffectBonus.HIGH;
        }
        else if ( effect > 0.75f ) {
            effectPoints = (int)EffectBonus.MEDIUM;
        }
        else if ( effect > 0.3f ) {
            effectPoints = (int)EffectBonus.LOW;
        }
        else {
            effectPoints = (int)EffectBonus.NONE;
        }

        return effectPoints;
    }

    #endregion

    #region Singleton

    private ScoreManager () { }

    protected static ScoreManager _instance = null;

    public static ScoreManager Instance {
        get {
            if ( _instance == null ) {
                _instance = new ScoreManager();
            }

            return _instance; 
        }
    }

    #endregion
}
