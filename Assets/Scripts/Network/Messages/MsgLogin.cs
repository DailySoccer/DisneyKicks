using UnityEngine;

[NetMessage(MsgType.Login)]
public class MsgLogin : MensajeBase
{
    public string m_alias;
    public string m_goalkeeper;
    public string m_thrower;
    public string m_goalkeeperEquipacion;
    public string m_throwerEquipacion;
    public int m_version;
    public int m_duelosJugados;
    public int m_duelosGanados;
    public int m_skillLevel;

    public MsgLogin()
    {
        //Random.seed = (int)System.DateTime.Now.Ticks;
        //m_userID = (uint)(Random.value * uint.MaxValue);
        m_version = Stats.version;
    }

    public override void process() {
        Shark.instance.ConnectionID = m_alias;
        //Shark.instance.mensaje<MsgCreateMatch>().send(0);
        Shark.instance.mensaje<MsgGetLobby>().send(0);
        ifcDialogBox.instance.Hide();
    }
}
