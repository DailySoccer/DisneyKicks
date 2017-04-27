/*using System.Collections.Generic;
using UnityEngine;

[NetMessage( MsgType.PlayerReactions )]
class MsgPlayerReactions : MensajeBase {
    public FieldControl.MatchState matchState;
    public PlayerReactions[] m_playersReactions;

    public MsgPlayerReactions() {
    }

    public override void process() {
        // TODO: Casificar las reacciones en cada jugador.
        foreach (PlayerReactions reaction in m_playersReactions) {
            reaction.process();
        }

        FieldControl.instance.nextMatchState = matchState;
        FieldControl.instance.controlMode = FieldControl.ControlMode.reactions;

        switch (matchState) {                
            case FieldControl.MatchState.KickOff:
                BallControl.instance.owner = null;
                BallControl.instance.position = Vector3.zero;
                break;
        }


    }
}*/
