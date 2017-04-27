using UnityEngine;
using System.Collections;

public class StateMachine
{
    public class BaseState{
        public virtual void OnEnter(MonoBehaviour _target) { }
        public virtual void OnFrame(MonoBehaviour _target) { }
        public virtual void OnExit(MonoBehaviour _target) { }
    }

    MonoBehaviour m_target;

    public BaseState current { get; private set; }
    BaseState m_deferredState = null;
    float m_deferredTime = 0;
    public BaseState changeState { 
        private get { return m_deferredState; } 
        set { 
            m_deferredState = value; 
            m_deferredTime = 0; 

            // muestra el estado entrante a la maquina de estados
            //if (value!= null) 
            //    Debug.LogWarning(">>> Maquina estados '" + this.ToString() + "' entra estado :" + value.ToString()); 
        } }

    public void deferredState(BaseState _next, float _time) {
        m_deferredState = _next;
        m_deferredTime = _time;
    }

    public StateMachine(MonoBehaviour _target) {
        m_target = _target;
    }

    public void DoFrame()
    {
        if (current != null) current.OnFrame(m_target);
        if (changeState != null )
        {
            if (m_deferredTime > 0) m_deferredTime -= Time.deltaTime;
            else
            {
                m_deferredTime = 0;
                if (current != null) current.OnExit(m_target);
                current = changeState;
                changeState = null;
                if (current != null) current.OnEnter(m_target);
            }
        }
    }

}
