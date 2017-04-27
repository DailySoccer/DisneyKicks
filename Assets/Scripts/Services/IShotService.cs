using System;
using UnityEngine;

public struct ShotInfo {
  /// <summary>
  /// Effect applied to the ball, in the horizontal plane.
  /// </summary>
  public float Effect01 { get; set; }

  /// <summary>
  /// Shot target.
  /// </summary>
  public Vector3 Target { get; set; }

  public float TimeRatio { get; set; }
}

public interface IShotService {

  void RegisterListener(Action<ShotInfo> listener);
  void UnregisterListener(Action<ShotInfo> listener);

  void OnShotExecuted(ShotInfo shotInfo);
}
