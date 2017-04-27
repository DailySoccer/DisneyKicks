using UnityEngine;
using System.Collections;

public class cntMissions : MonoBehaviour {

    private const int NUM_MISSIONS_PAGE = 10; // misiones por página    

    public btnButton m_flechaIzq;
    public btnButton m_flechaDer;

    // textos para mostrar el nivel actual
    private GUIText m_LevelLabel1;
    private GUIText m_LevelLabel1_Shadow;
    private GUIText m_LevelLabel2;
    private GUIText m_LevelLabel2_Shadow;

    /// <summary>
    /// Numero de mision seleccionada actualmente
    /// </summary>
    public int currentMission { get { return m_numCurrentMission; } }
    private int m_numCurrentMission = 0;

    /// <summary>
    /// Numero de nivel seleccionado actualmente
    /// </summary>
    private int m_numCurrentLevel = 0;

    /// <summary>
    /// Nivel seleccionado actualmente
    /// </summary>
    public GameLevelMission currentLevelMission { 
        get {
            if (m_currentLevelMission == null) {
                GameLevel gl = MissionManager.instance.GetGameLevel(m_numCurrentMission);
                GameLevelMission glm = gl.GetGameLevelMission(0);
                ShowLevelMission(glm, 0); // <= se carga el primer nivel de la mision
            }
            return m_currentLevelMission; 
        } 
    }
    private GameLevelMission m_currentLevelMission;

    
    //private btnMissionButton[] m_missionButtons = new btnMissionButton[ NUM_MISSIONS_PAGE ];
    private cntMissionButton[] m_missionButtons = new cntMissionButton[NUM_MISSIONS_PAGE];

    private cntTooltipLevelSelection m_levelSelectionTooltip = null;

    #region MonoBehaviour

    void Awake () {
        m_instance = this;
    }


    /// <summary>
    /// Obtiene las referencias a los elementos de esta interfaz
    /// </summary>
    private void GetReferencias() {
        if (m_LevelLabel1 == null)
            m_LevelLabel1 = transform.FindChild("Current Level/levelLabel1").GetComponent<GUIText>();
        if (m_LevelLabel1_Shadow == null)
            m_LevelLabel1_Shadow = transform.FindChild("Current Level/levelLabel1/shadow").GetComponent<GUIText>();
        if (m_LevelLabel2 == null)
            m_LevelLabel2 = transform.FindChild("Current Level/levelLabel2").GetComponent<GUIText>();
        if (m_LevelLabel2_Shadow == null)
            m_LevelLabel2_Shadow = transform.FindChild("Current Level/levelLabel2/shadow").GetComponent<GUIText>();

        // obtener todos los botones de mision
        if (m_missionButtons[0] == null) {
            for (int i = 0; i < NUM_MISSIONS_PAGE; ++i)
                m_missionButtons[i] = transform.Find("missionButton" + (i + 1)).GetComponent<cntMissionButton>();
        }

        // tooltip de seleccion de niveles
        if (m_levelSelectionTooltip == null)
            m_levelSelectionTooltip = transform.Find("Tooltip").GetComponent<cntTooltipLevelSelection>();

        // acciones de los botones de flecha
        if (m_flechaIzq == null)
            m_flechaIzq = transform.FindChild("flecha_izq").GetComponent<btnButton>();
        m_flechaIzq.action = (_name) => { GeneralSounds_menu.instance.select(); goPage(m_numCurrentLevel - 1); };

        if (m_flechaDer == null)
            m_flechaDer = transform.FindChild("flecha_dcha").GetComponent<btnButton>();
        m_flechaDer.action = (_name) => { GeneralSounds_menu.instance.select(); goPage(m_numCurrentLevel + 1); };
    }


    void Start () {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        //m_numCurrentMission = 0;
        //m_numCurrentLevel = 0;
        //Refresh();
    }

    #endregion

    #region Singleton

    public static cntMissions instance {
        get {
            if (m_instance == null) {
                Transform tr = ifcCarrera.instance.transform.FindChild("cntMissions");
                if (tr != null) {
                    m_instance = tr.GetComponent<cntMissions>();
                    m_instance.Start();
                }
            }
            return m_instance; 
        }
    }
    private static cntMissions m_instance;

    #endregion

    public void Refresh ()
    {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        int tmp = m_numCurrentLevel;
        //m_numCurrentLevel = -1;
        goPage( tmp );
    }

    private void goPage (int _page) {
        // si se va a listar la misma pagina de logros que se esta visualizando
//        if ( m_numCurrentLevel == _page )
//            return;

        if(_page / 2 != stadium_control.estadioIndex)
        {
            stadium_control.estadioIndex = _page / 2;
            stadium_control.instance.SetScenario();
        }

        m_numCurrentLevel = _page; // guardar la pagina a mostrar

        SetLastMissionOfLevel(_page);

        UpdateArrowsState(); // comprobar si hay que mostrar la flecha paginar izda y derecha
        //UpdateMissionButtons();
        SeleccionarMisionDefault();
        UpdateLevelLabel();
        
    }

    private void UpdateArrowsState () {
        m_flechaIzq.gameObject.SetActive( m_numCurrentLevel > 0 );
        bool flechaDerecha = m_numCurrentLevel < (MissionManager.instance.numNiveles - 1 );
        flechaDerecha = flechaDerecha && (m_numCurrentLevel < MissionManager.instance.GetMissionLevel(MissionManager.instance.GetGameLevelMission(Interfaz.ultimaMisionDesbloqueada)).levelNumber);
        m_flechaDer.gameObject.SetActive( flechaDerecha );
    }

