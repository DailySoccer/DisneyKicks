using UnityEngine;
using System.Collections;

public class GeneralSounds : MonoBehaviour {
  public static GeneralSounds instance;
  public AudioSource parallelAudio;
  public AudioClip perfectClip;
  public AudioClip perfectBullseyeClip;
  public AudioClip[] chutClip;
  public AudioClip errorClip;
  public AudioClip gameOverClip;
  public AudioClip[] cheerClip;
  public AudioClip[] booClip;
  public AudioClip[] reboundClips;
  public AudioClip[] miniHitClip;
  public AudioClip puntosClip;
  public AudioClip cortinillaInClip;
  public AudioClip cortinillaOutClip;
  public AudioClip bullseyeClip;
  public AudioClip inicioClip;
  public AudioClip warningClip;
  public AudioClip posteClip;
  public AudioClip vidaClip;
  public AudioClip redClip;
  public AudioClip botonClip;
  public AudioClip chronoBeatClip;
  public AudioClip yellowZoneClip;
  public AudioClip goalKeeperGoalClip;
  public AudioClip lonaGoalClip;
  public AudioClip normalGoalClip;
  public AudioClip barreraClip;
  public AudioClip outOfRangeClip;
  public AudioClip goalkeeperStoppedClip;
  public AudioClip goalkeeperSuccessClip;
  public AudioClip unlockClip;
  public AudioClip primaClip;
  public AudioClip powerupBarOnClip;
  public AudioClip powerupBarOffClip;
  public AudioClip usePowerupClip;
  public AudioClip pausaClip;
  public AudioClip continuarClip;
  public AudioClip victoriaClip;
  public AudioClip derrotaClip;


  SuperTweener.volume vol;
  float originalVolume = 0f;

  void Awake()
  {
    instance = this;
    originalVolume = GetComponent<AudioSource>().volume;
    GetComponent<AudioSource>().volume = 0f;
  }

  public void CleanAudioFade()
  {
    SuperTweener.Kill(vol);
  }

  void Start()
  {
    UnMuteVolume();
    ServiceLocator.Request<IShotService>().RegisterListener(chut);
    ServiceLocator.Request<IShotResultService>().RegisterListener(resultSound);
    Invoke ("inicio", 2f);
    //Invoke("cortinillaOut", 1f);
  }

  public void MuteVolume()
  {
    SuperTweener.Kill(vol);
    vol = new SuperTweener.volume(gameObject, 2f,  0.0f, SuperTweener.LinearNone);
  }

  void PlayOneShot(AudioClip _clip, float _volume = -1f)
  {
    if(_volume >= 0f) GetComponent<AudioSource>().PlayOneShot(_clip, _volume);
    else GetComponent<AudioSource>().PlayOneShot(_clip);
    //Debug.Log ("GPlay: " + _clip);
  }

  public void UnMuteVolume()
  {
    SuperTweener.Kill(vol);
    vol = new SuperTweener.volume(gameObject, 2f, ifcOpciones.fx ? originalVolume : 0.0f, SuperTweener.LinearNone);
  }

  public void click()
  {
    if(ifcOpciones.fx) PlayOneShot(botonClip);
  }

    public void pausa()
    {
        PlayOneShot(pausaClip);
    }

    public void victoria()
    {
        PlayOneShot(victoriaClip);
    }

    public void derrota()
    {
        PlayOneShot(derrotaClip);
    }

    public void continuar()
    {
        PlayOneShot(continuarClip);
    }

  public void usePowerup()
  {
    PlayOneShot(usePowerupClip);
  }

  public void inicio()
  {
    PlayOneShot(inicioClip);
  }

  public void powerupBarOn()
  {
    PlayOneShot(powerupBarOnClip);
  }

  public void powerupBarOff()
  {
    PlayOneShot(powerupBarOffClip);
  }

  public void redHit()
  {
    PlayOneShot(redClip);
  }

  public void prima()
  {
    PlayOneShot(primaClip);
  }

  public void vidaExtra()
  {
    PlayOneShot(vidaClip);
  }

  public void unlock()
  {
    PlayOneShot(unlockClip);
  }

  public void posteHit()
  {
    PlayOneShot(posteClip);
  }

