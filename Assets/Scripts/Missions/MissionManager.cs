using UnityEngine;
using System;
using System.Collections.Generic;


/// <summary>
/// Clase para gestionar las misiones del juego
/// </summary>
public class MissionManager {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Nombre del fichero desde el que se cargan las misiones
    /// </summary>
    private const string _levelListFileName = "BK_Missions";
  

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------

    #region Singleton

    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static MissionManager instance {
        get {
            if (m_instance == null) {
                m_instance = new MissionManager();
            }
            return m_instance;
        }
    }

    private static MissionManager m_instance = null;

    #endregion

    // lista de niveles
    private List<GameLevel> m_levels;

    /// <summary>
    /// Numero de niveles
    /// </summary>
    public int numNiveles { get { return (m_levels == null) ? 0 : m_levels.Count; } }

    /// <summary>
    /// Devuelve una lista construida con todas las misiones
    /// </summary>
    public List<GameLevelMission> ListaLevelMissions {
        get {
            List<GameLevelMission> misiones = new List<GameLevelMission>();
            for(int i = 0; i < m_levels.Count ; ++i)
            {
                misiones.AddRange(m_levels[i].ListaMisiones);
            }
            return  misiones;
        }
    }

    // mision cargada actualmente
    private Mission m_currentMission = null;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR E INICIALIZACION  -------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Constructs a new Mission List given a string. That string is expected to contain
    /// a JSON sctructure defining the Mission List object.
    /// </summary>
    private MissionManager() {
        /*System.IO.StreamReader levelListFile = new System.IO.StreamReader(
                Application.dataPath + "/Missions/" + _levelListFileName + ".json",
                System.Text.Encoding.UTF8 );
        string levelListData = levelListFile.ReadToEnd();

        levelListFile.Close();*/

        TextAsset textAsset = (TextAsset) Resources.Load("Missions/" + _levelListFileName, typeof(TextAsset));
        if (textAsset == null)
            Debug.LogWarning(">>> No se ha podido cargar el fichero de descripcion de misiones: " + _levelListFileName);

        // crear la lista de niveles
        m_levels = new List<GameLevel>();
        var gameLevelsListJson = (Dictionary<string, object>) MiniJSON.Json.Deserialize(textAsset.text);
        var levelsList = (List<object>) gameLevelsListJson["Levels"];
        if (levelsList != null) {
            foreach (var level in levelsList) {
                m_levels.Add(new GameLevel((Dictionary<string, object>) level));
            }
        }
    }



    public static string GetMissionData(string missionName) {
        /*
        System.IO.StreamReader missionFile = new System.IO.StreamReader(
                Application.dataPath + "/Missions/" + missionName + ".json",
                System.Text.Encoding.UTF8 );
        string missionData = missionFile.ReadToEnd();

        missionFile.Close();
         */

        TextAsset textAsset = (TextAsset) Resources.Load("Missions/" + missionName, typeof(TextAsset));
        if (textAsset == null) {
            Debug.LogWarning(">>> No se ha podido cargar el fichero de la mission: " + missionName);
            return null;
        }

        return textAsset.text;
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Devuelve el GameLevel que ocupa la posicion "_posicion"
    /// </summary>
    /// <param name="_posicion"></param>
    /// <returns></returns>
    public GameLevel GetGameLevel(int _posicion) {
        if (m_levels == null || _posicion < 0 || _posicion >= m_levels.Count)
            return null;
        else
            return m_levels[_posicion];
    }


    /// <summary>
    /// Devuelve un GameLevelMission por su nombre con el que lo identifican los WebServices
    /// </summary>
    /// <param name="_missionId"></param>
    /// <returns></returns>
    public GameLevelMission GetGameLevelMission(string _missionId) {
        if (m_levels != null)
            for (int i = 0; i < m_levels.Count; ++i) {
                // comprobar si este nivel contiene la mision que se esta buscando
                GameLevelMission mision = m_levels[i].GetGameLevelMission(_missionId);
                if (mision != null)
                    return mision;
            }

        Debug.LogWarning(">>> No se ha encontrado una mision con id=" + _missionId);
        return null;
    }

    /// <summary>
    /// Devuelve un GameLevelMission por su indice
    /// </summary>
    /// <param name="_missionIndex"></param>
    /// <returns></returns>
    public GameLevelMission GetGameLevelMission(int _missionIndex) {
        if(_missionIndex >= ListaLevelMissions.Count) _missionIndex = ListaLevelMissions.Count - 1;
        if(_missionIndex < 0) _missionIndex = 0;
        foreach(GameLevelMission gm in ListaLevelMissions)
        {
            if(gm.Index == _missionIndex)
            {
                return gm;
            }
        }

        Debug.LogWarning(">>> No se ha encontrado una mision con indice=" + _missionIndex);
        return null;
    }


    /// <summary>
    /// Establece cual es la mision actual que hay en juego
    /// </summary>
    /// <param name="missionName"></param>
    /// <returns></returns>
    public bool SetCurrentMission(string missionName, int index) {
        // obtener la mision actual
        m_currentMission = new Mission(GetMissionData(missionName), index);

        // Reset the Mission Stats
        MissionStats.Instance.Reset();

        return true;
    }

    /// <summary>
    /// Reinicia la mision actual
    /// </summary>
    public void ClearCurrentMission() {
        // obtener la mision actual
        m_currentMission = null;
        // Reset the Mission Stats
        if(MissionStats.Instance != null)
        {
            MissionStats.Instance.Reset();
        }
    }


    /// <summary>
    /// Comprueba si hay mision actual
    /// </summary>
    /// <returns></returns>
    public bool HasCurrentMission () {
        return ( m_currentMission != null );
    }

    /// <summary>
    /// Devuelve la mision actual
    /// </summary>
    /// <returns></returns>
    public Mission GetMission() {
        if (m_currentMission != null) {
            return m_currentMission;
        } else {
            throw new NullReferenceException("No hay ninguna mision cargada");
        }
    }

    public GameLevel GetMissionLevel( GameLevelMission _mission)
    {
        foreach(GameLevel level in m_levels)
        {
            if(level.ListaMisiones.Contains(_mission))
            {
                return level;
            }
        }
        return null;
    }

}


/// <summary>
/// Informacion de una mision de un nivel
/// </summary>
public class GameLevelMission {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public string MissionFileName { get; protected set; }

