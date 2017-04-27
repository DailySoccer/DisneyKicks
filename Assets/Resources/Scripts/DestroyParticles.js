#pragma strict
//var time = 30;

function Start () {

}


/////Destroy(gameObject, particleSystem.duration+0.5f);


function Update () {
Destroy(gameObject, GetComponent.<ParticleSystem>().duration+0.5f);
}

