using UnityEngine;
using System.Collections;

public class setLookAt_scoreboard : MonoBehaviour {


	void LateUpdate ()
	{
		GetComponent<SmoothLookAt>().target = Thrower.instance.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck");
	}

}
