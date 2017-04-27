using UnityEngine;
using System.Collections.Generic;

public class cntGameOverScoreOverall : MonoBehaviour {

    private GUIText _pointsLabel;
    private GUIText _pointsValue;
    private GUIText _coinRewardLabel;
    private GUIText _coinRewardValue;

	void Awake () {
	    _pointsLabel = getGUITextByName( "puntuacion/Nombre" );
        _pointsValue = getGUITextByName( "puntuacion/Valor" );
        _coinRewardLabel = getGUITextByName( "recompensa/Nombre" );
        _coinRewardValue = getGUITextByName( "recompensa/Valor" );
	}

    public void SetPointsAndReward (int points, int coinReward) {
        _pointsLabel.text = LocalizacionManager.instance.GetTexto(73).ToUpper();
        _coinRewardLabel.text = LocalizacionManager.instance.GetTexto(193).ToUpper();

        _pointsValue.text = points.ToString();
        _coinRewardValue.text = coinReward.ToString() + " ¤";
    }
	
	private GUIText getGUITextByName (string guiName) {
        Transform t = transform.Find( guiName );
        if ( t != null ) {
            return t.gameObject.GetComponent<GUIText>();
        }

        throw new System.NullReferenceException( "No se ha encontrado el GUIText " + guiName );
    }
}
