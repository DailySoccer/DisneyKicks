using UnityEngine;
using System.Collections;

public class BaseCameraState<T> where T: StateMachine.BaseState, new()
{
    public BaseCameraState() : base() { }
//    static T m_instance;
//    public static T instance{ get{ if(m_instance==null) m_instance = new T(); return m_instance; } }
}
