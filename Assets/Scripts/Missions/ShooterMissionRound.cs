using UnityEngine;
using System;
using System.Collections.Generic;

public class ShooterMissionRound : MissionRound {

    public class YellowZoneDesc {
        public enum SizeOfYellowZone { S, M, L, }
        public SizeOfYellowZone Size { get; private set; }

        public YellowZoneDesc (string yellowZoneData) {
            Size = GetSize( yellowZoneData );
        }

        private SizeOfYellowZone GetSize (string yellowZoneSize) {
            if ( yellowZoneSize == "small" ) { return SizeOfYellowZone.S; }
            else if ( yellowZoneSize == "medium" ) { return SizeOfYellowZone.M; }
            else if ( yellowZoneSize == "large" ) { return SizeOfYellowZone.L; }
            else {
                throw new ArgumentException( "Tamaño de zona amarilla ( " + yellowZoneSize + " ) incorrecto, repasa melon" );
            }
        }
    }

    public class BullseyeDesc {

        public SizeOfBullseye Size { get; private set; }
        public HeightOfBullseye Height { get; private set; }
        public bool HasStaticSize { get; private set; }

        public bool HasStaticPosition { get; private set; }

        public ZoneOfBullseye bullseyePosition { get; private set; }

        public bool HasYellowZone;
        public YellowZoneDesc yellowZoneDesc = null;

        public BullseyeDesc (Dictionary<string, object> bullseyeData) {
            
            
            if ( bullseyeData.ContainsKey( "Size" ) ) {
                Size = GetSize( (string)bullseyeData[ "Size" ] );
            }
            else {
                Size = SizeOfBullseye.L;
            }

            if ( bullseyeData.ContainsKey( "Height" ) ) {
                Height = GetHeight( (string)bullseyeData[ "Height" ] );
            }
            else {
                Height = GetRandomHeight();
            }

            if ( bullseyeData.ContainsKey( "StaticPosition" ) ) {
                HasStaticPosition = (bool)bullseyeData[ "StaticPosition" ];
            }
            else {
                HasStaticPosition = true;
            }

            if ( bullseyeData.ContainsKey( "Position" ) ) {
                bullseyePosition = GetBullseyePosition( (string)bullseyeData[ "Position" ] );
            }
            else {
                bullseyePosition = GetRandomBullseyePosition();
            }

            if ( bullseyeData.ContainsKey( "StaticSize" ) ) {
                HasStaticSize = (bool)bullseyeData[ "StaticSize" ];
                if(!HasStaticSize)
                {
                    Size = SizeOfBullseye.L;
                }
            }
            else {
                HasStaticSize = true;
            }

            if ( bullseyeData.ContainsKey( "YellowZone" ) ) {
                HasYellowZone = true;
                yellowZoneDesc = new YellowZoneDesc( (string)bullseyeData[ "YellowZone" ] );
            }
            else {
                HasYellowZone = false;
            }
        }

        private SizeOfBullseye GetSize (string bullseyeSize) {
            if ( bullseyeSize == "small" ) { return SizeOfBullseye.S; }
            else if ( bullseyeSize == "medium" ) { return SizeOfBullseye.M; }
            else if ( bullseyeSize == "large" ) { return SizeOfBullseye.L; }
            else {
                throw new ArgumentException( "Tamaño de diana ( " + bullseyeSize + " ) incorrecto, repasa melon" );
            }
        }

        private HeightOfBullseye GetHeight (string bullseyeHeight) {
            if ( bullseyeHeight == "low" ) { return HeightOfBullseye.Baja; }
            else if ( bullseyeHeight == "medium" ) { return HeightOfBullseye.Media; }
            else if ( bullseyeHeight == "high" ) { return HeightOfBullseye.Alta; }
            else if ( bullseyeHeight == "random" ) { return GetRandomHeight(); }
            else {
                throw new ArgumentOutOfRangeException( "Altura ( " + bullseyeHeight + " ) no reconocida" );
            }
        }

        private HeightOfBullseye GetRandomHeight () {
            int idx = UnityEngine.Random.Range( 0, 3 );
            switch ( idx ) {
                case 0: return HeightOfBullseye.Baja;
                case 1: return HeightOfBullseye.Media;
                case 2: return HeightOfBullseye.Alta;
            }

            return HeightOfBullseye.Media;
        }

        private ZoneOfBullseye GetBullseyePosition (string bullseyePosition) {
            if ( bullseyePosition == "center" ) { return ZoneOfBullseye.Centro; }
            else if ( bullseyePosition == "side" ) { return ZoneOfBullseye.Lados; }
            else if ( bullseyePosition == "random" ) { return GetRandomBullseyePosition(); }
            else {
                throw new ArgumentException( "Posicion de diana ( " + bullseyePosition + " ) incorrecta, repasa melon" );
            }
        }

