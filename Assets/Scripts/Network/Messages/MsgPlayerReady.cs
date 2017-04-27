using UnityEngine;

[NetMessage( MsgType.ReadyPlayer )]
class MsgPlayerReady : MensajeBase {

    public MsgPlayerReady() { }

    public override void process()
    {
    }
}