using UnityEngine;
using System.Collections;

public class GoalCamera : MonoBehaviour
{

    public static GoalCamera instance { get; private set; }
    void Awake() { instance = this; }


    public float m_goalKeeperTime = 1.0f;
    public float m_throwerTime = 2.0f;

    public Vector3 acceleration;
    public Vector3 m_cameraOrigin;
    public Transform m_dummyRight;
    public Transform m_dummyLeft;
    public Vector3 m_shotEndPoint;

    public StateMachine stateMachine { private set; get; }

    // Use this for initialization
    void Start() { stateMachine = new StateMachine(this); }

    // Update is called once per frame
    void Update() { stateMachine.DoFrame(); }
}
