using UnityEngine;

[NetMessage( MsgType.RequestDuel )]
class MsgRequestDuel : MensajeBase {

    public UsuarioNet m_client;
    public string m_challenge;
    public string m_uid;

    public MsgRequestDuel() { }

    public override void process() {
        Usuario user = new Usuario(m_client);
        ifcDuelo.m_rival = MsgLobbyGroup.NetToUsuario(m_client);


        ifcDialogBox.instance.ShowTwoButtonDialog(
            ifcDialogBox.TwoButtonType.POSITIVE_NEGATIVE,
            LocalizacionManager.instance.GetTexto(106).ToUpper(),
            string.Format(LocalizacionManager.instance.GetTexto(107), "<color=#ddf108> " + user.alias + "</color>", user.getRatio() + "%"),
            LocalizacionManager.instance.GetTexto(45).ToUpper(),
            LocalizacionManager.instance.GetTexto(49).ToUpper(),
            // accion al pulsar aceptar
            (_name1) => { AceptarDuelo(); },
            // accion al pulsar cancelar
            (_name2) => { RechazarDuelo(); });
        ifcDialogBox.instance.WaitToCloseAutomatically(Stats.TIEMPO_ESPERA_RIVAL_ACEPTAR_RETO, (_name) => { RechazarDuelo(); });

        /*
        ifcDialogBox.instance.Show(
            ifcDialogBox.TipoBotones.ACEPTAR_CERRAR,
            "¡DUELO RECIBIDO!",
            "El jugador <color=#ffd200>" + user.alias + " (" + user.getRatio() + "% de victorias)</color>\n te reta a un duelo",
            // accion al pulsar aceptar
            (_name1) => { AceptarDuelo(); },
            // accion al pulsar cancelar
            (_name2) => { RechazarDuelo(); }, 
            null, "ACEPTAR", "",
            // accion a realizar cuando vence el tiempo de espera
            Stats.TIEMPO_ESPERA_RIVAL_ACEPTAR_RETO, (_name3) => { RechazarDuelo(); }
        );
         */
    }

    void RechazarDuelo() {
        MensajeBase msg = Shark.instance.mensaje<MsgChallengeAnswer>();
        (msg as MsgChallengeAnswer).m_accepted = false;
        msg.send();
    }

    void AceptarDuelo() {
        MensajeBase msg = Shark.instance.mensaje<MsgChallengeAnswer>();
        (msg as MsgChallengeAnswer).m_accepted = true;
        msg.send();
        ifcDuelo.m_rival = MsgLobbyGroup.NetToUsuario(m_client);
    }
}