using UnityEngine;
using System.Collections;

public class ifcPausa : ifcBase
{

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public static ifcPausa instance { get; protected set; }

    bool m_pauseEnabled = true;
    public bool PauseEnabled {
      get{return m_pauseEnabled;}
      set{m_pauseEnabled = value; ifcThrower.instance.SetPauseEnabled(value);}
    }

    float m_realTimeScale;

    // elementos de esta interfaz
    private cntGameOverAchievements m_achievements;
    private btnButton m_btnFx;
    private btnButton m_btnMusica;
    private btnButton m_btnSalir;
    private btnButton m_btnRepetir;
    private btnButton m_btnPlay;
    private btnButton m_btnContinuar;
    private btnButton m_btnVelo;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake()
    {
        instance = this;
        m_realTimeScale = Time.timeScale;
    }


    /// <summary>
    /// Obtener la referencia a los elementos de la interfaz
    /// </summary>
    private void GetReferencias() {
        if (m_achievements == null)
            m_achievements = transform.Find("ObjetivosFase").gameObject.GetComponent<cntGameOverAchievements>();

        // boton efectos
        if (m_btnFx == null) {
            m_btnFx = transform.FindChild("btnEfectos").GetComponent<btnButton>();
            m_btnFx.Toggle = true;
            m_btnFx.action = (_name) => {
                ifcOpciones.fx = !ifcOpciones.fx;
                GeneralSounds.instance.click();
                PlayerPrefs.SetInt("fx", ifcOpciones.fx ? 1 : 0);
                RefrescarBotonesAudio();
                //setFX();
            };
        }

        // boton musica
        if (m_btnMusica == null) {
            m_btnMusica = transform.FindChild("btnMusica").GetComponent<btnButton>();
            m_btnMusica.Toggle = true;
            m_btnMusica.action = (_name) => {
                ifcOpciones.music = !ifcOpciones.music;
                GeneralSounds.instance.click();
                PlayerPrefs.SetInt("music", ifcOpciones.music ? 1 : 0);
                RefrescarBotonesAudio();
                //setMusic();
            };
        }

        // boton salir
        if (m_btnSalir == null) {
            m_backMethod = Back;
            m_btnSalir = getComponentByName("btnSalir").GetComponent<btnButton>();
            m_btnSalir.action = (_name) => {
				getComponentByName("btnSalir").layer = 2;
                GeneralSounds_menu.instance.back();
                Time.timeScale = m_realTimeScale;
                PersistenciaManager.instance.GuardarPartidaSinglePlayer();
                FieldControl.instance.goToMenu();
                EscudosManager.instance.ComprobarEscudosConsumidos();
            };
        }

        // boton repetir
        if (m_btnRepetir == null) {
            m_btnRepetir = getComponentByName("btnRepetir").GetComponent<btnButton>();
            m_btnRepetir.action = (_name) => {
				getComponentByName("btnRepetir").layer = 2;
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.confirmClip);

                PersistenciaManager.instance.GuardarPartidaSinglePlayer();
                Interfaz.recompensaAcumulada = 0;
                GeneralSounds.instance.click();
                EscudosManager.instance.ComprobarEscudosConsumidos();
                //Cortinilla.instance.Return(false);
                GeneralSounds.instance.CleanAudioFade();

                if (EscudosManager.escudoEquipado != EscudosManager.instance.escudoPorDefecto) {
                    ifcDialogBox.instance.ShowTwoButtonDialog(
                        ifcDialogBox.TwoButtonType.POSITIVE_NEGATIVE,
                        LocalizacionManager.instance.GetTexto(216).ToUpper(),
                        string.Format(LocalizacionManager.instance.GetTexto(217), "<color=#ddf108>" + EscudosManager.escudoEquipado.nombre + "</color>", "<color=#ddf108>" + EscudosManager.escudoEquipado.numUnidades + "</color>"),
                        LocalizacionManager.instance.GetTexto(281),
                        LocalizacionManager.instance.GetTexto(282),
                        // callback si el usuario acepta
                        (_name1) => { Cortinilla.instance.Return(false); Time.timeScale = m_realTimeScale;},
                        // callback si el usuario cancela
                    (_name1) => { EscudosManager.escudoEquipado = EscudosManager.instance.escudoPorDefecto; Cortinilla.instance.Return(false); Time.timeScale = m_realTimeScale;},
                    true);
                } else {
                    Time.timeScale = m_realTimeScale;
                    Cortinilla.instance.Return(false);
                }
            };
        }

