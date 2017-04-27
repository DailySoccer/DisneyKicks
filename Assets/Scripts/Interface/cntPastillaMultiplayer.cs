using UnityEngine;
using System.Collections;

/// <summary>
/// Control para mostrar la pastilla de marcadores de gol del modo multijugaor
/// </summary>
public class cntPastillaMultiplayer : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    private int NUM_TIROS = 5;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------

    // objeto prefab para poder generar instancias
    public cntMarcadorGolMultijugador m_prefMarcadorGol;
    public bool marcadorIzquierda;

    private MatchStateSimple m_currentState;

    // elementos de esta interfaz
    private GUIText m_txtNombreJugador;
    private GUIText m_txtNumRonda;
    private GUIText m_txtTotalGoles;
    private int m_lanzamientos = 0;
    public int Lanzamientos {
        set{m_lanzamientos = value; SetNumRonda(value);}
        get{return m_lanzamientos;}
    }

    // marcadores de gol individuales
    private cntMarcadorGolMultijugador[] m_marcadoresGol;

    public static cntPastillaMultiplayer marcadorLocal { 
        get {
            if (m_marcadorLocal == null && GameObject.Find("pastillaMultiplayerIzda"))
                m_marcadorLocal = GameObject.Find("pastillaMultiplayerIzda").GetComponent<cntPastillaMultiplayer>();
            return m_marcadorLocal;
        }
    }
    private static cntPastillaMultiplayer m_marcadorLocal;

    public static cntPastillaMultiplayer marcadorRemoto { 
        get {
            if (m_marcadorRemoto == null && GameObject.Find("pastillaMultiplayerDcha"))
                m_marcadorRemoto = GameObject.Find("pastillaMultiplayerDcha").GetComponent<cntPastillaMultiplayer>();
            return m_marcadorRemoto;
        }
    }
    private static cntPastillaMultiplayer m_marcadorRemoto;

    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Inicializa este marcador
    /// </summary>
    /// <param name="_nombreJugador"></param>
    public void Inicializar(string _nombreJugador) {
        m_currentState = new MatchStateSimple{ score = 0, marker = new int[5], rounds = 0 }; 
        // obtener la referencia a los elementos de la interfaz
        if (m_txtNombreJugador == null)
            m_txtNombreJugador = transform.FindChild("txtNombre").GetComponent<GUIText>();
        if (m_txtNumRonda == null)
            m_txtNumRonda = transform.FindChild("txtNumRonda").GetComponent<GUIText>();
        if (m_txtTotalGoles == null)
            m_txtTotalGoles = transform.FindChild("txtTotalGoles").GetComponent<GUIText>();
        m_marcadoresGol = new cntMarcadorGolMultijugador[NUM_TIROS];
        for (int i = 0; i < NUM_TIROS; ++i) {
            m_marcadoresGol[i] = transform.FindChild("contenedorGoles/marcadorGol" + (i + 1).ToString()).GetComponent<cntMarcadorGolMultijugador>();
        }
        
        // actualizar los valores mostrados
        m_txtNombreJugador.text = _nombreJugador;
        m_txtNumRonda.text = "0";

        // indicar que aun no se ha tirado ningun balon
        for (int i = 0; i < NUM_TIROS; ++i)
            SetEstadoLanzamiento(i, cntMarcadorGolMultijugador.Estado.SIN_TIRAR);

        m_txtTotalGoles.text = "0";
    }


    /// <summary>
    /// Actualiza el valor del numero de ronda
    /// </summary>
    /// <param name="_numRonda"></param>
    public void SetNumRonda(int _numRonda) {
        m_txtNumRonda.text = _numRonda.ToString();
    }


    /// <summary>
    /// Actualiza el estado del marcador de tiro especificado
    /// </summary>
    /// <param name="_numLanzamiento">Debe ser un valor en el intervalo [0 .. NUM_TIROS[</param>
    /// <param name="_estado"></param>
    public void SetEstadoLanzamiento(int _numLanzamiento, cntMarcadorGolMultijugador.Estado _estado) {
        if (m_marcadoresGol != null) {
            if (_numLanzamiento >= 0 && _numLanzamiento < m_marcadoresGol.Length)
                m_marcadoresGol[_numLanzamiento].SetEstado(_estado);
        }
    }


    /// <summary>
    /// Actualiza el numero de goles marcados
    /// </summary>
    private void ActualizarTotalGoles(int _totalGoles) {
        m_txtTotalGoles.text = _totalGoles.ToString();
    }

    public void SetEstado(MatchStateSimple _state) {
        m_currentState = _state;
        ActualizarTotalGoles(_state.score);
        for(int i = 0; i < NUM_TIROS; ++i)
        {
            SetEstadoLanzamiento(i, (cntMarcadorGolMultijugador.Estado)_state.marker[i]);
        }
        //SetNumRonda(_state.rounds);
    }

    public MatchStateSimple AddResult(bool _success)
    {
        if(m_currentState.rounds < m_currentState.marker.Length) m_currentState.marker[m_currentState.rounds] = _success ? 1: 2;
        if(_success) m_currentState.score++;
        SetEstado(m_currentState);
        m_currentState.rounds++;
        return m_currentState;
    }
}
