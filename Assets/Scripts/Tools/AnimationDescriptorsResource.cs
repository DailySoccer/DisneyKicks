using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationGroup
{
  General,
  Idle,
  IdleCB,
  Correr,
  CorrerCB,
  Pase,
  Recepcion,
  Robo
};


[System.Serializable]
public class AnimationDescriptorsResource : ScriptableObject
{
  [System.Serializable]
  public class AnimationDescriptorResource
  {
    [SerializeField]
    public string m_name;
    [SerializeField]
    public Vector3 m_velocity;
    [SerializeField]
    public AnimationGroup m_group = AnimationGroup.General;

    [SerializeField]
    public float m_grabTime;
    [SerializeField]
    public Vector3 m_grabDiff;

    public AnimationDescriptorResource(string _name, Vector3 _velocity, float _grabTime, Vector3 _grabDiff)
    {
      m_name = _name;
      m_velocity = _velocity;
      m_grabTime = _grabTime;
      m_grabDiff = _grabDiff;
    }
  };

  [SerializeField]
  public AnimationDescriptorResource[] m_descriptors;

  public bool Add(string _name, Vector3 _velocity, float _grabTime, Vector3 _grabDiff)
  {
    AnimationDescriptorResource[] tmp;
    if (m_descriptors != null) {
      for (int i = 0; i < m_descriptors.Length; ++i) {
        if (m_descriptors[i].m_name == _name ) {
          m_descriptors[i].m_velocity = _velocity;
          m_descriptors[i].m_grabTime = _grabTime;
          m_descriptors[i].m_grabDiff = _grabDiff;
          return m_descriptors[i].m_velocity != _velocity;
        }
      }
      tmp = new AnimationDescriptorResource[m_descriptors.Length + 1];
      System.Array.Copy(m_descriptors, tmp, m_descriptors.Length);
      tmp[m_descriptors.Length] = new AnimationDescriptorResource(_name, _velocity, _grabTime, _grabDiff);
    } else
    {
      tmp = new AnimationDescriptorResource[1];
      tmp[0] = new AnimationDescriptorResource(_name, _velocity, _grabTime, _grabDiff);
    }
    m_descriptors = tmp;
    return true;
  }

  public AnimationDescriptorResource GetByName(string _name)
  {
      for (int i = 0; i < m_descriptors.Length; ++i)
        if (m_descriptors[i].m_name == _name )
          return m_descriptors[i];
      return null;
  }
};