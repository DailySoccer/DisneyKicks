using System;
using UnityEngine;

public struct DefenseInfo {

  /// <summary>
  /// Defense target.
  /// </summary>
  public Vector3 Target;
  /// <summary>
  /// Forced result (for IA and network)
  /// </summary>
  public bool ForcedResult;
  /// <summary>
  /// Defense time (for network)
  /// </summary>
  public float DefTime;

  public GKResult Result;
}

public interface IDefenseService {

  void RegisterListener(Action<DefenseInfo> listener);
  void UnregisterListener(Action<DefenseInfo> listener);

  void OnDefenseExecuted(DefenseInfo defenseInfo);
}