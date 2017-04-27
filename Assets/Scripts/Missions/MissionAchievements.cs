using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionAchievementBuilder {

    public static MissionAchievement CreateAchievement (Dictionary<string, object> achievementData) {
        
        string missionAchievementType = (string)achievementData[ "Type" ];

        MissionAchievement newMissionAchievement = null;        
        if ( missionAchievementType == "score" ) { newMissionAchievement = new ScoreAchievement(); }
        else if ( missionAchievementType == "shooterWin" ) { newMissionAchievement = new ShooterWinAchievement(); }
        else if ( missionAchievementType == "shooterEffectBonus" ) { newMissionAchievement = new ShooterEffectBonusAchievement(); }
        else if ( missionAchievementType == "goalkeeperWin" ) { newMissionAchievement = new GoalkeeperWinAchievement(); }
        else if ( missionAchievementType == "perfect" ) { newMissionAchievement = new PerfectAchievement(); }
        else if ( missionAchievementType == "life" ) { newMissionAchievement = new LifeAchievement(); }
        else {
            throw new ArgumentOutOfRangeException( "El tipo de objetivo ( " + missionAchievementType + " ) no existe, revisa madafaka" );
        }

        newMissionAchievement.LoadData( achievementData );
        return newMissionAchievement;
    }
}

public class MissionAchievement {

    public int Code { get; protected set; }
    public string AchivementType { get; protected set; }
    public string DescriptionID { get; protected set; }

    public bool frozen = false;

    protected bool m_achieved = false;

    /// <summary>
    /// Si es true, el achievement sera de tipo racha
    /// </summary>
    protected bool _isStreak = false;

    public virtual void LoadData (Dictionary<string, object> achievementData) {
        Code = (int)achievementData[ "Code" ] - 1;
        AchivementType = (string)achievementData[ "Type" ];
        //esto desactiva la funcionalidad de cargar la descripcion de la mision desde el archivo, porque choca con localizacion
        /*if(achievementData.ContainsKey("DescriptionID"))
        {
            DescriptionID = (string)achievementData[ "DescriptionID" ];
        }
        else
        {
            DescriptionID = null;
        }*/
    }

    public virtual bool IsAchieved() {
        return m_achieved;
    }


    public void SetAchieved(bool _achieved) {
        m_achieved = _achieved;
    }


    protected void ReadStreakParam (Dictionary<string, object> achievementData) {
        if ( achievementData.ContainsKey( "isStreak" ) ) {
            _isStreak = (bool)achievementData[ "isStreak" ];
        } 
    }
}

public class ScoreAchievement : MissionAchievement {    

    private int _scoreTarget = 0;

    public override void LoadData (Dictionary<string, object> achievementData) {
        base.LoadData( achievementData );

        if ( achievementData.ContainsKey( "scoreTarget" ) ) {
            _scoreTarget = (int)achievementData[ "scoreTarget" ];
            if(string.IsNullOrEmpty(DescriptionID))
            {
                //DescriptionID = "Consigue " + _scoreTarget.ToString() + " punto"  + ((_scoreTarget > 1) ? "s" : "");
                DescriptionID = string.Format(LocalizacionManager.instance.GetTexto(74),
                                    _scoreTarget.ToString(),
                                    ((_scoreTarget > 1) ? LocalizacionManager.instance.GetTexto(73) : LocalizacionManager.instance.GetTexto(72)));
            }
        }
        else {
            throw new ArgumentException( "ScoreAchievement: No se ha encontrado el parametro scoreTarget" );
        }
    }

    public override bool IsAchieved () {
        // si el objetivo de mision ya ha sido conseguido previamente
        if (m_achieved || frozen) return m_achieved;

        // comprobar si la escena actual es de menu => devolver el valor almacenado
        if (Application.loadedLevelName == "Menus")
            return m_achieved;
        else {
            // calcular si se ha conseguido el logro
            m_achieved = (ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Points >= _scoreTarget);
            return m_achieved;
        }
    }
}

