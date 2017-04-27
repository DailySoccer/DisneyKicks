using UnityEngine;
using System.Collections;

/// <summary>
/// Clase para generar una caja de dialogo generica
/// </summary>
public class ifcDialogBox: ifcBase {


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Enumerado para indexar las texturas de los botones de este dialogo
    /// </summary>
    public enum IdTexturas { POSITIVE = 0, COINS = 1, BITOONS = 2, NEGATIVE = 3, EQUIPAR = 4 };

    /// <summary>
    /// Enumerado con los tipos de dialogo de un unico boton
    /// </summary>
    public enum OneButtonType { POSITIVE, COINS, BITOONS };

    /// <summary>
    /// Enumerado con los tipos de dialogo de dos botones
    /// </summary>
    public enum TwoButtonType { POSITIVE_NEGATIVE, COINS_BITOONS, BITOONS_EQUIPAR, BITOONS_DESEQUIPAR };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    static ifcDialogBox m_instance;
    public static ifcDialogBox instance { get{return m_instance;} protected set{if(m_instance == null)m_instance = value; else GameObject.Destroy(value.gameObject);} }

    // texturas para los elementos de esta interfaz
    // NOTA: asignarles valor desde la interfaz de Unity
    public Texture[] m_texturasBtnIcon;
    public Texture[] m_texturasBtnSingle;
    public Texture[] m_texturasBtnIzdo;
    public Texture[] m_texturasBtnDcho;

    // fuente a utilizar en la caja de texto
    public Font m_fuenteCajaTextInput;

    // elementos graficos del dialogo
    private GUIText m_titulo;
    private GUIText m_tituloSombra;
    private GUIText m_texto;
    private btnButton m_btnCerrar;      // aspa para cerrar el dialogo
    private btnButton m_btnSingle;      // boton cuando el dialogo tiene un unico boton
    private btnButton m_btnIzdo;        // boton izquierdo cuando el dialogo tiene mas de un boton
    private btnButton m_btnDcho;        // boton derecho cuando el dialogo tiene mas de un boton
    private btnButton m_btnVelo;        // boton para el velo
    private GUIText m_cronometro;

    
    private btnButton m_btnAceptar;
    
    private btnButton m_btnExtra;
    private GUIText m_txtBtnAceptar;
    private GUIText m_txtBtnExtra;

    // variables para controlar el tiempo que se esta mostrando este dialogo
    private float m_timeLimit = 0.0f;
    private float m_tiempoMostrandoDialogo = 0.0f;
    private btnButton.guiAction m_timeLimitCallback = null;

    // texto editado en este control cuando se abre se muestra en modo "ShowTextInput"
    public string textoEditado { get { return m_textoEditado; } }
    private string m_textoEditado = "";

    // indica si actualmente se esta mostrando la caja para introducir texto
    private bool m_mostrandoTextInput = false;

    // estilos para mostrar la caja para introducir texto
    private GUIStyle m_estiloTextInput = null;
    private Rect m_rectanguloCajaTextInput;

    //interfaz subyacente
    private ifcBase m_interfazAnterior;

    // tipo de botones actual
    //private TipoBotones m_tipoBotones;

