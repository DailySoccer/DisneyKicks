using UnityEngine;
using System.Collections;

public class cntSelectorJugadorEquipacion : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Modos en los que puede funcionar este control
    /// </summary>
    public enum Modo { JUGADOR, EQUIPACION };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static cntSelectorJugadorEquipacion instance {
        get {
            if (m_instance == null) {
                Transform tr = ifcVestuario.instance.transform.FindChild("selectorJugadorEquipacion");
                if (tr != null) {
                    m_instance = tr.GetComponent<cntSelectorJugadorEquipacion>();
                    m_instance.Start();
                }
            }

            return m_instance; 
        }
    }
    private static cntSelectorJugadorEquipacion m_instance;

    // elementos de esta interfaz
    //private btnButton m_btnJugador;
    //private btnButton m_btnEquipar;
    private btnButton m_btnJugadorNavIzda;
    private btnButton m_btnJugadorNavDcha;
    private btnButton m_btnEquiparNavIzda;
    private btnButton m_btnEquiparNavDcha;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    private void ObtenerReferencias() {
        // obtener las referencias a los elementos de este control
        //m_btnJugador = transform.FindChild("btnJugador").GetComponent<btnButton>();
        //m_btnEquipar = transform.FindChild("btnEquipar").GetComponent<btnButton>();
        m_btnJugadorNavIzda = transform.FindChild("btnJugadorNavIzda").GetComponent<btnButton>();
        m_btnJugadorNavDcha = transform.FindChild("btnJugadorNavDcha").GetComponent<btnButton>();
        m_btnEquiparNavIzda = transform.FindChild("btnEquiparNavIzda").GetComponent<btnButton>();
        m_btnEquiparNavDcha = transform.FindChild("btnEquiparNavDcha").GetComponent<btnButton>();
    }


	// Use this for initialization
	void Start () {
        // obtener referencias a los elementos graficos de esta interfaz
        ObtenerReferencias();

        /*
        // boton jugador
        m_btnJugador.action = (_name) => {
            Interfaz.ClickFX();

            // actualizar este control
            ActualizarEstadoBotones(Modo.JUGADOR);
            RefrescarContador();

            // refrescar el jugador y actualizar su tooltip asociado
            
            ifcVestuario.instance.CambiarJugadorSeleccionado(0, true);
        };
         */

        // boton jugador navegar izquierda
        m_btnJugadorNavIzda.action = (_name) => {
            GeneralSounds_menu.instance.select();
            ifcVestuario.instance.CambiarJugadorSeleccionado(-1);
        };

        // boton jugador navegar derecha
        m_btnJugadorNavDcha.action = (_name) => {
            GeneralSounds_menu.instance.select();
            ifcVestuario.instance.CambiarJugadorSeleccionado(+1);
        };

        /*
        // boton equipacion
        m_btnEquipar.action = (_name) => {
            Interfaz.ClickFX();

            // actualizar este control
            ActualizarEstadoBotones(Modo.EQUIPACION);
            RefrescarContador();

            // refrescar la equipacion y actualizar su tooltip asociado
            ifcVestuario.instance.CambiarEquipacionSeleccionada(0, true);
        };
         */

        // boton equipacion navegar izquierda
        m_btnEquiparNavIzda.action = (_name) => {
            GeneralSounds_menu.instance.select();
            ifcVestuario.instance.CambiarEquipacionSeleccionada(-1, true);
        };

        // boton equipacion navegar derecha
        m_btnEquiparNavDcha.action = (_name) => {
            GeneralSounds_menu.instance.select();
            ifcVestuario.instance.CambiarEquipacionSeleccionada(+1, true);
        };

        // por defecto mostrar el modo jugador
        //ActualizarEstadoBotones(Modo.JUGADOR);
	}

	
    /*
    /// <summary>
    /// Actualiza el estado del control en funcion del modo en el que esta trabajando
    /// </summary>
    /// <param name="_modo"></param>
    private void ActualizarEstadoBotones(Modo _modo) {
        m_modo = _modo;

        if (_modo == Modo.JUGADOR) {
            m_btnJugador.Select();
            m_btnEquipar.Deselect();
        } else {
            m_btnJugador.Deselect();
            m_btnEquipar.Select();
        }

        m_btnJugadorNavDcha.gameObject.SetActive(_modo == Modo.JUGADOR);
        m_btnJugadorNavIzda.gameObject.SetActive(_modo == Modo.JUGADOR);

        m_btnEquiparNavDcha.gameObject.SetActive(_modo == Modo.EQUIPACION);
        m_btnEquiparNavIzda.gameObject.SetActive(_modo == Modo.EQUIPACION);
    }
    */


}
