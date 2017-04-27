using UnityEngine;
using System.Collections;

public class fadeAndDestroy : MonoBehaviour {

  public float time = 2f;
  public float fadeOutTime = 1f;

  void Update () {
    time -= Time.deltaTime;
    if(time < fadeOutTime)
    {
      Color col = GetComponent<Renderer>().material.GetColor("_TintColor");
      col.a = time / fadeOutTime;
      GetComponent<Renderer>().material.SetColor("_TintColor", col);
    }
    if(time < 0f) Destroy(gameObject);
  }
}
