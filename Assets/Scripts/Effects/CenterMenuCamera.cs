using UnityEngine;
using System.Collections;

public class CenterMenuCamera : MonoBehaviour
{
    void Update ()
    {
        if (Interfaz.instance != null && Interfaz.instance.goalkeeperModel != null && Interfaz.instance.throwerModel != null) {
            if (Interfaz.instance.goalkeeperModel.activeSelf && Interfaz.instance.throwerModel.activeSelf) {
                Vector3 pos1 = GameObject.Find("girarLanzadorCollider").transform.position + Vector3.up * 0.5f;
                Vector3 pos2 = GameObject.Find("girarPorteroCollider").transform.position + Vector3.up * 0.5f;
                transform.position = (pos1 + pos2) / 2f;
            } else if (Interfaz.instance.throwerModel.activeSelf) {
                Vector3 pos1 = GameObject.Find("girarLanzadorCollider").transform.position + Vector3.up * 0.5f;
                transform.position = pos1;
            } else if (Interfaz.instance.goalkeeperModel.activeSelf) {
                Vector3 pos2 = GameObject.Find("girarPorteroCollider").transform.position + Vector3.up * 0.5f;
                transform.position = pos2;
            } else {
                transform.position = new Vector3(-5f, 1.4f, -8.8f);
            }
        }
    }
}
