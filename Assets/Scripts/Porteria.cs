using UnityEngine;
using System.Collections;

public class Porteria : MonoBehaviour {

  public Material m_MaterialTiro;
  public Material m_MaterialParada;

  public static Porteria instance { get; private set; }
  Transform shape;

  public Vector3 position
  {
    get{ return transform.position; }
    set{ transform.position = value; }
  }

  void Awake()
  {
    instance = this;
    shape = transform.Find ("GoalCollider").transform;
  }

  public float HalfHorizontalSize {get{return (shape.localScale.x/2);} set{}}
  public float VerticalSize {get{return (shape.localScale.y);} set{}}

  public Vector3 GetRandomPoint(float xMin = 0f, float xMax = 1f) {
      Vector3 point = Vector3.zero;
      Vector3 ballPos = transform.position;
      point.z = -49.5f;

      /*if(Random.Range(0f,1f) < 0.5f) Random.Range(0.3f, ballPos.x + (shape.localScale.x/2) * 0.90f);
      else point.x = Random.Range(ballPos.x - (shape.localScale.x/2) * 0.90f, -0.3f);*/

      point.x = Random.Range(xMin * (shape.localScale.x - 0.25f), xMax * (shape.localScale.x - 0.25f) );
      point.x *= (Random.Range(0,1f) > 0.5) ? 1f : -1f;

      point.y = Random.Range(ballPos.y, ballPos.y + (shape.localScale.y - 0.1f));

      return point;
  }

  public Rect GetRect()
  {
    Vector3 ballPos = transform.position;
    Rect res = new Rect();
    res.xMin = ballPos.x - (shape.localScale.x/2);
    res.xMax = ballPos.x + (shape.localScale.x/2);
    res.yMin = ballPos.y;
    res.yMax = ballPos.y + shape.localScale.y;
    return res;
  }

  public void SetKeeperMaterial(bool _keeper)
  {
    Renderer ren = transform.Find("Modelo").GetComponent<Renderer>();
    Material[] mats = ren.materials;
    if (_keeper) mats[1] = m_MaterialParada;
    else mats[1] = m_MaterialTiro;

    ren.materials = mats;
  }

}
