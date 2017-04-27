using UnityEngine;
using System.Collections;

public class hit : MonoBehaviour {
  public float life = 1f;
  public float maxSize = 1f;
  Transform onda;
  float time = 0f;

  public Color color {
    set {
      transform.Find("punto").GetComponent<Renderer>().material.SetColor("_TintColor", value);
      transform.Find("onda").GetComponent<Renderer>().material.SetColor("_TintColor", value);
    }
  }

  void Start() {
    onda = transform.Find ("onda");
    onda.localScale = Vector3.zero;
  }

  void Update () {
    transform.localScale = Vector3.one * (Camera.main.transform.position - transform.position).magnitude / 10f;
    time += Time.deltaTime;
    if(time >= life) Destroy (gameObject);
    float relativeLife = time / life;
    onda.localScale = Vector3.one * relativeLife * maxSize;
    Color col = onda.GetComponent<Renderer>().material.GetColor("_TintColor");
    col.a = 1f - relativeLife;
    onda.GetComponent<Renderer>().material.SetColor("_TintColor", col);
  }
}
