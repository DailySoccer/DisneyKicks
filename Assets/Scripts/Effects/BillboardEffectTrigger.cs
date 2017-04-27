using UnityEngine;
using System.Collections.Generic;

public class BillboardEffectTrigger : MonoBehaviour {

    public GameObject idleBillboard;
    public GameObject powerupBillboard;
    public GameObject[] billboards;
    private GameObject activeBillboard;
    private List<GameObject> nextBillboards;
    private float countdown = 0f;

    void Start ()
    {
        nextBillboards = new List<GameObject>();
        ServiceLocator.Request<IPowerupService>().RegisterListener(TriggerBillboard);
        ServiceLocator.Request<IShotResultService>().RegisterListener(Reset);
        activeBillboard = idleBillboard;
    }

    void Update()
    {
        if(countdown > 0f)
        {
            countdown -= Time.timeScale == 0f ? 0f : (Time.deltaTime / Time.timeScale);
            if(countdown <= 0) Continue ();
        }
    }

    void Continue()
    {
        activeBillboard.SetActive(false);
        GameObject next = nextBillboards[0];
        next.SetActive(true);
        activeBillboard = next;
        nextBillboards.Remove(next);
        AnimatedTextureUV animControl = next.transform.GetChild(0).GetComponent<AnimatedTextureUV>();
        if(nextBillboards.Count != 0) countdown = animControl.AnimLength;
        
    }

    void TriggerBillboard(PowerupUsage _info)
    {
        nextBillboards.Clear();
        nextBillboards.Add (powerupBillboard);
        nextBillboards.Add (billboards[_info.AbsId]);
        nextBillboards.Add (idleBillboard);
        countdown = 0.001f;
    }

    void Reset()
    {
        Reset(new ShotResult());
    }

    void Reset(ShotResult _info)
    {
        activeBillboard.SetActive(false);
        idleBillboard.SetActive(true);
        activeBillboard = idleBillboard;
        countdown = 0f;
        nextBillboards.Clear();
    }

}
