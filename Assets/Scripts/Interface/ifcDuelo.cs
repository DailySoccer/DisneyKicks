using UnityEngine;
using System.Collections;
using System.Linq;


/// <summary>
/// Clase para mostrar la interfaz de duelo. Admite dos estados:
/// ShowRivales(): muestra los posibles rivales para un duelo
/// ShowVs(): muestra la pantalla de versus
/// Nota: los popUps que se muestran sobre esta pantalla se gestionan desde los controles "cntInfoJugadorDuelo" de esta interfaz
/// </summary>
public class ifcDuelo : ifcBase {


    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Numero de jugadores rivales a mostrar en esta pantalla
    /// </summary>
    private const int NUM_JUGADORES_RIVALES = 8;
    private const int NUM_JUGADORES_PAGINA = 4;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static ifcDuelo instance { 
        get {
            if (m_instance == null) {
                Transform tr = Interfaz.instance.transform.FindChild("Duelo");
                if (tr != null) {
                    m_instance = tr.GetComponent<ifcDuelo>();
                    m_instance.Start();
                }
            }

            return m_instance; 
        } 
    }
    private static ifcDuelo m_instance;

    // variables para la instanciacion de prefabs (inicializarlas en la interfaz de Unity)
    public GameObject prefInfoJugadorDuelo;      // <= prefab para mostrar la informacion de los jugadores disponibles para el duelo

    // controles para mostrar los jugadores disponibles para el duelo
    private cntInfoJugadorDuelo[] m_cntInfoJugadoresDuelo;

    // otros elementos de interfaz
    private GameObject m_imagenVs;
	private GameObject PantallaBuscandoRival;
	private GameObject OBJBarraBuscandoRival;
    private GUITexture m_barraProgreso;
    private GUITexture m_barraProgresoFondo;
	private GUITexture m_barraProgresoBuscandoRival;
	private GUITexture m_barraProgresoFondoBuscandoRival;


    // game objects para mostrar los jugadores secundarios
    private GameObject m_jugadorSecundarioLocal;
    private GameObject m_jugadorSecundarioRemoto;

    // variables para controlar el tiempo transcurrido en esta pantalla en modo VS (para saber cuando saltar a la siguiente)
    private bool m_modoVs = false;
    private float m_tiempoTranscurridoEnPantallaVs = 0.0f;

	// variables para controlar el tiempo transcurrido en esta pantalla en modo BUSCANDO RIVAL (para saber cuando saltar a la siguiente)
	private float m_tiempoTranscurridoEnPantallaBuscandoRival = 0.0f;

    // holder para guardar el rival del (posible) duelo
    public static Usuario m_rival;

    //indice de paginacion del lobby
    private int lobbyIndex = 0;
    private Usuario[] lastLobby;

	//booleana para tiempo de espera en Buscar Rivales
	private bool m_duelAccepted;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    // Use this for initialization
    void Start () {
        // obtener las referencias a algunos elementos de interfaz
        //m_btnAtras = transform.FindChild("btnAtras").gameObject;

		//interfaz BuscandoRival
		PantallaBuscandoRival = transform.FindChild ("PantallaBuscandoRival").gameObject;
		OBJBarraBuscandoRival = transform.FindChild ("BarraBuscandoRival").gameObject;
		m_barraProgresoBuscandoRival = transform.FindChild("BarraBuscandoRival/barraProgreso").GetComponent<GUITexture>();
		m_barraProgresoFondoBuscandoRival = transform.FindChild("BarraBuscandoRival/barraProgresoFake").GetComponent<GUITexture>();
		m_tiempoTranscurridoEnPantallaBuscandoRival = 0.0f;
		
		//interfaz VS
        m_imagenVs = transform.FindChild("vs").gameObject;
        m_barraProgreso = transform.FindChild("vs/barraProgreso").GetComponent<GUITexture>();
        m_barraProgresoFondo = transform.FindChild("vs/barraProgresoFake").GetComponent<GUITexture>();

        this.m_backMethod = Back;

        // crear los controles para mostrar la informacion de los jugadores disponibles para el duelo
        if (m_cntInfoJugadoresDuelo == null) {
            m_cntInfoJugadoresDuelo = new cntInfoJugadorDuelo[NUM_JUGADORES_PAGINA];
            GameObject go;
            for (int i = 0; i < m_cntInfoJugadoresDuelo.Length; ++i) {
                go = (GameObject) GameObject.Instantiate(prefInfoJugadorDuelo);
                m_cntInfoJugadoresDuelo[i] = go.GetComponent<cntInfoJugadorDuelo>();
                m_cntInfoJugadoresDuelo[i].Inicializar(this.transform, "infoJugadorDuelo" + i, new Vector3(0.14f + (i * 0.24f), 0.185f, 0.0f));
            }
        }
			
        ShowConectando();
  }

