using UnityEngine;
using System.Collections;

public class chronoEffect : MonoBehaviour
{

    void Start()
    {
        transform.Find ("VFX_IT_PUP_ST_001_Chrono").GetComponent<AnimatedTextureUV>().AnimLength = InputManager.instance.throwerGestureTime;
        transform.Find ("VFX_IT_PUP_ST_001_Chrono").GetComponent<Renderer>().enabled = false;
    }

    void Update ()
    {
        if(InputManager.instance.Blocked)
        {
            Destroy (gameObject);
        }
        if(InputManager.instance.m_initializedGesture)
        {
            transform.Find ("VFX_IT_PUP_ST_001_Chrono").GetComponent<AnimatedTextureUV>().Play();
            transform.Find ("VFX_IT_PUP_ST_001_Chrono").GetComponent<Renderer>().enabled = true;
            transform.Find ("VFX_IT_PUP_ST_001_Chrono_Flash").GetComponent<Renderer>().enabled = false;
        }
        if(InputManager.instance.m_finishedGesture)
        {
            transform.Find ("VFX_IT_PUP_ST_001_Chrono").GetComponent<Renderer>().enabled = false;
            transform.Find ("VFX_IT_PUP_ST_001_Chrono_Flash").GetComponent<Renderer>().enabled = true;
            transform.Find ("VFX_IT_PUP_ST_001_Chrono").GetComponent<AnimatedTextureUV>().Stop();
        }
    }
}
