using UnityEngine;
using System.Collections;


/// <summary>
/// Clase para mostrar la interfaz de carrera. 
/// </summary>
public class ifcCarrera : ifcBase {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public static ifcCarrera instance {
        get {
            if (m_instance == null) {
                Transform tr = Interfaz.instance.transform.FindChild("Carrera");
                if (tr != null) {
                    m_instance = tr.GetComponent<ifcCarrera>();
                    m_instance.Start();
                }
            }
            return m_instance;
        }
    }
    private static ifcCarrera m_instance;

    /// <summary>
    /// Indica si se esta mostrando una fase de lanzador o una de portero
    /// </summary>
    public bool estoyMostrandoMisionDeLanzador = false;

    // elementos de esta interfaz
    private cntMissions m_controlMissions; // Referencia al tooltipLevelSelection de esta pantalla (a partir del cual se puede obtener la mision y el nivel dentro de la mision seleccionados)
    private btnButton m_btnAtras;

    


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------

    void Awake() {
        m_instance = this;
    }


    /// <summary>
    /// Obtiene las referencias a los elementos de esta interfaz
    /// </summary>
    private void GetReferencias() {
        // boton de volver atras
        if (m_btnAtras == null) {
            m_btnAtras = getComponentByName("btnAtras").GetComponent<btnButton>();
            m_backMethod = Back;
            m_btnAtras.action = Back;
        }

        // referencia al tooltipLevelSelection de esta pantalla
        if (m_controlMissions == null)
            m_controlMissions = transform.FindChild("cntMissions").GetComponent<cntMissions>();
    }


    void Start () {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();
    }


    void Back (string _name = "") {
        GeneralSounds_menu.instance.back();
        ifcMainMenu.instance.SetVisible(true);
        new SuperTweener.move( gameObject, 0.25f, new Vector3( 1.0f, 0.0f, 0.0f ), SuperTweener.CubicOut );
        new SuperTweener.move(ifcMainMenu.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
        cntMenuDesplegableOpciones.instance.Plegar();

        ifcBase.activeIface = ifcMainMenu.instance;

        // mostrar los dos modelos de jugadores
        Interfaz.instance.RefrescarModelosJugadores(true, true);
    }


    /// <summary>
    /// Refresca esta pagina
    /// </summary>
    public void Refresh() {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        cntMissions.instance.Refresh();
    }


    /// <summary>
    /// Muestra la ultima mision desbloqueada por el usuario
    /// </summary>
    public void MostrarUltimaMision() {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        m_controlMissions.SeleccionarUltimaMisionAndNivel();
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


}
