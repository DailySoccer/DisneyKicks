using UnityEngine;
using System.Collections;

namespace ThrowerCameraStates
{
    public class Init : StateMachine.BaseState
    {
        static Init m_instance;
        public static Init instance { get { if (m_instance == null) m_instance = new Init(); return m_instance; } }
        private Init() { }

        public override void OnEnter(MonoBehaviour _target) {
        /*GoalCamera target = _target as GoalCamera;
            target.transform.position = Thrower.instance.transform.position - Thrower.instance.transform.forward * 1.2f + Vector3.up * 2.0f + Thrower.instance.transform.right * 0.5f;
            target.transform.LookAt( Porteria.instance.position );
            target.m_cameraOrigin = Camera.main.transform.position;*/
        }

        public override void OnFrame(MonoBehaviour _target)
        {
          GoalCamera target = _target as GoalCamera;
          target.transform.position = Vector3.Lerp(target.transform.position, Thrower.instance.transform.position - Thrower.instance.transform.forward * 1.2f + Vector3.up * 2.0f + Thrower.instance.transform.right * 0.5f, 0.2f);
          target.transform.LookAt( Porteria.instance.position );
          target.m_cameraOrigin = Camera.main.transform.position;
        }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class Sharpshooter : StateMachine.BaseState
    {
        static Sharpshooter m_instance;
        public static Sharpshooter instance { get { if (m_instance == null) m_instance = new Sharpshooter(); return m_instance; } }
        private Sharpshooter() { }

        float upwMin = 0.27f;
        float upwMax = 0.27f;

        int fViewMin = 10;
        int fViewMax = 25;

        float distMin = 7f;
        float distMax = 2.8f;

        public override void OnEnter(MonoBehaviour _target) {
        /*GoalCamera target = _target as GoalCamera;
            target.transform.position = Thrower.instance.transform.position - Thrower.instance.transform.forward * 1.2f + Vector3.up * 2.0f + Thrower.instance.transform.right * 0.5f;
            target.transform.LookAt( Porteria.instance.position );
            target.m_cameraOrigin = Camera.main.transform.position;*/

        }

        public override void OnFrame(MonoBehaviour _target)
        {
          float posLerp = Mathf.InverseLerp(-19.90874f, -37.67828f, BallPhysics.instance.transform.position.z);
          float dist = Mathf.Lerp(distMin, distMax, posLerp);
          int view = Mathf.RoundToInt(Mathf.Lerp((float)fViewMin, (float)fViewMax, posLerp));
          _target.GetComponent<Camera>().fieldOfView = Mathf.Lerp(_target.GetComponent<Camera>().fieldOfView, view, 0.1f);
          Vector3 upw = Vector3.up * Mathf.Lerp(upwMin, upwMax, posLerp);
          Vector3 fordw = (BallPhysics.instance.transform.position - Porteria.instance.transform.position).normalized * dist;
          
          //_target.camera.fieldOfView = Mathf.Lerp(45, 15, 0.1f);
          GoalCamera target = _target as GoalCamera;
          target.transform.position = Vector3.Lerp(target.transform.position, BallPhysics.instance.transform.position + fordw + upw, 0.1f);
          target.transform.LookAt( Porteria.instance.position + Vector3.up * (Porteria.instance.VerticalSize / 2f) );
          target.m_cameraOrigin = Camera.main.transform.position;
        }
        public override void OnExit(MonoBehaviour _target) { _target.GetComponent<Camera>().fieldOfView = 45; }
    }

    public class Wait : StateMachine.BaseState
    {
        SuperTweener.action m_tween;

        static Wait m_instance;
        public static Wait instance { get { if (m_instance == null) m_instance = new Wait(); return m_instance; } }
        private Wait() { }

