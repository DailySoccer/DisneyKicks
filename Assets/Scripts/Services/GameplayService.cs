using UnityEngine;
using System;

public class GameplayService : IGameplayService {

  public static GameMode initialGameMode = GameMode.Shooter;
  public static ModoJuego modoJuego = InfoModosJuego.instance.GetModoJuego("SHOOTER_NORMAL_MODE"); // <= modo por defecto

  public static GameLevelMission gameLevelMission;

  public static bool networked = false;

  GameMode gameMode = GameMode.Shooter;
  Difficulty difficulty = Difficulty.Easy;
  int successfullyShots = 0;
  int shotsToMediumDificulty = 10;
  int shotsToHardDificulty = 20;

  bool auto = false;

  public GameplayService() {
    ServiceLocator.Register<IGameplayService>( this );

    ServiceLocator.Request<IShotResultService>().RegisterListener( TryShotResult );
  }

  /// <summary>
  /// Delegate to register on to receive notifications
  /// on executed shots.
  /// </summary>
  private event Action<GameInfo> GameModeModified = null;

  public void RegisterListener(Action<GameInfo> listener) {
    GameModeModified += listener;
  }

  public void UnregisterListener(Action<GameInfo> listener) {
    GameModeModified -= listener;
  }

  public Vector3 GetShotPosition() {
    return AreaManager.GetRandomPoint( this.difficulty, gameMode);
  }

  public GameMode GetGameMode() {
    return gameMode;
  }

  public void SetGameMode(GameMode mode) {
//se retira la ayuda ingame
/*
        if(mode == GameMode.Shooter)
        {
            ifcAyudaInGame.instance.Show(ifcAyudaInGame.mode.iniesta);
        }
        else
        {
            ifcAyudaInGame.instance.Show(ifcAyudaInGame.mode.casillas);
        }
*/

    if(gameMode != mode)
    {
      gameMode = mode;
      SendModification();
    }

    ResetTime();
    ResetGravity();
    ResetCamera();
  }

  public static bool IsGoalkeeper()
  {
    return ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper;
  }

  public void ResetGravity()
  {
    if(gameMode == GameMode.Shooter)
    {
      Physics.gravity = new Vector3(0f,-6f, 0f);
    }
    else
    {
      Physics.gravity = new Vector3(0f,-6.0f, 0f);
    }
  }

  public void ResetTime()
  {
    if(gameMode == GameMode.Shooter)
    {
      Time.timeScale = 1f;
    }
    else
    {
      Time.timeScale = 0.8f;
      if(PowerupService.instance.IsPowerActive(Powerup.TiempoBala))
      {
        Time.timeScale = 0.45f;
      }
    }
  }

    public void ResetCamera()
    {
        if(gameMode == GameMode.Shooter)
        {
            Camera.main.fieldOfView = 45f;
        }
        else
        {
            float ratio = Camera.main.aspect;
            Camera.main.fieldOfView = Mathf.Clamp((45f - ((45f - 33f)/((16f/9f) - (4f/3f))) * (ratio - (4f/3f))), 20f, 50f);
            //Camera.main.transform.rotation = Quaternion.Euler((7f / ((16f/9f) - (4f/3f)))*(ratio - (4f/3f)),0f,0f); 
        }
    }

  public bool GetAuto() {
    return this.auto;
  }

  public void SwitchAuto(bool active) {
    this.auto = active;
  }

  public void SwitchGameMode() {
    FieldControl.instance.goalKeeper = true;
    this.SetGameMode(gameMode == GameMode.Shooter ? GameMode.GoalKeeper : GameMode.Shooter);
  }

  void SendModification() {
    GameInfo info = new GameInfo();
    info.Mode = gameMode;
    info.Difficulty = difficulty;
    GameModeModified(info);
  }

  public Difficulty GetDifficulty() {
    return this.difficulty;
  }

  public void SetDifficulty(Difficulty difficulty) {
    this.difficulty = difficulty;
  }

  public void ResetSuccessfullyShots() {
    this.successfullyShots = 0;
    SetDifficulty( Difficulty.Easy );
  }

  public void SuccessfullyShotDone() {
    this.successfullyShots++;
    if (this.successfullyShots >= shotsToMediumDificulty) {
      SetDifficulty( Difficulty.Medium );
    } else if (this.successfullyShots >= shotsToHardDificulty) {
      SetDifficulty( Difficulty.Hard );
    }
  }

  public void SetShotsToMediumDificulty(int _value) {
    this.shotsToMediumDificulty = _value;
  }

  public void SetShotsToHardDificulty(int _value) {
    this.shotsToHardDificulty = _value;
  }

  public void TryShotResult(ShotResult shotResult) {
    if (shotResult.Result == Result.Goal) {
      SuccessfullyShotDone();
    }
  }
}
