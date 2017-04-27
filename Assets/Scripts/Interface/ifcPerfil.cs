using UnityEngine;
using System.Collections;

public class ifcPerfil : ifcBase
{
    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    // posibles tipos de pagina que puede mostrar este control
    public enum TipoPagina { LANZADOR, PORTERO, DUELO };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static ifcPerfil instance { 
        get {
            if (m_instance == null) {
                Transform tr = Interfaz.instance.transform.FindChild("Perfil");
                if (tr != null) {
                    m_instance = tr.GetComponent<ifcPerfil>();
                    m_instance.Start();
                }
            }
            return m_instance;
        } 
    }
    private static ifcPerfil m_instance;

    // tipo de pagina que se esta mostrando actualmente
    public TipoPagina m_tipoPagina = TipoPagina.LANZADOR;

    // elementos de la interfaz
    private btnButton m_btnAtras;
    private btnButton m_btnLanzador;
    private btnButton m_btnPortero;
    private btnButton m_btnDuelo;
    private GUIText m_txtTitulo;
    private GUIText m_txtTituloSombra;
    private GUIText m_txtLogros;
    private GUIText[] m_infoTextos;
    private GUIText[] m_infoValores;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    /// <summary>
    /// Obtiene la referencia a los elementos de esta interfaz
    /// </summary>
    private void GetReferencias() {

        // boton atras
        if (m_btnAtras == null) {
            m_btnAtras = getComponentByName("btnAtras").GetComponent<btnButton>();
            m_backMethod = Back;
            m_btnAtras.action = Back;
        }

        // campos de texto
        if (m_txtTitulo == null)
            m_txtTitulo = transform.FindChild("informacion/titulo").GetComponent<GUIText>();
        
        if (m_txtTituloSombra == null)
            m_txtTituloSombra = transform.FindChild("informacion/tituloSombra").GetComponent<GUIText>();
        
        if (m_txtLogros == null)
            m_txtLogros = transform.FindChild("informacion/logros/Puntos").GetComponent<GUIText>();

        if (m_infoTextos == null || m_infoValores == null) {
            m_infoTextos = new GUIText[4];
            m_infoValores = new GUIText[4];
            for (int i = 0; i < 4; ++i) {
                m_infoTextos[i] = transform.FindChild("informacion/info_" + i + "/Nombre").GetComponent<GUIText>();
                m_infoValores[i] = transform.FindChild("informacion/info_" + i + "/Puntos").GetComponent<GUIText>();
            }
        }

        // boton de lanzador
        if (m_btnLanzador == null) {
            m_btnLanzador = transform.FindChild("botones/btnLanzador").GetComponent<btnButton>();
            m_btnLanzador.action = (_name) => {
                Interfaz.ClickFX();
                ShowPagina(TipoPagina.LANZADOR);
            };
        }

        // boton de portero
        if (m_btnPortero == null) {
            m_btnPortero = transform.FindChild("botones/btnPortero").GetComponent<btnButton>();
            m_btnPortero.action = (_name) => {
                Interfaz.ClickFX();
                ShowPagina(TipoPagina.PORTERO);
            };
        }

        // boton de duelo
        if (m_btnDuelo == null) {
            m_btnDuelo = transform.FindChild("botones/btnDuelo").GetComponent<btnButton>();
            m_btnDuelo.action = (_name) => {
                Interfaz.ClickFX();
                ShowPagina(TipoPagina.DUELO);
            };
        }
    }


	// Use this for initialization
	void Start () {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();       
    }


    void Back(string _target = "")
    {
        if(Application.loadedLevel == 0)
        {
            GeneralSounds_menu.instance.back();

            new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name) => { SetVisible(false); });
            cntBarraSuperior.instance.VolverAPantallaAnterior();

