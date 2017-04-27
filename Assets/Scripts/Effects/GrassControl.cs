using UnityEngine;
using System.Collections;

public class GrassControl : MonoBehaviour
{
    public Material[] grassMaterials;
    private int grassIndex = 0;
    private Renderer grassRenderer;

    void Awake()
    {
        grassIndex = Random.Range (0, grassMaterials.Length);

        grassRenderer = transform.Find("campo01").GetComponent<Renderer>();
        SetScenario();
    }

    void SetScenario()
    {
        grassRenderer.material = grassMaterials[grassIndex];
    }
}