public class PerfectAchievement : MissionAchievement {

    private int _perfectTarget = 0;

    public override void LoadData (Dictionary<string, object> achievementData) {
        base.LoadData( achievementData );

        if ( achievementData.ContainsKey( "perfectTarget" ) ) {
            _perfectTarget = (int)achievementData[ "perfectTarget" ];
        }
        else {
            throw new ArgumentException( "PerfectAchievement: No se ha encontrado el parametro perfectTarget" );
        }

        ReadStreakParam( achievementData );

        if(string.IsNullOrEmpty(DescriptionID))
        {
            //DescriptionID = (_isStreak ? "Encadena " : "Logra ") + _perfectTarget + " Perfecto" + ((_perfectTarget > 1) ? "s" : "");
            DescriptionID = string.Format("{1} {2}{0}",
                                (_isStreak ? LocalizacionManager.instance.GetTexto(292) : ""),
                                _perfectTarget,
                                ((_perfectTarget > 1) ? LocalizacionManager.instance.GetTexto(77) : LocalizacionManager.instance.GetTexto(76)));
        }
    }

    public override bool IsAchieved () {
        // si el objetivo de mision ya ha sido conseguido previamente
        if (m_achieved || frozen) return m_achieved;

        // comprobar si la escena actual es de menu => devolver el valor almacenado
        if (Application.loadedLevelName == "Menus")
            return m_achieved;
        else {
            // calcular si se ha conseguido el logro
            if (_isStreak) {
                m_achieved = (MissionStats.Instance.GetStat("perfects").GetMaxStreak() >= _perfectTarget);
            } else {
                m_achieved = (MissionStats.Instance.GetStat("perfects").GetTotal() >= _perfectTarget);
            }
            return m_achieved;
        }
    }
}

public class ShooterWinAchievement : MissionAchievement {

    public enum ShooterWinElement {
        Generic,
        Target,
        Goalkeeper,
        Sheet,
    }

    private int _winTarget = 0;
    private ShooterWinElement _winElement = ShooterWinElement.Generic;

    public override void LoadData (Dictionary<string, object> achievementData) {
        base.LoadData( achievementData );

        if ( achievementData.ContainsKey( "winTarget" ) ) {
            _winTarget = (int)achievementData[ "winTarget" ];
        }
        else {
            throw new ArgumentException( "ShooterWinAchievement: No se ha encontrado el parametro winTarget" );
        }

        ReadStreakParam( achievementData );

        if ( achievementData.ContainsKey( "winAgainst" ) ) {
            _winElement = GetElement( (string)achievementData[ "winAgainst" ] );
        }
        else {
            _winElement = ShooterWinElement.Generic;
        }

        if(_winElement == ShooterWinElement.Generic)
        {
            if(string.IsNullOrEmpty(DescriptionID))
            {
                //DescriptionID = (_isStreak ? "Encadena " : "Consigue ") + _winTarget + " aciertos";
                DescriptionID = String.Format(LocalizacionManager.instance.GetTexto(50),
                    (_isStreak ? "" : LocalizacionManager.instance.GetTexto(292)),
                    _winTarget);
            }
        }
        else
        {
            string element = "";
            switch(_winElement)
            {
                case ShooterWinElement.Goalkeeper: element = LocalizacionManager.instance.GetTexto(15); break;
                case ShooterWinElement.Sheet: element = LocalizacionManager.instance.GetTexto(56); break;
                case ShooterWinElement.Target: element = LocalizacionManager.instance.GetTexto(57); break;
            }
            if(string.IsNullOrEmpty(DescriptionID))
            {
                //DescriptionID = (_isStreak ? "Encadena " : "Acierta ") + _winTarget + " tiro" + ((_winTarget > 1) ? "s" : "")+" a " + element;
                DescriptionID = String.Format(LocalizacionManager.instance.GetTexto(53),
                                    (_isStreak ? "" : LocalizacionManager.instance.GetTexto(292)),
                                    _winTarget,
                                    ((_winTarget > 1) ? LocalizacionManager.instance.GetTexto(54) : LocalizacionManager.instance.GetTexto(55)),
                                    element
                                );
            }
        }
    }

