//#define SHOW_DATA

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.Security.Cryptography;
using System.Text;

public class BI : MonoBehaviour
{
    public static string baseURL = "http://biservicesdev.bitoon.mad:8080/biservicerest/rest/biservice";     // <= DESARROLLO
    //public static string baseURL = "https://biservices2.bitoon.com/biservicerest/rest/biservice";    // <= PRODUCCION
    public static string PlatformHash { get; private set; }
    static string m_AppID = "g00020";
    static string m_AppVersion = "v00100";
    static string m_PlatformType = "p00005";

    static BI m_instance;
    public static BI instance { get { if (m_instance == null) { m_instance = (new GameObject("BI")).AddComponent<BI>(); GameObject.DontDestroyOnLoad(m_instance.gameObject); } return m_instance; } }

    public int m_pending = 0;

    Dictionary<string, string> m_header = null;
    byte[] m_postData = null;

    public delegate void callBack(object _ret);
    public delegate void stringCallBack(string _ret);

    public class Monitor {
        bool m_disposed = false;
        WWW m_www;
        public float progress { get { return m_www != null ? m_www.progress : 1; } }
        internal bool disposed { get { return m_disposed; } }
        public void Dispose() { m_disposed = true; m_www = null; }
        public Monitor(WWW _www) { m_www = _www; }
    };

    void Awake() {
        //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        m_header = new Dictionary<string, string>();
//        m_header.Add("Accept-Encoding", "deflate, gzip");
        m_header.Add("CONTENT-TYPE", "application/json");

        if (!PlayerPrefs.HasKey("BIPlatformHash")) {
            PlatformHash = Sha1Sum(System.DateTime.Now.Ticks.ToString());
            PlayerPrefs.SetString("BIPlatformHash", PlatformHash);
        }
        else {
            PlatformHash = PlayerPrefs.GetString("BIPlatformHash");
        }
    }

    /// <summary>
    /// Realiza una petición GET.
    /// </summary>
    /// <param name="_url">Url o path relativo al servidor de servicios web. Si se usa el path, debe empezar por '/'</param>
    /// <param name="_ok">delegado(object _ret) ejecutado si descarga se efectuo correctamente.</param>
    /// <param name="_error">delegado(string _ret) ejecutado si descarga se efectuo incorrectamente.</param>
    /// <returns>retorna un monitor para poder </returns>
    public Monitor callWS(string _url, callBack _ok = null, stringCallBack _error = null, string _json = "") {
        return AddFile(_url[0] == '/' ? baseURL + _url : _url, _ok, _error, _json);
    }

    Monitor AddFile(string _url, callBack _ok = null, stringCallBack _error = null, string _json = "")
    {
        m_pending++;
        gameObject.SetActive(true);
        if (m_postData == null) m_postData = System.Text.UTF8Encoding.UTF8.GetBytes("pragma=no-cache");
        WWW www;
        if (_json != string.Empty){
            www = new WWW(_url, Encoding.ASCII.GetBytes(_json), m_header);
        }
        else
            www = new WWW(_url, m_postData, m_header);
        Monitor m = new Monitor(www);
        StartCoroutine(DowloadFile(www, _ok, _error, m));
        return m;
    }

    IEnumerator DowloadFile(WWW _www, callBack _ok, stringCallBack _error, Monitor _monitor) {
        yield return _www;
        if (!_monitor.disposed) {
            if (_www.error == null) {
                try {
                    byte[] res = _www.bytes;
#if SHOW_DATA
          foreach(var pair in _www.responseHeaders ) Debug.Log(pair.Key +": "+pair.Value );
#endif
                    if (_www.responseHeaders.ContainsKey("CONTENT-ENCODING") && _www.responseHeaders["CONTENT-ENCODING"] == "gzip")
                        res = Ionic.Zlib.GZipStream.UncompressBuffer(res);

                    if (_ok != null) {
                        if (_www.responseHeaders.ContainsKey("CONTENT-TYPE")) {
                            string[] type = _www.responseHeaders["CONTENT-TYPE"].Split(';');
                            switch (type[0]) {
                                case "image/jpeg":
                                case "image/png":
                                    Texture2D txt = _www.texture;
                                    txt.name = "downloaded";
                                    if (_ok != null) _ok(txt);
                                    break;
                                case "application/json":
                                    Debug.Log(System.Text.UTF8Encoding.ASCII.GetString(res));
                                    if (_ok != null) _ok((IDictionary)MiniJSON.Json.Deserialize(System.Text.UTF8Encoding.ASCII.GetString(res)));
                                    break;
                                case "text/plain":
                                default:
                                    if (_ok != null) _ok(System.Text.UTF8Encoding.ASCII.GetString(res));
                                    break;
                            }
                        }
                    }
                }
                catch (System.Exception _e) {
                    if (_error != null) _error(_www.error + _e.ToString());
                }
            }
            else
                if (_error != null) _error(_www.error + "  (" + _www.url + ")");
        }
        _monitor.Dispose();
        _www.Dispose();
        m_pending--;
        if (m_pending == 0) gameObject.SetActive(false);
    }

