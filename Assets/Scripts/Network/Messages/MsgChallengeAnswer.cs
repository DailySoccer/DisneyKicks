using UnityEngine;

[NetMessage( MsgType.ChallengeAnswer )]
class MsgChallengeAnswer : MensajeBase {
    public bool m_accepted;
    public MsgChallengeAnswer() { }

    public override void process()
    {
        ifcDialogBox.instance.Hide();

        // si se ha rechazado el duelo
        if (!m_accepted) {
            // mostrar un dialogo para informar al usuario
            ifcDialogBox.instance.ShowOneButtonDialog(
                ifcDialogBox.OneButtonType.POSITIVE,
                LocalizacionManager.instance.GetTexto(100).ToUpper(),
                string.Format(LocalizacionManager.instance.GetTexto(101), "<color=#ddf108> " + ifcDuelo.m_rival.alias + "</color>"),
                LocalizacionManager.instance.GetTexto(45).ToUpper());

            // indicar que este jugador ya no tiene que pagar el duelo
            //Interfaz.m_tegoQuePagarDuelo = false;
        }
    }
}