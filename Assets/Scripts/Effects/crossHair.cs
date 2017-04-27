using UnityEngine;
using System.Collections;

public class crossHair : MonoBehaviour {
  public float time = 1.2f;
  //public float alphaTime = 1.2f;
  public float alpha = 0f;
  Transform onda;
  Transform punto;
  float timeCount = 0f;
  Vector3 originalScale;

  void Start() {
    onda = transform.Find ("onda");
    originalScale = onda.localScale;
    punto = transform.Find ("punto");

    Color col = onda.GetComponent<Renderer>().material.GetColor("_TintColor");
    col.a = alpha;
    onda.GetComponent<Renderer>().material.SetColor("_TintColor", col);


    col = punto.GetComponent<Renderer>().material.GetColor("_TintColor");
    col.a = alpha;
    punto.GetComponent<Renderer>().material.SetColor("_TintColor", col);
  }

  void Update () {
    timeCount +=  (Time.timeScale == 0f ? 0f: Time.deltaTime);
    if(/*timeCount >= alphaTime ||*/ timeCount >= time) Destroy (gameObject);
    float relativeLife;
    if(time > 0f)
    {
      relativeLife = (time - timeCount) / time;
    }
    else
    {
      relativeLife = 0f;
    }

    onda.localScale = originalScale * relativeLife;

    /*Color col = onda.renderer.material.GetColor("_TintColor");
    col.a = 1f - Mathf.Clamp01(timeCount / alphaTime);
    onda.renderer.material.SetColor("_TintColor", col);*/

    /*col = punto.renderer.material.GetColor("_TintColor");
    col.a = 1f - Mathf.Clamp01(timeCount / alphaTime);
    punto.renderer.material.SetColor("_TintColor", col);*/
  }
}
