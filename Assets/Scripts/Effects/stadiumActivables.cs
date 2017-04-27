using UnityEngine;
using System.Collections;

public class stadiumActivables : MonoBehaviour
{
    public GameObject[] whenGoal;
    public GameObject[] whenReset;

    void Start()
    {
        GameObject.Find ("triggerer").GetComponent<TriggerEffectsKicks>().whenGoal = whenGoal;
		GameObject.Find ("triggerer").GetComponent<TriggerEffectsKicks>().whenReset = whenReset;
    }
}
