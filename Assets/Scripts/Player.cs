using System;
using UnityEngine;

public class Player : IPlayerService, IDisposable
{
  public const int recompensaMultijugadorWin = 500;
  public const int recompensaMultijugadorFail = 250;

  int maxAttempts;
  public int attempts;

  int points;
  bool gameOver = false;

  static MatchState m_serverState;
  static bool newState = false;
  public static MatchState serverState
  {
    get{ return m_serverState; }
    set{ m_serverState = value; newState = true;}
  }

  public void SetLives(int lives)
  {
    attempts = lives;
  }

  public static int record_thrower = -1;
  public static int record_keeper = -1;

  private event Action<PlayerInfo> PlayerUpdated = null;


  /// <summary>
  /// Metodo para dejar al jugador en estado de game over
  /// </summary>
  public void SetGameOver() {
      
      //this.attempts = 0;
      this.gameOver = true;
  }

  public bool IsGameOver()
  {
    return gameOver;
  }



  public Player(int attempts) {
    this.maxAttempts = attempts;
    this.attempts = maxAttempts;

    serverState = new MatchState {marker_1 = new int[]{0,0,0,0,0}, marker_2  = new int[]{0,0,0,0,0}, rounds = 0, score_1 = 0, score_2 = 0};
    newState = false;

    ServiceLocator.Register<IPlayerService>( this );
    //ServiceLocator.Request<IShotResultService>().RegisterListener( TryShotResult );
    ////ServiceLocator.Request<IBullseyeService>().RegisterListener( TryBullEyeSuccess );
  }

  public PlayerInfo GetPlayerInfo() {
    return new PlayerInfo() {
      Attempts = this.attempts,
      Points = this.points
    };
  }

  private void ShotFailed() {
    // en modo multijugador o time_attack => no restar vidas
    if(GameplayService.networked || GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK)
        return; //TODO mejorar las condiciones de gameover + network


    if (this.attempts > 0)
    {
      --this.attempts;
    }

    // si no se esta jugando en modo time_attack y se agotan las vidas => game over
    if (GameplayService.modoJuego.tipoModo != ModoJuego.TipoModo.TIME_ATTACK && this.attempts <= 0) { 
      //points = 0;
      gameOver = true;
      // End game
    }
  }

  public void AddPoints(int _points) {
    this.points += _points;
    //UnityEngine.Debug.Log( "Adding " + _points + " points. Total: " + this.points );
    UpdateEvent();
  }

  void UpdateEvent() {
    PlayerInfo info = new PlayerInfo();
    info.Attempts = attempts;
    info.Points = points;
    PlayerUpdated(info);
  }

