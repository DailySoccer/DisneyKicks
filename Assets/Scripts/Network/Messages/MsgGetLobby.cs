
[NetMessage(MsgType.GetLobby)]
public class MsgGetLobby : MensajeBase {
    public int m_flags;
    public int m_matchID;

    public bool send(int _flags ) {
        m_flags = _flags;
        m_matchID = 0;
        return base.send();
    }

    public override void process() {
    }
}