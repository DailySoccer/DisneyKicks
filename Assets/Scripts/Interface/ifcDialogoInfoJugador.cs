using UnityEngine;
using System.Collections;

public class ifcDialogoInfoJugador : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static ifcDialogoInfoJugador instance { get { return m_instance; } }
    private static ifcDialogoInfoJugador m_instance;

    // referencias a los elementos graficos de esta interfaz
    private Transform m_cajaContenedora; 
    private GUIText m_txtNombre;
    private GUIText m_txtNombreSombra;
    private GUIText m_txtPais;
    private GUIText m_txtPaisSombra;
    private GUITexture[] m_iconoHabilidad;
    private GUIText[] m_txtHabilidadesTitulo;
    private GUIText[] m_txtHabilidadesTexto;
    private btnButton m_btnPlegarDesplegar;
    private GUITexture m_avatar;
    private btnButton m_btnFichaSiguiente;
    private btnButton m_btnFichaAnterior;
    private btnButton m_btnVelo;    // <= el velo se comporta como un boton de cerrar

    private cntTooltipItemDisponible m_tooltipItemDisponible;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    // Use this for initialization
    void Start ()
    {
        // obtener las referencias a los elementos de la interfaz
        m_cajaContenedora = transform.FindChild("caja");

        m_tooltipItemDisponible = transform.Find("caja/tooltipItemDisponible").GetComponent<cntTooltipItemDisponible>();
        //Debug.LogError("(m_tooltipItemDisponible.transform.localPosition.x / ifcBase.scaleWFactor) => " + (m_tooltipItemDisponible.transform.localPosition.x / ifcBase.scaleWFactor));
        //m_tooltipItemDisponible.transform.localPosition = new Vector3(m_tooltipItemDisponible.transform.localPosition.x / ifcBase.scaleWFactor, m_tooltipItemDisponible.transform.localPosition.y, m_tooltipItemDisponible.transform.localPosition.z);
        float newX = (0.5f / Camera.main.aspect) * (16f / 9f);
        m_tooltipItemDisponible.transform.localPosition = new Vector3(newX, m_tooltipItemDisponible.transform.localPosition.y, m_tooltipItemDisponible.transform.localPosition.z);

        m_txtNombre = m_cajaContenedora.FindChild("nombre").GetComponent<GUIText>();
        m_txtNombreSombra = m_cajaContenedora.FindChild("nombre/sombra").GetComponent<GUIText>();
        m_txtPais = m_cajaContenedora.FindChild("pais").GetComponent<GUIText>();
        m_txtPaisSombra = m_cajaContenedora.FindChild("pais/sombra").GetComponent<GUIText>();

        m_iconoHabilidad = new GUITexture[3];
        m_txtHabilidadesTitulo = new GUIText[3];
        m_txtHabilidadesTexto = new GUIText[3];
        for (int i = 0; i < m_txtHabilidadesTexto.Length; ++i) {
            m_iconoHabilidad[i] = m_cajaContenedora.FindChild("habilidad" + (i + 1) + "/icono").GetComponent<GUITexture>();
            m_txtHabilidadesTitulo[i] = m_cajaContenedora.FindChild("habilidad" + (i + 1) + "/titulo").GetComponent<GUIText>();
            m_txtHabilidadesTexto[i] = m_cajaContenedora.FindChild("habilidad" + (i + 1) + "/texto").GetComponent<GUIText>();
        }

        m_btnPlegarDesplegar = m_cajaContenedora.FindChild("btnPlegarDesplegar").GetComponent<btnButton>();
        m_btnPlegarDesplegar.action = (_name) => {
            Interfaz.ClickFX();
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
            Plegar();
        };

        m_btnVelo = transform.FindChild("velo").GetComponent<btnButton>();
        m_btnVelo.action = (_name) => {
            Interfaz.ClickFX();
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
            Plegar();
        };

        m_avatar = m_cajaContenedora.FindChild("avatar").GetComponent<GUITexture>();

        m_btnFichaSiguiente = m_cajaContenedora.Find("btnFichaSiguiente").GetComponent<btnButton>();
        m_btnFichaAnterior = m_cajaContenedora.Find("btnFichaAnterior").GetComponent<btnButton>();

        // ocultar este formulario
        gameObject.SetActive(false);
    }


    /// <summary>
    /// Muestra la informacion asociada al jugador recibido como parametro
    /// </summary>
    /// <param name="_jugador"></param>
    public void Show(Jugador _jugador) {
        if (_jugador == null)
            return;
        else {
            m_txtNombre.text = _jugador.nombre.ToUpper();
            m_txtNombreSombra.text = _jugador.nombre.ToUpper();
            m_txtPais.text = _jugador.pais.ToUpper();
            m_txtPaisSombra.text = _jugador.pais.ToUpper();

            m_tooltipItemDisponible.Show(_jugador);

            // mostrar las habilidades que tiene el jugador
            int i = 0;
            if (_jugador.habilidades != null) {
                for (; i < _jugador.habilidades.Length; ++i) {
                    m_iconoHabilidad[i].texture = AvataresManager.instance.GetTexturaHabilidad((int)_jugador.habilidades[i]);
                    m_txtHabilidadesTitulo[i].GetComponent<txtText>().SetText(Habilidades.SkillToString(_jugador.habilidades[i]).ToUpper());
                    m_txtHabilidadesTexto[i].GetComponent<txtText>().SetText(Habilidades.SkillDescription(_jugador.habilidades[i]));
                }
            }

            // ocultar los textos para mostrar habilidades que sobran 
            for (; i < m_txtHabilidadesTexto.Length; ++i) {
                m_iconoHabilidad[i].texture = null;
                m_txtHabilidadesTitulo[i].text = "";
                m_txtHabilidadesTexto[i].text = "";
            }

            // mostrar el avatar del jugador y los botones para paginar fichas
            if (InfoJugadores.instance.EsLanzador(_jugador.assetName)) {
                m_avatar.texture = AvataresManager.instance.GetTexturaLanzadorCuerpoEntero(_jugador.idModelo);
                m_btnFichaAnterior.action = (_name) => { 
                    ifcVestuario.instance.CambiarJugadorSeleccionado(-1);
                    Show(InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower));
                };
                m_btnFichaSiguiente.action = (_name) => {
                    ifcVestuario.instance.CambiarJugadorSeleccionado(+1);
                    Show(InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower));
                };
            } else {
                m_avatar.texture = AvataresManager.instance.GetTexturaPorteroCuerpoEntero(_jugador.idModelo);
                m_btnFichaAnterior.action = (_name) => {
                    ifcVestuario.instance.CambiarJugadorSeleccionado(-1);
                    Show(InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper));
                };
                m_btnFichaSiguiente.action = (_name) => {
                    ifcVestuario.instance.CambiarJugadorSeleccionado(+1);
                    Show(InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper));
                };
            }
        }
    }


    /// <summary>
    /// Despliega este dialogo
    /// </summary>
    public void Desplegar() {
        // mostrar este control y desplegarlo
        gameObject.SetActive(true);
        new SuperTweener.move(m_cajaContenedora.gameObject, 0.25f, new Vector3(0.0f, 0.5f, 1001.0f), SuperTweener.CubicOut);
    }


    /// <summary>
    /// Pliega este dialogo
    /// </summary>
    public void Plegar() {
        // desplegar este control y despues ocultarlo
        new SuperTweener.move(m_cajaContenedora.gameObject, 0.25f, new Vector3(-1.0f, 0.5f, 1001.0f), SuperTweener.CubicOut, 
            // on end callback
            (_base) => { 
                // mostrar el boton info
                ifcVestuario.instance.SetVisibleInfoButton(true);

                // ocultar este control
                gameObject.SetActive(false); 
            });
    }
}
