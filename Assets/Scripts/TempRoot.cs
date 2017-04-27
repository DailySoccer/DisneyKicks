using UnityEngine;
using System.Collections;

public class TempRoot : MonoBehaviour {
	
	public GameObject BallPrefab;
	GameObject m_ball;
		
	void Start () {
		new ShotService();
		m_ball = GameObject.Instantiate(BallPrefab) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (Debug.isDebugBuild && Input.GetKeyUp(KeyCode.X))
        {
			ResetBall();
		}
	}
	
	void OnGUI() {

        if (GUI.Button(new Rect(0, Screen.height - 100, 100, 100), "ResetBall"))
            ResetBall();
		
		 if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 100, 100, 100), "ResetBall"))
            ResetBall();
	}
	
	void ResetBall() {
		m_ball.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
		m_ball.transform.position = new Vector3(m_ball.transform.position.x, 0.1f, m_ball.transform.position.z);
		m_ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
		m_ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		m_ball.transform.LookAt(Camera.main.GetComponent<Camera>().transform.position + Camera.main.GetComponent<Camera>().transform.forward * 200f);
	}

}