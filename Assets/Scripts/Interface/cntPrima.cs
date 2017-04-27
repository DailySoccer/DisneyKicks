using UnityEngine;
using System.Collections;

public class cntPrima : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// indica cuanto tiempo permanece este control desplegado
    /// </summary>
    private const float TIEMPO_DESPLEGADO = 1.0f;

    // desplazamiento que se aplica a los controles para que se desplacen a su posicion de plegados
    private int OFFSET_X = (int) (230 * ifcBase.scaleFactor);

    // tiempo que tarda en plegarse / desplegarse la prima
    private const float TIEMPO_PLIEGUE = 0.25f;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // texturas para la prima cuando ha sido conseguida y para cuando no
    public Texture m_texturaPrimaConseguida;
    public Texture m_texturaPrimaNoConseguida;

    // elementos de la interfaz
    private GUIText m_textoObjetivo;
    private GUITexture m_imgFondo;
    private GUITexture m_icono;
    private btnButton m_btnConseguido;

    // indica si este reto esta conseguido o no
    private bool m_conseguido;

    // indica si este control esta desplegado
    private bool m_desplegado;

    // indica el instante en el que se ha desplegado este control
    private float m_instanteDespliegue;

    // posicion original de los elementos de esta interfaz (cuando estan desplegados)
    private Vector2 m_pixelOffsetTexto;
    private Rect m_pixelInsetFondoDesplegado;
    private Rect m_pixelInsetIconoDesplegado;


    // ------------------------------------------------------------------------------
    // ---  METOODOS  ---------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Start() {
        // si no se han asignado valor a las texturas mostrar un warning
        if (m_texturaPrimaConseguida == null)
            Debug.LogWarning("No se ha asignado textura a 'm_texturaPrimaConseguida'");
        if (m_texturaPrimaNoConseguida == null)
            Debug.LogWarning("No se ha asignado textura a 'm_texturaPrimaNoConseguida'");
    }


    /// <summary>
    /// Obtener las referencias a los elementos graficos de esta interfaz
    /// </summary>
    private void GetReferencias() {
        if (m_textoObjetivo == null)
            m_textoObjetivo = transform.FindChild("txtObjetivo").GetComponent<GUIText>();

        if (m_imgFondo == null)
            m_imgFondo = transform.FindChild("fondo").GetComponent<GUITexture>();

        if (m_icono == null)
            m_icono = transform.FindChild("btnConseguido/icono").GetComponent<GUITexture>();

        if (m_btnConseguido == null)
            m_btnConseguido = transform.FindChild("btnConseguido").GetComponent<btnButton>();
    }


    /// <summary>
    /// Inicializa este control
    /// </summary>
    /// <param name="_textoObjetivo">Texto del objetivo</param>
    /// <param name="_conseguido">Indica si se ha conseguido o no este objetivo</param>
    public void Inicializar(string _textoObjetivo, bool _conseguido) {
        GetReferencias();

        m_conseguido = _conseguido;

        m_textoObjetivo.text = _textoObjetivo;

        PintarComoConseguido(_conseguido);

        // al pulsar sobre el control => desplegarlo
        m_btnConseguido.action = (_name) => {
            Desplegar();
        };

        // obtener las coordenadas de los elementos cuando estan plegados
        m_pixelOffsetTexto = m_textoObjetivo.pixelOffset;
        m_pixelInsetFondoDesplegado = m_imgFondo.pixelInset;
        m_pixelInsetIconoDesplegado = m_icono.pixelInset;

        // plegar los elementos
        m_desplegado = false;
        m_textoObjetivo.pixelOffset = new Vector2(m_pixelOffsetTexto.x + OFFSET_X, m_pixelOffsetTexto.y);
        m_imgFondo.GetComponent<GUITexture>().pixelInset = new Rect(m_pixelInsetFondoDesplegado.xMin + OFFSET_X, m_pixelInsetFondoDesplegado.yMin, m_pixelInsetFondoDesplegado.width, m_pixelInsetFondoDesplegado.height);
        m_icono.GetComponent<GUITexture>().pixelInset = new Rect(m_pixelInsetIconoDesplegado.xMin + OFFSET_X, m_pixelInsetIconoDesplegado.yMin, m_pixelInsetIconoDesplegado.width, m_pixelInsetIconoDesplegado.height);
        m_btnConseguido.GetComponent<GUITexture>().pixelInset = m_icono.GetComponent<GUITexture>().pixelInset;
    }


    /// <summary>
    /// Actualiza el estado de este control
    /// </summary>
    /// <param name="_conseguido"></param>
    public void RefreshConseguido(bool _conseguido) {
        // si el reto pasa de no estar conseguido a estarlo => desplegar el control
        if (!m_conseguido && _conseguido) {
            Desplegar();
        }

        PintarComoConseguido(_conseguido);
    }


    /// <summary>
    /// Pinta el objetivo de mision como conseguido o no
    /// </summary>
    /// <param name="_conseguido"></param>
    private void PintarComoConseguido(bool _conseguido) {
        // actualizar el estado
        m_conseguido = _conseguido;

        // actualizar la opacidad y la textura del icono para mostrar la prima
        m_icono.texture = (_conseguido) ? m_texturaPrimaConseguida : m_texturaPrimaNoConseguida;
        m_icono.color = _conseguido ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 51.0f / 255.0f);
        m_textoObjetivo.color = _conseguido ? new Color(1.0f, 1.0f, 1.0f, 1.0f) : new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }


    /// <summary>
    /// Despliega este control
    /// </summary>
    private void Desplegar() {
        if (!m_desplegado) {
            GeneralSounds.instance.prima();

            // plegar cada uno de los elementos de la prima
            new SuperTweener.MoveGuitextPixelOffset(m_textoObjetivo.gameObject, TIEMPO_PLIEGUE, m_pixelOffsetTexto);
            new SuperTweener.MoveGuitexturePixelInset(m_imgFondo.gameObject, TIEMPO_PLIEGUE, m_pixelInsetFondoDesplegado);
            new SuperTweener.MoveGuitexturePixelInset(m_icono.gameObject, TIEMPO_PLIEGUE, m_pixelInsetIconoDesplegado);
            new SuperTweener.MoveGuitexturePixelInset(m_btnConseguido.gameObject, TIEMPO_PLIEGUE, m_pixelInsetIconoDesplegado, SuperTweener.CubicOut,
                // una vez desplegado
                (_name) => {
                    // indicar que el control esta desplagado y almacenar el instante actual
                    m_desplegado = true;
                    m_instanteDespliegue = Time.realtimeSinceStartup;
                });
        }
    }


    public void Update() {
        // si este control esta desplegado
        if (m_desplegado) {
            // si ha vencido el tiempo que este control tenia que estar desplegado
            if ((Time.realtimeSinceStartup - m_instanteDespliegue) > TIEMPO_DESPLEGADO) {
                // volver a plegar el control
                m_desplegado = false;

                // plegar cada uno de los elementos de este control
                new SuperTweener.MoveGuitextPixelOffset(m_textoObjetivo.gameObject, TIEMPO_PLIEGUE, new Vector2(m_pixelOffsetTexto.x + OFFSET_X, m_pixelOffsetTexto.y));
                new SuperTweener.MoveGuitexturePixelInset(m_imgFondo.gameObject, TIEMPO_PLIEGUE, new Rect(m_pixelInsetFondoDesplegado.xMin + OFFSET_X, m_pixelInsetFondoDesplegado.yMin, m_pixelInsetFondoDesplegado.width, m_pixelInsetFondoDesplegado.height));
                new SuperTweener.MoveGuitexturePixelInset(m_icono.gameObject, TIEMPO_PLIEGUE, new Rect(m_pixelInsetIconoDesplegado.xMin + OFFSET_X, m_pixelInsetIconoDesplegado.yMin, m_pixelInsetIconoDesplegado.width, m_pixelInsetIconoDesplegado.height));
                new SuperTweener.MoveGuitexturePixelInset(m_btnConseguido.gameObject, TIEMPO_PLIEGUE, new Rect(m_pixelInsetIconoDesplegado.xMin + OFFSET_X, m_pixelInsetIconoDesplegado.yMin, m_pixelInsetIconoDesplegado.width, m_pixelInsetIconoDesplegado.height));
            }
        }

    }
}
