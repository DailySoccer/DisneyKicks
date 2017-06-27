using UnityEngine;

public struct UsuarioNet {
    public string alias;
    public string uid;
    public int numVictorias;
    public int numDerrotas;
    public bool initMode;
    public string characterThrower;
    public string characterGoalkeeper;
    public string equipacionThrower;
    public string equipacionGoalkeeper;
    public bool isBot; //3 leyes de la robotica aplican

    // public int skillLevel;
}

[NetMessage(MsgType.LobbyGroup)]
public class MsgLobbyGroup : MensajeBase
{
    public UsuarioNet[] m_clients;
    
    public override void process()
    {
		ifcDuelo.instance.AsignarRival(NetToUsuario(m_clients));
    }

    public static Usuario[] NetToUsuario(UsuarioNet[] _usuarios)
    {
        Usuario[] users = new Usuario[_usuarios.Length];
        for(int i = 0; i < _usuarios.Length; i++)
        {
            users[i] = NetToUsuario(_usuarios[i]);
        }
        return users;
    }

    public static Usuario NetToUsuario(UsuarioNet _usuario)
    {
        Usuario user = new Usuario(_usuario);
        user.charThrower = InfoJugadores.instance.GetJugador( _usuario.characterThrower );
        user.charGoalkeeper = InfoJugadores.instance.GetJugador( _usuario.characterGoalkeeper );
        user.initMode = _usuario.initMode;
        user.numDerrotas = _usuario.numDerrotas;
        user.numVictorias = _usuario.numVictorias;
        user.equipacionShooter = EquipacionManager.instance.GetEquipacion(_usuario.equipacionThrower);
        user.equipacionGoalkeeper = EquipacionManager.instance.GetEquipacion(_usuario.equipacionGoalkeeper);
        user.yoRobot = _usuario.isBot;
        user.uid = _usuario.uid;
        user.alias = _usuario.alias;
        // user.skillLevel = _usuario.skillLevel;
        return user;
    }
}
