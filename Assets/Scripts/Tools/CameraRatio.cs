using UnityEngine;
using System.Collections;

public class CameraRatio : MonoBehaviour
{
    public float m_aspect;

	// Update is called once per frame
	void Start () {
        GetComponent<Camera>().aspect = m_aspect;
	
	}
}
