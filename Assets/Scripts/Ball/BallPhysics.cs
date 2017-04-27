using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class BallPhysics : MonoBehaviour
{
  public enum BallState {
    Waiting,
    Flying,
    Idle,
    Cooldown
  }
  public static BallPhysics instance { get; private set; }
  public GameObject RightGoal;
  public float maxForcePower;       //Max and min initial power of the shot force
  public float minForcePower;
  public float maxResultForceX;     //Max X of the final result force
  public float maxForceResultY;
  public float minForceResultY;
  public float maxEffectPower;      //Max initial intensity of the effect force
  public float maxEffectForceX;     //Max X component of the result force
  public float maxAngularVelocity;
  public float sleepVelocity;
  public float sleepAngularVelocity;
  public float gestureXCoefficient; //Coefficient applied to the gesture direction
  public float gestureYCoefficient;
  public float torquePower;

  public bool noCatch = false;

  public LayerMask mask;

  BallState m_state = BallState.Waiting;
  public BallState state {
    get{return m_state;}
    set{m_state = value;}
  }


  bool m_locked = false;
  public bool locked {
    get{return m_locked;}
    set{m_locked = value;}
  }

  public const float maxEffectDeg = 35f;
  public const float minAngDeg = 7f;

  float effect = 0f;

  public Hexaedron ballHexaedron;

  Vector3 normal = Vector3.right;
  Vector3 m_reboundVector = Vector3.zero;

  //All of them depend on the distance
  float endPassTime;
  float deltaPassTime;
  float passTime;

  private bool ballAwake = false;

  void Awake() {
    instance = this;
  }

  void Start()
  {
    ServiceLocator.Request<IShotService>().RegisterListener(KickStarted);
    ServiceLocator.Request<IShotResultService>().RegisterListener(KickEnded);

    this.ballHexaedron = new Hexaedron(this.gameObject);

    GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
    GetComponent<Rigidbody>().sleepVelocity = sleepVelocity;
    GetComponent<Rigidbody>().sleepAngularVelocity = sleepAngularVelocity;
    //resetBall();
  }

  public bool ReboundBall()
  {
    if(PowerupService.instance.IsPowerActive(Powerup.Resbaladiza) && noCatch) return false;
    if (Goalkeeper.instance && Goalkeeper.instance.ClearBall && ! Goalkeeper.instance.GrabBall)
    {
      GeneralSounds.instance.rebound();
      GetComponent<Rigidbody>().velocity = m_reboundVector;
      if(GameplayService.IsGoalkeeper())
      {
        GeneralSounds.instance.cheer();
      }
      Goalkeeper.instance.MakeResult(false);
      FieldControl.instance.setRoundCooldown(1.5f);
      return true;
    }
    else return false;
  }
  
  void Update()
  {
    if(endPassTime > 0f)
    {
      endPassTime -= Time.deltaTime;
    }

    if (state == BallState.Flying)
    {
      if (endPassTime <= 0.0f)
      {
        ReboundBall();
      }
    }

    if(  state == BallState.Idle
      || state == BallState.Flying
      || state == BallState.Cooldown)
    {
      passTime += Time.deltaTime;
    }
  }

  const float maxIdleTime = 1.5f;
  float idleTime = 0f;

  void LateUpdate()
  {
    bool wasAwake = ballAwake;
    ballAwake = GetComponent<Rigidbody>().velocity.sqrMagnitude > 3f;

    if(state == BallState.Idle)
    {
      idleTime += Time.deltaTime;
      if(idleTime > maxIdleTime)
      {
        idleTime = 0f;
        ballAwake = false;
      }
    }
    else
    {
      idleTime = 0f;
    }
    if(idleTime > 0.5f)
    {
      wasAwake = true;
    }

    if (state == BallState.Idle && wasAwake && !ballAwake)
    {
      if(idleTime > 0.5f)
      {
          wasAwake = false;
          //if(ServiceLocator.Request<IGameplayService>().GetGameMode() != GameMode.GoalKeeper)
          //{
            ShotResult result = new ShotResult() { 
                Result = Result.Stopped, 
                Point = transform.position, 
                Rebounded = false,
                ScorePoints = 0,
                DefenseResult = GKResult.ThrowerFail
            };
            ServiceLocator.Request<IShotResultService>().OnShotEnded(result);
            FieldControl.instance.setRoundCooldown(1f);
      }
    }
  }

  void FixedUpdate()
  {
    if (state == BallState.Flying && ballAwake) {
      /*RaycastHit hit;
      if(Physics.SphereCast(transform.position, 0.11f, rigidbody.velocity, out hit, rigidbody.velocity.magnitude * Time.fixedDeltaTime, mask.value))
      {
        //transform.position = hit.point + hit.normal * 0.055f;
        rigidbody.velocity = hit.normal * 3f;
        state = BallState.Idle;
        if(!GameplayService.IsGoalkeeper())
        {
            GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.HitBarrera.instance;
        }
        if(hit.collider.animation != null) hit.collider.animation.Play();
        GeneralSounds.instance.barrera();
        if(hit.collider.tag == "Sabana")
        {
            kickEffects.instance.setSabana = true;
        }
        else
        {
            kickEffects.instance.setBarrera = true;
        }
        //Debug.Break();
      }*/

      GetComponent<Rigidbody>().velocity += normal * Time.fixedDeltaTime;
      GetComponent<Rigidbody>().maxAngularVelocity = float.MaxValue;
      GetComponent<Rigidbody>().AddTorque(Vector3.up * 100f * (effect + 0.1f) * Time.fixedDeltaTime * -1f, ForceMode.Force );
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Sabana")
    {
      if(collision.gameObject.GetComponent<Animation>() != null && !collision.gameObject.GetComponent<Animation>().isPlaying)
      {
        collision.gameObject.GetComponent<Animation>().Play();
      }
        if(!GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallState.Flying)
        {
            GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.HitBarrera.instance;
        }
      if(state != BallState.Cooldown) state = BallState.Idle;
      GeneralSounds.instance.barrera();
      kickEffects.instance.setSabana = true;
    }

    if(collision.gameObject.tag == "Poste")
    {
      GeneralSounds.instance.posteHit();
      if(state == BallState.Flying) kickEffects.instance.setPoste = true;
            if(!GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallState.Flying)
            {
                GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.HitBarrera.instance;
            }
            if(state != BallState.Cooldown) state = BallState.Idle;
    }

    if(collision.gameObject.tag == "Red")
    {
      GeneralSounds.instance.posteHit();
      if(state == BallState.Flying) kickEffects.instance.setPoste = true;
            if(!GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallState.Flying)
            {
                GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.HitBarrera.instance;
            }
            if(state != BallState.Cooldown) state = BallState.Idle;
    }

    if(collision.gameObject.tag == "Larguero")
    {
      GeneralSounds.instance.posteHit();
      if(state == BallState.Flying) kickEffects.instance.setLarguero = true;
            if(!GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallState.Flying)
            {
                GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.HitBarrera.instance;
            }
            if(state != BallState.Cooldown) state = BallState.Idle;
    }

    if(collision.gameObject.tag == "Barrera")
    {
      GeneralSounds.instance.posteHit();
      if(state != BallState.Cooldown) state = BallState.Idle;
      if(!GameplayService.IsGoalkeeper())
      {
          GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.HitBarrera.instance;
      }
      GeneralSounds.instance.barrera();
      kickEffects.instance.setBarrera = true;
    }

    if((collision.gameObject.tag == "Grass") && (state == BallState.Idle || state == BallState.Cooldown)) GeneralSounds.instance.miniHit();
    if(collision.gameObject.tag == "Red" && (state == BallState.Flying))
    {
      GeneralSounds.instance.redHit();
    }

    if(state == BallState.Flying && (collision.gameObject.tag != "Grass"))
    {
        if(state != BallState.Cooldown) state = BallState.Idle;
    }
  }

  public Vector3 predictedHit = Vector3.zero;

  void OnDrawGizmos()
  {
    Gizmos.DrawSphere(predictedHit, 0.15f);
  }

  void KickStarted(ShotInfo _info)
  {
      ServiceLocator.Request<IGameplayService>().ResetTime();

      if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
          GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.Hit.instance;
      else
          GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Hit.instance;

    state = BallState.Flying;
    m_locked = true;

    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter ) {
      if(Goalkeeper.instance && GameplayService.networked) _info.Target = Goalkeeper.instance.ApplyIADefense(_info.Target, GameplayService.networked);
      else if(Goalkeeper.instance)
      {
        Goalkeeper.instance.ApplyIADefense(_info.Target, false);
        //Goalkeeper.instance.SimulateDefense(_info.Target, false);
        //Goalkeeper.instance.DoDefense();
      }
    }
    else {
      _info.Target = Goalkeeper.instance.SimulateDefense(_info.Target, !GameplayService.networked);
    }

    predictedHit = _info.Target;
    kickEffects.instance.ballPoint = predictedHit;

    Vector3 dir = transform.position - _info.Target;
    Vector3 dir2;
    float d = dir.magnitude;
    float ang = AnguloMinimo(_info.Target) * 2f * Mathf.Clamp(_info.TimeRatio, 0.5f, 1f);//((GameplayService.networked || GameplayService.IsGoalkeeper()) ? 2f : 1f);
    float angEfecto = _info.Effect01 * BallPhysics.instance.MaxEffect();
    effect = _info.Effect01;
    float h = _info.Target.y - transform.position.y;

    // Calcula la velocidad inicial que necesito para llegar desde donde esta el balon a destino con el angulo indicado
    float g = -Physics.gravity.y;
    float a = Mathf.Tan(ang) - (h / d);

    if (a < 0)
    {
      Debug.Log("ERROR1111!!! ang " + ang + " h " + h + " d " + d);
      //a = Mathf.Tan(ang);
      //return;
    }
    float c = Mathf.Cos(ang);
    float b = 2 * c * c * a;
    if (b < 0.1f)
    {
      Debug.Log("ERROR2222!!!");
      b = 0.1f;
    }

    float v0 = Mathf.Sqrt((g * d) / b);

    //float deltaPassTime = ((d * (1f + Mathf.Sin(ang))) / v0);
    deltaPassTime = (d / (v0 * c));
    endPassTime = deltaPassTime;
    passTime = 0;

    float v1 = ((d * Mathf.Sin(angEfecto)) / deltaPassTime);

    dir = Quaternion.Euler(0.0f, Mathf.Atan2(dir.z, -dir.x) * Mathf.Rad2Deg, ang * Mathf.Rad2Deg) * Vector3.right;
    dir2 = Quaternion.Euler(0, angEfecto * Mathf.Rad2Deg, 0) * new Vector3(dir.z * -1, dir.y, dir.x);

    normal = (-dir2 * v1) / deltaPassTime * 2;
    GetComponent<Rigidbody>().velocity = dir * v0 + dir2 * v1;

    if(Goalkeeper.instance) m_reboundVector = _info.Target - Goalkeeper.instance.transform.position;
    m_reboundVector *= Random.Range(2.0f, 2.5f);
    m_reboundVector.z *= Random.Range(2.0f, 2.5f);

    if(PowerupService.instance.IsPowerActive(Powerup.Destello)) kickEffects.instance.Destello(deltaPassTime - 0.25f);

    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) {
      if(Goalkeeper.instance)
      {
        if(!GameplayService.networked) Goalkeeper.instance.TryGrab(deltaPassTime/*endPassTime*/);
        else Goalkeeper.instance.FakeGrabTime(deltaPassTime);
      }
    }
    else {
      float deltaGrab = Goalkeeper.instance.SetBallTime(deltaPassTime/*endPassTime*/);
      //kickEffects.instance.aracnoSense(_info.Target, deltaGrab);
      ServiceLocator.Request<IDifficultyService>().ConfigCrossHair(_info.Target, deltaGrab);
    }
  }

  public float passTimeNormalized()
  {
    if (state == BallState.Flying || state == BallState.Idle) return passTime / deltaPassTime;
    return 0;
  }


  void KickEnded(ShotResult _result)
  {
    state = BallState.Cooldown;
    // Pone la camara en cooldown.
    if(ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts > 0)
    {
      if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
          GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.CoolDown.instance;
      else
          GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.CoolDown.instance;
    }

    Physics.gravity = new Vector3(0f,-9.8f, 0f);
  }


  float ClampAbsoluteValue(float value, float maxValue)
  {
    if (Mathf.Abs(value) > maxValue)
    {
      value = maxValue * (value < 0 ? -1 : 1);
    }
    return value;
  }

  public float AnguloMinimo(Vector3 target) {
    Vector3 proyeccionHorizontal = new Vector3(target.x, transform.position.y, target.z);
    float distanciaL = (proyeccionHorizontal - transform.position).magnitude;
    float anguloMinimo = Mathf.Atan2(target.y - transform.position.y, distanciaL);
    if(anguloMinimo < (minAngDeg * Mathf.Deg2Rad)) anguloMinimo = (minAngDeg * Mathf.Deg2Rad);
    return anguloMinimo*1.2f;
  }

  public float MaxEffect()
  {
    float distancia = (transform.position - Porteria.instance.position).magnitude;
    float result = Mathf.Lerp(0, maxEffectDeg, Mathf.Clamp01(distancia / 40f));
    return result * Mathf.Deg2Rad;
  }

  public void Prepare(Vector3 _position)
  {
    ServiceLocator.Request<IGameplayService>().ResetGravity();
    GetComponent<Rigidbody>().velocity = Vector3.zero;
    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    transform.position = _position;
    state = BallState.Waiting;
    kickEffects.instance.StopRoundEffects();
    noCatch = false;
  }

}
