using UnityEngine;
using System.Collections;

public class bullseyePoints : MonoBehaviour {

  int m_points = 0;
  public int points {
    get{ return m_points; }
    set{ m_points = value; apply(); }
  }

  string m_label = "";
  public string label {
    get{ return m_label; }
    set{ m_label = value; apply_text(); }
  }

  Color m_color = Color.white;
  public Color color {
    get{ return m_color; }
    set{ m_color = value; apply_color(); }
  }

  Texture2D m_texture;
  public Texture2D texture {
    get{ return m_texture; }
    set{ m_texture = value; apply_texture(); }
  }

  public float time = 1f;
  private float maxTime;
  private TextMesh text;
  private Renderer img;

  void Start () {
    text = transform.GetChild(0).GetComponent<TextMesh>();
    img = transform.GetChild(0).GetComponent<Renderer>();
    maxTime = time;
    //apply ();
    transform.LookAt(Camera.main.GetComponent<Camera>().transform);
  }

  void apply() {
    transform.localScale = Vector3.one * (Vector3.Distance(Porteria.instance.transform.position, Camera.main.transform.position) / 20f);
  }

  void apply_text() {
    text = transform.GetChild(0).GetComponent<TextMesh>();
    text.text = m_label;
    transform.localScale = Vector3.one * (Vector3.Distance(Porteria.instance.transform.position, Camera.main.transform.position) / 10f);
  }

  void apply_texture() {
    img = transform.GetChild(0).GetComponent<Renderer>();
    img.GetComponent<Renderer>().material.SetTexture("_MainTex", m_texture);
    transform.localScale = Vector3.one * (Vector3.Distance(Porteria.instance.transform.position, Camera.main.transform.position) / 10f);
  }

  void apply_color() {
    text = transform.GetChild(0).GetComponent<TextMesh>();
    text.color = m_color;
  }

  void Update () {
    time -= Time.deltaTime;
    if(time < 0f) Destroy(gameObject);
    Color col;
    if(text) col = text.color;
    else col = img.GetComponent<Renderer>().material.GetColor("_TintColor");
    col.a = time / maxTime;
    if(text) text.color = col;
    else img.GetComponent<Renderer>().material.SetColor("_TintColor", col);
    transform.position += Vector3.up * Time.deltaTime * 1.5f;
    transform.LookAt(Camera.main.GetComponent<Camera>().transform);

  }
}