  bool firstShot = true;
  public void TryShotResult(ShotResult shotResult)
  {
    bool playingGoalkeeper = GameplayService.IsGoalkeeper();
    if(playingGoalkeeper)
    {
        Habilidades.EndRound(shotResult.Result == Result.Goal);
    }

    if(!GameplayService.networked)
    {
      if(!playingGoalkeeper)
      {
        if (shotResult.Result == Result.Goal || shotResult.Result == Result.Target)
        {
          if (shotResult.Perfect && attempts < maxAttempts)
          {
            GeneralSounds.instance.vidaExtra();
            kickEffects.instance.ExtraLife(shotResult.Point);
            this.attempts++;
            if(Habilidades.IsActiveSkill(Habilidades.Skills.VIP) && (attempts < maxAttempts))
            {
                this.attempts++;
                kickEffects.instance.ExtraLife(shotResult.Point + (Vector3.up * -1f));
            }
          }
          /*if (this.attempts >= maxAttempts && shotResult.Perfect)
          {
            if (perfects > 1 || Habilidades.IsActiveSkill(Habilidades.Skills.VIP))
            {
                if (this.attempts != 3)
                {
                  GeneralSounds.instance.vidaExtra();
                  kickEffects.instance.ExtraLife(shotResult.Point);
                  this.attempts = 3;
                }
            }
          }
          if(this.attempts < maxAttempts) this.attempts = maxAttempts;*/
        }
        else
        {
          ShotFailed();
        }
      }
      else
      {
          if (shotResult.Result == Result.Goal)
          {
              ShotFailed();
          }
          else
          {
              if (shotResult.Perfect && attempts < maxAttempts)
              {
                GeneralSounds.instance.vidaExtra();
                kickEffects.instance.ExtraLife(shotResult.Point);
                this.attempts++;
                if(Habilidades.IsActiveSkill(Habilidades.Skills.VIP) && (attempts < maxAttempts))
                {
                    this.attempts++;
                    kickEffects.instance.ExtraLife(shotResult.Point + (Vector3.up * -0.5f));
                }
              }
              //else if (this.attempts < maxAttempts && shotResult.Result != Result.OutOfBounds) this.attempts = maxAttempts;
          }
      }

      SumarDineroPorPrimas();

    }
    else if(GameplayService.networked)
    {
        bool isPlayer1 = GameplayService.initialGameMode != GameMode.Shooter;

        if(playingGoalkeeper) cntPastillaMultiplayer.marcadorRemoto.Lanzamientos++;
        else cntPastillaMultiplayer.marcadorLocal.Lanzamientos++;

        if(newState)
        {
          Debug.Log ("APPLYING");
          cntPastillaMultiplayer.marcadorLocal.SetEstado(GetSimpleState(serverState, isPlayer1));
          cntPastillaMultiplayer.marcadorRemoto.SetEstado(GetSimpleState(serverState, !isPlayer1));
          newState = false;
        }
        else
        {
            Debug.Log ("CALCULATING");
            MatchState tempstate = serverState;
            if((isPlayer1 == playingGoalkeeper)) tempstate.rounds++;
            if(playingGoalkeeper)
            {
                MatchStateSimple state =  cntPastillaMultiplayer.marcadorRemoto.AddResult(shotResult.Result == Result.Goal);
                if(isPlayer1)
                {
                    tempstate.marker_2 = state.marker;
                    tempstate.score_2 = state.score;
                }
                else
                {
                    tempstate.marker_1 = state.marker;
                    tempstate.score_1 = state.score;
                }
            }
            else
            {
                MatchStateSimple stateF = cntPastillaMultiplayer.marcadorLocal.AddResult(shotResult.Result == Result.Goal);
                if(isPlayer1)
                {
                    tempstate.marker_1 = stateF.marker;
                    tempstate.score_1 = stateF.score;
                }
                else
                {
                    tempstate.marker_2 = stateF.marker;
                    tempstate.score_2 = stateF.score;
                }
            }
            serverState = tempstate;

            if(!playingGoalkeeper)
            {
                Debug.Log ("SENDING");
                MsgSendState msg = Shark.instance.mensaje<MsgSendState>();
                msg.state = tempstate;
                msg.defense = MsgDefend.ToDefenseInfoNet(Vector3.zero, shotResult.DefenseResult);
                msg.send();
            }
            newState = false;
        }

        if(isPlayer1 == playingGoalkeeper)//fin de ronda
        {
          if(serverState.rounds > 4 && serverState.score_1 != serverState.score_2) //fin de partida
          {
            gameOver = true;

            bool winner = isPlayer1 ? (serverState.score_1 > serverState.score_2) : (serverState.score_2 > serverState.score_1);
            bool perfect = isPlayer1 ? (serverState.score_2 == 0) : (serverState.score_1 == 0);
            perfect = perfect && (isPlayer1 ? (serverState.score_1 >= 5) : (serverState.score_2 >= 5));

            if(winner)
            {
              Interfaz.MonedasSoft += recompensaMultijugadorWin;
            }
            else
            {
              Interfaz.MonedasSoft += recompensaMultijugadorFail;
            }

            PersistenciaManager.instance.GuardarPartidaMultiPlayer(winner, perfect);
          }
        }
    }

    int points = shotResult.ScorePoints + shotResult.EffectBonusPoints;
    // actualizar la informacion de cada ronda
    RoundInfoManager.instance.AcumularRonda(shotResult.Result, points, shotResult.Perfect);//, GameplayService.IsGoalkeeper() ? (shotResult.DefenseResult == GKResult.Perfect) : (shotResult.Perfect));
    
    // acumular la recompensa
    // DINERO! MONEDAS!
    int monedas = Mathf.FloorToInt((float) points * DifficultyService.GetRatioRecompensa());
    Interfaz.recompensaAcumulada += monedas;
    Interfaz.MonedasSoft += monedas;


    if(gameOver && !GameplayService.networked)
    {
        //ifcGameOver.instance.resultTime = FieldControl.instance.seconds;
        Mission mission = MissionManager.instance.GetMission();
        if(attempts > 0)
        {
            // persistir la mision que se acaba de superar
            PersistenciaManager.instance.ActualizarUltimoNivelDesbloqueado(mission.indexMision + 1);

            // comprobar si en la mision que se acaba de superar se desbloquea alguna equipacion
            Equipacion equipacionDesbloqueada = EquipacionManager.instance.GetEquipacionDesbloqueableEnFase(mission.indexMision);
            if (equipacionDesbloqueada != null) {
                // registrar el dialogo para mostrar el aviso de que se ha obtenido una nueva equipacion y actualizar su estado
                equipacionDesbloqueada.estado = Equipacion.Estado.DISPONIBLE;
                DialogManager.instance.RegistrarDialogo(new DialogDefinition(DialogDefinition.TipoDialogo.EQUIPACION_DESBLOQUEADA, equipacionDesbloqueada));
            }
            
            // comprobar si en la mision que se acaba de superar se ha desbloqueado algun escudo
            Debug.Log(">>> Compruebo escudo => fase ");
            Escudo escudoDesbloqueado = EscudosManager.instance.GetEscudoDesbloqueableEnFase(mission.indexMision);
            if (escudoDesbloqueado != null) {
                // registrar el dialogo para mostrar el aviso de que se ha obtenido un nuevo escudo y desbloquearlo
                escudoDesbloqueado.bloqueado = false;
                DialogManager.instance.RegistrarDialogo(new DialogDefinition(DialogDefinition.TipoDialogo.ESCUDO_DESBLOQUEADO, escudoDesbloqueado));
            }
        }
    }

    if(firstShot) firstShot = false;
    UpdateEvent();
  }

