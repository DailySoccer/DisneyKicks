using System;
using UnityEngine;

public enum Result {
  Goal,
  OutOfBounds,
  /// <summary>Goalkeeper catches the ball.</summary>
  Saved,
  /// <summary>Ball stops on the ground before anything interesting happens.</summary>
  Stopped,
  Target
}

public enum GKResult {
  Good, // El portero ha parado la bola (no la ha cogido).
  Perfect, // El portero ha atrapado la bola (es el puto amo).
  Late, // Fallo. El portero se ha tirado bien, pero tarde.
  Early, // Fallo. El portero se ha tirado bien, pero antes de tiempo.
  Fail, // Fallo. El portero se ha tirado mal.
  IAFail, // Fallo forzado. Este caso solo se da cuando jugamos de Lanzador y forzamos a que la IA falle.
  ThrowerFail, // El lanzador ha tirado la bola fuera o a un poste.
  Idle, // El portero no ha llegado a tirarse. Se ha quedao pasmao. Ni la ha olido.
}

public enum AreaResultValues {
  NoAreaExists,
  BallHitsArea,
  BallFailsArea
}

public struct ShotResult {
  public Result Result { get; set; }

  /// <summary>
  /// The point at which we decided the ball effected the result.
  /// If stopped, the point where it is at, if goal, the point
  /// in which it entered the goal, or exit the field.
  /// </summary>
  public Vector3 Point { get; set; }

  /// <summary>
  /// The ball has hit an obstacle on its path.
  /// </summary>
  public bool Rebounded { get; set; }

  public int ScorePoints { get; set; }

  public bool Perfect { get; set; }

  public int EffectBonusPoints { get; set; }

  public int Precision { get; set; }

  //public bool AreaFail { get; set; }
  public AreaResultValues AreaResult { get; set; }

  public GKResult DefenseResult { get; set; }  
}

public interface IShotResultService {

  void RegisterListener(Action<ShotResult> listener);
  void UnregisterListener(Action<ShotResult> listener);

  void OnShotEnded(ShotResult shotResult);

}
