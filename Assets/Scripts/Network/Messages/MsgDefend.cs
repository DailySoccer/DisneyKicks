using UnityEngine;

public struct DefenseInfoNet{
    /// <summary>
    /// Defense target.
    /// </summary>
    public float X;
    public float Y;
    public float Z;

    public bool success;
    public bool perfect;
    public bool noDefense;
    public bool precisionFail;
    public bool late;
    public bool outOfBounds;
    public int powerupUsed;
}

[NetMessage(MsgType.Defense)]
public class MsgDefend: MensajeBase
{
    public DefenseInfoNet info;
    public MatchState state;
    public bool ignoreState;
    public int points;

    public MsgDefend() {
        state = new MatchState();
        state.marker_1 = new int[5];
        state.marker_2 = new int[5];
        ignoreState = false;
    }

    public override void process() {
        InputManager.instance.m_timeToThrow = InputManager.instance.deltaTimeToThrow;
        GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Wait.instance;
        //Time.timeScale = ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper ? 0.8f : 1f; //TODO parametrizar mejor los tiempos de cada modo de juego
        ServiceLocator.Request<IGameplayService>().ResetTime();
        DefenseInfo infoR = UnloadDefense(info);
        Goalkeeper.instance.m_futureDefense = infoR;
        Goalkeeper.instance.SetupNetworkDefense(infoR);
        if(info.powerupUsed != -1)
            PowerupService.instance.UsePowerup((Powerup)info.powerupUsed);

        if(!ignoreState)
        {
            Player.serverState = state;
        }
    }

    public void LoadDefense(Vector3 _info, GKResult _result)
    {
        info = ToDefenseInfoNet(_info, _result);
        if(PowerupService.instance.usedGoalkeeperPowerup)
            info.powerupUsed = (int)PowerupService.instance.GoalkeeperPowerup;
        else
            info.powerupUsed = -1;
    }

    public static DefenseInfoNet ToDefenseInfoNet(Vector3 _info, GKResult _result)
    {
        DefenseInfoNet infoN = new DefenseInfoNet();
        infoN.X = _info.x;
        infoN.Y = _info.y;
        infoN.Z = _info.z;
        infoN.noDefense = _result == GKResult.Idle;
        infoN.perfect = _result == GKResult.Perfect;
        infoN.success = _result == GKResult.Good
                        || _result == GKResult.Perfect;
        infoN.outOfBounds = _result == GKResult.ThrowerFail;
        infoN.precisionFail = _result == GKResult.Fail;
        infoN.late = _result == GKResult.Late;
        return infoN;
    }

    public DefenseInfo UnloadDefense(DefenseInfoNet _info)
    {
        DefenseInfo result = new DefenseInfo();
        result.Target = new Vector3(_info.X, _info.Y, _info.Z);
        result.ForcedResult = true;
        result.Result = _info.success ? (_info.perfect ? GKResult.Perfect : GKResult.Good) 
                        : (_info.precisionFail ? GKResult.Fail : (_info.late ? GKResult.Late : GKResult.Early));
        if(_info.noDefense) result.Result = GKResult.Idle;  //cuidado con las prioridades!!
        return result;
    }
}
