using UnityEngine;
using System.Collections;

public class Goalkeeper : MonoBehaviour
{
  static Goalkeeper m_goalko;
  public static Goalkeeper instance { get{return m_goalko;} set{m_goalko = value; } }

  int perfectPoints = 500;
  int normalPoints = 100;

  const float successPrecision = 1f;

  bool forceRePositioning = false;

  bool doGOAnim = false;

  AnimationDescriptorsResource m_animationDescriptorsResource;

  Transform m_bip01;
  Transform m_ball;

  private bool m_debugTrace = false;

  public bool lastPerfect = false;
  GKResult m_lastResult = GKResult.Idle;
  public GKResult lastResult
  {
    get {return m_lastResult;}
    set {m_lastResult = value;}
  }
  public int lastPrecision = 0;

  void Awake()
  {
  }

  // Use this for initialization
  void Start()
  {
    instance = this;
    EquipacionManager.instance.PintarEquipacionesIngame(true, Goalkeeper.instance.gameObject);
    m_bip01 = transform.Find("Bip01");
    m_ball = m_bip01.Find("Balon");
    m_animationDescriptorsResource = Resources.Load("AnimationDescriptorsResource") as AnimationDescriptorsResource;
    ServiceLocator.Request<IDefenseService>().RegisterListener(CalcDefenseResult);
    ServiceLocator.Request<IShotResultService>().RegisterListener(Celebrate);
  }

  public bool GrabBall { private set; get; }
  //public bool isGrabbing { get { return m_grab; }  }
  public bool ClearBall { private set; get; }

  bool m_grab = false;
  public float m_animTime;
  float m_grabTime;
  float m_ballTime {get; set;}
  public DefenseInfo m_futureDefense {get; set;}
  float simAnimTime = 0f;
  public bool m_networkDefenseReceived = false;
  public bool m_networkDefenseSent = false;
  string m_animName = "";
  string m_animIdle = "";
  int m_lastFase = 0;
  //bool destello = false;
  Vector3 m_animDesp;
  AnimationState m_animationState;
  AnimationDescriptorsResource.AnimationDescriptorResource m_descriptor;

  GKResult m_forcedResult = GKResult.Fail;

  bool m_gestureReady = false;
  Vector3 m_defensePoint = Vector3.zero;
  bool m_ballReady = false;
  public Vector3 m_ballPoint = Vector3.zero;
  string m_ballCasilla = "";
  bool m_ready = false;
  public bool forceWin = false;

  public static Vector3 defaultPosition = new Vector3(0, 0, -49.5f);

  public void Reset()
  {
    m_grab = false;
    m_ready = true;
    ClearBall = false;
    transform.position = defaultPosition;
    GetComponent<Animation>().Play("P_IdleDef01");
    // Mira a la pelota.
    Vector3 v = BallPhysics.instance.transform.position;
    v.y=0;
    transform.LookAt(v);
    m_ballReady = false;
    m_gestureReady = false;
    forceWin = false;
    m_networkDefenseReceived = false;
    m_networkDefenseSent = false;
    lastResult = GKResult.Idle;
  }

  Vector3 m_targetDef;
  public void SetupNetworkDefense(DefenseInfo _info)
  {
    m_futureDefense = _info;
    m_forcedResult = _info.Result;
    m_networkDefenseReceived = true;
  }

  public void DoDefense()
  {
    //GetGrab(m_futureDefense);
    CalcDefenseResult(m_futureDefense);
  }

    void OnDestroy()
    {
        ServiceLocator.Request<IDefenseService>().UnregisterListener(CalcDefenseResult);
        ServiceLocator.Request<IShotResultService>().UnregisterListener(Celebrate);
    }

