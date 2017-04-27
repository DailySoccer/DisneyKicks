using UnityEngine;
using System.Collections;

public class FadeWithTime : MonoBehaviour
{
    public Renderer rend;
    public float time;
    private float originalTime;
    private Color originalColor;

    public void Fade(float _time)
    {
        time = _time;
        originalTime = _time;
        originalColor = rend.material.GetColor("_TintColor");
    }

    void Update ()
    {
        if(time > 0f)
        {
            time -= Time.deltaTime;
            if(time < 0f) Destroy(gameObject);
            Color col = originalColor;
            col.a = Mathf.Lerp(originalColor.a, 0f, 1f - (time/originalTime));
            rend.material.SetColor("_TintColor", col);
        }
    }
}
