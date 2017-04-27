using UnityEngine;

public class DifficultyArea : MonoBehaviour {

  public Difficulty difficulty = Difficulty.Medium;
  public bool goalKeeper = false;

  public Rect Area {
    get {
      Vector3 pos = transform.position;
      Vector3 scale = transform.localScale;
      return new Rect(
        pos.x - scale.x * .5f,
        pos.z - scale.z * .5f,
        scale.x,
        scale.z );
    }
  }

  void Start() {
    AreaManager.AddArea( this );
  }

  public Vector3 GetRandomPoint() {
    return new Vector3(
      Random.Range( Area.x, Area.x + Area.width ),
      0,
      Random.Range( Area.y, Area.y + Area.height ) );
  }

  public Vector3 GetRandomPointParamX( float _percent ) {
    //NOTA: solo sirve para el modo portero del kicks
    float x = 0f;
    if(_percent < 0) {
      _percent *= -1f;
      float x1 = Random.Range(Area.x, Area.x + Area.width * _percent / 2f);
      float x2 = Random.Range(Area.x + Area.width * (1f - (_percent/2f)), Area.x + Area.width);
      x = (Random.Range (0f,1f) > 0.5f) ? x1 : x2 ;
    }
    else {
        x = Random.Range(Area.center.x - (Area.width * _percent)/2f, Area.center.x + (Area.width * _percent)/2f);
    }
    return new Vector3(
      x,
      0,
      Random.Range( Area.y, Area.y + Area.height ) );
  }

  //// new Color( .435f, .847f, .917f, .6f );
  private static Color easyColor = new Color( 0, 1, 0, .4f );

  //// new Color( .890f, .662f, .305f, .6f );
  private static Color mediumColor = new Color( 0, 0, 1, .4f );

  //// new Color( .937f, .274f, .274f, .6f );
  private static Color hardColor = new Color( 1, 0, 0, .4f );

  void OnDrawGizmos() {
    var prevColor = Gizmos.color;

    switch (difficulty) {
      case Difficulty.Easy:
        Gizmos.color = DifficultyArea.easyColor;
        break;
      case Difficulty.Medium:
        Gizmos.color = DifficultyArea.mediumColor;
        break;
      case Difficulty.Hard:
        Gizmos.color = DifficultyArea.hardColor;
        break;
    }

    Gizmos.DrawLine( new Vector3( Area.xMin, 0, Area.yMin ), new Vector3( Area.xMax, 0, Area.yMin ) );
    Gizmos.DrawLine( new Vector3( Area.xMax, 0, Area.yMin ), new Vector3( Area.xMax, 0, Area.yMax ) );
    Gizmos.DrawLine( new Vector3( Area.xMax, 0, Area.yMax ), new Vector3( Area.xMin, 0, Area.yMax ) );
    Gizmos.DrawLine( new Vector3( Area.xMin, 0, Area.yMax ), new Vector3( Area.xMin, 0, Area.yMin ) );

    Gizmos.color = prevColor;
  }

  void OnDrawGizmosSelected() {
    var prevColor = Gizmos.color;

    switch (difficulty) {
      case Difficulty.Easy:
        Gizmos.color = DifficultyArea.easyColor;
        break;
      case Difficulty.Medium:
        Gizmos.color = DifficultyArea.mediumColor;
        break;
      case Difficulty.Hard:
        Gizmos.color = DifficultyArea.hardColor;
        break;
    }

    Gizmos.DrawCube( transform.position, new Vector3( transform.localScale.x, .1f, transform.localScale.z ) );

    Gizmos.color = prevColor;
  }

  void OnDestroy() {
    AreaManager.RemArea(this);
  }
}
