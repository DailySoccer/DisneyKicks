using UnityEngine;
using System.Collections;
using System;

public struct ShotInfoNet {
    /// <summary>
    /// Effect applied to the ball, in the horizontal plane.
    /// </summary>
    public float Effect01;
    
    /// <summary>
    /// Shot target.
    /// </summary>
    public float X;
    public float Y;
    public float Z;

    public float TimeRatio;
    public int usedPower;
}

public struct MatchState{
    public int score_1;
    public int score_2;
    public int rounds;
    public int[] marker_1;
    public int[] marker_2;
}

public struct MatchStateSimple{
    public int score;
    public int rounds;
    public int[] marker;
}

[NetMessage(MsgType.Throw)]
public class MsgThrow : MensajeBase
{
    public ShotInfoNet info;
    public MatchState state;
    public int points;

    public MsgThrow()
    {
        state = new MatchState();
        state.marker_1 = new int[5];
        state.marker_2 = new int[5];
    }

    public override void process() {
        FieldControl.instance.ShotWhenReady(info);
        //Player.serverState = state;
    }

    public void LoadShot(ShotInfo _info)
    {
        info = new ShotInfoNet();
        info.Effect01 = _info.Effect01;
        info.X = _info.Target.x;
        info.Y = _info.Target.y;
        info.Z = _info.Target.z;
        info.TimeRatio = _info.TimeRatio;
        if(PowerupService.instance.usedShooterPowerup)
        {
            info.usedPower = (int)PowerupService.instance.ShooterPowerup;
        }
        else
        {
            info.usedPower = -1;
        }
    }

    public static ShotInfo UnloadShot(ShotInfoNet _info)
    {
        ShotInfo result = new ShotInfo();
        result.Effect01 = _info.Effect01;
        result.Target = new Vector3(_info.X, _info.Y, _info.Z);
        result.TimeRatio = _info.TimeRatio;
        return result;
    }
}
