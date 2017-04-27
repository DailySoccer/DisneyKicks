using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;

public class DownloadDaemonJSON : MonoBehaviour {

	public static string baseURL = "https://kicksws.bitoon.mad";//"https://ws.ligabbvagame.com";
	public static string mediaURL = "https://kicksws.bitoon.mad";//"https://ws.ligabbvagame.com";
    static DownloadDaemonJSON m_instance;
    public static DownloadDaemonJSON instance { get { if (m_instance == null) { m_instance = (new GameObject("DownloadDaemonJSON")).AddComponent<DownloadDaemonJSON>(); GameObject.DontDestroyOnLoad(m_instance.gameObject); } return m_instance; } }

    public int m_pending = 0;

    Dictionary<string, string> m_header = null;
    byte[] m_postData = null;

    string m_session;

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
        m_header = new Dictionary<string, string>();
        //m_header.Add("Content-Type", "application/x-www-form-urlencoded");
        m_header.Add("Content-Type", "application/json");
        m_header.Add("Accept", "application/json");
    }


    /// <summary>
    /// Realiza una petición GET / POST
    /// Nota: Si el (_form != null) entonces el paso de parametros es por POST, si (_form == null) entonces el paso de parametros es por GET y hay que 
    /// pasar los parametros en la "_url"
    /// </summary>
    /// <param name="_url">Url o path relativo al servidor de servicios web. Si se usa el path, debe empezar por '/'</param>
    /// <param name="_ok">delegado(object _ret) ejecutado si descarga se efectuo correctamente.</param>
    /// <param name="_error">delegado(string _ret) ejecutado si descarga se efectuo incorrectamente.</param>
    /// <returns>retorna un monitor para poder </returns>
    public Monitor callWS(string _url, callBack _ok = null, stringCallBack _error = null, string _json = "") {
        return AddFile(_url[0] == '/' ? baseURL + _url : _url, _ok, _error, _json);
    }

    /// <summary>
    /// Añade un fichero al gestor de descargas.
    /// </summary>
    /// <param name="_url">Url de descarga o path relativo al servidor de media. Si se usa el path, debe empezar por '/'</param>
    /// <param name="_ok">delegado(object _ret) ejecutado si descarga se efectuo correctamente.</param>
    /// <param name="_error">delegado(string _ret) ejecutado si descarga se efectuo incorrectamente.</param>
    /// <returns>retorna un monitor para poder </returns>
    public Monitor getMediaFile(string _url, callBack _ok = null, stringCallBack _error = null) {
        return AddFile(_url[0] == '/' ? mediaURL + _url : _url, _ok, _error);
    }



    Monitor AddFile(string _url, callBack _ok = null, stringCallBack _error = null, string _json = "") {
        m_pending++;
        gameObject.SetActive(true);
        if (m_postData == null)
            m_postData = System.Text.UTF8Encoding.UTF8.GetBytes("pragma=no-cache");
        WWW www;
        if (_json != string.Empty) {
            www = new WWW(_url, UTF8Encoding.UTF8.GetBytes(_json), m_header);
        } else
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
                                    if (_ok != null)
                                        _ok(txt);
                                    break;
                                case "application/json":
                                    if (_ok != null)
                                        _ok((IDictionary) MiniJSON.Json.Deserialize(System.Text.UTF8Encoding.UTF8.GetString(res)));
                                    break;
                                case "text/plain":
                                default:
                                    if (_ok != null)
                                        _ok(System.Text.UTF8Encoding.UTF8.GetString(res));
                                    break;
                            }
                        }
                    }
                } catch (System.Exception _e) {
                    if (_error != null)
                        _error(_www.error + _e.ToString());
                }
            } else
                if (_error != null)
                    _error(_www.error + "  (" + _www.url + ")");
        }
        _monitor.Dispose();
        _www.Dispose();
        m_pending--;
        if (m_pending == 0)
            gameObject.SetActive(false);
    }


    public static Hashtable SplitParams(string _string) {
        Hashtable tmp = new Hashtable();
        string[] pairs = _string.Split(';');
        foreach (string pair in pairs) {
            string[] param = pair.Split('=');
            tmp.Add(param[0].ToLower(), param[1]);
        }
        return tmp;
    }
      

}
