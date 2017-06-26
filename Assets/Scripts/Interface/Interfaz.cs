#define DEBUG_KICKS


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ifcBase : MonoBehaviour
{
    public static bool blocked = false; //ñapeja, sorry //TODO hacerlo mejor
    public static float scaleFactor = 1f;
    public static float scaleWFactor = 1f;
    public static float origFactor = -1f;
    public static float origWFactor = -1f;
    public static int origResolution = -1;
    public static int origWResolution = -1;
    public static ifcBase activeIface;

    public btnButton.guiAction m_backMethod;

    public float currentScale = 1f;
    public float currentWScale = 1f;

    public static void ScaleGUIOBject(GameObject guiObject, float _prevScale = 1f)
    {
        GUIText text = guiObject.GetComponent<GUIText>();
        if (text != null)
        {
            text.pixelOffset *= scaleFactor * (1f / _prevScale);
            text.fontSize = Mathf.RoundToInt(text.fontSize * scaleFactor * (1f / _prevScale));
            txtText gText = guiObject.GetComponent<txtText>();
            if (gText != null)
            {
                gText.baseWidth = (int)(gText.baseWidth * scaleFactor * (1f / _prevScale));
                gText.Fix();
            }
        }

        GUITexture texture = guiObject.GetComponent<GUITexture>();
        if (texture != null)
        {
            bool fullWidth = guiObject.GetComponent<ifcScaleWidth>() != null;
            Rect tempRect = texture.pixelInset;
            tempRect.xMin *= fullWidth ? (scaleWFactor * 1f / _prevScale) : (scaleFactor * 1f / _prevScale);
            tempRect.yMin *= scaleFactor * 1f / _prevScale;
            tempRect.xMax *= fullWidth ? (scaleWFactor * 1f / _prevScale) : (scaleFactor * 1f / _prevScale);
            tempRect.yMax *= scaleFactor * 1f / _prevScale;
            texture.pixelInset = tempRect;
        }
    }

    public static void Scale(GameObject _interface, float _prevScale = 1f)
    {
        foreach (Transform t in _interface.transform)
        {
            if (t.GetComponent<ifcBase>() != null)
            {
                Scale(t.gameObject, t.GetComponent<ifcBase>().currentScale);
                t.GetComponent<ifcBase>().currentScale = scaleFactor;
                t.GetComponent<ifcBase>().currentWScale = scaleWFactor;
            }
            else
            {
                if (t.childCount > 0) Scale(t.gameObject, _prevScale);
                ScaleGUIOBject(t.gameObject, _prevScale);
            }
        }
        if (_interface.GetComponent<ifcBase>() != null)
        {
            _interface.GetComponent<ifcBase>().currentScale = scaleFactor;
            _interface.GetComponent<ifcBase>().currentWScale = scaleWFactor;
        }
    }

    public static Rect ScaleRect(Rect _rect, float _scale)
    {
        return ScaleRect(_rect, _scale, _scale);
    }

    public static Rect ScaleRect(Rect _rect, float _scaleX, float _scaleY)
    {
        Rect _result = new Rect();
        _result.xMin = _rect.xMin * _scaleX;
        _result.xMax = _rect.xMax * _scaleX;
        _result.yMin = _rect.yMin * _scaleY;
        _result.yMax = _rect.yMax * _scaleY;
        return _result;
    }

    public GameObject getComponentByName(string _name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == _name)
                return child.gameObject;
        }
        return null;
    }
}

public class Interfaz : MonoBehaviour
{

    public struct progress
    {
        public int goals;
        public int goalsStopped;
        public int throwOut;
        public int targets;
        public int totalPoints;
        public int deflected;
        public int record;
        public int perfects;
    };

    public enum ResultType
    {
        encaja = 0,
        atrapa = 1,
        despeja = 2,
        fuera = 3,
        target = 4
    };

    public static Interfaz instance { get; private set; }

    public static string authCode = "1111111112";
    public static int m_ts = 0;
    public static string m_uid = "";
    public static string m_sessionToken = "";
    public static string m_uname = "";
    public GameObject m_cortinillaPrefab;

    static bool m_wsReady = false;
    public static string paramURL = "";

    /// <summary>
    /// Skill del Usuario (se modifica según vaya ganando o perdiendo partidos)
    /// </summary>
    public static int SkillLevel { get { return m_skillLevel; } set { m_skillLevel = value; } }
    static int m_skillLevel = 0;

    public static progress m_asThrower;
    public static progress m_asKeeper;
    public int m_time = 147;
    public static int m_ranking;
    public static int m_duelsPlayed = 0;
    public static int m_duelsWon = 0;
    public static int m_perfectDuels = 0;


    public List<string> progressLogros;
    public static List<string> lastProgressLogros;

    /// <summary>
    /// Posibles formas de pago al realizar una compra
    /// </summary>
    public enum TipoPago { PRECOMPRA, HARD, SOFT };

    /// <summary>
    /// Indica si el usuario ha iniciado sesion con facebook
    /// </summary>
    public static bool iniciadaSesionConFacebook
    {
        get { return m_iniciadaSesionConFacebook; }
        set
        {
            m_iniciadaSesionConFacebook = value;

            // actualizar el estado de las interfaces
            /*if (ifcMainMenu.instance != null) 
                ifcMainMenu.instance.RefrescarBotones();*/
        }
    }
    private static bool m_iniciadaSesionConFacebook = false;


    /// <summary>
    /// Dinero SOFT del usuario
    /// </summary>
    public static int MonedasSoft
    {
        get { return m_monedasSoft; }
        set
        {
            m_monedasSoft = value;
            PersistenciaManager.instance.SaveMoney();
            if (cntBarraSuperior.instance != null)
                cntBarraSuperior.instance.ActualizarDinero();
        }
    }
    private static int m_monedasSoft = 0;
    /// <summary>
    /// asigna el valor de la moneda soft sin almacenarlo en las preferencias
    /// </summary>
    /// <param name="_monedaSoft"></param>
    public static void SetMonedaSoft_SinPersistencia(int _monedaSoft)
    {
        m_monedasSoft = _monedaSoft;
    }


    /// <summary>
    /// Dinero HARD del usuario
    /// </summary>
    public static int MonedasHard
    {
        get { return m_monedasHard; }
        set
        {
            m_monedasHard = value;
            PersistenciaManager.instance.SaveHardMoney();
            if (cntBarraSuperior.instance != null)
                cntBarraSuperior.instance.ActualizarDinero();
        }
    }
    private static int m_monedasHard = 0;
    /// <summary>
    /// asigna el valor de la moneda hard sin almacenarlo en las preferencias
    /// </summary>
    /// <param name="_monedaHard"></param>
    public static void SetMonedaHard_SinPersistencia(int _monedaHard)
    {
        m_monedasHard = _monedaHard;
    }

    /// <summary>
    /// Indica la ultima mision que el jugador ha conseguido desbloquear
    /// Nota: cada nivel se compone de 10 misiones, por lo tanto el nivel al que pertenece una mision se puede calcular como: numNivel = (numMision / 10)
    /// </summary>
    public static int ultimaMisionDesbloqueada
    {
        get { return m_ultimaMisionDesbloqueada; }
        set
        {
            m_ultimaMisionDesbloqueada = value;
        }
    }
    private static int m_ultimaMisionDesbloqueada = 0;

    // URL de los servicios web
    //public static string baseUrl = "https://kickswsdesa.bitoon.com"; // <== URL desarrollo
    public static string baseUrl = "https://kickspro.bitoon.com"; // <= URL produccion


    // directorio de la aplicacion
    public static string gameUrl = "bkicks"; // Bt

    public static int m_eqCasillas = 0; // TODO 
    public static int m_eqIniesta = 0;
    public static int m_achievements = 0;
    public static int m_achievementsTotal = 0;
    public static int m_partidas = 5;
    public static int m_nextTryIn = 0;
    public static int m_nextTryTime = 0;

    public GameObject[] goalkeepers;
    public GameObject[] throwers;

    public GameObject goalkeeperModel;
    public GameObject throwerModel;

    public Vector3 m_posicionInstancioacionLanzador;   // posicion donde se ha creado la instancia del lanzador
    public Vector3 m_posicionInstanciacionPortero;    // posicion donde se ha creado la instancia del portero

    public Jugador m_goalKeeper;
    public Jugador m_thrower;

    public Jugador m_goalKeeperVestuario;
    public Jugador m_throwerVestuario;

