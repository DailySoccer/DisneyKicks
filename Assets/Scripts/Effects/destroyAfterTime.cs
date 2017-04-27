using UnityEngine;
using System.Collections;

public class destroyAfterTime : MonoBehaviour {
  public float time = 1f;
  public Renderer alpha;

  void Update ()
  {
    time -=  (Time.timeScale == 0f ? 0f: Time.deltaTime / Time.timeScale);
    if(alpha != null) // un poquito apañado para los targetHit...
    {
      Color col = alpha.material.GetColor("_TintColor");
      col.a = Mathf.Clamp01(time/2f);
      alpha.material.SetColor("_TintColor", col);
    }
    if(time <= 0) Remove ();
  }

  public void Remove()
  {
	Destroy (gameObject);
  }
}
