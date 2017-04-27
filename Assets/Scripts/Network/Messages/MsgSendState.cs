using UnityEngine;

[NetMessage(MsgType.SendState)]
public class MsgSendState: MensajeBase
{
    public MatchState state;
    public DefenseInfoNet defense;

    public MsgSendState() {
        state = new MatchState();
        state.marker_1 = new int[5];
        state.marker_2 = new int[5];
    }

    public override void process() {
        Player.serverState = state;
    }
}
