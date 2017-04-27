using System;
using System.Timers;
using UnityEngine;

public class ApplicationEvents : IApplicationEvents, IDisposable  {

  public ApplicationEvents(){
    ServiceLocator.Register<IApplicationEvents>( this );
  }

  private event Action<AppEvent> sendEvent = null;

  public void RegisterListener(Action<AppEvent> listener) {
    sendEvent += listener;
  }

  public void UnregisterListener(Action<AppEvent> listener) {
    sendEvent -= listener;
  }

  public void ThrowEvent(AppEvent _appEvent) {
    if (sendEvent != null) {
      sendEvent( _appEvent );
    }
  }

  public void Dispose() {
    ServiceLocator.Remove<IApplicationEvents>();
  }
}
