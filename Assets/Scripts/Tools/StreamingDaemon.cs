using UnityEngine;
using System;
using System.Xml;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;

public class StreamingDaemon : MonoBehaviour
{
  public delegate void petitionCallback(AssetBundle _bundle, System.Object _userData);
  public delegate void progressCallback(float _progress, System.Object _userData);

  public struct Petition{
    public WWW www;
    public System.Object userData;
    public petitionCallback onDone;
    public progressCallback onProgress;
    public Petition(string _url, petitionCallback _onDone, progressCallback _onProgress, System.Object _userData){
      userData = _userData;
      onDone = _onDone;
      onProgress = _onProgress;
#if (!UNITY_WEBPLAYER)
      www = new WWW(_url);
#else
//      www = WWW.LoadFromCacheOrDownload(_url, 1);
      www = new WWW(_url);
/*
      MemoryStream ms = new MemoryStream();
      GZipStream Compress = new GZipStream(ms, CompressionMode.Decompress);
      Compress.Write( www.bytes, 0, www.bytes.Length );
      Compress.Close();
      byte[] bt = ms.GetBuffer();
*/
#endif
      www.threadPriority  = ThreadPriority.Normal;
    }

    public Petition Clone(petitionCallback _onDone, progressCallback _onProgress, System.Object _userData) {
    Petition tmp = new Petition();
      tmp.www = www;
      tmp.userData = _userData;
      tmp.onDone = _onDone;
      tmp.onProgress = _onProgress;
    return tmp;
    }
  };

  static StreamingDaemon m_daemon = null;
  protected static void addFileToStream(string _name, petitionCallback _onDone = null, progressCallback _onProgress = null, System.Object _userData = null)
  {
    if (m_daemon == null) m_daemon = new GameObject("StreamingDaemon", typeof(StreamingDaemon)).GetComponent<StreamingDaemon>();
    m_daemon.addFile(_name, _onDone, _onProgress, _userData);
  }

  public static void addPackFile(string _name, petitionCallback _onDone = null, progressCallback _onProgress = null, System.Object _userData = null) {
    if (m_daemon == null) m_daemon = new GameObject("StreamingDaemon", typeof(StreamingDaemon)).GetComponent<StreamingDaemon>();
    if (m_packs.ContainsKey(_name)) {
      AssetBundle ab = m_packs[_name].addRef();
      if (_onDone != null) _onDone(ab, _userData);
    } else
      addFileToStream(_name, _onDone, _onProgress, _userData);
  }

  public static void Unload(AssetBundle _bundle, bool _unloadAllLoadedObjects){
    foreach (KeyValuePair<string, pack> pair in m_packs)
      if (pair.Value.ab == _bundle){
        if( pair.Value.unload() ) m_packs.Remove(pair.Key);
        break;
      }
  }

  public class pack {
    public int refs;
    public AssetBundle ab;
    public pack(AssetBundle _ab) {
      refs = 0;      
      ab = _ab;
      addRef();
    }

    public AssetBundle addRef() { 
      refs++;
      //MyDebug.LogWarning(">>>>>>>>>>>>>>>>>>> AÑADE referencia " + Utils.getReference(ab.GetHashCode()) + " " + refs);
      return ab;
    }

    public bool unload() {
      refs--;
      //MyDebug.LogWarning(">>>>>>>>>>>>>>>>>>> ELIMINA referencia " + Utils.getReference(ab.GetHashCode()) + " " + refs);
      if (refs == 0) {
        //MyDebug.LogWarning("Descarga " + Utils.getReference(ab.GetHashCode()));
        ab.Unload(true); 
        return true; 
      }
      return false;
    }
  };

  static Dictionary<string, pack> m_packs = new Dictionary<string, pack>();
  public static Dictionary<string, pack> packs { get { return m_packs; } set { m_packs = value; } }
  public static AssetBundle getPackFile(string _name) {
    if (m_packs.ContainsKey(_name)) return m_packs[_name].ab;
    return null;
  }

  List<Petition> m_petitions = new List<Petition>();
  public void addFile(string _name, petitionCallback _onDone, progressCallback _onProgress, System.Object _userData){
    foreach (Petition pet in m_petitions){
      if (pet.www.url == _name) {
          m_petitions.Add( pet.Clone(_onDone, _onProgress, _userData) );
        return;
      }
    }
    this.gameObject.SetActive(true);
    m_petitions.Add(new Petition(_name, _onDone, _onProgress, _userData));
  }

  void Awake(){
    GameObject.DontDestroyOnLoad(this.gameObject);
    this.gameObject.SetActive(false);
    this.gameObject.layer = 31;
  }

