using UnityEngine;

[NetMessage( MsgType.StartMatch )]
class MsgStartMatch : MensajeBase {
    public bool startsGoalkeeper;
    public MsgStartMatch() { }
    public override void process()
    {
        Debug.LogWarning(">>> MsgStartMatch: proceso mensaje");

        /*
        // comprobar si el jugador tiene pendiente pagar este duelo 
        if (Interfaz.m_tegoQuePagarDuelo) {
            // enviar el pago del duelo a los web services
            Debug.LogWarning(">>> MsgStartMatch: envio pago");
            Interfaz.instance.pagarDuelo();
        }
        */

        if(!startsGoalkeeper) GameplayService.initialGameMode = GameMode.GoalKeeper;
        else GameplayService.initialGameMode = GameMode.Shooter;

        // ifcDuelo.instance.ShowVs(ifcDuelo.m_rival, false);
		ifcDuelo.instance.DuelAccepted(ifcDuelo.m_rival);
    }
}