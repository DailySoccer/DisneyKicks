using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ifcLogros : ifcBase
{
    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ---------------------------------------------------------------------------


    /// <summary>
    /// Numero de logros que se muestran en cada pagina
    /// </summary>
    private int NUM_LOGROS_PAGINA = 3;


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public enum Modo {
        NONE,
        PORTERO,
        LANZADOR,
        DUELO
    };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    // instancia de esta clase
    public static ifcLogros instance { 
        get {
            if (m_instance == null) {
                Transform tr = Interfaz.instance.transform.FindChild("Logros");
                if (tr != null) {
                    m_instance = tr.GetComponent<ifcLogros>();
                    m_instance.Start();
                }
            }

            return m_instance;
        } 
    }
    private static ifcLogros m_instance;

    // modo en el que se esta mostrando esta interfaz
    public Modo m_modo = Modo.LANZADOR;

    // elementos de esta interfaz
    private GUIText m_txtTitulo;
    private GUIText m_txtTituloSombra;
    private btnButton m_btnAtras;
    private btnButton m_btnLanzador;
    private btnButton m_btnPortero;
    private btnButton m_btnDuelo;
    private cntVisualizadorLogro[] m_visualizadoresLogros;


    // ------------------------------------------------------------------------------
    // ---  METODODS  ---------------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    /// <summary>
    /// Obtienen las referencias a los elementos de la interfaz
    /// </summary>
    private void GetReferencias() {
        // referencia a los textos
        if (m_txtTitulo == null)
            m_txtTitulo = transform.FindChild("ContenedorLogros/titulo").GetComponent<GUIText>();
        if (m_txtTituloSombra == null)
            m_txtTituloSombra = transform.FindChild("ContenedorLogros/tituloSombra").GetComponent<GUIText>();

        // referencia a los botones
        if (m_btnAtras == null) {
            m_btnAtras = getComponentByName("btnAtras").GetComponent<btnButton>();
            m_backMethod = Back;
            m_btnAtras.action = Back;
        }

        if (m_btnLanzador == null) {
            m_btnLanzador = transform.FindChild("botones/btnLanzador").GetComponent<btnButton>();
            m_btnLanzador.action = (_name) => {
                Interfaz.ClickFX();
                OnPulsadoBotonSeleccionModo(Modo.LANZADOR);
            };
        }
        if (m_btnPortero == null) {
            m_btnPortero = transform.FindChild("botones/btnPortero").GetComponent<btnButton>();
            m_btnPortero.action = (_name) => {
            Interfaz.ClickFX();
            OnPulsadoBotonSeleccionModo(Modo.PORTERO);
        };
        }
        if (m_btnDuelo == null) {
            m_btnDuelo = transform.FindChild("botones/btnDuelo").GetComponent<btnButton>();
            m_btnDuelo.action = (_name) => {
                Interfaz.ClickFX();
                OnPulsadoBotonSeleccionModo(Modo.DUELO);
            };
        }

        // obtener la referencia a los visores de logros
        if (m_visualizadoresLogros == null) {
            m_visualizadoresLogros = new cntVisualizadorLogro[NUM_LOGROS_PAGINA];
            for (int i = 0; i < NUM_LOGROS_PAGINA; ++i) {
                m_visualizadoresLogros[i] = transform.FindChild("ContenedorLogros/visualizadorLogro" + (i + 1).ToString()).GetComponent<cntVisualizadorLogro>();
            }
        }
    }


    // Use this for initialization
    void Start() {
        // obtener las referencias a los elementos de la interfaz
        GetReferencias();

        // refrescar las listas de logros
        Refresh();
    }


    void Back(string _target = "") {
        GeneralSounds_menu.instance.back();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name) => { SetVisible(false); });
        cntBarraSuperior.instance.VolverAPantallaAnterior();
        Interfaz.instance.Thrower = Interfaz.instance.Thrower;
        Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
    }


    public void OnPulsadoBotonSeleccionModo(Modo _modo) {
        // obtener las referencias a los elementos de la interfaz
        GetReferencias();

        // guardar el modo
        m_modo = _modo;

        // mostrar el titulo (localizado) que corresponda
        string strTitulo = "";
        switch (_modo) {
            case Modo.LANZADOR:
                strTitulo = LocalizacionManager.instance.GetTexto(14).ToUpper();
                break;
            case Modo.PORTERO:
                strTitulo = LocalizacionManager.instance.GetTexto(15).ToUpper();
                break;
            case Modo.DUELO:
                strTitulo = LocalizacionManager.instance.GetTexto(9).ToUpper();
                break;
        }
        m_txtTitulo.text = strTitulo;
        m_txtTituloSombra.text = strTitulo;

        // inhabilitar los botones
        m_btnLanzador.Deselect();
        m_btnPortero.Deselect();
        m_btnDuelo.Deselect();

        // actualizar la pagina en funcion del modo seleccionado y habilitar el boton que corresponda
        switch (_modo) {
            case Modo.LANZADOR:
                //Interfaz.instance.RefrescarModelosJugadores(true, false);
                ActualizarPaginaLogros(LogrosManager.logrosLanzador);
                m_btnLanzador.Select();
                break;
                
            case Modo.PORTERO:
                //Interfaz.instance.RefrescarModelosJugadores(false, true);
                ActualizarPaginaLogros(LogrosManager.logrosPortero);
                m_btnPortero.Select();
                break;

            case Modo.DUELO:
                //Interfaz.instance.RefrescarModelosJugadores(true, true);
                ActualizarPaginaLogros(LogrosManager.logrosDuelo);
                m_btnDuelo.Select();
                break;
        }

        Interfaz.instance.Thrower = Interfaz.instance.Thrower;
        Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
    }

    /// <summary>
    /// Refresca las pagina de logros
    /// </summary>
    public void Refresh() {
        // obtener las referencias a los elementos de la interfaz
        GetReferencias();

        // ordenar las listas de logros por su progreso y mostrarlas
        LogrosManager.instance.OrdenarListasLogrosPorProgreso();
    }


    /// <summary>
    /// Metodo para actualizar la pagina de logros
    /// </summary>
    /// <param name="_gruposLogros"></param>
    private void ActualizarPaginaLogros(List<GrupoLogros> _gruposLogros) {
        // ocultar por defecto los visualizadores de logros
        for (int i= 0; i < NUM_LOGROS_PAGINA; ++i) {
            m_visualizadoresLogros[i].gameObject.SetActive(false);
        }

        if (_gruposLogros != null) {
            // pintar los grupos de logros en los controles
            for (int i = 0; (i < _gruposLogros.Count) && (i < NUM_LOGROS_PAGINA); ++i) {
                m_visualizadoresLogros[i].gameObject.SetActive(true);
                m_visualizadoresLogros[i].ShowValues(_gruposLogros[i]);
            }
        }
    }


    // <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


}
