using UnityEngine;
using System.Collections;


/// <summary>
/// Control para mostrar si un jugador o equipacion esta DISPONIBLE o BLOQUEADO
/// </summary>
public class cntTooltipItemDisponible: MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // Texturas de fondo de este tooltip segun su comportamiento
    // NOTA: asignarles valor desde la interaz de Unity
    public Texture m_texturaFondoDisponible;
    public Texture m_texturaFondoBloqueado;

    // elementos visuales de este control
    private GUIText m_txtTitulo;
    private GUIText m_txtTituloSombra;
    private GUIText m_txtTexto;
    private btnButton m_btnBoton;
    private GUIText m_txtBoton;
    private GUIText m_txtBotonSombra;
    private GUITexture m_fondo;


    // ------------------------------------------------------------------------------
    // ---  METODOS PUBLICOS  ------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake() {
        if (m_texturaFondoDisponible == null)
            Debug.LogWarning("La textura 'm_texturaFondoDisponible' no tiene valor asignado");
        if (m_texturaFondoBloqueado == null)
            Debug.LogWarning("La textura 'm_texturaFondoBloqueado' no tiene valor asignado");
    }


    /// <summary>
    /// Oculta este control
    /// </summary>
    public void Hide() {
        this.gameObject.SetActive(false);
    }


    /// <summary>
    /// Muestra la informacion del jugador en funcion de su estado
    /// </summary>
    /// <param name="_jugador"></param>
    public void Show(Jugador _jugador) {

        if (_jugador == null)
            transform.gameObject.SetActive(false);
        else {
            switch (_jugador.estado) {
                case Jugador.Estado.BLOQUEADO:
                    ShowInfo(
                        LocalizacionManager.instance.GetTexto(20), 
                        "", //string.Format(LocalizacionManager.instance.GetTexto(21), "<color=#ddf108> " + _jugador.faseDesbloqueo + "</color>"), 
                        false, LocalizacionManager.instance.GetTexto(22), 
                        m_texturaFondoBloqueado,
                        (_name) => {
                            // XIMO: 19/06/2017: Actualmente no queremos un sistema de "Compra de Jugadores"
                            /*
                            ifcDialogBox.instance.ShowOneButtonDialog(
                                ifcDialogBox.OneButtonType.BITOONS,
                                LocalizacionManager.instance.GetTexto(91).ToUpper(),
                                string.Format(LocalizacionManager.instance.GetTexto(92), "<color=#ddf108> " + _jugador.nombre + "</color>", "<color=#ddf108> " + _jugador.precioEarlyBuy + " " + LocalizacionManager.instance.GetTexto(47) + "</color>"),
                                _jugador.precioEarlyBuy.ToString(),
                                // callback si el usuario acepta comprar el jugador
                                (_name1) => { Interfaz.instance.comprarJugador(_jugador, Interfaz.TipoPago.PRECOMPRA); },
                                true);
                            */
                        });
                    break;

                case Jugador.Estado.DISPONIBLE:
                    ShowInfo(LocalizacionManager.instance.GetTexto(23), string.Format(LocalizacionManager.instance.GetTexto(24), "<color=#ddf108> " + _jugador.nombre + "</color>"), true, LocalizacionManager.instance.GetTexto(30), m_texturaFondoDisponible,
                        (_name) => {
                            // XIMO: 19/06/2017: Actualmente no queremos un sistema de "Compra de Jugadores"
                            /*
                            ifcDialogBox.instance.ShowTwoButtonDialog(
                                ifcDialogBox.TwoButtonType.COINS_BITOONS,
                                LocalizacionManager.instance.GetTexto(91).ToUpper(),
                                string.Format(LocalizacionManager.instance.GetTexto(93), "<color=#ddf108> " + _jugador.nombre + "</color>"),
                                _jugador.precioSoft.ToString(),
                                _jugador.precioHard.ToString(),
                                // callback si el usuario acepta comprar el jugador con SOFT
                                (_name1) => { Interfaz.instance.comprarJugador(_jugador, Interfaz.TipoPago.SOFT); },
                                // callback si el usuario acepta comprar el jugador con HARD
                                (_name1) => { Interfaz.instance.comprarJugador(_jugador, Interfaz.TipoPago.HARD); },
                                true);
                            */
                    });
                    break;

                case Jugador.Estado.ADQUIRIDO:
                    // oculto el control
                    transform.gameObject.SetActive(false);
                    break;
            }
        }
    }


    /// <summary>
    /// Muestra la informacion de la EQUIPACION en funcion de su estado
    /// </summary>
    /// <param name="_equipacion"></param>
    public void Show(Equipacion _equipacion) {
        if (_equipacion == null)
            transform.gameObject.SetActive(false);
        else {
            switch (_equipacion.estado) {
                case Equipacion.Estado.BLOQUEADA:
                    ShowInfo(
                        LocalizacionManager.instance.GetTexto(26), 
                        "", // string.Format(LocalizacionManager.instance.GetTexto(27), "<color=#ddf108> " + _equipacion.faseDesbloqueo + "</color>"), 
                        false, LocalizacionManager.instance.GetTexto(22), 
                        m_texturaFondoBloqueado,
                        (_name) => {
                            ifcDialogBox.instance.ShowOneButtonDialog(
                                ifcDialogBox.OneButtonType.BITOONS,
                                LocalizacionManager.instance.GetTexto(88).ToUpper(),
                                LocalizacionManager.instance.GetTexto(87),
                                _equipacion.precioEarlyBuy.ToString(),
                                // callback para realizar la compra
                                (_name1) => { Interfaz.instance.comprarEquipacion(_equipacion, Interfaz.TipoPago.PRECOMPRA); },
                                true);
                        });
                    break;

                case Equipacion.Estado.DISPONIBLE:
                    ShowInfo(LocalizacionManager.instance.GetTexto(28), LocalizacionManager.instance.GetTexto(29), true, LocalizacionManager.instance.GetTexto(30), m_texturaFondoDisponible,
                        (_name) => {
                            ifcDialogBox.instance.ShowTwoButtonDialog(
                                ifcDialogBox.TwoButtonType.COINS_BITOONS,
                                LocalizacionManager.instance.GetTexto(88).ToUpper(),
                                LocalizacionManager.instance.GetTexto(89),
                                _equipacion.precioSoft.ToString(),
                                _equipacion.precioHard.ToString(),
                                // callback si el usuario acepta comprar la equipacion con dinero SOFT
                                (_name1) => { Interfaz.instance.comprarEquipacion(_equipacion, Interfaz.TipoPago.SOFT); },
                                // callback si el usuario acepta comprar la equipacion con dinero HARD
                                (_name1) => { Interfaz.instance.comprarEquipacion(_equipacion, Interfaz.TipoPago.HARD); },
                                true);
                        });
                    break;

                case Equipacion.Estado.ADQUIRIDA:
                    // oculto el control
                    transform.gameObject.SetActive(false);
                    break;
            }
        }
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS PRIVADOS  ------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Actualiza el estado de este tooltip
    /// </summary>
    /// <param name="_titulo"></param>
    /// <param name="_texto"></param>
    /// <param name="_mostrarboton"></param>
    /// <param name="_textoBoton"></param>
    /// <param name="texturaFondo"></param>
    /// <param name="_onClickCallback"></param>
    private void ShowInfo(string _titulo, string _texto, bool _mostrarboton = false, string _textoBoton = "", Texture _texturaFondo = null, btnButton.guiAction _onClickCallback = null) {
        // obtener las referencias a los elementos de la interfaz
        if (m_txtTitulo == null)
            m_txtTitulo = transform.FindChild("titulo").GetComponent<GUIText>();
        if (m_txtTituloSombra == null)
            m_txtTituloSombra = transform.FindChild("titulo/sombra").GetComponent<GUIText>();
        if (m_txtTexto == null)
            m_txtTexto = transform.FindChild("texto").GetComponent<GUIText>();
        if (m_btnBoton == null)
            m_btnBoton = transform.FindChild("btnDesbloquear").gameObject.GetComponent<btnButton>();
        if (m_txtBoton == null)
            m_txtBoton = transform.FindChild("btnDesbloquear/texto").GetComponent<GUIText>();
        if (m_txtBotonSombra == null)
            m_txtBotonSombra = transform.FindChild("btnDesbloquear/texto/sombra").GetComponent<GUIText>();
        if (m_fondo == null)
            m_fondo = transform.FindChild("fondo").GetComponent<GUITexture>();

        // actualizar los textos
        m_txtTitulo.text = _titulo.ToUpper();
        m_txtTituloSombra.text = _titulo.ToUpper();
        m_txtTexto.text = _texto;
        m_txtTexto.GetComponent<txtText>().Fix();

        // actualizar el fondo
        m_fondo.texture = _texturaFondo;

        // actualizar el estado del boton
        if (_mostrarboton) {
            m_txtBoton.text = _textoBoton.ToUpper();
            m_txtBotonSombra.text = _textoBoton.ToUpper();
            m_btnBoton.action = _onClickCallback;
        }
        m_btnBoton.SetEnabled(_mostrarboton);
        m_btnBoton.gameObject.SetActive(_mostrarboton);

        // activar este control
        this.gameObject.SetActive(true);
    }

    
}