        public override void OnEnter(MonoBehaviour _target)
        {
            var config = ServiceLocator.Request<IDifficultyService>().GetLastShotConfig();
            Vector3 towardsGoal = (Porteria.instance.position - BallPhysics.instance.transform.position).normalized;
            Vector3 pan = config.Position - towardsGoal * 10f + new Vector3(0, 1.8f, 0);
            m_tween = new SuperTweener.move(GoalCamera.instance.gameObject, InputManager.instance.deltaTimeToThrow * 0.75f, pan, SuperTweener.QuadraticOut, (obj) =>{ (_target as GoalCamera).stateMachine.changeState = OnRun.instance; });
        }
        public override void OnFrame(MonoBehaviour _target) { (_target as GoalCamera).transform.LookAt( Porteria.instance.position ); }
        public override void OnExit(MonoBehaviour _target)
        {
            SuperTweener.Kill(m_tween);
        }
    }

    public class OnRun : StateMachine.BaseState
    {
        static OnRun m_instance;
        public static OnRun instance { get { if (m_instance == null) m_instance = new OnRun(); return m_instance; } }
        private OnRun() { }

        Vector3 m_offset;
        Transform m_playerBip01;

        public override void OnEnter(MonoBehaviour _target) {
            m_playerBip01 = Thrower.instance.transform.Find("Bip01");
            m_offset = _target.transform.position - m_playerBip01.position;
        }
        public override void OnFrame(MonoBehaviour _target) {
            _target.transform.position = m_playerBip01.position + m_offset;
        }
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


        //bool m_useLeft = false;
        Vector3 m_oldPostion;