            Interfaz.instance.Thrower = Interfaz.instance.Thrower;
            Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
        }
    }


    // recarga los valores de esta interfaz
    public void Refresh() {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        // actualiza la pagina actual
        ShowPagina(m_tipoPagina);
    }


    /// <summary>
    /// Muestra la pagina recibida como parametro
    /// </summary>
    /// <param name="_tipoPagina"></param>
    private void ShowPagina(TipoPagina _tipoPagina) {
        // titulo
        string strTitulo = "";
        switch (_tipoPagina) {
            case TipoPagina.LANZADOR:
                strTitulo = LocalizacionManager.instance.GetTexto(14).ToUpper();
                break;
            case TipoPagina.PORTERO:
                strTitulo = LocalizacionManager.instance.GetTexto(15).ToUpper();
                break;
            case TipoPagina.DUELO:
                strTitulo = LocalizacionManager.instance.GetTexto(9).ToUpper();
                break;
        }
        m_txtTitulo.text = strTitulo;
        m_txtTituloSombra.text = strTitulo;
        m_tipoPagina = _tipoPagina;

        // resaltar el boton que corresponda
        m_btnLanzador.Deselect();
        m_btnPortero.Deselect();
        m_btnDuelo.Deselect();

        // actualizar los valores en funcion del tipo de pagina
        switch (_tipoPagina) {
            case TipoPagina.LANZADOR:
                m_btnLanzador.Select();

                m_infoTextos[0].text = LocalizacionManager.instance.GetTexto(32).ToUpper(); // puntuacion
                m_infoTextos[1].text = LocalizacionManager.instance.GetTexto(33).ToUpper(); // goles
                m_infoTextos[2].text = LocalizacionManager.instance.GetTexto(34).ToUpper(); // dianas
                m_infoTextos[3].text = LocalizacionManager.instance.GetTexto(35).ToUpper(); // balones fallados

                m_infoValores[0].text = Interfaz.m_asThrower.record.ToString();
                m_infoValores[1].text = (Interfaz.m_asThrower.goals + Interfaz.m_asThrower.targets).ToString();
                m_infoValores[2].text = Interfaz.m_asThrower.targets.ToString();
                m_infoValores[3].text = Interfaz.m_asThrower.throwOut.ToString();

                m_txtLogros.text = LogrosManager.instance.GetNumLogrosConseguidos(LogrosManager.TipoLogros.LANZADOR) + " / " + LogrosManager.instance.GetNumTotalLogros(LogrosManager.TipoLogros.LANZADOR);
                break;

            
            case TipoPagina.PORTERO:
                m_btnPortero.Select();

                m_infoTextos[0].text = LocalizacionManager.instance.GetTexto(32).ToUpper(); // puntuacion
                m_infoTextos[1].text = LocalizacionManager.instance.GetTexto(36).ToUpper(); // balones despejados
                m_infoTextos[2].text = LocalizacionManager.instance.GetTexto(37).ToUpper(); // balones parados
                m_infoTextos[3].text = LocalizacionManager.instance.GetTexto(38).ToUpper(); // balones encajados

                m_infoValores[0].text = Interfaz.m_asKeeper.record.ToString();
                m_infoValores[1].text = Interfaz.m_asKeeper.deflected.ToString();
                m_infoValores[2].text = Interfaz.m_asKeeper.goalsStopped.ToString();
                m_infoValores[3].text = Interfaz.m_asKeeper.goals.ToString();

                m_txtLogros.text = LogrosManager.instance.GetNumLogrosConseguidos(LogrosManager.TipoLogros.PORTERO) + " / " + LogrosManager.instance.GetNumTotalLogros(LogrosManager.TipoLogros.PORTERO);
                break;
            

            case TipoPagina.DUELO:
                m_btnDuelo.Select();

                m_infoTextos[0].text = LocalizacionManager.instance.GetTexto(39).ToUpper(); // duelos jugados
                m_infoTextos[1].text = LocalizacionManager.instance.GetTexto(40).ToUpper(); // duelos ganados
                m_infoTextos[2].text = LocalizacionManager.instance.GetTexto(41).ToUpper(); // duelos perfectos
                m_infoTextos[3].text = LocalizacionManager.instance.GetTexto(42).ToUpper(); // victorias

                m_infoValores[0].text = Interfaz.m_duelsPlayed.ToString();
                m_infoValores[1].text = Interfaz.m_duelsWon.ToString();
                m_infoValores[2].text = Interfaz.m_perfectDuels.ToString();
                m_infoValores[3].text = ((Interfaz.m_duelsPlayed == 0) ? 0f : Mathf.RoundToInt(((float)Interfaz.m_duelsWon / (float)Interfaz.m_duelsPlayed) * 100f)).ToString() + "%";

                m_txtLogros.text = LogrosManager.instance.GetNumLogrosConseguidos(LogrosManager.TipoLogros.DUELO) + " / " + LogrosManager.instance.GetNumTotalLogros(LogrosManager.TipoLogros.DUELO);
                break;
        }

        // actualizar los modelos de los jugadores
        Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
        Interfaz.instance.Thrower = Interfaz.instance.Thrower;
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }

}
