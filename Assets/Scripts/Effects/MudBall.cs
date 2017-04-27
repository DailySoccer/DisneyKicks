using UnityEngine;
using System.Collections;

public class MudBall : MonoBehaviour
{
    GameObject manchaSuelo;
    void Start()
    {
        manchaSuelo = transform.Find ("Mancha1").gameObject;
        manchaSuelo.transform.parent = null;
    }

    public void Clear ()
    {
        Destroy(manchaSuelo);
        Destroy (gameObject);
    }


}
