using System;
using UnityEngine;

public class ShotResultService : IShotResultService, IDisposable {
  //bool firstShot = true; //para los caballeros que hacen BI

  public static bool noDefense = true;

  public ShotResultService() {
    ServiceLocator.Register<IShotResultService>( this );
  }

  private event Action<ShotResult> ShotFinished = null;

  public void RegisterListener(Action<ShotResult> listener) {
    ShotFinished += listener;
  }

  public void UnregisterListener(Action<ShotResult> listener) {
    ShotFinished -= listener;
  }

  public void OnShotEnded (ShotResult shotResult)
  {
    BallPhysics.instance.state = BallPhysics.BallState.Cooldown;

    UnityEngine.Debug.Log( "RESULTADO: " + shotResult.Result +
                           " PUNTOS: " + shotResult.ScorePoints + 
                           " MINUTOS: " + FieldControl.instance.minutes + 
                           " GK_RESULTADO " + shotResult.DefenseResult);

      // si estamos en modo time_attack y el tiro ha llegado fuera de tiempo
    if ( GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK && 
         cntCronoTimeAttack.instance.tiempoRestante <= 0.0f ) {
        Debug.LogWarning( ">>> TIRO FUERA DE TIEMPO" );

        if ( ShotFinished != null ) {
            ShotFinished( shotResult );
        }
        return;
    }

    if ( ( GameplayService.networked ) && 
         ( !Goalkeeper.instance.m_networkDefenseSent ) && 
         ( ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper ) ) {

        MsgDefend msg = Shark.instance.mensaje<MsgDefend>();
        /*
        msg.info.success = shotResult.Result == Result.OutOfBounds
                        || shotResult.Result == Result.Saved
                        || shotResult.Result == Result.Stopped;
        msg.info.late = true;
        msg.info.precisionFail = false;
        msg.info.perfect = false;
        msg.info.noDefense = noDefense;
        */
        msg.LoadDefense( DefenseService.lastDefense.Target, shotResult.DefenseResult );
        if ( noDefense ) {
            msg.info.noDefense = true;
        }
        msg.points = shotResult.ScorePoints;// + shotResult.ScorePoints;
        msg.send();
    }

    // Calculamos la puntuacion correspondiente al resultado del tiro y la sumamos
    ScoreManager.Instance.CalculateScore( ref shotResult );
    ScoreManager.Instance.AddScore( shotResult );


    if (!GameplayService.networked) {
        MissionStats.Instance.OnShotResult(shotResult);
    }

    ServiceLocator.Request<IDifficultyService>().OnShotFinished(shotResult);
    ServiceLocator.Request<IPlayerService>().TryShotResult(shotResult);

    if(ServiceLocator.Request<IPlayerService>().IsGameOver())
    {
        FieldControl.instance.GameOver();
    }

    // Lanzamos el evento de ShotFinished, para notificar a todos los interesados
    // que el tiro ya se ha resuelto
    if ( ShotFinished != null ) {
      ShotFinished( shotResult );
    }
  }

  public void Dispose() {
    ServiceLocator.Remove<IShotResultService>();
  }
}
