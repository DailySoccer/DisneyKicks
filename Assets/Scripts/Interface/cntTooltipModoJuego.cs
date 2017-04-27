using UnityEngine;
using System.Collections;


/// <summary>
/// DEPRECATED: Control para mostrar un tooltip sobre un boton de modo de juego
/// </summary>
public class cntTooltipModoJuego : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static cntTooltipModoJuego instance { get { return m_instance; } }
    private static cntTooltipModoJuego m_instance;

    // texturas para el fondo de este control
    // NOTA: asignarles valor desde la interfaz de unity
    public Texture2D m_fondo_izda;
    public Texture2D m_fondo_dcha;

    // elementos de esta interfaz
    private btnButton m_boton;
    private GUIText m_txtTitulo;
    private GUIText m_txtTexto;
    private GUITexture m_fondo;

    // tiempo que se esta mostrando este tooltip
    private float m_tiempoRestanteMostrarTooltip;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    public void Start() {
        // obtener las referencias a los controles de esta interfaz
        m_boton = transform.FindChild("btnDesbloquear").GetComponent<btnButton>();
        m_txtTitulo = transform.FindChild("titulo").GetComponent<GUIText>();
        m_txtTexto= transform.FindChild("texto").GetComponent<GUIText>();
        m_fondo = transform.FindChild("fondo").GetComponent<GUITexture>();

        // por defecto ocultar este control
        transform.gameObject.SetActive(false);
    }


    /// <summary>
    /// Metodo para mostrar el tooltip "modo de juego disponible"
    /// </summary>
    /// <param name="_posicion"></param>
    /// <param name="_modoJuego"></param>
    /// <param name="_pintarPorDerecha">Indica si el popup debe pintarse hacia la derecha o la izquierda de la posicion indicada</param>
    public void ShowModoDisponible(Vector3 _posicion, ModoJuego _modoJuego, bool _pintarPorDerecha) {
        // reposisicionar los elementos de la interfaz
        PosicionarElementos(_posicion, _pintarPorDerecha);

        // actualizar los textos
        m_txtTitulo.text = "¡MODO DISPONIBLE!";
        m_txtTexto.text = "<color=#000000>ACTÍVALO POR " + _modoJuego.precioDesbloqueo + "$</color>";
        
        // mostrar el boton
        m_boton.gameObject.SetActive(true);
        m_boton.action = (_name) => {
            //Interfaz.instance.comprarModoJuego(_modoJuego);
        };
        
        // mostrar este control
        transform.gameObject.SetActive(true);

        // inicializar la opacidad de todos los elementos de este control al maximo
        m_boton.GetComponent<GUITexture>().color = new Color(m_boton.GetComponent<GUITexture>().color.r, m_boton.GetComponent<GUITexture>().color.g, m_boton.GetComponent<GUITexture>().color.b, 1.0f);
        m_txtTitulo.color = new Color(m_txtTitulo.color.r, m_txtTitulo.color.g, m_txtTitulo.color.b, 1.0f);
        m_txtTexto.color = new Color(m_txtTexto.color.r, m_txtTexto.color.g, m_txtTexto.color.b, 1.0f);

        // inicializar el tiempo
        m_tiempoRestanteMostrarTooltip = Stats.TIEMPO_TOOLTIP_MODO_JUEGO;
    }


    /// <summary>
    /// Metodo para mostrar el tooltip "consigue el logro '_nombreLogro' para desbloquear este modo"
    /// </summary>
    /// <param name="_posicion"></param>
    /// <param name="_nombreLogro"></param>
    /// <param name="_pintarPorDerecha">Indica si el popup debe pintarse hacia la derecha o la izquierda de la posicion indicada</param>
    public void ShowBloqueado(Vector3 _posicion, string _nombreLogro, bool _pintarPorDerecha) {
        // reposisicionar los elementos de la interfaz
        PosicionarElementos(_posicion, _pintarPorDerecha);

        // actualizar los textos
        m_txtTitulo.text = "¡MODO BLOQUEADO!";
        m_txtTexto.text = "Consigue el logro <color=#ffd200>\"" + _nombreLogro + "\"</color>\n para obtener este modo.";

        // mostrar el boton
        m_boton.gameObject.SetActive(false);

        // inicializar la opacidad de todos los elementos de este control al maximo
        m_boton.GetComponent<GUITexture>().color = new Color(m_boton.GetComponent<GUITexture>().color.r, m_boton.GetComponent<GUITexture>().color.g, m_boton.GetComponent<GUITexture>().color.b, 1.0f);
        m_txtTitulo.color = new Color(m_txtTitulo.color.r, m_txtTitulo.color.g, m_txtTitulo.color.b, 1.0f);
        m_txtTexto.color = new Color(m_txtTexto.color.r, m_txtTexto.color.g, m_txtTexto.color.b, 1.0f);

        // mostrar este control
        transform.gameObject.SetActive(true);

        // inicializar el tiempo
        m_tiempoRestanteMostrarTooltip = Stats.TIEMPO_TOOLTIP_MODO_JUEGO;
    }


    /// <summary>
    /// Reposiciona los elementos de este control
    /// </summary>
    /// <param name="_posicionControl">Posicion base del control</param>
    /// <param name="_pintarPorDerecha">Indica si el popup debe pintarse hacia la derecha o la izquierda de la posicion indicada</param>
    private void PosicionarElementos(Vector3 _posicionControl, bool _pintarPorDerecha) {
        float xOffset = 0.0f;

        if (_pintarPorDerecha) {
            // ajustar el fondo y la posicion del tooltip
            m_fondo.texture = m_fondo_dcha;
            transform.localPosition = new Vector3(_posicionControl.x + 0.18f, _posicionControl.y - 0.02f, _posicionControl.z);
            xOffset = 0.0f;
        } else {
            // ajustar el fondo y la posicion del tooltip
            m_fondo.texture = m_fondo_izda;
            transform.localPosition = new Vector3(_posicionControl.x - 0.16f, _posicionControl.y - 0.02f, _posicionControl.z);
            xOffset = -18.0f;
        }

        // reposicionar los objetos de la interfaz
        m_txtTitulo.pixelOffset = new Vector2(xOffset, 37.0f);
        m_txtTexto.pixelOffset = new Vector2(xOffset, -2.0f);
        m_txtTexto.pixelOffset = new Vector2(xOffset, -2.0f);
        m_boton.GetComponent<GUITexture>().pixelInset = new Rect((-m_boton.GetComponent<GUITexture>().pixelInset.width / 2.0f) + xOffset, m_boton.GetComponent<GUITexture>().pixelInset.yMin, m_boton.GetComponent<GUITexture>().pixelInset.width, m_boton.GetComponent<GUITexture>().pixelInset.height);
    }


    /// <summary>
    /// Oculta este control
    /// </summary>
    public void Hide() {
        transform.gameObject.SetActive(false);
    }


    void Update() {
        m_tiempoRestanteMostrarTooltip -= Time.deltaTime;

        // comprobar si ha vencido el tiempo de mostrar este tooltip
        if (m_tiempoRestanteMostrarTooltip <= 0.0f) {
            Hide();

            /*
            // mientras la opacidad no de los elementos del control no sea 0 => disminuir la opacidad de los elementos del tooltip
            float nuevaOpacidad = Mathf.Max(m_boton.guiTexture.color.a - Time.deltaTime, 0.0f);
            if (nuevaOpacidad == 0.0f) {
                // ocultar el tooltip
                Hide();
            } else {
                // actualizar la opacidad de todos los elementos de este control al maximo
                m_boton.guiTexture.color = new Color(m_boton.guiTexture.color.r, m_boton.guiTexture.color.g, m_boton.guiTexture.color.b, nuevaOpacidad);
                m_txtTitulo.color = new Color(m_txtTitulo.color.r, m_txtTitulo.color.g, m_txtTitulo.color.b, nuevaOpacidad);
                m_txtTexto.color = new Color(m_txtTexto.color.r, m_txtTexto.color.g, m_txtTexto.color.b, nuevaOpacidad);
            }
             */

        }
    }


}
