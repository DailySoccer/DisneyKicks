using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

public class InputManager : MonoBehaviour {

    /*public static float throwerGestureTime
    {
        set{}
        get
        {
            float res = 0.8f;
            if(PowerupService.instance.IsPowerupActive(Powerup.
            return 0.8f;
        }
    }*/
    public const float goalkeeperGestureTime = 1.5f;
    public const float throwerGestureBaseTime = 0.8f;
    public float throwerGestureTime// = 0.8f;
    {
        get
        {
            return(PowerupService.instance.IsPowerActive(Powerup.Concentracion) ? throwerGestureBaseTime + 1f : throwerGestureBaseTime);
        }

        set{}
    }

    public static InputManager instance { get; private set; }

  public struct PanInfo {
    public Vector3 Position;
    public float TimeStamp;
    
    public PanInfo(Vector3 pos, float time) {
      Position = pos;
      TimeStamp = time;
    }
  }

  List<PanInfo> m_panPoints = new List<PanInfo>();
  public float m_timeToThrow = 0;
  public float deltaTimeToThrow = 0.5f;
  float m_minSQRDistanceRange = 0.03f * 0.03f;
  const float maxElevationDegrees = 45;
  const float maxAlturaTarget = 5.5f;
  public GameObject m_pointPrefab;
  public bool m_initializedGesture = false;
  public bool m_finishedGesture = false;

  float m_gestureTime = 0.0f;
  float m_lastGestureTime = 0.0f;

  int m_blocked = 0;
  public bool Blocked
  {
    get{return m_blocked > 0;}
    set
    {
      if(value) m_blocked++;
      else
      {
        //if(m_blocked == 0) throw new System.InvalidOperationException("NOOOOOO!!!!");
        m_blocked--;
        if(m_blocked < 0) m_blocked = 0;
      }
    }
  }

  LineRenderer gestureLine;
  Vector2 m_lastScreenPosition;

  void Awake()
  {
      instance = this;
  }

  void Start () {
    GetComponent<PanGesture>().StateChanged += EventOnPan;
    GetComponent<TapGesture>().StateChanged += EventOnTap;

    gestureLine = transform.Find("GestureLine").GetComponent<LineRenderer>();

    ServiceLocator.Request<IShotResultService>().RegisterListener(ShotFinished);
  }

  void ClearEffects()
  {
    m_panPoints.Clear();
    InputChrono.instance.GetComponent<GUIText>().enabled = false;
    //kickEffects.instance.Focus(false);
  }

  void InitGesture() {
    if(PowerupService.instance.IsPowerActive(Powerup.Concentracion) && !GameplayService.IsGoalkeeper())
    {
        InputChrono.instance.GetComponent<GUIText>().enabled = true;
        InputChrono.instance.time = throwerGestureTime;
    }

    /*if(PowerupService.instance.IsPowerActive(Powerup.Manoplas))
    {
        kickEffects.instance.Focus(true);
    }*/


    m_initializedGesture = false;
    m_finishedGesture = false;
    m_gestureTime = (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) ? throwerGestureTime : goalkeeperGestureTime;
    m_panPoints.Clear();
  }

