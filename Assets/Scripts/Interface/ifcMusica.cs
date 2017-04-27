using UnityEngine;
using System.Collections;

public class ifcMusica : ifcBase
{    
    public static ifcMusica instance { get; protected set; }

    public float volumenOriginal = 0.15f;

    void Awake()
    {
        instance = this;
        //volumenOriginal = audio.volume;
        GetComponent<AudioSource>().volume = 0;
    }

    SuperTweener.volume vol;

    public void musicOn(float time = 2.0f)
    {
        if (!ifcOpciones.music) return;

        SuperTweener.Kill(vol);
        vol = new SuperTweener.volume(gameObject, time, volumenOriginal, SuperTweener.LinearNone);
    }

    public void musicOff(float time =2.0f)
    {
        SuperTweener.Kill(vol);
        vol = new SuperTweener.volume(gameObject, time, 0.0f, SuperTweener.LinearNone);
    }
}