    public static Hashtable SplitParams(string _string) {
        Hashtable tmp = new Hashtable();
        string[] pairs = _string.Split(';');
        foreach (string pair in pairs)
        {
            string[] param = pair.Split('=');
            tmp.Add(param[0].ToLower(), param[1]);
        }
        return tmp;
    }

    public static void AppStart()
    {
        int eventID = 1;
        string l = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
        CallBI(string.Format(
            "<'record':<'clientId':1,'eventType':1,'id':'{0}','appId':'{1}','version':'{2}','platform':'{3}','deviceId':'{4}','timestamp':'{5}','userId':'{6}','event':'e{7}'>>",
            Sha1Sum(l),
            m_AppID,
            m_AppVersion,
            m_PlatformType,
            PlatformHash,
            l,
            Interfaz.m_uid,
            eventID.ToString().PadLeft(5, '0')));
    }

    // _modo: 1- casillas, 2-Iniesta, 3-Multyplayer, 4-Training
    // _ultimaFase: Fase final.
    public static void EndGame(int _modo, int _ultimaFase) 
    {
        try
        {
            int eventID = 2;
            string l = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
            CallBI(string.Format(
                "<'record':<'clientId':1,'eventType':1,'id':'{0}','appId':'{1}','version':'{2}','platform':'{3}','deviceId':'{4}','timestamp':'{5}','userId':'{6}','event':'e{7}','mode':'m{8}','final':'f{9}'>>",
                Sha1Sum(l),
                m_AppID,
                m_AppVersion,
                m_PlatformType,
                PlatformHash,
                l,
                Interfaz.m_uid,
                eventID.ToString().PadLeft(5, '0'),
                _modo.ToString().PadLeft(5, '0'),
                _ultimaFase.ToString().PadLeft(5, '0')));
        }
        catch { }
    }

    // _tipo: 1- Conseguidos, 2- Canjeados.
    public static void PuntosBBVA(int _tipo, int _puntos)
    {
        int eventID = 3;
        string l = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
        CallBI(string.Format(
            "<'record':<'clientId':1,'eventType':1,'id':'{0}','appId':'{1}','version':'{2}','platform':'{3}','deviceId':'{4}','timestamp':'{5}','userId':'{6}','event':'e{7}','type':'t{8}','puntos':'{9}'>>",
            Sha1Sum(l),
            m_AppID,
            m_AppVersion,
            m_PlatformType,
            PlatformHash,
            l,
            Interfaz.m_uid,
            eventID.ToString().PadLeft(5, '0'),
            _tipo.ToString().PadLeft(5, '0'),
            _puntos.ToString()));
    }

    public static void Logro(int _tipo, int _detalle)
    {
        int eventID = 4;
        string l = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
        CallBI(string.Format(
            "<'record':<'clientId':1,'eventType':1,'id':'{0}','appId':'{1}','version':'{2}','platform':'{3}','deviceId':'{4}','timestamp':'{5}','userId':'{6}','event':'e{7}','type':'t{8}','detail':'d{9}'>>",
            Sha1Sum(l),
            m_AppID,
            m_AppVersion,
            m_PlatformType,
            PlatformHash,
            l,
            Interfaz.m_uid,
            eventID.ToString().PadLeft(5, '0'),
            _tipo.ToString().PadLeft(5, '0'),
            _detalle.ToString().PadLeft(5, '0')));
    }

    public static void Login(string _id)
    {
        int eventID = 5;
        string l = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
        CallBI(string.Format(
            "<'record':<'clientId':1,'eventType':1,'id':'{0}','appId':'{1}','version':'{2}','platform':'{3}','deviceId':'{4}','timestamp':'{5}','userId':'{6}','event':'e{7}','userId':'t{8}'>>",
            Sha1Sum(l),
            m_AppID,
            m_AppVersion,
            m_PlatformType,
            PlatformHash,
            l,
            Interfaz.m_uid,
            eventID.ToString().PadLeft(5, '0'),
            _id ));
    }
    // Error en cortapega
    public static void Publicidad(int _tipo, int _contenido)
    {
        int eventID = 6;
        string l = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
        CallBI(string.Format(
            "<'record':<'clientId':1,'eventType':1,'id':'{0}','appId':'{1}','version':'{2}','platform':'{3}','deviceId':'{4}','timestamp':'{5}','userId':'{6}','event':'e{7}','type':'z{8}','contenido':'c{9}'>>",
            Sha1Sum(l),
            m_AppID,
            m_AppVersion,
            m_PlatformType,
            PlatformHash,
            l,
            Interfaz.m_uid,
            eventID.ToString().PadLeft(5, '0'),
            _tipo.ToString().PadLeft(5, '0'),
            _contenido.ToString().PadLeft(5, '0')));
    }


    public static void CallBI(string _data) {
        BI.instance.callWS("/record", (object _ret) => { }, (string _ret) => { Debug.Log("ER >>>> BI " + _ret); }, _data.Replace('<', '{').Replace('>', '}'));
    }

    public static string Sha1Sum(string strToEncrypt) {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        // encrypt bytes
        System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        byte[] hashBytes = sha1.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        return hashString.PadLeft(32, '0');
    }
}