  void OnDestroy() {
    foreach (var pack in m_packs)
      pack.Value.unload();
  }

  void Update(){
    foreach(Petition petition in m_petitions){
      if (petition.www.isDone){
        m_petitions.Remove(petition);
        if (m_petitions.Count == 0){
            this.gameObject.SetActive(false);
        }
        if (petition.www.error == null){
          int idx = petition.www.url.IndexOf("/StreamingAssets")+1;
          idx = petition.www.url.LastIndexOf("/")+1;
          string name = petition.www.url.Substring(idx, petition.www.url.Length-(idx+5));
		  //Utils.addReference(petition.www.assetBundle.GetHashCode(), name);
					
          if (!m_packs.ContainsKey(name)) m_packs.Add(name, new pack(petition.www.assetBundle));
          
          if (petition.onDone != null) petition.onDone(petition.www.assetBundle, petition.userData );
        }
        else{
          Debug.Log("Error al solicitar el fichero " + petition.www.url);
          if (petition.onDone != null) petition.onDone(null, petition.userData);
        }
        break;
      }
      else{
        if (petition.onProgress != null) petition.onProgress(petition.www.progress, petition.userData);
      }
    }
  }

  static Dictionary<string, string> m_versions;
  public static void processVersionList(string _text) {
#if (UNITY_WEBPLAYER)
    m_versions = new Dictionary<string, string>();
    string[] lines = _text.Split('\n');
    string cmp = "";
    foreach (string line in lines) {
      if (line.Length > 23) {
        string entry = line.TrimEnd(new char[] { (char)13 });
        string idx = entry.Substring(entry.Length - 5, 5).ToLower();
        if (idx == ".pack") {
          idx = entry.Substring(18, entry.Length - (18 + 5));
          try {
            cmp+=">>> " + idx + " " + entry+"\n";
            m_versions.Add(idx, entry);
          } catch (Exception e) {
            Debug.Log("ERROR EN " + idx + " " + entry + " " + e);
          }
        }
      }
    }

    Debug.Log(cmp);

#endif
  }

// GET URLS
  public static string getURL(string _name) {
    string prefix = "/StreamingAssets";
    string subfix = "";
    if (Application.isEditor || Application.platform < RuntimePlatform.OSXWebPlayer) prefix = "file:///" + Application.dataPath + "/StreamingAssets";
    else prefix = Application.dataPath + "/";
	if(Application.platform == RuntimePlatform.Android) prefix = "jar:file://" + Application.dataPath + "!/assets";
	if(Application.platform == RuntimePlatform.IPhonePlayer) prefix = "file:///" +Application.dataPath + "/Raw";
#if (UNITY_WEBPLAYER)
    if (!Application.isEditor && m_versions!=null) {
      // Control de versiones.
      if (m_versions.ContainsKey(_name)) return prefix + m_versions[_name];
    }
#endif
    return prefix + subfix + "/" + _name + ".pack";
  }
}

public class StreamingPack{  
  public delegate void petitionCallback(System.Object _userData, AssetBundle _bundle );
  public delegate void progressCallback(float _progress, int _filesPending, System.Object _userData);

  int m_pendingFiles = 1;

  //string            m_name;
  petitionCallback  m_onDone;
  progressCallback  m_onProgress;
  System.Object     m_userData;

  public StreamingPack( string _name, petitionCallback _onDone, progressCallback _onProgress = null, System.Object _userData = null ){
    //m_name = _name;
    m_onDone = _onDone;
    m_onProgress = _onProgress;
    m_userData = _userData;
  }

  public void addPackFile(string _name, petitionCallback _onDone = null ){
    if (StreamingDaemon.packs.ContainsKey(_name)) {
      StreamingDaemon.packs[_name].addRef();
    }
    else{
      m_pendingFiles++;
      string name = StreamingDaemon.getURL(_name);
      StreamingDaemon.addPackFile( name, onDone, onProgress, _onDone);
    }
  }

  public void close(){    
    onDone(null, null);
  }

  public void onDone(AssetBundle _bundle, System.Object _userData){
    m_pendingFiles--;
    if (_userData != null && _userData is petitionCallback) (_userData as petitionCallback)(m_userData, _bundle);
    if (m_pendingFiles == 0){//Fisnish.      
      if (m_onDone != null) m_onDone(m_userData,null);
    }
  }

  public void onProgress(float _progress, System.Object _userData){
    if (m_onProgress != null) m_onProgress(_progress, m_pendingFiles, _userData);
  }

}