    public int Goalkeeper
    {
        get { return (ifcBase.activeIface == ifcVestuario.instance) ? m_goalKeeperVestuario.index : m_goalKeeper.index; }

        set
        {
            int modu = InfoJugadores.instance.numPorteros; // (goalkeepers.Length); // + 1);
            Jugador temp = InfoJugadores.instance.GetPortero(((value % modu) + modu) % modu);
            if (temp.estado == Jugador.Estado.ADQUIRIDO || m_goalKeeper == null)
            {
                m_goalKeeper = temp;
            }
            m_goalKeeperVestuario = temp;
            FieldControl.localGoalkeeper = m_goalKeeper;

            string name = "P_IdleInterface_01";
            float time = 0f;
            int index = 0;
            Quaternion rotation = Quaternion.identity;
            if (goalkeeperModel != null)
            {
                name = goalkeeperModel.GetComponent<CicleAnimations>().GetAnim();
                index = goalkeeperModel.GetComponent<CicleAnimations>().index;
                time = goalkeeperModel.GetComponent<Animation>()[name].time;
                rotation = goalkeeperModel.transform.rotation;
            }
            PlayerPrefs.SetString("selectedGoalkeeper", m_goalKeeper.assetName);

            bool mustShow = true;
            float xCoord = 0f;

            if (((ifcBase.activeIface == ifcVestuario.instance) || (ifcBase.activeIface == ifcCarrera.instance)) && ifcBase.activeIface != null)
            {
                xCoord = Stats.PLAYER_VESTUARIO_COORDENADA_X;
                if (ifcBase.activeIface == ifcVestuario.instance) mustShow = ifcVestuario.instance.m_tipoVestuario == ifcVestuario.TipoVestuario.PORTERO;
                else if (ifcBase.activeIface == ifcCarrera.instance) mustShow = !ifcCarrera.instance.estoyMostrandoMisionDeLanzador;
            }
            else if (((ifcBase.activeIface == ifcLogros.instance) || (ifcBase.activeIface == ifcPerfil.instance)) && ifcBase.activeIface != null)
            {
                xCoord = Stats.PLAYER_VESTUARIO_COORDENADA_X;
                if (ifcBase.activeIface == ifcLogros.instance)
                {
                    mustShow = (ifcLogros.instance.m_modo == ifcLogros.Modo.PORTERO) || (ifcLogros.instance.m_modo == ifcLogros.Modo.DUELO);
                    if (ifcLogros.instance.m_modo == ifcLogros.Modo.DUELO)
                    {
                        xCoord = 0.42f;
                    }
                }
                else if (ifcBase.activeIface == ifcPerfil.instance)
                {
                    mustShow = (ifcPerfil.instance.m_tipoPagina == ifcPerfil.TipoPagina.PORTERO) || (ifcPerfil.instance.m_tipoPagina == ifcPerfil.TipoPagina.DUELO);
                    if (ifcPerfil.instance.m_tipoPagina == ifcPerfil.TipoPagina.DUELO)
                    {
                        xCoord = 0.42f;
                    }
                }
            }
            else
            {
                xCoord = 0.42f;
            }

            if (ifcBase.activeIface == ifcDuelo.instance)
            {
                mustShow = false;
            }

            GameObject.Find("girarPortero").transform.GetChild(0).GetComponent<Collider>().enabled = mustShow;


            if (mustShow)
            {
                Jugador infoJugador = m_goalKeeperVestuario;
                Vector3 screenpoint = new Vector3(xCoord, 0.225f, 0f);
                GameObject aux = InstantiatePlayerAtScreenRelative(screenpoint, true, infoJugador.idModelo); //vector es posicion del control, ajustada
                GameObject.Find("girarPortero").transform.position = aux.transform.position;

                Destroy(goalkeeperModel);
                goalkeeperModel = aux;
                goalkeeperModel.GetComponent<CicleAnimations>().index = index;
                goalkeeperModel.GetComponent<Animation>().Play(name, PlayMode.StopAll);
                goalkeeperModel.GetComponent<Animation>()[name].time = time;
                if (rotation != Quaternion.identity)
                {
                    goalkeeperModel.transform.rotation = rotation;
                }
            }
            else if (throwerModel != null)
            {
                if (goalkeeperModel != null)
                    goalkeeperModel.SetActive(false);
            }

            // asignar la equipacion que corresponda al portero
            EquipacionManager.instance.CambiarEquipacionPortero();

            // asignarle el numero de dorsal al jugador
            if (goalkeeperModel != null)
            {
                Numbers numbersComponent = goalkeeperModel.transform.FindChild("Body").gameObject.GetComponent<Numbers>();
                numbersComponent.number = temp.numDorsal;
            }
        }
    }

    public int Thrower
    {
        get
        {
            if (ifcBase.activeIface == ifcVestuario.instance)
                return (m_throwerVestuario != null) ? m_throwerVestuario.index : 0;
            else
                return (m_thrower != null) ? m_thrower.index : 0;
        }

        set
        {
            int modu = InfoJugadores.instance.numLanzadores; //(throwers.Length); // + 1);
            Jugador temp = InfoJugadores.instance.GetTirador(((value % modu) + modu) % modu);
            if (temp.estado == Jugador.Estado.ADQUIRIDO || m_thrower == null)
            {
                m_thrower = temp;
            }
            m_throwerVestuario = temp;
            FieldControl.localThrower = m_thrower;

            float time = 0f;
            Quaternion rotation = Quaternion.identity;
            string name = "IdleInterface_01";
            int index = 0;
            if (throwerModel != null)
            {
                name = throwerModel.GetComponent<CicleAnimations>().GetAnim();
                index = throwerModel.GetComponent<CicleAnimations>().index;
                time = throwerModel.GetComponent<Animation>()[name].time;
                rotation = throwerModel.transform.rotation;
            }
            PlayerPrefs.SetString("selectedThrower", m_thrower.assetName);

            bool mustShow = true;
            float xCoord = 0f;

            if (((ifcBase.activeIface == ifcVestuario.instance) || (ifcBase.activeIface == ifcCarrera.instance)) && ifcBase.activeIface != null)
            {
                xCoord = Stats.PLAYER_VESTUARIO_COORDENADA_X;
                if (ifcBase.activeIface == ifcVestuario.instance) mustShow = ifcVestuario.instance.m_tipoVestuario == ifcVestuario.TipoVestuario.LANZADOR;
                else if (ifcBase.activeIface == ifcCarrera.instance) mustShow = ifcCarrera.instance.estoyMostrandoMisionDeLanzador;
            }
            else if (((ifcBase.activeIface == ifcLogros.instance) || (ifcBase.activeIface == ifcPerfil.instance)) && ifcBase.activeIface != null)
            {
                xCoord = Stats.PLAYER_VESTUARIO_COORDENADA_X;

                if (ifcBase.activeIface == ifcLogros.instance)
                {
                    if (ifcLogros.instance.m_modo == ifcLogros.Modo.DUELO)
                    {
                        xCoord = 0.15f;
                    }
                    mustShow = (ifcLogros.instance.m_modo == ifcLogros.Modo.LANZADOR) || (ifcLogros.instance.m_modo == ifcLogros.Modo.DUELO);
                }
                else if (ifcBase.activeIface == ifcPerfil.instance)
                {
                    if (ifcPerfil.instance.m_tipoPagina == ifcPerfil.TipoPagina.DUELO)
                    {
                        xCoord = 0.15f;
                    }
                    mustShow = (ifcPerfil.instance.m_tipoPagina == ifcPerfil.TipoPagina.LANZADOR) || (ifcPerfil.instance.m_tipoPagina == ifcPerfil.TipoPagina.DUELO);
                }
            }
            else
            {
                xCoord = 0.15f;
            }


            if (mustShow)
            {
                Jugador infoJugador = temp;
                Vector3 screenpoint = new Vector3(xCoord, 0.225f, 0f);
                GameObject aux = InstantiatePlayerAtScreenRelative(screenpoint, false, infoJugador.idModelo); //vector es posicion del control, ajustada
                GameObject.Find("girarLanzador").transform.position = aux.transform.position;

                Destroy(throwerModel);
                throwerModel = aux;
                throwerModel.GetComponent<CicleAnimations>().index = index;
                throwerModel.GetComponent<Animation>().Play(name, PlayMode.StopAll);
                throwerModel.GetComponent<Animation>()[name].time = time;
                if (rotation != Quaternion.identity)
                {
                    throwerModel.transform.rotation = rotation;
                }
            }
            else if (throwerModel != null)
            {
                throwerModel.SetActive(false);
            }

            // asignar la equipacion que corresponda al lanzador
            EquipacionManager.instance.CambiarEquipacionLanzador();
            GameObject.Find("girarLanzador").transform.GetChild(0).GetComponent<Collider>().enabled = mustShow;

            // asignarle el numero de dorsal al jugador
            if (throwerModel != null)
            {
                Numbers numbersComponent = throwerModel.transform.FindChild("Body").gameObject.GetComponent<Numbers>();
                numbersComponent.number = temp.numDorsal;
            }
        }
    }

    public void ResetJugador(bool _portero)
    {
        if (_portero)
        {
            m_goalKeeperVestuario = m_goalKeeper;
        }
        else
        {
            m_throwerVestuario = m_thrower;
        }
    }

    public static bool m_firstTime = true;
    bool m_firstFrame = true;

    string ParseParam(string _paramName)
    {
        string param = Application.srcValue;
        int firstChar = param.IndexOf(_paramName);
        if (firstChar == -1)
        {
            //Debug.LogError("WebPlayer parameter not found: " + _paramName);
            return null;
        }
        else
        {
            firstChar += _paramName.Length + 2;
        }
        int lastChar = param.IndexOf('\'', firstChar);
        param = param.Substring(firstChar, lastChar - firstChar);
        return param;
    }

    void GetToken(string _url)
    {
        paramURL = _url;
        authCode = ParseParam("authCode");
        m_wsReady = true;
    }

    void Awake()
    {

        if (PlayerPrefs.GetInt("firstTimeRate", 0) == 0)
        {
            PlayerPrefs.SetInt("firstTimeRate", 1);
#if UNITY_ANDROID
            EtceteraAndroid.resetAskForReview();
#endif
        }

        instance = this;
#if DEBUG_KICKS
        m_wsReady = true;
#endif
        //Debug.Log (Security.PrefetchSocketPolicy("http://biservicesdev.bitoon.mad:8080/", 843, 10000));
        ifcBase.scaleFactor = (float)Screen.height / (float)705;
        if (Screen.height <= 0) ifcBase.scaleFactor = 1;
        ifcBase.scaleWFactor = (float)Screen.width / (float)940;
        if (Screen.width <= 0) ifcBase.scaleWFactor = 1;

        if (ifcBase.origFactor < 0f)
        {
            ifcBase.origFactor = ifcBase.scaleFactor;
            ifcBase.origWFactor = ifcBase.scaleWFactor;
            ifcBase.origResolution = Screen.height;
            ifcBase.origWResolution = Screen.width;
        }
        Debug.Log(">>> ScaleFactor: W=" + ifcBase.scaleWFactor + "   H=" + ifcBase.scaleWFactor);

        // reescalar la interfaz para adaptarla al dispositivo
        ifcBase.Scale(this.gameObject);
        ifcBase.Scale(Cortinilla.instance.gameObject, Cortinilla.instance.currentScale);
        ifcBase.Scale(ifcItemRevealDialog.instance.gameObject, ifcItemRevealDialog.instance.currentScale);

#if !UNITY_ANDROID && !UNITY_IPHONE
        //Screen.SetResolution(940, 705, false);
#endif

        if (Application.isWebPlayer)
        {
            authCode = ParseParam("authCode");
            string sv1 = ParseParam("biwsURL");
            if (sv1 != null) BI.baseURL = WWW.UnEscapeURL(sv1) + "/biservicerest/rest/biservice";
            string sv2 = ParseParam("kwsURL");
            if (sv2 != null)
            {
                baseUrl = WWW.UnEscapeURL(sv2);
                DownloadDaemon.baseURL = WWW.UnEscapeURL(sv2);
                DownloadDaemon.mediaURL = WWW.UnEscapeURL(sv2);

                DownloadDaemonJSON.baseURL = WWW.UnEscapeURL(sv2);
                DownloadDaemonJSON.mediaURL = WWW.UnEscapeURL(sv2);
                m_wsReady = true;
            }
        }
        else
        {
#if UNITY_ANDROID || UNITY_IPHONE
            m_wsReady = false;
#else
            m_wsReady = true;
#endif
        }

        goalkeeperModel = GameObject.Find("GoalKeeperIface");
        throwerModel = GameObject.Find("JugadorIface");

        Goalkeeper = InfoJugadores.instance.GetJugador(PlayerPrefs.GetString("selectedGoalkeeper", "IT_PLY_GK_0003")).index;
        Thrower = InfoJugadores.instance.GetJugador(PlayerPrefs.GetString("selectedThrower", "IT_PLY_ST_0003")).index;

        if (Cortinilla.instance == null)
            GameObject.Instantiate(m_cortinillaPrefab, new Vector3(-0.6f, 0.5f, 50), Quaternion.identity).name = "Cortinilla";

        // TEST ELO
        // ClashRoyaleELO.TEST();
    }

