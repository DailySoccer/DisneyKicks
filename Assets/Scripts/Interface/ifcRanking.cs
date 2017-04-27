using UnityEngine;
using System.Collections;

public class ifcRanking : ifcBase{

    // ------------------------------------------------------------------------------
    // ---  ESTRUCTURAS / ENUMERADOS  -----------------------------------------------
    // -----------------------------------------------------------------------------


    public struct RankingEntry
    {
        public int m_pos;
        public string m_name;
        public int m_points;
    };

    public enum mode {
        LANZADOR,
        PORTERO,
        TOTAL
    };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------
    

    // instancia de esta clase
    public static ifcRanking instance { get; protected set; }

    // modo en el que se esta mostrando esta interfaz
    private mode m_currentMode = mode.LANZADOR;

    // elementos graficos de esta interfaz
    private btnButton m_btnLanzador;
    private btnButton m_btnPortero;
    private btnButton m_btnTotal;
    private GUIText[] m_txtNombres;
    private GUIText[] m_txtPuntos;

    public RankingEntry[] m_entries = new RankingEntry[6 * 3];

    // rankings de usuarios
    private RankingEntry[] m_rankingLanzador;
    private RankingEntry[] m_rankingPortero;
    private RankingEntry[] m_rankingMultiplayer;

    // posicion del usuario en los rankings
    private RankingEntry m_posUsuarioLanzador;
    private RankingEntry m_posUsuarioPortero;
    private RankingEntry m_posUsuarioMultiplayer;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake() {
        instance = this;
    }


	// Use this for initialization
	void Start () {
        m_backMethod = Back;

        // nombres de usuario del ranking
        m_txtNombres = new GUIText[6];
        for (int i = 0; i < m_txtNombres.Length; ++i)
            m_txtNombres[i] = transform.FindChild("Pos" + i + "/Nombre").GetComponent<GUIText>();

        // puntuaciones en el ranking
        m_txtPuntos = new GUIText[6];
        for (int i = 0; i < m_txtPuntos.Length; ++i)
            m_txtPuntos[i] = transform.FindChild("Pos" + i + "/Puntos").GetComponent<GUIText>();

        // boton de lanzador
        m_btnLanzador = getComponentByName("btnIniesta").GetComponent<btnButton>();
        m_btnLanzador.action = (_name) => {
            Interfaz.ClickFX();
            SetMode(mode.LANZADOR);
        };

        // boton de portero
        m_btnPortero = getComponentByName("btnCasillas").GetComponent<btnButton>();
        m_btnPortero.action = (_name) => {
            Interfaz.ClickFX();
            SetMode(mode.PORTERO);
        };

        // boton total
        m_btnTotal = getComponentByName("btnTotal").GetComponent<btnButton>();
        m_btnTotal.action = (_name) => {
            Interfaz.ClickFX();
            SetMode(mode.TOTAL);
        };

        // boton atras
        getComponentByName("btnAtras").GetComponent<btnButton>().action = Back;
    }


    /// <summary>
    /// Almacena la informacion del usuario local en el ranking
    /// </summary>
    /// <param name="_posUsuarioLanzador"></param>
    /// <param name="_posUsuarioPortero"></param>
    /// <param name="_posUsuarioMultiplayer"></param>
    public void SetInfoPosUsuario(RankingEntry _posUsuarioLanzador, RankingEntry _posUsuarioPortero, RankingEntry _posUsuarioMultiplayer) {
        m_posUsuarioLanzador = _posUsuarioLanzador;
        m_posUsuarioPortero = _posUsuarioPortero;
        m_posUsuarioMultiplayer = _posUsuarioMultiplayer;
    }


    /// <summary>
    /// Almacena la informacion de las primeras posiciones del ranking
    /// </summary>
    /// <param name="_rankingLanzador"></param>
    /// <param name="_rankingPortero"></param>
    /// <param name="_rankingMultiplayer"></param>
    public void SetInfoRankings(RankingEntry[] _rankingLanzador, RankingEntry[] _rankingPortero, RankingEntry[] _rankingMultiplayer) {
        m_rankingLanzador = _rankingLanzador;
        m_rankingPortero = _rankingPortero;
        m_rankingMultiplayer = _rankingMultiplayer;
    }


    void Back(string _target ="")
    {
        GeneralSounds_menu.instance.back();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        cntBarraSuperior.instance.VolverAPantallaAnterior();
    }

    
    public void Refresh() {
        SetMode(m_currentMode);
    }


    void SetMode(mode _mode)
    {
        m_currentMode = _mode;

        // actualizar el estado de los botones ----------------------------------

        // desactivar los botones
        m_btnLanzador.Deselect();
        m_btnPortero.Deselect();
        m_btnTotal.Deselect();

        // activar el boton y mostrar los modelos de los jugadores que corresponda
        switch (_mode) {
            case mode.LANZADOR:
                m_btnLanzador.Select();
                Interfaz.instance.RefrescarModelosJugadores(true, false);
                break;

            case mode.PORTERO:
                m_btnPortero.Select();
                Interfaz.instance.RefrescarModelosJugadores(false, true);
                break;

            case mode.TOTAL:
                m_btnTotal.Select();
                Interfaz.instance.RefrescarModelosJugadores(true, true);
                break;
        }

        // actualizar el ranking de usuarios ------------------------------------

        // por defecto eliminar los datos
        for (int i = 0; i < m_txtNombres.Length; ++i) {
            m_txtNombres[i].text = "";
            m_txtPuntos[i].text = "0";
        }


        switch (_mode) {
            case mode.LANZADOR:
                // ranking de lanzador
                if (m_rankingLanzador != null) {
                    for (int i = 0; i < m_txtNombres.Length - 1; ++i) {
                        m_txtNombres[i].text = m_rankingLanzador[i].m_name;
                        m_txtPuntos[i].text = m_rankingLanzador[i].m_points.ToString();
                    }
                }

                // posicion del usuario local
                m_txtNombres[5].text = m_posUsuarioLanzador.m_name;
                m_txtPuntos[5].text = m_posUsuarioLanzador.m_points.ToString();
                break;

            case mode.PORTERO:
                // ranking de portero
                if (m_rankingPortero != null) {
                    for (int i = 0; i < m_rankingPortero.Length - 1; ++i) {
                        m_txtNombres[i].text = m_rankingPortero[i].m_name;
                        m_txtPuntos[i].text = m_rankingPortero[i].m_points.ToString();
                    }
                }

                // posicion del usuario local
                m_txtNombres[5].text = m_posUsuarioPortero.m_name;
                m_txtPuntos[5].text = m_posUsuarioPortero.m_points.ToString();
                break;

            case mode.TOTAL:
                // ranking de portero
                if (m_rankingMultiplayer != null) {
                    for (int i = 0; i < m_rankingMultiplayer.Length - 1; ++i) {
                        m_txtNombres[i].text = m_rankingMultiplayer[i].m_name;
                        m_txtPuntos[i].text = m_rankingMultiplayer[i].m_points.ToString();
                    }
                }

                // posicion del usuario local
                m_txtNombres[5].text = m_posUsuarioMultiplayer.m_name;
                m_txtPuntos[5].text = m_posUsuarioMultiplayer.m_points.ToString();
                break;
        }
    }

	
}