        if (m_btnPlay == null) {
            m_btnPlay = transform.FindChild("btnPlay").GetComponent<btnButton>();
            m_btnPlay.action = Back;
        }
        if (m_btnContinuar == null) {
            m_btnContinuar = transform.FindChild("btnContinuar").GetComponent<btnButton>();
            m_btnContinuar.action = Back;
        }
        if (m_btnVelo == null) {
            m_btnVelo = transform.FindChild("btnVelo").GetComponent<btnButton>();
            m_btnVelo.action = Back;
        }
    }


    // Use this for initialization
    void Start()
    {
        // Obtener la referencia a los elementos de la interfaz
        GetReferencias();

        if(GameplayService.networked)
        {
            m_achievements.gameObject.SetActive(false);
        }

        if (!PlayerPrefs.HasKey("calidad"))
        {
            ifcOpciones.calidad = 1;
            PlayerPrefs.SetInt("calidad", ifcOpciones.calidad);
        }
        else
        {
            ifcOpciones.calidad = PlayerPrefs.GetInt("calidad");
        }

        if (!PlayerPrefs.HasKey("fx"))
        {
            ifcOpciones.fx = true;
            PlayerPrefs.SetInt("fx", ifcOpciones.fx ? 1 : 0);
        }
        else
        {
            ifcOpciones.fx = PlayerPrefs.GetInt("fx") == 1;
        }
        if (!PlayerPrefs.HasKey("music"))
        {
            ifcOpciones.music = true;
            PlayerPrefs.SetInt("music", ifcOpciones.music ? 1 : 0);            
        }
        else
        {
            ifcOpciones.music = PlayerPrefs.GetInt("music") == 1;
        }

        RefrescarBotonesAudio();
        //setFX();
        //setMusic();
        //setCalidad();
    }

    void Back(string _target = "")
    {
        GeneralSounds.instance.continuar();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(-2.0f, 0.5f, 0.0f), SuperTweener.CubicOut, (obj) => { Time.timeScale = m_realTimeScale; SetVisible(false); });
        new SuperTweener.move(transform.parent.GetComponent<ifcBase>().getComponentByName("btnPausa"), 0.25f, new Vector3(0.0f, 1.0f, 0.0f), SuperTweener.CubicOut);
        ifcBase.activeIface = ifcThrower.instance;
        InputManager.instance.Blocked = false;
        GeneralSounds.instance.UnMuteVolume();
    }

    public void RefreshObjetivos()
    {
        // Obtener la referencia a los elementos de la interfaz
        GetReferencias();

        transform.Find("tituloMision").GetComponent<GUIText>().text = LocalizacionManager.instance.GetTexto(11).ToUpper() + " " + (MissionManager.instance.GetMission().indexMision + 1).ToString();
        transform.Find("tituloMision").GetChild(0).GetComponent<GUIText>().text = transform.Find("tituloMision").GetComponent<GUIText>().text;

        for(int i = 0 ; i < MissionManager.instance.GetMission().Achievements.Count ; i++)
        {
            bool retoEstaPartida = MissionManager.instance.GetMission().Achievements[i].IsAchieved();
            bool retoCargado = GameplayService.gameLevelMission.GetAchievements()[i].IsAchieved();
            string descripcion = MissionManager.instance.GetMission().Achievements[i].DescriptionID;
            m_achievements.SetGameOverAchievement( i,
                                                  descripcion,
                                                  retoCargado,
                                                  retoEstaPartida );
        }
    }


    public void RefrescarBotonesAudio() {
        // boton FX
        if (ifcOpciones.fx)
            m_btnFx.Select();
        else
            m_btnFx.Deselect();

        // boton musica
        if (ifcOpciones.music)
            m_btnMusica.Select();
        else
            m_btnMusica.Deselect();
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


}
