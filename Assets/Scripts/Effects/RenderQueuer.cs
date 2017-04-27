using UnityEngine;
using System.Collections;

public class RenderQueuer : MonoBehaviour
{
    public int offset;

    public void Start()
    {
        foreach(Material m in GetComponent<Renderer>().materials)
        {
            m.renderQueue = m.renderQueue + offset;
        }
    }
}
