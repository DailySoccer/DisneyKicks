using UnityEngine;
using System.Collections;

public class CameraSync : MonoBehaviour
{
    public Camera targetCamera;

    void Update()
    {
        GetComponent<Camera>().fieldOfView = targetCamera.fieldOfView;
    }
}
