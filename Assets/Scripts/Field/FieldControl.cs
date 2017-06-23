using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class FieldControl : MonoBehaviour {

  public static FieldControl instance { get; private set; }

  public static Jugador localThrower;
  public static Jugador localGoalkeeper;

  public int doppelgangerindex = 0;

    public Jugador ThrowerObject {
        get{
            if(GameplayService.networked)
            {
                return (GameplayService.IsGoalkeeper()? ifcDuelo.m_rival.charThrower : localThrower );
            }
            else
            {
                if(MissionManager.instance.GetMission().PlayerType == GameMode.GoalKeeper)
                {
                    GoalkeeperMissionRound round = (MissionManager.instance.GetMission().GetRoundInfo() as GoalkeeperMissionRound);
                    if(round == null)
                    {
                        round = (MissionManager.instance.GetMission().GetPrevRoundInfo() as GoalkeeperMissionRound);
                    }
                    return round.Character;
                }
                else
                {
                    return localThrower;
                }
            }
            //return (GameplayService.networked ? (GameplayService.IsGoalkeeper()? ifcDuelo.m_rival.charThrower : localThrower ) : localThrower);
        }
        set{}
    }

    public Jugador GoalkeeperObject
    {
        get{
            Jugador goalkeeper;
            if(GameplayService.networked)
            {
                goalkeeper = (!GameplayService.IsGoalkeeper()? ifcDuelo.m_rival.charGoalkeeper : localGoalkeeper );
            }
            else
            {
                if(MissionManager.instance.GetMission().PlayerType == GameMode.Shooter)
                {
                    ShooterMissionRound round = (MissionManager.instance.GetMission().GetRoundInfo() as ShooterMissionRound);
                    if(round == null)
                    {
                        round = (MissionManager.instance.GetMission().GetPrevRoundInfo() as ShooterMissionRound);
                    }
                    return round.Character;
                }
                else
                {
                    goalkeeper = localGoalkeeper;
                }
            }
            doppelgangerindex = goalkeeper.idModelo;
            return goalkeeper;
            //Jugador index = (GameplayService.networked ? (!GameplayService.IsGoalkeeper()? ifcDuelo.m_rival.charGoalkeeper : localGoalkeeper ) : localGoalkeeper); doppelgangerindex = index.idModelo; return index;
        }
        set{}
    }

    Jugador GoalkeeperReversed
    {
        //ATENCION: lo siguiente parece setar al reves en duelo, pero es porque se pasa a modo portero despues de haber instanciado el modelo cuyo indice se averigua aqui
        get{Jugador index = (GameplayService.networked ? (GameplayService.IsGoalkeeper()? ifcDuelo.m_rival.charGoalkeeper : localGoalkeeper ) : localGoalkeeper); doppelgangerindex = index.idModelo; return index;}
        set{}
    }

  private int playerAttempts = 3;
  ShotInfo lastShotinfo;
  Vector3 lastTarget;

  //prefabs
  public GameObject ballprefab;
  public GameObject difficultyAreas;
  public GameObject fieldDummy1;
  public GameObject fieldDummy2;
  public GameObject goalBoundingBox;
  private GameObject ball;
  public GameObject DifficultyAreas { get; private set; }
  public GameObject visuals;
  public GameObject audioObject;

  public GameObject[] goalKeeperPrefab;
  public GameObject[] throwerPrefab;
  public Transform m_goalCameraReference;

  public Vector3 m_PanCameraPosition = Vector3.zero;

  public float m_offsetCamera = 2.0f;

  private List<IDisposable> disposables = new List<IDisposable>();

  private Player player;
  private Rect fieldBounds;
  private Bounds goalBounds;  

  public bool pendingGameOver = false;

  public int minutes = 0;
  public int seconds = 0;  
    
  float totalTime = 0f;
  //float gotoMenuTime = 0f;

  public bool goalKeeper {
    set{
      if(!GameplayService.networked)
      {
        if(Goalkeeper.instance) GameObject.Destroy(Goalkeeper.instance.gameObject);
        if(value)
        {
            Goalkeeper.instance = (GameObject.Instantiate(goalKeeperPrefab[GoalkeeperObject.idModelo], new Vector3(0f,0f,-49.5f), Quaternion.identity) as GameObject).GetComponent<Goalkeeper>();
        }

        /*if(value && !Goalkeeper.instance)
        {
            Goalkeeper.instance = (GameObject.Instantiate(goalKeeperPrefab[GoalkeeperObject.idModelo], new Vector3(0f,0f,-49.5f), Quaternion.identity) as GameObject).GetComponent<Goalkeeper>();
        }
        else if(!value && Goalkeeper.instance) GameObject.Destroy(Goalkeeper.instance.gameObject);*/
      }
      else
      {
          if(Goalkeeper.instance != null) GameObject.Destroy(Goalkeeper.instance.gameObject);
          Goalkeeper.instance = (GameObject.Instantiate(goalKeeperPrefab[GoalkeeperReversed.idModelo], new Vector3(0f,0f,-49.5f), Quaternion.identity) as GameObject).GetComponent<Goalkeeper>();
      }
    }
    get{
      return (Goalkeeper.instance != null);
    }
  }

  public bool HasBullseye { get; set; }
  public bool HasSheet { get; set; }
  public bool HasWall { get; set; }

  public float roundCooldown = 0f;

  private IGameplayService gpService;

  public bool playingGoalKeeper { get{ return GameplayService.IsGoalkeeper(); } }

  void Awake()
  {
    instance = this;
    InitializeServices();
    MissionStats.Instance = new MissionStats();
  }

  void SpawnThrower()
  {
    Transform spawn = GameObject.Find("throwerSpawn").transform;
    GameObject.Instantiate( throwerPrefab.First( prefab => prefab!=null && prefab.name.Contains(ThrowerObject.assetName) ), spawn.position, spawn.rotation );
  }

  void Start() {
    //PowerupService.ownInventory = new PowerupInventory(true);//H4CK
    // Try to load the level configuration
    if (!GameplayService.networked && MissionManager.instance.SetCurrentMission(GameplayService.gameLevelMission.MissionFileName, GameplayService.gameLevelMission.Index))
    {
        GameplayService.gameLevelMission.Freeze();
        GameplayService.initialGameMode = MissionManager.instance.GetMission().PlayerType;
        Debug.Log("Mission " + GameplayService.gameLevelMission.MissionFileName + " successfully loaded!");
        Debug.Log( "Rounds loaded = " + MissionManager.instance.GetMission().RoundsCount );

        // inicializar el "RoundInfoManager" para esta mision
        RoundInfoManager.instance.Inicializar();
    }
    // FPA (04/01/17): Eliminado GameAnalitics de momento. 
    // GA.API.Design.NewEvent("PartidaIniciada:"+(GameplayService.networked ? "Multiplayer":"Singleplayer"), (GameplayService.networked ? 0f:(float)MissionManager.instance.GetMission().indexMision), Vector3.zero);

    doppelgangerindex = localGoalkeeper.idModelo;

    goalKeeper = GameplayService.initialGameMode == GameMode.GoalKeeper;
    InstantiatePrefabs();
    GetFieldAndGoalBounds();
    ServiceLocator.Request<IShotService>().RegisterListener( ShotStarted );
    ServiceLocator.Request<IShotResultService>().RegisterListener( ShotFinished );
    gpService = ServiceLocator.Request<IGameplayService>();
    gpService.RegisterListener( ChangeGameMode );
    ServiceLocator.Request<IGameplayService>().SetGameMode(GameplayService.initialGameMode);

    ServiceLocator.Request<IPowerupService>().RegisterListener(SharpCamera);


    if ( ifcBase.activeIface != ifcAyudaInGame.instance ) ifcThrower.instance.Invoke( "setFase", 2f );

    totalTime = 0f;
    minutes = 0;
    seconds = 0;

    ResetGameStats();
    ResetDuelGameStats();

    //goalKeeper = true;
    setRoundCooldown(0.1f);
    SpawnThrower();

    Habilidades.ResetPremonicion();

    EscudosManager.instance.DecrementaEscudoActual();

    if(!GameplayService.networked)
    {
        GameObject.Find("pastilla-disparos/ronda/texto").GetComponent<GUIText>().text = "0/"+MissionManager.instance.GetMission().RoundsCount;
        GameObject.Find("pastilla-disparos/ronda/textoSombra").GetComponent<GUIText>().text = GameObject.Find("pastilla-disparos/ronda/texto").GetComponent<GUIText>().text;
    }
  }

  public bool playOnCourse = false;

  void SharpCamera(PowerupUsage _info)
  {
    if(_info.Value == Powerup.Sharpshooter && (GoalCamera.instance.stateMachine.current == ThrowerCameraStates.Init.instance))
    {
        GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Sharpshooter.instance;
    }
  }

  void ChangeGameMode(GameInfo _info) {
      //goalKeeper = _info.Mode == GameMode.GoalKeeper;
      Porteria.instance.SetKeeperMaterial(playingGoalKeeper);
      gpService.SwitchAuto(playingGoalKeeper);
      if (playingGoalKeeper) GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.Init.instance;
      else GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Init.instance;
  }

  void UpdateMinutes(){
    totalTime += Time.timeScale > 0f ? Time.deltaTime / Time.timeScale : 0f;
    float delta = totalTime;
    float fseconds = delta;
    seconds = Mathf.CeilToInt(fseconds);
    float fminutes = fseconds / 60f;
    minutes = Mathf.CeilToInt(fminutes);
  }

  void Update() {
    //DEBUUUUUUUUUUUUUG
      if (Debug.isDebugBuild) {
          if (Input.GetKeyDown("space")) gpService.SwitchGameMode();
          if (Input.GetKeyDown("z")) goalKeeper = true;
          if (Input.GetKeyDown("a")) ServiceLocator.Request<IPlayerService>().SetLives(3);
          if (Input.GetKeyDown("s"))
          {
            if(Time.timeScale == 0.05f)
            {
              ServiceLocator.Request<IGameplayService>().ResetTime();
            }
            else Time.timeScale = 0.05f;
          }
          if(Input.GetKeyDown("o")) ServiceLocator.Request<IDifficultyService>().PrevFase();
          if(Input.GetKeyDown("p")) ServiceLocator.Request<IDifficultyService>().NextFase();
      }
    //GUUUUUUUUUUUUUDEB

    UpdateMinutes();

    if (playOnCourse && (BallPhysics.instance.state == BallPhysics.BallState.Flying || BallPhysics.instance.state == BallPhysics.BallState.Idle)) {
      CheckIfShotFinished();
    }
    else if(roundCooldown > 0f)
    {
      roundCooldown -= Time.deltaTime;
      if(roundCooldown <= 0f)
      {
        if(!pendingGameOver) StartPlay();
        else
        {
          //if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) GoalCamera.instance.StateMachine.changeState = ThrowerCameraStates.GameOver.instance;
          ServiceLocator.Request<IApplicationEvents>().ThrowEvent(AppEvent.GameOverEnqueued);

          //setRoundCooldown(1f);
          //gotoMenuTime = 6f;
        }
      }
    }

    /*if(gotoMenuTime > 0f)
    {
      gotoMenuTime -= Time.deltaTime / Time.timeScale;
      if(gotoMenuTime <= 0f || Input.GetMouseButtonUp(0) && !endGame)
      {
        SuperTweener.Flush();
        endGame = true;
        goToMenu();
      }
    }*/
  }

  void GetFieldAndGoalBounds() {
    Transform dummy1 = fieldDummy1.transform;
    Transform dummy2 = fieldDummy2.transform;
    fieldBounds = new Rect( dummy1.position.x, dummy1.position.z,
        Mathf.Abs( dummy1.transform.position.x - dummy2.transform.position.x ),
        Mathf.Abs( dummy1.transform.position.z - dummy2.transform.position.z ) );

    goalBounds = new Bounds( goalBoundingBox.transform.position, goalBoundingBox.transform.localScale );
  }
  
  private void CheckIfShotFinished() {

      // La bola ha salido del campo y no es un rebote
      if ( ( !fieldBounds.Contains( new Vector2( ball.transform.position.x, ball.transform.position.z ) ) ) && 
           ( !BallPhysics.instance.ReboundBall() ) ) {

          // El resultado por defecto es fallo
          ShotResult shotResult = new ShotResult() {
              Result = Result.OutOfBounds,
              Point = ball.transform.position,
              Rebounded = false,
              ScorePoints = 0,
              Perfect = false,
              EffectBonusPoints = 0
          };

          // Bola dentro de la porteria
          if ( goalBounds.Contains( ball.transform.position ) ) {

              if ( Goalkeeper.instance != null ) {
                  // Hay portero...
                  shotResult.Result = Result.Goal;
              }
              else if ( HasBullseye ) {
                  // ... o hay diana
                  BullseyeImpactInfo bInfo = ServiceLocator.Request<IDifficultyService>().GetBullseye().CheckThrow( lastShotinfo );
                  if ( bInfo.Points > 0 ) {
                      // Si hemos puntuado con la diana, modificamos el ShotResult
                      shotResult.Result = Result.Target;
                      shotResult.ScorePoints = bInfo.Points;
                      shotResult.Perfect = ( bInfo.Ring == 0 );

                      lastTarget = lastShotinfo.Target;
                      kickEffects.instance.targetHit( lastTarget );
                  }
                  else {
                      // no hemos dado a la diana, comprobamos si la diana tiene zona amarilla
                      Rect area = ServiceLocator.Request<IDifficultyService>().GetRect();
                      if ( area.width > 0.1f ) {
                          Vector2 planeHit = new Vector2( ball.transform.position.x, ball.transform.position.y );
                          if ( area.Contains( planeHit + new Vector2( 3.55f, 0f ) ) ) {
                              // Hay zona amarilla y le hemos atizado con la bola
                              shotResult.Result = Result.Goal;
                              shotResult.AreaResult = AreaResultValues.BallHitsArea;
                              kickEffects.instance.DrawZone( true, ball.transform.position );
                          }
                          else {
                              // Hay zona amarilla pero no hemos conseguido darle
                              shotResult.AreaResult = AreaResultValues.BallFailsArea;
                              kickEffects.instance.DrawZone( false, ball.transform.position );
                          }
                      }
                      else {
                          // No habia zona amarilla, asi que lo tomamos como gol
                          shotResult.Result = Result.Goal;
                          shotResult.AreaResult = AreaResultValues.NoAreaExists;
                      }
                  }
              }
              else if ( HasSheet ) {
                  // ... o hay sabana
                  float sheetMultiplicator = SabanasManager.instance.GetScore( shotResult.Point );
                  if ( sheetMultiplicator > 0.0f ) {
                      // si se ha sacado algo de puntuacion...
                      shotResult.Result = Result.Target;
                      shotResult.ScorePoints = (int)sheetMultiplicator;
                  }
              }
          }
          else {
              // Bola fuera de la porteria
              if ( Goalkeeper.instance != null ) {
                  Goalkeeper.instance.lastResult = GKResult.ThrowerFail;
                  shotResult.DefenseResult = GKResult.ThrowerFail;
              }
          }

          if ( playingGoalKeeper ) {
              shotResult.Perfect = Goalkeeper.instance.lastPerfect;
              shotResult.Precision = Goalkeeper.instance.lastPrecision;
              shotResult.DefenseResult = Goalkeeper.instance.lastResult;
          }

          // Propagamos el evento de ShotEnded
          ServiceLocator.Request<IShotResultService>().OnShotEnded( shotResult );

          playOnCourse = false;
      }
      else {
          // Debug.Log( "INSIDE THE FIELD" );
      }      
  }

  public void GameOver() {
    // FPA (04/01/17): Eliminado GameAnalitics de momento. 
    // GA.API.Design.NewEvent("PartidaTerminada:"+(GameplayService.networked ? "Multiplayer":"Singleplayer"), seconds, Vector3.zero);

    GeneralSounds.instance.gameOver();
    ifcPausa.instance.PauseEnabled = false;
    pendingGameOver = true;

    if (!GameplayService.networked) {
        BI.EndGame(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter ? 2 : 1, ServiceLocator.Request<IDifficultyService>().GetFase());
        //    ifcThrower.instance.showGameOver();
        setRoundCooldown(3f);
    } 

    // comprobar si hay que cambiar a la camara de GAME_OVER
    if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) {
        // camara para lanzador
        GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.GameOver.instance;
    } else if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper && GameplayService.networked) {
        // camara para portero en modo duelo
        Debug.LogWarning(">>> ENTRA CAMARA DE PORTERO DUELO");
        GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.CamaraGameOverDueloPortero.instance;
    }

    // comprobar si se ha conseguido nuevos logros
    int recompensaPendiente = 0;
    List<string> idsLogros = new List<string>();

    if (PersistenciaManager.instance.CheckHayLogrosSinRecompensar(ref recompensaPendiente, ref idsLogros)) {
        // mostrar en la barra de opciones una "exclamacion"
        cntBarraSuperior.flagNuevosLogros = true;

        // actualizar el dinero
        Interfaz.MonedasHard += recompensaPendiente;

        // actualizar el progreso de los logros para que esta alerta no se dispare mas
        PersistenciaManager.instance.SaveLogros();

        // crear los dialogos para mostrar cada uno de los logros desbloqueados
        for (int i = 0; i < idsLogros.Count; ++i) {
            DialogManager.instance.RegistrarDialogo(new DialogDefinition(DialogDefinition.TipoDialogo.LOGRO_DESBLOQUEADO, idsLogros[i]));
        }
    }

    //(GameplayService.networked && GameplayService.IsGoalkeeper()))                         // en modo MULTIJUGADOR y si soy TIRADOR (NOTA; soy tirador si acabo jugando como portero)

      //Invoke("goToMenu", 3f);
  }

  public void goToMenu()
  {
      SuperTweener.Flush();
      Cortinilla.instance.Return();
  }

  private void InitializeServices() {
    disposables.Add( new ApplicationEvents() );
    disposables.Add( new ShotResultService() );
    disposables.Add( new ShotService() );
    disposables.Add( new DefenseService() );
    disposables.Add( new DifficultyService() );
    disposables.Add( new BullseyeService() );
    new GameplayService();
    disposables.Add( new PowerupService() );
    player = new Player( playerAttempts );
    disposables.Add( player );

  }

  private void InstantiatePrefabs() {
    DifficultyAreas = (GameObject)GameObject.Instantiate( difficultyAreas );
    DifficultyAreas.name = "Areas";
    ball = GameObject.Find("Balon");//(GameObject)GameObject.Instantiate( ballprefab, Vector3.zero, Quaternion.identity );
    ball.name = "ball";
    GameObject.Instantiate(visuals);
    GameObject.Instantiate(audioObject);
  }

  void ShotStarted(ShotInfo _shot)
  {
    lastShotinfo = _shot;
    lastTarget = _shot.Target;

    ScoreManager.Instance.SetLastShotInfo( lastShotinfo );
  }

  public void setRoundCooldown(float _time)
  {
    if(roundCooldown < _time)
    {
      roundCooldown = _time;
    }
  }

    public void ShotWhenReady(ShotInfoNet _info)
    {
        infoNet = _info;
        StartCoroutine("DelayShot");
    }

    ShotInfoNet infoNet;
    
    IEnumerator DelayShot()
    {
        while(BallPhysics.instance.state != BallPhysics.BallState.Waiting)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if(infoNet.usedPower != -1)
        {
            PowerupService.instance.UsePowerup((Powerup)infoNet.usedPower);
        }
        yield return new WaitForSeconds(3f);
        Thrower.instance.shotInfo = MsgThrow.UnloadShot(infoNet);
        Thrower.instance.DoThrow();
        GeneralSounds.instance.avisoDisparo();
        yield return true;
    }


  void ShotFinished(ShotResult result) {
    // Contabilizar los resultados.
    ComputeStats( result );

    ComputeDuelGameStats( result ); // contabilizamos resultados de Duelo

    if (playOnCourse) {
      playOnCourse = false;

      float time;
      if(Goalkeeper.instance)
      {
        time = (Goalkeeper.instance.GrabBall && result.Result == Result.Saved) ? 3f : 1.5f;
        if(gpService.GetGameMode() == GameMode.GoalKeeper && time < 1.5f) time = 2.5f;
      }
      else time = 1.5f;

      if(result.Result == Result.Stopped)
      {
        time = 0.25f;
        if(playingGoalKeeper) time += 1f;
      }

      setRoundCooldown(time);

    }
  }

  void StartPlay()
  {
    InputManager.instance.Blocked = false;

    if(GameplayService.networked) gpService.SwitchGameMode();

    var config = ServiceLocator.Request<IDifficultyService>().GetNextShotConfig();
    if (playingGoalKeeper) GoalCamera.instance.stateMachine.changeState = GoalKeeperCameraStates.Init.instance;
    else GoalCamera.instance.stateMachine.changeState = ThrowerCameraStates.Init.instance;
    kickEffects.instance.ShowRect();
    Vector3 pos = config.Position;
    playOnCourse = true;

    if (MissionManager.instance.HasCurrentMission()) {
        MissionRound mr = MissionManager.instance.GetMission().GetRoundInfo();
        if ( mr.HasPopUp() ) {
            Debug.Log( "PopUp: " + mr.GetPopUp().MessageID );
            Tutorial.instance.ThrowTuto(mr.GetPopUp().MessageID);
        }
        else
        {
            Tutorial.instance.DeactivateTutorial();
        }
    }

    // en modo duelo => ocultar la pastilla del tutorial
    if (GameplayService.networked)
        Tutorial.instance.DeactivateTutorial();

    BallPhysics.instance.Prepare(pos + new Vector3( 0, .1f, 0 ));

    Destroy(Thrower.instance.gameObject);
    SpawnThrower();

    if(Goalkeeper.instance) Goalkeeper.instance.Reset();

    Porteria.instance.SetKeeperMaterial(playingGoalKeeper);

    if(gpService.GetAuto()) Auto_Thrower.instance.Ready();
    if(!GameplayService.IsGoalkeeper() && GameplayService.networked)
    {
      Debug.Log(">>> DESDE AQUI SE LANZA EL CRONOMETRO");
      cntCuentaAtras.instance.Activar(
          // accion a realizar si se termina el tiempo
          (_name) => {
            InputManager.instance.Blocked = true;
            InputManager.instance.SendShotInfo(Auto_Thrower.GetRandomFail());
          });
    }

    Thrower.instance.SetPositionFor(ball.transform.position);

    Tutorial.instance.EnableTutorial();

    ifcThrower.instance.UpdateHabilidades();
  }

  void Destroy() {
    ServiceLocator.Request<IShotResultService>().UnregisterListener( ShotFinished );
    foreach (var disposable in disposables) {
      disposable.Dispose();
    }
    disposables.Clear();
  }

    #region Game Statistics

    public int golesMarcados = 0;
    public int dianasAcertadas = 0;
    public int balonesFallados = 0;
    public int balonesDespejados = 0;
    public int balonesParados = 0;
    public int golesEncajados = 0;

    private void ResetGameStats () {
        golesMarcados = 0;
        dianasAcertadas = 0;
        balonesFallados = 0;
        balonesDespejados = 0;
        balonesParados = 0;
        golesEncajados = 0;
    }

    private void ComputeStats (ShotResult result) {
        switch ( result.Result ) {
            case Result.Saved:
                balonesParados++;
                break;
            case Result.Stopped:
                balonesDespejados++;
                break;
            case Result.Goal:
                golesMarcados++;
                golesEncajados++;
                break;
            case Result.OutOfBounds:
                balonesFallados++;
                break;
            case Result.Target:
                dianasAcertadas++;
                break;
        }
    }

    #endregion

    #region Duel Game Stats

    public int DuelGameLocalGoalkeeperShots { get; protected set; }
    public int DuelGameLocalGoalkeeperStops { get; protected set; }
    public int DuelGameLocalShooterShots { get; protected set; }
    public int DuelGameLocalShooterGoals { get; protected set; }

    private void ResetDuelGameStats () {
        DuelGameLocalGoalkeeperShots = 0;
        DuelGameLocalGoalkeeperStops = 0;
        DuelGameLocalShooterShots = 0;
        DuelGameLocalShooterGoals = 0;
    }

    private void ComputeDuelGameStats (ShotResult result) {
        if ( GameplayService.networked ) {
            switch ( gpService.GetGameMode() ) {
                case GameMode.Shooter:
                    DuelGameLocalShooterShots++; // nuevo tiro como Lanzador
                    if ( result.Result == Result.Goal ) {
                        DuelGameLocalShooterGoals++; // win
                    }
                    break;
                case GameMode.GoalKeeper:
                    DuelGameLocalGoalkeeperShots++; // nuevo tiro como Portero
                    if ( ( result.Result == Result.Saved ) || 
                         ( result.Result == Result.Stopped ) ){
                        DuelGameLocalGoalkeeperStops++; // win
                    }
                    break;
            }
        }
    }

    public string GetDuelGameShooterGoalStat () {
        return ( DuelGameLocalShooterGoals.ToString() + 
                 " / " +
                 DuelGameLocalShooterShots.ToString() );
    }

    public string GetDuelGameGoalkeeperStopStat () {
        return ( DuelGameLocalGoalkeeperStops.ToString() +
                 " / " +
                 DuelGameLocalGoalkeeperShots.ToString() );
    }

    #endregion
}
