using UnityEngine;
using System.Collections;

public class Thrower : MonoBehaviour
{
  public static Thrower instance { get; private set; }

  public ShotInfo shotInfo;

  static AnimationDescriptorsResource m_animationDescriptorsResource;
  AnimationState m_animationState;

  Transform m_bip01;
  Transform m_ball;
  Transform m_feet;
  string m_idleAnim = "Idle01";

//  bool m_grab = false;

  string m_tipoTiro = "";

  void Awake()
  {
    instance = this;
    EquipacionManager.instance.PintarEquipacionesIngame(false, Thrower.instance.gameObject);
  }

  // Use this for initialization
  void Start()
  {
    ServiceLocator.Request<IShotResultService>().RegisterListener(resultAnimations);
    m_bip01 = transform.Find("Bip01");
    m_ball = m_bip01.Find("Balon");
    m_feet = m_bip01.Find("Bip01 Footsteps");

    if(m_animationDescriptorsResource == null) m_animationDescriptorsResource = Resources.Load("AnimationDescriptorsResource") as AnimationDescriptorsResource;
  }

  public void SetPositionFor(Vector3 _ball)
  {
    transform.position = _ball;
    if(Goalkeeper.instance != null) transform.LookAt(Goalkeeper.instance.transform.position);
    else transform.LookAt(Porteria.instance.transform.position);

    GetComponent<Animation>().Play("IdleTiroPenalti01");

    float distance = (BallPhysics.instance.transform.position - Porteria.instance.position).magnitude;
    if (distance > 35f)
      m_tipoTiro = "TiroFalta01";
    else if (distance > 25f)
      m_tipoTiro = "TiroFalta02";
    else
      m_tipoTiro = "TiroPenalti01";

    Vector3 pos = transform.localToWorldMatrix.MultiplyPoint( -m_animationDescriptorsResource.GetByName(m_tipoTiro).m_grabDiff );
    pos.y = 0;
    transform.position = pos;
  }



  public void DoThrow()
  {
    // si estamos en modo time_attack y se ha agotado el tiempo => no realizar tiros fuera de tiempo
    if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK && cntCronoTimeAttack.instance.tiempoRestante <= 0.0f)
      return;
    GetComponent<Animation>().CrossFade(m_tipoTiro); // Lanza animacion de tiro.
    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
    {
      kickEffects.instance.ballEffect(BallPhysics.instance.transform.position);
    }

    // en modo multijugador, si estoy manejando al tirador => forzar a que se aplique la camara de tiro
    if (GameplayService.networked && !GameplayService.IsGoalkeeper()) {
        GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.OnRun.instance;
    }

    m_animationState = GetComponent<Animation>()[m_tipoTiro];
  }

  public void resultAnimations(ShotResult _info)
  {
    if(this == null) return;

    bool isPlayer1 = GameplayService.initialGameMode != GameMode.Shooter;
    int score1 = isPlayer1 ? Player.serverState.score_1 : Player.serverState.score_2;
    int score2 = isPlayer1 ? Player.serverState.score_2 : Player.serverState.score_1;

    bool victory =(_info.Result == Result.Goal || _info.Result == Result.Target);
    bool gameOver = ServiceLocator.Request<IPlayerService>().IsGameOver();

    if(gameOver)
    {
        if(GameplayService.networked)
        {
            victory = (score1 > score2) && !GameplayService.IsGoalkeeper() || (score1 < score2) && GameplayService.IsGoalkeeper();
        }
        else
        {
            victory = ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts > 0;
            if(MissionManager.instance.GetMission().PlayerType == GameMode.GoalKeeper)
            {
                victory = !victory;
            }
        }
    }

    if(victory)
    {
        if(!gameOver)
        {
            GetComponent<Animation>().Play("Celebracion_01");
        }
        else
        {
            m_animationState = GetComponent<Animation>()["Celebracion_02"];
            GetComponent<Animation>().Play("Celebracion_02");
            m_idleAnim = "IdleCelebracionFinal";
        }
    }
    else
    {
      if(!gameOver)
      {
        if(Random.Range(0,1f) > 0.5f) GetComponent<Animation>().Play("Fallo_01");
      }
      else
      {
        Vector3 pos = m_bip01.transform.position;
        pos.y = m_feet.position.y;
        transform.position = pos;
        m_animationState = GetComponent<Animation>()["FalloFinal"];
        GetComponent<Animation>().Play("FalloFinal");
        m_idleAnim = "IdleFalloFinal";
      }
    }
  }

  void Update()
  {
    if (m_animationState != null && !m_animationState.enabled)
    {
      if(ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts > 0)
      {
        Vector3 pos = m_bip01.transform.position;
        pos.y = m_feet.position.y;
        transform.position = pos;
      }


      GetComponent<Animation>().Play(m_idleAnim);
      GetComponent<Animation>().Sample();
      m_animationState = null;
    }
  }

  public void EventHide(string _param) {
    ServiceLocator.Request<IShotService>().OnShotExecuted(Thrower.instance.shotInfo);
    if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
        GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.Fly.instance;
    else
        GoalCamera.instance.stateMachine.deferredState(ThrowerCameraStates.Fly.instance, 0.3f);
  }
}
