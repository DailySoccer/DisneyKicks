using UnityEngine;
using System.Collections;

public class Doppelganger : MonoBehaviour
{
    public string m_animName;
    public float m_balltime;

    void Update ()
    {
        m_balltime -= Time.deltaTime;

        /*
        animation[m_animName].speed =  Mathf.Clamp01(m_balltime / originalTime);

        if(m_balltime <= 0f)
        {
            Destroy (gameObject);
        }
        */

        if(m_balltime <= 0f)
        {
            GetComponent<Animation>()[m_animName].speed = 1f;
        }

        if(!GetComponent<Animation>().isPlaying) Destroy(gameObject);
    }
}
