using UnityEngine;
using System.Collections;

public class BackOfFieldTrigger : MonoBehaviour {
	
	float time = 0f;
	Vector3 lastColPoint = Vector3.zero;
	
	void OnTriggerEnter (Collider other){
		//Debug.Log("ENTER GOAL TRIGGER");
		time = 1f;
		lastColPoint = other.GetComponent<Collider>().transform.position;
	}
	
	void OnTriggerStay(Collider other) {
	  /*
	  if(this.ballHexaedron.DetectPercentageOfCollision(0.5f, this.gameObject)){
		ShotResult shotResult = new ShotResult();
		shotResult.Result = Result.OutOfBounds;
		shotResult.Point = this.ballHexaedron.GetCenterOfBounds();
		ServiceLocator.Request<IShotResultService>().OnShotEnded(shotResult);
	  }
	  */
	}
	
	void OnDrawGizmos() {
		if(time > 0f) {
			time -= Time.deltaTime;
			Gizmos.DrawSphere(lastColPoint, 1f);
			
		}
	}
}
