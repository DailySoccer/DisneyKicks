using UnityEngine;
using System.Collections;

public class ifcItemRevealDialog : ifcBase {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static ifcItemRevealDialog instance { get { return m_instance; } }
    private static ifcItemRevealDialog m_instance;

    // texturas para las recompensas
    // NOTA: asignar valor a estas texturas desde la interfaz de Unity
    public Texture m_textureRewardHardCash;
    public Texture m_textureRewardSoftCash;

    // elementos de la interfaz
    private Transform m_caja;

    // elementos de la interfaz
    private btnButton m_btnCancelar;
    private btnButton m_btnAceptar;
    private GUIText m_txtTitulo;
    private GUIText m_txtTituloSombra;
    private GUIText m_txtTexto;
    private GUITexture m_imgAvatar;     // imagen para mostrar jugadores/equipaciones
    private GUITexture m_imgEscudo;     // imagen exclusiva para mostrar escudos
    private GUITexture m_imgCash;       // imagen para mostrar el hard/soft cash
    private GUIText m_txtEscudoBoost;
    private GUIText m_txtCantidadCash;
    private GUIText m_txtCantidadCashSombra;
    private btnButton m_velo; // <= el velo funciona como el boton de cerrar


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake() {
        // asegurarse de que unicamente hay una instancia de esta clase => para utilizar el patron singleton
        if (m_instance == null) {
            m_instance = this;
        } else
            DestroyImmediate(gameObject);

        // mostrar alertas si no se ha inicializado las texturas
        if (m_textureRewardHardCash == null)
            Debug.LogWarning("No se ha asignado textura a 'm_textureRewardHardCash'");
        if (m_textureRewardSoftCash == null)
            Debug.LogWarning("No se ha asignado textura a 'm_textureRewardSoftCash'");
    }


    private void GetReferencias() {
        // mostrar el dialogo mediano
        m_caja = transform.FindChild("caja");

        // obtener referencias a los elementos de la interfaz
        m_btnAceptar = m_caja.FindChild("btnAceptar").GetComponent<btnButton>();
        m_btnCancelar = m_caja.FindChild("btnCancelar").GetComponent<btnButton>();
        m_txtTitulo = m_caja.FindChild("titulo").GetComponent<GUIText>();
        m_txtTituloSombra = m_txtTitulo.transform.FindChild("sombra").GetComponent<GUIText>();
        m_txtTexto = m_caja.FindChild("texto").GetComponent<GUIText>();
        m_imgAvatar = m_caja.FindChild("imgAvatar").GetComponent<GUITexture>();
        m_imgEscudo = m_caja.FindChild("imgAvatarEscudo").GetComponent<GUITexture>();
        m_txtEscudoBoost = m_imgEscudo.transform.FindChild("texto").GetComponent<GUIText>();
        m_imgCash = m_caja.FindChild("imgCash").GetComponent<GUITexture>();
        m_txtCantidadCash = m_imgCash.transform.FindChild("texto").GetComponent<GUIText>();
        m_txtCantidadCashSombra = m_imgCash.transform.FindChild("textoSombra").GetComponent<GUIText>();
        m_velo = transform.FindChild("velo").GetComponent<btnButton>();
    }


	// Use this for initialization
	void Start () {
	    // hacer que este dialogo persista entre escenas
        DontDestroyOnLoad(gameObject);

        // ocultar este dialogo
        transform.gameObject.SetActive(false);
	}


    /// <summary>
    /// Muestra la informacion asociada a un jugador
    /// </summary>
    /// <param name="_jugador"></param>
    /// <param name="_onCloseCallback">Accion a realizar cuando se cierre el dialogo</param>
    public void ShowJugadorDesbloqueado(Jugador _jugador, btnButton.guiAction _onCloseCallback = null) {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        // textura del jugador
        if (InfoJugadores.instance.EsLanzador(_jugador.assetName))
            m_imgAvatar.texture = AvataresManager.instance.GetTexturaLanzador(_jugador.idModelo);
        else
            m_imgAvatar.texture = AvataresManager.instance.GetTexturaPortero(_jugador.idModelo);
        ShowImagen(m_imgAvatar);

        // textos
        m_txtTitulo.text = LocalizacionManager.instance.GetTexto(254).ToUpper();
        m_txtTituloSombra.text = m_txtTitulo.text;
        m_txtTexto.text = string.Format(LocalizacionManager.instance.GetTexto(255), "<color=#ddf108>" + _jugador.nombre + "</color>");

        // boton cancelar => ocultarlo
        m_btnCancelar.gameObject.SetActive(false);

        // mostrar este dialogo
        transform.gameObject.SetActive(true);
        MostrarDialogoConSuperTweener(_onCloseCallback);
    }


    /// <summary>
    /// Muestra la informacion asociada a una equipacion
    /// </summary>
    /// <param name="_equipacion"></param>
    /// <param name="_onCloseCallback">Accion a realizar cuando se cierre el dialogo</param>
    public void ShowEquipacionDesbloqueada(Equipacion _equipacion, btnButton.guiAction _onCloseCallback = null) {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        // textura de la equipacion
        if (EquipacionManager.instance.EsEquipacionDeLanzador(_equipacion.assetName))
            m_imgAvatar.texture = AvataresManager.instance.GetTexturaAvatarEquipacionesLanzador(_equipacion.assetName);
        else
            m_imgAvatar.texture = AvataresManager.instance.GetTexturaAvatarEquipacionesPortero(_equipacion.assetName);
        ShowImagen(m_imgAvatar);

        // textos
        m_txtTitulo.text = LocalizacionManager.instance.GetTexto(254).ToUpper();
        m_txtTituloSombra.text = m_txtTitulo.text;
        m_txtTexto.text = string.Format(LocalizacionManager.instance.GetTexto(255), "<color=#ddf108>" + LocalizacionManager.instance.GetTexto(256) + "</color>");

        // boton cancelar => ocultarlo
        m_btnCancelar.gameObject.SetActive(false);

        // mostrar este dialogo
        transform.gameObject.SetActive(true);
        MostrarDialogoConSuperTweener(_onCloseCallback);
    }


