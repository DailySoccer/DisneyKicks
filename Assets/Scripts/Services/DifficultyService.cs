using System;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyService : IDifficultyService, IDisposable {

  const int roundsPorFase = 5;

  /// <summary>
  /// Completed rounds of a phase. 
  /// </summary>
  int completedRounds = 0;

  /// <summary>
  /// Number of phase we are in. Each phase has <roundsPorFase> rounds.
  /// </summary>
  public int fase = 0;

  public static GameObject currentBullseye;
  float crossHairTime = 1f;
  float successRadius = 0.5f;
  float centerPercent = 1f;
  float autoEffect = 0f;
  float crossHairAlpha = 1f;
  bool hardPositioning = false;
  Vector3 hardPosition = Vector3.zero;
  int multiplicador = 1;

  int barrera = 0;
  Rect area;
  bool firstGame = true;
  bool currentRect = false;
  Vector3 initialThrowPosition = new Vector3(0,0,-30f);


  int popPortero = 0;
  bool popZona = false;

  private event Action<ShotConfiguration> sendEvent = null;

  public void RegisterListener(Action<ShotConfiguration> listener) {
    sendEvent += listener;
  }

  public void UnregisterListener(Action<ShotConfiguration> listener) {
    sendEvent -= listener;
  }

  public void NewFase() {
    fase++;
    completedRounds = 0;
    /*if (sendEvent != null && ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts > 0) {
      sendEvent( fase );
    }*/
  }

  public void NextFase()
  {
    NewFase();
    sendEvent(lastConfig);
  }

  public int GetRounds()
  {
    return completedRounds;
  }

  public void PrevFase()
  {
    fase--;
    completedRounds = 0;
    sendEvent(lastConfig);
  }

  void MakeRect(float _area)
  {
    Rect porteria = new Rect(-3.55f, 0, 7.1f, 2.33f);
    float ratio = UnityEngine.Random.Range(0.80f, 1.20f);

    float w = Mathf.Sqrt(_area / ratio);
    float h = ratio * w;

    if(h > porteria.height)
    {
      h = porteria.height;
      w = _area / h;
    }
    else if(h < 1f) h = 1f;

    float x = UnityEngine.Random.Range (0, porteria.width - w);
    float y = UnityEngine.Random.Range (0f, porteria.height - h);

    /*if(currentBullseye != null)
    {
      if(currentBullseye.transform.position.x > Porteria.instance.transform.position.x)
      {
        if(Mathf.Abs(currentBullseye.transform.position.x) - Mathf.Abs(Porteria.instance.transform.position.x) < w)
        {

        }
      }
    }*/

    area = new Rect(x,y,w,h);
  }

  public Rect GetRect()
  {
    if(currentRect) return area;
    else return new Rect(0,0,0,0);
  }

  public float GetPanTime()
  {
    if(barrera > 0) return 0.4f;
    else return 0.6f;
  }

  private  struct ShotInformation {
    public ShotConfiguration Shot { get; set; }
    public bool Result { get; set; }
  }

  private Difficulty currentDifficulty = Difficulty.Easy;
  public Difficulty CurrentDifficulty {
    get { return currentDifficulty; }
    private set { currentDifficulty = value; }
  }

  private int successes = 0;

  private List<ShotInformation> shotHistory = new List<ShotInformation>();
  private ShotInformation? CurrentShot = null;

  public DifficultyService() {
    ServiceLocator.Register<IDifficultyService>( this );

    //ServiceLocator.Request<IShotResultService>().RegisterListener( OnShotFinished );
  }

  ShotConfiguration lastConfig;

  public ShotConfiguration GetNextShotConfig() {
    bool newFase = setupFase();

    Vector3 position;

    if ( hardPositioning ) {
        position = hardPosition;
    }
    else {
        position = AreaManager.GetRandomPoint( CurrentDifficulty, 
            ServiceLocator.Request<IGameplayService>().GetGameMode(), centerPercent );
    }

    ShotConfiguration config = new ShotConfiguration() {
      Mode = GameMode.Shooter,
      //Position = AreaManager.GetRandomPoint( CurrentDifficulty, ServiceLocator.Request<IGameplayService>().GetGameMode() ),
      Position = position,
      Bullseyes = null,
      Difficulty = CurrentDifficulty,
      Fase = fase,
      IsNewFase = newFase
    };

    // mostrar la barrera (si procede)
    SetupWall( position );       

    // mostrar sábana (si procede)
    SetupSheet();

    lastConfig = config;

    ShotInformation info = new ShotInformation() {
      Shot = config,
      Result = false
    };

    CurrentShot = info;
    sendEvent(config);
    return config;
  }

  public ShotConfiguration GetLastShotConfig() {
    return lastConfig;
  }

  bool setupFase() {

    ResetRoundInfo(); // Reseteamos las variables HasBullseye, HasSheet y HasWall

    if ( !GameplayService.networked )
    {
        bool newFase = false;
        if ( completedRounds == 0 && !firstGame ) {
            NewFase();
            newFase = true;
        }
        else {
            firstGame = false;
        }
        
        if ( ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter ) {
            setupFaseIniesta();
        }
        else {
            setupFaseCasillas();
        }

        return newFase;
    }
    else {
        setupFaseNetwork();
        return false;
    }
  }

  public void OnShotFinished(ShotResult result) {

    shotHistory.Add( CurrentShot.Value );
    completedRounds = ( completedRounds + 1 );

    if ( ( !GameplayService.networked ) &&
         ( MissionManager.instance.HasCurrentMission() ) ) {
        Mission mission = MissionManager.instance.GetMission();

        mission.NextRound(); // next round


        // has this mission finished ?
        // fin de mision
        if ( mission.IsMissionFinished() ) {
            ServiceLocator.Request<IPlayerService>().SetGameOver();

            FieldControl.instance.GameOver();
        }
    }

    if (result.Result == Result.Goal) {
        CurrentShot = new ShotInformation() {
        Result = true,
        Shot = CurrentShot.Value.Shot
      };

      ++successes;
      //UnityEngine.Debug.Log( "Successes: " + successes );
    }
  }

  public void Dispose() {
    //ServiceLocator.Request<IShotResultService>().UnregisterListener( OnShotFinished );
  }

  public float GetEffect()
  {
    return autoEffect;
  }

  void setupFaseNetwork()
    {
        if(!FieldControl.instance.goalKeeper) FieldControl.instance.goalKeeper = true;
        hardPositioning = true;
        hardPosition = new Vector3(0,0,-30f);
        currentRect = false;
        crossHairAlpha = Habilidades.IsActiveSkill(Habilidades.Skills.Premonicion) ? 1f : 0f;
        successRadius = 1.25f;
    }    

  /// <summary>
  /// Prepara la partida para jugar en modo TIRADOR
  /// </summary>
  public void setupFaseIniesta() {

      if ( MissionManager.instance.HasCurrentMission() ) {
          SetupShooterMissionRound();
      }
      else {
          SetHardPositioning();
          multiplicador = GetMultiplier();
          SetCenterPercent();
          SetCurrentDifficulty();
          bool hayPortero = SetGoalKeeper();
          SetBullEyes( hayPortero );
          bool hayZona = SetZonaTiro( hayPortero );
          ShowPopUp(hayPortero, hayZona);
      }
  }

  /// <summary>
  /// Prepara la partida para jugar en modo PORTERO
  /// </summary>
  public void setupFaseCasillas() {
      if ( MissionManager.instance.HasCurrentMission() ) {
          SetupGoalkeeperMissionRound();
      }
      else {
          SetCrossAirAlpha();
          //SetSuccessRadius();
          multiplicador = GetMultiplier();
          SetHardPositioning();
          SetCurrentDifficulty();
          SetCenterPercent();
          SetAutoEffect();
          ShowPopUp();
      }
  }

  public void SetupWall (Vector3 position) {
      int barrerasExtra = 0;
      if(Habilidades.IsActiveSkill(Habilidades.Skills.Barrera)) barrerasExtra = 2;
      if(Habilidades.IsActiveSkill(Habilidades.Skills.BarreraPro)) barrerasExtra = 3;
      if ( ( MissionManager.instance.HasCurrentMission() ) &&
           ( MissionManager.instance.GetMission().PlayerType == GameMode.Shooter ) ) {
        ShooterMissionRound mr = (ShooterMissionRound)MissionManager.instance.GetMission().GetRoundInfo();
        if ( mr.HasWall ) {
            FieldControl.instance.HasWall = true;
            BarreraManager.instance.Create( mr.WallSize + barrerasExtra, position );
        }
        else {
            FieldControl.instance.HasWall = false;
            BarreraManager.instance.Create( 0 + barrerasExtra, position );
        }
      }
      else {
          int wallPlayers = GetNumJugadoresBarrera();
          if ( wallPlayers > 0 ) {
              FieldControl.instance.HasWall = true;
              BarreraManager.instance.Create( wallPlayers + barrerasExtra, position );
          }
          else {
              FieldControl.instance.HasWall = false;
              BarreraManager.instance.Create( 0 + barrerasExtra, position );
          }
      }
  }

  public void SetupSheet () {
      if ( ( MissionManager.instance.HasCurrentMission() ) &&
           ( MissionManager.instance.GetMission().PlayerType == GameMode.Shooter ) ) {
          ShooterMissionRound mr = (ShooterMissionRound)MissionManager.instance.GetMission().GetRoundInfo();
          if ( mr.HasSheet ) {
              FieldControl.instance.HasSheet = true;

              // llamar al manager de sabanas e inicializarlo con el array de sectores-dificultad leido de la mision              
              SabanasManager.instance.ShowSabanas( mr.SheetSectorDifficulties );
          }
          else {
              FieldControl.instance.HasSheet = false;
              SabanasManager.instance.ShowSabanas( null );
          }
      }      
  }

  public static string FaseToName(int _fase)
  {
    string[] niveles = new string[] {
      "CALENTAMIENTO",
      "APRENDIZ",
      "NOVATO",
      "INICIADO",
      "ASPIRANTE",
      "AMATEUR",
      "FEDERADO",
      "PROFESIONAL",
      "LIGA ADELANTE",
      "LIGA BBVA",
      "TITULAR",
      "CONVOCADO",
      "CAMPEÓN",
      "BOTA DE ORO",
      "ESTRELLA",
      "SÚPER ESTRELLA",
      "GALÁCTICO",
      "ÍDOLO",
      "ASTRO",
      "MITO",
      ""        // <= EL ULTIMO VALOR ES VACIO PARA NO TENER QUE MOSTRAR REPETIDAMENTE LAS ALERTAS DE CAMBIO DE NIVEL
    };
    if(_fase < 0) _fase = 0;
    if(_fase >= niveles.Length) _fase = niveles.Length - 1;
    return niveles[_fase];
  }

  public void ConfigCrossHair( Vector3 _position, float _time ) {
    kickEffects.instance.aracnoSense(_position, _time * crossHairTime, crossHairAlpha);
    kickEffects.instance.AreaIntuition(_position, _time, 1f);
  }

  public int GetFase()
  {
    return fase;
  }

  public Bullseye GetBullseye()
  {
    return currentBullseye ? currentBullseye.GetComponent<Bullseye>() : null;
  }

  // ------------------------------------------------------------------------------
  // ---  METODOS PARA CALCULAR LOS ELEMENTOS DE JUEGO ----------------------------
  // ------------------------------------------------------------------------------

  // NOTAS:
  // GameplayService.networked => Indica que se esta jugando en multijugador
  // GameplayService.initialGameMode => Indica modo (no multijugador): PORTERO, JUGADOR
  // GameplayService.modoJuego.tipoModo => Indica modo: NORMAL, EXPERTO, LEYENDA, TIME_ATTACK
  // fase => indica que ronda de 5 tiros se esta jugando (cada 5 tiros se incrementa en uno)
  // won_rounds => indica que tiro se esta ejecutando dentro de una fase (toma valores en el rango [0..4])
  // para saber que tiro se esta realizando hacer el calculo:  (fase * 5) + won_rounds

  /// <summary>
  /// Devuelve el numero de jugadores que debe haber de barrera en funcion de la fase actual y el modo de juego seleccionado
  /// Nota: 0 es que no hay barrera
  /// </summary>
  /// <returns></returns>
  private int GetNumJugadoresBarrera() {
      // valor por defecto
      int numJugadores = 0;

      // si es TIRADOR
      if (GameplayService.initialGameMode == GameMode.Shooter) {
          // devolver un numero de jugadores de la barrera conforme al modo de juego: NORMAL, EXPERTO, LEYENDA...
          switch (GameplayService.modoJuego.tipoModo) {
              case ModoJuego.TipoModo.EXPERTO:
                  if (fase == 0 || fase == 1 || fase == 3)
                      numJugadores = 2;
                  else if (fase == 4 || fase == 5 || fase == 6)
                      numJugadores = 3;
                  else if (fase == 7 || fase >= 9) 
                      numJugadores = 4;
                  break;

              case ModoJuego.TipoModo.LEYENDA:
                  if (fase == 3 || fase == 4)
                      numJugadores = 1;
                  else if (fase == 6)
                      numJugadores = 2;
                  else if (fase == 7 || (fase >= 9 && (fase % 2 == 1)))  // <= a partir de la fase 9 cada fase impar
                      numJugadores = 3;
                  break;
          }
      }

      return numJugadores;
  }


  /// <summary>
  /// Devuelve el multilplicador de puntos en funcion de la fase actual y el modo de juego seleccionado
  /// </summary>
  /// <returns></returns>
  public int GetMultiplier() {
      if (ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) {
          if (fase > 9)
              multiplicador = 3;
          else if (fase > 4)
              multiplicador = 2;
          else
              multiplicador = 1;
      } else {
          if (fase > 7)
              multiplicador = 3;
          else if (fase > 2)
              multiplicador = 2;
          else
              multiplicador = 1;
      }
      return multiplicador;
  }

  /// <summary>
  /// Devuelve la opacidad de el punto de mira hacia el que va el balon en funcion de la fase actual y el modo de juego seleccionado
  /// </summary>
  private void SetCrossAirAlpha() {
      // por defecto
      crossHairAlpha = 0.0f;
      
      // en la primera fase del modo portero
      if (GameplayService.initialGameMode == GameMode.GoalKeeper) {
          if (fase == 0)
              crossHairAlpha = 1.0f;
      }
      if(Habilidades.IsActiveSkill(Habilidades.Skills.Premonicion))
      {
        crossHairAlpha = 1f;
      }
  }

  /// <summary>
  /// Devuelve el radio sobre el punto destino del tiro que define que una parada es efectiva
  /// </summary>
  /// <param name="_target"></param>
  /// <returns></returns>
  public float GetSuccessRadius(Vector3 _target) {
      float SUCCESS_RADIUS_MIN = 1.00f;
      float SUCCESS_RADIUS_MAX = 1.50f;

      if(Habilidades.IsActiveSkill(Habilidades.Skills.Practico))
      {
        SUCCESS_RADIUS_MIN = 1.25f;
        SUCCESS_RADIUS_MAX = 1.75f;
      }

      //successRadius = 1.5;

      // desviacion en la horizontal
      float hDesviacion = Mathf.Clamp01(Mathf.Abs(_target.x) / 3.5f);

      // desviacion en la vertical
      float vDesviacion = Mathf.Clamp01(Mathf.Abs(_target.y - 1.2f) / 1.1f);

      // calcular la desviacion global (ponderando la vertical y horizontal)
      float desviacion01 = hDesviacion * 0.6f + vDesviacion * 0.4f;

      // calcular el success radius
      successRadius = Mathf.Lerp(SUCCESS_RADIUS_MAX, SUCCESS_RADIUS_MIN, desviacion01);// <= a mayor desviacion menor radio de acierto (mas dificil de parar)
      if(Habilidades.IsActiveSkill(Habilidades.Skills.Mago_balon) && Mathf.Abs(ShotService.lastShot.Effect01) > 0.65f)
      {
          successRadius *= 0.8f;
      }
      return successRadius;
  }

    /// <summary>
    /// Devuelve el radio de la parada perfecta.
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    public float GetPerfectRadius()
    {
        float PERFECT_RADIUS_BASE = 0.3f;

        float perfectRadius = PERFECT_RADIUS_BASE;
        if(Habilidades.IsActiveSkill(Habilidades.Skills.Practico))
        {
            perfectRadius = 0.1f;
        }
        if(PowerupService.instance.IsPowerActive(Powerup.Manoplas))
        {
            perfectRadius *= 2.5f;
        }
        return perfectRadius;
    }

  /// <summary>
  /// Muestra un pop up en funcion de la fase actual y el modo de juego seleccionado y actualiza el valor de GoalKeeper 
  /// </summary>
  private void ShowPopUp(bool _hayPortero = false, bool _hayZona = false) {
      // en el modo time_attack no se muestran popUps
      //if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK)
      //    return;

      // en el modo multijugador => no hay PopUps
      if (GameplayService.networked)
          return;

      if (GameplayService.initialGameMode == GameMode.GoalKeeper) {
          // popups de PORTERO
			if (completedRounds == 0) {     // <= mostrar el popUp solo en el primer tiro de la fase
              if (fase == 0 && GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK)
              {
                  ifcPopUp.instance.Show("¡MODO TIME ATTACK!", "Compite contra el reloj sumando segundos al cronómetro con cada parada.");
              }
              else if (fase == 1 && GameplayService.modoJuego.tipoModo != ModoJuego.TipoModo.TIME_ATTACK)
                  ifcPopUp.instance.Show("ATENCIÓN", "¡A partir de ahora juegas sin ayuda!");
              else if (fase == 3 && GameplayService.modoJuego.tipoModo != ModoJuego.TipoModo.TIME_ATTACK)
                  ifcPopUp.instance.Show("ATENCIÓN", "La dificultad seguirá aumentando, ¿estás preparado?");
          }
      } else {
          // popups de TIRADOR
			if (completedRounds == 0) {     // <= mostrar el popUp solo en el primer tiro de la fase
              if (fase == 0) {
                  switch (GameplayService.modoJuego.tipoModo) {
                      case ModoJuego.TipoModo.EXPERTO:
                          ifcPopUp.instance.Show("¡MODO EXPERTO!", "Ahora las barreras se interpondrán entre tu jugador y el objetivo.");
                          break;
                      case ModoJuego.TipoModo.LEYENDA:
                          ifcPopUp.instance.Show("¡MODO LEYENDA!", "Ahora inténtalo con dianas móviles y pruebas de mayor dificultad");
                          break;
                      case ModoJuego.TipoModo.TIME_ATTACK:
                          ifcPopUp.instance.Show("¡MODO TIME ATTACK!", "Consigue segundos con cada acierto y ¡Cuidado, los fallos restan tiempo!");
                          break;
                  }
              }
              else if(GameplayService.modoJuego.tipoModo != ModoJuego.TipoModo.TIME_ATTACK)
              {
                  if (_hayPortero && popPortero == 0)//(fase == 2 && !FieldControl.instance.goalKeeper)
                  {
                      ifcPopUp.instance.Show("¡BIEN JUGADO!", "Pero ahora tendrás que batirme a mí.");
                      popPortero ++;
                  }
                  else if (!popZona && _hayZona)//(fase == 3)
                  {
                      ifcPopUp.instance.Show("ZONA DE PRECISIÓN", "Procura acertar en la diana o en la zona de precisión para evitar perder una vida.");
                      popZona = true;
                  }
                  else if (_hayPortero && popPortero == 1)//(fase == 5 && !FieldControl.instance.goalKeeper)
                  {
                      ifcPopUp.instance.Show("¡NO ESTÁ MAL!", "Ahora empléate a fondo...");
                      popPortero++;
                  }
                  else if (_hayPortero && popPortero == 2) //((fase == 8 || fase == 10) && !FieldControl.instance.goalKeeper)
                  {
                      ifcPopUp.instance.Show("¡HAS LLEGADO LEJOS!", "Pero ahora se acabó lo fácil...");
                      popPortero++;
                  }
              }
          }
      }
  }

  /// <summary>
  /// Define la posicion del tirador en funcion de la fase actual y el modo de juego seleccionado
  /// </summary>
  private void SetHardPositioning() {
      // valor por defecto
      hardPositioning = false;

      if (GameplayService.initialGameMode == GameMode.GoalKeeper) {
          // para PORTERO
          switch (fase) {
              case 0:
                  hardPositioning = true;
                  hardPosition = new Vector3(0f, 0f, -27f);
                  break;
              case 1:
                  hardPositioning = true;
                  hardPosition = new Vector3(0f, 0f, -27f);
                  break;
              case 2:
                  hardPositioning = true;
                  hardPosition = new Vector3(-10f, 0f, -30f);
                  break;
          }
      } else {
          // para TIRADOR
          if ( fase == 0 && completedRounds == 0 ) {
              hardPositioning = true;
              hardPosition = initialThrowPosition;
          }
      }
  }

  /// <summary>
  /// Calcula la distancia desde la que debe disparar el jugador en funcion de la fase actual y el modo de juego seleccionado
  /// </summary>
  /// <returns></returns>
  private void SetCurrentDifficulty() {
      // valor por defecto
      currentDifficulty = Difficulty.Medium;

      if (GameplayService.initialGameMode == GameMode.GoalKeeper) {
          // PORTERO
          if (fase < 2)
              currentDifficulty = Difficulty.Easy;
          else if (fase < 5)
              currentDifficulty = Difficulty.Medium;
          else
              currentDifficulty = Difficulty.Hard;

      } else {
          // para TIRADOR
          switch (fase) {
              case 0:
                  currentDifficulty = Difficulty.Easy;
                  break;
              case 1:
                  currentDifficulty = Difficulty.Medium;
                  break;
              case 2:
                  currentDifficulty = Difficulty.Medium;
                  break;
              case 3:
                  currentDifficulty = Difficulty.Easy;
                  break;
              case 4:
                  currentDifficulty = Difficulty.Easy;
                  break;
              case 5:
                  currentDifficulty = Difficulty.Medium;
                  break;
              case 6:
                  currentDifficulty = Difficulty.Hard;
                  break;
              case 7:
                  currentDifficulty = Difficulty.Easy;
                  break;
              case 8:
                  currentDifficulty = Difficulty.Medium;
                  break;
              default:
                  currentDifficulty = Difficulty.Hard;
                  break;
          }
      }
  }

  /// <summary>
  /// Si 0.2f centra al tirador en la zona desde la que dispara, si -0.2f lo situa en los eextremos
  /// </summary>
  private void SetCenterPercent() {
      // valor por defecto
      centerPercent = 1.0f;

      if ( MissionManager.instance.HasCurrentMission() ) {
          if ( MissionManager.instance.GetMission().PlayerType == GameMode.GoalKeeper ) {
              GoalkeeperMissionRound mr = (GoalkeeperMissionRound)MissionManager.instance.GetMission().GetRoundInfo();
              centerPercent = ( mr.IsCenteredShot ? 0.2f : 1.0f );
          }
      }     
  }

  /// <summary>
  /// Devuelve un valor para la coordenada X del tiro a porteria en funcion del modo y la fase que se esta jugando
  /// </summary>
  /// <param name="_ballPosX"></param>
  /// <param name="_porteriaLocalScaleX">Valor del localScale de la porteria en el eje X</param>
  /// <returns></returns>
  public float GetCoordenadaXTiro(float _ballPosX, float _porteriaLocalScaleX) {
      float desviacionMax = 0.9f;
      float desviacionMin = 0.0f;

      // en modo portero => definir las desviaciones maxima y minima en funcion de la fase
      if (GameplayService.initialGameMode == GameMode.GoalKeeper) {
          if (fase < 1) {
              desviacionMin = 0.2f;
              desviacionMax = 0.4f;
          } else if (fase < 2) {
              desviacionMin = 0.0f;
              desviacionMax = 0.5f;
          } else if (fase < 4) {
              desviacionMin = 0.3f;
              desviacionMax = 0.6f;
          } else if (fase < 5) {
              desviacionMin = 0.4f;
              desviacionMax = 0.7f;
          } else {
              desviacionMin = 0.6f;
              desviacionMax = 0.9f;
          }
      }

      // calcular una desviacion entre la maxima y la minima (con un signo al azar)
      float desviacion = UnityEngine.Random.Range(desviacionMin, desviacionMax);

      // cambiar el signo del disparo al azar
      if (UnityEngine.Random.Range(-1.0f, 1.0f) < 0)
          desviacion = -desviacion;

      return (_ballPosX + (_porteriaLocalScaleX / 2) * desviacion);
  }


  /// <summary>
  /// Cantidad de efecto que le da a la bola el tirador
  /// </summary>
  private void SetAutoEffect() {
      // valor por defecto
      autoEffect = 0.0f;

      if (GameplayService.initialGameMode == GameMode.GoalKeeper) {
          // si es PORTERO
          if (fase < 1)
              autoEffect = 0.0f; // efecto nulo
          else if (fase < 2)
              autoEffect = UnityEngine.Random.Range(0.0f, 0.4f); // efecto leve
          else if (fase < 5)
              autoEffect = UnityEngine.Random.Range(0.4f, 0.8f); // efecto medio
          else
              autoEffect = UnityEngine.Random.Range(0.8f, 1.0f); // efecto dificil
      } 
  }

  /// <summary>
  /// Comprueba si debe haber o no portero y lo muestra u oculta en consecuencia
  /// Nota: Esto es para modo PORTERO
  /// </summary>
  private bool SetGoalKeeper() {
      bool hayPortero = false;

      // si es PORTERO
      if (GameplayService.initialGameMode == GameMode.Shooter) {
          switch (GameplayService.modoJuego.tipoModo) {
              case ModoJuego.TipoModo.NORMAL:
              case ModoJuego.TipoModo.LEYENDA:
                  hayPortero = (fase == 2 || fase == 5 || (fase > 9 && (fase % 2 == 0)));   // <= cada fase despues de la 9 que sea par
                  break;

              case ModoJuego.TipoModo.EXPERTO:
                  hayPortero = (fase == 2 || fase == 4 || fase == 6 || fase == 8 || (fase > 9 && (fase % 2 == 0)));   // <= cada fase despues de la 9 que sea par
                  break;
          }
      }

      FieldControl.instance.goalKeeper = hayPortero;
      return hayPortero;
  }

  /// <summary>
  /// Comprueba como se deben pintar las dianas
  /// </summary>
  /// <param name="_hayPortero">Indica si hay portero para este tiro</param>
  /// <returns></returns>
  private void SetBullEyes(bool _hayPortero) {
      if (currentBullseye != null)
          GameObject.Destroy(currentBullseye);

      // SI ES TIRADOR
      if (GameplayService.initialGameMode == GameMode.Shooter) {
          // si no hay portero
          if (!_hayPortero) {
              // si no es una fase en la que hay portero pero no debe haber dianas expresamente
              if (!(GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.EXPERTO && fase == 0)) {
                  SizeOfBullseye[] sizes = { };
                  HeightOfBullseye[] heights = { };
                  ZoneOfBullseye zone = ZoneOfBullseye.Centro;
                  Vector3 velocidadInicial = Vector3.zero;

                  // usar esta variable para asegurar que a partir de la fase 9 las fases se ciclan
                  int auxFase = fase;
                  if (auxFase > 10) {
                      if (fase % 2 == 1)
                          // si la fase es impar => como la 9
                          auxFase = 9;
                      else
                          // si la fase es par => como la 10
                          auxFase = 10;
                  }
                  // definir la posicion inicial de las dianas
                  switch (auxFase) {
                      case 0:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.L, SizeOfBullseye.M };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Baja };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 1:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.L };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Baja };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 2:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.L, SizeOfBullseye.M };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Baja };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 3:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.L };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Baja };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 4:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.S };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 5:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.S };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 6:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.M };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Centro;
                          break;
                      case 7:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.S };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Lados;
                          break;
                      case 8:
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.S };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Lados;
                          break;
                      case 9: // NOTA: a partir de la fase 9, las fases impares se ejecutan de esta manera
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.S };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Lados;
                          break;
                      case 10: // NOTA: a partir de la fase 10, las fases pares se ejecutan de esta manera
                          sizes = new SizeOfBullseye[] { SizeOfBullseye.S };
                          heights = new HeightOfBullseye[] { HeightOfBullseye.Media, HeightOfBullseye.Alta };
                          zone = ZoneOfBullseye.Lados;
                          break;
                  }

                  // asignar velocidad a las dianas
                  // NOTA: si las dianas se mueven => hacer que su tamaño sea grande
                  if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.LEYENDA && (auxFase == 0 || auxFase == 1 || auxFase == 3 || auxFase == 4 || auxFase == 6 || auxFase == 7 || auxFase == 9)) {
                      velocidadInicial = new Vector3(0.3f, 0.3f, 0.0f);
                      sizes = new SizeOfBullseye[] { SizeOfBullseye.L };
                  }

                  BullseyeManager.Instance.defSize = sizes;
                  BullseyeManager.Instance.defHeight = heights;
                  BullseyeManager.Instance.defZone = zone;
                  BullseyeManager.Instance.defInitialSpeed = velocidadInicial;

                  // crear la diana
                  currentBullseye = BullseyeManager.Instance.CreateBullseye();
              }
          }
      }
  }

  /// <summary>
  /// Define las regiones de acierto de tiro (los rectangulos esos que se pintan de amarillo)
  /// </summary>
  /// <param name="_hayPortero"></param>
  public bool SetZonaTiro(bool _hayPortero) {
      // valor por defecto
      currentRect = false;

      // si es TIRADOR y no hay portero
      if ((GameplayService.initialGameMode == GameMode.Shooter) && (!_hayPortero)) {
          switch (GameplayService.modoJuego.tipoModo) {
              case ModoJuego.TipoModo.NORMAL:
              case ModoJuego.TipoModo.TIME_ATTACK:
                  if (fase > 2)
                      currentRect = true;
                  break;
              case ModoJuego.TipoModo.EXPERTO:
                  if (fase == 3 || fase == 5 || fase == 7 || (fase >= 9 && (fase % 2 == 1)))   // <= a partir de la fase 9 las impares
                      currentRect = true;
                  break;
              case ModoJuego.TipoModo.LEYENDA:
                  if (fase == 4 || fase == 7 || (fase >= 9 && (fase % 2 == 1)))   // <= a partir de la fase 9 las impares
                      currentRect = true;
                  break;
          }
      }

      // si hay que mostrar barrera => crearla
      if (currentRect)
          MakeRect(Mathf.Clamp(6f - (fase - 3f) * 1f, 1f, 10f));

      return currentRect;
  }

  /// <summary>
  /// Devuelve la probabilidad BASE de parada en funcion de la fase actual y el modo de juego seleccionado
  /// NOTA: Esta funcion es estatica para poder ser accedida desde algunos puntos concretos del codigo
  /// </summary>
  /// <param name="_fase"></param>
  /// <param name="_tipoModoJuego"></param>
  /// <returns></returns>
  public static float GetProbabilidadBaseDeParada()
  {
      // valor por defecto
      float baseProb = 0.2f;

      if(MissionManager.instance.HasCurrentMission())
      {
          if(MissionManager.instance.GetMission().PlayerType == GameMode.Shooter)
          {
            baseProb = (MissionManager.instance.GetMission().GetRoundInfo() as ShooterMissionRound).GoalkeeperSkill;
          }
      }

      return baseProb;
  }

  /// <summary>
  /// Metodo encargado de aplicar modificadores al cronometro del modo time_attack de TIRADOR
  /// </summary>
  /// <param name="_info"></param>
  public static void ModificarTiempoDeTimeAttack_Tirador(ShotResult _info) {
      if (GameplayService.initialGameMode == GameMode.Shooter && GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK) {
          // tiro perfecto
          if (_info.Perfect) {
              cntCronoTimeAttack.instance.AddTiempo(6.0f);
          } else {
              switch (_info.Result) {
                  case Result.Target: // diana
                      cntCronoTimeAttack.instance.AddTiempo(4.0f);
                      break;

                  case Result.Goal: // gol
                      // comprobar si hay portero
                      if (FieldControl.instance.goalKeeper) {
                          cntCronoTimeAttack.instance.AddTiempo(4.0f);
                      } else {
                          // comprobar si hay definida una zona
                          switch (_info.AreaResult) {
                              // no hay zona definida => gol normal
                              case AreaResultValues.NoAreaExists:
                                  cntCronoTimeAttack.instance.AddTiempo(2.0f);
                                  break;

                              // gol fuera de zona => cuenta como fallo
                              case AreaResultValues.BallFailsArea:
                                  cntCronoTimeAttack.instance.AddTiempo(-4.0f);
                                  break;

                              // gol en zona
                              case AreaResultValues.BallHitsArea:
                                  cntCronoTimeAttack.instance.AddTiempo(2.0f);
                                  break;
                          }
                      }
                      break;

                  case Result.OutOfBounds: // FUERA
                      cntCronoTimeAttack.instance.AddTiempo(-4.0f);
                      break;

                  case Result.Stopped: // despeje
                      cntCronoTimeAttack.instance.AddTiempo(-5.0f);
                      break;

                  case Result.Saved: // parada
                      cntCronoTimeAttack.instance.AddTiempo(-5.0f);
                      break;
              }
          }
      }
  }

  /// <summary>
  /// Metodo encargado de aplicar modificadores al cronometro del modo time_attack de PORTERO
  /// </summary>
  /// <param name="_info"></param>
  public static void ModificarTiempoDeTimeAttack_Portero(ShotResult _info) {
      if (GameplayService.initialGameMode == GameMode.GoalKeeper && GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK) {

          switch (_info.Result) {
              case Result.OutOfBounds: // fuera
                  cntCronoTimeAttack.instance.AddTiempo(6.0f);
                  break;

              case Result.Goal: // gol
                  cntCronoTimeAttack.instance.AddTiempo(-5.0f);
                  break;

              case Result.Saved: // parada
                  if (_info.DefenseResult == GKResult.Perfect) {
                      // parada perfecta
                      cntCronoTimeAttack.instance.AddTiempo(10.0f);
                  } else {
                      cntCronoTimeAttack.instance.AddTiempo(4.0f);
                  }
                  break;

              case Result.Stopped: // despeje
                  if (_info.DefenseResult == GKResult.Perfect) {
                      // despeje perfecto
                      cntCronoTimeAttack.instance.AddTiempo(8.0f);
                  } else {
                      cntCronoTimeAttack.instance.AddTiempo(4.0f);
                  }
                  break;
          }
      }
  }


  /// <summary>
  /// Pinta las sabanas que corresponda delante de la porteria
  /// </summary>
  public void PintarSabanas() {
      SabanasManager.TipoSabanaInidividual[] tiposSabanas = new SabanasManager.TipoSabanaInidividual[SabanasManager.NUM_MAX_SABANAS];

      // F4KE: ejemplo de array de sabanas
      tiposSabanas[0] = SabanasManager.TipoSabanaInidividual.M;
      tiposSabanas[1] = SabanasManager.TipoSabanaInidividual.NONE;
      tiposSabanas[2] = SabanasManager.TipoSabanaInidividual.S;

      // llamar al manager de sabanas
      SabanasManager.instance.ShowSabanas(tiposSabanas);
  }


  #region Missions support

  private void ResetRoundInfo () {
      FieldControl.instance.HasBullseye = false;
      FieldControl.instance.HasSheet = false;
      FieldControl.instance.HasWall = false;
  }

  private void SetupShooterMissionRound () {
      multiplicador = 1; // not used in Bitoon Kicks

      SetCenterPercent();

      ShooterMissionRound round = (ShooterMissionRound)MissionManager.instance.GetMission().GetRoundInfo();
      currentDifficulty = round.GetDifficulty();

      FieldControl.instance.goalKeeper = round.HasGoalkeeper;

      currentRect = false;

      // Nos cepillamos cualquier diana que pudiera haber antes
      if ( currentBullseye != null ) {
          GameObject.Destroy( currentBullseye );
          currentBullseye = null;
      }

      // Y ahora generamos una diana nueva si es necesario
      if ( round.HasBullseye ) {
          SetMissionBullseye( round.bullseyeDesc );
          FieldControl.instance.HasBullseye = true;
      }
      else {
          FieldControl.instance.HasBullseye = false;
      }
  }

  private static System.Timers.Timer aTimer;

  private void SetupGoalkeeperMissionRound () {
      multiplicador = 1; // not used in Bitoon Kicks

      GoalkeeperMissionRound round = (GoalkeeperMissionRound)MissionManager.instance.GetMission().GetRoundInfo();

      crossHairAlpha = round.HasHelp ? 1.0f : 0.0f;
      if(Habilidades.IsActiveSkill(Habilidades.Skills.Premonicion))
      {
        crossHairAlpha = 1f;
      }
      currentDifficulty = round.GetDifficulty();

      SetCenterPercent();

      autoEffect = round.BallEffect;

      if(round.HasPowerUp)
      {
        Auto_Thrower.instance.queuedPowerUp = round.PowerupType;
        Auto_Thrower.instance.powerUpEnqueued = true;
      }
  }

  


  private void SetMissionBullseye (ShooterMissionRound.BullseyeDesc bullseyeDesc) {

      BullseyeManager.Instance.defSize = new SizeOfBullseye[] { bullseyeDesc.Size };
      BullseyeManager.Instance.defHeight = new HeightOfBullseye[] { bullseyeDesc.Height };
      BullseyeManager.Instance.defZone = bullseyeDesc.bullseyePosition;
      BullseyeManager.Instance.defInitialSpeed = bullseyeDesc.HasStaticPosition ? Vector3.zero : new Vector3( 0.3f, 0.3f, 0.0f );
      BullseyeManager.Instance.staticSize = bullseyeDesc.HasStaticSize;

      // crear la diana
      currentBullseye = BullseyeManager.Instance.CreateBullseye();

      // Yellow zone if any
      if ( bullseyeDesc.HasYellowZone ) {
          currentRect = true;
          MakeRect( GetYellowZoneSize( bullseyeDesc.yellowZoneDesc ) );
      }
  }

  private float GetYellowZoneSize (ShooterMissionRound.YellowZoneDesc yellowZoneDesc) {
      switch ( yellowZoneDesc.Size ) {
          case ShooterMissionRound.YellowZoneDesc.SizeOfYellowZone.S: return 1.0f;
          case ShooterMissionRound.YellowZoneDesc.SizeOfYellowZone.M: return 5.0f;
          case ShooterMissionRound.YellowZoneDesc.SizeOfYellowZone.L: return 10.0f;
      }

      throw new ArgumentOutOfRangeException( "Tamaño no reconocido" );
  }

  /// <summary>
  /// Devuelve el ratio por el que hay que multiplicar los punto
  /// </summary>
  /// <param name="_ganadoDuelo">Este parametro es solo relevante si se esta en modo duelo (multiplayer), true si el jugador local ha ganado el duelo, false en caso contrario.</param>
  /// <returns></returns>
  public static float GetRatioRecompensa(bool _ganadoDuelo = false) {
      float ratio = 1.0f;   // valor por defecto

      if(GameplayService.networked)
      {
        ratio = 1.0f / 20.0f;
      }
      else
      {
        ratio = 1.0f / 30.0f;
      }

      return ratio;
  }

  #endregion
}
