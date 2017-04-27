using UnityEngine;
using System.Collections.Generic;

public class cntGameOverScores : MonoBehaviour {

    public class ScoreIndicator {
        public GameObject IndicatorObject;
        public GUIText Label;
        public GUIText Value;

        public ScoreIndicator (GameObject indicatorObject) {
            IndicatorObject = indicatorObject;

            Label = indicatorObject.transform.Find( "Nombre" ).gameObject.GetComponent<GUIText>();
            Value = indicatorObject.transform.Find( "Puntos" ).gameObject.GetComponent<GUIText>();            
        }

        public void SetActive (bool bActive) {
            IndicatorObject.SetActive( bActive );
        }
    }

    private List<ScoreIndicator> _scoreIndicators;

	void Awake () {
        _scoreIndicators = new List<ScoreIndicator>();

        int idx = 1;
        bool bIndicatorFound = false;
	    do {
            Transform t = transform.Find( "Indicador" + idx );
            if ( t != null ) {
                _scoreIndicators.Add( new ScoreIndicator( t.gameObject ) );                    

                bIndicatorFound = true;
                idx++;
            }
            else {
                bIndicatorFound = false;
            }


        } while ( bIndicatorFound  );
	}

    public void SetIndicator (int indicator, string _label, string _value) {
        if ( ( indicator > 0 ) && ( indicator <= _scoreIndicators.Count ) ) {
            _scoreIndicators[ indicator - 1 ].SetActive( true );

            _scoreIndicators[ indicator - 1 ].Label.text = _label;
            _scoreIndicators[ indicator - 1 ].Value.text = _value;
        }
    }

    public void ResetIndicators () {
        foreach ( var indicator in _scoreIndicators ) {
            indicator.SetActive( false );
        }
    }
}
