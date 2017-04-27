using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase se encargara de llevar las estadisticas de la mision.
/// Se subscribe al servicio IShotResultService y va guardando la informacion de cada tiro.
/// Esta informacion se utilizara al finalizar la partida, para saber si se han cumplido
/// los objetivos de la mision.
/// </summary>
public class MissionStats : IDisposable {

    public class MissionStat {
        private int _total;
        private int _streak;
        private int _maxStreak;

        public MissionStat () {
            Reset();
        }

        public void Reset () {
            _total = 0;
            ResetStreak();
            _maxStreak = 0;
        }

        public void Update (bool bIncrease) {
            if ( bIncrease ) {
                _total++;
                IncreaseStreak();
            }
            else {
                ResetStreak();
            }
        }

        public int GetTotal () { return _total; }

        public int GetMaxStreak () { return _maxStreak; }

        private void ResetStreak () {
            _streak = 0;
        }

        private void IncreaseStreak () {
            _streak++;
            if ( _streak > _maxStreak ) {
                _maxStreak = _streak;
            }
        }
    }

    Dictionary<string, MissionStat> _missionStats = new Dictionary<string, MissionStat>() {
        // Aciertos Lanzador
        { "shooterWinsGeneric", new MissionStat() },
        { "shooterWinsTarget", new MissionStat() },
        { "shooterWinsGoalkeeper", new MissionStat() },
        { "shooterWinsSheet", new MissionStat() },
        // Aciertos Portero
        { "goalkeeperWinsGeneric", new MissionStat() },
        { "goalkeeperCatched", new MissionStat() },
        { "goalkeeperSaved", new MissionStat() },
        { "goalkeeperReceivedGoals", new MissionStat() },
        // Perfects
        { "perfects", new MissionStat() },
        // Effect Bonuses
        { "effectBonusGeneric", new MissionStat() },
        { "effectBonusLow", new MissionStat() },
        { "effectBonusMedium", new MissionStat() },
        { "effectBonusHigh", new MissionStat() },
    };

    public void Reset () {
        foreach ( var stat in _missionStats ) {
            stat.Value.Reset();
        }        
    }

    public void OnShotResult (ShotResult result) {
        // Perfects
        _missionStats[ "perfects" ].Update( result.Perfect );

        // Lanzador

        // acierto = hemos metido gol o le hemos dado a una diana
        _missionStats[ "shooterWinsGeneric" ].Update( ( result.Result == Result.Goal || result.Result == Result.Target ) );
        // acierto diana = le hemos dado a una diana
        _missionStats[ "shooterWinsTarget" ].Update( ( result.Result == Result.Target && FieldControl.instance.HasBullseye ) );
        // acierto portero = hemos metido gol habiendo un portero
        _missionStats[ "shooterWinsGoalkeeper" ].Update( ( result.Result == Result.Goal && FieldControl.instance.goalKeeper ) );        
        // acierto sabana = hemos metido gol habiendo una sabana
        _missionStats[ "shooterWinsSheet" ].Update( ( result.Result == Result.Target && FieldControl.instance.HasSheet ) );

        // En result.EffectBonusPoints se guardan los puntos obtenidos por bonus de efecto
        _missionStats[ "effectBonusGeneric" ].Update( ( result.EffectBonusPoints != (int)ScoreManager.EffectBonus.NONE ) );
        _missionStats[ "effectBonusLow" ].Update( ( result.EffectBonusPoints == (int)ScoreManager.EffectBonus.LOW ) );
        _missionStats[ "effectBonusMedium" ].Update( ( result.EffectBonusPoints == (int)ScoreManager.EffectBonus.MEDIUM ) );
        _missionStats[ "effectBonusHigh" ].Update( ( result.EffectBonusPoints == (int)ScoreManager.EffectBonus.HIGH ) );

        // Portero

        // acierto = parado, despejado o el lanzador tiro fuera
        _missionStats[ "goalkeeperWinsGeneric" ].Update( ( result.Result == Result.Saved || result.Result == Result.Stopped || result.Result == Result.OutOfBounds ) );
        // parado
        _missionStats[ "goalkeeperCatched" ].Update( ( result.Result == Result.Saved ) );
        // despejado
        _missionStats[ "goalkeeperSaved" ].Update( ( result.Result == Result.Stopped ) );
        // recibido gol
        _missionStats[ "goalkeeperReceivedGoals" ].Update( ( result.Result == Result.Goal ) );
    }

    public MissionStat GetStat (string statName) {
        return ( _missionStats[ statName ] );
    }

    #region Singleton

    public MissionStats () {
        //ServiceLocator.Request<IShotResultService>().RegisterListener( OnShotResult );
    }

    public static MissionStats Instance;

    #endregion

    #region IDisposable

    public void Dispose () {
        //ServiceLocator.Request<IShotResultService>().UnregisterListener( OnShotResult );
    }

    #endregion
}