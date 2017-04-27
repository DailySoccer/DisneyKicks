using UnityEngine;
using System.Collections.Generic;

public class kickEffects : MonoBehaviour {

  public GameObject hitPrefab;
  public GameObject trailPrefab;
  public GameObject pointsPrefab;
  //public GameObject defCirclePrefab;
  public GameObject crossHairPrefab;
  //public GameObject destelloPrefab;
  public GameObject cartelaPrefab;
  public GameObject targetHitPrefab;
  public GameObject ballEffectPrefab;

  public GameObject areaPrefab;
  public GameObject defensePrefab;
  public GameObject ballHitPrefab;

  public GameObject areaIntuicionPrefab;
  public GameObject destelloPowerupPrefab;
  public GameObject particulasSlipperyPrefab;
  public GameObject sharpshooterPrefab;
  public GameObject chronoPrefab;

  private GameObject fullCamEffect;

  public Material glovePowerupMaterial;
  public GameObject glovePowerupPrefabL;
  public GameObject glovePowerupPrefabR;
  public GameObject phasePowerupPrefab;
  public GameObject bulletTimePowerupBallPrefab;
  public GameObject bulletTimePowerupBonePrefab;


  public GameObject debugCirclePrefab;
  public GameObject debugTargetPrefab;

  //public Texture2D[] cartelas;
  //public Texture2D[] puntuaciones;
  //public Texture2D[] puntuacionesEfecto;
  public Texture2D[] powerupCartelas;

  public Texture2D cartelaPowerupPortero;
  public Texture2D cartelaPowerupLanzador;

  public Material dummy; //importante para que importe este shader

  public static kickEffects instance;

  private List<GameObject> DestroyOnResult;

  Vector3 tempPosition;
  bool areaSuccess = false;
  int tempPoints;

  void Awake() {
    instance = this;
    DestroyOnResult = new List<GameObject>();
  }

  void Start () {
    fullCamEffect = transform.Find("fullcam").gameObject;
    IShotResultService shotResultService = ServiceLocator.Request<IShotResultService>();
    shotResultService.RegisterListener(hitEffect);
    shotResultService.RegisterListener(stopBallTrail);    
    shotResultService.RegisterListener(MakeConfettis);

	//del merge catacrock, esta parte parece ser de Javi

    shotResultService.RegisterListener( ShotResultThrowerFeedback );
    shotResultService.RegisterListener( DrawPoints );
    shotResultService.RegisterListener( ShotResultGoalkeeperFeedback );
    
    ServiceLocator.Request<IPowerupService>().RegisterListener(PowerupCartela);

	//del merge catacrock, eventos de los powerups
    shotResultService.RegisterListener(realBallHit);
    shotResultService.RegisterListener(StopResultEffects);
    
    ServiceLocator.Request<IPowerupService>().RegisterListener(SlipperyParticles);
    ServiceLocator.Request<IPowerupService>().RegisterListener(SharpShooter);
    ServiceLocator.Request<IPowerupService>().RegisterListener(Chrono);
    ServiceLocator.Request<IPowerupService>().RegisterListener(Manoplas);
	//fin merge catacrock

    // en modo time_attack
    if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK) {
        if (GameplayService.initialGameMode == GameMode.Shooter)
            // si es tirador
            shotResultService.RegisterListener(DifficultyService.ModificarTiempoDeTimeAttack_Tirador);
        else
            // si es portero
            shotResultService.RegisterListener(DifficultyService.ModificarTiempoDeTimeAttack_Portero);
    }

    IBullseyeService bullseyeService = ServiceLocator.Request<IBullseyeService>();
    bullseyeService.RegisterListener( ShotResultBullseyeFeedback );
    IShotService shotService = ServiceLocator.Request<IShotService>();
    shotService.RegisterListener(startBallTrail);
    shotService.RegisterListener(BalonFase);
    shotService.RegisterListener(BulletTime);

