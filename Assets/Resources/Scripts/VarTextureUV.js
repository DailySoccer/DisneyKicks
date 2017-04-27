var uvTileX = 4; //Here you can place the number of columns of your sheet.
                           //The above sheet has 24

var uvTileY = 4; //Here you can place the number of rows of your sheet.
                          //The above sheet has 1
var offsetX = 0.0;
var offsetY = 0.0;


function Update () {

    
    var size = Vector2 (1.0 / uvTileX, 1.0 / uvTileY);
	var offset = Vector2 (offsetX / 100, offsetY / 100);
   
    
	GetComponent.<Renderer>().materials[3].SetTextureOffset ("_MainTex", offset);
	GetComponent.<Renderer>().materials[3].SetTextureScale ("_MainTex", size);
}