using UnityEngine;
using System;
using System.Collections.Generic;

public class MissionRound {

    public class RoundPopUp {
        private string _messageID = "";
        public string MessageID {
            get { return _messageID; }
        }

        public RoundPopUp (int popUpData) {
            _messageID = LocalizacionManager.instance.GetTexto(Tutorial.tutorialIndexes[popUpData]);
        }
    }

    public enum ShotPosition {
        Near,
        Medium,
        Far,
    }

    protected ShotPosition _shotPosition = ShotPosition.Medium;

    protected RoundPopUp _roundPopUp = null;    

    public MissionRound (Dictionary<string, object> roundData) {
        if ( roundData.ContainsKey( "ShotPosition" ) ) {
            _shotPosition = GetShotPosition( (string)roundData[ "ShotPosition" ] );
        }
        else {
            throw new ArgumentException( "No has metido posición del tiro, y eso es basico. Shame on you..." );
        }

        if ( roundData.ContainsKey( "PopUp" ) ) {
            _roundPopUp = new RoundPopUp( (int)roundData[ "PopUp" ] );
        }        
    }

    public bool HasPopUp () {
        return ( _roundPopUp != null );
    }

    public RoundPopUp GetPopUp () {
        return _roundPopUp;
    }

    public virtual Difficulty GetDifficulty () {
        return Difficulty.Medium; // Default value
    }

    ShotPosition GetShotPosition (string position) {
        if ( position == "near" ) { return ShotPosition.Near; }
        else if ( position == "medium" ) { return ShotPosition.Medium; }
        else if ( position == "far" ) { return ShotPosition.Far; }
        else {
            throw new ArgumentException( "El tipo ( " + position + " no es valido para la posicion de una ronda" );
        }
    }
}