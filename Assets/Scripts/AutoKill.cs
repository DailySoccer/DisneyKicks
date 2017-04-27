using UnityEngine;
using System.Collections;

public class AutoKill : MonoBehaviour {
	
	float m_curTime = 0f;
	public float m_maxTime;
	
	// Use this for initialization
	void Awake () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_curTime += Time.deltaTime;
		
		if(m_curTime > m_maxTime)
			GameObject.Destroy(this.gameObject);
	}
}