    public static void ClickFX()
    {
        if (ifcOpciones.fx)
        {
            GeneralSounds_menu.instance.click();
        }
    }

    public void CleanPlayers()
    {
        if (Thrower == throwers.Length) Thrower = Random.Range(0, throwers.Length);
        else Thrower = Thrower;

        if (Goalkeeper == goalkeepers.Length) Goalkeeper = Random.Range(0, goalkeepers.Length);
        else Goalkeeper = Goalkeeper;
    }

    public int GetPositionInGoalkeepers(string key) {
        int position = -1;
        for (int i=0; i<goalkeepers.Length; i++) {
            if (goalkeepers[i] != null && goalkeepers[i].name.Contains(key)) {
                position = i;
                break;
            }
        }
        return position;
    }

    public int GetPositionInThrowers(string key) {
        int position = -1;
        for (int i=0; i<throwers.Length; i++) {
            if (throwers[i] != null && throwers[i].name.Contains(key)) {
                position = i;
                break;
            }
        }
        return position;
    }

    public static int MatchResult(int ownerELO, int ownerScore, int opponentELO, int opponentScore) {
        Debug.Log("MatchResult");
        int mod = ClashRoyaleELO.Result(ownerELO, ownerScore, opponentELO, opponentScore);
        SkillLevel += mod;
        SkillLevel = Mathf.Max(0, SkillLevel);

        PersistenciaManager.instance.GuardarSkillLevel();
        return mod;
    }

    /*void RepositionThrower()
    {
      Vector3 screenPosGK = Camera.main.WorldToScreenPoint(goalkeeperModel.transform.position);
      Vector3 deltapos = new Vector3((Screen.width * -0.29f),0,0);
      Ray worldPosTh = Camera.main.ScreenPointToRay(screenPosGK + deltapos);
      Plane groundPlane = new Plane(Vector3.up, goalkeeperModel.transform.position);
      float hitDistance;
      groundPlane.Raycast(worldPosTh, out hitDistance);
      Vector3 newPos = worldPosTh.GetPoint(hitDistance);
      Vector3 oldPos = throwerModel.transform.position;
      throwerModel.transform.position = newPos;
      Transform rotator = GameObject.Find ("girarLanzador").transform;
      rotator.position += newPos - oldPos;
    }*/

#if UNITY_ANDROID
    void CargarInventarioAndroid(List<GooglePurchase> arg1, List<GoogleSkuInfo> arg2)
    {
        GoogleIABManager.queryInventorySucceededEvent -= CargarInventarioAndroid;
        PurchaseManager.InventarioTienda = arg2;
    }
#endif

    void Start()
    {
        Debug.Log("DIMENSIONES PANTALLA: " + Screen.width + "x" + Screen.height);

        PurchaseManager.InitStore();

#if UNITY_ANDROID
        GoogleIABManager.queryInventorySucceededEvent += CargarInventarioAndroid;
        //IrACompras(new List<GooglePurchase>(), new List<GoogleSkuInfo>()); //f4ke para entrar en la seccion de compras
#else
#endif

        ifcBase.activeIface = ifcMainMenu.instance;
        // recuperar la informacion de las preferencias
        PersistenciaManager.instance.LoadAllData();

        EscudosManager.instance.ComprobarEscudosConsumidos();

        Shark.instance.OnConnect = () =>
        {
            MsgLogin msg = Shark.instance.mensaje<MsgLogin>();
            msg.m_alias = Interfaz.m_uname;
            msg.m_goalkeeper = InfoJugadores.instance.GetPortero(Goalkeeper).assetName;
            msg.m_thrower = InfoJugadores.instance.GetTirador(Thrower).assetName;
            msg.m_throwerEquipacion = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada().assetName;
            msg.m_goalkeeperEquipacion = EquipacionManager.instance.GetEquipacionPorteroSeleccionada().assetName;
            msg.m_duelosJugados = Interfaz.m_duelsPlayed;
            msg.m_duelosGanados = Interfaz.m_duelsWon;
            msg.m_skillLevel = Interfaz.SkillLevel;
            if (msg == null) Debug.Log("Error en MsgLogin");
            else msg.send();
        };
        Shark.instance.OnDisconnect = () => { };
        Shark.instance.Desconectar();

        MissionManager.instance.ClearCurrentMission();

        //getServerIP(Stats.tipo, Stats.zona);

        progressLogros = new List<string>();

        //RepositionThrower();
        GameObject.Find("girarLanzador").transform.position = GetGroundScreenProjection(new Vector3(0.15f, 0.225f, 0f));
        GameObject.Find("girarPortero").transform.position = GetGroundScreenProjection(new Vector3(0.42f, 0.225f, 0f));

        transform.Find("MainMenu/bitoon_logo").GetComponent<btnButton>().action = (_name) =>
        {
            BI.Publicidad(3, 3); // Logo de Bitoon 
            Application.OpenURL("http://www.bitoon.com");
        };

#if UNITY_IPHONE
    EtceteraBinding.askForReview(2,72, "Please rate my app!", "It will really make me happy if you do...", "887914760");
#elif UNITY_ANDROID
        //EtceteraAndroid.resetAskForReview();
        EtceteraAndroid.askForReview(2, 0, 72, "Please rate my app!", "It will really make me happy if you do...");
#endif
        cntMenuDesplegableOpciones.instance.onStart();
    }

    void Update()
    {

        m_firstTime = false;
        if (m_firstFrame && m_wsReady)
        {
            m_achievementsTotal = 0; // cntLogros.instance.m_logros.m_lista.Length;
            m_firstFrame = false;
            // getAuth();
            // CalculateRevealLogros();
            BI.AppStart();
        }
    }