  private void EventOnTap(object sender, TouchScript.Events.GestureStateChangeEventArgs e) {
    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) return;
    TapGesture tap = (TapGesture)sender;
    switch(e.State) {
      case Gesture.GestureState.Began:
      break;
      
      case Gesture.GestureState.Changed:
      break;
      
      case Gesture.GestureState.Ended:
      if(BallPhysics.instance.state == BallPhysics.BallState.Flying)
        sendDefense(tap.ScreenPosition);
      break;
    }
  }

  private void EventOnPan(object sender, TouchScript.Events.GestureStateChangeEventArgs e)
  {
    if(Blocked) return;
    bool playingGoalkeeper = GameplayService.IsGoalkeeper();
    if(playingGoalkeeper && BallPhysics.instance.state != BallPhysics.BallState.Flying && BallPhysics.instance.state != BallPhysics.BallState.Waiting && BallPhysics.instance.state != BallPhysics.BallState.Idle) return;
    if(ifcBase.activeIface == ifcAyudaInGame.instance) return;
    PanGesture panGest = (PanGesture)sender;
    switch(e.State) {
      case Gesture.GestureState.Began:
        InitGesture();
        if(!playingGoalkeeper)
        {
          Vector2 ballPos = new Vector2(Camera.main.WorldToScreenPoint(BallPhysics.instance.transform.position).x, Camera.main.WorldToScreenPoint(BallPhysics.instance.transform.position).y);
          float inputArea = (3500f * Mathf.Clamp(ifcBase.scaleFactor,1f,float.MaxValue) * (PowerupService.instance.IsPowerActive(Powerup.Sharpshooter)? 15f : 1f));
#if UNITY_ANDROID || UNITY_IPHONE
          float dpi = (float)Screen.dpi;
          if(dpi < 25 || dpi > 1000)
          {
            dpi = 100;
          }
          inputArea *= dpi / 100f;
#endif
          if((panGest.ScreenPosition - ballPos).sqrMagnitude > inputArea)
          {
            m_finishedGesture = true;
            m_initializedGesture = false;
            return;
          }
          else
          {
            m_initializedGesture = true;
          }
        }
        else
        {
          Vector3 gkPosition = Goalkeeper.instance.transform.FindChild("Body").GetComponent<Renderer>().bounds.center;
          Vector2 gkPos = new Vector2(Camera.main.WorldToScreenPoint(gkPosition).x,Camera.main.WorldToScreenPoint(gkPosition).y);

          float inputArea = (3500f * Mathf.Clamp(ifcBase.scaleFactor,1f,float.MaxValue));
#if UNITY_ANDROID || UNITY_IPHONE
          float dpi = (float)Screen.dpi;
          if(dpi < 25 || dpi > 1000)
          {
            dpi = 100;
          }
          inputArea *= dpi / 100f;
#endif
          if((panGest.ScreenPosition - gkPos).sqrMagnitude > (inputArea))
          {
            m_finishedGesture = true;
            m_initializedGesture = false;
            return;
          }
          else
          {
            m_initializedGesture = true;
          }
        }
        addPoint(new PanInfo(panGest.ScreenPosition, Time.time));
        break;
      
      case Gesture.GestureState.Changed:
        if(m_finishedGesture) return;
        if(playingGoalkeeper)
        {
          m_lastScreenPosition = panGest.ScreenPosition;
        }

        /*if(m_panPoints.Count <= 1) {
          addPoint(new PanInfo(panGest.ScreenPosition, Time.time));
          return;
        }*/

        Vector2 direction = panGest.PreviousScreenPosition - panGest.ScreenPosition;
        if (direction.sqrMagnitude > m_minSQRDistanceRange)
        {
          addPoint(new PanInfo(panGest.ScreenPosition, Time.time));
        }

        updateLine();
        break;

      case Gesture.GestureState.Ended:
        //addPoint(new PanInfo(panGest.ScreenPosition, Time.time));
        if(m_finishedGesture) return;


        if(playingGoalkeeper && (BallPhysics.instance.state == BallPhysics.BallState.Flying || BallPhysics.instance.state == BallPhysics.BallState.Waiting || BallPhysics.instance.state == BallPhysics.BallState.Idle)) sendDefense(m_lastScreenPosition);
        else if (BallPhysics.instance.state == BallPhysics.BallState.Waiting) EndPan();

        ClearEffects();
        break;
    }
  }

  void ShotFinished(ShotResult _info)
  {
    m_finishedGesture = true;
    m_initializedGesture = false;
    ClearEffects();
  }

  void addPoint(PanInfo _panInfo)
  {
    if(m_panPoints.Count == 0 && !GameplayService.IsGoalkeeper())
    {
      PanInfo info = new PanInfo(Camera.main.WorldToScreenPoint(BallPhysics.instance.transform.position), Time.time);
      m_panPoints.Add(info);
    }
    else m_panPoints.Add(_panInfo);
    /*float d = m_panPoints[0].Position.x - _panInfo.Position.x;
    float ad = Mathf.Abs(d);

    if (ad > m_maxAbsEffect) {
      m_maxAbsEffect = ad;
      m_maxEffect = d;
    }*/
  }

    const int precision = 5;
    bool GetEffectSentido()
    {
        int clockwise = 0;
        int counter = 0;
        
        for(int i = precision; i < m_panPoints.Count-precision; i++)
        {
            PanInfo p = m_panPoints[i];
            PanInfo p_ = m_panPoints[i-precision];
            PanInfo p1 = m_panPoints[i+precision];
            Vector2 a = p_.Position - p.Position;
            Vector2 b = p.Position - p1.Position;
            Vector3 cross = Vector3.Cross(a, b);

            if(cross.z > 0) clockwise++;
            else counter ++;
        }
        return clockwise > counter;
    }

  float GetEffectDeviation() {
    if(m_panPoints.Count < precision + 1) return 0;

    bool gestoDerecha = false;
    gestoDerecha = GetEffectSentido();

    Vector3 vectorRecto = (m_panPoints[m_panPoints.Count-1].Position - m_panPoints[0].Position);
    float cA = vectorRecto.y;
    float cB = vectorRecto.x * -1;

    float distanciaExtrema = 0f;

    foreach(PanInfo info in m_panPoints) {
      Vector2 point = info.Position - m_panPoints[0].Position;
      float distanciaPunto = Mathf.Abs(cA*point.x + cB*point.y);
      if(distanciaExtrema < distanciaPunto)
      {
        distanciaExtrema = distanciaPunto;
      }
    }
    Rect extremos = ExtremosCurva();
    distanciaExtrema /= extremos.height;
    float efecto = Mathf.Clamp01(Mathf.Abs(distanciaExtrema/extremos.height));
    efecto *= (gestoDerecha ? -1f : 1f);
    Debug.Log("Curvatura: " + (efecto*100f).ToString() + "%  efecto: " + (efecto * BallPhysics.instance.MaxEffect()*Mathf.Rad2Deg)+"º  Max: " + (BallPhysics.instance.MaxEffect()*Mathf.Rad2Deg));

    //VERSION ANTERIOR
    /*Vector2 v0 = m_panPoints[1].Position - m_panPoints[0].Position;
    Vector2 v1 = m_panPoints[m_panPoints.Count - 1].Position - m_panPoints[0].Position;
    Vector3 normal = Vector3.Cross(v0, v1);
    return (normal.z < 0 ? m_maxAbsEffect : -m_maxAbsEffect) / 250.0f;*/

    return efecto;
  }

  Rect ExtremosCurva()
  {
    float winnerX = float.MinValue;
    float winnerY = float.MinValue;
    float loserX = float.MaxValue;
    float loserY = float.MaxValue;
    foreach(PanInfo info in m_panPoints)
    {
      Vector2 temp = info.Position - m_panPoints[0].Position;
      if(temp.x > winnerX) winnerX = temp.x;
      if(temp.x < loserX) loserX = temp.x;
      if(temp.y > winnerY) winnerY = temp.y;
      if(temp.y < loserY) loserY = temp.y;
    }
    Rect result = new Rect(loserX,loserY,0,0);
    result.xMax = winnerX;
    result.yMax = winnerY;
    return result;
  }

  float LengthCurva()
  {
    float length = 0f;
    for(int i = 1 ; i < m_panPoints.Count ; i++)
    {
      length += Vector2.Distance(m_panPoints[i].Position, m_panPoints[i-1].Position);
    }
    return length;
  }

  static Plane planeGoal = new Plane(new Vector3(0, 0, 1), 49.5f );
  public static Plane planeGoalClose = new Plane(new Vector3(0, 0, 1), 49f );
  public static Plane planeGoalExact = new Plane(new Vector3(0, 0, 1), 50.5f );
  static Plane planeFloor = new Plane(Vector3.up, 0);

  Vector3 projectFloor(Vector2 _point)
  {
    Ray ray = Camera.main.ScreenPointToRay(_point);
    float distance;
    planeFloor.Raycast(ray, out distance);
    return ray.GetPoint(distance);
  }

  void EndPan(bool _ok=true) {
    bool playingGoalKeeper = GameplayService.IsGoalkeeper();
    //m_initializedGesture = false;
    if(playingGoalKeeper)
    {
        return;
    }

    if (m_finishedGesture)
    {
      Debug.Log("<TIRO NO>");

      m_panPoints.Clear();
      m_initializedGesture = false;
      return;
    }

    m_finishedGesture = true;
    m_gestureTime = 0f;
    if(LengthCurva() < 100f * ifcBase.scaleFactor)
    {
      //Camera.main.fieldOfView =45;
      m_initializedGesture = false;
      m_panPoints.Clear();
      return;
    }

    if (!playingGoalKeeper && !_ok && m_panPoints.Count<3)
    {
        m_initializedGesture = false;
        m_panPoints.Clear();
        return;
    }

    Blocked = true;
    sendShoot ();

    if(GameplayService.networked && GoalCamera.instance.stateMachine.current == ThrowerCameraStates.Sharpshooter.instance)
    {
        GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Init.instance;
    }
  }

  void sendShoot() {
    bool playingGoalkeeper = ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper;

    float dd;
    Vector3 target = Vector3.zero;
    bool stopped = false;
    Ray rd = Camera.main.ScreenPointToRay(m_panPoints[m_panPoints.Count -1].Position);
    float efecto = GetEffectDeviation();

    if(!playingGoalkeeper && Goalkeeper.instance)
    {
      if(!GameplayService.networked) planeGoal.Raycast(rd, out dd);
      else planeGoalClose.Raycast(rd, out dd);
      target = rd.origin + (rd.direction * dd);
      if(target.y < 0.1f) target.y = 0.1f;
      else if (target.y > 5f) target.y = 5f;
      if(target.x < -25f) target.x = -25f;
      else if(target.x > 25f) target.x = 25f;
      stopped = Goalkeeper.instance.IADefense(target, efecto);
    }

    if(!stopped && !GameplayService.networked)
    {
      planeGoalExact.Raycast(rd, out dd);
      target = rd.origin + (rd.direction * dd);
      if(target.y < 0.2f) target.y = 0.2f;
      else if (target.y > 10f) target.y = 10f;
      if(target.x < -25f) target.x = -25f;
      else if(target.x > 25f) target.x = 25f;
    }

    ShotInfo shotInfo = new ShotInfo();
    shotInfo = new ShotInfo();
    shotInfo.Target = target;
    shotInfo.TimeRatio = 1f - Mathf.Clamp01( m_lastGestureTime / throwerGestureTime );

    //float elevation = BallPhysics.instance.AnguloMinimo(target);//Random.Range(BallPhysics.instance.AnguloMinimo(Thrower.instance.shotInfo.Target), maxElevationDegrees * Mathf.Deg2Rad);
    shotInfo.Effect01 = efecto;

    Camera.main.fieldOfView = 45;

    SendShotInfo(shotInfo);

  }


  public void SendShotInfo(ShotInfo _shot)
  {
        Tutorial.instance.EraseTutorial(_shot);
        Thrower.instance.shotInfo = _shot;

        cntCuentaAtras.instance.Detener();

        if(GameplayService.networked)
        {
            MsgThrow msg = Shark.instance.mensaje<MsgThrow>();
            msg.LoadShot(Thrower.instance.shotInfo);
            ShotResult res = new ShotResult{Result = Result.Goal};
            ScoreManager.Instance.CalculateScore(ref res);
            msg.points = res.ScorePoints + res.EffectBonusPoints;
            msg.send();
            //Time.timeScale = 0.05f;
        }
        
        ClearEffects();

        // Retrasa la cámara.
        
        if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
            GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.Wait.instance;
        else
        {
            //f4ke aqui para fakear los tiros de camara en single
            if(GameplayService.networked && !Goalkeeper.instance.m_networkDefenseReceived)
            {
                GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.WaitNetworkCamera.instance;
            }
            else
            {
                GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Wait.instance;
            }
        }
        
        if (ServiceLocator.Request<IGameplayService>().GetGameMode() != GameMode.GoalKeeper) {
            // Mete tiempo de espera para el tiro.
            m_timeToThrow = GameplayService.networked ? float.MaxValue : deltaTimeToThrow;
            Material mat = gestureLine.GetComponent<Renderer>().material;
            Color col = mat.GetColor("_TintColor");
            col.a = 0;
            mat.SetColor("_TintColor", col);
        }
        //    Thrower.instance.DoThrow();
  }

  void sendDefense(Vector2 _screenPosition) {
    Blocked = true;
    Ray ray = Camera.main.ScreenPointToRay(_screenPosition);
    float distance;
    planeGoalClose.Raycast(ray, out distance);
    Vector3 target = ray.GetPoint(distance);
    DefenseInfo defenseInfo = new DefenseInfo();
    if(target.y < 0)
    {
        target.y = 0.1f;
    }
    defenseInfo.Target = target;
    ServiceLocator.Request<IDefenseService>().OnDefenseExecuted(defenseInfo);
  }

  void Update()
  {
      /*if(PowerupService.instance.IsPowerActive(Powerup.Sharpshooter))
      {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, m_fview, 3 * (Time.timeScale == 0f ? 0f : Time.deltaTime/(Time.timeScale)));
      }*/

      if (m_timeToThrow != 0)
      {
          m_timeToThrow -= Time.deltaTime;
          if (m_timeToThrow < 0)
          {
              m_timeToThrow = 0;
              Thrower.instance.DoThrow();
          }
      }    

    Material mat = gestureLine.GetComponent<Renderer>().material;
    Color col = mat.GetColor("_TintColor");
    if(m_panPoints.Count == 0)
    {
      col.a = Mathf.Lerp(col.a, 0f, 0.1f);
    }
    else
    {
     col.a = 1f;
    }
    mat.SetColor("_TintColor", col);
    if(col.a < 0.01) gestureLine.SetVertexCount(0);
  }

  public void LateUpdate()
  {
    if(m_gestureTime > 0f)
    {
      m_gestureTime -= Time.deltaTime / Time.timeScale;
      m_lastGestureTime = m_gestureTime;
      if(m_gestureTime < 0f)
      {
        EndPan(true);
      }
    }
  }


  void updateLine()
  {
    Transform cam = Camera.main.transform;
    Vector3 planePosition = cam.position + cam.forward * (Camera.main.nearClipPlane + 0.1f);
    Plane plane = new Plane(cam.forward, planePosition);

    gestureLine.SetVertexCount(m_panPoints.Count);
    float scale = Camera.main.fieldOfView / 45f;
    gestureLine.SetWidth(scale * 0.0055f, scale * 0.0055f);

    for(int i = 0; i < m_panPoints.Count ; i++)
    {
      Vector3 point = m_panPoints[i].Position;

      /*if(PowerupService.instance.IsPowerActive(Powerup.Sharpshooter))
      {
        //Vector3 halfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Vector3 halfScreen = m_panPoints[m_panPoints.Count-1].Position;
        point -= halfScreen;
        point *= 1f / scale;//(((m_panPoints.Count - i) / m_panPoints.Count) / scale);
        point += halfScreen;
      }*/

      Ray ray = Camera.main.ScreenPointToRay(point);
      float distance;
      plane.Raycast(ray, out distance);

      gestureLine.SetPosition(i, ray.GetPoint(distance));
    }
  }

}
