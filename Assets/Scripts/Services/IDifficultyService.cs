using UnityEngine;
using System;

public struct ShotConfiguration {
  public GameMode Mode { get; set; }
  public Vector3 Position { get; set; }
  public Bullseye[] Bullseyes { get; set; }
  public Difficulty Difficulty {get; set; }
  public int Fase {get; set; }
  public bool IsNewFase {get; set; }
}

public interface IDifficultyService {
  ShotConfiguration GetNextShotConfig();
  ShotConfiguration GetLastShotConfig();

  void RegisterListener(Action<ShotConfiguration> listener);
  void UnregisterListener(Action<ShotConfiguration> listener);
  void ConfigCrossHair( Vector3 _position, float time );
  float GetSuccessRadius(Vector3 _target);
  float GetPerfectRadius();
  int GetFase();
  int GetRounds();
  int GetMultiplier();
  float GetCoordenadaXTiro(float _ballPosX, float _porteriaLocalScaleX);

  float GetEffect();
  float GetPanTime();

  Rect GetRect();

  void NextFase();
  void PrevFase();

  void OnShotFinished(ShotResult _shotResult);

  Bullseye GetBullseye();
}