    /// <summary>
    /// Realiza un connect (con valores F4KE) contra los webservices
    /// </summary>
    void connect(DownloadDaemonJSON.callBack _callbackOk, DownloadDaemonJSON.stringCallBack _callbackError)
    {
        // valores F4KE
        string fbUid = "100003829369724";
        string fbAlias = "juanfrancisco.oriolsanchez";
        string fbFirstName = "juanfrancisco";
        string fbLastName = "oriolsanchez";

        string jsonParam = "{\"fbUid\":\"" + fbUid + "\", ";
        jsonParam += "\"alias\":\"" + fbAlias + "\", ";
        jsonParam += "\"firstName\":\"" + fbFirstName + "\", ";
        jsonParam += "\"lastName\":\"" + fbLastName + "\", ";
        jsonParam += "\"apnId\":null, \"gcmId\":null, \"password\":null, \"mail\":null, \"friends\":[]}";

        // llamar al webservice para efectuar la compra
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/users/connect",
            // callback si todo OK
            (object _ret) =>
            {
                if (_ret == null)
                    Debug.Log("Connect no ha devuelto info");
                else
                {
                    Dictionary<string, System.Object> info = _ret as Dictionary<string, System.Object>;
                    if (info == null)
                        Debug.Log("Connect no ha devuelto info");
                    else
                    {
                        // obtener el token e indicar que estamos conectados a facebook
                        m_sessionToken = info["token"].ToString();
                        iniciadaSesionConFacebook = true;

                        Debug.LogWarning(">>> Connect OK, token=" + m_sessionToken);

                        // ejecutar el callback
                        if (_callbackOk != null)
                            _callbackOk(_ret.ToString());

                        /*
                        // calcular y actualizar el dinero del jugador
                        Dictionary<string, object> monedas = info["currency"] as Dictionary<string, object>;
                        Interfaz.m_monedasSoft = (int) monedas["softCurrency"];
                        Interfaz.m_monedasHard = (int) monedas["hardCurrency"];
                        cntBarraSuperior.instance.ActualizarDinero();
                         */
                    }
                }

            },
            // callback si error
            (string _ret) =>
            {
                Debug.Log("ER >>>> connect " + _ret);

                if (_callbackError != null)
                    _callbackError(_ret.ToString());
            },
            // parametros
            jsonParam);
    }



    /// <summary>
    /// Obtiene el ranking de usuarios
    /// </summary>
    /// <param name="_numElementosRanking">Numero de elementos en el ranking</param>
    void getRanking(int _numElementosRanking)
    {
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/main/rankings?token=" + m_sessionToken,
            // callback si todo OK
            (object _ret) =>
            {
                Dictionary<string, System.Object> rankingInfo = _ret as Dictionary<string, System.Object>;
                if (rankingInfo == null)
                    Debug.LogWarning("No hay informacion de los rankings");
                else
                {
                    // obtener la posicion del usuario local los distintos rankings
                    ifcRanking.RankingEntry posUsuarioLanzador = GetRankingEntry("playerShooter", rankingInfo);
                    ifcRanking.RankingEntry posUsuarioPortero = GetRankingEntry("playerGoalKeeper", rankingInfo);
                    ifcRanking.RankingEntry posUsuarioMultiplayer = GetRankingEntry("playerMultiplayer", rankingInfo);

                    // obtener la informacion de los distintos rankings
                    ifcRanking.RankingEntry[] rankingLanzador = GetRanking("shooters", rankingInfo, _numElementosRanking);
                    ifcRanking.RankingEntry[] rankingPortero = GetRanking("goalKeepers", rankingInfo, _numElementosRanking);
                    ifcRanking.RankingEntry[] rankingMultiplayer = GetRanking("multiplayer", rankingInfo, _numElementosRanking);

                    // pasar a la interfaz del ranking la informacion obtenida
                    ifcRanking.instance.SetInfoPosUsuario(posUsuarioLanzador, posUsuarioPortero, posUsuarioMultiplayer);
                    ifcRanking.instance.SetInfoRankings(rankingLanzador, rankingPortero, rankingMultiplayer);
                    Debug.LogWarning(">>> Guardo los rankings");

                    // si los rankings estan visibles => repintarlos
                    if (ifcBase.activeIface == ifcRanking.instance)
                        ifcRanking.instance.Refresh();
                }
            },
            // callback si error
            (string _ret) =>
            {
                Debug.Log("ER getRanking >>>> " + _ret);
            },
            // parametros
            "{\"maxResults\": " + _numElementosRanking + "}");
    }


    /// <summary>
    /// Busca en el diccionario "_info" una la entrada de tipo "RankingEntry" con nombre "_nombreEntrada"
    /// </summary>
    /// <param name="_nombreEntrada"></param>
    /// <param name="_info"></param>
    /// <returns></returns>
    private ifcRanking.RankingEntry GetRankingEntry(string _nombreEntrada, Dictionary<string, System.Object> _info)
    {
        ifcRanking.RankingEntry rankingEntry = new ifcRanking.RankingEntry();

        if (_info != null)
        {
            if (_info.ContainsKey(_nombreEntrada))
            {
                Dictionary<string, System.Object> infoUsuario = _info[_nombreEntrada] as Dictionary<string, System.Object>;
                if (infoUsuario != null)
                {
                    rankingEntry.m_name = infoUsuario["alias"].ToString();
                    rankingEntry.m_pos = (int)infoUsuario["rank"];
                    rankingEntry.m_points = (int)infoUsuario["score"];
                }
                else
                    Debug.LogWarning(">>> No se ha encontrado la entrada " + _nombreEntrada);
            }
            else
                Debug.LogWarning(">>> No se ha encontrado la entrada " + _nombreEntrada);
        }
        else
            Debug.LogWarning(">>> No se ha encontrado la entrada " + _nombreEntrada);

        return rankingEntry;
    }


    /// <summary>
    /// Busca en el diccionario "_info" una la entrada de tipo "RankingEntry" con nombre "_nombreEntrada" y devuelve el ranking contenido en ella
    /// </summary>
    /// <param name="_nombreEntrada"></param>
    /// <param name="_info"></param>
    /// <param name="_numElementosRanking"></param>
    /// <returns></returns>
    private ifcRanking.RankingEntry[] GetRanking(string _nombreEntrada, Dictionary<string, System.Object> _info, int _numElementosRanking)
    {
        ifcRanking.RankingEntry[] rankingEntrys = new ifcRanking.RankingEntry[_numElementosRanking];

        if (_info != null)
        {
            if (_info.ContainsKey(_nombreEntrada))
            {
                List<System.Object> infoUsuarios = _info[_nombreEntrada] as List<System.Object>;
                foreach (System.Object elem in infoUsuarios)
                {
                    Dictionary<string, System.Object> infoUsuario = elem as Dictionary<string, System.Object>;
                    int posRanking = (int)infoUsuario["rank"];
                    if ((posRanking - 1) < _numElementosRanking)
                    {
                        rankingEntrys[posRanking - 1].m_name = infoUsuario["alias"].ToString();
                        rankingEntrys[posRanking - 1].m_pos = posRanking;
                        rankingEntrys[posRanking - 1].m_points = (int)infoUsuario["score"];
                    }
                }
            }
            else
                Debug.LogWarning(">>> No se ha encontrado la entrada " + _nombreEntrada);
        }
        else
            Debug.LogWarning(">>> No se ha encontrado la entrada " + _nombreEntrada);

        return rankingEntrys;
    }


    public static int recompensaAcumulada = 0;

    static void addLogro(string logro)
    {
        Debug.Log("ADDLOGRO " + logro);
        if (cntLogros.instance != null) cntLogros.instance.Unlock(logro);
        else m_achievements++;
        if (ifcLogroInGame.m_logroConseguido != null)
        {
            ifcLogroInGame.m_logroConseguido.Enqueue(logro);
            BI.Logro(ifcLogroInGame.instance.m_logros.Code2ID(logro), 0);
            int pts = ifcLogroInGame.instance.m_logros.getLogroByCode(logro).m_premio;
            if (pts != 0) BI.PuntosBBVA(1, pts);
        }
    }


    /// <summary>
    /// Envia los resultados de la partida a los webservices
    /// NOTA: los resultados se recogen de la clase "RoundInfoManager" (donde se han ido acumulando tiro a tiro)
    /// </summary>
    public void EnviarResultadosPartidaSinglePlayer()
    {
        //char[] res = new char[] { 'e', 'a', 'd', 'f', 't' };
        string mode = (GameplayService.initialGameMode == GameMode.Shooter) ? "t" : "p";

        // construir la lista con el nombre de los logros conseguidos en esta mision
        GameLevelMission gameLevel = MissionManager.instance.GetGameLevelMission(GameplayService.gameLevelMission.MissionName);
        string objetivosConseguidos = "";
        if (gameLevel == null)
        {
            Debug.LogWarning(">>> No hay informacion del nivel " + GameplayService.gameLevelMission.MissionName);
        }
        else
        {
            // lista de objetivos del nivel
            List<MissionAchievement> listaObjetivos = gameLevel.GetAchievements();
            if (listaObjetivos == null)
                Debug.LogWarning(">>> El nivel " + GameplayService.gameLevelMission.MissionName + " no tiene objetivos asociados");
            else
            {
                for (int i = 0; i < listaObjetivos.Count; ++i)
                {
                    if (listaObjetivos[i].IsAchieved())
                    {
                        if (i > 0)
                            objetivosConseguidos += ", ";
                        objetivosConseguidos += "\"" + listaObjetivos[i].Code + "\"";
                    }
                }
            }
        }

        // construir el json para enviar el progreso
        string param = "{\"ratio\":" + EscudosManager.escudoEquipado.ToString() + ", \"actions\":[";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"t\", \"value\":" + RoundInfoManager.instance.numTargets.ToString() + "}, ";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"e\", \"value\":" + RoundInfoManager.instance.numEncajados.ToString() + "}, ";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"a\", \"value\":" + RoundInfoManager.instance.numAtrapados.ToString() + "}, ";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"d\", \"value\":" + RoundInfoManager.instance.numDespejados.ToString() + "}, ";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"f\", \"value\":" + RoundInfoManager.instance.numFueras.ToString() + "}, ";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"p\", \"value\":" + RoundInfoManager.instance.puntos.ToString() + "}, ";
        param += "{\"mode\":\"" + mode + "\", \"action\":\"time\", \"value\":" + RoundInfoManager.instance.time.ToString() + "} ";
        param += "], ";
        param += "\"mission\": { \"missionId\":\"" + GameplayService.gameLevelMission.MissionName + "\"}, ";
        param += "\"objetives\":{  \"missionId\":\"" + GameplayService.gameLevelMission.MissionName + "\", \"objetives\":[" + objetivosConseguidos + "]}";
        param += "}";

        Debug.LogWarning("URL: " + baseUrl + "/" + gameUrl + "/rest/main/track/" + m_sessionToken);
        Debug.LogWarning("param = " + param);

        // llamar al servicio web
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/main/track/" + m_sessionToken,
            // callback si todo OK
            (object _ret) =>
            {
                if (_ret == null)
                {
                    Debug.LogWarning(">>> EnviarResultadosPartidaSinglePlayer no ha devuelto informacion");
                }
                else
                {
                    Dictionary<string, System.Object> info = _ret as Dictionary<string, System.Object>;

                    // comprobar si hay recompensas


                    // calcular y actualizar el dinero del jugador
                    Dictionary<string, object> monedas = info["currency"] as Dictionary<string, object>;
                    Interfaz.MonedasSoft = (int)monedas["softCurrency"];
                    Interfaz.MonedasHard = (int)monedas["hardCurrency"];
                    if (cntBarraSuperior.instance != null)
                        cntBarraSuperior.instance.ActualizarDinero();

                    Debug.LogWarning(">>> Actualizo el dinero del jugador: SOFT=" + Interfaz.MonedasSoft + "   HARD=" + Interfaz.MonedasHard);
                }
            },
            // callback si error
            (string _ret) =>
            {
                Debug.Log("ER >>>> EnviarResultadosPartidaSinglePlayer " + _ret);
            },
            // parametros
            param);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_uid"></param>
    public void getMultiplayerCard(string _uid)
    {
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/multi/card",
            // callback si todo OK
            (object _ret) =>
            {
                Debug.Log(">>>> getMultiplayerCard ha devuelto: " + _ret.ToString());
            },
            // callback si error
            (string _ret) =>
            {
                Debug.Log("ER >>>> getMultiplayerCard " + _ret);
            },
            "{\"uid\":\"" + _uid + "\"}");
    }


    /// <summary>
    /// Realiza el proceso de compra de un jugador contra los webservices
    /// </summary>
    /// <param name="_jugador"></param>
    public void comprarJugador(Jugador _jugador, TipoPago _tipoPago)
    {
        // XIMO: 19/06/2017: Actualmente no queremos un sistema de "Compra de Jugadores"
        /*
        if (_jugador == null || _jugador.estado == Jugador.Estado.ADQUIRIDO)
            return;

        // comprobar si el metodo de pago afecta al dinero soft o al hard
        if (_tipoPago == TipoPago.SOFT)
        {
            // pago con dinero SOFT
            if (Interfaz.MonedasSoft < _jugador.precioSoft)
            {
                ifcDialogBox.instance.ShowOneButtonDialog(
                    ifcDialogBox.OneButtonType.POSITIVE,
                    LocalizacionManager.instance.GetTexto(84).ToUpper(),
                    string.Format(LocalizacionManager.instance.GetTexto(90), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(46) + "</color>", "<color=#ddf108> " + _jugador.nombre + "</color>"),
                    LocalizacionManager.instance.GetTexto(45).ToUpper(),
                    (_name) => { ifcBuyHardCashDialogBox.Instance.Show(false); }
                );
                return;
            }
            Interfaz.MonedasSoft -= _jugador.precioSoft;
        }
        else
        {
            // pago con dinero HARD
            int precioHard = (_tipoPago == TipoPago.PRECOMPRA) ? _jugador.precioEarlyBuy : _jugador.precioHard;

            if (Interfaz.MonedasHard < precioHard)
            {
                ifcDialogBox.instance.ShowOneButtonDialog(
                    ifcDialogBox.OneButtonType.POSITIVE,
                    LocalizacionManager.instance.GetTexto(84).ToUpper(),
                    string.Format(LocalizacionManager.instance.GetTexto(90), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(47) + "</color>", "<color=#ddf108> " + _jugador.nombre + "</color>"),
                    LocalizacionManager.instance.GetTexto(45).ToUpper(),
                    (_name) => { ifcBuyHardCashDialogBox.Instance.Show(true); }
                );
                return;
            }
            Interfaz.MonedasHard -= precioHard;
        }

        // actualizar el dinero que se muestra en la barra superior
        cntBarraSuperior.instance.ActualizarDinero();

        GeneralSounds_menu.instance.comprar();

        // marcar el jugador como ADQUIRIDO y repintarlo
        _jugador.estado = Jugador.Estado.ADQUIRIDO;
        if (ifcVestuario.instance != null)
            ifcVestuario.instance.RefreshInfo();

        // guardar la compra del jugador en las preferencias
        PersistenciaManager.instance.SaveJugadoresComprados();

        if (goalkeeperModel.activeSelf == true) Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;
        if (throwerModel.activeSelf == true) Interfaz.instance.Thrower = Interfaz.instance.Thrower;
        */
    }


    /// <summary>
    /// Realizar el proceso de compra de una equipacion contra los webservices
    /// </summary>
    /// <param name="_equipacion"></param>
    /// <param name="_tipoPago">Indica como se va a realizar el pago de la compra</param>
    public void comprarEquipacion(Equipacion _equipacion, TipoPago _tipoPago)
    {
        // XIMO: 20/06/2017: Actualmente no queremos un sistema de "Compra de Equipaciones"
        /*

        if (_equipacion == null || _equipacion.estado == Equipacion.Estado.ADQUIRIDA)
            return;

        // comporbar si el metodo de pago afecta al dinero soft o al hard
        if (_tipoPago == TipoPago.SOFT)
        {
            // pago con dinero SOFT
            if (Interfaz.MonedasSoft < _equipacion.precioSoft)
            {
                ifcDialogBox.instance.ShowOneButtonDialog(
                    ifcDialogBox.OneButtonType.POSITIVE,
                    LocalizacionManager.instance.GetTexto(84).ToUpper(),
                    string.Format(LocalizacionManager.instance.GetTexto(86), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(46) + "</color>"),
                    LocalizacionManager.instance.GetTexto(45).ToUpper(),
                    (_name) => { ifcBuyHardCashDialogBox.Instance.Show(false); });
                return;
            }

            Interfaz.MonedasSoft -= _equipacion.precioSoft;
        }
        else
        {
            // pago con dinero HARD
            int precioHard = (_tipoPago == TipoPago.PRECOMPRA) ? _equipacion.precioEarlyBuy : _equipacion.precioHard;

            if (Interfaz.MonedasHard < _equipacion.precioHard)
            {
                ifcDialogBox.instance.ShowOneButtonDialog(
                    ifcDialogBox.OneButtonType.POSITIVE,
                    LocalizacionManager.instance.GetTexto(84).ToUpper(),
                    string.Format(LocalizacionManager.instance.GetTexto(86), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(47) + "</color>"),
                    LocalizacionManager.instance.GetTexto(45).ToUpper(),
                    (_name) => { ifcBuyHardCashDialogBox.Instance.Show(true); });
                return;
            }

            Interfaz.MonedasHard -= precioHard;
        }

        // actualizar el dinero que se muestra en la barra superior
        cntBarraSuperior.instance.ActualizarDinero();

        GeneralSounds_menu.instance.comprar();

        // actualizar el estado de la equipacion
        _equipacion.estado = Equipacion.Estado.ADQUIRIDA;
        if (ifcVestuario.instance != null)
            ifcVestuario.instance.RefreshInfo();

        // guardar la compra de la equipacion en las preferencias
        PersistenciaManager.instance.SaveEquipacionesCompradas();

        //equiparla de inmediato (estas asignaciones lo actualizaran)
        EquipacionManager.instance.idEquipacionPorteroSeleccionada = EquipacionManager.instance.idEquipacionPorteroSeleccionada;
        EquipacionManager.instance.idEquipacionLanzadorSeleccionada = EquipacionManager.instance.idEquipacionLanzadorSeleccionada;
        */
    }

    /*
    /// <summary>
    /// Realiza el proceso de compra de un power up
    /// </summary>
    /// <param name="_powerUpDescriptor"></param>
    public void comprarPowerUp(PowerUpDescriptor _powerUpDescriptor) {
        if (_powerUpDescriptor == null)
            return;

        // comprobar que el usuario tenga suficiente dinero para la compra
        if (Interfaz.MonedasSoft < _powerUpDescriptor.precioSoft) {
            ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "AVISO", "No dispones de suficientes monedas para comprar el " + _powerUpDescriptor.nombre + ".");
            return;
        }

        // llamar al web service de la compra
        comprarItem(_powerUpDescriptor.idWs, 1, true,
            // callback si todo OK
            (string _ret) => {
                // incrementar la cantidad de power ups y repintar la tienda
                PowerupService.ownInventory.IncrementarCantidadPowerUp(_powerUpDescriptor.idWs, 1);

                if (ifcVestuario.instance != null)
                    ifcVestuario.instance.RefreshInfo();
            },
            // callback si error
            (string _ret) => {
                Debug.Log("ER >>>> comprarPowerUp " + _ret);
            });
    }
    */

    /// <summary>
    /// Metodo generico para comprar items
    /// </summary>
    /// <param name="_codigoItem">Codigo para identificar el item contra los webservices</param>
    /// <param name="_cantidad">Cantidad de items a comprar</param>
    /// <param name="_useSoft">True si la compra se hace con moneda soft, false si es con hard</param>
    /// <param name="_callbackOk"></param>
    /// <param name="_callBackError"></param>
    private void comprarItem(string _codigoItem, int _cantidad = 1, bool _useSoft = false, DownloadDaemonJSON.stringCallBack _callbackOk = null, DownloadDaemonJSON.stringCallBack _callBackError = null)
    {
        string useSoft = (_useSoft) ? "true" : "false";

        // llamar al webservice para efectuar la compra
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/market/purchase/" + m_sessionToken,
            // callback si todo OK
            (object _ret) =>
            {
                // actualizar el dinero del usuario en la barra de menu
                Dictionary<string, System.Object> currency = _ret as Dictionary<string, System.Object>;
                if (currency != null)
                {
                    Interfaz.MonedasSoft = (int)currency["softCurrency"];
                    Interfaz.MonedasHard = (int)currency["hardCurrency"];
                    cntBarraSuperior.instance.ActualizarDinero();
                }
                else
                    Debug.LogWarning(">>> No he podido obtenido el dinero de la llamada");

                if (_callbackOk != null)
                    _callbackOk(_ret.ToString());
            },
            // callback si error
            (string _ret) =>
            {
                if (_callBackError != null)
                    _callBackError(_ret.ToString());
            },
            "{\"itemCode\":\"" + _codigoItem + "\", \"quantity\":" + _cantidad.ToString() + ", \"useSoft\":" + useSoft + "}");
    }



    /// <summary>
    /// Metodo para indicar a los webservices que se ha utilizado un item
    /// </summary>
    /// <param name="_idItem"></param>
    /// <param name="_cantidad"></param>
    public void consumirItem(string _idItem, int _cantidad = 1)
    {
        Debug.Log(">>> URL: " + baseUrl + "/" + gameUrl + "/rest/market/use/" + m_sessionToken);
        Debug.Log(">>> parametros: " + "{\"itemCode\":\"" + _idItem + "\", \"quantity\":" + _cantidad + "}");

        // llamar al webservice para efectuar la compra
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/market/use/" + m_sessionToken,
            // callback si todo OK
            (object _ret) =>
            {
                Debug.Log(">>> Item " + _idItem + " utilizado OK");
            },
            // callback si error
            (string _ret) =>
            {
                Debug.Log("ER >>>> usarItem: " + _ret);
            },
            "{\"itemCode\": \"" + _idItem + "\", \"quantity\": " + _cantidad + "}");
    }

    /*
    /// <summary>
    /// Solicita la lista de items adquiridos por el usuario (y actualiza la interfaz en consecuencia)
    /// </summary>
    public void getItemsComprados() {
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/market/get/" + m_sessionToken,
            // callback si todo OK
            (object _ret) => {
                // obtener la informacion de cada uno de los elementos de la tienda
                Dictionary<string, System.Object> content = _ret as Dictionary<string, System.Object>;


                // obtener los JUGADORES
                if (content.ContainsKey("PLAYER")) {
                    List<System.Object> listaItems = content["PLAYER"] as List<System.Object>;
                    foreach (Dictionary<string, object> item in listaItems) {
                        // comprobar si el item es un "JUGADOR"
                        if (item["categoria"].ToString() == "PLAYER") {
                            Jugador jugador = InfoJugadores.instance.GetJugador(item["codigo"].ToString());
                            if (jugador == null) {
                                Debug.LogWarning(">>> No se ha encontrado ningun jugador con id=\"" + item["codigo"].ToString() + "\"");
                            } else {
                                // comprobar si el jugador esta DISPONIBLE (desbloqueado)
                                if ((int) item["purchased"] < 0) {
                                    Debug.LogWarning(">>> El jugador " + jugador.assetName + " esta DISPONIBLE");
                                    jugador.estado = Jugador.Estado.DISPONIBLE;

                                    // comprobar si el jugador ha sido ADQUIRIDO (comprado)
                                } else if ((int) item["purchased"] > 0) {
                                    Debug.LogWarning(">>> El jugador " + jugador.assetName + " esta ADQUIRIDO");
                                    jugador.estado = Jugador.Estado.ADQUIRIDO;
                                }
                            }
                        }
                    } // foreach
                }


                // obtener las EQUIPACIONES
                if (content.ContainsKey("EQUIPMENT")) {
                    List<System.Object> listaItems = content["EQUIPMENT"] as List<System.Object>;
                    foreach (Dictionary<string, object> item in listaItems) {
                        // comprobar si el item es un "JUGADOR"
                        if (item["categoria"].ToString() == "EQUIPMENT") {
                            Equipacion equipacion = EquipacionManager.instance.GetEquipacion(item["codigo"].ToString());
                            if (equipacion == null) {
                                Debug.LogWarning(">>> No se ha encontrado ninguna equipacion con id=\"" + item["codigo"].ToString() + "\"");
                            } else {
                                // comprobar si la equipacion esta DISPONIBLE (desbloqueada)
                                if ((int) item["purchased"] < 0) {
                                    Debug.LogWarning(">>> La equipacion " + equipacion.assetName + " esta DISPONIBLE");
                                    equipacion.estado = Equipacion.Estado.DISPONIBLE;

                                    // comprobar si la equipacion ha sido ADQUIRIDA (comprada)
                                } else if ((int) item["purchased"] > 0) {
                                    Debug.LogWarning(">>> La equipacion " + equipacion.assetName + " esta ADQUIRIDA");
                                    equipacion.estado = Equipacion.Estado.ADQUIRIDA;
                                }
                            }
                        }
                    } // foreach
                }
                

                // obtener los POWERUPS
                if (content.ContainsKey("POWUP")) {
                    // crear un inventario de power ups con los items recibidos
                    PowerupInventory powerupInventory = new PowerupInventory();
                    List<System.Object> listaItems = content["POWUP"] as List<System.Object>;
                    foreach (Dictionary<string, object> item in listaItems) {
                        if (item["categoria"].ToString() == "POWUP") {
                            // modificar el numero de powerups adquiridos
                            powerupInventory.SetCantidadPowerUp(item["codigo"].ToString(), (int) item["purchased"]);
                        }
                    } // foreach

                    // guardar el inventario de power ups
                    PowerupService.ownInventory = powerupInventory;
                }


                // obtener los ESCUDOS
                if (content.ContainsKey("SHIELD")) {
                    List<System.Object> listaItems = content["SHIELD"] as List<System.Object>;
                    foreach (Dictionary<string, object> item in listaItems) {
                        if (item["categoria"].ToString() == "SHIELD") {
                            // modificar el numero de escudos adquiridos
                            Escudo escudo = EscudosManager.instance.GetEscudo(item["codigo"].ToString());
                            if (escudo == null)
                                Debug.LogWarning(">>> No se ha encontrado ningun escudo con id=\"" + item["codigo"].ToString() + "\"");
                            else{
                                Debug.LogWarning(">>> Del escudo " + item["codigo"].ToString() + " se han encontrado " + item["purchased"].ToString() + " unidades");
                                escudo.numUnidades = (int) item["purchased"];
                            }
                        }
                    } // foreach
                }

                // fuerza a que si los jugadores y equipaciones seleccionadas actualmente no estan ADQUIRIDOS, sean sustidos por unos que si
                ifcVestuario.instance.ComprobarJugadoresYEquipacionesAdquiridos(true);
            },
            // callback si error
            (string _ret) => {
                Debug.Log("ER >>>> comprobarItemsAdquiridos " + _ret);
            },
            "{\"uid\":\"" + m_uid + "\"}");
    }
    */

    /// <summary>
    /// Obtiene la IP de uin servidor multiplayer"
    /// </summary>
    /// <param name="_uid"></param>
    public void getServerIP(string _tipo, int _zona)
    {
        // HACK: Acceso al servidor "directamente" sin el servicio Web (del que no disponemos del código fuente)
        string ip = "95.215.60.186";
        int port = 5555;

        Shark.instance.m_URL = ip;
        Shark.instance.m_Port = port;
        ifcVestuario.instance.GoDuelo();
        Debug.Log("SERVIDOR ENCONTRADO: " + ip + ":" + port);

        /*
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/gameserver-manager/rest/servers/get",
            // callback si todo OK
            (object _ret) =>
            {
                Dictionary<string, System.Object> content = _ret as Dictionary<string, System.Object>;
                List<System.Object> servidores = content["servers"] as List<System.Object>;
                if (servidores == null || servidores.Count == 0)
                {
                    Debug.LogWarning(">>> getServerIP: No ha devuelto servidores para {\"tipo\":\"" + _tipo + "\",\"zona\":" + _zona.ToString() + ",\"version\":\"" + Stats.version + "\"}");
                    ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(83).ToUpper(),
                            LocalizacionManager.instance.GetTexto(98),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
                }
                else
                {
                    Dictionary<string, System.Object> serv = servidores[0] as Dictionary<string, System.Object>;
                    if (serv["ip"] as string != "")
                    {
                        string ip = serv["ip"] as string;
                        int port = (int)serv["puerto"];
                        Shark.instance.m_URL = ip;
                        Shark.instance.m_Port = port;
                        ifcVestuario.instance.GoDuelo();
                        Debug.Log("SERVIDOR ENCONTRADO: " + ip + ":" + port);
                    }
                    else
                    {
                        ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(83).ToUpper(),
                            LocalizacionManager.instance.GetTexto(98),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
                    }
                }
            },

            // callback si error
            (string _ret) =>
            {
                Debug.Log("ER >>>> getServer " + _ret);
                ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(83).ToUpper(),
                            LocalizacionManager.instance.GetTexto(99),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
            },

            ("{\"tipo\":\"" + _tipo + "\",\"zona\":" + _zona.ToString() + ",\"version\":\"" + Stats.version + "\"}")
        );
        */
    }



    public GameObject InstantiatePlayerAtScreenRelative(Vector3 _screenPoint, bool _isGoalkeeper, int _index, Equipacion _equipacion = null)
    {
        Vector3 newPos = GetGroundScreenProjection(_screenPoint);

        // guardar la posicion donde se ha instanciado el jugador
        if (_isGoalkeeper)
        {
            /*if(m_posicionInstanciacionPortero == Vector3.zero)
            {
                m_posicionInstanciacionPortero = newPos;
            }
            GameObject.Find("girarPortero/girarPorteroCollider").transform.GetComponent<RotateOnDrag>().ApplyOffset(newPos - m_posicionInstanciacionPortero);*/
            m_posicionInstanciacionPortero = newPos;
        }
        else
        {
            /*if(m_posicionInstancioacionLanzador == Vector3.zero)
            {
                m_posicionInstancioacionLanzador = newPos;
            }
            GameObject.Find("girarLanzador/girarLanzadorCollider").transform.GetComponent<RotateOnDrag>().ApplyOffset(newPos - m_posicionInstancioacionLanzador);*/
            m_posicionInstancioacionLanzador = newPos;
        }


        Vector3 cameraProjection = new Vector3(Camera.main.transform.position.x, 0.1f, Camera.main.transform.position.z);
        Quaternion groundLookAt = Quaternion.LookRotation((cameraProjection - newPos).normalized);
        GameObject model = GameObject.Instantiate(_isGoalkeeper ? (goalkeepers[_index]) : (throwers[_index]), newPos, groundLookAt) as GameObject;
        model.GetComponent<Animation>()[model.GetComponent<Animation>().clip.name].normalizedTime = Random.Range(0f, 1f);
        if (_equipacion != null)
        {
            model.transform.FindChild("Body").gameObject.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", _isGoalkeeper ? EquipacionManager.instance.m_texturasPortero[_equipacion.idTextura] : EquipacionManager.instance.m_texturasLanzador[_equipacion.idTextura]);
            model.transform.FindChild("Body").gameObject.GetComponent<Renderer>().materials[0].color = Color.grey;
        }
        return (model);
    }


    public Vector3 GetGroundScreenProjection(Vector3 _screenPoint)
    {
        Vector3 screenPixels = new Vector3(_screenPoint.x * Screen.width, _screenPoint.y * Screen.height, 0);
        Ray worldPosTh = Camera.main.ScreenPointToRay(screenPixels);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        float hitDistance;
        groundPlane.Raycast(worldPosTh, out hitDistance);
        Debug.DrawRay(worldPosTh.origin, worldPosTh.direction * 10f, Color.red, 10f);
        Vector3 newPos = worldPosTh.GetPoint(hitDistance);
        return newPos;
    }

    /*
    public void CalculateRevealLogros()
    {
        bool hayNuevosLogros = false;

        List<string> nuevos = new List<string>();
        if(lastProgressLogros == null)
        {
            lastProgressLogros = progressLogros;
            return;
        }
        foreach(string s in progressLogros)
        {
            if(!lastProgressLogros.Contains(s))
            {
                nuevos.Add(s);
                hayNuevosLogros = true;
            }
        }
        lastProgressLogros = progressLogros;

        // si hay nuevos logros => mostrar alerta en barra de navegacion y un dialogo
        if (hayNuevosLogros) {
            cntBarraSuperior.instance.MostrarQueHayNuevosLogros();
            ifcRevealLogros.Instance.Show(nuevos.ToArray());
        }
        return;
    }
    */


    /// <summary>
    /// Metodo para solicitar todos los datos del usuario a los web services
    /// </summary>
    public void UpdateUserInfoFromWS()
    {
        iniciadaSesionConFacebook = true; // F4KE => habilita boton multiplayer y la barra superior


        // intentar conectar contra los webservices con las credenciales de facebook
        /*
        connect(
            // callback si todo ha ido OK
            (_name) => {
                // pedir los datos de usuario, los items comprados y los rankings
                getStats();
                getItemsComprados();
                getRanking(5);
            },
            // callback si ERROR
            (_name) => {
                Interfaz.iniciadaSesionConFacebook = false;
                ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "NO SE PUDO INICIAR SESIÓN", "No se ha podido iniciar sesión en Bitoon Kicks");
            });
        */
        /*
        m_uid = "41253abb-1a23-4912-8ef0-43b5078e9e99";  // <= F4KE
        m_sessionToken = "C53A602B068304E586FEB1EE653DF32132E25F9E334F91D42AF00D06"; // <= F4KE

         // <= F4KE
         */
    }



    #region DEPRECATED

    /*
    /// <summary>
    /// Obtiene el progreso del usuario
    /// </summary>
    void getStats() {
        DownloadDaemon.instance.callWS(baseUrl + "/" + gameUrl + "/rest/main/progress/" + m_sessionToken + "/",   // m_uname
            (object _ret) => {

                Dictionary<string, System.Object> progress = _ret as Dictionary<string, System.Object>;
                if (progress != null) {

                    // obtener el tiempo
                    m_time = (int) progress["spentTime"];

                    // obtener las mejores puntuaciones
                    Player.record_keeper = (int) progress["bestGoalKeeperScoring"];
                    Player.record_thrower = (int) progress["bestShooterScoring"];
                    Interfaz.m_asKeeper.record = (int) progress["bestGoalKeeperScoring"];
                    Interfaz.m_asThrower.record = (int) progress["bestShooterScoring"];

                    // calcular y actualizar el dinero del jugador
                    Dictionary<string, object> monedas = progress["currency"] as Dictionary<string, object>;
                    Interfaz.MonedasSoft = (int) monedas["softCurrency"];
                    Interfaz.MonedasHard = (int) monedas["hardCurrency"];

                    Debug.LogWarning(">>> Dinero SOFT=" + Interfaz.MonedasSoft + "   HARD=" + Interfaz.MonedasHard);

                    cntBarraSuperior.instance.ActualizarDinero();

                    if (progress.ContainsKey("nextTryTime"))
                        m_nextTryTime = (int) progress["nextTryTime"];

                    // obtener los logros conseguidos
                    Dictionary<string, System.Object> achievements = progress["achievements"] as Dictionary<string, System.Object>;
                    if (achievements != null) {
                        if (achievements["list"] is string) {
                            cntLogros.instance.Unlock(achievements["list"] as string);
                            progressLogros.Add(achievements["list"] as string);
                        } else {
                            List<System.Object> achievementsList = achievements["list"] as List<System.Object>;
                            if (achievementsList != null) {
                                m_achievements = 0;
                                foreach (System.Object ele in achievementsList) {
                                    Dictionary<string, System.Object> logro = ele as Dictionary<string, System.Object>;
                                    cntLogros.instance.Unlock(logro["achCode"].ToString());
                                    progressLogros.Add(logro["achCode"].ToString());
                                }
                            }
                        }
                    }

                    // obtener el avance como portero
                    Dictionary<string, System.Object> goalkeeper = progress["goalkeeper"] as Dictionary<string, System.Object>;
                    if (goalkeeper != null) {
                        m_asKeeper.targets = (int) goalkeeper["targets"];
                        m_asKeeper.goals = (int) goalkeeper["goals"];
                        m_asKeeper.goalsStopped = (int) goalkeeper["goalsStopped"];
                        m_asKeeper.throwOut = (int) goalkeeper["out"];
                        m_asKeeper.totalPoints = (int) goalkeeper["totalPoints"];
                        m_asKeeper.deflected = (int) goalkeeper["deflected"];
                    } else
                        Debug.Log("NO goalkeeper");

                    // obtener el avance como lanzador
                    Dictionary<string, System.Object> shooter = progress["shooter"] as Dictionary<string, System.Object>;
                    if (shooter != null) {
                        m_asThrower.targets = (int) shooter["targets"];
                        m_asThrower.goals = (int) shooter["goals"];
                        m_asThrower.goalsStopped = (int) shooter["goalsStopped"];
                        m_asThrower.throwOut = (int) shooter["out"];
                        m_asThrower.totalPoints = (int) shooter["totalPoints"];
                        m_asThrower.deflected = (int) shooter["deflected"];
                    } else
                        Debug.Log("NO shooter");
                    
                    // comprobar que misiones han sido desbloqueadas
                    if (progress.ContainsKey("missions")) {
                        List<System.Object> listaMisiones = progress["missions"] as List<System.Object>;
                        if (listaMisiones != null) {
                            foreach (System.Object elem in listaMisiones) {
                                Dictionary<string, System.Object> infoMision = elem as Dictionary<string, System.Object>;
                                // marcar la mision como desbloqueada
                                GameLevelMission misionInfo = MissionManager.instance.GetGameLevelMission(infoMision["missionId"].ToString());
                                misionInfo.MissionUnlocked = true;

                                Debug.LogWarning(">>> Mision desbloqueada: " + infoMision["missionId"]); 
                            }
                        } else
                            Debug.Log("NO missions");
                    } else
                        Debug.Log("NO missions");

                    // actualizar la interfaces relacionadas
                    ifcPerfil.instance.Refresh();
                    ifcLogros.instance.Refresh();
                    ifcCarrera.instance.Refresh();

                    ifcMainMenu.instance.SetButton("btnLogros", true);
                    
                } else
                    Debug.Log("NO PROGRESS");

                //ifcUsuario.instance.Refresh(m_uname);

                if (m_partidas > 0)
                    ifcMainMenu.instance.SetButton("btnJugar", true);
            },
            (string _ret) => {
                Debug.Log("ER >>>> getStats " + _ret);
            }); //, form);
    }


    /// <summary>
    /// Realiza el proceso de compra de un escudo contra los webservices
    /// </summary>
    /// <param name="_escudo"></param>
    public void comprarEscudo(Escudo _escudo) {
        if (_escudo == null)
            return;

        // comprobar que el usuario tenga suficiente dinero para la compra
        if (Interfaz.m_monedasHard < _escudo.precioHard) {
            ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "AVISO", "No dispones de suficientes monedas para comprar el " + _escudo.nombre + ".");
            return;
        }

        // llamar al web service de la compra
        comprarItem(_escudo.id, 1, false,
            // callback si todo OK
            (string _ret) => {
                // incrementar la cantidad de escudos y repintar la tienda
                ++_escudo.numUnidades;
                if (ifcVestuario.instance != null)
                    ifcVestuario.instance.RefreshInfo();
            },
            // callback si error
            (string _ret) => {
                Debug.Log("ER >>>> comprarEscudo " + _ret);
            });
    }
    
    
    /// <summary>
    /// Paga un duelo
    /// </summary>
    /// <param name="_jugador"></param>
    public void pagarDuelo() {
        string idItemDuelo = "IT_MTP_01";   // <= id del articulo en la tienda

        // llamar al webservice para efectuar la compra
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/market/purchase/" + m_uid,
            // callback si todo OK
            (object _ret) => {
                // actualizar el dinero del usuario en local
                m_monedasSoft -= Stats.PRECIO_RETO;

                Debug.LogWarning(">>> RETO PAGADO");
            },
            // callback si error
            (string _ret) => {
                Debug.Log("ER >>>> comprarModoJuego " + _ret);
            },
            "{\"itemCode\":\"" + idItemDuelo + "\", \"quantity\":1}");
    }

    
    void getRanking()
    {
        WWWForm form = new WWWForm();
        form.AddField("param", "{'uid':'" + m_uid + "', 'maxResults'=" + 5 + "}");
        DownloadDaemon.instance.callWS(baseUrl + "/" + gameUrl + "/rest/main/rankings",
            (object _ret) => {
                Dictionary<string, System.Object> rankings = _ret as Dictionary<string, System.Object>;
//                Dictionary<string, System.Object> rankings = ret["rankings"] as Dictionary<string, System.Object>;
                if (rankings != null) {
                    Dictionary<string, System.Object> playerShooter = rankings["playerShooter"] as Dictionary<string, System.Object>;
                    if (playerShooter != null) {
                        ifcRanking.instance.m_entries[5].m_pos = (int)playerShooter["rank"];
                        ifcRanking.instance.m_entries[5].m_name = playerShooter["alias"] as string;
                        ifcRanking.instance.m_entries[5].m_points = (int)playerShooter["score"];
                    }

                    List<System.Object> shooters = rankings["shooters"] as List<System.Object>;
                    if (shooters != null) {
                        foreach (System.Object ent in shooters) {
                            Dictionary<string, System.Object> e = ent as Dictionary<string, System.Object>;
                            if (e != null) {
                                int r = (int)e["rank"];
                                ifcRanking.instance.m_entries[r - 1].m_pos = r;
                                ifcRanking.instance.m_entries[r - 1].m_name = e["alias"] as string;
                                ifcRanking.instance.m_entries[r - 1].m_points = (int)e["score"];
                            }
                        }
                    }

                    Dictionary<string, System.Object> playerGoalKeeper = rankings["playerGoalKeeper"] as Dictionary<string, System.Object>;
                    if (playerGoalKeeper != null) {
                        ifcRanking.instance.m_entries[11].m_pos = (int)playerGoalKeeper["rank"];
                        ifcRanking.instance.m_entries[11].m_name = playerGoalKeeper["alias"] as string;
                        ifcRanking.instance.m_entries[11].m_points = (int)playerGoalKeeper["score"];
                    }

                    List<System.Object> goalKeepers = rankings["goalKeepers"] as List<System.Object>;
                    if (goalKeepers != null) {
                        foreach (System.Object ent in goalKeepers) {
                            Dictionary<string, System.Object> e = ent as Dictionary<string, System.Object>;
                            if (e != null) {
                                int r = (int)e["rank"];
                                ifcRanking.instance.m_entries[6+r - 1].m_pos = r;
                                ifcRanking.instance.m_entries[6+r - 1].m_name = e["alias"] as string;
                                ifcRanking.instance.m_entries[6+r - 1].m_points = (int)e["score"];
                            }
                        }
                    }

                    Dictionary<string, System.Object> playerGlobal = rankings["playerGlobal"] as Dictionary<string, System.Object>;
                    if (playerGlobal != null) {
                        ifcRanking.instance.m_entries[17].m_pos = (int)playerGlobal["rank"];
                        ifcRanking.instance.m_entries[17].m_name = playerGlobal["alias"] as string;
                        ifcRanking.instance.m_entries[17].m_points = (int)playerGlobal["score"];
                    }

                    List<System.Object> globals = rankings["globals"] as List<System.Object>;
                    if (globals != null) {
                        foreach (System.Object ent in globals) {
                            Dictionary<string, System.Object> e = ent as Dictionary<string, System.Object>;
                            if (e != null) {
                                int r = (int)e["rank"];
                                ifcRanking.instance.m_entries[12 + r - 1].m_pos = r;
                                ifcRanking.instance.m_entries[12 + r - 1].m_name = e["alias"] as string;
                                ifcRanking.instance.m_entries[12 + r - 1].m_points = (int)e["score"];
                            }
                        }
                    }
                }
                ifcRanking.instance.Refresh();
                ifcMainMenu.instance.SetButton("btnRanking", true);
            },
            (string _ret) => {
                Debug.Log("ER getRanking >>>> " + _ret);
            }, form);
    }  
      
    
    
    /// <summary>
    /// Obtiene la lista de favoritos de los servicios web y la almacena en "UsuariosFavoritos.instance"
    /// </summary>
    /// <param name="_uid"></param>
    public void getContacts(string _uid) {
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/multi/contacts",
            // callback si todo OK
            (object _ret) => {
                // generar la lista de favoritos
                List<Usuario> listaFavoritos = new List<Usuario>();

                // obtener la informacion de cada uno de los contactos
                Dictionary<string, System.Object> content = _ret as Dictionary<string, System.Object>;
                List<System.Object> listaContactos = content["contacts"] as List<System.Object>;
                foreach (System.Object contentItem in listaContactos) {
                    Dictionary<string, System.Object> infoUsuario = contentItem as Dictionary<string, System.Object>;

                    if (infoUsuario != null) {
                        listaFavoritos.Add(new Usuario(
                            infoUsuario["alias"] as string,
                            (int) infoUsuario["won"],
                            (int) infoUsuario["lost"]));
                    }
                }

                // guardar la lista de favoritos
                UsuariosFavoritos.instance.SetFavoritos(listaFavoritos);

                // actualizar el pintado del listado favoritos
                if (ifcFavoritos.instance != null)
                    ifcFavoritos.instance.UpdatePaginaListadoFavoritos();
            },
            // callback si error
            (string _ret) => {
                Debug.Log("ER >>>> getContacts " + _ret);
            },
            "{\"uid\":\"" + _uid + "\"}");
    }
    

    /// <summary>
    /// Añade un usuario a la lista de favoritos
    /// </summary>
    /// <param name="_alias">Nombre del usuario a añadir</param>
    /// <param name="_esRobot">True si el usuario a añadir es un robot</param>
    /// <param name="_onOkCallback">Callback a ejecutar si todo ha hido ok</param>
    /// <param name="_onErrorCallback">Callback a ejecutar si se produce un error</param>
    public void addContacts(string _alias, bool _esRobot, btnButton.guiAction _onOkCallback, btnButton.guiAction _onErrorCallback) {
        // si el usuario es un bot => añadirlo a la lista de favoritos como bot
        if (_esRobot) {
            // añadir el usuario a la lista de bots
            UsuariosFavoritos.instance.AddFavoritoBot(new Usuario(_alias, 0, 0, 0, _esRobot));

            // actualizar el pintado del listado favoritos
            if (ifcFavoritos.instance != null)
                ifcFavoritos.instance.UpdatePaginaListadoFavoritos();
        } else {
            // el usuario NO es un bot => realizar la consulta al webservice
            DownloadDaemonJSON.instance.callWS(
                // URL del servicio
                baseUrl + "/" + gameUrl + "/rest/multi/add",
                // callback si todo OK
                (object _ret) => {
                    Debug.Log(">>> Contacto añadido OK");

                    // pedir de nuevo la lista de contactos a los WS
                    getContacts(m_uid);

                    // comprobar si hay que ejecutar el callback de todo ok
                    if (_onOkCallback != null)
                        _onOkCallback("");
                },
                // callback si error
                (string _ret) => {
                    Debug.Log("ER >>>> addContact " + _ret);
                    // comprobar si hay que ejecutar el callback de error
                    if (_onErrorCallback != null)
                        _onErrorCallback("");
                },
                // parametros de la llamada
                "{\"uid\":\"" + m_uid + "\",\"aliases\":[\"" + _alias + "\"]}");
        }
    }


    /// <summary>
    /// Elimina un usuario de la lista de favoritos
    /// </summary>
    /// <param name="_alias">Nombre del usuario a eliminar</param>
    /// <param name="_onErrorCallback">Callback a ejecutar si se produce un error</param>
    public void deleteContacts(string _alias, btnButton.guiAction _onErrorCallback = null) {
        // intentar eliminar el usuario de la lista de bots
        if (UsuariosFavoritos.instance.DeleteFavoritoBot(_alias)) {
            // si se esta mostrando el listado de favoritos => pintarla de nuevo
            if (ifcFavoritos.instance != null)
                ifcFavoritos.instance.UpdatePaginaListadoFavoritos();
        } else {
            // el usuario NO es un bot => intentar eliminarlo llamando a los webservices
            DownloadDaemonJSON.instance.callWS(
                // URL del servicio
                baseUrl + "/" + gameUrl + "/rest/multi/delete",
                // callback si todo OK
                (object _ret) => {
                    Debug.Log(">>> Contacto eliminado OK");

                    // actualizar la lista de usuarios en memoria
                    UsuariosFavoritos.instance.DeleteFavorito(_alias);

                    // si se esta mostrando el listado de favoritos => pintarla de nuevo
                    if (ifcFavoritos.instance != null)
                        ifcFavoritos.instance.UpdatePaginaListadoFavoritos();

                },
                // callback si error
                (string _ret) => {
                    Debug.Log("ER >>>> deleteContact " + _ret);

                    // comprobar si hay que ejecutar el callback de error
                    if (_onErrorCallback != null)
                        _onErrorCallback("");
                },
                // parametros de la llamada
                "{\"uid\":\"" + m_uid + "\",\"aliases\":[\"" + _alias + "\"]}");
        }
    }

    
    /// <summary>
    /// Compra un modo de juego
    /// </summary>
    /// <param name="_jugador"></param>
    public void comprarModoJuego(ModoJuego _modoJuego) {
        // comprobar que el usuario tenga suficiente dinero para la compra
        if (Interfaz.m_monedas < _modoJuego.precioDesbloqueo) {
            ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CANCELAR, "AVISO", "No dispones de suficientes monedas para desbloquear el modo " + _modoJuego.nombre + ".");
            return;
        }

        // llamar al webservice para efectuar la compra
        DownloadDaemonJSON.instance.callWS(
            // URL del servicio
            baseUrl + "/" + gameUrl + "/rest/market/purchase/" + m_uid,
            // callback si todo OK
            (object _ret) => {
                // marcar el modo de juego como ADQUIRIDO y repintarlo
                _modoJuego.estado = ModoJuego.Estado.ADQUIRIDO;
                if (ifcJugar.instance != null)
                    ifcJugar.instance.RefrescarModosDeJuego();

                // actualizar el dinero del usuario
                m_monedas -= _modoJuego.precioDesbloqueo;
            },
            // callback si error
            (string _ret) => {
                Debug.Log("ER >>>> comprarModoJuego " + _ret);
            },
            "{\"itemCode\":\"" + _modoJuego.codigo + "\", \"quantity\":1}");
    }


    void getAuth()
    {
        WWWForm form = new WWWForm();
        form.AddField("param", authCode );
        DownloadDaemon.instance.callWS(baseUrl + "/" + gameUrl + "/rest/authservice/getUID",
            (object _ret) => {
                Dictionary<string, System.Object> content = _ret as Dictionary<string, System.Object>;
                m_uname = content["alias"] as string;
                m_uid = content["gameId"] as string;
                m_ts = (int)content["ts"];
                getStats();
                getItemsComprados(m_uid);
                getRanking();

                // una vez se conoce el id del usuario => pedir sus favoritos
                Interfaz.instance.getContacts(m_uid);   // "6CB23ACB"

                // F4KE: generar una lista de usuarios favoritos F4KE
                //UsuariosFavoritos.instance.F4KE_GenerarInstanciaParaPruebas(25);
            },
            (string _ret) =>
            {
                Debug.Log("ER >>>> getAuth " + _ret);
            }, form);
    }
      
     
     
    public static void sendRound(bool _goalkeeper, ResultType _result, int _points, float _recompensaRatio, int _minuts=0){
    char[] res= new char[]{'e','a','d','f','t'};
        WWWForm form = new WWWForm();
        form.AddField("param", "{'uid':'" + m_uid + "','mode':'" + (_goalkeeper ? "p" : "t") + "','result':'" + res[(int) _result] + "','points':" + _points + ", 'matchEnd':" + _minuts + ",'ratio':" + _recompensaRatio + "}");
            DownloadDaemon.instance.callWS(baseUrl + "/" + gameUrl + "/rest/main/track", 
            (object _ret)=> {
                Dictionary<string, System.Object> achievements = _ret as Dictionary<string, System.Object>;
//                Dictionary<string, System.Object> achievements = ret["achievements"] as Dictionary<string, System.Object>;
                if (achievements != null)
                {
                    List<System.Object> achievementsList = achievements["list"] as List<System.Object>;
                    if (achievementsList != null){
                        foreach (System.Object ele in achievementsList)
                        {
                            Dictionary<string, System.Object> logro = ele as Dictionary<string, System.Object>;
                            addLogro(logro["achCode"].ToString());
                            Debug.Log("MULTILOG " + ele as string);
                        }
                        if (ifcLogros.instance) ifcLogros.instance.Refresh();
                    }
                }
            },
            (string _ret)=> {
                Debug.Log("ER >>>> sendRound " + _ret);
            }, form);
    }
    */

    #endregion




    // referencia a los objetos que permiten rotar los modelos de los jugadores
    private RotateOnDrag m_goGirarLanzador;
    private RotateOnDrag m_goGirarPortero;
    private bool m_previousMostrarLanzador = true;
    private bool m_previousMostrarPortero = true;

    /// <summary>
    /// Metodo para mostrar u ocultar los modelos de los jugadores
    /// </summary>
    /// <param name="_mostrarLanzador"></param>
    /// <param name="_mostrarPortero"></param>
    public void RefrescarModelosJugadores(bool _mostrarLanzador, bool _mostrarPortero, bool _forzar = false)
    {
        // obtener las referencias a los objetos que permiten rotar los modelos de los jugadores

        if (m_goGirarLanzador == null)
            m_goGirarLanzador = GameObject.Find("girarLanzador/girarLanzadorCollider").GetComponent<RotateOnDrag>();
        if (m_goGirarPortero == null)
            m_goGirarPortero = GameObject.Find("girarPortero/girarPorteroCollider").transform.GetComponent<RotateOnDrag>();

        if (!_forzar && (m_previousMostrarPortero == _mostrarPortero) && (m_previousMostrarLanzador == _mostrarLanzador)) return; //optimizacion
        m_previousMostrarPortero = _mostrarPortero;
        m_previousMostrarLanzador = _mostrarLanzador;

        // mostrar / ocultar los modelos de los jugadores
        if (throwerModel != null)
            throwerModel.gameObject.SetActive(_mostrarLanzador);

        if (goalkeeperModel != null)
            goalkeeperModel.gameObject.SetActive(_mostrarPortero);

        if (_mostrarLanzador)
        {
            Thrower = Thrower;
            if (!_mostrarPortero) GameObject.Find("girarPortero").transform.position = Vector3.zero;
        }

        if (_mostrarPortero)
        {
            Goalkeeper = Goalkeeper;
            if (!_mostrarLanzador) GameObject.Find("girarLanzador").transform.position = Vector3.zero;
        }

        /*// si se muestran los dos jugadores
        if (_mostrarLanzador && _mostrarPortero) {
            // llevar a los modelos de los jugadores y los objetos que permiten rotarlos a su posicion original
            throwerModel.transform.position = m_posicionInstancioacionLanzador;
            m_goGirarLanzador.ClearOffset();
            goalkeeperModel.transform.position = m_posicionInstanciacionPortero;
            m_goGirarPortero.ClearOffset();

        } else {
            // mostrar a los jugadores en la posicion central a su punto de instanciacion
            Vector3 posMedia = GetGroundScreenProjection(new Vector3(Stats.PLAYER_VESTUARIO_COORDENADA_X, 0.225f, 0f));//(m_posicionInstancioacionLanzador + m_posicionInstanciacionPortero) / 2.0f;
            Vector3 posLanzador = _mostrarLanzador ? posMedia : Vector3.zero;
            Vector3 posPortero = !_mostrarLanzador ? posMedia : Vector3.zero;

            throwerModel.transform.position = posLanzador;
            goalkeeperModel.transform.position = posPortero;

            m_goGirarLanzador.ApplyOffset(posLanzador - m_posicionInstancioacionLanzador);
            m_goGirarPortero.ApplyOffset(posPortero - m_posicionInstanciacionPortero);
        }*/

    }


    /// <summary>
    /// Devuelve true si el modelo de lanzador esta visible
    /// </summary>
    /// <returns></returns>
    public bool GetModeloLanzadorEstaVisible()
    {
        return throwerModel.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Devuelve true si el modelo de portero esta visible
    /// </summary>
    /// <returns></returns>
    public bool GetModeloPorteroEstaVisible()
    {
        return goalkeeperModel.gameObject.activeInHierarchy;
    }

}