    private void UpdateLevelLabel () {
        char[] separador = {' '};
        string[] partesNombreNivel = MissionManager.instance.GetGameLevel(m_numCurrentLevel).LevelName.Split(separador);

        m_LevelLabel1.text = LocalizacionManager.instance.GetTexto(12).ToUpper() + " " + int.Parse(partesNombreNivel[1]);
        m_LevelLabel1_Shadow.text = m_LevelLabel1.text;
        m_LevelLabel2.text = m_LevelLabel1.text;
        m_LevelLabel2_Shadow.text = m_LevelLabel1.text;
    }

    private void SetLastMissionOfLevel (int _levelNumber)
    {
        if(_levelNumber >= MissionManager.instance.GetMissionLevel(MissionManager.instance.GetGameLevelMission(Interfaz.ultimaMisionDesbloqueada)).levelNumber)
        {
            m_numCurrentMission = Interfaz.ultimaMisionDesbloqueada % MissionManager.instance.GetGameLevel(_levelNumber).numMissions;
        }
        else
        {
            Debug.Log(">>> _levelNumber: " + _levelNumber);
            Debug.Log(">>> MissionManager.instance: " + MissionManager.instance);
            Debug.Log(">>> MissionManager.instance.GetGameLevel(_levelNumber): " + MissionManager.instance.GetGameLevel(_levelNumber));

            m_numCurrentMission = MissionManager.instance.GetGameLevel(_levelNumber).numMissions - 1;
        }
    }

    private void UpdateMissionButtons () {
        GameLevel gameLevel = MissionManager.instance.GetGameLevel(m_numCurrentLevel);

        // actualizar la pagina de misiones
        for (int i = 0; i < m_missionButtons.Length; ++i) {
            GameLevelMission glm = gameLevel.GetGameLevelMission(i);

            bool misionDesbloqueada = (m_numCurrentLevel * 10 + i) <= Interfaz.ultimaMisionDesbloqueada;
            m_missionButtons[i].Init(glm.Index, glm, misionDesbloqueada);
        }

        // actualizar la pagina de misiones
        /*
        GameLevel gameLevel = MissionManager.instance.GetGameLevel(m_numCurrentLevel);
        for ( int i = 0; i < NUM_MISSIONS_PAGE; ++i ) {
            if ( i < gameLevel.numMissions && Interfaz.ultimaMisionDesbloqueada >= gameLevel.GetGameLevelMission(i).Index) {
                btnMissionButton bm = m_missionButtons[ i ];
                GameLevelMission glm = gameLevel.GetGameLevelMission(i);
                int numBoton = i;
                bm.Init( ( i + 1 ).ToString(), glm.MissionGameMode,
                         glm.MissionUnlocked, glm.numEstrellas,
                         (_name) => {
                             ShowLevelMission(glm);
                             MostrarBotonSeleccionado(numBoton);
                         } );
                m_missionButtons[ i ].gameObject.SetActive( true );
            }
            else {                
                m_missionButtons[ i ].gameObject.SetActive( false );
            }
        }
         */
    }


    /// <summary>
    /// Muestra la informacion asociada al nivel recibido como parametro
    /// </summary>
    /// <param name="_glm"></param>
    /// <param name="_numBoton">Numero de boton asociado a esta mision</param>
    public void ShowLevelMission(GameLevelMission _glm, int _numBoton) {
        GetReferencias();

        // guardar la mision como seleccionada
        m_currentLevelMission = _glm;

        // mostrar la info de la mision y actualizar el estado del boton "jugar"
        bool misionDesbloqueada = _numBoton <= Interfaz.ultimaMisionDesbloqueada;
        m_levelSelectionTooltip.SetInfo(_glm, misionDesbloqueada);

        Debug.Log(">>> Nombre mision = " + _glm.MissionFileName);

        // hacer que se muestre el modelo del portero o del jugador en la interfaz
        Interfaz.instance.RefrescarModelosJugadores(_glm.MissionGameMode == GameMode.Shooter, _glm.MissionGameMode == GameMode.GoalKeeper);
    }


    /// <summary>
    /// Muestra la informacion de la primera mision
    /// </summary>
    public void SeleccionarMisionDefault() {
        // obtiene las referencias a los elementos de esta interfaz
        GetReferencias();

        // seleccionar la primera mision y actualizar los botones
        //m_numCurrentMission = 0;
        SetLastMissionOfLevel(m_numCurrentLevel);
        UpdateMissionButtons();

        // dejar seleccionada la primera levelmission de la pagina
        GameLevel gameLevel = MissionManager.instance.GetGameLevel(m_numCurrentLevel);
        ShowLevelMission(gameLevel.GetGameLevelMission(m_numCurrentMission), m_numCurrentMission);
        MostrarBotonSeleccionado(m_numCurrentMission);
    }

    public void SeleccionarUltimaMisionAndNivel()
    {
        // obtiene las referencias a los elementos de esta interfaz
        GetReferencias();

        m_numCurrentLevel = MissionManager.instance.GetMissionLevel(MissionManager.instance.GetGameLevelMission(Interfaz.ultimaMisionDesbloqueada)).levelNumber;
        Refresh();
        SeleccionarMisionDefault();
    }

    /// <summary>
    /// Resalta el boton "_numSeleccionado" como seleccionado y el resto no
    /// </summary>
    /// <param name="_numBoton"></param>
    public void MostrarBotonSeleccionado(int _numBoton) {
        // obtiene las referencias a los elementos de esta interfaz
        //GetReferencias();

        // resaltar el boton de mision que corresponda
        for (int i = 0; i < m_missionButtons.Length; ++i) {
            if (i == _numBoton)
                m_missionButtons[i].Select();
            else
                m_missionButtons[i].Deselect();
        }
    }


}
