using UnityEngine;
using System.Collections.Generic;

public class AnimationsFeed : MonoBehaviour {
  public bool random = false;
  AnimationState m_animstate;
  int index = 0;
  string[] names;

  void Start ()
  {
    names = new string[GetComponent<Animation>().GetClipCount()];
    int count = 0;
    foreach ( AnimationState clip in GetComponent<Animation>()) {
      names[count] = clip.name;
      count++;
    }
    m_animstate = GetComponent<Animation>()[names[index]];
    GetComponent<Animation>().Play(names[index]);
  }
  
  void Update ()
  {
    if(!m_animstate.enabled)
    {
      if(!random)
      {
        index = (index + 1) % GetComponent<Animation>().GetClipCount();
      }
      else
      {
        index = Random.Range(0,GetComponent<Animation>().GetClipCount());
      }
      m_animstate = GetComponent<Animation>()[names[index]];
      GetComponent<Animation>().CrossFade(names[index]);
    }
  }
}
