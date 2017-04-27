
[NetMessage(MsgType.Error)]
public class MsgError : MensajeBase {

    public int m_code;

    public override void process()
    {
        ErrorHub.ProcessError(m_code);
    }
}