using UnityEngine;
using System.Collections;

/// <summary>
/// Pantalla de game over para single player
/// </summary>
public class ifcGameOverSingle : ifcBase {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static ifcGameOverSingle instance { get; protected set; }

    // texturas para utilizar de fondo
    // NOTA: asignar valores a estas texturas desde la interfaz de Unity
    public Texture m_texturaFondoVictoria;
    public Texture m_texturaFondoDerrota;

    // elementos de esta interfaz
    private GUITexture m_fondo;

    protected GUIText m_goTitleLabel;
    protected GUIText m_goTitleLabelShadow;
    protected GUIText m_txtMision;
    protected GUIText m_txtMisionSombra;

    protected cntGameOverScores m_goScores;
    protected cntGameOverScoreOverall m_goScoreOverall;
    protected cntGameOverAchievements m_goAchievements;

    protected btnButton m_goRepeatGame;
    protected btnButton m_goContinue;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake () {
        instance = this;

        // Cachear todos los controles
        m_fondo = transform.FindChild("Fondo").GetComponent<GUITexture>();
        m_goTitleLabelShadow = transform.FindChild("TituloSombra").GetComponent<GUIText>();
        m_txtMision = transform.FindChild("Mision").GetComponent<GUIText>();
        m_txtMisionSombra = transform.FindChild("MisionSombra").GetComponent<GUIText>();

        Transform t = transform.Find( "Titulo" );
        if ( t != null ) {
            m_goTitleLabel = t.gameObject.GetComponent<GUIText>();
        }
        t = transform.Find( "Puntuaciones" );
        if ( t != null ) {
            m_goScores = t.gameObject.GetComponent<cntGameOverScores>();
        }
        t = transform.Find( "Totales" );
        if ( t != null ) {
            m_goScoreOverall = t.gameObject.GetComponent<cntGameOverScoreOverall>();
        }
        t = transform.Find( "ObjetivosFase" );
        if ( t != null ) {
            m_goAchievements = t.gameObject.GetComponent<cntGameOverAchievements>();
        }

        t = transform.Find( "btnRepetir" );
        if ( t != null ) {
            m_goRepeatGame = t.gameObject.GetComponent<btnButton>();
            m_goRepeatGame.action = (_name) => {
                GeneralSounds.instance.click();
                EscudosManager.instance.ComprobarEscudosConsumidos();

                if(EscudosManager.escudoEquipado != EscudosManager.instance.escudoPorDefecto)
                {
                    ifcDialogBox.instance.ShowTwoButtonDialog(
                        ifcDialogBox.TwoButtonType.POSITIVE_NEGATIVE,
                        LocalizacionManager.instance.GetTexto(216).ToUpper(),
                        string.Format(LocalizacionManager.instance.GetTexto(217), "<color=#ddf108>" + EscudosManager.escudoEquipado.nombre + "</color>", "<color=#ddf108>" + EscudosManager.escudoEquipado.numUnidades + "</color>"),
                        LocalizacionManager.instance.GetTexto(281).ToUpper(),
						LocalizacionManager.instance.GetTexto(282).ToUpper(),
                        // callback si el usuario acepta
                        (_name1) => { Cortinilla.instance.Return( false ); },
                        // callback si el usuario cancela
                    (_name1) => { EscudosManager.escudoEquipado = EscudosManager.instance.escudoPorDefecto; Cortinilla.instance.Return( false ); },
                    true);
                }
                else
                {
                    Cortinilla.instance.Return( false );
                }
                
            };
        }
        t = transform.Find( "btnContinuar" );
        if ( t != null ) {
            m_goContinue = t.gameObject.GetComponent<btnButton>();
            m_goContinue.action = (_name) => {
                GeneralSounds.instance.click();
                ifcMainMenu.goCarrera = true;
                FieldControl.instance.goToMenu();
                EscudosManager.instance.ComprobarEscudosConsumidos();
            };
        }
    }


    public void GetReferencias() {

    }


