using System;

public enum MsgType : ushort {
    LoginFacebook = 1000,
    Login = 1001,
    CreateMatch = 1002,
    ReadyMatch = 1004,
    ReadyPlayer = 1005,
    StartMatch = 1006,
    PlayerActions = 1007,
    PlayerReactions = 1008,
    GetLobby = 1009,
    LobbyGroup = 1010,
    RequestDuel = 1011,
    Throw = 1012,
    Defense = 1013,
    ChallengeAnswer = 1014,
    Error = 1015,
    GameOver = 1016,
    SendState = 1017,
}

public class MensajeBase
{
	protected ushort m_len;
	protected MsgType m_id;

    public MsgType ID { get { return m_id; } set { m_id = value; } }
    public ushort Len { get { return m_len; } set { m_len = value; } }

    public virtual void process() { }
    public virtual bool send(){
        return Shark.instance.enviar(this);  
    }

}
