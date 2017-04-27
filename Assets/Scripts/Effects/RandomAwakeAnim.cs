using UnityEngine;
using System.Collections;

public class RandomAwakeAnim : MonoBehaviour
{
  float prob = 0.3f;
  string anim = "";

  public void Awake()
  {
    foreach(AnimationState state in GetComponent<Animation>())
    {
      anim = state.name;
    }
  }

  public void OnEnable()
  {
    float tirada = Random.Range(0f,1f);
    if(tirada > prob)
    {
      GetComponent<Animation>()[anim].normalizedTime = 1f;
    }
  }
}
