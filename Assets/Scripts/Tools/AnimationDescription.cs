using UnityEngine;
using System.Collections;

public class AnimationDescriptors//: ScriptableObject
{
  AnimationDescriptor[] m_animations;

  class AnimationDescriptor{
    public AnimationState m_state;

    Vector3 m_velocity;
    public AnimationDescriptor(AnimationState _state, Vector3 _velocity){
      m_state = _state;
      m_velocity = _velocity;
    }

    public Vector3 calc(){
      return m_state.weight * m_state.speed * m_velocity;
    }
  };

  public AnimationDescriptors(Animation _animation, Animation _source){

    AnimationDescriptorsResource adr = Resources.Load("AnimationDescriptorsResource", typeof(AnimationDescriptorsResource)) as AnimationDescriptorsResource;
    if (adr != null){
      m_animations = new AnimationDescriptor[adr.m_descriptors.Length];
      _source = (GameObject.Instantiate(_source.gameObject) as GameObject).GetComponent<Animation>();
      for (int i = 0; i < adr.m_descriptors.Length; ++i){
        AnimationClip clip = _source.GetClip(adr.m_descriptors[i].m_name);
        if (clip == null) Debug.LogError("No se encuentra la animacion " + adr.m_descriptors[i].m_name + " " + _source.GetClipCount() );
        else{
          _animation.AddClip(clip, adr.m_descriptors[i].m_name);
          m_animations[i] = new AnimationDescriptor(_animation[adr.m_descriptors[i].m_name], adr.m_descriptors[i].m_velocity);
        }
      }
      GameObject.Destroy(_source.gameObject);
    }
  }


  public Vector3 getVelocity(){
    Vector3 vel = Vector3.zero;
    foreach (AnimationDescriptor desc in m_animations)
      if (desc.m_state!=null && desc.m_state.enabled)
        vel += desc.calc();
  return vel;
  }

}