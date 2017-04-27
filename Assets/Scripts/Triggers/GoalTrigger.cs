using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalTrigger : MonoBehaviour {
	
	float time = 0f;
	Vector3 lastColPoint = Vector3.zero;
	
	void OnDrawGizmos() {
		if(time > 0f) {
			time -= Time.deltaTime;
			Gizmos.DrawSphere(lastColPoint, 1f);
			
		}
	}
}
