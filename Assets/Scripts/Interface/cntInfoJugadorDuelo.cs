#define DEBUG_RETO

using UnityEngine;
using System.Collections;

public class cntInfoJugadorDuelo: ifcBase {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    // elementos visuales de este control
    private GUIText m_txtUsuario;
    private btnButton m_btnRetar;
    private GameObject m_jugadorEscena;

    private Usuario m_usuario;             // informacion del usuario

    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Inicializar este control
    /// </summary>
    /// <param name="_parent">GameObject padre de este control en la jerarquia</param>
    /// <param name="_name"></param>
    /// <param name="_localPosition"></param>
    public void Inicializar(Transform _parent, string _name, Vector3 _localPosition) {
        this.transform.parent = _parent.transform;
        this.name = _name;
        this.transform.localPosition = _localPosition;

        // guardar referencias a los elementos visuales de este control
        m_txtUsuario = transform.FindChild("txtUsuario").gameObject.GetComponent<GUIText>();
        m_btnRetar = transform.FindChild("btnRetar").gameObject.GetComponent<btnButton>();
    }


    /// <summary>
    /// Muestra el control con los valores de los parametros recibidos
    /// </summary>
    /// <param name="_usuario"></param>
    /// <param name="_habilitarRetar">Indica si pulsar al pulsar sobre este control se permite retar a este jugador o no</param>
    /// <param name="_ocultarBotonesFavorito">Fuerza que no se muestren los botones añadir / eliminar favorito</param>
    public void AsignarValores(Usuario _usuario, bool _habilitarRetar = true, bool _ocultarBotonesFavorito = false) {
        // guardar la informacion del usuario
        m_usuario = _usuario;

        // actualizar textos
        m_txtUsuario.text = _usuario.alias;

        // actualizar collider
        if (_habilitarRetar) {
            m_btnRetar.action = (_name) => {
                Interfaz.ClickFX();

#if !DEBUG_RETO
                /*
                // comprobar si el jugador tiene suficiente dinero para ofrecer un duelo
                //H4CK para evitar pagar duelos
                if (Interfaz.m_monedas< Stats.PRECIO_RETO) {
                    // informar al jugador de que no tiene suficiente dinero para retar al rival
                    ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CANCELAR, "DINERO INSUFICIENTE", "Para retar a <color=#ffd200>" + _usuario.alias + "</color> a un duelo necesitas disponer al menos <color=#ffd200>" + Stats.PRECIO_RETO + "$</color>.");

                } else {
                */
#endif
                // retar al rival
                ifcDuelo.m_rival = _usuario;

                ifcDialogBox.instance.ShowTwoButtonDialog(
                    ifcDialogBox.TwoButtonType.POSITIVE_NEGATIVE,
                    LocalizacionManager.instance.GetTexto(102).ToUpper(),
                    string.Format(LocalizacionManager.instance.GetTexto(103), "<color=#ddf108> " + _usuario.alias + "</color>", _usuario.getRatio() + "%"),
                    LocalizacionManager.instance.GetTexto(45).ToUpper(),
                    LocalizacionManager.instance.GetTexto(48).ToUpper(),
                    // accion al pulsar aceptar
                    (_name1) => {
                        // enviar el mensaje al server
                        MensajeBase msg = Shark.instance.mensaje<MsgRequestDuel>();
                        (msg as MsgRequestDuel).m_challenge = _usuario.alias;
                        (msg as MsgRequestDuel).m_uid = _usuario.uid;
                        msg.send();

                        // indicar que este jugador paga por el duelo
                        // Interfaz.m_tegoQuePagarDuelo = true;

                        // mostrar un dialogo mientras se espera la respuesta
                        ifcDialogBox.instance.ShowZeroButtonDialog(LocalizacionManager.instance.GetTexto(104).ToUpper(), LocalizacionManager.instance.GetTexto(105));
                        ifcDialogBox.instance.WaitToCloseAutomatically(Stats.TIEMPO_ESPERA_CONFIRMACION_RETO_RIVAL);
                    });

#if !DEBUG_RETO
                //}
#endif
            };
            
            m_btnRetar.gameObject.SetActive(true);
        } else
            m_btnRetar.gameObject.SetActive(false);

        // instanciar y mostrar el jugador en la escena
        if(m_jugadorEscena != null) GameObject.Destroy(m_jugadorEscena);
        Vector3 relPosition = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        m_jugadorEscena = Interfaz.instance.InstantiatePlayerAtScreenRelative(relPosition, _usuario.initMode, _usuario.DefaultCharacter.idModelo, (_usuario.initMode) ? _usuario.equipacionGoalkeeper : _usuario.equipacionShooter);

        // actualizar control
        gameObject.SetActive(true);
    }


    /// <summary>
    /// Muestra / Oculta el control
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
        if(m_jugadorEscena != null) m_jugadorEscena.SetActive(_visible);
    }
}