        public override void OnEnter(MonoBehaviour _target) { }
        public override void OnFrame(MonoBehaviour _target) {
            GoalCamera target = _target as GoalCamera;
            target.transform.position = Vector3.Slerp(target.transform.position, BallPhysics.instance.transform.position + Vector3.up * 1.5f, Time.deltaTime*1.6f/* 2.5f*/);
            // Aceleracion de la pelota
            target.acceleration = target.transform.position + (target.transform.position - m_oldPostion) * 5.0f;
            if(target.acceleration.z < -45f) target.acceleration.z = -45f;
            m_oldPostion = target.transform.position;

            Vector3 pos = BallPhysics.instance.transform.position;
            pos.y = 1.8f;
            Quaternion q = Quaternion.LookRotation(pos - target.transform.position);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, q, Time.deltaTime*1.4f);
        }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class HitBarrera : StateMachine.BaseState
    {
        static HitBarrera m_instance;
        public static HitBarrera instance { get { if (m_instance == null) m_instance = new HitBarrera(); return m_instance; } }
        private HitBarrera() { }

        public override void OnEnter(MonoBehaviour _target) { }
        public override void OnFrame(MonoBehaviour _target) {
            GoalCamera target = _target as GoalCamera;
            Vector3 vel = BallPhysics.instance.GetComponent<Rigidbody>().velocity;
            vel.y = 0f;
            Vector3 posi = BallPhysics.instance.transform.position;
            posi.y = 0f;
            target.transform.position = Vector3.Slerp(target.transform.position, posi + new Vector3(0, 1.5f, 5f), Time.deltaTime*3f/* 2.5f*/);

            Vector3 pos = BallPhysics.instance.transform.position;
            pos.y = 1f;
            Quaternion q = Quaternion.LookRotation(pos - target.transform.position);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, q, Time.deltaTime*1.5f);
        }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class Action : StateMachine.BaseState
    {
        static Action m_instance;
        public static Action instance { get { if (m_instance == null) m_instance = new Action(); return m_instance; } }
        private Action() { }

        public override void OnEnter(MonoBehaviour _target) { }
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
            target.transform.position = Vector3.Slerp(target.transform.position, target.acceleration, Time.deltaTime);
            Vector3 p = Porteria.instance.transform.position;
            p.y = target.transform.position.y;
            Quaternion q = Quaternion.LookRotation(p - target.transform.position);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, q, Time.deltaTime);
        }
        public override void OnExit(MonoBehaviour _target) { }
    }

    public class GameOver : StateMachine.BaseState
    {
        static GameOver m_instance;
        public static GameOver instance { get { if (m_instance == null) m_instance = new GameOver(); return m_instance; } }
        private GameOver() { }

        public override void OnEnter(MonoBehaviour _target)
        {
            /*GoalCamera target = _target as GoalCamera;
            Quaternion q = Quaternion.LookRotation(Thrower.instance.transform.position - target.transform.position);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, q, Time.deltaTime);*/
            GoalCamera target = _target as GoalCamera;
            var config = ServiceLocator.Request<IDifficultyService>().GetLastShotConfig();
            Vector3 towardsGoal = (Porteria.instance.position - Thrower.instance.transform.position).normalized;
            Vector3 pan = config.Position - towardsGoal * 6f + new Vector3(0, 1.8f, 0);
            Quaternion q = Quaternion.LookRotation(Thrower.instance.transform.position - pan);
            new SuperTweener.move(GoalCamera.instance.gameObject, 3f, pan, SuperTweener.SinusoidalIn);
            new SuperTweener.rotate(target.gameObject,3f,q,SuperTweener.QuadraticOut);
        }
        public override void OnFrame(MonoBehaviour _target)
        {
            /*GoalCamera target = _target as GoalCamera;
            Quaternion q = Quaternion.LookRotation(Thrower.instance.transform.position - target.transform.position);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, q, Time.deltaTime * 2f);*/
        }
        public override void OnExit(MonoBehaviour _target) { }
    }


    /// <summary>
    /// Clase que espera un tiempo antes de pasar al estado "SelectNetworkCamera"
    /// </summary>
    public class WaitNetworkCamera: StateMachine.BaseState {
        static WaitNetworkCamera m_instance;
        public static WaitNetworkCamera instance { get { if (m_instance == null) m_instance = new WaitNetworkCamera(); return m_instance; } }

        private float m_tiempoEspera; 

        private WaitNetworkCamera() { }

        public override void OnEnter(MonoBehaviour _target) {
            m_tiempoEspera = 3.0f;      // <= tiempo de espera que debe transcurrir para saltar al siguiente estado 
        }

        public override void OnFrame(MonoBehaviour _target) {
            m_tiempoEspera -= Time.deltaTime;
            if (m_tiempoEspera <= 0.0f) {
                GoalCamera.instance.stateMachine.changeState = SelectNetworkCamera.instance;
            }
        }

        public override void OnExit(MonoBehaviour _target) { }
    }


    /// <summary>
    /// Clase que aleatoriamente elige una camara al azar
    /// </summary>
    public class SelectNetworkCamera: StateMachine.BaseState {
        static SelectNetworkCamera m_instance;
        public static SelectNetworkCamera instance { get { if (m_instance == null) m_instance = new SelectNetworkCamera(); return m_instance; } }

        private bool m_hayQueCambiarCamara = false;
        private int m_idUltimaCamaraMostrada = -1;

        private SelectNetworkCamera() { }

        public override void OnEnter(MonoBehaviour _target) {
            m_hayQueCambiarCamara = true;
        }

        public override void OnFrame(MonoBehaviour _target) {
            // comprobar si hay que cambiar la camara
            if (m_hayQueCambiarCamara) {
                // obtener aleatoriamente una camara que sea diferente a la ultima mostrada
                int idNuevaCamara = Random.Range(0, 4);
                while (idNuevaCamara == m_idUltimaCamaraMostrada) {
                    idNuevaCamara = Random.Range(0, 4);
                }

                switch (idNuevaCamara) {
                    case 0: // camara que apunta a los pies del jugador
                        GoalCamera.instance.stateMachine.changeState = CamaraMirandoPelotaPiesTirador.instance;
                        break;

                   case 1: // camara que rota alrededor del jugador
                        CamaraRotacionAlrededorPunto.instance.SetValues(
                            Thrower.instance.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck").transform.position, 0.1f, 4.5f, 0.05f);
                        GoalCamera.instance.stateMachine.changeState = CamaraRotacionAlrededorPunto.instance;
                        break;
                        
                    case 2: // camara que enfoca al portero
                        CamaraDePuntoAPunto.instance.SetValues(
                            new Vector3(0.0f, 0.8f, -47.0f),
                            new Vector3(0.0f, 0.2f, -30f),
                            Goalkeeper.instance.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1").transform.position,
                            new Vector3(0.0f, 6.0f, Goalkeeper.instance.transform.position.z));
                        GoalCamera.instance.stateMachine.changeState = CamaraDePuntoAPunto.instance;
                        break;

                    case 3: // camara que empieza detras de la porteria y se eleva poco a poco hasta grabar al jugador
                        CamaraDePuntoAPunto.instance.SetValues(
                            new Vector3(0.0f, 2.85f, -58.0f),
                            new Vector3(0.0f, 5.5f, -48.0f),
                            BallPhysics.instance.transform.position,
                            Thrower.instance.transform.position, 3.0f, 45.0f, 25.0f);
                        GoalCamera.instance.stateMachine.changeState = CamaraDePuntoAPunto.instance;
                        break;

                    /*
                    case 4: // camara panoramica del estadio
                        CamaraPanoramicaEstadio.instance.SetValues(Vector3.zero, 4.0f, 20f * ((Random.Range(0, 2)) == 1 ? 1f : -1f), 4f, Random.Range(0f, 360f));
                        GoalCamera.instance.stateMachine.changeState = CamaraPanoramicaEstadio.instance;
                        break;

                    case 5: // camara va del jugador al portero por un lateral del campo
                        CamaraDePuntoAPunto.instance.SetValues(
                            new Vector3(-14.0f, 1.7f, Thrower.instance.transform.position.z - 7.0f),
                            new Vector3(-14.0f, 2.0f, Porteria.instance.transform.position.z + 7.0f),
                            Thrower.instance.transform.position,
                            Porteria.instance.transform.position, 3.0f, 45.0f, 35.0f);
                        GoalCamera.instance.stateMachine.changeState = CamaraDePuntoAPunto.instance;
                        break;
                   */
                }

                m_idUltimaCamaraMostrada = idNuevaCamara;
                m_hayQueCambiarCamara = false;
            }
        }

        public override void OnExit(MonoBehaviour _target) {  }
    }


    /// <summary>
    /// Camara que rota alrededor de un punto
    /// </summary>
    public class CamaraRotacionAlrededorPunto: StateMachine.BaseState {
        // parametros editables de la camara
        private Vector3 m_ancla;                        // objetivo al que apunta la camara
        private float m_altura;                         // altura a la que se coloca la camara
        private float m_distancia;                      // distancia (en el plano XZ) a la que se coloca la camara del objetivo
        private float m_vueltas_por_segundo;            // indica cuantas vueltas debe dar la camara al objetivo por segundo
        private float m_time;                           // tiempo que dura esta camara      

        // angulo que tiene actualmente la camara
        private float m_angle;

        static CamaraRotacionAlrededorPunto m_instance;
        public static CamaraRotacionAlrededorPunto instance { get { if (m_instance == null) m_instance = new CamaraRotacionAlrededorPunto(); return m_instance; } }

        private CamaraRotacionAlrededorPunto() { }


        /// <summary>
        /// Asigna los valores para establecer los parametros de funcionamiento de la camara
        /// </summary>
        /// <param name="_ancla">Punto alrededor del cual rota la camara</param>
        /// <param name="_altura">Altura (en el eje Y) a la que se posiciona la camara</param>
        /// <param name="_distancia">Distancia (en el plano XZ) a la que rota la camara alrededor del "_ancla"</param>
        /// <param name="_vueltas_por_Segundo">Vueltas por segundo que da la camara</param>
        /// <param name="_tiempo">TIempo que dura esta camara</param>
        /// <param name="_anguloInicial">Angulo desde el que empieza a rotar la camara</param>
        public void SetValues(Vector3 _ancla, float _altura, float _distancia, float _vueltas_por_Segundo, float _tiempo = 3.0f, float _anguloInicial = 0.0f) {
            m_ancla = _ancla;
            m_altura = _altura;
            m_distancia = _distancia;
            m_vueltas_por_segundo = _vueltas_por_Segundo;
            m_angle = _anguloInicial;
            m_time = _tiempo;
        }


        public override void OnEnter(MonoBehaviour _target) {
        }

        public override void OnFrame(MonoBehaviour _target) {
            // aumentar el angulo
            m_angle += (Time.deltaTime * (2 * Mathf.PI) * m_vueltas_por_segundo);

            // rotar la camara respecto al ancla
            GoalCamera.instance.transform.position = new Vector3(
                m_ancla.x + Mathf.Cos(m_angle) * m_distancia,
                m_altura,
                m_ancla.z + Mathf.Sin(m_angle) * m_distancia);

            GoalCamera.instance.transform.LookAt(m_ancla);

            m_time -= Time.deltaTime;
            if(m_time <= 0f)
            {
                GoalCamera.instance.stateMachine.changeState = SelectNetworkCamera.instance;
            }
        }

        public override void OnExit(MonoBehaviour _target) {  }
    }


    public class CamaraMirandoPelotaPiesTirador : StateMachine.BaseState
    {
        private float m_time = 2f;

        static CamaraMirandoPelotaPiesTirador m_instance;
        public static CamaraMirandoPelotaPiesTirador instance { get { if (m_instance == null) m_instance = new CamaraMirandoPelotaPiesTirador(); return m_instance; } }
        private CamaraMirandoPelotaPiesTirador() { }
        
        public override void OnEnter(MonoBehaviour _target)
        {
            Transform transf = (_target as GoalCamera).GetComponent<Camera>().transform;
            transf.position = Thrower.instance.transform.position;
            transf.position += Vector3.up * 0.2f;
            transf.LookAt(BallPhysics.instance.transform);
            transf.position += transf.forward * -0.7f;
            transf.position += Vector3.right * -1f;
            m_time = 2f;
        }
        public override void OnFrame(MonoBehaviour _target) {
            Transform transf = (_target as GoalCamera).GetComponent<Camera>().transform;
            transf.LookAt(BallPhysics.instance.transform);
            transf.Translate(transf.right * Time.deltaTime);
            
            m_time -= Time.deltaTime;
            if(m_time <= 0f)
            {
                GoalCamera.instance.stateMachine.changeState = SelectNetworkCamera.instance;
            }
        }
        public override void OnExit(MonoBehaviour _target)
        {
        }
    }


    public class CamaraPanoramicaEstadio : StateMachine.BaseState
    {
        // parametros editables de la camara
        private Vector3 m_ancla;                        // objetivo al que apunta la camara
        private float m_altura = 4.0f;                  // altura a la que se coloca la camara
        private float m_vueltas_por_segundo = 0.25f;    // indica cuantas vueltas debe dar la camara al objetivo por segundo
        
        // angulo que tiene actualmente la camara
        private float m_angle;

        private float m_time = 2f;

        static CamaraPanoramicaEstadio m_instance;
        public static CamaraPanoramicaEstadio instance { get { if (m_instance == null) m_instance = new CamaraPanoramicaEstadio(); return m_instance; } }
        private CamaraPanoramicaEstadio() { }
        
        public void SetValues(Vector3 _ancla, float _altura, float _vueltas_por_Segundo, float _time = 2f, float _anguloInicial = 0.0f) {
            m_ancla = _ancla;
            m_altura = _altura;
            m_vueltas_por_segundo = _vueltas_por_Segundo;
            m_angle = _anguloInicial;
            m_time = _time;
        }

        public override void OnEnter(MonoBehaviour _target)
        {
            Transform transf = (_target as GoalCamera).GetComponent<Camera>().transform;
            transf.position = m_ancla;
            transf.position += Vector3.up * m_altura;
            transf.rotation = Quaternion.identity;
            transf.Rotate(new Vector3(0f,m_angle,0f));
            m_time = 2f;
            (_target as GoalCamera).GetComponent<Camera>().fieldOfView = 80f;
        }
        public override void OnFrame(MonoBehaviour _target) {
            Transform transf = (_target as GoalCamera).GetComponent<Camera>().transform;
            transf.Rotate(new Vector3(0f,m_vueltas_por_segundo * Time.deltaTime,0f));
            
            m_time -= Time.deltaTime;
            if(m_time <= 0f)
            {
                GoalCamera.instance.stateMachine.changeState = SelectNetworkCamera.instance;
            }
        }
        public override void OnExit(MonoBehaviour _target)
        {
            (_target as GoalCamera).GetComponent<Camera>().fieldOfView = 45f;
        }
    }


    /// <summary>
    /// Camara que va de un punto a otro del espacio
    /// </summary>
    public class CamaraDePuntoAPunto: StateMachine.BaseState {
        static CamaraDePuntoAPunto m_instance;
        public static CamaraDePuntoAPunto instance { get { if (m_instance == null) m_instance = new CamaraDePuntoAPunto(); return m_instance; } }

        // parametros para definir el comportamiento de la camara
        private Vector3 m_posInicial;
        private Vector3 m_posFinal;
        private Vector3 m_anclaInicial;
        private Vector3 m_anclaFinal;
        private float m_fovInicial;
        private float m_fovFinal;
        private float m_tiempoTransicion;

        private float m_tiempoRestanteTransicion;


        private CamaraDePuntoAPunto() { }

        /// <summary>
        /// Inicializa la camara
        /// </summary>
        /// <param name="_posInicial">Posicion inicial del movimiento de la camara</param>
        /// <param name="_posFinal">Posicion final del movimiento de la camara</param>
        /// <param name="_anclaInicial">Posicion original hacia la que mira la camara</param>
        /// <param name="_anclaFinal">Posicion final hacia la que mira camara</param>
        /// <param name="_tiempoTransicion"></param>
        public void SetValues(Vector3 _posInicial, Vector3 _posFinal, Vector3 _anclaInicial, Vector3 _anclaFinal, float _tiempoTransicion = 3.0f, float _fovInicial = 45.0f, float _fovFinal = 45.0f) {
            m_posInicial = _posInicial;
            m_posFinal = _posFinal;
            m_anclaInicial = _anclaInicial;
            m_anclaFinal = _anclaFinal;
            m_tiempoTransicion = _tiempoTransicion;
            m_fovInicial = _fovInicial;
            m_fovFinal = _fovFinal;

            m_tiempoRestanteTransicion = _tiempoTransicion;
        }


        public override void OnEnter(MonoBehaviour _target) {
        }

        public override void OnFrame(MonoBehaviour _target) {
            // decrementar el tiempo restante
            m_tiempoRestanteTransicion -= Time.deltaTime;
            
            if (m_tiempoRestanteTransicion >= 0.0f) {
                // desplazar la camara
                GoalCamera.instance.transform.position = Vector3.Lerp(m_posInicial, m_posFinal, 1 - (m_tiempoRestanteTransicion / m_tiempoTransicion));
                GoalCamera.instance.transform.LookAt(Vector3.Lerp(m_anclaInicial, m_anclaFinal, 1 - (m_tiempoRestanteTransicion / m_tiempoTransicion)));
                (_target as GoalCamera).GetComponent<Camera>().fieldOfView = Mathf.Lerp(m_fovInicial, m_fovFinal, 1 - (m_tiempoRestanteTransicion / m_tiempoTransicion));
            } else {
                // cambiar de camara
                GoalCamera.instance.stateMachine.changeState = SelectNetworkCamera.instance;
            }

            
        }

        public override void OnExit(MonoBehaviour _target) {  
            // restaurar el valor del FieldOfView
            (_target as GoalCamera).GetComponent<Camera>().fieldOfView = 45.0f;
        }
    }


}
