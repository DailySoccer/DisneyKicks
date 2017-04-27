using System;

public class DefenseService : IDefenseService, IDisposable {

  public DefenseService() {
    ServiceLocator.Register<IDefenseService>( this );
  }

  /// <summary>
  /// Delegate to register on to receive notifications
  /// on executed shots.
  /// </summary>
  private event Action<DefenseInfo> DefenseExecuted = null;

  public void RegisterListener(Action<DefenseInfo> listener) {
    DefenseExecuted += listener;
  }

  public void UnregisterListener(Action<DefenseInfo> listener) {
    DefenseExecuted -= listener;
  }

  public static DefenseInfo lastDefense;

  public void OnDefenseExecuted(DefenseInfo defenseInfo) {
    if (DefenseExecuted != null) {
      lastDefense = defenseInfo;
      DefenseExecuted( defenseInfo );
    }
  }

  public void Dispose() {
    DefenseExecuted = null;
    ServiceLocator.Remove<IDefenseService>();
  }
}
