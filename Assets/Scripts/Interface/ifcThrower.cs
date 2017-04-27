using UnityEngine;
using System.Collections;

public class ifcThrower : ifcBase {
    public static ifcThrower instance { get; protected set; }

    int m_rondas = 0;

    public Texture m_balonOff;
    public Texture m_balonOn;
    public Texture[] m_niveles;
    // textura para mostrar en la pantalla de ayuda en el boton "atras"

    bool m_recordNotified = false;


    void Awake()
    {
      instance = this;
      ifcBase.activeIface = this;
    }

    public void SetPauseEnabled(bool _mode)
    {
      getComponentByName("btnPausa").SetActive(_mode);
    }

/*
#if !UNITY_EDITOR
    void OnApplicationFocus(bool focusStatus) {
      if(!focusStatus && !GameplayService.networked && ifcPausa.instance.PauseEnabled) DoPause();
    }
#endif
*/

    void DoPause()
    {
        GeneralSounds.instance.click();
        GeneralSounds.instance.MuteVolume();

        GeneralSounds.instance.pausa();

        ifcPausa.instance.SetVisible(true);
        ifcPausa.instance.RefreshObjetivos();
        new SuperTweener.move(ifcPausa.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut);
        new SuperTweener.move(getComponentByName("btnPausa"), 0.25f, new Vector3(-1.0f, 1.0f, 0.0f), SuperTweener.CubicOut);
        Time.timeScale = 0.0f;
        ifcBase.activeIface = ifcPausa.instance;
        InputManager.instance.Blocked = true;
    }

    void Update()
    {
        if(Input.GetKeyUp("escape"))
        {
            if(ifcBase.activeIface == this && ifcPausa.instance.PauseEnabled)
            {
                if(ifcPausa.instance.PauseEnabled) DoPause();
            }
            else if(ifcBase.activeIface != this )
            {
                ifcBase.activeIface.m_backMethod("");
            }
        }
#if UNITY_EDITOR
        if(Input.GetKeyDown("1")) PowerupService.instance.UsePowerup((Powerup)0);
        if(Input.GetKeyDown("2")) PowerupService.instance.UsePowerup((Powerup)1);
        if(Input.GetKeyDown("3")) PowerupService.instance.UsePowerup((Powerup)2);
        if(Input.GetKeyDown("4")) PowerupService.instance.UsePowerup((Powerup)3);
        if(Input.GetKeyDown("5")) PowerupService.instance.UsePowerup((Powerup)4);
        if(Input.GetKeyDown("6")) PowerupService.instance.UsePowerup((Powerup)5);
        if(Input.GetKeyDown("7")) PowerupService.instance.UsePowerup((Powerup)6);
        if(Input.GetKeyDown("8")) PowerupService.instance.UsePowerup((Powerup)7);
        if(Input.GetKeyDown("9")) PowerupService.instance.UsePowerup((Powerup)8);
#endif

        // comprobar si el balon esta en estado de espera
        if (BallPhysics.instance.state == BallPhysics.BallState.Waiting &&
            (GameplayService.IsGoalkeeper() || (GoalCamera.instance.stateMachine.current == ThrowerCameraStates.Init.instance)) &&
            (!InputManager.instance.m_initializedGesture)) {
            // comprobar si la pastilla de powerups debe estar visible
            if (!cntPastillaPowerups.instance.estaVisible) {
                if ((GameplayService.IsGoalkeeper() && !PowerupService.instance.usedGoalkeeperPowerup) || (!GameplayService.IsGoalkeeper() && !PowerupService.instance.usedShooterPowerup))
                {
                    cntPastillaPowerups.instance.Show();
                }
            }
        } else {
            // asegurarse de que la pastilla de powerups esta oculta
            if (cntPastillaPowerups.instance.estaVisible)
            {
                cntPastillaPowerups.instance.Hide();
            }
        }

        // si la barra de powerups no esta visible 
        

    }