    // valor de la coordenada "y" del texto cuando no aparece el texto asociado al cronometro
    private float m_yTextoSinCrono{get{ return ( -57.0f * ifcBase.scaleFactor);}set{}}
    private float m_yTextoConCrono{get{ return ( -35.0f * ifcBase.scaleFactor);}set{}}


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        instance = this;
        GameObject.DontDestroyOnLoad(transform.gameObject);
    }


    void Start() {
        Transform cajaTransform = transform.FindChild("caja");

        this.m_backMethod = Back;

        // obtener las referencias a los elementos de la interfaz
        m_titulo = cajaTransform.FindChild("titulo").GetComponent<GUIText>();
        m_tituloSombra = cajaTransform.FindChild("tituloSombra").GetComponent<GUIText>();
        m_texto = cajaTransform.FindChild("texto").GetComponent<GUIText>();
        m_btnCerrar = cajaTransform.FindChild("btnCerrar").gameObject.GetComponent<btnButton>();
        m_btnSingle = cajaTransform.FindChild("btnSingle").GetComponent<btnButton>();
        m_btnIzdo = cajaTransform.FindChild("btnIzdo").GetComponent<btnButton>();
        m_btnDcho = cajaTransform.FindChild("btnDcho").GetComponent<btnButton>();
        m_btnVelo = transform.FindChild("velo").GetComponent<btnButton>();
        m_cronometro = cajaTransform.FindChild("cronometro").GetComponent<GUIText>();

        // obtener la posicion por defecto del texto (sin cronometro)
        m_yTextoSinCrono = m_texto.pixelOffset.y;

        // reescalar 
        ifcBase.Scale(gameObject);

        // ocultar este control
        this.gameObject.SetActive(false);
    }


    void Update() {
        // comprobar si el formulario tiene limite de tiempo
        if (m_timeLimit > 0.0f) {
            m_tiempoMostrandoDialogo += Time.deltaTime;

            // actualizar el cronometro
            int segundosRestantes = (int)Mathf.Max((m_timeLimit - m_tiempoMostrandoDialogo), 0.0f) + 1;
            m_cronometro.text = (segundosRestantes / 60).ToString("D2") + ":" + (segundosRestantes % 60).ToString("D2");

            // si se supera el limite de tiempo ejecutar la accion correspondiente
            if (m_tiempoMostrandoDialogo > m_timeLimit) {
                gameObject.SetActive(false);

                // ejecutar el callback correspondiente
                if (m_timeLimitCallback != null)
                    m_timeLimitCallback("");
            }
        }
    }

    /// <summary>
    /// Muestra un cuadro de input de texto
    /// </summary>
    public void ShowTextInput(string _textoInicial = "")
    {
        m_textoEditado = _textoInicial;
        m_mostrandoTextInput = true;
        m_texto.pixelOffset = new Vector2(m_texto.pixelOffset.x, m_yTextoConCrono);     // posicionar el texto a la altura que le corresponde (con input)
    }


    /// <summary>
    /// Actualiza el estado de la caja para introducir texto
    /// </summary>
    void OnGUI() {
        if (instance.enabled) {
            // si hay que mostrar el textarea
            if (m_mostrandoTextInput) {
                // si aun no se ha definido el estilo del textarea
                if (m_estiloTextInput == null) {
                    // definir la caja para el textarea
                    float anchoCaja = 200 * ifcBase.scaleFactor;
                    float altoCaja = 60 * ifcBase.scaleFactor;
                    m_rectanguloCajaTextInput = new Rect((Screen.width - anchoCaja) / 2, (Screen.height - altoCaja) / 2 - (10 * scaleFactor), anchoCaja, altoCaja);

                    // definir el estilo del textarea
                    m_estiloTextInput = new GUIStyle(GUI.skin.textArea);
                    m_estiloTextInput.alignment = TextAnchor.MiddleCenter;
                    m_estiloTextInput.font = m_fuenteCajaTextInput;
                    m_estiloTextInput.normal.textColor = new Color(1f, 1f,1f, 1f);
                    m_estiloTextInput.fontSize = Mathf.RoundToInt(34 * ifcBase.scaleFactor);
                    m_estiloTextInput.wordWrap = false;                   
                }

                // Mostrar el texarea
                GUI.SetNextControlName("DialogInputText");
                m_textoEditado = GUI.TextArea(m_rectanguloCajaTextInput, m_textoEditado, 12, m_estiloTextInput);
                try {
                    // pasar el foco a la caja de texto
                    GUI.FocusControl("DialogInputText");    // NOTA: Esta linea genera las alertas de error: "!dest.m_MultiFrameGUIState.m_NamedKeyControlList" pero esto no afecta al funcionamiento de la aplicacion
                    // para mas info sobre el error: http://forum.unity3d.com/threads/158676-!dest-m_MultiFrameGUIState-m_NamedKeyControlList
                } catch { };
            }
        }
    }


    /// <summary>
    /// Muestra un dialogo sin botones (sin contar el de cerrar)
    /// </summary>
    /// <param name="_titulo"></param>
    /// <param name="_texto"></param>
    /// <param name="_mostrarCerrar"></param>
    /// <param name="_onCerrarCallback"></param>
    public void ShowZeroButtonDialog(string _titulo, string _texto, bool _mostrarCerrar = false, btnButton.guiAction _onCerrarCallback = null, bool _mute = false) {
        // inicializar los botones del dialogo
        m_btnSingle.gameObject.SetActive(false);
        m_btnIzdo.gameObject.SetActive(false);
        m_btnDcho.gameObject.SetActive(false);

        // inicializar las partes comunes a todos los dialogos
        GenericShowDialog(_titulo, _texto, _mostrarCerrar, _onCerrarCallback, _mute);
    }


    /// <summary>
    /// Muestra un dialogo con un solo boton (sin contar el de cerrar)
    /// </summary>
    /// <param name="_oneButtonType"></param>
    /// <param name="_titulo"></param>
    /// <param name="_texto"></param>
    /// <param name="_textoBoton"></param>
    /// <param name="_onBotonCallback"></param>
    /// <param name="_mostrarCerrar"></param>
    /// <param name="_onCerrarCallback"></param>
    public void ShowOneButtonDialog(OneButtonType _oneButtonType, string _titulo, string _texto, string _textoBoton, btnButton.guiAction _onBotonCallback = null, bool _mostrarCerrar = false, btnButton.guiAction _onCerrarCallback = null, bool _mute = false) {
        // calcular el id de las texturas del boton
        int idTexturaBoton = 0;
        int idTexturaIconoBoton = 0;
        switch (_oneButtonType) {
            case OneButtonType.POSITIVE:
                idTexturaBoton = (int) IdTexturas.POSITIVE;
                idTexturaIconoBoton = idTexturaBoton;
                break;
            case OneButtonType.COINS:
                idTexturaBoton = (int) IdTexturas.COINS;
                idTexturaIconoBoton = idTexturaBoton;
                break;
            case OneButtonType.BITOONS:
                idTexturaBoton = (int) IdTexturas.BITOONS;
                idTexturaIconoBoton = idTexturaBoton;
                break;
        }
        
        // inicializar los botones del dialogo
        ShowButton(m_btnSingle, m_texturasBtnSingle[idTexturaBoton], _textoBoton, _onBotonCallback, m_texturasBtnIcon[idTexturaIconoBoton], _mute);
        m_btnSingle.gameObject.SetActive(true);
        m_btnIzdo.gameObject.SetActive(false);
        m_btnDcho.gameObject.SetActive(false);

        // inicializar las partes comunes a todos los dialogos
        GenericShowDialog(_titulo, _texto, _mostrarCerrar, _onCerrarCallback, _mute);

    }


    /// <summary>
    /// Muestra un dialogo de dos botones (sin contar el de cerrar)
    /// </summary>
    /// <param name="_twoButtonType"></param>
    /// <param name="_titulo"></param>
    /// <param name="_texto"></param>
    /// <param name="_textoBotonIzdo"></param>
    /// <param name="_textoBotonDcho"></param>
    /// <param name="_onBotonIzdoCallback"></param>
    /// <param name="_onBotonDchoCallback"></param>
    /// <param name="_mostrarCerrar"></param>
    /// <param name="_onCerrarCallback"></param>
    public void ShowTwoButtonDialog(TwoButtonType _twoButtonType, string _titulo, string _texto, string _textoBotonIzdo, string _textoBotonDcho, btnButton.guiAction _onBotonIzdoCallback = null, btnButton.guiAction _onBotonDchoCallback = null, bool _mostrarCerrar = false, btnButton.guiAction _onCerrarCallback = null, bool _mute = false) {
        // calcular el id de las texturas de los botones
        int idTexturasBotonIzdo = 0;
        int idTexturasBotonDcho = 0;
        int idTexturaIconoBtnIzdo = 0;
        int idTexturaIconoBtnDcho = 0;
        switch (_twoButtonType) {
            case TwoButtonType.POSITIVE_NEGATIVE:
                idTexturasBotonIzdo = (int)IdTexturas.POSITIVE;
                idTexturasBotonDcho = (int)IdTexturas.NEGATIVE;
                idTexturaIconoBtnIzdo = idTexturasBotonIzdo;
                idTexturaIconoBtnDcho = idTexturasBotonDcho;
                break;
            case TwoButtonType.COINS_BITOONS:
                idTexturasBotonIzdo = (int) IdTexturas.COINS;
                idTexturasBotonDcho = (int) IdTexturas.BITOONS;
                idTexturaIconoBtnIzdo = idTexturasBotonIzdo;
                idTexturaIconoBtnDcho = idTexturasBotonDcho;
                break;
            case TwoButtonType.BITOONS_EQUIPAR:
                idTexturasBotonIzdo = (int) IdTexturas.BITOONS;
                idTexturasBotonDcho = (int) IdTexturas.EQUIPAR;
                idTexturaIconoBtnIzdo = idTexturasBotonIzdo;
                idTexturaIconoBtnDcho = idTexturasBotonDcho;
                break;
            case TwoButtonType.BITOONS_DESEQUIPAR:
                idTexturasBotonIzdo = (int) IdTexturas.BITOONS;
                idTexturasBotonDcho = (int) IdTexturas.NEGATIVE;
                idTexturaIconoBtnIzdo = idTexturasBotonIzdo;
                idTexturaIconoBtnDcho = (int) IdTexturas.EQUIPAR;
                break;
        }

        // inicializa los botones del dialogo
        ShowButton(m_btnIzdo, m_texturasBtnIzdo[idTexturasBotonIzdo], _textoBotonIzdo, _onBotonIzdoCallback, m_texturasBtnIcon[idTexturaIconoBtnIzdo], _mute);
        ShowButton(m_btnDcho, m_texturasBtnDcho[idTexturasBotonDcho], _textoBotonDcho, _onBotonDchoCallback, m_texturasBtnIcon[idTexturaIconoBtnDcho], _mute);
        m_btnSingle.gameObject.SetActive(false);
        m_btnIzdo.gameObject.SetActive(true);
        m_btnDcho.gameObject.SetActive(true);

        // inicializar las partes comunes a todos los dialogos
        GenericShowDialog(_titulo, _texto, _mostrarCerrar, _onCerrarCallback, _mute);

    }


    /// <summary>
    /// Hace que si el usuario no pulsa ningun boton del dialogo, transcurridos "_timeLimit" segundos este se cierre solo (y ejecute el callback "_timeLimitCallBack")
    /// NOTA: llamar a esta funcion despues de la llamada "Show" correspondiente (si se hace despues "Show" la anulara)
    /// NOTA: el tiempo debe ser mayor que "0.0f"
    /// </summary>
    /// <param name="_timeLimit"></param>
    /// <param name="_timeLimitCallBack"></param>
    public void WaitToCloseAutomatically(float _timeLimit = 0.0f, btnButton.guiAction _timeLimitCallBack = null) {
        // comprobar si el formulario tiene limite de tiempo
        if (_timeLimit > 0.0f) {
            m_timeLimit = _timeLimit;
            m_tiempoMostrandoDialogo = 0.0f;
            m_timeLimitCallback = _timeLimitCallBack;

            // mostrar el cronometro
            m_cronometro.text = ((int) _timeLimit).ToString();
            m_cronometro.gameObject.SetActive(true);
            m_texto.pixelOffset = new Vector2(m_texto.pixelOffset.x, m_yTextoConCrono);     // posicionar el texto a la altura que le corresponde (con crono)
        } else {
            m_timeLimit = 0.0f;
            m_cronometro.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Metodo generico para inicializar un boton
    /// </summary>
    /// <param name="_boton"></param>
    /// <param name="_textura"></param>
    /// <param name="_texto"></param>
    /// <param name="_onCallback"></param>
    /// <param name="_texturaIcono"></param>
    private void ShowButton(btnButton _boton, Texture _textura, string _texto, btnButton.guiAction _onCallback = null, Texture _texturaIcono = null, bool _mute = false) {
        if (_boton != null) {
            // texto del boton
            _boton.transform.FindChild("texto").GetComponent<GUIText>().text = _texto;
            _boton.transform.FindChild("textoSombra").GetComponent<GUIText>().text = _texto;

            // asignar la textura de fondo del boton
            _boton.GetComponent<GUITexture>().texture = _textura;
            _boton.m_current = _textura;

            // asignar la textura del icono del boton
            _boton.transform.FindChild("icono").GetComponent<GUITexture>().texture = _texturaIcono;

            // callback del boton
            _boton.action = (_name) => {
                Interfaz.ClickFX();

                // indicar que ya no hay cierre de este dialogo por tiempo
                m_timeLimit = 0.0f;
                m_timeLimitCallback = null;

                // ocultar este control
                Hide(_mute);

                // ejecutar la accion asociada
                if (_onCallback != null)
                _onCallback(_name);
            };

            // mostrarlo
            _boton.gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// Inicializa y muestra las partes comunes a todos los tipos de dialogo
    /// </summary>
    /// <param name="_titulo"></param>
    /// <param name="_texto"></param>
    private void GenericShowDialog(string _titulo, string _texto, bool _mostrarCerrar = false, btnButton.guiAction _onCerrarCallback = null, bool _mute = false) {
        //guardar la interfaz actual para recuperarla al cerrar el dialogo
        m_interfazAnterior = ifcBase.activeIface;

        // indicar que no hay cierre de este dialogo por tiempo
        m_timeLimit = 0.0f;
        m_timeLimitCallback = null;

        // titulo
        m_titulo.text = _titulo.ToUpper();
        m_tituloSombra.text = m_titulo.text;

        // texto del control
        m_texto.text = _texto;
        m_texto.GetComponent<txtText>().Fix();  // <= ajustar los retornos de carro
        m_texto.pixelOffset = new Vector2(m_texto.pixelOffset.x, m_yTextoSinCrono);     // posicionar el texto a la altura que le corresponde (sin crono)

        // inicializar el boton "Cerrar"
        if (_mostrarCerrar) {
            m_btnCerrar.action = (_name)=>{
                // hacer sonar el click y ocultar este dialogo
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);

                // indicar que ya no hay cierre de este dialogo por tiempo
                m_timeLimit = 0.0f;
                m_timeLimitCallback = null;

                // ocultar este control
                gameObject.SetActive(false);

                // ejecutar el callback correspondiente
                if (_onCerrarCallback != null)
                    _onCerrarCallback(_name);
            };

            m_btnVelo.action = (_name)=>{
                // hacer sonar el click y ocultar este dialogo
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);

                // indicar que ya no hay cierre de este dialogo por tiempo
                m_timeLimit = 0.0f;
                m_timeLimitCallback = null;

                // ocultar este control
                gameObject.SetActive(false);

                // ejecutar el callback correspondiente
                if (_onCerrarCallback != null)
                    _onCerrarCallback(_name);
            };

            m_btnCerrar.gameObject.SetActive(true);
        } else {
            m_btnCerrar.gameObject.SetActive(false);
            m_btnVelo.action = null;
        }

        // ocultar el cronometro
        m_cronometro.gameObject.SetActive(false);

        // ocultar la caja de texto
        m_mostrandoTextInput = false;

        // mostrar este control
        this.gameObject.SetActive(true);

        if(!_mute) GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOnClip);
    }

    /// <summary>
    /// Callback para el boton cerrar y velo
    /// </summary>
    void AccionCerrar(btnButton.guiAction _onCerrarCallback = null) {

    }


    /// <summary>
    /// Oculta el control
    /// </summary>
    public void Hide(bool _mute = false) {

        // indicar que actualmente ya no se esta mostrando la caja de texto
        m_mostrandoTextInput = true;
        ifcBase.activeIface = m_interfazAnterior;

        gameObject.SetActive(false);
        if(!_mute) GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
    }


    /// <summary>
    /// Metodo para tratar el evento de pulsar "BACK" en el dispositivo
    /// </summary>
    /// <param name="_input"></param>
    void Back(string _input) {

        if (m_btnCerrar.gameObject.activeInHierarchy) { // <= si el formulario tiene boton de cerrar visible
            Hide();
        }
    }



    /*
    /// <summary>
    /// Muestra el dialogo con las opciones especificadas
    /// </summary>
    /// <param name="_botones">Tipo de botones a mostrar</param>
    /// <param name="_titulo">Titulo</param>
    /// <param name="_texto">Texto descriptivo</param>
    /// <param name="_onAceptarCallback">Accion a realizar si el usuario pulsa el boton "ACEPTAR"</param>
    /// <param name="_onCerrarCallback">Accion a realizar si el usuario pulsa el boton "CERRAR" (el aspa "X" de la esquina superior derecha)</param>
    /// <param name="_onExtraCallback">Accion a realizar si el usuario pulsa el boton "EXTRA"</param>
    /// <param name="_txtBtnAceptar">Texto a mostrar sobre el boton "ACEPTAR"</param>
    /// <param name="_txtBtnExtra">Texto a mostrar sobre el boton "EXTRA"</param>
    /// <param name="_timeLimit">Indica si el dialogo se muestra durante un periodo de tiempo limitado</param>
    /// <param name="_timeLimitCallBack">Accion a realizar si vence el _timeLimit</param>
    /// <param name="_extraBtnEnabled">Habilita o deshabilita el boton "EXTRA"</param>
    public void Show(TipoBotones _botones, string _titulo, string _texto,
        btnButton.guiAction _onAceptarCallback = null, btnButton.guiAction _onCerrarCallback = null, btnButton.guiAction _onExtraCallback = null,
        string _txtBtnAceptar = "ACEPTAR", string _txtBtnExtra = "EXTRA",
        float _timeLimit = 0.0f, btnButton.guiAction _timeLimitCallBack = null, bool _extraBtnEnabled = true) {

        m_tipoBotones = _botones;

        //guardar la interfaz anterior para recuperarla
        m_interfazAnterior = ifcBase.activeIface;

        // incializar los textos
        m_titulo.text = _titulo.ToUpper();
        m_tituloSombra.text = m_titulo.text;

        m_texto.text = _texto;
        m_texto.GetComponent<txtText>().Fix();  // <= ajustar los retornos de carro

        // incializar los botones
        ShowButtons(_botones, _onAceptarCallback, _onCerrarCallback, _onExtraCallback, _txtBtnAceptar, _txtBtnExtra, _extraBtnEnabled);

        // comprobar si el formulario tiene limite de tiempo
        if (_timeLimit > 0.0f) {
            m_timeLimit = _timeLimit;
            m_tiempoMostrandoDialogo = 0.0f;
            m_timeLimitCallback = _timeLimitCallBack;

            // mostrar el cronometro
            m_cronometro.text = ((int) _timeLimit).ToString();
            m_cronometro.gameObject.SetActive(true);
        } else {
            m_timeLimit = 0.0f;
            m_cronometro.gameObject.SetActive(false);
        }

        // ocultar la caja de texto
        m_mostrandoTextInput = false;

        // mostrar este control
        this.gameObject.SetActive(true);
    }

    
    /// <summary>
    /// Muestra el dialogo con un campo para introducir texto con las opciones especificadas
    /// </summary>
    /// <param name="_botones">Tipo de botones a mostrar</param>
    /// <param name="_titulo">Titulo</param>
    /// <param name="_texto">Texto descriptivo</param>
    /// <param name="_onAceptarCallback">Accion a realizar si el usuario pulsa el boton "ACEPTAR"</param>
    /// <param name="_onCerrarCallback">Accion a realizar si el usuario pulsa el boton "CERRAR" (el aspa "X" de la esquina superior derecha)</param>
    /// <param name="_onExtraCallback">Accion a realizar si el usuario pulsa el boton "EXTRA"</param>
    /// <param name="_txtBtnAceptar">Texto a mostrar sobre el boton "ACEPTAR"</param>
    /// <param name="_txtBtnExtra">Texto a mostrar sobre el boton "EXTRA"</param>
    /// <param name="_timeLimit">Indica si el dialogo se muestra durante un periodo de tiempo limitado</param>
    /// <param name="_timeLimitCallBack">Accion a realizar si vence el _timeLimit</param>
    /// <param name="_extraBtnEnabled">Habilita o deshabilita el boton "EXTRA"</param>
    public void ShowTextInput(TipoBotones _botones, string _titulo, string _texto,
        btnButton.guiAction _onAceptarCallback = null, btnButton.guiAction _onCerrarCallback = null, btnButton.guiAction _onExtraCallback = null,
        string _txtBtnAceptar = "ACEPTAR", string _txtBtnExtra = "EXTRA",
        float _timeLimit = 0.0f, btnButton.guiAction _timeLimitCallBack = null, bool _extraBtnEnabled = true) {
        // por defecto limpiar el campo para introducir texto
        m_textoEditado = "";

        // mostrar el formulario
        this.Show(_botones, _titulo, _texto, _onAceptarCallback, _onCerrarCallback, _onExtraCallback, _txtBtnAceptar, _txtBtnExtra, _timeLimit, _timeLimitCallBack);

        // indicar que actualmente se esta mostrando la caja de texto
        m_mostrandoTextInput = true;
    }
    

    /// <summary>
    /// Actualiza el estado de los botones del dialogo
    /// </summary>
    /// <param name="_tipoBotones"></param>
    /// <param name="_onAceptarCallBack"></param>
    /// <param name="_onCerrarCallBack"></param>
    /// <param name="_onExtraCallBack"></param>
    /// <param name="textoBtn1"></param>
    /// <param name="textoBtn2"></param>
    /// <param name="_extraBtnEnabled"></param>
    private void ShowButtons(TipoBotones _tipoBotones,
        btnButton.guiAction _onAceptarCallBack = null, btnButton.guiAction _onCerrarCallBack = null, btnButton.guiAction _onExtraCallBack = null,
        string _textoBtnAceptar = "ACEPTAR", string _textoBtnExtra = "EXTRA", bool _extraBtnEnabled = true) {

        // inicializar el boton "Aceptar"
        if (_tipoBotones != TipoBotones.NONE && _tipoBotones != TipoBotones.CERRAR) {
            m_btnAceptar.action = (_name) => {
                // hacer sonar el click y ocultar este dialogo
                //Interfaz.ClickFX();
                //GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.confirmClip);

                gameObject.SetActive(false);

                // ejecutar el callback correspondiente
                if (_onAceptarCallBack != null)
                    _onAceptarCallBack(_name);
            };
            m_btnAceptar.gameObject.SetActive(true);
            m_txtBtnAceptar.text = _textoBtnAceptar;
        } else
            m_btnAceptar.gameObject.SetActive(false);

        // inicializar el boton "Cerrar"
        if (_tipoBotones != TipoBotones.NONE) {
            m_btnCerrar.action = (_name) => {
                // hacer sonar el click y ocultar este dialogo
                GeneralSounds_menu.instance.back();
                gameObject.SetActive(false);

                // ejecutar el callback correspondiente
                if (_onCerrarCallBack != null)
                    _onCerrarCallBack(_name);
            };
            m_btnCerrar.gameObject.SetActive(true);
        } else
            m_btnCerrar.gameObject.SetActive(false);

        // inicializar el boton "Extra"
        if (_tipoBotones == TipoBotones.ACEPTAR_CERRAR_EXTRA) {
            m_btnExtra.action = (_name) => {
                // hacer sonar el click y ocultar este dialogo
                gameObject.SetActive(false);

                // ejecutar el callback correspondiente
                if (_onExtraCallBack != null)
                    _onExtraCallBack(_name);
            };
            m_btnExtra.gameObject.SetActive(true);
            m_txtBtnExtra.text = _textoBtnExtra;

            // habilitar o deshabilitar el boton "EXTRA"
            m_btnExtra.SetEnabled(_extraBtnEnabled);
        } else
            m_btnExtra.gameObject.SetActive(false);

        // posicionar los botones
        if (_tipoBotones == TipoBotones.ACEPTAR_CERRAR_EXTRA) {
            // si se muestran dos botones => posicionar los botones "aceptar" y "extra"
            m_btnAceptar.transform.localPosition = new Vector3(-0.13f, -0.11f, m_btnAceptar.transform.localPosition.z);
            m_btnExtra.transform.localPosition = new Vector3(0.13f, -0.11f, m_btnExtra.transform.localPosition.z);
        } else {
            // si se muestra solo un boton => centrar el boton "aceptar"
            m_btnAceptar.transform.localPosition = new Vector3(0.0f, -0.11f, m_btnAceptar.transform.localPosition.z);
        }

    }
    */

}
