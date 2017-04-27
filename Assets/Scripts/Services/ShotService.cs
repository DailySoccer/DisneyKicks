using System;

public class ShotService : IShotService, IDisposable {

  public static ShotInfo lastShot;

  public ShotService() {
    ServiceLocator.Register<IShotService>( this );
  }

  /// <summary>
  /// Delegate to register on to receive notifications
  /// on executed shots.
  /// </summary>
  private event Action<ShotInfo> ShotExecuted = null;

  public void RegisterListener(Action<ShotInfo> listener) {
    ShotExecuted += listener;
  }

  public void UnregisterListener(Action<ShotInfo> listener) {
    ShotExecuted -= listener;
  }

  public void OnShotExecuted(ShotInfo shotInfo) {
    lastShot = shotInfo;
    if (ShotExecuted != null) {
      ShotExecuted( shotInfo );
    }
  }

  public void Dispose() {
    ShotExecuted = null;
    ServiceLocator.Remove<IShotService>();
  }
}
