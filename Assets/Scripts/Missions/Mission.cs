using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Mission {

    public int indexMision;

    private string _name;
    public string Name {
        get { return _name; }
    }

    private GameMode _playerType;
    public GameMode PlayerType {
        get { return _playerType; }
    }

    public int RoundsCount {
        get { return _rounds.Count; }
    }

    private List<MissionRound> _rounds;
    private int _currentRound = 0;

    private List<MissionAchievement> _achievements;
    public List<MissionAchievement> Achievements {
        get { return _achievements; }
    }

    private bool m_frozenAchievements;
    //basicamente congela los retos para que no se añadan ni quiten nuevos
    //de est aforma hay un modelo para comparar el antes y despues de una partida
    public bool frozenAchievements
    {
        get{return m_frozenAchievements;}
        set
        {
            m_frozenAchievements = value;
            foreach(MissionAchievement ma in _achievements)
            {
                ma.frozen = value;
            }
        }
    }

    /// <summary>
    /// Constructs a new Mission given a string. That string is expected to contain
    /// a JSON sctructure defining the Mission object.
    /// </summary>
    /// <param name="missionData"></param>
    public Mission (string missionData, int _index) {
        indexMision = _index;

        var missionJson = (Dictionary<string, object>)MiniJSON.Json.Deserialize( missionData );

        if (missionJson == null) {
            Debug.LogError(">>> No se ha podido interpretar el JSON de mision: " + missionData);
        } else {

            _name = (string) missionJson["MissionCode"];

            _playerType = GetPlayerType((string) missionJson["PlayerType"]);

            var roundsList = (List<object>) missionJson["Rounds"];
            if (roundsList.Any()) {
                _rounds = new List<MissionRound>();
                foreach (var round in roundsList) {
                    Dictionary<string, object> roundDict = (Dictionary<string, object>) round;

                    if (roundDict.ContainsKey("Combo")) {
                        int roundsInCombo = (int) roundDict["Combo"];
                        Dictionary<string, object> comboRoundDict = (Dictionary<string, object>) roundDict["ComboRound"];
                        for (int i = 0; i < roundsInCombo; ++i) {
                            _rounds.Add(CreateMissionRound(comboRoundDict));
                        }
                    } else {
                        _rounds.Add(CreateMissionRound(roundDict));
                    }
                }
            }

            _currentRound = 0;

            var achievementsList = (List<object>) missionJson["Achievements"];
            if (achievementsList.Any()) {
                _achievements = new List<MissionAchievement>();
                foreach (var achievement in achievementsList) {
                    _achievements.Add(
                        MissionAchievementBuilder.CreateAchievement((Dictionary<string, object>) achievement));
                }
            }
        }
    }

    public void NextRound () {
        if ( !IsMissionFinished() ) {
            _currentRound++;
        }
    }

    public MissionRound GetRoundInfo () {
        if ( !IsMissionFinished() ) {
            return _rounds[ _currentRound ];
        }
        else {
            return null;
        }
    }

    public MissionRound GetPrevRoundInfo () {
        if ( _currentRound > 0 ) {
            return _rounds[ _currentRound - 1 ];
        }
        else {
            return null;
        }
    }

    public bool IsMissionFinished () {
        return ( _currentRound >= _rounds.Count );
    }   

    MissionRound CreateMissionRound (Dictionary<string, object> roundData) {
        MissionRound mr = null;

        if ( _playerType == GameMode.Shooter ) {
            mr = new ShooterMissionRound( roundData );
        }
        else if ( _playerType == GameMode.GoalKeeper ) {
            mr = new GoalkeeperMissionRound( roundData );
        }

        return mr;
    }

    public static GameMode GetPlayerType (string playerType) {
        if ( playerType == "Shooter" ) {
            return GameMode.Shooter;
        }
        else if ( playerType == "Goalkeeper" ) {
            return GameMode.GoalKeeper;
        }
        else {
            throw new ArgumentException( "El tipo de jugador ( " + playerType + " ) no exite, gañan!" );
        }
    }
}