    public override bool IsAchieved () {
        // si el objetivo de mision ya ha sido conseguido previamente
        if (m_achieved || frozen) return m_achieved;

        // comprobar si la escena actual es de menu => devolver el valor almacenado
        if (Application.loadedLevelName == "Menus")
            return m_achieved;
        else {
            // calcular si se ha conseguido el logro
            bool bReturn = false;

            switch (_winElement) {
                case ShooterWinElement.Generic:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("shooterWinsGeneric").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("shooterWinsGeneric").GetTotal() >= _winTarget);
                    break;
                case ShooterWinElement.Target:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("shooterWinsTarget").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("shooterWinsTarget").GetTotal() >= _winTarget);
                    break;
                case ShooterWinElement.Goalkeeper:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("shooterWinsGoalkeeper").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("shooterWinsGoalkeeper").GetTotal() >= _winTarget);
                    break;
                case ShooterWinElement.Sheet:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("shooterWinsSheet").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("shooterWinsSheet").GetTotal() >= _winTarget);
                    break;
            }

            m_achieved = bReturn;
            return m_achieved;
        }
    }

    private ShooterWinElement GetElement (string elementName) {
        if ( elementName == "target" ) { return ShooterWinElement.Target; }
        else if ( elementName == "goalkeeper" ) { return ShooterWinElement.Goalkeeper; }
        else if ( elementName == "sheet" ) { return ShooterWinElement.Sheet; }
        else {
            throw new ArgumentOutOfRangeException( "El tipo de elemento ( " + elementName + " ) no existe, repasa melon!" );
        }
    }
}

public class ShooterEffectBonusAchievement : MissionAchievement {

    private int _bonusTarget = 0;

    private ScoreManager.EffectBonus _bonusType = ScoreManager.EffectBonus.NONE;
    
    public override void LoadData (Dictionary<string, object> achievementData) {
        base.LoadData( achievementData );

        if ( achievementData.ContainsKey( "bonusTarget" ) ) {
            _bonusTarget = (int)achievementData[ "bonusTarget" ];
        }
        else {
            throw new ArgumentException( "ShooterEffectBonusAchievement: No se ha encontrado el parametro bonusTarget" );
        }

        ReadStreakParam( achievementData );

        if ( achievementData.ContainsKey( "bonusType" ) ) {
            _bonusType = GetBonusType( (string)achievementData[ "bonusType" ] );
        }
        else {
            _bonusType = ScoreManager.EffectBonus.NONE;
        }
        string tipoEfecto = "";
        switch(_bonusType)
        {
            case ScoreManager.EffectBonus.NONE: tipoEfecto = ""; break;
            case ScoreManager.EffectBonus.LOW: tipoEfecto = LocalizacionManager.instance.GetTexto(60); break;
            case ScoreManager.EffectBonus.MEDIUM: tipoEfecto = LocalizacionManager.instance.GetTexto(59); break;
            case ScoreManager.EffectBonus.HIGH: tipoEfecto = LocalizacionManager.instance.GetTexto(58); break;
        }

        if(string.IsNullOrEmpty(DescriptionID))
        {
            //DescriptionID = (_isStreak ? "Encadena " : "Logra ") + _bonusTarget + " bonus por efecto " + tipoEfecto;
            DescriptionID = string.Format(LocalizacionManager.instance.GetTexto(63),
                            (_isStreak ? LocalizacionManager.instance.GetTexto(292) : ""),
                            _bonusTarget,
                            tipoEfecto,
                            ((_bonusTarget > 1) ? LocalizacionManager.instance.GetTexto(62) : LocalizacionManager.instance.GetTexto(61)));
        }
    }

