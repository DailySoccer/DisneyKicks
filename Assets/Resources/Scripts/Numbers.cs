using UnityEngine;
using System.Collections;

public class Numbers : MonoBehaviour {


    public int number;
    public Color color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private Vector2 offset;

	// Update is called once per frame
	void Update () {

        if (number < 1)
            number = 1;
        if (number > 16)
            number = 16;

        int auxNumber = number - 1; // <= para trabajar con un numero en el rango [0 .. 11]

        offset = new Vector2(
            (float) (auxNumber % 4) * 0.25f,
            (float) ((int) (auxNumber / 4)) * 0.25f);

        Vector2 size = new Vector2(1.0f / 4.0f, 1.0f / 4.0f);

        GetComponent<Renderer>().materials[3].SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().materials[3].SetTextureScale("_MainTex", size);
        GetComponent<Renderer>().materials[3].SetColor("_Color", color);
	}


}