  public void puntos ()
  {
    Invoke("puntos2", 0.10f);
  }

  void puntos2 ()
  {
    PlayOneShot(puntosClip);
  }

  public void cortinillaOut ()
  {
    PlayOneShot(cortinillaOutClip, 1f);
  }

  public void perfect()
  {
    PlayOneShot(perfectClip);
  }

  public void barrera()
  {
    PlayOneShot(barreraClip);
  }

  public void perfectBullseye()
  {
    PlayOneShot(perfectBullseyeClip);
  }


  public void chut(ShotInfo _info)
  {
    float distance = (BallPhysics.instance.transform.position - Porteria.instance.position).magnitude;
    if (distance > 35f)
      PlayOneShot(chutClip[2]);
    else if (distance > 25f)
      PlayOneShot(chutClip[1]);
    else
      PlayOneShot(chutClip[0]);
  }

  public void avisoDisparo()
  {
    PlayOneShot(warningClip);
  }

  public void gameOver()
  {
    PlayOneShot(gameOverClip);
  }

  public void rebound()
  {
    PlayOneShot(reboundClips[Random.Range (0,reboundClips.Length)]);
  }

  public void miniHit()
  {
    PlayOneShot(miniHitClip[Random.Range(0, miniHitClip.Length)]);
  }

  public void cheer()
  {
    PlayOneShot(cheerClip[Random.Range(0,cheerClip.Length)]);
  }

  public void boo()
  {
    PlayOneShot(booClip[Random.Range(0,booClip.Length)]);
  }

  public void bullseye()
  {
    PlayOneShot(bullseyeClip);
  }

  float time;
  public void chronobeat(float _proporcion)
  {
    if(!ifcOpciones.fx) return;
    if(_proporcion < 0) _proporcion = 1f;
    _proporcion *= _proporcion;
    Debug.Log (_proporcion);
    GetComponent<AudioSource>().volume = Mathf.Clamp(_proporcion * originalVolume, originalVolume * 0.1f, originalVolume);
    time -= Time.deltaTime;
    if(time <= 0f)
    {
      parallelAudio.PlayOneShot(chronoBeatClip, Mathf.Clamp(1f - _proporcion, 0.1f, 1f));
      time = 0.825f;
    }
  }

  public void resultSound(ShotResult _info)
  {
    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
    {
      if(_info.Result == Result.Saved)
      {
        cheer();
      }
      else if(_info.Result == Result.Goal)
      {
        boo();
        PlayOneShot(errorClip);
      }
      else if(_info.Result == Result.Stopped)
      {
        PlayOneShot(goalkeeperSuccessClip);
      }
    }
    else
    {
      if(_info.Result == Result.Goal || _info.Result == Result.Target )
      {
        cheer();
      }
      else if(_info.Result == Result.Saved || _info.Result == Result.Stopped || _info.Result == Result.OutOfBounds)
      {
        boo();
      }


        if(!GameplayService.networked && MissionManager.instance.HasCurrentMission())
        {
            ShooterMissionRound round = MissionManager.instance.GetMission().GetPrevRoundInfo() as ShooterMissionRound;

            if (round == null) {
                Debug.LogError(">>> ROUND ES NULL!!!");
                return;
            }

            if(round.HasYellowZone && (_info.Result == Result.Goal))
            {
                PlayOneShot(yellowZoneClip);
            }
            else if(round.HasGoalkeeper && (_info.Result == Result.Goal))
            {
                PlayOneShot(goalKeeperGoalClip);
            }
            else if(round.HasSheet && (_info.Result == Result.Goal))
            {
                PlayOneShot(lonaGoalClip);
            }
            else if(_info.Result == Result.Target)
            {
                if(_info.Perfect)
                {
                    perfectBullseye();
                }
                else
                {
                    bullseye();
                }
            }
            else if(_info.Result == Result.Goal)
            {
                PlayOneShot(normalGoalClip);
            }
            else if(_info.DefenseResult == GKResult.Good || _info.DefenseResult == GKResult.Perfect)
            {
                PlayOneShot(goalkeeperStoppedClip);
            }
        }
    }

    if(_info.Result == Result.OutOfBounds)
    {
        PlayOneShot(outOfRangeClip);
    }
  }

}
