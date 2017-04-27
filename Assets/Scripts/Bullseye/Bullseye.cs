using UnityEngine;
using System.Collections;

public class Bullseye : MonoBehaviour {

  [Range(0, 5.0f)]
  public float destructionTime = 2.0f;
  public Vector3 vel;

  Hexaedron ballHexaedron;
  public int[] Points;
  public int numberOfTrames;
  public bool staticSize = true;

  private float passedTime = 0f;
  const float sizeRatio = 0.5f;
  private Vector3 initSize;
  
  public float radiusOfBullseye
  {
    get{return this.gameObject.GetComponent<Collider>().bounds.extents.x;}
    set{}
  }
  float radiusForEachRing;


  public void Init(int[] Points) {
      SetPoints(Points);
      numberOfTrames = Points.Length;
      this.radiusOfBullseye = this.gameObject.GetComponent<Collider>().bounds.extents.x;
      this.radiusForEachRing = radiusOfBullseye / numberOfTrames;
      initSize = transform.localScale;
  }

  public void Update()
  {
    transform.position += (vel * Time.deltaTime);
    Rect porteria = Porteria.instance.GetRect();
    if(transform.position.y < porteria.yMin) vel.y *= -1f;
    else if(transform.position.y + radiusOfBullseye > porteria.yMax) vel.y *= -1f;
    if(transform.position.x - radiusOfBullseye < porteria.xMin) vel.x *= -1f;
    else if(transform.position.x + radiusOfBullseye > porteria.xMax) vel.x *= -1f;
    if(!staticSize)
    {
      passedTime += Time.deltaTime;
      transform.localScale = new Vector3(initSize.x + Mathf.Sin(passedTime) * sizeRatio, 1f, initSize.z + Mathf.Sin(passedTime) * sizeRatio);
      this.radiusOfBullseye = this.gameObject.GetComponent<Collider>().bounds.extents.x;
      this.radiusForEachRing = radiusOfBullseye / numberOfTrames;
      
    }
        if(GoalCamera.instance.stateMachine.current != ThrowerCameraStates.Init.instance && GoalCamera.instance.stateMachine.current != ThrowerCameraStates.Sharpshooter.instance)
        {
            transform.GetChild(1).GetComponent<Renderer>().material.SetColor( "_TintColor", Color.clear );
        }
  }

  void DoEvent()
  {
    if(bullseyeImpactInfo.Points == 0) return;
    ServiceLocator.Request<IBullseyeService>().OnBullseyeImpacted(bullseyeImpactInfo);
    // Disable collider to prevent further collisions
    gameObject.GetComponent<Collider>().enabled = false;
    StartCoroutine( FadeOut() );
  }

  BullseyeImpactInfo bullseyeImpactInfo;

  public BullseyeImpactInfo CheckThrow (ShotInfo _shot) {

    bullseyeImpactInfo.Points = 0;
    if ( Vector3.Distance( _shot.Target, transform.position ) <= radiusOfBullseye ) {
      bullseyeImpactInfo = new BullseyeImpactInfo();

      bullseyeImpactInfo.Distance = this.CalculateDistance(_shot.Target);
      bullseyeImpactInfo.Position = _shot.Target;
      bullseyeImpactInfo.Points = CalculateScore(bullseyeImpactInfo);
      bullseyeImpactInfo.Ring = calculateRingOfImpact(bullseyeImpactInfo);
    }

    DoEvent();
    return bullseyeImpactInfo;
  }

  /*void OnTriggerEnter (Collider other){
    // TODO : Detect somehow shots coming from behind

    this.ballHexaedron = other.GetComponent<BallPhysics>().ballHexaedron;
    BullseyeImpactInfo bullseyeImpactInfo = new BullseyeImpactInfo();
    bullseyeImpactInfo.Distance = this.CalculateDistance(other.gameObject);
    bullseyeImpactInfo.Position = this.transform.position;
    bullseyeImpactInfo.Points = CalculateScore(bullseyeImpactInfo);
    bullseyeImpactInfo.Ring = calculateRingOfImpact(bullseyeImpactInfo);
    ServiceLocator.Request<IBullseyeService>().OnBullseyeImpacted(bullseyeImpactInfo);

    // Disable collider to prevent further collisions
    gameObject.collider.enabled = false;
    GeneralSounds.instance.bullseye();

    StartCoroutine( FadeOut() );
  }*/

  IEnumerator FadeOut() {
    Color startColor = transform.GetChild(0).GetComponent<Renderer>().material.GetColor( "_TintColor" );
    float alpha = startColor.a;
    float time = 0;
    float ratio = 1;

    while (ratio > 0) {
      transform.GetChild(0).GetComponent<Renderer>().material.SetColor( "_TintColor", new Color( startColor.r, startColor.g, startColor.b, alpha * ratio ) );
      transform.GetChild(1).GetComponent<Renderer>().material.SetColor( "_TintColor", new Color( startColor.r, startColor.g, startColor.b, 0 ) );
  
      time += Time.deltaTime;
      ratio = 1 - (time / destructionTime);
  
      yield return new WaitForEndOfFrame();
    }

    GameObject.Destroy( this );
  }

  float CalculateDistance(Vector3 point){
    return Vector3.Distance(this.transform.position, point);// -  ballHexaedron.GetRadius();
  }

  public void SetPoints(int[] Points){
    this.Points = Points;
  }

  public int CalculateScore(BullseyeImpactInfo bullseyeImpactInfo){
    int ringOfImpact = calculateRingOfImpact(bullseyeImpactInfo);
    return Points[ringOfImpact];
  }

  public int calculateRingOfImpact(BullseyeImpactInfo bullseyeImpactInfo){
    int result = (int)(bullseyeImpactInfo.Distance / this.radiusForEachRing);
    return result < numberOfTrames ? result : numberOfTrames - 1;
  }

}
