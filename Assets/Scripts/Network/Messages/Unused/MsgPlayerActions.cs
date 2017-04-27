/*using System.Collections.Generic;
using UnityEngine;

[NetMessage( MsgType.PlayerActions )]
class MsgPlayerActions : MensajeBase {

    /// <summary>
    /// Do not touch. Needed for serialization.
    /// </summary>
    public bytes bytes;

    public IEnumerable<PlayerAction> Actions { get; set; }

    public MsgPlayerActions() {
        Actions = new List<PlayerAction>();
    }

    public override bool send() {
        bytes = PlayerAction.SerializeActions( this.Actions );

        return base.send();
    }
}*/
