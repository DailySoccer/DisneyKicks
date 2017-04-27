using UnityEngine;
using System.Collections;

public class AreaIntuicion : MonoBehaviour
{
    public float time = 1f;
    void Update ()
    {
        time-=Time.deltaTime;
        if (time <= 0f)
        {
            Destroy (gameObject);
        }
    }
}