    /// <summary>
    /// Mostrar la pantalla de "Conectando"
    /// </summary>
    public void ShowConectando() {

        // indicar que la pantalla no esta en modo VS
        m_modoVs = false;

        m_duelAccepted = false;

        // mostrar la informacion de todos los jugadores rivales
        for (int i = 0; i < m_cntInfoJugadoresDuelo.Length; ++i) {
            m_cntInfoJugadoresDuelo[i].SetVisible(false);
        }

        // ocultar la imagen de vs
        m_imagenVs.SetActive(false);
    }


    /// <summary>
    /// Mostrar la pantalla de duelo en modo "Mostrar Rivales"
    /// </summary>
    public void AsignarRival(Usuario[] _clientes) {
		
        cntBarraSuperior.instance.SetVisible(false);

        lastLobby = _clientes;

		//TODO: coger el primer usuario humano de la lista y 
		// m_rival = _clientes [0];
		m_rival = _clientes.First( cliente => cliente.yoRobot );

        // indicar que la pantalla no esta en modo VS
        m_modoVs = false;

        // ocultar la imagen de vs
        m_imagenVs.SetActive(false);
		m_tiempoTranscurridoEnPantallaBuscandoRival = 0.0f;
		MensajeBase msg = Shark.instance.mensaje<MsgRequestDuel>();
		(msg as MsgRequestDuel).m_challenge = m_rival.alias;
		(msg as MsgRequestDuel).m_uid = m_rival.uid;
		msg.send();
     }

	public void DuelAccepted(Usuario _usuarioRival) {
		
		m_duelAccepted = true;
		OBJBarraBuscandoRival.SetActive (true);

	}


    /// <summary>
    /// Mostrar la pantalla de duelo en modo "VS"
    /// </summary>
    /// <param name="_usuarioRival"></param>
    public void ShowVs(Usuario _usuarioRival) {

        // ocultar la informacion de los jugadores (salvo la del primero y el ultimo)
        for (int i = 1; i < m_cntInfoJugadoresDuelo.Length - 1; ++i) {
            m_cntInfoJugadoresDuelo[i].SetVisible(false);
        }

        // mostrar la info del jugador local y del rival
        Usuario me = new Usuario(Interfaz.m_uname, 0, 0, "");

        //me.alias += " (TU)";

        me.initMode = !m_rival.initMode;
        me.charGoalkeeper = Interfaz.instance.m_goalKeeper;
        me.charThrower = Interfaz.instance.m_thrower;
        me.equipacionShooter = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada();
        me.equipacionGoalkeeper = EquipacionManager.instance.GetEquipacionPorteroSeleccionada();

        m_cntInfoJugadoresDuelo[0].AsignarValores(me, false, true);
        m_cntInfoJugadoresDuelo[m_cntInfoJugadoresDuelo.Length - 1].AsignarValores(_usuarioRival, false);

        // mostrar la imagen de vs
        m_imagenVs.SetActive(true);

        // indicar que la pantalla esta en modo VS
        m_modoVs = true;
        m_tiempoTranscurridoEnPantallaVs = 0.0f;

        // mostrar el jugador secundario del usuario local
        if (m_jugadorSecundarioLocal != null)
            GameObject.Destroy(m_jugadorSecundarioLocal);

        Vector3 posicionJugadorSecundarioLocal = m_cntInfoJugadoresDuelo[0].transform.position;
        posicionJugadorSecundarioLocal.x += 0.14f;
        posicionJugadorSecundarioLocal.y += 0.065f;
        Equipacion equipacionJugadorLocal = ((me.initMode)? (EquipacionManager.instance.GetEquipacionLanzadorSeleccionada()) : (EquipacionManager.instance.GetEquipacionPorteroSeleccionada()));
        m_jugadorSecundarioLocal = Interfaz.instance.InstantiatePlayerAtScreenRelative(posicionJugadorSecundarioLocal, !me.initMode, me.secondaryCharacter.idModelo, equipacionJugadorLocal);

        // mostrar el jugador secundario del usuario remoto
        if (m_jugadorSecundarioRemoto != null)
            GameObject.Destroy(m_jugadorSecundarioRemoto);

        Vector3 posicionJugadorSecundarioRemoto = m_cntInfoJugadoresDuelo[m_cntInfoJugadoresDuelo.Length - 1].transform.position;
        posicionJugadorSecundarioRemoto.x -= 0.14f;
		posicionJugadorSecundarioRemoto.y += 0.065f;
        m_jugadorSecundarioRemoto = Interfaz.instance.InstantiatePlayerAtScreenRelative(posicionJugadorSecundarioRemoto, !_usuarioRival.initMode, _usuarioRival.secondaryCharacter.idModelo, (_usuarioRival.initMode)?_usuarioRival.equipacionShooter:_usuarioRival.equipacionGoalkeeper);
    }