  // Update is called once per frame
  void Update ()
  {

    if(Input.GetKeyUp("l"))
    {
        m_debugTrace = !m_debugTrace;
    }

    m_ballTime -= Time.deltaTime;
    /*if(m_ballTime - simAnimTime <= 0.1f && destello)
    {
      destello = false;
      kickEffects.instance.destello(m_ballPoint);
    }*/ //destello perfect quitado, mejor no borrar todavia...
    if (m_animName!=string.Empty)
    {
      // Endereza al portero hasta m_animTime
      m_animTime -= Time.deltaTime;

      if (m_animTime <= 0f)
      {
        GetComponent<Animation>().CrossFade(m_animName);
        // Lanza la animacion del portero
        m_animationState = GetComponent<Animation>()[m_animName];

        if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper 
            && !GameplayService.networked)// <= si no estamos en modo duelo
            GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.Action.instance; 
        /*else
            GoalCamera.instance.StateMachine.changeState = ThrowerCameraStates.Action.instance; */
        m_animName = "";
      }
    }

    if (m_animationState != null && m_animationState.enabled)
    {
      if(m_ballTime <= 0) m_animationState.speed = 1f;
      transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.05f);
    }

    if ((m_animationState != null && !m_animationState.enabled && !m_ready) || (doGOAnim && (m_animationState == null || !m_animationState.enabled)))
    {
      if(!ServiceLocator.Request<IPlayerService>().IsGameOver() || forceRePositioning)
      {
        forceRePositioning = false;
        Vector3 pos = m_bip01.transform.position;
        pos.y = 0;
        transform.position = pos;
      }
      else if(doGOAnim)
      {
          doGOAnim = false;
          Vector3 pos = m_bip01.transform.position;
          pos.y = 0;
          transform.position = pos;
          m_ready = false;
          m_animTime = 0f;


        bool isPlayer1 = GameplayService.initialGameMode != GameMode.Shooter;
        int score1 = isPlayer1 ? Player.serverState.score_1 : Player.serverState.score_2;
        int score2 = isPlayer1 ? Player.serverState.score_2 : Player.serverState.score_1;

        bool victory = (ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts > 0);

        

        if(GameplayService.networked)
        {
            victory = (score1 > score2) && GameplayService.IsGoalkeeper() || (score1 < score2) && !GameplayService.IsGoalkeeper();
        }
        else
        {
            if(MissionManager.instance.GetMission().PlayerType == GameMode.Shooter)
            {
                victory = !victory;
            }
        }

          if(!victory)
          {
            //animaciones de GameOver
            m_animName = "P_FalloFinal";
            m_animIdle = "P_IdleFalloFinal";
            m_grab = false;
          }
          else
          {
            if(GrabBall)
            {
              m_animName = "P_Celebracion_03";
              forceRePositioning = true;
            }
            else
            {
              m_animName = "P_CelebracionFinal";
            }

            m_animIdle = "P_IdleCelebracionFinal";
          }
          GetComponent<Animation>().Play(m_animName);
          return;
      }
      if (m_animIdle==string.Empty) m_animIdle = "P_Idle01";
      GetComponent<Animation>().Play(m_animIdle);

      GetComponent<Animation>().Sample();
      m_animationState = null;
    }
  }


  void ShowCartela(ShotResult _info) {
      /*if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
      {
        if(_info.Result != Result.OutOfBounds)
          kickEffects.instance.imgLabel(cartela, Porteria.instance.transform.position + Vector3.up * 3f);
      }*/
  }


  public void Celebrate(ShotResult _info)
  {
    bool celebrate = m_lastFase != ServiceLocator.Request<IDifficultyService>().GetFase();

    // en modo TIME_ATTACK
    if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK) {
        // si se ha agotado el tiempo del cronometro => mostrar la animacion de derrota
        if (cntCronoTimeAttack.instance.tiempoRestante <= 0.0f) {
            if (m_animName != "P_FalloFinal") {   // <= si no se esta reproduciendo ya una animacion de derrota
                m_animName = "P_FalloFinal";
                m_animIdle = "P_IdleFalloFinal";
                GetComponent<Animation>().Play(m_animName);
            }
        }

        return;
    }

    if(celebrate && !ServiceLocator.Request<IPlayerService>().IsGameOver())
    {
      m_lastFase = ServiceLocator.Request<IDifficultyService>().GetFase();
      switch(_info.Result)
      {
        case Result.Stopped:
          if (_info.Point.x > 0)
            m_animIdle = "P_Celebracion_01";
          else
            m_animIdle = "P_Celebracion_02";
          FieldControl.instance.setRoundCooldown(4.5f);
          break;
        case Result.Saved:
          m_animIdle = "P_Celebracion_04";
          FieldControl.instance.setRoundCooldown(4.5f);
          break;
        case Result.OutOfBounds:
          break;
        case Result.Goal:
          break;
      }
    }
    else //estas se reprducen siempre
    {
      if(ServiceLocator.Request<IPlayerService>().IsGameOver())
      {
        doGOAnim = true;
      }
      else if(_info.Result == Result.Goal)
      {
          m_animIdle = "P_Fallo_01";
          if(GameplayService.IsGoalkeeper() && !m_gestureReady) GetComponent<Animation>().CrossFade(m_animIdle);
          FieldControl.instance.setRoundCooldown(2f);
      }
    }
  }

  void LateUpdate()
  {
    if (m_grab)
    {
      BallPhysics.instance.transform.position = m_ball.position;
      BallPhysics.instance.transform.rotation = m_ball.rotation;
    }
  }

  public void EventShow(string _param)
  {
    if(PowerupService.instance.IsPowerActive(Powerup.Resbaladiza) && BallPhysics.instance.noCatch) return;
    //ShotResult result = new ShotResult() { Result = Result.Saved, Point = transform.position, Rebounded = false };

//    GameObject go = GameObject.Find("trail");
//    if(go!=null) go.renderer.enabled = false;
    m_grab = true;
    //ServiceLocator.Request<IShotResultService>().OnShotEnded(result);
    MakeResult(true);
  }


  public void MakeResult (bool _saved) {

    int points = 0;

    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
    {
      if(lastPerfect) points = perfectPoints;
      else points = normalPoints;
    }
    ShotResult result = new ShotResult() { 
        Result = _saved ? Result.Saved : Result.Stopped, 
        Point = BallPhysics.instance.transform.position, 
        Rebounded = !_saved,
        ScorePoints = points, 
        Perfect = lastPerfect,
        EffectBonusPoints = 0, 
        Precision = lastPrecision, 
        DefenseResult = lastResult
    };
    ServiceLocator.Request<IShotResultService>().OnShotEnded(result);

  }

  void CalcDefenseResult(DefenseInfo _info)
  {
    if(m_gestureReady) return;
    //kickEffects.instance.defenseCircle(_info);
    m_gestureReady = true;
    m_defensePoint = _info.Target;
    if(m_ballReady)
    {
      GetGrab(_info.Target);
      ShotResultService.noDefense = false;
      /*if(GameplayService.IsGoalkeeper() && GameplayService.networked)
      {
        MsgDefend msg = Shark.instance.mensaje<MsgDefend>();
        msg.LoadDefense(preResult, lastResult);
        msg.send();
        m_networkDefenseSent = true;
      }*/
    }
    else
    {
      lastResult = GKResult.Early;
    }
  }

  public Vector2 GetCasilla (Vector3 _position) {
    Vector2 casilla = Vector2.zero;
    if (_position.y > 1.80f) casilla.y = 2;
    else if (_position.y > 0.90f) casilla.y = 1;
    else casilla.y = 0;
    int h = 1 + (int)((-_position.x + 3.66f) / 1.04f);
    casilla.x = h;
    if (h < 1)
    {
      casilla.x = 1;
    }
    else if( h > 7 )
    {
      casilla.x = 7;
    }
    return casilla;
  }

  public string MakeAnimSufix(Vector3 _position) {
    string sufix = "";
    Vector2 casilla = GetCasilla(_position);
    if (casilla.y == 2) sufix += "A";
    else if (casilla.y == 1) sufix += "M";
    else sufix += "B";

    sufix += casilla.x;
    if (casilla.x == -1)
    {
      return "";
    }
    else return sufix;
  }

  public Vector3 ApplyIADefense(Vector3 _position, bool networkMode = true)
  {
    //aqui se efectu ael resultado del tiro
    //si la coge, o no, se decide en IADefense
    //tambien simula jugador network
    //if(m_forcedResult == GKResult.Idle) return _position;
    bool success = (m_forcedResult == GKResult.Good) || (m_forcedResult == GKResult.Perfect);
    m_ballCasilla = MakeAnimSufix(_position);
    Vector3 randomPosition = _position;
    if(!success)
    {
      Vector2 casilla = GetCasilla(_position);

      if (casilla.x <= 1)
      {
        casilla.x ++;
      }
      else if (casilla.x >= 7)
      {
        casilla.x --;
      }
      else
      {
        if(Random.Range(0f,1f) > 0.5f) casilla.x ++;
        else casilla.x --;
      }
      randomPosition.x = -1f * ((casilla.x - 1) * 1.04f - 3.66f);
    }

    if(success) return GetGrab(new DefenseInfo{Target = networkMode ? m_futureDefense.Target : _position , ForcedResult = true, Result = m_forcedResult});
    else
    {
      GetGrab(new DefenseInfo{Target = networkMode ? m_futureDefense.Target : randomPosition , ForcedResult = true, Result = m_forcedResult});
      return _position;
    }
  }

  public bool IADefense(Vector3 _position, float _efecto)
  {
    // obtener la probabilidad base de parada
      float baseProb = DifficultyService.GetProbabilidadBaseDeParada();//ServiceLocator.Request<IDifficultyService>().GetFase(), GameplayService.modoJuego.tipoModo);

    if(PowerupService.instance.IsPowerActive(Powerup.Destello))
    {
        baseProb -= 0.2f;
    }

    if(PowerupService.instance.IsPowerActive(Powerup.Phase))
    {
        baseProb -= 0.2f;
    }

    if(Habilidades.IsActiveSkill(Habilidades.Skills.Mago_balon) && Mathf.Abs(_efecto) > 0.65f)
    {
        baseProb -= 0.2f;
    }

      // modificador en la horizontal
    float hBonus = Mathf.Clamp01(Mathf.Abs(_position.x) / 3.5f);
    baseProb -= 0.05f * hBonus;
    
      // modificador en la vertical
      float vBonus = Mathf.Clamp01(Mathf.Abs(_position.y - 1.2f) / 1.1f);
    baseProb -= 0.05f * vBonus;

      // modificador por efecto
    baseProb -= 0.05f * Mathf.Abs(_efecto);

    baseProb = Mathf.Clamp01(baseProb);

    Debug.Log("Probabilidad de parada: " + baseProb + "(X " + hBonus + " Y " + vBonus + " E " + _efecto + ")");

    //aqui se decidiria si el portero coge o no la pelota (tirada de dado para resolver si la parada es efectiva)
    float rand = Random.Range(0f,1f);
    bool success = rand < baseProb;

    //m_forcedResult = success ? GKResult.Good : GKResult.Fail;

      // forcedResult
    if(success) m_forcedResult = (Random.Range(0f,1f) < 0.25f) ? GKResult.Perfect : GKResult.Good;
    else m_forcedResult = GKResult.Fail;
    return success;
  }


  public Vector3 SimulateDefense(Vector3 _position, bool _influenceBall = true) {
    m_ballPoint = _position;
    GoalCamera.instance.m_shotEndPoint = m_ballPoint;
    m_grabTime = 0f;
    string sufix = MakeAnimSufix(_position);

    m_ballCasilla = sufix;
    if(sufix == "") return _position;

    sufix += "_01";
    string animName = "P_Par" + sufix;
    AnimationDescriptorsResource.AnimationDescriptorResource descriptor = m_animationDescriptorsResource.GetByName(animName);

    if (descriptor == null)
    {
      animName = "P_Dpj" + sufix;
      descriptor = m_animationDescriptorsResource.GetByName(animName);
    }

    m_ballReady = true;
    if(m_gestureReady) GetGrab(m_defensePoint);
    Vector3 preResult = transform.position + descriptor.m_grabDiff;
    preResult.z = -1 * InputManager.planeGoalClose.distance;
    simAnimTime = descriptor.m_grabTime;
    if(_influenceBall)
    {
      m_ballPoint = preResult;
      GoalCamera.instance.m_shotEndPoint = m_ballPoint;
    }
    else
    {
      m_ballPoint = _position;
      return _position;
    }
    return preResult;
  }

  public Vector3 GetGrab(Vector3 _target)
  {
    DefenseInfo info = new DefenseInfo{Target = _target, ForcedResult = false};
    return GetGrab(info);
  }

  public Vector3 GetGrab(DefenseInfo _info)
  {
    bool playingGoalkeeper = ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper;
    m_ready = false;
    ClearBall = false;
    GrabBall = false;

    if(_info.Result == GKResult.Idle) return _info.Target;

    transform.position = new Vector3(0, 0, -49.5f);
    bool success = true;
    string sufix = MakeAnimSufix(_info.Target);
    if (sufix == "")
    {
      m_animName = "";
      m_animIdle = "";
      return _info.Target;
    }

    if(!GameplayService.IsGoalkeeper()) lastResult = GKResult.IAFail;


    float precision = Mathf.Clamp01(Vector3.Distance(_info.Target, m_ballPoint)
                        / ServiceLocator.Request<IDifficultyService>().GetSuccessRadius(_info.Target));

        if(m_debugTrace)
        {
            Debug.Log ("PRECISION: " + precision);
            kickEffects.instance.DebugCircle(_info.Target, ServiceLocator.Request<IDifficultyService>().GetPerfectRadius() * 2f, new Color(1,0.5f,0));
            kickEffects.instance.DebugCircle(_info.Target, ServiceLocator.Request<IDifficultyService>().GetSuccessRadius(_info.Target) * 2f, Color.cyan);
            kickEffects.instance.DebugTarget(m_ballPoint);
        }

    if(!GameplayService.networked && !playingGoalkeeper) precision = Random.Range (0f,1f);

#region powerup reflejo
    if(PowerupService.instance.IsPowerActive(Powerup.Reflejo))
    {
            int casillaX = (int)GetCasilla(_info.Target).x;
            if(casillaX == 7)
            {
                casillaX = 6;
            }
            else if(casillaX == 0)
            {
                casillaX = 1;
            }


            Vector3[] targets = {GetCasillaPosition(casillaX+1,0), GetCasillaPosition(casillaX,0), GetCasillaPosition(casillaX-1,0),
                GetCasillaPosition(casillaX+1,1), GetCasillaPosition(casillaX,1), GetCasillaPosition(casillaX-1,1),
                GetCasillaPosition(casillaX+1,2), GetCasillaPosition(casillaX,2), GetCasillaPosition(casillaX-1,2)
            };

            /*Vector3[] targets = {GetCasillaPosition(0,0), GetCasillaPosition(1,0), GetCasillaPosition(2,0), GetCasillaPosition(3,0), GetCasillaPosition(4,0), GetCasillaPosition(5,0), GetCasillaPosition(6,0), GetCasillaPosition(7,0),
                GetCasillaPosition(0,1), GetCasillaPosition(1,1), GetCasillaPosition(2,1), GetCasillaPosition(3,1), GetCasillaPosition(4,1), GetCasillaPosition(5,1), GetCasillaPosition(6,1), GetCasillaPosition(7,1),
                GetCasillaPosition(0,2), GetCasillaPosition(1,2), GetCasillaPosition(2,2), GetCasillaPosition(3,2), GetCasillaPosition(4,2), GetCasillaPosition(5,2), GetCasillaPosition(6,2), GetCasillaPosition(7,2)
            };*/

            /*Vector3[] targets = {GetCasillaPosition(0,0), GetCasillaPosition(1,0), GetCasillaPosition(2,0),
                GetCasillaPosition(0,1), GetCasillaPosition(1,1), GetCasillaPosition(2,1), 
                GetCasillaPosition(0,2), GetCasillaPosition(1,2), GetCasillaPosition(2,2)
            };*/

        Time.timeScale = 0.5f;

        for(int i = 0 ; i < targets.Length ; i++)
        {
            Vector3 mirrorTarget = targets[i];//new Vector3(_info.Target.x * -1f , _info.Target.y, _info.Target.z);
            float precision2 = Mathf.Clamp01(Vector3.Distance(mirrorTarget, m_ballPoint)
                                             / ServiceLocator.Request<IDifficultyService>().GetSuccessRadius(mirrorTarget));
    
            Vector3 mirrorPos = _info.Target;
            if(precision2 < precision)
            {
                mirrorPos = _info.Target;
                precision = precision2;
                sufix = MakeAnimSufix(mirrorTarget);
                _info.Target = mirrorTarget;
            }
            else
            {
                mirrorPos = mirrorTarget;
            }
            
            GameObject doppelganger = Instantiate(FieldControl.instance.goalKeeperPrefab[FieldControl.instance.doppelgangerindex], Goalkeeper.instance.transform.position, Goalkeeper.instance.transform.rotation) as GameObject;
    
            Destroy(doppelganger.GetComponent<Goalkeeper>());//.enabled = false;
    
            string mirrorAnimName = MakeAnimSufix(mirrorPos);
            mirrorAnimName += "_01";
            mirrorAnimName = "P_Dpj" + mirrorAnimName;
    
            doppelganger.GetComponent<Animation>()[mirrorAnimName].speed = m_animationDescriptorsResource.GetByName(mirrorAnimName).m_grabTime / m_ballTime;
    
            Doppelganger comp = doppelganger.AddComponent<Doppelganger>();
            comp.m_balltime = m_ballTime;
            comp.m_animName = mirrorAnimName;
    
            doppelganger.GetComponent<Animation>().Play(mirrorAnimName);
            kickEffects.instance.DoDoppelganger(doppelganger);
        }
    }
#endregion

    if(playingGoalkeeper)
    {
      //if(m_ballCasilla == sufix) success = true;
      if(precision < successPrecision) success = true;
      else success = false;
    }
    else
    {
      success = _info.Result == GKResult.Good || _info.Result == GKResult.Perfect;
    }

    if(success) sufix = m_ballCasilla;

    sufix += "_01";

    m_animName = "P_Dpj" + sufix;

    m_descriptor = m_animationDescriptorsResource.GetByName(m_animName);

    bool possiblePerfect = false;
    lastPerfect = false;

    if(playingGoalkeeper)
    {
      SetGrabTime(m_descriptor.m_grabTime);

      if(m_animTime <= 0f && !(lastResult == GKResult.Early)) //para comprobar que no se haya hecho la defensa antes incluso de chutar, da 0 en ese caso
      {
        lastResult = GKResult.Late;
        success = false;
      }
      else if(m_animTime >= 1f)
      {
        lastResult = GKResult.Early;
        success = false;
        m_animTime = 0.1f;
      }
      else if(m_animTime <= 0.2f && success)
      {
        possiblePerfect = true;
        success = true;
      }
      else if(m_animTime <= 0.4f && success)
      {
        success = true;
      }
      else if(!success)
      {
        lastResult = GKResult.Fail;
        m_animTime = 0f;
      }
    }

    m_animIdle = "P_Idle01";

    ClearBall = success;

    //preparar el perfect
    float precisionDist = ServiceLocator.Request<IDifficultyService>().GetPerfectRadius();
    bool perfectRadius = Vector3.Distance(_info.Target, m_ballPoint) <= precisionDist;
    float precisionRatio = precisionDist / ServiceLocator.Request<IDifficultyService>().GetSuccessRadius(_info.Target);

    if(PowerupService.instance.IsPowerActive(Powerup.Resbaladiza) && success)
    {
        float max = successPrecision - precisionRatio;
        float transfProb = (precision - precisionRatio);
        float finalProb = 0f;
        if(transfProb < 0f) finalProb = 0.25f;
        else if(transfProb > max) finalProb = 0.55f;
        else
        {
            finalProb = 0.4f * (transfProb / max) + 0.1f;
        }
        bool forcedFail = finalProb > Random.Range(0f,1f);
        BallPhysics.instance.noCatch = forcedFail;
        Debug.Log("GreasyBall prob = " + finalProb);
    }

    if(BallPhysics.instance.state != BallPhysics.BallState.Idle)
    {
      if(( playingGoalkeeper && success && perfectRadius )
          || (!playingGoalkeeper && _info.Result == GKResult.Perfect))
      {
        if(possiblePerfect || !playingGoalkeeper)
        {
          AnimationDescriptorsResource.AnimationDescriptorResource tempDescriptor = m_animationDescriptorsResource.GetByName("P_Par" + sufix);
          if(tempDescriptor != null)
          {
            m_animName = "P_Par" + sufix;
            m_animIdle = "P_IdleCB01";
            m_descriptor = tempDescriptor;
            GrabBall = true;
          }
        }
      }
      else possiblePerfect = false;
    }


    if(playingGoalkeeper && possiblePerfect)
    {
      lastResult = GKResult.Perfect;
      lastPrecision = 0;
      GeneralSounds.instance.perfect();
      possiblePerfect = true;
    }
    else if ( playingGoalkeeper && success)
    {
        if(precision < 0.2)
        {
            lastPrecision = 1;
        }
        else if(precision < 0.45)
        {
          lastPrecision = 2;
        }
        else
        {
          lastPrecision = 3;
        }
        lastResult = GKResult.Good;
        possiblePerfect = false;
    }
    lastPerfect = possiblePerfect;

    forceWin = success;

    if(playingGoalkeeper && success) AdjustAnimation();


    Vector3 preResult = transform.position + m_descriptor.m_grabDiff;

    //preResult.z = InputManager.planeGoalClose.distance;
    return  preResult;
  }

  public void TryGrab(float _time)
  {
    if (m_animName == string.Empty) return;
    m_animTime = _time - m_descriptor.m_grabTime;
    m_animDesp = m_bip01.localToWorldMatrix.MultiplyPoint(m_descriptor.m_grabDiff);
  }

  public void FakeGrabTime(float _time)
  {
    if (m_animName == string.Empty) return;
    m_animTime = _time - m_descriptor.m_grabTime;
    if(m_forcedResult == GKResult.Late) m_animTime += 0.5f;
    else if(m_forcedResult == GKResult.Early) m_animTime -= 0.5f;
    m_animDesp = m_bip01.localToWorldMatrix.MultiplyPoint(m_descriptor.m_grabDiff);
  }

  public Vector3 GetCasillaPosition(int x, int y)
  {
    Vector3 result = Vector3.zero;
    result.x = Porteria.instance.transform.position.x + (Porteria.instance.HalfHorizontalSize) - (((Porteria.instance.HalfHorizontalSize * 2f) / 7f) * x);
    result.y = Porteria.instance.transform.position.y + ((Porteria.instance.VerticalSize)/3f) * y;
    result.z = -1 * InputManager.planeGoalClose.distance;
    return result;
  }

  public float SetBallTime(float _time)
  {
    m_ballTime = _time;
    //destello = true;
    return _time - simAnimTime;
  }

  public void SetGrabTime(float _time)
  {
    m_grabTime = _time;
    m_animTime = m_ballTime - _time;
  }

  public void AdjustAnimation()
  {
    GetComponent<Animation>()[m_animName].speed = m_grabTime / m_ballTime;
    m_animTime = 0f;
  }

  public void OnDrawGizmos()
  {
    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawSphere(m_defensePoint, 0.1f);
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(m_ballPoint, 0.1f);
      Gizmos.color = Color.white;
    }
  }

  public void EventHide(string _param)
  {
    //ServiceLocator.Request<IShotService>().OnShotExecuted(Thrower.instance.shotInfo);
    m_grab = false;
    if(BallPhysics.instance.state != BallPhysics.BallState.Waiting)
    {
      Physics.gravity = new Vector3(0f,-9.81f,0f);
      BallPhysics.instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
      BallPhysics.instance.GetComponent<Rigidbody>().AddForce(new Vector3(0f,2f,1f).normalized * 10f, ForceMode.VelocityChange);
      GeneralSounds.instance.chut(new ShotInfo());
    }
  }
}
