using UnityEngine;
using System.Collections;

public class InputChrono : MonoBehaviour {

    public float time;
    public static InputChrono instance;

    void Start()
    {
        instance = this;
    }

    void Update ()
    {
        if(time > 0f)
        {
            time -= (Time.timeScale == 0f) ? 0f : (Time.deltaTime / Time.timeScale);
            if(time <= 0f) time = 0f;
            GetComponent<GUIText>().text = time.ToString("#.00");
        }
    }
}
