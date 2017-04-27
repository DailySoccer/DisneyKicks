using UnityEngine;
using System.Collections;

public class cntVisualizadorLogro : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // texturas
    // NOTA: asignarles valor desde la interfaz de Unity
    public Texture m_texturaBarraBrogresoNormal;
    public Texture m_texturaBarraBrogresoLogroNuevo;
    public Texture m_texturaBarraBrogresoLogroCompletado;
    public Texture m_texturaFrontNormal;
    public Texture m_texturaFrontLogroNuevo;
    public Texture m_texturaFrontLogroCompletado;
    public Texture m_texturaBackNormal;
    public Texture m_texturaBackNuevo;

    // elementos de esta interfaz
    private GUIText m_txtTitulo;
    private GUIText m_txtSubtitulo;
    private GUIText m_txtNivel;
    private GUIText m_txtNivelSombra;
    private GUIText m_txtRecompensa;
    private GUIText m_txtProgreso;
    private GUIText m_txtProgresoSombra;
    private GUITexture m_barraProgreso;
    private GUITexture m_barraProgresoFront;
    private GUITexture m_barraProgresoBack;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    void Awake() {
        // mostrar warnings si falta por definir alguna textura
        if (m_texturaBarraBrogresoNormal == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaBarraBrogresoNormal'");
        if (m_texturaBarraBrogresoLogroNuevo == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaBarraBrogresoLogroNuevo'");
        if (m_texturaBarraBrogresoLogroCompletado == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaBarraBrogresoLogroCompletado'");
        if (m_texturaFrontNormal == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaFrontNormal'");
        if (m_texturaFrontLogroNuevo == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaFrontLogroNuevo'");
        if (m_texturaFrontLogroCompletado == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaFrontLogroCompletado'");
        if (m_texturaBackNormal == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaBackNormal'");
        if (m_texturaBackNuevo == null)
            Debug.LogWarning("No se ha definido la textura 'm_texturaBackNuevo'");
    }


    /// <summary>
    /// Obtiene las referencias de los elementos de la interfaz
    /// </summary>
    private void GetReferencias() {
        if (m_txtTitulo == null)
            m_txtTitulo = transform.FindChild("txtTitulo").GetComponent<GUIText>();
        if (m_txtSubtitulo == null)
            m_txtSubtitulo = transform.FindChild("txtSubtitulo").GetComponent<GUIText>();
        if (m_txtNivel == null)
            m_txtNivel = transform.FindChild("txtNivel").GetComponent<GUIText>();
        if (m_txtNivelSombra == null)
            m_txtNivelSombra = transform.FindChild("txtNivelSombra").GetComponent<GUIText>();
        if (m_txtRecompensa == null)
            m_txtRecompensa = transform.FindChild("txtRecompensa").GetComponent<GUIText>();
        if (m_txtProgreso == null)
            m_txtProgreso = transform.FindChild("BarraProgreso/txtProgreso").GetComponent<GUIText>();
        if (m_txtProgresoSombra == null)
            m_txtProgresoSombra = transform.FindChild("BarraProgreso/txtProgresoSombra").GetComponent<GUIText>();
        if (m_barraProgreso == null)
            m_barraProgreso = transform.FindChild("BarraProgreso/barra").GetComponent<GUITexture>();
        if (m_barraProgresoFront == null)
            m_barraProgresoFront = transform.FindChild("BarraProgreso/front").GetComponent<GUITexture>();
        if (m_barraProgresoBack == null)
            m_barraProgresoBack = transform.FindChild("BarraProgreso/back").GetComponent<GUITexture>();
    }


    /// <summary>
    /// Muestra en el control los valores asociados a un logro
    /// </summary>
    /// <param name="_grupoLogros"></param>
    public void ShowValues(GrupoLogros _grupoLogros) {
        if (_grupoLogros != null) {
            // obtener las referencias a los elementos de la interfaz
            GetReferencias();

            // actualizar los textos
            m_txtTitulo.text = _grupoLogros.nombre.ToUpper();
            m_txtSubtitulo.text = _grupoLogros.descripcion;

            // progreso del logro
            m_txtProgreso.text = _grupoLogros.progreso.ToString() + " / " + _grupoLogros.valorSuperarLogro.ToString();
            m_txtProgresoSombra.text = m_txtProgreso.text;

            // mostrar el nivel
            if (_grupoLogros.nivelAlcanzado < 0)
                m_txtNivel.text = "";
            else
                m_txtNivel.text = (LocalizacionManager.instance.GetTexto(31) + " " + (_grupoLogros.nivelAlcanzado + 1).ToString()).ToUpper();
            m_txtNivelSombra.text = m_txtNivel.text;

            // mostrar la recompensa
            if (_grupoLogros.recompensa < 0)
                m_txtRecompensa.text = "";
            else
				m_txtRecompensa.text = _grupoLogros.recompensa.ToString() + " §";

            // actualizar el valor de la barra de progreso
            Rect rectBarraProgreso = transform.FindChild("BarraProgreso/fondo").GetComponent<GUITexture>().pixelInset;
            transform.FindChild("BarraProgreso/barra").GetComponent<GUITexture>().pixelInset = new Rect(rectBarraProgreso.xMin, rectBarraProgreso.yMin, (rectBarraProgreso.width * _grupoLogros.porcentajeSuperadoLogro), rectBarraProgreso.height);          

            // texturas por defecto
            m_barraProgreso.texture = m_texturaBarraBrogresoNormal;
            m_barraProgresoFront.texture = m_texturaFrontNormal;
            m_barraProgresoBack.texture = m_texturaBackNormal;

            // comprobar si hay que destacar el logro de alguna manera en especial -------------------

            // comprobar si se han completado todos los niveles del logro
            if (_grupoLogros.superadosTodosLosLogros) {
                m_txtProgreso.text = LocalizacionManager.instance.GetTexto(288).ToUpper();
                m_txtProgresoSombra.text = m_txtProgreso.text;

                m_barraProgreso.texture = m_texturaBarraBrogresoLogroCompletado;
                m_barraProgresoFront.texture = m_texturaFrontLogroCompletado;
            }

            // comprobar si el logro se ha obtenido recientemente (es nuevo)
            if (_grupoLogros.subidaNivelReciente) {
                m_barraProgreso.texture = m_texturaBarraBrogresoLogroNuevo;
                m_barraProgresoFront.texture = m_texturaFrontLogroNuevo;
                m_barraProgresoBack.texture = m_texturaBackNuevo;

                _grupoLogros.subidaNivelReciente = false; // <= haciendo esto ya no se vuelve a destacar la subida de nivel en este grupo de logros
            }
        }
    }


}