    /// <summary>
    /// Muestra la informacion asociada a un escudo
    /// </summary>
    /// <param name="_escudo"></param>
    /// <param name="_onCloseCallback"></param>
    public void ShowEscudoDesbloqueado(Escudo _escudo, btnButton.guiAction _onCloseCallback = null) {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        // textura del boton
        m_imgEscudo.texture = AvataresManager.instance.GetTexturasEscudosDesbloqueados(_escudo.idTextura);
        ShowImagen(m_imgEscudo);

        // textos
        m_txtTitulo.text = LocalizacionManager.instance.GetTexto(254).ToUpper();
        m_txtTituloSombra.text = m_txtTitulo.text;
        m_txtTexto.text = string.Format(LocalizacionManager.instance.GetTexto(255), "<color=#ddf108>" + LocalizacionManager.instance.GetTexto(257) + "</color>");
        m_txtEscudoBoost.text = _escudo.boost.ToString("f1");

        // boton cancelar => ocultarlo
        m_btnCancelar.gameObject.SetActive(false);

        // mostrar este dialogo
        transform.gameObject.SetActive(true);
        MostrarDialogoConSuperTweener(_onCloseCallback);
    }


    /// <summary>
    /// Muestra la informacion asociada a un logro
    /// </summary>
    /// <param name="_idLogroDesbloqueado"></param>
    /// <param name="_onCloseCallback"></param>
    public void ShowLogroDesbloqueado(string _idLogroDesbloqueado, btnButton.guiAction _onCloseCallback = null) {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        // obtener la informacion del logro y del grupo al que pertenece
        GrupoLogros grupoContenedorLogroDesbloqueado = null;
        Logro logroDesbloqueado = LogrosManager.instance.GetLogro(_idLogroDesbloqueado, ref grupoContenedorLogroDesbloqueado);

        // textura de la moneda
        m_imgCash.texture = m_textureRewardHardCash;
        ShowImagen(m_imgCash);

        // textos
        m_txtTitulo.text = string.Format(LocalizacionManager.instance.GetTexto(287), "<color=#ddf108>" + grupoContenedorLogroDesbloqueado.nombre + "</color>").ToUpper();
        m_txtTituloSombra.text = string.Format(LocalizacionManager.instance.GetTexto(287), grupoContenedorLogroDesbloqueado.nombre).ToUpper();
        m_txtTexto.text = string.Format(grupoContenedorLogroDesbloqueado.descriptionConFormato, "<color=#ddf108>" + logroDesbloqueado.valorSuperarLogro + "</color>");
        m_txtCantidadCash.text = logroDesbloqueado.recompensa.ToString();
        m_txtCantidadCashSombra.text = m_txtCantidadCash.text;

        // boton cancelar => ocultarlo
        m_btnCancelar.gameObject.SetActive(false);

        // mostrar este dialogo
        transform.gameObject.SetActive(true);
        MostrarDialogoConSuperTweener(_onCloseCallback);
    }


    /// <summary>
    /// Muestra una imagen en concreto de este control y oculta el resto
    /// </summary>
    /// <param name="_img"></param>
    private void ShowImagen(GUITexture _img) {
        // ocultar el resto de imagenes
        m_imgAvatar.gameObject.SetActive(false);
        m_imgEscudo.gameObject.SetActive(false);
        m_imgCash.gameObject.SetActive(false);

        // mostrar la imagen recibida como parametro
        if (_img != null)
            _img.gameObject.SetActive(true);
    }


    /// <summary>
    /// Aplica un supertweener a este dialogo para que aparezca desde fuera de la pantalla
    /// </summary>
    /// <param name="_onCloseCallback">Accion a realizar cuando se cierre el dialogo</param>
    private void MostrarDialogoConSuperTweener(btnButton.guiAction _onCloseCallback = null) {
        // colocar el dialogo por fuera de la pantalla y hacerlo aparecer con un supertweener
        m_caja.transform.position = new Vector3(-0.5f, 0.5f, 200.0f);
        new SuperTweener.move(m_caja.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 200.0f), null,
            // callback para cuando el supertweener termine
            (_name) => {
                InicializarBotonesCerrarDialogo(_onCloseCallback);
            });
    }


    /// <summary>
    /// Inicializa los botones para cerrar el dialogo
    /// </summary>
    /// <param name="_onCloseCallback"></param>
    private void InicializarBotonesCerrarDialogo(btnButton.guiAction _onCloseCallback = null) {
        // boton aceptar
        m_btnAceptar.action = (_name) => {
            // ocultar formulario
            transform.gameObject.SetActive(false);

            // ejecutar la accion asociada
            if (_onCloseCallback != null)
                _onCloseCallback("");
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
            Interfaz.ClickFX();
        };

        // velo (funciona como el boton aceptar)
        m_velo.action = (_name) => {
            // ocultar formulario
            transform.gameObject.SetActive(false);

            // ejecutar la accion asociada
            if (_onCloseCallback != null)
                _onCloseCallback("");
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
        };
    }
}
