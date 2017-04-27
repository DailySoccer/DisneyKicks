var target : Transform;
var damping = 6.0;
var smooth = true;

@script AddComponentMenu("Camera-Control/Smooth Look At")

var onlyYAxis = false;

function LateUpdate () {
	if (target) {
		if (smooth)
		{
			var pos = target.position - transform.position;
			if(onlyYAxis)
			{
				 pos = (new Vector3(target.position.x, 0f, target.position.z) - 
						new Vector3(transform.position.x, 0f, transform.position.z));
			}
			// Look at and dampen the rotation
			var rotation = Quaternion.LookRotation(pos);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		}
		else
		{
			transform.LookAt(new Vector3(target.position.x, onlyYAxis ? transform.position.y : target.position.y, target.position.z));
		}
	}
}

function Start () {
	// Make the rigid body not change rotation
   	if (GetComponent.<Rigidbody>())
		GetComponent.<Rigidbody>().freezeRotation = true;
}