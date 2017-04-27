using UnityEngine;
using System.Collections;

public class PhaseBall : MonoBehaviour {

    bool mode = true;
    float invisibleTime = 0.3f;
    float visibleTime = 0.1f;
    float countdown = 0.1f;

    void Start()
    {
        countdown = visibleTime;
        ServiceLocator.Request<IShotResultService>().RegisterListener(Destroy);
    }

    void Destroy(ShotResult _info)
    {
        GetComponent<Renderer>().enabled = true;
        transform.Find ("trail").GetComponent<Renderer>().enabled = true;
        ServiceLocator.Request<IShotResultService>().UnregisterListener(Destroy);
        Destroy (this);
    }

    void Update ()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0f)
        {
            countdown = mode ? invisibleTime : visibleTime;
            mode = !mode;
            GetComponent<Renderer>().enabled = mode;
            transform.Find ("trail").GetComponent<Renderer>().enabled = mode;
        }
    }
}
