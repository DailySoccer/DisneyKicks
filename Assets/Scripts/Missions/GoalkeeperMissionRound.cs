using UnityEngine;
using System;
using System.Collections.Generic;

public class GoalkeeperMissionRound : MissionRound {

    public bool HasHelp { get; private set; }

    public bool IsCenteredShot { get; private set; }

    public float BallEffect { get; private set; }

    public Vector2 ShotRange { get; private set; }

    public Jugador Character { get; private set; }

    public bool HasPowerUp { get; private set; }
    public Powerup PowerupType { get; private set; }

    public GoalkeeperMissionRound (Dictionary<string, object> roundData)
        : base( roundData ) {

        if ( roundData.ContainsKey( "Help" ) ) {
            HasHelp = (bool)roundData[ "Help" ];
        }
        else {
            HasHelp = false;
        }

        if ( roundData.ContainsKey( "Character" ) ) {
            Character = InfoJugadores.instance.GetJugador((string)roundData[ "Character" ]);
        }
        else {
            string[] porterosDefault = new string[]{"IT_PLY_ST_0003", "IT_PLY_ST_0004", "IT_PLY_ST_0005"};
            Character = InfoJugadores.instance.GetJugador(porterosDefault[UnityEngine.Random.Range(0, porterosDefault.Length)]);
        }

        if ( roundData.ContainsKey( "CenteredShot" ) ) {
            IsCenteredShot = (bool)roundData[ "CenteredShot" ];
        }
        else {
            IsCenteredShot = false;
        }

        if ( roundData.ContainsKey( "ShotRangeMin" ) ) {
            float min = (float)(double)roundData[ "ShotRangeMin" ];
            float max = (float)(double)roundData[ "ShotRangeMax" ];
            ShotRange = new Vector2(min, max);
        }
        else {
            ShotRange = new Vector2(0f,1f);
        }

        if ( roundData.ContainsKey( "BallEffect" ) ) {
            BallEffect = (float)(double)roundData[ "BallEffect" ];
        }
        else {
            BallEffect = 0.0f;
        }

        if ( roundData.ContainsKey( "PowerUp" ) ) {
            HasPowerUp = true;
            PowerupType = GetPowerUp( (string)roundData[ "PowerUp" ] );
        }
        else {
            HasPowerUp = false;
        }
    }

    public override Difficulty GetDifficulty () {
        switch ( _shotPosition ) {
            case ShotPosition.Near: return Difficulty.Hard;
            case ShotPosition.Medium: return Difficulty.Medium;
            case ShotPosition.Far: return Difficulty.Easy;
        }

        throw new ArgumentOutOfRangeException( "No existe esa dificultad..." );
    }

    private Powerup GetPowerUp (string powerupType) {
        if ( powerupType == "phase" ) { return Powerup.Phase; }
        else if ( powerupType == "greasy" ) { return Powerup.Resbaladiza; }
        else if ( powerupType == "flash" ) { return Powerup.Destello; }
        else if ( powerupType == "focus" ) { return Powerup.Concentracion; }
        else {
            throw new ArgumentOutOfRangeException( "No existe el tipo de powerup ( " + powerupType + " ) -> Revisa!!" );
        }
    }
}