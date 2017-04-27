using UnityEngine;
using System.Collections;

public class btnEquipation : MonoBehaviour
{
    public Texture m_unlock;
    public Texture m_lock;
    public Texture m_select;

    bool m_selected = false;

    static btnEquipation m_oldSelected = null;

    GameObject m_text;

	void Awake () {
        GetComponent<GUITexture>().texture = m_lock;
	}

    void Start() {
        m_text = transform.Find("Text").gameObject;
        doLock();
    }

    public delegate void guiAction(string _name);
    public guiAction action { private get; set; }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void doLock()
    {
        GetComponent<GUITexture>().texture = m_lock;
        m_text.SetActive(false);
        gameObject.layer = 1 << 1;
    }

    public void unlock()
    {
        m_selected = false;
        GetComponent<GUITexture>().texture = m_unlock;
        m_text.SetActive(true);
        gameObject.layer = 0;
    }

    public static void reset()
    {
        m_oldSelected = null;
    }

    public void select()
    {
        if (m_oldSelected!=null && m_oldSelected.gameObject.layer == 0) m_oldSelected.unlock();
        m_selected = true;
        GetComponent<GUITexture>().texture = m_select;
        m_text.SetActive(true);
        gameObject.layer = 0;

        m_oldSelected = this;
    }

    void OnMouseEnter()
    {
        if (m_selected) return;
        GetComponent<GUITexture>().texture = m_select;
    }

    void OnMouseExit()
    {
        if (m_selected) return;
        GetComponent<GUITexture>().texture = m_unlock;
    }

    void OnMouseUpAsButton()
    {
        select();
        if (action != null) action(name);
    }
}
