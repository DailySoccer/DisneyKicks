using UnityEngine;
using System.Collections;

public class btnLogro : MonoBehaviour
{
    public Texture m_unlock;
    public Texture m_lock;
    LogrosDescription.descLogro m_logro;
    Color m_colorLock;
    Color m_colorUnlock;

    GameObject m_icon;
    // Use this for initialization
	void Awake () {
        GetComponent<GUITexture>().texture = m_lock;
	}

    void Start() {
        m_colorLock = new Color(1,1,1,0.5f);
        m_colorUnlock = Color.white;
        m_icon = transform.Find("icono").gameObject;
    }

    public delegate void guiAction(string _name);
    public guiAction action { private get; set; }


    void OnMouseEnter()
    {
        new SuperTweener.scale(gameObject, 0.1f, new Vector3(0.005f, 0.005f, 1), SuperTweener.ElasticInOut, ( _obj1) => { new SuperTweener.scale(gameObject, 0.1f, new Vector3(0, 0, 1), SuperTweener.ElasticInOut); } );
    }

    public void set(LogrosDescription.descLogro _logro){
        m_logro = _logro;
        Texture t = Resources.Load("Icos/" + m_logro.m_codigo) as Texture;
        m_icon.GetComponent<GUITexture>().texture = t;
        m_icon.GetComponent<GUITexture>().color = m_logro.m_desbloqueado ? m_colorUnlock : m_colorLock;

        Rect r = m_icon.GetComponent<GUITexture>().pixelInset;
        r.x = -t.width *ifcBase.scaleFactor / 2.0f;
        r.y = -t.height *ifcBase.scaleFactor / 2.0f;
        r.width = t.width *ifcBase.scaleFactor;
        r.height = t.height *ifcBase.scaleFactor;
        m_icon.GetComponent<GUITexture>().pixelInset = r;

        GetComponent<GUITexture>().texture = m_logro.m_desbloqueado ? m_unlock : m_lock;

        action = (string _name) => {
            ifcTooltip.instance.showLogro(m_logro.m_codigo);
            Vector3 p = transform.position;
//                Vector3 p =Camera.mainCamera.ScreenToViewportPoint(Input.mousePosition);
            p.z = 1;
            p.y += 0.08f;
            ifcTooltip.instance.transform.position = p;
        };
    }


    void OnMouseUpAsButton()
    {
        if (action != null) action(name);
    }
}
