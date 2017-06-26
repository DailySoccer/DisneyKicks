using UnityEngine;
using System.Collections;

/// <summary>
/// Pantalla de game over para multi player
/// </summary>
public class ifcGameOverMulti : ifcBase {


    public class GameOverField {
        public GUIText Label;
        public GUIText Value;

        public GameOverField (GUIText _label, GUIText _value) {
            Label = _label;
            Value = _value;
        }

        public void SetFieldData (string fieldLabel, int fieldValue) {
            Label.text = fieldLabel;
            Value.text = fieldValue.ToString();
        }

        public void SetFieldData (string fieldLabel, string fieldValue) {
            Label.text = fieldLabel;
            Value.text = fieldValue;
        }
    }


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    // instancia de esta clase
    public static ifcGameOverMulti instance { get { return m_instance; } }
    private static ifcGameOverMulti m_instance;

    // texturas para el fondo de esta pantalla
    // Nota: asignarles valor desde la interfaz de unity
    public Texture m_texturaFondoVictoria;
    public Texture m_texturaFondoDerrota;

    // elementos de esta interfaz
    private GUITexture m_imgFondo;
    private GUIText m_goTitleLabel;
    private GUIText m_goTitleLabelSombra;
    private btnButton m_goRepeatGame;
    private btnButton m_goContinue;

    private GameOverField m_stoppedShots;
    private GameOverField m_scoredGoals;
    private GameOverField m_playerScore;
    private GameOverField m_playerReward;
    private GameOverField m_playerSkillLevel;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    
    void Awake () {
        m_instance = this;

        // Cachear todos los controles
        m_goTitleLabel = transform.FindChild("Titulo").GetComponent<GUIText>();
        m_goTitleLabelSombra = transform.FindChild("TituloSombra").GetComponent<GUIText>();

        m_stoppedShots = new GameOverField(
            transform.Find( "bloqueInfo1/balonesParados/Nombre" ).gameObject.GetComponent<GUIText>(),
            transform.Find( "bloqueInfo1/balonesParados/Puntos" ).gameObject.GetComponent<GUIText>() );
        m_scoredGoals = new GameOverField(
            transform.Find( "bloqueInfo1/golesMarcados/Nombre" ).gameObject.GetComponent<GUIText>(),
            transform.Find( "bloqueInfo1/golesMarcados/Puntos" ).gameObject.GetComponent<GUIText>() );
        m_playerScore = new GameOverField(
            transform.Find( "bloqueInfo2/puntuacion/Nombre" ).gameObject.GetComponent<GUIText>(),
            transform.Find( "bloqueInfo2/puntuacion/Valor" ).gameObject.GetComponent<GUIText>() );
        m_playerReward = new GameOverField(
            transform.Find( "bloqueInfo2/recompensa/Nombre" ).gameObject.GetComponent<GUIText>(),
            transform.Find( "bloqueInfo2/recompensa/Valor" ).gameObject.GetComponent<GUIText>() );
        
        m_playerSkillLevel = new GameOverField(
            transform.Find( "skillLevel/Nombre" ).gameObject.GetComponent<GUIText>(),
            transform.Find( "skillLevel/Valor" ).gameObject.GetComponent<GUIText>() );

        Transform t = transform.Find("btnRepetir");
        m_imgFondo = transform.FindChild("Fondo").GetComponent<GUITexture>();

        if ( t != null ) {
            m_goRepeatGame = t.gameObject.GetComponent<btnButton>();
            m_goRepeatGame.action = (_name) => {
                Debug.Log( ">>> HAS PULSADO REPETIR" );
                
                // TODO: implementar la revancha!!
            };
        }
        t = transform.Find( "btnContinuar" );
        if ( t != null ) {
            m_goContinue = t.gameObject.GetComponent<btnButton>();
            m_goContinue.action = (_name) => {
                Debug.Log( ">>> HAS PULSADO CONTINUAR" );
                // ifcMainMenu.goDuelo = true;
                GeneralSounds.instance.click();
                FieldControl.instance.goToMenu();
            };
        }
    }

    public void RefreshData () {
        bool isPlayer1 = GameplayService.initialGameMode != GameMode.Shooter;
        int localPlayerScore = isPlayer1 ? Player.serverState.score_1 : Player.serverState.score_2;
        int remotePlayerScore = isPlayer1 ? Player.serverState.score_2 : Player.serverState.score_1;

        bool HasLocalPlayerWon = ( localPlayerScore >= remotePlayerScore );

        SetDuelResult( HasLocalPlayerWon ); // victoria o derrota
       
        // TODO: sacar los balones parados y goles marcados del jugador local
        m_stoppedShots.SetFieldData( LocalizacionManager.instance.GetTexto(190).ToUpper(), FieldControl.instance.GetDuelGameGoalkeeperStopStat() );
        m_scoredGoals.SetFieldData( LocalizacionManager.instance.GetTexto(191).ToUpper(), FieldControl.instance.GetDuelGameShooterGoalStat() );
        m_playerScore.SetFieldData( LocalizacionManager.instance.GetTexto(73).ToUpper(), localPlayerScore ); // TODO: calcular la puntuación correcta
        m_playerReward.SetFieldData(LocalizacionManager.instance.GetTexto(187).ToUpper(), ((HasLocalPlayerWon) ? 500 : 250) + " ¤");
  
        // TODO: dar al jugador la recompensa que le corresponde

        // TODO: El SkillLevel del Oponente es el del propio player "modificado" (hasta que nos
        int modOpponent = Cheats.Instance != null ? Cheats.Instance.OpponentELOMod : 0;
        int modSkillPlayer = Interfaz.MatchResult(Interfaz.SkillLevel, localPlayerScore, Interfaz.SkillLevel + modOpponent, remotePlayerScore);
        m_playerSkillLevel.SetFieldData(LocalizacionManager.instance.GetTexto(295).ToUpper(), modSkillPlayer);
    }    

    void SetDuelResult (bool bVictory) {
        // actualizar el titulo de "victoria" o "derrota" y la textura de fondo
        if (bVictory) {
            m_goTitleLabel.text = LocalizacionManager.instance.GetTexto(184).ToUpper();
            m_goTitleLabel.color = new Color(221.0f / 255.0f, 241.0f / 255.0f, 8.0f / 255.0f); // VERDE

        } else {
            m_goTitleLabel.text = LocalizacionManager.instance.GetTexto(185).ToUpper();
            m_goTitleLabel.color = new Color(184.0f / 255.0f, 30.0f / 255.0f, 70.0f / 255.0f); // ROJO
        }
        m_goTitleLabelSombra.text = m_goTitleLabel.text;

        // seleccionar la textura de fondo
        m_imgFondo.texture = (bVictory) ? m_texturaFondoVictoria : m_imgFondo.texture = m_texturaFondoDerrota;
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


}