    public void UpdateHabilidades()
    {
        transform.FindChild("txtHabilidades").GetComponent<GUIText>().text = "HABILIDADES: " + Habilidades.GetAllHabilidadesTexto();
    }

// Use this for initialization
    void Start () {
      ifcBase.Scale(this.gameObject);

      getComponentByName("btnPausa").GetComponent<btnButton>().action = (_name) => {
        DoPause ();
      };

      if(ifcBase.activeIface != ifcAyudaInGame.instance) 
          ifcBase.activeIface = this;

      // comprobar si hay que mostrar la interfaz modo un jugador o la de multijugador
      if (GameplayService.networked) {
          // modo multijugador
          ifcPausa.instance.PauseEnabled = false;
          GameObject.Find ("pastillaPrimas").SetActive(false);
          transform.FindChild("pastilla-disparos").gameObject.SetActive(false);
          transform.FindChild("pastillaMultiplayerDcha").gameObject.SetActive(true);
          transform.FindChild("pastillaMultiplayerIzda").gameObject.SetActive(true);
          cntPastillaMultiplayer.marcadorLocal.Inicializar(Interfaz.m_uname);
          cntPastillaMultiplayer.marcadorRemoto.Inicializar(ifcDuelo.m_rival.alias);
      } else {
          // modo un jugador
          transform.FindChild("pastilla-disparos").gameObject.SetActive(true);
          transform.FindChild("pastillaMultiplayerDcha").gameObject.SetActive(false);
          transform.FindChild("pastillaMultiplayerIzda").gameObject.SetActive(false);

          // comprobar si el modo de juego es modo time_attack => inicializar la pastilla de disparos
          if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK) {
              // ocultar los balones de la pastilla multijugador
              for (int i = 1; i <= 3; ++i)
                  transform.FindChild("pastilla-disparos/imgBalon" + i).gameObject.SetActive(false);

              // inicializar el cronometro de juego y arrancarlo
              cntCronoTimeAttack.instance.Inicializar(Stats.TIME_ATTACK_TIEMPO_BASE,
                  // ACCCIONES A REALIZAR CUANDO SE AGOTA EL TIEMPO DEL CRONOMETRO EN EL MODO TIME_ATTACK
                  (_name) => {
                      // dejar al player en estado de GameOver
                      FieldControl.instance.GameOver();

                      // guardar el valor del tiempo de juego para mostrarlo en la pantalla de game over
                      //ifcGameOver.instance.resultTime = FieldControl.instance.seconds;

                      if (GameplayService.initialGameMode == GameMode.Shooter) {
                          // si juego como tirador => simular un tiro fallido para que se resuelva el fin de la partida
                          Thrower.instance.resultAnimations( new ShotResult() { Result = Result.OutOfBounds, Point = Vector3.zero, Rebounded = false, ScorePoints = 0 } );
                      } else {
                          // si juego como portero => simular un gol para que se resuelva el fin de la partida
                          Goalkeeper.instance.Celebrate( new ShotResult() { Result = Result.Goal, Point = Vector3.zero, Rebounded = false, ScorePoints = 0 } );
                      }

                      // mostrar cartela de "FIN DEL TIEMPO"
                      ShotFeedbackManager.Instance.SpawnShotReviewFeedback( "¡FIN DEL TIEMPO!" );
                  });
              // NOTA: el crono ya no se activa aqui => si no al cerrar el popup
              //cntCronoTimeAttack.instance.SetActivo(true);
          } else
              // ocultar el cronometro del time_attack
              cntCronoTimeAttack.instance.SetVisible(false);
      }

      m_showingPhase = false;
      ServiceLocator.Request<IPlayerService>().RegisterListener(UpdatePlayerData);
      ServiceLocator.Request<IDifficultyService>().RegisterListener(setFase);
      ServiceLocator.Request<IShotResultService>().RegisterListener( shotEnded );
      ServiceLocator.Request<IApplicationEvents>().RegisterListener( ShowGameOver );
      setLifes(3);

      //setMultiplicador(1);

      GameObject.Find("txtRecordPuntos").GetComponent<GUIText>().text = (GameplayService.initialGameMode == GameMode.Shooter) ? Player.record_thrower.ToString() : Player.record_keeper.ToString();

