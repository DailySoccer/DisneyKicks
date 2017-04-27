using UnityEngine;
using System.Collections;

public class SharpShooterCrosshair : MonoBehaviour
{

    void Update ()
    {
        if(InputManager.instance.Blocked)
        {
            Destroy (gameObject);
        }

        //Vector3 pos = Camera.main.WorldToScreenPoint(DifficultyService.currentBullseye.transform.position);
        Vector3 ballPos = Camera.main.WorldToScreenPoint(BallPhysics.instance.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.m_initializedGesture ? Input.mousePosition : ballPos);
        transform.position = ray.direction * 1.5f + ray.origin;
    }
}