  /*Interfaz.ResultType enumConversion( Result _result)
  {
    switch(_result)
    {
      case Result.Goal:
        return Interfaz.ResultType.encaja;
        break;
      case Result.OutOfBounds:
        return Interfaz.ResultType.fuera;
        break;
      case Result.Saved:
        return Interfaz.ResultType.atrapa;
        break;
      case Result.Stopped:
        return Interfaz.ResultType.despeja;
        break;
      case Result.Target:
        return Interfaz.ResultType.target;
        break;
    }
    return Interfaz.ResultType.fuera;
  }*/

  ////public void TryBullEyeSuccess(BullseyeImpactInfo bullEyeImpactInfo) {
  ////  points += bullEyeImpactInfo.Points;
  ////UnityEngine.Debug.Log("points is:" + points);
  ////}

  public void Dispose() {
    ServiceLocator.Remove<IPlayerService>();
  }

  public void RegisterListener(Action<PlayerInfo> listener) {
    PlayerUpdated += listener;
  }

  public void UnregisterListener(Action<PlayerInfo> listener) {
    PlayerUpdated -= listener;
  }

  
  public static MatchStateSimple GetSimpleState(MatchState _state, bool _isPlayer1)
  {
    MatchStateSimple result = new MatchStateSimple();
    if(_isPlayer1)
    {
      result.score = _state.score_1;
      result.marker = _state.marker_1;
    }
    else
    {
      result.score = _state.score_2;
      result.marker = _state.marker_2;
    }
    result.rounds = _state.rounds;
    return result;
  }

    private int[] retosYaRecompensados;

    public int[] GetRecompensas()
    {
        if(retosYaRecompensados == null)
        {
            retosYaRecompensados = new int[4];
            for(int i = 0 ; i < 4 ; i++)
            {
                retosYaRecompensados[i] = 0;
            }
        }
        return retosYaRecompensados;
    }

    public void SumarDineroPorPrimas()
    {
        int total = 0;
        int[] recompensas = GetRecompensasRetos();

        if(retosYaRecompensados == null)
        {
            retosYaRecompensados = new int[recompensas.Length];
            for(int i = 0 ; i < recompensas.Length ; i++)
            {
                retosYaRecompensados[i] = 0;
            }
        }

        for(int i = 0 ; i < recompensas.Length ; i++)
        {
            if(retosYaRecompensados[i] == 0)
            {
                total += recompensas[i];
                retosYaRecompensados[i] = recompensas[i];
            }
        }
        Debug.Log("Dineros por retos de mision: " + recompensas[0] + " + " + recompensas[1] + " + " + recompensas[2] + " + " + recompensas[3] + " = " + total);
        Interfaz.recompensaAcumulada += total;
        Interfaz.MonedasSoft += total;
    }

    //primas
    //para sacar las recompensas monetarias que se han ganado
    //aqui lo dejo como un array porque ya se lo que va a ocurrir en el futuro :)
    public int[] GetRecompensasRetos()
    {
        int[] recompensas = new int[MissionManager.instance.GetMission().Achievements.Count];

        for(int i = 0 ; i < MissionManager.instance.GetMission().Achievements.Count ; i++)
        {
            bool retoEstaPartida = MissionManager.instance.GetMission().Achievements[i].IsAchieved();
            bool retoCargado = GameplayService.gameLevelMission.GetAchievements()[i].IsAchieved();
            int numNivel = GameplayService.gameLevelMission.Index;
            int valorNivel = 0;
            switch(i)
            {
                case 0: valorNivel = 100; valorNivel += 10 * numNivel; break;
                case 1: valorNivel = 200; valorNivel += 20 * numNivel; break;
                case 2: valorNivel = 300; valorNivel += 40 * numNivel; break;
                case 3: valorNivel = 400; valorNivel += 80 * numNivel; break;
            }
            recompensas[i] = (retoEstaPartida && !retoCargado) ? valorNivel : 0;
        }

        return recompensas;
    }
}
