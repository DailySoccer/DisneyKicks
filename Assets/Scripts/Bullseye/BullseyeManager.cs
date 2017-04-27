using UnityEngine;
using System.Collections;
using System;

public enum SizeOfBullseye {
  S,
  M,
  L
}

public enum HeightOfBullseye {
  Baja,
  Media,
  Alta
}

public enum ZoneOfBullseye {
  Centro,
  Lados
}

public class BullseyeManager : MonoBehaviour {

  public GameObject BullseyePrefab;
  public GameObject CameraReference;
  private static BullseyeManager instance;

  public static BullseyeManager Instance { get { return instance; } }

  public SizeOfBullseye[] defSize = {SizeOfBullseye.L, SizeOfBullseye.M};
  public ZoneOfBullseye defZone = ZoneOfBullseye.Centro;
  public HeightOfBullseye[] defHeight = {HeightOfBullseye.Baja};
  public bool staticSize = true;
  public Vector3 defInitialSpeed = Vector3.zero;

  private float BullseyeSmallSize = 0.75f;
  private float BullseyeMediumSize = 1f;
  private float BullseyeLargeSize = 1.5f;

  void Start() {
    if (instance == null) {
      BullseyeManager.instance = this;
    } else if (instance != this) {
      throw new ApplicationException( "No more than one BullseyeManager allowed in the scene!" );
    }
  }

  public GameObject CreateBullseye() {
    Vector3 position = Vector3.zero;

    GameObject bullseye = (GameObject)GameObject.Instantiate( BullseyePrefab, position, Quaternion.identity );

    //tamaño
    SizeOfBullseye size = defSize[Mathf.FloorToInt(UnityEngine.Random.Range(0f,1f) * defSize.Length)];
    if(Habilidades.IsActiveSkill(Habilidades.Skills.Vista_halcon))
    {
      if(size == SizeOfBullseye.S) size = SizeOfBullseye.M;
      else if(size == SizeOfBullseye.M) size = SizeOfBullseye.L;
    }
    SetSize( bullseye, size );
    float radius = bullseye.GetComponent<Bullseye>().radiusOfBullseye;

    //altura
    int maxheight = 0;
    int minheight = 10;
    foreach(HeightOfBullseye h in defHeight)
    {
      int heightInt = (int)h;
      if(maxheight < heightInt) maxheight = heightInt;
      if(minheight > heightInt) minheight = heightInt;
    }
    position.y = UnityEngine.Random.Range(minheight * (2.25f+radius)/3f, (maxheight + 1) * (2.25f-radius)/3f);

    //posicion horizontal
    if(defZone == ZoneOfBullseye.Centro) 
        position.x = UnityEngine.Random.Range(-1.5f, 1.5f);
    else 
        position.x = UnityEngine.Random.Range(1.5f, 3.4f-radius) * ((UnityEngine.Random.Range(0f,1f) > 0.5f) ? 1f : -1f);
    position.z = Porteria.instance.transform.position.z;
    bullseye.transform.position = position;

    ////bullseye.transform.LookAt( this.CameraReference.transform.position );
    bullseye.transform.Rotate( new Vector3( 90, 0, 0 ) );

    // velocidad
    bullseye.transform.GetComponent<Bullseye>().vel = new Vector3(
          defInitialSpeed.x * (UnityEngine.Random.Range(0, 2) == 1 ? -1 : 1),
          defInitialSpeed.y * (UnityEngine.Random.Range(0, 2) == 1 ? -1 : 1),
          defInitialSpeed.z);

    bullseye.transform.GetComponent<Bullseye>().staticSize = staticSize;

    bullseye.GetComponent<Bullseye>().Init( new int[] { 
        (int)ScoreManager.BullsEyeScore.Red, 
        (int)ScoreManager.BullsEyeScore.Yellow, 
        (int)ScoreManager.BullsEyeScore.Blue, 
        (int)ScoreManager.BullsEyeScore.White 
    } );

    return bullseye;
  }

  public void SetSize(GameObject bullseye, SizeOfBullseye size) {
    switch (size) {
      case SizeOfBullseye.S:
        bullseye.transform.localScale = new Vector3(this.BullseyeSmallSize, 1f, this.BullseyeSmallSize);
        break;

      case SizeOfBullseye.M:
         bullseye.transform.localScale = new Vector3(this.BullseyeMediumSize, 1f, this.BullseyeMediumSize);
        break;

      case SizeOfBullseye.L:
         bullseye.transform.localScale = new Vector3(this.BullseyeLargeSize, 1f, this.BullseyeLargeSize);
        break;
    }
  }

}