    public string MissionName { get; protected set; }

    public GameMode MissionGameMode { get; protected set; }

    public bool MissionUnlocked;

    public int Index;

    /// <summary>
    /// Numero de estrellas conseguidas en esta mision
    /// </summary>
    public int numEstrellas { 
        get {
            int _numEstrellas = 0;
            if (LevelMision != null && LevelMision.Achievements != null) {
                for (int i = 0; i < LevelMision.Achievements.Count; ++i) {
                    if (LevelMision.Achievements[i].IsAchieved())
                        ++_numEstrellas;
                }
            }
            return _numEstrellas; 
        } 
    }

    private Mission LevelMision;

    public void Freeze()
    {
        LevelMision.frozenAchievements = true;
    }

    public void UnFreeze()
    {
        LevelMision.frozenAchievements = false;
    }


    public int GetRoundsCount()
    {
        return LevelMision.RoundsCount;
    }


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public GameLevelMission(string missionName, int _index) {
        Index = _index;
        MissionFileName = missionName;

        string contenidoFichero = MissionManager.GetMissionData(missionName);
        if (contenidoFichero == null) {
            Debug.LogError(">>> No se ha podido leer el contenido de la mision " + missionName);
        } else {
            // TODO
            // - Esto tendría que petar en caso de no encontrar un fichero de mision...
            // - Es necesario cargar y descodificar toda la info de las misiones? Optimizar
            // - Si no encuentra un fichero de misión, le pongo como misión bloqueada, esto lo tendremos que sacar de algún lado
            //try {
            LevelMision = new Mission(MissionManager.GetMissionData(missionName), _index);
            MissionName = LevelMision.Name;
            MissionGameMode = LevelMision.PlayerType;
            MissionUnlocked = true;
            //}
            /*catch ( Exception e ) {
                Debug.LogError("Excepcion al cargar la mision " + missionName + " en GameLevelMission(): " + e.ToString());

                LevelMision = null;
                MissionName = "Mission";
                MissionGameMode = GameMode.Shooter;
                MissionUnlocked = false;
                MissionStars = 0;
            } */
        }
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Devuelve los objetivos de mision
    /// </summary>
    /// <returns></returns>
    public List<MissionAchievement> GetAchievements() {
        if (LevelMision != null) {
            return LevelMision.Achievements;
        }
        return null;
    }

}


/// <summary>
/// Informacion del nivel
/// </summary>
public class GameLevel {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public string LevelName { get; protected set; }

    public int levelNumber { get; protected set; }

    private List<GameLevelMission> m_levelMissions;

    public  List<GameLevelMission> ListaMisiones { get{ return m_levelMissions; } set{} }

    /// <summary>
    /// Numero de misiones de este nivel
    /// </summary>
    public int numMissions { get { return (m_levelMissions == null) ? 0 : m_levelMissions.Count; } }


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public GameLevel(Dictionary<string, object> gameLevelData) {

        LevelName = (string) gameLevelData["LevelName"];

        levelNumber = (int) gameLevelData["LevelNumber"];

        var gameLevelMissionList = (List<object>) gameLevelData["LevelMissions"];
        m_levelMissions = new List<GameLevelMission>();
        for (int i = 0; i < gameLevelMissionList.Count; ++i) {
            m_levelMissions.Add(new GameLevelMission((string) gameLevelMissionList[i], (levelNumber * 10) + i));
        }
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Devuelve la mision del nivel que ocupa la posicion "_posicion"
    /// </summary>
    /// <param name="_posicion"></param>
    /// <returns></returns>
    public GameLevelMission GetGameLevelMission(int _posicion) {
        if (m_levelMissions == null || _posicion < 0 || _posicion >= m_levelMissions.Count)
            return null;
        else
            return m_levelMissions[_posicion];
    }

    /// <summary>
    /// Devuelbe la mision del nivel por el nombre que la identifica contra los webservices
    /// </summary>
    /// <param name="_missionName"></param>
    /// <returns></returns>
    public GameLevelMission GetGameLevelMission(string _missionName) {
        if (m_levelMissions == null)
            return null;
        else {
            for (int i = 0; i < m_levelMissions.Count; ++i) {
                if (m_levelMissions[i].MissionName == _missionName)
                    return m_levelMissions[i];
            }
        }

        return null;
    }

}