    public override bool IsAchieved () {
        // si el objetivo de mision ya ha sido conseguido previamente
        if (m_achieved || frozen) return m_achieved;

        // comprobar si la escena actual es de menu => devolver el valor almacenado
        if (Application.loadedLevelName == "Menus")
            return m_achieved;
        else {
            // calcular si se ha conseguido el logro
            bool bReturn = false;

            switch (_bonusType) {
                case ScoreManager.EffectBonus.NONE:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("effectBonusGeneric").GetMaxStreak() >= _bonusTarget) :
                        (MissionStats.Instance.GetStat("effectBonusGeneric").GetTotal() >= _bonusTarget);
                    break;
                case ScoreManager.EffectBonus.LOW:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("effectBonusLow").GetMaxStreak() >= _bonusTarget) :
                        (MissionStats.Instance.GetStat("effectBonusLow").GetTotal() >= _bonusTarget);
                    break;
                case ScoreManager.EffectBonus.MEDIUM:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("effectBonusMedium").GetMaxStreak() >= _bonusTarget) :
                        (MissionStats.Instance.GetStat("effectBonusMedium").GetTotal() >= _bonusTarget);
                    break;
                case ScoreManager.EffectBonus.HIGH:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("effectBonusHigh").GetMaxStreak() >= _bonusTarget) :
                        (MissionStats.Instance.GetStat("effectBonusHigh").GetTotal() >= _bonusTarget);
                    break;
            }

            m_achieved = bReturn;
            return m_achieved;
        }
    }

    private ScoreManager.EffectBonus GetBonusType (string bonusType) {
        if ( bonusType == "low" ) { return ScoreManager.EffectBonus.LOW; }
        else if ( bonusType == "medium" ) { return ScoreManager.EffectBonus.MEDIUM; }
        else if ( bonusType == "high" ) { return ScoreManager.EffectBonus.HIGH; }
        else {
            throw new ArgumentOutOfRangeException( "El tipo de elemento ( " + bonusType + " ) no existe, repasa melon!" );
        }
    }
}

public class GoalkeeperWinAchievement : MissionAchievement {

    public enum GoalkeeperWinType {
        Generic,
        BallCatched,
        BallSaved,
        GoalsReceived,
    }

    private int _winTarget = 0;

    private GoalkeeperWinType _winType = GoalkeeperWinType.Generic;
    
    public override void LoadData (Dictionary<string, object> achievementData) {
        base.LoadData( achievementData );

        if ( achievementData.ContainsKey( "winTarget" ) ) {
            _winTarget = (int)achievementData[ "winTarget" ];
        }
        else {
            throw new ArgumentException( "GoalkeeperWinAchievement: No se ha encontrado el parametro winTarget" );
        }

        ReadStreakParam( achievementData );

        if ( achievementData.ContainsKey( "winType" ) ) {
            _winType = GetWinType( (string)achievementData[ "winType" ] );
        }
        else {
            _winType = GoalkeeperWinType.Generic;
        }

        if(_winType == GoalkeeperWinType.GoalsReceived)
        {
            if(string.IsNullOrEmpty(DescriptionID))
            {
                DescriptionID = LocalizacionManager.instance.GetTexto(75);
            }
        }
        else
        {
            if(string.IsNullOrEmpty(DescriptionID))
            {
                //DescriptionID = (_isStreak ? "Encadena " : "Realiza ") + _winTarget + ((_winType == GoalkeeperWinType.BallSaved || _winType == GoalkeeperWinType.Generic) ? " parada" : " Perfecto") + ((_winTarget > 1) ? "s" : "");
                if(_winType == GoalkeeperWinType.Generic)
                {
					DescriptionID = string.Format(LocalizacionManager.instance.GetTexto(293), _winTarget, LocalizacionManager.instance.GetTexto(292));
                }
                else
                {
                    DescriptionID = string.Format(LocalizacionManager.instance.GetTexto(68),
                                    (_isStreak ? LocalizacionManager.instance.GetTexto(292) : ""),
                                    _winTarget,
                                    ((_winType == GoalkeeperWinType.BallSaved) ? "" : (LocalizacionManager.instance.GetTexto(67))),
                                    ((_winTarget > 1) ? LocalizacionManager.instance.GetTexto(66) : LocalizacionManager.instance.GetTexto(65)));
                }
            }
        }
    }

