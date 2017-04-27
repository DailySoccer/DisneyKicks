using UnityEngine;
using System.Collections;

public class Auto_Thrower : MonoBehaviour {
  public static Auto_Thrower instance { get; private set; }

  public float pauseTime = 2f;
  bool m_receivedDefense = false; //para añadir la excepcion de tirar antes de tiempo a la regla de no tirar cuando la interfaz esta bloqueada
  public Powerup queuedPowerUp;
  public bool powerUpEnqueued = false;

  void Awake() {
    instance = this;
  }

  void Start()
  {
    ServiceLocator.Request<IDefenseService>().RegisterListener(DefenseSent);
  }

  private float countDown = 0f;

  void DefenseSent(DefenseInfo _info)
  {
    if(BallPhysics.instance.state == BallPhysics.BallState.Waiting) m_receivedDefense = true; //para que tire siempre que se haya hecho ya la defensa
  }

  public void Ready() {
    SetTime(pauseTime);
  }

  void Update()
  {
    if(!GameplayService.networked)
    {
      if(countDown > 0f && ifcBase.activeIface != ifcAyudaInGame.instance && (!InputManager.instance.Blocked || m_receivedDefense))
      {
        countDown -= (Time.timeScale == 0f ? 0f: Time.deltaTime / Time.timeScale);
        if(countDown <= 0f) Shoot();
      }
    }
  }

  void doShoot() {
    if(powerUpEnqueued)
    {
        powerUpEnqueued = false;
        PowerupService.instance.UsePowerup(queuedPowerUp);
    }
    if(!GameplayService.networked && PowerupService.instance.IsPowerActive(Powerup.Concentracion))
    {
        Thrower.instance.shotInfo.TimeRatio = 0.75f;
    }
    Thrower.instance.DoThrow();
    GeneralSounds.instance.avisoDisparo();
  }

  public void SetTime(float _time)
  {
    countDown += _time;
  }

  void Shoot() {
    m_receivedDefense = false;
    ShotInfo shot = new ShotInfo();
    shot.TimeRatio = 1f;
    shot.Effect01 = ServiceLocator.Request<IDifficultyService>().GetEffect();
    float min = 0f;
    float max = 1f;
    if(MissionManager.instance.HasCurrentMission() && MissionManager.instance.GetMission().PlayerType == GameMode.GoalKeeper)
    {
        min = (MissionManager.instance.GetMission().GetRoundInfo() as GoalkeeperMissionRound).ShotRange.x;
        max = (MissionManager.instance.GetMission().GetRoundInfo() as GoalkeeperMissionRound).ShotRange.y;
    }
    shot.Target = Porteria.instance.GetRandomPoint(min, max);
    Thrower.instance.shotInfo = shot;
    Invoke ("doShoot", 0.75f);
  }

  public static ShotInfo GetRandomFail()
  {
    ShotInfo result = new ShotInfo();
    result.TimeRatio = 1f;
    bool horizontalFail = Random.Range(0f,1f) < 0.5f;
    float x = Random.Range(-1f * Porteria.instance.HalfHorizontalSize, Porteria.instance.HalfHorizontalSize);
    if(horizontalFail)
    {
      x = (Random.Range(0f,1f) < 0.5) ?
            Random.Range (Porteria.instance.HalfHorizontalSize + 1f, Porteria.instance.HalfHorizontalSize + 5f) :
            Random.Range (-1f * Porteria.instance.HalfHorizontalSize - 1f, -1f * Porteria.instance.HalfHorizontalSize - 5f) ;
    }
    float y = Random.Range(horizontalFail ? 0.1f : (Porteria.instance.VerticalSize + 1f), Porteria.instance.VerticalSize + 5f);
    result.Target = new Vector3(x, y, -49f);
    result.Effect01 = Random.Range(-0.5f, 0.5f);
    return result;
  }
}