    public void RefreshData () {

        if (IsMissionSuccessful()) {
            SetMissionSuccesful(true); // victoria
            GeneralSounds.instance.victoria();
            m_fondo.texture = m_texturaFondoVictoria;
        } else {
            SetMissionSuccesful(false); // derrota            
            GeneralSounds.instance.derrota();
            m_fondo.texture = m_texturaFondoDerrota;
        }
        SetMissionScores( ServiceLocator.Request<IGameplayService>().GetGameMode() ); // Puntuaciones
        
        // texto con el numero de mision
        int numMision = MissionManager.instance.GetMission().indexMision + 1;

        m_txtMision.text = LocalizacionManager.instance.GetTexto(11).ToUpper() + " " + numMision;
        m_txtMisionSombra.text = m_txtMision.text;

        // Puntuacion total y recompensa
        SetMissionScoreOverall(
            ( (int)ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Points ),
            ( (int)Interfaz.recompensaAcumulada ) );
        
        SetMissionAchievements(); // Objetivos conseguidos
    }

    bool IsMissionSuccessful () {
        // La condicion para que la mision se finalice con exito es:
        // - Haber completado todas las rondas
        // - Tener al menos una vida
        if ( ( MissionManager.instance.GetMission().IsMissionFinished() ) &&
             ( ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts > 0 ) ) {
            return true;
        }        
        return false;
    }

    void SetMissionSuccesful (bool bSuccess) {
        m_goTitleLabel.text = (bSuccess) ? LocalizacionManager.instance.GetTexto(115).ToUpper() : LocalizacionManager.instance.GetTexto(116).ToUpper();
        m_goTitleLabelShadow.text = m_goTitleLabel.text;
        if (bSuccess)
            m_goTitleLabel.color = new Color(221.0f / 255.0f, 241.0f / 255.0f, 8.0f / 255.0f); // VERDE
        else
            m_goTitleLabel.color = new Color(184.0f / 255.0f, 30.0f / 255.0f, 70.0f / 255.0f); // ROJO
    }

    void SetMissionScores (GameMode gameMode) {
        m_goScores.ResetIndicators();

        switch ( gameMode ) {
            case GameMode.GoalKeeper: {
                    string winsValue = MissionStats.Instance.GetStat( "goalkeeperWinsGeneric" ).GetTotal().ToString() +
                                       " / " +
                                       MissionManager.instance.GetMission().RoundsCount.ToString();
                    m_goScores.SetIndicator( 1, LocalizacionManager.instance.GetTexto(33).ToUpper(), winsValue );
                    m_goScores.SetIndicator( 2, LocalizacionManager.instance.GetTexto(77).ToUpper(), MissionStats.Instance.GetStat( "perfects" ).GetTotal().ToString() );
                }
                break;
            case GameMode.Shooter: {
                    string winsValue = MissionStats.Instance.GetStat( "shooterWinsGeneric" ).GetTotal().ToString() +
                                       " / " +
                                       MissionManager.instance.GetMission().RoundsCount.ToString();
                    m_goScores.SetIndicator( 1, LocalizacionManager.instance.GetTexto(33).ToUpper(), winsValue );
                    m_goScores.SetIndicator( 2, LocalizacionManager.instance.GetTexto(77).ToUpper(), MissionStats.Instance.GetStat( "perfects" ).GetTotal().ToString() );
                    m_goScores.SetIndicator( 3, LocalizacionManager.instance.GetTexto(253).ToUpper(), MissionStats.Instance.GetStat( "effectBonusGeneric" ).GetTotal().ToString() );
                }
                break;
        }
    }

    void SetMissionScoreOverall (int points, int coinReward) {
        m_goScoreOverall.SetPointsAndReward( points, coinReward );
    }

    void SetMissionAchievements () {
        for(int i = 0 ; i < MissionManager.instance.GetMission().Achievements.Count ; i++)
        {
            bool retoEstaPartida = MissionManager.instance.GetMission().Achievements[i].IsAchieved();
            bool retoCargado = GameplayService.gameLevelMission.GetAchievements()[i].IsAchieved();
            string descripcion = MissionManager.instance.GetMission().Achievements[i].DescriptionID;
            m_goAchievements.SetGameOverAchievement( i,
                                                  descripcion,
                                                  retoCargado,
                                                  retoEstaPartida,
                                                  ServiceLocator.Request<IPlayerService>().GetRecompensas()[i] );
        }
        /*// TODO: falta la parte de persistencia

        int idx = 1;
        foreach ( var achievement in MissionManager.instance.GetMission().Achievements ) {
            Debug.LogWarning(">>> El objetivo:" + achievement.Code + " esta conseguido:" + achievement.IsAchieved());

            _goAchievements.SetGameOverAchievement( idx,
                achievement.DescriptionID,
                achievement.IsAchieved() );
            idx++;
        }*/
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


}