    void Update() {
		if (!m_modoVs) {
			m_tiempoTranscurridoEnPantallaBuscandoRival = Mathf.Min(m_tiempoTranscurridoEnPantallaBuscandoRival + Time.deltaTime, Stats.TIEMPO_ESPERA_MOSTRAR_PANTALLA_BUSCANDORIVAL);

			// actualizar la barra de progreso
			m_barraProgresoBuscandoRival.pixelInset = new Rect(
				m_barraProgresoFondoBuscandoRival.pixelInset.xMin,
				m_barraProgresoFondoBuscandoRival.pixelInset.yMin,
				m_barraProgresoFondoBuscandoRival.pixelInset.width * (m_tiempoTranscurridoEnPantallaBuscandoRival / Stats.TIEMPO_ESPERA_MOSTRAR_PANTALLA_BUSCANDORIVAL),
				m_barraProgresoFondoBuscandoRival.pixelInset.height);

			// comprobar si ya se ha superado el tiempo de espera y pasar a la siguiente pantalla
			if (m_tiempoTranscurridoEnPantallaBuscandoRival >= Stats.TIEMPO_ESPERA_MOSTRAR_PANTALLA_BUSCANDORIVAL && m_duelAccepted) {
				Debug.Assert (m_duelAccepted, "m_duelAccepted == false!!!");
				ShowVs (m_rival);
			}
		}
        // si la pantalla esta en modo VS
        else if (m_modoVs) {
			// ocultar la pantalla buscando rival
			PantallaBuscandoRival.SetActive (false);
			OBJBarraBuscandoRival.SetActive (false);
            m_tiempoTranscurridoEnPantallaVs = Mathf.Min(m_tiempoTranscurridoEnPantallaVs + Time.deltaTime, Stats.TIEMPO_ESPERA_MOSTRAR_PANTALLA_VS);

            // actualizar la barra de progreso
            m_barraProgreso.pixelInset = new Rect(
                m_barraProgresoFondo.pixelInset.xMin,
                m_barraProgresoFondo.pixelInset.yMin,
                m_barraProgresoFondo.pixelInset.width * (m_tiempoTranscurridoEnPantallaVs / Stats.TIEMPO_ESPERA_MOSTRAR_PANTALLA_VS),
                m_barraProgresoFondo.pixelInset.height);

            // comprobar si ya se ha superado el tiempo de espera y pasar a la siguiente pantalla
            if (m_tiempoTranscurridoEnPantallaVs >= Stats.TIEMPO_ESPERA_MOSTRAR_PANTALLA_VS) {
                Cortinilla.instance.Play();
                m_modoVs = false;
            }
        }
    }


    void Back(string _name = "") {

		//ADRIAN metodo para volver atrás desde el menú Duelo, comento lo que había y re-aprovecho lo que me interesa
//        GeneralSounds_menu.instance.back();
//
//
//        Shark.instance.Desconectar();
//
//        // mostrar de nuevo la barra de opciones
//        cntBarraSuperior.instance.SetVisible(true);
//
//        ifcBase.activeIface = ifcVestuario.instance;
//
//        GameplayService.networked = false;
//
//        //revelar jugadores
//        //Interfaz.instance.goalkeeperModel.SetActive(true);
//        //Interfaz.instance.throwerModel.SetActive(true);
//        Interfaz.instance.Thrower = Interfaz.instance.Thrower;
//        Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
//        
//        foreach(cntInfoJugadorDuelo jugador in m_cntInfoJugadoresDuelo)
//        {
//            jugador.SetVisible(false);
//        }
//
//        ifcVestuario.instance.SetVisible(true);
//        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
//        new SuperTweener.move(ifcVestuario.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut);

		//ADRIAN esto es lo que me interesa
		ifcBottomBar.instance.NumPantalla = 2;
		ifcBottomBar.instance.MostrarEscenaNueva ();
		new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
		new SuperTweener.move(ifcBottomBar.instance.gameObject, 0.25f, new Vector3(0.5f, 0f, 0.0f), SuperTweener.CubicOut);
		GeneralSounds_menu.instance.back();
		Shark.instance.Desconectar();
		// mostrar de nuevo la barra de opciones
		cntBarraSuperior.instance.SetVisible(true);
		GameplayService.networked = false;
		Interfaz.instance.Thrower = Interfaz.instance.Thrower;
		Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
		foreach(cntInfoJugadorDuelo jugador in m_cntInfoJugadoresDuelo)
		{
			jugador.SetVisible(false);
		}
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


}
