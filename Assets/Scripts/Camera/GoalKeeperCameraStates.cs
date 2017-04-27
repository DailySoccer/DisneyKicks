using UnityEngine;
using System.Collections;

namespace GoalKeeperCameraStates
{
    public class Init : StateMachine.BaseState
    {
        static Init m_instance;
        public static Init instance { get { if (m_instance == null) m_instance = new Init(); return m_instance; } }
        private Init() { }

        public override void OnEnter(MonoBehaviour _target)
        {
            GoalCamera target = _target as GoalCamera;

            //target.transform.position = FieldControl.instance.m_goalCameraReference.position - Vector3.up * -1f;
			target.transform.position = (Goalkeeper.defaultPosition - BallPhysics.instance.transform.position).normalized * 9f + Goalkeeper.defaultPosition + Vector3.up * 2.82f;
            //target.transform.rotation = FieldControl.instance.m_goalCameraReference.rotation;
            target.transform.LookAt(BallPhysics.instance.transform.position);
        }

        public override void OnFrame(MonoBehaviour _target) { }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class Wait : StateMachine.BaseState
    {
        static Wait m_instance;
        public static Wait instance { get { if (m_instance == null) m_instance = new Wait(); return m_instance; } }
        private Wait() { }

        public override void OnEnter(MonoBehaviour _target) { }
        public override void OnFrame(MonoBehaviour _target) { }
        public override void OnExit(MonoBehaviour _target) { }
    }


    public class Hit : StateMachine.BaseState
    {
        static Hit m_instance;
        public static Hit instance { get { if (m_instance == null) m_instance = new Hit(); return m_instance; } }
        private Hit() { }

        public override void OnEnter(MonoBehaviour _target) { }
        public override void OnFrame(MonoBehaviour _target) { }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class Fly : StateMachine.BaseState
    {
        static Fly m_instance;
        public static Fly instance { get { if (m_instance == null) m_instance = new Fly(); return m_instance; } }
        private Fly() { }

        public override void OnEnter(MonoBehaviour _target) { }
        public override void OnFrame(MonoBehaviour _target) { }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class Action : StateMachine.BaseState
    {
        static Action m_instance;
        public static Action instance { get { if (m_instance == null) m_instance = new Action(); return m_instance; } }
        private Action() { }

        public override void OnEnter(MonoBehaviour _target) {
            GoalCamera target = _target as GoalCamera;
            Vector3 p = Vector3.Slerp(target.transform.position, target.m_shotEndPoint - target.transform.forward * 5f, 0.5f);
            new SuperTweener.move(target.gameObject, 0.5f, p, SuperTweener.QuartOut);
        }
        public override void OnFrame(MonoBehaviour _target) { }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class CoolDown : StateMachine.BaseState
    {
        static CoolDown m_instance;
        public static CoolDown instance { get { if (m_instance == null) m_instance = new CoolDown(); return m_instance; } }
        private CoolDown() { }

        public override void OnEnter(MonoBehaviour _target) { }
        public override void OnFrame(MonoBehaviour _target)
        {
            GoalCamera target = _target as GoalCamera;
            Vector3 p = Goalkeeper.instance.m_ballPoint;
            p.y = target.transform.position.y;
            Quaternion q = Quaternion.LookRotation(p - target.transform.position);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, q, Time.deltaTime);
        }
        public override void OnExit(MonoBehaviour _target) { }
    }


    /// <summary>
    /// Camara de game over de portero en modo duelo
    /// </summary>
    public class CamaraGameOverDueloPortero: StateMachine.BaseState {
        static CamaraGameOverDueloPortero m_instance;
        public static CamaraGameOverDueloPortero instance { get { if (m_instance == null) m_instance = new CamaraGameOverDueloPortero(); return m_instance; } }

        public override void OnEnter(MonoBehaviour _target) {
            GoalCamera target = _target as GoalCamera;
            target.transform.position = new Vector3(-4.0f, 0.25f, -46.0f);          
        }


        public override void OnFrame(MonoBehaviour _target) {
            GoalCamera target = _target as GoalCamera;
            target.transform.LookAt(Goalkeeper.instance.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck").transform.position);
        }


        public override void OnExit(MonoBehaviour _target) {
        }


    }
}
