using UnityEngine;
using System.Collections;

public class cntLogros : MonoBehaviour
{
    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // -----------------------------------------------------------------------------


    // cantidad de logros que hay de cada tipo
    private const int NUM_LOGROS_PORTERO = 20;
    private const int NUM_LOGROS_TIRADOR = 20;
    private const int NUM_LOGROS_MULTIJUGADOR = 9;

    // cantidad de logros por pagina
    private const int NUM_LOGROS_PAGINA = 9;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public static cntLogros instance { get; protected set; }

    public btnButton m_flechaIzq;
    public btnButton m_flechaDer;

    ifcLogros.Modo m_currentMode = ifcLogros.Modo.NONE;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake()
    {
        instance = this;
    }    

    public LogrosDescription m_logros;
    GameObject[] m_icons = new GameObject[NUM_LOGROS_PAGINA];
    public int m_current = 0;
    // Use this for initialization
    void Start () {
        if (m_logros != null) {
            foreach (LogrosDescription.descLogro logro in m_logros.m_lista)
                logro.m_desbloqueado = false;

            for (int i = 0; i < NUM_LOGROS_PAGINA; ++i)
                m_icons[i] = transform.Find("logro_" + (i + 1)).gameObject;
            m_flechaIzq.action = (_name) => {
                goPage(m_currentMode, m_current - 1);
            };
            m_flechaDer.action = (_name) => {
                goPage(m_currentMode, m_current + 1);
            };
        }
    }
    
    public void Unlock(string _code) {
        foreach (LogrosDescription.descLogro logro in m_logros.m_lista) {
            if (logro.m_codigo == _code)
            {
                Interfaz.m_achievements++;
                logro.m_desbloqueado = true;
            }
        }
    }

    public void Refresh(ifcLogros.Modo _mode)
    {
        int tmp = m_current;
        m_current = -1;
        goPage(_mode,tmp);
    }


    private void goPage(ifcLogros.Modo _mode, int _page) {
        // si no hay lista de logros
        if (m_logros.m_lista == null || m_logros.m_lista.Length == 0)
            return;

        // si se va a listar la misma pagina de logros que se esta visualizando
        if (m_current == _page && m_currentMode == _mode)
            return;

        // si cambia de modo => mostrar la pagina 0
        if (m_currentMode != _mode)
            _page = 0;

        // guardar el modo y la pagina a mostrar
        m_current = _page;
        m_currentMode = _mode;

        // calcular la posicion del primer logro y la cantidad de logros de este tipo
        int primerLogroDeEsteTipo;
        int numLogrosDeEsteTipo;
        switch (_mode) {
            case ifcLogros.Modo.PORTERO: // PORTERO
                primerLogroDeEsteTipo = 0;
                numLogrosDeEsteTipo = NUM_LOGROS_PORTERO;
                break;

            case ifcLogros.Modo.LANZADOR: // TIRADOR
                primerLogroDeEsteTipo = NUM_LOGROS_PORTERO;
                numLogrosDeEsteTipo = NUM_LOGROS_TIRADOR;
                break;

            default: // MULTIJUGADOR
                primerLogroDeEsteTipo = NUM_LOGROS_PORTERO + NUM_LOGROS_TIRADOR;
                numLogrosDeEsteTipo = NUM_LOGROS_MULTIJUGADOR;
                break;
        }
      
        // comprobar si hay que mostrar la flecha paginar izda y derecha
        m_flechaIzq.gameObject.SetActive(m_current != 0);
        m_flechaDer.gameObject.SetActive(m_current < ((numLogrosDeEsteTipo - 1) / NUM_LOGROS_PAGINA));

        // pintar los logros de la pagina
        int primerLogroPagina = primerLogroDeEsteTipo + (_page * NUM_LOGROS_PAGINA);
        int ultimoLogroDeEsteTipo = primerLogroDeEsteTipo + numLogrosDeEsteTipo;
        for (int i = 0; (i < NUM_LOGROS_PAGINA); ++i) {
            if (primerLogroPagina + i < ultimoLogroDeEsteTipo) {
                // mostrar el logro
                m_icons[i].GetComponent<btnLogro>().set(m_logros.m_lista[primerLogroPagina + i]);
                m_icons[i].SetActive(true);
            } else {
                // ocultar el logro
                m_icons[i].SetActive(false);
            }
        }
    }


}
