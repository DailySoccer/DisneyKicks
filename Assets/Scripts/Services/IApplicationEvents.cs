using UnityEngine;
using System;

public enum AppEvent {
  Start,
  GameOverEnqueued
}

public interface IApplicationEvents {

  void RegisterListener(Action<AppEvent> listener);
  void UnregisterListener(Action<AppEvent> listener);

  void ThrowEvent(AppEvent _appEvent);
}
