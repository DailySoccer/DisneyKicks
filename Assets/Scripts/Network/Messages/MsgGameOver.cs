using UnityEngine;

[NetMessage(MsgType.GameOver)]
public class MsgGameOver: MensajeBase
{
    public MatchState m_state;

    public MsgGameOver() {
    }

    public override void process() {
        Shark.instance.Desconectar();
        Player.serverState = m_state;
        FieldControl.instance.roundCooldown = 3f;
        //MsgGameOver msg = Shark.instance.mensaje<MsgGameOver>();
        //msg.send();
    }
}