    public override bool IsAchieved () {
        // si el objetivo de mision ya ha sido conseguido previamente
        if (m_achieved || frozen) return m_achieved;

        // comprobar si la escena actual es de menu => devolver el valor almacenado
        if (Application.loadedLevelName == "Menus")
            return m_achieved;
        else {
            // calcular si se ha conseguido el logro
            bool bReturn = false;

            switch (_winType) {
                case GoalkeeperWinType.Generic:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("goalkeeperWinsGeneric").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("goalkeeperWinsGeneric").GetTotal() >= _winTarget);
                break;
                case GoalkeeperWinType.BallCatched:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("goalkeeperCatched").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("goalkeeperCatched").GetTotal() >= _winTarget);
                    break;
                case GoalkeeperWinType.BallSaved:
                    bReturn = (this._isStreak) ?
                        (MissionStats.Instance.GetStat("goalkeeperSaved").GetMaxStreak() >= _winTarget) :
                        (MissionStats.Instance.GetStat("goalkeeperSaved").GetTotal() >= _winTarget);
                    break;
                case GoalkeeperWinType.GoalsReceived:
                    bReturn = ((MissionStats.Instance.GetStat("goalkeeperReceivedGoals").GetTotal() <= _winTarget) && MissionManager.instance.GetMission().IsMissionFinished());
                    break;
            }

            m_achieved = bReturn;
            return m_achieved;
        }
    }

    private GoalkeeperWinType GetWinType (string winType) {
        if ( winType == "catched" ) { return GoalkeeperWinType.BallCatched; }
        else if ( winType == "generic" ) { return GoalkeeperWinType.Generic; }
        else if ( winType == "saved" ) { return GoalkeeperWinType.BallSaved; }
        else if ( winType == "received" ) { return GoalkeeperWinType.GoalsReceived; }
        else {
            throw new ArgumentOutOfRangeException( "El tipo de elemento ( " + winType + " ) no existe, repasa melon!" );
        }
    }
}

public class LifeAchievement : MissionAchievement {

    private int _lifeTarget = 0;

    public override void LoadData (Dictionary<string, object> achievementData) {
        base.LoadData( achievementData );

        if ( achievementData.ContainsKey( "lifeTarget" ) ) {
            _lifeTarget = (int)achievementData[ "lifeTarget" ];
        }
        else {
            throw new ArgumentException( "LifeAchievement: No se ha encontrado el parametro lifeTarget" );
        }
        if(string.IsNullOrEmpty(DescriptionID))
        {
            //DescriptionID = "Termina con " + _lifeTarget.ToString() + " vida" + ((_lifeTarget > 1) ? "s" : "");
            DescriptionID = string.Format(LocalizacionManager.instance.GetTexto(71),
                                _lifeTarget.ToString(),
                                ((_lifeTarget > 1) ? LocalizacionManager.instance.GetTexto(70) : LocalizacionManager.instance.GetTexto(69)));
        }
    }

    public override bool IsAchieved () {
        // si el objetivo de mision ya ha sido conseguido previamente
        if (m_achieved || frozen) return m_achieved;

        // comprobar si la escena actual es de menu => devolver el valor almacenado
        if (Application.loadedLevelName == "Menus")
            return m_achieved;
        else {
            // calcular si se ha conseguido el logro
            // Attempts = vidas
            
            m_achieved = ServiceLocator.Request<IPlayerService>().IsGameOver() && (ServiceLocator.Request<IPlayerService>().GetPlayerInfo().Attempts >= _lifeTarget);
            return m_achieved;
        }
    }
}