        private ZoneOfBullseye GetRandomBullseyePosition () {
            return ( ( UnityEngine.Random.Range(0, 100) < 50 ) ? ZoneOfBullseye.Centro : ZoneOfBullseye.Lados );
        }
    }

    public bool HasBullseye { get; private set; }
    public BullseyeDesc bullseyeDesc = null;
    public bool HasYellowZone {
        get{
            if(HasBullseye) {
                return (bullseyeDesc.HasYellowZone);
            }
            else {
                return false;
            }
        }
        private set{}
    }


    public bool HasGoalkeeper { get; private set; }
    public float GoalkeeperSkill { get; private set; }

    public Jugador Character { get; private set; }

    public bool HasSheet { get; private set; }
    public SabanasManager.TipoSabanaInidividual[] SheetSectorDifficulties { get; private set; }

    public bool HasWall { get; private set; }
    public int WallSize { get; private set; }

    public ShooterMissionRound (Dictionary<string, object> roundData)
        : base( roundData ) {

        if ( roundData.ContainsKey( "Bullseye" ) ) {
            HasBullseye = true;
            bullseyeDesc = new BullseyeDesc( (Dictionary<string, object>)roundData[ "Bullseye" ] );
        }
        else {
            HasBullseye = false;
        }

        if ( roundData.ContainsKey( "Goalkeeper" ) ) {
            HasGoalkeeper = true;
            GoalkeeperSkill = (float) (double) roundData["Goalkeeper"];
        }
        else {
            HasGoalkeeper = false;
        }

        if ( roundData.ContainsKey( "Character" ) ) {
            Character = InfoJugadores.instance.GetJugador( (string)roundData["Character"] );
        }
        else {
            string[] porterosDefault = new string[]{"IT_PLY_GK_0002", "IT_PLY_GK_0003"};
            Character = InfoJugadores.instance.GetJugador( porterosDefault[UnityEngine.Random.Range(0, porterosDefault.Length)]);
        }

        if ( roundData.ContainsKey( "Sheet" ) ) {
            HasSheet = true;

            var sheetSectorDifficultiesList = (List<object>)roundData[ "Sheet" ];
            SheetSectorDifficulties = new SabanasManager.TipoSabanaInidividual[ sheetSectorDifficultiesList.Count ];
            for ( int i = 0; i < sheetSectorDifficultiesList.Count; ++i ) {
                SheetSectorDifficulties[ i ] = GetSheetSectorDifficulty( (string)sheetSectorDifficultiesList[ i ] );
            }
        }
        else {
            HasSheet = false;
        }

        CheckRoundCorrectness();

        if ( roundData.ContainsKey( "Wall" ) ) {
            HasWall = true;
            WallSize = (int)roundData[ "Wall" ];
        }
        else {
            HasWall = false;
        }        
    }

    public override Difficulty GetDifficulty () {
        switch ( _shotPosition ) {
            case ShotPosition.Near: return Difficulty.Easy;
            case ShotPosition.Medium: return Difficulty.Medium;
            case ShotPosition.Far: return Difficulty.Hard;
        }

        throw new ArgumentOutOfRangeException( "No existe esa dificultad..." );
    }

    private void CheckRoundCorrectness () {
        if ( ( ( HasBullseye ) && ( HasGoalkeeper || HasSheet ) ) ||
             ( ( HasGoalkeeper ) && ( HasBullseye || HasSheet ) ) ||
             ( ( HasSheet ) && ( HasBullseye || HasGoalkeeper ) ) ) {
            throw new ArgumentException( "Ronda mal construida: las dianas, el portero y las sabanas son excluyentes" );
        }
    }

    private SabanasManager.TipoSabanaInidividual GetSheetSectorDifficulty (string ssd) {
        if ( ssd == "none" ) { return SabanasManager.TipoSabanaInidividual.NONE; }
        else if ( ssd == "blank" ) { return SabanasManager.TipoSabanaInidividual.BLANK; }
        else if ( ssd == "L" ) { return SabanasManager.TipoSabanaInidividual.L; }
        else if ( ssd == "M" ) { return SabanasManager.TipoSabanaInidividual.M; }
        else if ( ssd == "MM" ) { return SabanasManager.TipoSabanaInidividual.M; }
        else if ( ssd == "MS" ) { return SabanasManager.TipoSabanaInidividual.M; }
        else if ( ssd == "S" ) { return SabanasManager.TipoSabanaInidividual.S; }
        else if ( ssd == "SS" ) { return SabanasManager.TipoSabanaInidividual.S; }
        else {
            throw new ArgumentOutOfRangeException( "El tipo de sector de sabana ( " + ssd + " ) no se reconoce, repasa gandul!" );
        }
    }
}