using UnityEngine;
using System.Collections;

public struct Hexaedron{
	GameObject ball;
	Vector3[] pointsOfBounds;
	//Vector3 centerOfObject;
	Vector3 extendsOfObject;
	//Vector3 initialPosition;
	float totalPoints;
	
	public Hexaedron(GameObject ball, float totalPoints = 6){
		pointsOfBounds = new Vector3[6];
		this.ball = ball;
		this.totalPoints = totalPoints;
		//this.initialPosition = ball.transform.position;
		//this.centerOfObject = ball.collider.bounds.center;
		this.extendsOfObject = ball.GetComponent<Collider>().bounds.extents;
		initializePointOfBounds();
	}
	
	void initializePointOfBounds (){
	  this.pointsOfBounds[0] = this.ball.transform.forward * extendsOfObject.magnitude;
	  this.pointsOfBounds[1] = this.ball.transform.up * extendsOfObject.magnitude;
	  this.pointsOfBounds[2] = this.ball.transform.right * extendsOfObject.magnitude;
	  this.pointsOfBounds[3] = -this.ball.transform.forward * extendsOfObject.magnitude;
	  this.pointsOfBounds[4] = -this.ball.transform.up * extendsOfObject.magnitude;
	  this.pointsOfBounds[5] = -this.ball.transform.right * extendsOfObject.magnitude;
	}
	
	Vector3[] findPointsOfBounds (){
    Vector3 [] pointsOfBounds = new Vector3[6];

	  for (int i = 0; i < 6; i++) {
			pointsOfBounds[i] = this.pointsOfBounds[i] + ball.transform.position;
	  }

	  return pointsOfBounds;
	}
	
	public bool DetectPercentageOfCollision (float percentage, GameObject other){
		float hits = 0;
		foreach(Vector3 point in findPointsOfBounds()){
			if(other.GetComponent<Collider>().bounds.Contains(point)){
				++hits;
			}
		}

		if(((float)hits / (float)totalPoints) >= percentage){
			return true;
		}

		return false;
	}
	
	/*public Vector3 GetCenterOfBounds(){
		return this.centerOfObject = ball.collider.bounds.center;
	}*/
	
	public float GetRadius (){
		return this.extendsOfObject.magnitude;
	}
}
