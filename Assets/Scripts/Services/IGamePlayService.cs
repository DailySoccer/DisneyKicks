using UnityEngine;

using System;

public enum GameMode {
  Shooter,
  GoalKeeper
}

public enum Difficulty {
  Easy,
  Medium,
  Hard
}

public struct GameInfo {
  /// <summary>
  /// Game mode ( Shooter | GoalKeeper ).
  /// </summary>
  public GameMode Mode { get; set; }

  /// <summary>
  /// Game difficulty ( Easy | Medium | Hard ).
  /// </summary>
  public Difficulty Difficulty { get; set; }
}

public interface IGameplayService {

  void ResetGravity();

  void ResetTime();

  Vector3 GetShotPosition();

  GameMode GetGameMode();

  void SetGameMode(GameMode mode);

  void SwitchGameMode();

  void SwitchAuto(bool active);

  bool GetAuto();

  Difficulty GetDifficulty();

  void SetDifficulty(Difficulty difficulty);

  void RegisterListener(Action<GameInfo> listener);

  void UnregisterListener(Action<GameInfo> listener);
}
