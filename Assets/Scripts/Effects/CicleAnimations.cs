using UnityEngine;
using System.Collections.Generic;

public class CicleAnimations : MonoBehaviour {

    public string [] animationStates;
    public int index;
    public bool goalkeeper = false;

    public string GetAnim()
    {
        if(animationStates.Length>0)
            return animationStates[index];
        else return goalkeeper ? "P_IdleInterface_01" : "IdleInterface_01";
    }

    void Start ()
    {
        animationStates = goalkeeper ? new string[]{"P_IdleInterface_01","P_IdleInterface_02","P_IdleInterface_03","P_IdleInterface_04"} :
                                       new string[]{"IdleInterface_01","IdleInterface_02","IdleInterface_03","IdleInterface_04"}; //un mojon de forma de hacerlo, pero animation las da en desorden!!!
        /*int i = 0;
        foreach(AnimationState state in animation)
        {
            animationStates[i] = state.name;
            Debug.Log (animationStates[i]);
            i++;
        }*/
        //index = Random.Range(0, animationStates.Count);
    }

    void Update ()
    {
        AnimationState state = GetComponent<Animation>()[animationStates[index]];
        if(!state.enabled || state.time >= state.length)
        {
            index = Random.Range(0, animationStates.Length);
            GetComponent<Animation>().Play(animationStates[index], PlayMode.StopAll);
        }
    }
}