      // mostrar en el escudo la textura y el multiplicador de puntuacion correspondiente
      transform.FindChild("pastilla_nivel_multiplicador/btnNivel/IconoNo").GetComponent<GUITexture>().texture = AvataresManager.instance.GetTexturaEscudo(EscudosManager.escudoEquipado.idTextura);
      transform.FindChild("pastilla_nivel_multiplicador/btnNivel/txtNivel").GetComponent<GUIText>().text = EscudosManager.escudoEquipado.boost.ToString("f1").ToString();

      if((GameplayService.initialGameMode == GameMode.Shooter ? Player.record_thrower : Player.record_keeper) != -1) ShowRecord();

      // ocultar las pantalla de "gameover" y de pausa
      ifcGameOverSingle.instance.SetVisible(false);
      ifcGameOverMulti.instance.SetVisible(false);
      ifcPausa.instance.SetVisible(false);
    }


    void UpdatePlayerData(PlayerInfo _info) {
      setPoints(_info.Points);
      setLifes(_info.Attempts);
    }

    void shotEnded(ShotResult _info)
    {
      m_rondas ++;
      if (!GameplayService.networked) {
          GameObject.Find("pastilla-disparos/ronda/texto").GetComponent<GUIText>().text = m_rondas.ToString()+"/"+MissionManager.instance.GetMission().RoundsCount;
          GameObject.Find("pastilla-disparos/ronda/textoSombra").GetComponent<GUIText>().text = GameObject.Find("pastilla-disparos/ronda/texto").GetComponent<GUIText>().text;
      }
    }

    void ShowGameOver(AppEvent _event) {
        Debug.Log(">>> GAME OVER");

        // ocultar los elementos de la interfaz
        transform.FindChild("pastilla-disparos").gameObject.SetActive(false);
        transform.FindChild("pastillaPrimas").gameObject.SetActive(false);
        transform.FindChild("pastilla_nivel_multiplicador").gameObject.SetActive(false);
        transform.FindChild("pastillaMultiplayerDcha").gameObject.SetActive(false);
        transform.FindChild("pastillaMultiplayerIzda").gameObject.SetActive(false);

        if (_event != AppEvent.GameOverEnqueued)
            return;

        // llamar a la interfaz de gameover que corresponda
        if (GameplayService.networked) {
            // mostrar pantalla gameover de multi
            ShowMultiGameOver();
        } else {
            // mostrar las alrertas pendientes y la pantalla de gameover de single
            DialogManager.instance.MostrarDialogosRegistradosPendientes( 
                // callback para cuando se muestren todos los dialogos
                () => { ShowSingleGameOver(); });
        }

    }

    void ShowMultiGameOver() {

        ifcGameOverMulti.instance.RefreshData();

        ifcGameOverMulti.instance.SetVisible(true);
        new SuperTweener.move( ifcGameOverMulti.instance.gameObject, 0.25f, new Vector3( 0.5f, 0.5f, 0.0f ), SuperTweener.CubicOut );

        SetVisibleNivelMultiplicador( false );
    }

    void ShowSingleGameOver() {

        ifcGameOverSingle.instance.RefreshData();

        ifcGameOverSingle.instance.SetVisible(true);
        new SuperTweener.move(ifcGameOverSingle.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut);

        PersistenciaManager.instance.GuardarPartidaSinglePlayer();
    }

    void ShowRecord()
    {
        // si no estoy en modo multijugador => mostrar las alertas de record
        if (!GameplayService.networked) {
            new SuperTweener.moveLocal(getComponentByName("pastilla_record"), 0.5f, new Vector3(1.0f, 0.78f, 0.0f), SuperTweener.CubicOut,
              (GameObject _target) => {
                  new SuperTweener.none(gameObject, 3f, (_target2) => {
                      new SuperTweener.moveLocal(getComponentByName("pastilla_record"), 0.5f, new Vector3(1.3f, 0.78f, 0.0f), SuperTweener.CubicIn);
                  });
              });
            //SuperTweener.InWaitOut(getComponentByName("pastilla_record"), 0.33f, new Vector3(1f, 0.78f, 0), 5f);
        }
    }

    public void setLifes(int _lifes) {
      if(!GameplayService.networked && (GameplayService.modoJuego.tipoModo !=  ModoJuego.TipoModo.TIME_ATTACK))
      {
        GameObject.Find("imgBalon1").GetComponent<GUITexture>().texture = _lifes >= 1 ? m_balonOn : m_balonOff;
        GameObject.Find("imgBalon2").GetComponent<GUITexture>().texture = _lifes >= 2 ? m_balonOn : m_balonOff;
        GameObject.Find("imgBalon3").GetComponent<GUITexture>().texture = _lifes >= 3 ? m_balonOn : m_balonOff;
      }
    }

    public void setPoints(int _points) {
      int record = GameplayService.initialGameMode == GameMode.Shooter ? Player.record_thrower : Player.record_keeper;
      if(record != -1 && !m_recordNotified && _points > record)
      {
        m_recordNotified = true;
        ShowRecord();
      }
      if(record != -1 && _points > record)
      {
        if(GameplayService.initialGameMode == GameMode.Shooter) Player.record_thrower = _points;
        else Player.record_keeper = _points;
      }
      if (!GameplayService.networked) {
          GameObject.Find("pastilla-disparos/puntuacion/txtPuntos").GetComponent<GUIText>().text = _points.ToString();
          GameObject.Find("pastilla-disparos/puntuacion/txtPuntosSombra").GetComponent<GUIText>().text = _points.ToString();
          GameObject.Find("txtRecordPuntos").GetComponent<GUIText>().text = _points.ToString();
      }
    }

    bool m_showingPhase = false;
    public void setFase()
    {
      setFase(0);
    }

    public void setFase(ShotConfiguration _info)
    {
        if (_info.IsNewFase)
        {
          setFase(_info.Fase);
        }
        //setMultiplicador(ServiceLocator.Request<IDifficultyService>().GetMultiplier());
    }

    public void setFase(int _fase) {
        if (!m_showingPhase && !GameplayService.networked && DifficultyService.FaseToName(_fase) != "") {
            // si la ayuda esta oculta => asegurarse de que se muestra la pastilla de multiplicador
            if (ifcBase.activeIface != ifcAyudaInGame.instance)
                transform.FindChild("pastilla_nivel_multiplicador").gameObject.SetActive(true);

            m_showingPhase = true;
        }
        //setMultiplicador(ServiceLocator.Request<IDifficultyService>().GetMultiplier());
    }


    /// <summary>
    /// Muestra / oculta parte de la interfaz de esta pantalla para que no se solape con otras al utilizar el supertweener
    /// </summary>
    public void SetVisibleNivelMultiplicador(bool _visible) {
        GameObject go = transform.FindChild("pastilla_nivel_multiplicador").gameObject;
        if (go != null)
            go.SetActive(_visible);
    }

            /*
            // si es un gol perfecto
            if (_info.Perfect) {
                
            }
                // si un gol en una diana
            else if (_info.Result == Result.Target) {
                Debug.LogWarning(">>> GOL EN DIANA +8");
                cntCronoTimeAttack.instance.AddTiempo(8.0f);
            }
                // si un tiro termina en zona segura
            else if (!_info.AreaFail) {
                Debug.LogWarning(">>> TIRO EN ZONA SEGURA +6");
                cntCronoTimeAttack.instance.AddTiempo(6.0f);
            }
                // si es un tiro normal
            else if (_info.Points > 0) {
                // si es un gol normal con portero
                
                } else {
                    // gol normal
                    Debug.LogWarning(">>> GOL NORMAL +4");
                    cntCronoTimeAttack.instance.AddTiempo(4.0f);
                }
            } else {
                // no hay puntos => es un fallo
                Debug.LogWarning(">>> FALLO -4");
                cntCronoTimeAttack.instance.AddTiempo(-4.0f);
            }
        }
             * */
}
