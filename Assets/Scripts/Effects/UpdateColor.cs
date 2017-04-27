using UnityEngine;
using System.Collections;

public class UpdateColor : MonoBehaviour {

	public string colorName;
	
	public Color color;

	void Awake()
	{
		color = GetComponent<Renderer>().material.GetColor(colorName);
	}

	void Update ()
	{
		GetComponent<Renderer>().material.SetColor(colorName, color);
	}
}
