using UnityEngine;
using System.Collections;

public class ifcIconoModo : ifcBase
{
    public Texture m_iconoIniesta;
    public Texture m_iconoCasillas;

	// Use this for initialization
	void Start () {
        Rect rect = GetComponent<GUITexture>().pixelInset;
        if (GameplayService.initialGameMode == GameMode.GoalKeeper)
        {
            GetComponent<GUITexture>().texture = m_iconoCasillas;
        }
        else
        {
            GetComponent<GUITexture>().texture = m_iconoIniesta;
        }
        rect.x = -(GetComponent<GUITexture>().texture.width / 2.0f) + 70.0f;
        rect.y = -GetComponent<GUITexture>().texture.height / 2.0f;
        rect.width = GetComponent<GUITexture>().texture.width;
        rect.height = GetComponent<GUITexture>().texture.height;

        GetComponent<GUITexture>().pixelInset = rect;
    }
	
}
