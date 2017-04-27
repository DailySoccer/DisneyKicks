//#define SHOW_DATA

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System.Net;

public class DownloadDaemon : MonoBehaviour {
  public static string baseURL = "https://kicksws.bitoon.mad";//"https://ws.ligabbvagame.com";
  public static string mediaURL = "https://kicksws.bitoon.mad";//"https://ws.ligabbvagame.com";
  static DownloadDaemon m_instance;
  public static DownloadDaemon instance { get { if (m_instance == null) {m_instance = (new GameObject("DownloadDaemon")).AddComponent<DownloadDaemon>(); GameObject.DontDestroyOnLoad(m_instance.gameObject); }return m_instance; } }

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
    //System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
    m_header = new Dictionary<string, string>();
//    m_header.Add("Accept-Encoding", "deflate, gzip");
    
    m_header.Add("Content-Type", "application/x-www-form-urlencoded");
    //m_header.Add("Content-Type", "/application/json"); // <= ¿le sobra la "/" delante?

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
  public Monitor callWS(string _url, callBack _ok = null, stringCallBack _error = null, WWWForm _form=null) {
    return AddFile(_url[0] == '/' ? baseURL + _url : _url, _ok, _error, _form);
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

  Monitor AddFile(string _url, callBack _ok = null, stringCallBack _error = null, WWWForm _form = null) {
    m_pending++;
    gameObject.SetActive(true);
    if (m_postData == null) m_postData = System.Text.UTF8Encoding.UTF8.GetBytes("pragma=no-cache");
    WWW www;
    if (_form != null) {
      _form.AddField("pragma", "no-cache");
      www = new WWW(_url, _form.data, m_header);
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
          if (_www.responseHeaders.ContainsKey("SET-COOKIE")) {
            string setCookie = _www.responseHeaders["SET-COOKIE"];
            int ini = setCookie.IndexOf("JSESSIONID=");
            if (ini != -1) {
              m_session = setCookie.Substring(ini, setCookie.IndexOf(';', ini) - ini);
              m_header["Cookie"] = m_session;
            }
          }
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
//                  Debug.Log(System.Text.UTF8Encoding.ASCII.GetString(res)); //traza log WS
                  if (_ok != null) _ok( (IDictionary)MiniJSON.Json.Deserialize(System.Text.UTF8Encoding.ASCII.GetString(res)) );
                  break;
                case "text/plain":
                default:
                  Debug.Log(res.Length + " " + System.Text.UTF8Encoding.ASCII.GetString(res));
                  if (_ok!=null) _ok(System.Text.UTF8Encoding.ASCII.GetString(res));
                  break;
              }
            }
          }
        } catch(System.Exception _e) {
          if (_error != null) _error("["+_www.error+"] " + _e.ToString() );
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

  public static Hashtable SplitParams(string _string ){
    Hashtable tmp = new Hashtable();
    string[] pairs = _string.Split(';');
    foreach (string pair in pairs) {
      string[] param = pair.Split('=');
      tmp.Add(param[0].ToLower(), param[1]);
    }
    return tmp;
  }
      

}
