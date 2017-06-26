//#define DEBUG_MULTI

using UnityEngine;
using System.Collections;

public class ifcMainMenu : ifcBase {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static ifcMainMenu instance { get; protected set; }

    public Texture2D[] Escudos2D;

    // variables para indicar que hay que pintar una determinada interfaz
    private static bool m_showCarrera = false;
    private static bool m_showDuelo = false;

    // elementos graficos de esta interfaz
    private btnButton m_btnOpciones;
    private btnButton m_btnVelo;

	// añado la funcionalidad del nuevo boton DueloPLay
	private btnButton m_btnDueloPlay;

	// añado funcionalidad de Logo Liga
	private GUITexture ImagenLiga;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------

    void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
        ifcBase.activeIface = this;

        GameplayService.networked = false;

        /*
        // boton para Facebook 
        transform.FindChild("btnFacebook").GetComponent<btnButton>().action = (_name) => {
            Debug.Log(">>> Has pulsado en el boton de Facebook");
        };
         */

        // botones duelo y carrera
        transform.FindChild("botones/btnCarrera").GetComponent<btnButton>().action = OnCarrera;
        transform.FindChild("botones/btnDuelo").GetComponent<btnButton>().action = OnDuelo;

		// Textura del Logo Liga
		ImagenLiga = transform.FindChild("logo").GetComponent<GUITexture>();
        ActualizarLogoLiga();

		// nuevo boton DueloPlay
		m_btnDueloPlay = transform.FindChild("botones/btnDueloPLAY").GetComponent<btnButton>();

		m_btnDueloPlay.action = (_name) => {

			// Ocultar mainmenu y BottomBar
			new SuperTweener.move (ifcMainMenu.instance.gameObject, 0.25f, new Vector3 (1.5f, 0f, 0.0f), SuperTweener.CubicOut, (_target) => {
			});
			new SuperTweener.move (ifcBottomBar.instance.gameObject, 0.25f, new Vector3 (0f, -0.2f, 0.0f), SuperTweener.CubicOut, (_target) => {
			});

			GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.confirmClip);
			Interfaz.ClickFX();

