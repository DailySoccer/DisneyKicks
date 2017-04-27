using UnityEngine;
using System.Collections.Generic;

public class cntGameOverAchievements : MonoBehaviour {

    public class GameOverAchievement {
        public GameObject AchievementObject;
        public GUIText Label;
        public GUITexture Icon;
        public GUIText Reward;

        public GameOverAchievement (GameObject achievementObject) {
            AchievementObject = achievementObject;

            Label = achievementObject.transform.Find( "texto" ).gameObject.GetComponent<GUIText>();
            Icon = achievementObject.transform.Find( "estrella" ).gameObject.GetComponent<GUITexture>();
            if(achievementObject.transform.Find( "recompensa" ))
            {
                Reward = achievementObject.transform.Find( "recompensa" ).gameObject.GetComponent<GUIText>();
                Reward.text = "";
            }

        }
    }


    // texturas para mostrar que un objetivo se ha conseguido o no
    // NOTA: Asignarles valor desde la interfaz de Unity
    public Texture[] m_texturasObjetivosNoConseguidos;
    public Texture[] m_texturasObjetivosConseguidos;
    
    private List<GameOverAchievement> _gameOverAchievements;



    void Awake () {
        _gameOverAchievements = new List<GameOverAchievement>();

        int idx = 1;
        bool bAchievementFound = false;
        do {
            Transform t = transform.Find( "objetivo" + idx );
            if ( t != null ) {
                _gameOverAchievements.Add( new GameOverAchievement( t.gameObject ) );

                bAchievementFound = true;
                idx++;
            }
            else {
                bAchievementFound = false;
            }


        } while ( bAchievementFound );
    }

    public void SetGameOverAchievement (int achievement, string _label, bool _achievedBefore, bool _achievedNow, int _reward = 0) {
        if ( ( achievement > -1 ) && ( achievement < _gameOverAchievements.Count ) ) {
            _gameOverAchievements[ achievement ].Label.text = _label;
            //_gameOverAchievements[ achievement ].Icon.gameObject.SetActive( _achievedBefore || _achievedNow);
            Color colorIconAchieved = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            Color colorIconNotAchieved = new Color(0.5f, 0.5f, 0.5f, 50.0f / 255.0f);
            Color colorTextAchieved = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Color colorTextNotAchieved = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            if (_achievedBefore || _achievedNow) {
                _gameOverAchievements[achievement].Icon.GetComponent<GUITexture>().texture = m_texturasObjetivosConseguidos[achievement];
                _gameOverAchievements[achievement].Icon.GetComponent<GUITexture>().color = colorIconAchieved;
                _gameOverAchievements[achievement].Label.color = colorTextAchieved;
            } else {
                _gameOverAchievements[achievement].Icon.GetComponent<GUITexture>().texture = m_texturasObjetivosNoConseguidos[achievement];
                _gameOverAchievements[achievement].Icon.GetComponent<GUITexture>().color = colorIconNotAchieved;
                _gameOverAchievements[achievement].Label.color = colorTextNotAchieved;
            }
            /*
            if(_achievedBefore) col = Color.white;
            //if(_achievedNow) col = Color.green;
            if(_achievedNow && !_achievedBefore) col = Color.yellow;
            _gameOverAchievements[ achievement ].Icon.guiTexture.color = col;
             * */

            //if(_reward > 0) _gameOverAchievements[ achievement ].Reward.text = "+ " + _reward.ToString();
        }
    }

    public void ResetIndicators () {
        foreach ( var achievement in _gameOverAchievements ) {
            achievement.Icon.gameObject.SetActive( false );
        }
    }
}
