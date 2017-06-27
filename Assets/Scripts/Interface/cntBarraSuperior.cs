#define MONEY_FREE

using UnityEngine;
using System.Collections;

/// <summary>
/// Control para mostrar la barra superior de opciones
/// </summary>
public class cntBarraSuperior : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    /// <summary>
    /// Instancia de esya clase
    /// </summary>
    public static cntBarraSuperior instance { get { return m_instance; } }
    private static cntBarraSuperior m_instance;

    // texturas para el boton de logros
    // NOTA: asignar valor a estas variables desde la interfaz de Unity
    public Texture m_texturaLogros;
    public Texture m_texturaLogrosRecientes;

    // Ultima pantalla visitada antes de utilizar los botones de la barra de menu
    private ifcBase m_ultimaPantallaVisitada;

    // Posicion de la ultima pantalla visitada antes de utilizar los botones de la barra de menu cuando esta visible
    private Vector3 m_posVisibleUltimaPantallaVisitada;

    // variables para almacenar si los modelos de lanzador y portero estaban visibles antes de pulsar alguna opcion de este menu
    private bool m_modeloLanzadorVisibleUltimaPantallaVisitada;
    private bool m_modeloPorteroVisibleUltimaPantallaVisitada;

    // referencias a los elementos de esta interfaz
    private btnButton m_btnAvatar;
    private btnButton m_btnMonedaHard;
    private btnButton m_btnMonedaSoft;
    private btnButton m_btnLogros;
    private GUITexture m_iconoBotonLogros;
    private GUIText m_txtMonedaSoft;
    private GUIText m_txtMonedaSoftSombra;
    private GUIText m_txtMonedaHard;
    private GUIText m_txtMonedaHardSombra;
	private GUIText m_txtSkillLevel;
	private GUIText m_txtSkillLevelSombra;

    //con esta variable se indica desde ingame que al volver al menu tendra que anuncuar nuevos logros
    public static bool flagNuevosLogros = false;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------

    void Awake() {
        m_instance = this;

        // mostrar una alerta si faltan texturas por definir
        if (m_texturaLogros == null)
            Debug.LogWarning("So se ha asignado valor a 'm_texturaLogros'");
        if (m_texturaLogrosRecientes == null)
            Debug.LogWarning("So se ha asignado valor a 'm_texturaLogrosRecientes'");
    }


    private void GetReferencias() {
        if (m_btnAvatar == null)
            m_btnAvatar = transform.FindChild("btnPerfil").GetComponent<btnButton>();
        if (m_btnMonedaHard == null)
            m_btnMonedaHard = transform.FindChild("btnMonedaHard").GetComponent<btnButton>();
        if (m_btnMonedaSoft == null)
            m_btnMonedaSoft = transform.FindChild("btnMonedaSoft").GetComponent<btnButton>();
        if (m_btnLogros == null)
            m_btnLogros = transform.FindChild("btnLogros").GetComponent<btnButton>();
        if (m_iconoBotonLogros == null)
            m_iconoBotonLogros = transform.FindChild("btnLogros/Icono").GetComponent<GUITexture>();
    }


    // Use this for initialization
    void Start () {
        GetReferencias();
        
        // boton perfil
        m_btnAvatar.action = (_name) => {
            OnPerfil();
        };

        // boton moneda hard
        m_btnMonedaHard.action = (_name) => {
            Interfaz.ClickFX();
            ifcBuyHardCashDialogBox.Instance.Show(true);

            #if MONEY_FREE

            PurchaseManager.PerformPurchase("com.bitoon.kicksfootballwarriors.gembooster.1");
            ActualizarDinero();

            #endif
        };

        // boton moneda soft
        m_btnMonedaSoft.action = (_name) => {
            Interfaz.ClickFX();
            ifcBuyHardCashDialogBox.Instance.Show(false);

            #if MONEY_FREE

            PurchaseManager.PerformPurchase("com.bitoon.kicksfootballwarriors.coinbooster.1");
            ActualizarDinero();

            #endif
        };

        // boton logros
        m_btnLogros.action = (_name) => {
            // mostrar la textura normal del boton de logros
            m_iconoBotonLogros.texture = m_texturaLogros;

            OnLogros();
        };

        // mostrar el dinero que el usuario tiene actualmente
        ActualizarDinero();

		//mostrar el Skill Level del usuario
		ActualizaSkillLevel();

        if(flagNuevosLogros)
        {
            flagNuevosLogros = false;
            MostrarQueHayNuevosLogros();
        }
    }


    /// <summary>
    /// Metodo para volver a la pantalla previa antes de elegir alguna de las opciones de la barra de menu
    /// </summary>
    public void VolverAPantallaAnterior() {
        // volver a la pantalla previa al menu
        new SuperTweener.move(m_ultimaPantallaVisitada.gameObject, 0.25f, m_posVisibleUltimaPantallaVisitada, SuperTweener.CubicOut);
        ifcBase.activeIface = m_ultimaPantallaVisitada;

        // actualizar el estado de visibilidad de los modelos de los jugadores
        Interfaz.instance.RefrescarModelosJugadores(m_modeloLanzadorVisibleUltimaPantallaVisitada, m_modeloPorteroVisibleUltimaPantallaVisitada);
    }


    /// <summary>
    /// Muestra una exclamacion sobre el boton de logros
    /// </summary>
    public void MostrarQueHayNuevosLogros() {
        GetReferencias();
        m_iconoBotonLogros.texture = m_texturaLogrosRecientes;
    }


    /// <summary>
    /// Refresca las cantidades de dinero mostradas en la barra superior
    /// </summary>
    public void ActualizarDinero() {
        if (m_txtMonedaSoft == null)
            m_txtMonedaSoft = transform.FindChild("btnMonedaSoft/txtCantidad").GetComponent<GUIText>();
        
        if (m_txtMonedaSoftSombra == null)
            m_txtMonedaSoftSombra = transform.FindChild("btnMonedaSoft/txtCantidadSombra").GetComponent<GUIText>();

        if (m_txtMonedaHard == null)
            m_txtMonedaHard = transform.FindChild("btnMonedaHard/txtCantidad").GetComponent<GUIText>();

        if (m_txtMonedaHardSombra == null)
            m_txtMonedaHardSombra = transform.FindChild("btnMonedaHard/txtCantidadSombra").GetComponent<GUIText>();

        m_txtMonedaSoft.text = Interfaz.MonedasSoft.ToString();
        m_txtMonedaSoftSombra.text = Interfaz.MonedasSoft.ToString();
        m_txtMonedaHard.text = Interfaz.MonedasHard.ToString();
        m_txtMonedaHardSombra.text = Interfaz.MonedasHard.ToString();
    }

	/// <summary>
	/// Refresca el Skill Level mostrado en la barra superior
	/// </summary>

	public void ActualizaSkillLevel()
	{
		if (m_txtSkillLevel == null)
			m_txtSkillLevel = transform.FindChild("btnPerfil/txtCantidad").GetComponent<GUIText>();

		if (m_txtSkillLevelSombra == null)
			m_txtSkillLevelSombra = transform.FindChild("btnPerfil/txtCantidadSombra").GetComponent<GUIText>();
	
        m_txtSkillLevel.text = Interfaz.SkillLevel.ToString();
        m_txtSkillLevelSombra.text = Interfaz.SkillLevel.ToString();
	}


    /// <summary>
    /// Ir a la pantalla de perfil
    /// </summary>
    void OnPerfil() {
        // si ya se esta mostrando la pantalla de perfil => no hacer nada
        if (ifcBase.activeIface == ifcPerfil.instance)
            return;

        ifcPerfil.instance.SetVisible(true);
        ShowPantalla(ifcPerfil.instance, Vector3.zero);

        // actualizar la pantalla de perfil
        ifcPerfil.instance.Refresh();
    }


    /// <summary>
    /// Ir a la pantalla de logros
    /// </summary>
    private void OnLogros() {
        // si ya se esta mostrando la pantalla de logros => no hacer nada
        if (ifcBase.activeIface == ifcLogros.instance)
            return;

        ifcLogros.instance.SetVisible(true);
        ShowPantalla(ifcLogros.instance, Vector3.zero);

        // mostrar por defecto los logros de lanzador
        ifcLogros.instance.OnPulsadoBotonSeleccionModo(ifcLogros.Modo.LANZADOR);
    }


    /// <summary>
    /// Ir a la pantalla de ranking
    /// </summary>
    private void OnRanking() {
        ShowPantalla(ifcRanking.instance, Vector3.zero);

        // actualizar la pantalla de ranking
        ifcRanking.instance.Refresh();
    }


    /// <summary>
    /// Muestra la pantalla recibida como parametro y la lleva (con el supertweener a la posicion "_posicionDestino")
    /// </summary>
    /// <param name="_pantallaDestino"></param>
    /// <param name="_posicionDestino"></param>
	public void ShowPantalla(ifcBase _pantallaDestino, Vector3 _posicionDestino) {
        // obtener la interfaz actual
        ifcBase interfazActual = ifcBase.activeIface;

        // posicion destino de la interfaz que se va a ocultar
        Vector3 posicionDestinoInterfaz = new Vector3(-1.0f, 0.0f, 0.0f);

        // si la interfaz actual no es ninguna de las que se accede desde esta barra de botones => guardarla
        if (interfazActual != ifcPerfil.instance &&
            interfazActual != ifcLogros.instance) {
            m_ultimaPantallaVisitada = interfazActual;
            m_posVisibleUltimaPantallaVisitada = interfazActual.transform.localPosition;

            // calcular la posicion destino de la interfaz que se va a ocultar
            posicionDestinoInterfaz = new Vector3(-1.0f, m_posVisibleUltimaPantallaVisitada.y, m_posVisibleUltimaPantallaVisitada.z);

            // guardar el estado de visibilidad de los modelos de los jugadores
            m_modeloLanzadorVisibleUltimaPantallaVisitada = Interfaz.instance.GetModeloLanzadorEstaVisible();
            m_modeloPorteroVisibleUltimaPantallaVisitada = Interfaz.instance.GetModeloPorteroEstaVisible();
        }

        Interfaz.ClickFX();
        Interfaz.instance.CleanPlayers();

        // ocultar esta pantalla y pasar a la que corresponda
        new SuperTweener.move(interfazActual.gameObject, 0.25f, posicionDestinoInterfaz, SuperTweener.CubicOut,
            // on endCallback
            (_name) => {
                if (interfazActual == ifcPerfil.instance)
                    ifcPerfil.instance.SetVisible(false);

                if (interfazActual == ifcLogros.instance)
                    ifcLogros.instance.SetVisible(false);
            });
        new SuperTweener.move(_pantallaDestino.gameObject, 0.25f, _posicionDestino, SuperTweener.CubicOut);
        ifcBase.activeIface = _pantallaDestino;
    }


    /// <summary>
    /// Sirve para mostrar/ocultar esta barra de menu
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }

}