			// ocultar la barra superior
			//cntBarraSuperior.instance.SetVisible(false);
			Interfaz.instance.getServerIP(Stats.tipo, Stats.zona);
			ifcDialogBox.instance.ShowZeroButtonDialog(
				LocalizacionManager.instance.GetTexto(96).ToUpper(),
				LocalizacionManager.instance.GetTexto(97));
			//ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.NONE, "Conectando...", "Por favor, espere.");
		};

        // boton para mostrar las opciones
        m_btnOpciones = transform.FindChild("btnOpciones").GetComponent<btnButton>();
        m_btnOpciones.action = (_name) => {
            Interfaz.ClickFX();

            // mostrar el menu de opciones y el velo
            m_btnOpciones.gameObject.SetActive(false);
            cntMenuDesplegableOpciones.instance.Desplegar();
            m_btnVelo.gameObject.SetActive(true);
        };

        // velo (que se muestra cuando aparecen las opciones y que funciona como boton de plegar)
        m_btnVelo = transform.FindChild("btnVelo").GetComponent<btnButton>();
        m_btnVelo.action = (_name) => {
            Interfaz.ClickFX();

            // ocultar el menu de opciones y el velo
            cntMenuDesplegableOpciones.instance.Plegar();
            m_btnVelo.gameObject.SetActive(false);
        };
        m_btnVelo.gameObject.SetActive(false);  // <= ocultar por defecto el velo

        // esto no es hack, el otro jugador (IA o persona) tiene siempre el inventario falso
        PowerupService.rivalInventory = new PowerupInventory(true);
    }


    void Update()
    {
        if(Input.GetKeyUp("escape"))
        {
            if(ifcBase.activeIface == this)
            {
                Application.Quit();
            }
            else
            {
                ifcBase.activeIface.m_backMethod("");
            }
        }

        if(Input.GetKeyUp("+")) {
            int modOpponent = Cheats.Instance != null ? Cheats.Instance.OpponentELOMod : 0;
            Interfaz.MatchResult(Interfaz.SkillLevel, 1, Interfaz.SkillLevel+modOpponent, 0);
            stadium_control.instance.RefreshScenario();
            ActualizarLogoLiga();
            cntBarraSuperior.instance.ActualizaSkillLevel();
        }
        if(Input.GetKeyUp("-")) {
            int modOpponent = Cheats.Instance != null ? Cheats.Instance.OpponentELOMod : 0;
            Interfaz.MatchResult(Interfaz.SkillLevel, 0, Interfaz.SkillLevel+modOpponent, 1);
            stadium_control.instance.RefreshScenario();
            ActualizarLogoLiga();
            cntBarraSuperior.instance.ActualizaSkillLevel();
        }
    }


    private void OnDuelo (string _name) {
        ifcBase.activeIface = ifcVestuario.instance;
        Interfaz.ClickFX();

        ifcVestuario.instance.SetPantallaBack(this);
        
        // mostrar por defecto el vestuario en modo lanzador
        ifcVestuario.instance.SetVisible(true);
        Interfaz.instance.RefrescarModelosJugadores(true, false);
        ifcVestuario.instance.ShowAs(ifcVestuario.instance.m_tipoVestuario);

        // ocultar esta interfaz y mostrar el vestuario
        new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
        new SuperTweener.move(ifcVestuario.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut, (_target) => { });

        // establecer el valor de recompena acumulada en la partida a "0"
        Interfaz.recompensaAcumulada = 0;

        Interfaz.instance.CleanPlayers();
    }


    private void OnCarrera (string _name) {
        ifcCarrera.instance.SetVisible(true);
        ifcBase.activeIface = ifcCarrera.instance;
        Interfaz.ClickFX();
        Interfaz.instance.CleanPlayers();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
        new SuperTweener.move( ifcCarrera.instance.gameObject, 0.25f, new Vector3( 0.0f, 0.0f, 0.0f ), SuperTweener.CubicOut, (_target) => { } );

        // hacer que se muestre la ultima mision desbloqueada del modo carrera
        ifcCarrera.instance.MostrarUltimaMision();

        // establecer el valor de recompena acumulada en la partida a "0"
        Interfaz.recompensaAcumulada = 0;
    }


    /// <summary>
    /// Muestra / oculta el boton de opciones en funcion del valor del parametro "_visible"
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisibleBotonOpciones(bool _visible) {
        m_btnOpciones.gameObject.SetActive(_visible);
        m_btnVelo.gameObject.SetActive(!_visible);
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


    /// <summary>
    /// Oculta los elementos de interfaz que no deberian estar visibles (para optimizar el rendimiento de la aplicacion)
    /// </summary>
    public void OcultarElementosDeInterfazNoVisibles() {
        // pantallas
        if (ifcPerfil.instance != null)
            ifcPerfil.instance.SetVisible(false);
        if (ifcLogros.instance != null)
            ifcLogros.instance.SetVisible(false);
        if (ifcVestuario.instance != null)
            ifcVestuario.instance.SetVisible(false);
        if (ifcCarrera.instance != null)
            ifcCarrera.instance.SetVisible(m_showCarrera);
        if (ifcDuelo.instance != null)
            ifcDuelo.instance.SetVisible(m_showDuelo);

        // controles
        if (cntMenuDesplegableOpciones.instance != null)
            cntMenuDesplegableOpciones.instance.SetVisible(false);

        // comprobar si hay que ocultar esta pantalla
        if (m_showCarrera || m_showDuelo) {
            if (m_showCarrera) {
                ifcBase.activeIface = ifcCarrera.instance;
                m_showCarrera = false;
            }
            if (m_showDuelo) {
                ifcBase.activeIface = ifcDuelo.instance;
                m_showDuelo = false;
                cntBarraSuperior.instance.SetVisible(false);
            }

            SetVisible(false);
        } else {
            // indicar que esta es la pantalla activa y mostrarla
            ifcBase.activeIface = ifcMainMenu.instance;
            SetVisible(true);
        }
    }

	// metodo para actualizar el logo de la liga
    public void ActualizarLogoLiga() {
        int escudo = PlayerPrefs.GetInt("estadio", 0);
        ImagenLiga.texture = Escudos2D[escudo];
	}

}