    //shotService.RegisterListener(aracnoSense);
    IDefenseService defenseService = ServiceLocator.Request<IDefenseService>();
    defenseService.RegisterListener(defenseCircle);
  }


  public void DebugCircle(Vector3 _point, float _radius, Color _color)
  {
    GameObject aux = Instantiate(debugCirclePrefab, _point, Quaternion.identity) as GameObject;
    aux.GetComponent<Renderer>().material.SetColor("_TintColor", _color);
    aux.transform.localScale = Vector3.one * _radius;
  }

  public void DebugTarget(Vector3 _point)
  {
    Instantiate(debugTargetPrefab , _point, Quaternion.identity);
  }

  void PowerupCartela(PowerupUsage _info)
  {
    bool goalkeeper = _info.AbsId >= PowerupService.MAXPOWERUPSTIRADOR;
    float posPortero = 0.18f;
    float posLanzador = 0.8f;
    float posVPortero = 0.3f;
    float posVLanzador = 0.7f;

    // crear el texto de la cartela de powerup
    GameObject powerupGO = new GameObject();
    powerupGO.AddComponent<GUITexture>();
    powerupGO.layer = 14;
    powerupGO.transform.position = new Vector3(goalkeeper ? -1 : 2, !goalkeeper ? posVPortero : posVLanzador, 200);
    powerupGO.transform.localScale = new Vector3(0,0,1);
    powerupGO.GetComponent<GUITexture>().texture = goalkeeper ? cartelaPowerupPortero : cartelaPowerupLanzador;
    powerupGO.name = "cartelaPowerUp" + (goalkeeper ? "Portero" : "Lanzador") + "Texto";
    float ratio = 0.25f;//(float)(goalkeeper ? cartelaPowerupPortero : cartelaPowerupLanzador).height / (float)(goalkeeper ? cartelaPowerupPortero : cartelaPowerupLanzador).width;
    float hSize = (940 * ifcBase.scaleFactor / 2f);
    float escalaTexto = 1.3f;
    powerupGO.GetComponent<GUITexture>().pixelInset = new Rect(hSize * escalaTexto / -2, hSize * ratio * escalaTexto / -2, hSize * escalaTexto, hSize * ratio * escalaTexto); //new Rect(hSize / -2, hSize * ratio / -2, hSize, hSize * ratio);

    // crear el icono de la cartela de powerup
    hSize *= 0.5f; // escala para el icono
    GameObject iconGO = GameObject.Instantiate(powerupGO, powerupGO.transform.position, Quaternion.identity) as GameObject;
    float iconRatio = (float)powerupCartelas[_info.AbsId].height / (float)powerupCartelas[_info.AbsId].width;
    iconGO.GetComponent<GUITexture>().texture = powerupCartelas[_info.AbsId];
    iconGO.GetComponent<GUITexture>().pixelInset = new Rect(hSize / -2, hSize * iconRatio / -2, hSize, hSize * iconRatio);
    iconGO.name = "cartelaPowerUp" + (goalkeeper ? "Portero" : "Lanzador") + "Imagen";

    // ajustar la posicion del texto en funcion del tipo de powerup
    Rect newPixelInset = powerupGO.GetComponent<GUITexture>().pixelInset;
    newPixelInset.x += (goalkeeper ? 1 : -1) * (iconGO.GetComponent<GUITexture>().pixelInset.width + 160 * ifcBase.scaleFactor); // <= desplazar el texto a izda o dcha el ancho del icono de powerup
    powerupGO.GetComponent<GUITexture>().pixelInset = newPixelInset;

    //SuperTweener.InWaitOut(powerupGO, 0.5f, new Vector3(0, 0.5f, 2f), 0.5f, (GameObject _target3) => { Destroy(powerupGO);});


    new SuperTweener.move(powerupGO, 0.25f, new Vector3(goalkeeper ? posPortero : posLanzador, !goalkeeper ? posVPortero : posVLanzador, 300f), SuperTweener.CubicOut, (_target) =>
    {
        new SuperTweener.move(powerupGO, 0.5f, new Vector3(goalkeeper ? posPortero : posLanzador, !goalkeeper ? posVPortero : posVLanzador, 300f), null, (_target2) =>
        {
            new SuperTweener.move(powerupGO, 0.25f, new Vector3(!goalkeeper ? -1f : 2f, !goalkeeper ? posVPortero : posVLanzador, 300f), SuperTweener.CubicIn, (_target3) =>
            {
                Destroy(powerupGO);
            });
        });
    });

    new SuperTweener.move(iconGO, 0.25f, new Vector3(goalkeeper ? posPortero : posLanzador, !goalkeeper ? posVPortero : posVLanzador, 200f), SuperTweener.CubicOut, (_target) =>
    {
        new SuperTweener.move(iconGO, 0.5f, new Vector3(goalkeeper ? posPortero : posLanzador, !goalkeeper ? posVPortero : posVLanzador, 200f), null, (_target2) =>
        {
            new SuperTweener.move(iconGO, 0.25f, new Vector3(!goalkeeper ? -1f : 2f, !goalkeeper ? posVPortero : posVLanzador, 200f), SuperTweener.CubicIn, (_target3) =>
            {
                Destroy(iconGO);
            });
        });
    });

  }

    GameObject glove1;
    GameObject glove2;
    public Material lastMaterial = null;
    void Manoplas(PowerupUsage _info)
    {
        if(_info.Value != Powerup.Manoplas) return;
        Material[] materials = Goalkeeper.instance.transform.Find("Body").GetComponent<Renderer>().materials;
        lastMaterial = Goalkeeper.instance.transform.Find("Body").GetComponent<Renderer>().sharedMaterials[4];
        materials[4] = glovePowerupMaterial;
        Goalkeeper.instance.transform.Find("Body").GetComponent<Renderer>().materials = materials;
        Transform left = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand");
        Transform right = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand");
        glove1 = Instantiate(glovePowerupPrefabL, left.position, left.rotation) as GameObject;
        glove1.transform.parent = left;
        glove2 = Instantiate(glovePowerupPrefabR, right.position, right.rotation) as GameObject;
        glove2.transform.parent = right;
    }

    public void DoDoppelganger(GameObject _doppelganger)
    {
        EquipacionManager.instance.PintarEquipacionesIngame(true, _doppelganger);
        Material[] materiales = _doppelganger.transform.Find("Body").GetComponent<Renderer>().materials;
        foreach(Material m in materiales)
        {
            m.shader = Shader.Find("Particles/Alpha Blended Culled");//"Particles/Additive (Soft) (Culled)");
            m.SetColor("_TintColor", new Color(0.6f, 0.6f, 0.6f, 0.16f));
        }
        _doppelganger.transform.Find("Body").GetComponent<Renderer>().materials = materiales;
        materiales = _doppelganger.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Head").GetComponent<Renderer>().materials;
        foreach(Material m2 in materiales)
        {
            m2.shader = Shader.Find("Particles/Alpha Blended Culled");
            m2.SetColor("_TintColor", new Color(0.6f, 0.6f, 0.6f, 0.16f));
        }
        _doppelganger.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Head").GetComponent<Renderer>().materials = materiales;
        DestroyOnResult.Add(_doppelganger);
    }

    void BulletTime(ShotInfo _info)
    {
        if(!PowerupService.instance.IsPowerActive(Powerup.TiempoBala)) return;
        GameObject aux = Instantiate(bulletTimePowerupBallPrefab, BallPhysics.instance.transform.position, BallPhysics.instance.transform.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = BallPhysics.instance.transform;

        Transform bone = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/");
        aux = Instantiate(bulletTimePowerupBonePrefab, bone.position, bone.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = bone;

        bone = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 R Thigh/");
        aux = Instantiate(bulletTimePowerupBonePrefab, bone.position, bone.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = bone;

        bone = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 L Thigh/Bip01 L Calf/");
        aux = Instantiate(bulletTimePowerupBonePrefab, bone.position, bone.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = bone;

        bone = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/");
        aux = Instantiate(bulletTimePowerupBonePrefab, bone.position, bone.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = bone;

        bone = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/");
        aux = Instantiate(bulletTimePowerupBonePrefab, bone.position, bone.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = bone;

        bone = Goalkeeper.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/");
        aux = Instantiate(bulletTimePowerupBonePrefab, bone.position, bone.rotation) as GameObject;
        DestroyOnResult.Add(aux);
        aux.transform.parent = bone;
    }

  void hitEffect (ShotResult _result) {
    if(_result.Result == Result.OutOfBounds || _result.Result == Result.Goal || _result.Result == Result.Target)
    { 
      GameObject go = GameObject.Instantiate(hitPrefab, _result.Point, Quaternion.identity) as GameObject;
      if(_result.Result == Result.OutOfBounds) go.GetComponent<hit>().color = Color.red;
    }
  }


  void SharpShooter (PowerupUsage _info) {
    if(_info.Value != Powerup.Sharpshooter || GameplayService.IsGoalkeeper()) return;
    GameObject aux = Instantiate(sharpshooterPrefab) as GameObject;
    aux.transform.parent = Camera.main.transform;
    aux.transform.localPosition = new Vector3(0,0,1.5f);
    aux.transform.localRotation = Quaternion.identity;
  }

    void Chrono (PowerupUsage _info)
    {
        if(_info.Value != Powerup.Concentracion || GameplayService.IsGoalkeeper()) return;
        GameObject aux = Instantiate(chronoPrefab) as GameObject;
        aux.transform.parent = Camera.main.transform;
        aux.transform.localPosition = new Vector3(0,0,1.5f);
        aux.transform.localRotation = Quaternion.identity;
    }


  GameObject phaseParticles;
  public void BalonFase(ShotInfo _info)
  {
    if(!PowerupService.instance.IsPowerActive(Powerup.Phase)) return;
    BallPhysics.instance.gameObject.AddComponent<PhaseBall>();
    phaseParticles = Instantiate(phasePowerupPrefab, BallPhysics.instance.transform.position, BallPhysics.instance.transform.rotation) as GameObject;
    phaseParticles.transform.parent = BallPhysics.instance.transform;
  }

  public void targetHit (Vector3 _point)
  {
    GameObject.Instantiate(targetHitPrefab, _point, Quaternion.identity);
  }

  public void Focus(bool _mode)
    {
        fullCamEffect.GetComponent<GUITexture>().enabled = _mode;
    }

  GameObject greasyParticles;

  public void SlipperyParticles(PowerupUsage _info)
    {
        if(_info.Value != Powerup.Resbaladiza) return;
        if(greasyParticles != null) greasyParticles.GetComponent<MudBall>().Clear();
        greasyParticles = Instantiate(particulasSlipperyPrefab, BallPhysics.instance.transform.position, Quaternion.identity) as GameObject;
        greasyParticles.transform.parent = BallPhysics.instance.transform;
    }


    public void StopResultEffects(ShotResult _result)
    {
        if(phaseParticles != null) Destroy(phaseParticles);
        while(DestroyOnResult.Count > 0)
        {
            Destroy(DestroyOnResult[0]);
            DestroyOnResult.RemoveAt(0);
        }
        ServiceLocator.Request<IGameplayService>().ResetTime();
    }

    public void StopRoundEffects()
    {
        if(greasyParticles != null) greasyParticles.GetComponent<MudBall>().Clear();
        if(glove1 != null) Destroy(glove1);
        if(glove2 != null) Destroy(glove2);

        if(lastMaterial != null)
        {
            Material[] materials =  Goalkeeper.instance.transform.Find("Body").GetComponent<Renderer>().materials;
            materials[4] = lastMaterial;
            Goalkeeper.instance.transform.Find("Body").GetComponent<Renderer>().materials = materials;
            lastMaterial = null;
        }
    }

  public void Destello(float _time)
  {
      GameObject aux = GameObject.Instantiate(destelloPowerupPrefab, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
      aux.transform.parent = Camera.main.transform;
      aux.GetComponent<FadeWithTime>().Fade(_time);
  }

  public void ballEffect (Vector3 _position) {
    GameObject go = GameObject.Instantiate(ballEffectPrefab, _position, Quaternion.identity) as GameObject;
    go.GetComponent<hit>().color = Color.white;
  }

  /*public void destello ( Vector3 _position)
  {
    GameObject.Instantiate(destelloPrefab, _position, Quaternion.identity);
  }*/

  GameObject crossHair (Vector3 _position) {
    return GameObject.Instantiate(crossHairPrefab, _position, Quaternion.identity) as GameObject;
  }

  GameObject currentBallTrail;
  void startBallTrail (ShotInfo _info) {
    stopBallTrail(new ShotResult());
    GameObject ball = BallPhysics.instance.gameObject;
    currentBallTrail = GameObject.Instantiate(trailPrefab, ball.transform.position, Quaternion.identity) as GameObject;
    currentBallTrail.transform.parent = ball.transform;
    currentBallTrail.name = "trail";
//    currentBallTrail.transform.GetChild(0).GetComponent<ParticleSystem>().startRotation = -90f * Mathf.Deg2Rad * _info.Effect01;
  }

  void stopBallTrail (ShotResult _shot) {
    if(currentBallTrail != null) Destroy(currentBallTrail);
  }

  void MakeConfettis (ShotResult _shot)
  {
    /*GameObject go;
    //if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.Shooter) return;
    if(_shot.Result == Result.Stopped || _shot.Result == Result.Saved)
    {
      if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
        go = GameObject.Instantiate(paradaEffectPrefab[_shot.Precision], _shot.Point, Quaternion.identity) as GameObject;
    }*/
  }          

  public void AreaIntuition(Vector3 _position, float _time, float _size)
    {
        if(!PowerupService.instance.IsPowerActive(Powerup.Intuicion)) return;
        float halfSize = _size/2f;
        float angle = Random.Range (0f,360f);
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 offset = quat * new Vector3(Random.Range(0f, halfSize),0f,0f);
        GameObject area = Instantiate(areaIntuicionPrefab, _position + offset, Quaternion.identity) as GameObject;
        area.transform.localScale = Vector3.one * _size;
    }  

  public bool setPoste = false;
  public bool setLarguero = false;
  public bool setBarrera = false;
  public bool setSabana = false;

  public void aracnoSense(Vector3 _target, float _time, float _alpha) {
    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper)
    {
      crossHair cross = (crossHair(_target).GetComponent<crossHair>());
      cross.time = _time;
      cross.alpha = _alpha;
    }
  }
    
  Transform areaObject;
  public void ShowRect()
  {
    Rect area = ServiceLocator.Request<IDifficultyService>().GetRect();
    if(area.height > 0.1f)
    {
      if(!areaObject) areaObject = (Instantiate(areaPrefab) as GameObject).transform;
      areaObject.position = Porteria.instance.transform.position + new Vector3(area.center.x - 3.55f, area.center.y, 0);
      areaObject.localScale = new Vector3(area.width, area.height, 0.01f);
    }
    else
    {
      if(areaObject)
      {
        Destroy (areaObject.gameObject);
      }
    }
  }
    
  Vector3 defPoint;
  public void defenseCircle(DefenseInfo _info)
  {
    defPoint = _info.Target;
  }

  public Vector3 ballPoint;

  public void realBallHit(ShotResult _info)
  {
    if(ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper
       && _info.DefenseResult == GKResult.Fail)
    {
      GameObject.Instantiate(ballHitPrefab, ballPoint, Quaternion.identity);
      GameObject.Instantiate(defensePrefab, defPoint, Quaternion.identity);
    }
  }

  #region Shot Feedback management

  void ShotResultGoalkeeperFeedback (ShotResult _info) {

      if ( ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper ) {

          string feedbackText = "";

          switch ( _info.DefenseResult ) {
              case GKResult.Early: feedbackText = LocalizacionManager.instance.GetTexto(238).ToUpper(); break;
              case GKResult.Late: feedbackText = LocalizacionManager.instance.GetTexto(239).ToUpper(); break;
              case GKResult.Perfect: feedbackText = LocalizacionManager.instance.GetTexto(243).ToUpper(); break;
              case GKResult.Good:
                  switch ( _info.Precision ) {
                  case 1: feedbackText = LocalizacionManager.instance.GetTexto(241).ToUpper(); break;
                  case 2: feedbackText = LocalizacionManager.instance.GetTexto(240).ToUpper(); break;
                  case 3: feedbackText = LocalizacionManager.instance.GetTexto(242).ToUpper(); break;
                  }
                  break;
              case GKResult.Fail:feedbackText = LocalizacionManager.instance.GetTexto(244).ToUpper(); break;
              case GKResult.ThrowerFail:
                  if(setBarrera)
                  {
                    feedbackText = LocalizacionManager.instance.GetTexto(286).ToUpper(); break;
                  }
                  else if ( setLarguero )
                  {
                    feedbackText = LocalizacionManager.instance.GetTexto(252).ToUpper(); break;
                  }
                  else if ( setPoste )
                  {
                    feedbackText = LocalizacionManager.instance.GetTexto(251).ToUpper(); break;
                  }
                  else
                  {
                    feedbackText = LocalizacionManager.instance.GetTexto(245).ToUpper(); break;
                  }
          }

          if ( BallPhysics.instance.noCatch ) {
              feedbackText = LocalizacionManager.instance.GetTexto(246).ToUpper();
          }

          if ( _info.Result != Result.OutOfBounds ) {
              ShotFeedbackManager.Instance.SpawnShotReviewFeedback( feedbackText );
          }
          setBarrera = false;
      }
  }

  public GameObject ShotResultScoreFeedback (int _points, Vector3 _position) {      
      if ( GameplayService.networked ) {
          return null;
      }
      
      GameObject points = ShotFeedbackManager.Instance.SpawnShotFeedback( 
          "+" + _points.ToString(), _position, ShotFeedbackManager.ShotFeedbackTypes.Score );

      GeneralSounds.instance.puntos();

      return points;
  }

  void ShotResultBullseyeFeedback (BullseyeImpactInfo _info) {
      string feedbackString = "";
      switch ( _info.Ring ) {
          case 0: feedbackString = LocalizacionManager.instance.GetTexto(243).ToUpper(); break;
          case 1: feedbackString = LocalizacionManager.instance.GetTexto(241).ToUpper(); break;
          case 2: feedbackString = LocalizacionManager.instance.GetTexto(248).ToUpper(); break;
      }
      ShotFeedbackManager.Instance.SpawnShotReviewFeedback( feedbackString );
  }

  void DrawPoints (ShotResult _info) {
      if ( _info.ScorePoints > 0 ) {
          ShotResultScoreFeedback(
            ScoreManager.Instance.ApplyScoreMultiplierToScore( _info.ScorePoints ), _info.Point );
      }

      tempPoints = _info.EffectBonusPoints;
      tempPosition = _info.Point;

      Invoke( "DrawExtraPoints", 0.6f );
  }

  void DrawExtraPoints () {
      if ( tempPoints > 0 ) {
          MultiColoredString mcs = new MultiColoredString();

          switch ( tempPoints ) {
              case (int)ScoreManager.EffectBonus.LOW:
                  mcs.Append( new ColoredString( "#00ff00ff", "+" + tempPoints.ToString() ) );
                  break;
              case (int)ScoreManager.EffectBonus.MEDIUM:
                  mcs.Append( new ColoredString( "#00ffffff", "+" + tempPoints.ToString() ) );
                  break;
              case (int)ScoreManager.EffectBonus.HIGH:
                  mcs.Append( new ColoredString( "#ff00ffff", "+" + tempPoints.ToString() ) );
                  break;
          }

          mcs.Append( new ColoredString( "#ffffffff", " " + LocalizacionManager.instance.GetTexto(249).ToUpper() ) );
          ShotFeedbackManager.Instance.SpawnShotFeedback( 
              mcs, tempPosition, ShotFeedbackManager.ShotFeedbackTypes.EffectBonus );
      }
  }

  public void ExtraLife (Vector3 _position) {
      // mostrar la cartela de vida extra si no es modo time_attack
      if ( GameplayService.modoJuego.tipoModo != ModoJuego.TipoModo.TIME_ATTACK ) {
          ShotFeedbackManager.Instance.SpawnExtraLifeFeedback( 1, _position - ( Vector3.up * 0.5f ) );          
      }
  }

  public void DrawZone (bool _result, Vector3 _point) {
      tempPosition = _point;
      areaSuccess = _result;
      Invoke( "doDrawZone", _result ? 0.4f : 0f );
  }

  void doDrawZone () {
      ShotFeedbackManager.Instance.SpawnShotFeedback( 
          areaSuccess ? LocalizacionManager.instance.GetTexto(250).ToUpper() : LocalizacionManager.instance.GetTexto(244).ToUpper(), 
          tempPosition - Vector3.up * 0.25f,
          ShotFeedbackManager.ShotFeedbackTypes.YellowZone );
  }

  public void ShotResultThrowerFeedback (ShotResult _result) {
      if ( ServiceLocator.Request<IGameplayService>().GetGameMode() == GameMode.GoalKeeper ) return;

      if ( _result.Result == Result.OutOfBounds && !setPoste && !setLarguero ) {
          if ( ( ServiceLocator.Request<IDifficultyService>().GetRect().width < 0.1 ) ||
               ( _result.AreaResult == AreaResultValues.BallFailsArea ) ) {
               ShotFeedbackManager.Instance.SpawnShotReviewFeedback( LocalizacionManager.instance.GetTexto(245).ToUpper() );
          }
      }

        if ( _result.Result == Result.Stopped || _result.Result == Result.OutOfBounds) {
          if ( setPoste ) {
              ShotFeedbackManager.Instance.SpawnShotReviewFeedback( LocalizacionManager.instance.GetTexto(251).ToUpper());
          }
          else if ( setLarguero ) {
              ShotFeedbackManager.Instance.SpawnShotReviewFeedback( LocalizacionManager.instance.GetTexto(252).ToUpper() );
          }
          else if ( setBarrera ) {
              ShotFeedbackManager.Instance.SpawnShotReviewFeedback( LocalizacionManager.instance.GetTexto(286).ToUpper() );
          }
          else if(setSabana)
          {
              ShotFeedbackManager.Instance.SpawnShotReviewFeedback( LocalizacionManager.instance.GetTexto(291).ToUpper() );
          }
          else 
          {
              if (_result.Result == Result.Stopped && Goalkeeper.instance != null)
              {
                  ShotFeedbackManager.Instance.SpawnShotReviewFeedback(LocalizacionManager.instance.GetTexto(294).ToUpper());
              }
          }
      }

      if ( FieldControl.instance.goalKeeper ) {
          if ( ( _result.Result == Result.Goal ) || 
               ( _result.Result == Result.Target ) ) {
               ShotFeedbackManager.Instance.SpawnShotReviewFeedback( Random.Range( 0f, 1f ) > 0.5 ? LocalizacionManager.instance.GetTexto(241).ToUpper() : LocalizacionManager.instance.GetTexto(247).ToUpper() );
          }
      }

      setPoste = false;
      setLarguero = false;
      setBarrera = false;
      setSabana = false;
  }  

  #endregion
}
