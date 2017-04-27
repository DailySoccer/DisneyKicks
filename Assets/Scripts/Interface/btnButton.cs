using UnityEngine;
using System.Collections;

public class btnButton : MonoBehaviour
{
    protected Color m_base;
    public Color m_colorOver = Color.white;
    public Texture m_current;
    protected Texture m_IconCurrent;
    public Texture m_over;
    public Texture m_IconOver;
    public Texture m_IconSelect;
    public Texture m_select;
    bool m_hoverState;
    protected bool m_selected = false;
    public float m_zoom = 1f;
    protected bool m_zoomed = false;
    protected Rect m_originalInset;
    protected bool m_hoverAlive;
    public bool m_disabled = false;

    public bool Toggle { get; set; }

    protected GUITexture m_icono;

    void Awake () {
        m_current = GetComponent<GUITexture>().texture;
        
        m_base = GetComponent<GUITexture>().color;
        Toggle = false;

        Transform t = transform.Find("Icono");
        if (t != null) m_icono = t.GetComponent<GUITexture>(); else m_icono = null;
        if (m_icono != null)
        {
            m_icono.color = m_base;
            m_IconCurrent = m_icono.texture;
        }
    }

    public delegate void guiAction(string _name);

    public guiAction action { get; set; }

    public void reset()
    {
        OnMouseExit();
    }

//# if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUITexture t = GetComponent<GUITexture>();
        float dist = 0.5f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMin / Screen.width), (t.pixelInset.yMin / Screen.height), 0)),
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMax / Screen.width), (t.pixelInset.yMin / Screen.height), 0)));
        Gizmos.DrawLine(
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMin / Screen.width), (t.pixelInset.yMax / Screen.height), 0)),
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMax / Screen.width), (t.pixelInset.yMax / Screen.height), 0)));
        Gizmos.DrawLine(
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMin / Screen.width), (t.pixelInset.yMin / Screen.height), 0)),
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMin / Screen.width), (t.pixelInset.yMax / Screen.height), 0)));
        Gizmos.DrawLine(
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMax / Screen.width), (t.pixelInset.yMin / Screen.height), 0)),
            Camera.main.ViewportToWorldPoint(new Vector3(transform.position.x, transform.position.y, dist) + new Vector3((t.pixelInset.xMax / Screen.width), (t.pixelInset.yMax / Screen.height), 0)));
    }
//# endif

    void OnMouseOver()
    {
        if(!ifcBase.blocked && m_hoverState == false) SetHover(true);
        m_hoverAlive = true;
    }

    void LateUpdate()
    {
        if(m_hoverAlive) m_hoverAlive = false;
        else if(m_hoverState) SetHover(false);
    }

    void OnMouseEnter()
    {
        if(ifcBase.blocked) return;
        SetHover(true);
        /*
        if (m_selected) return;
        if (m_over == null)
        {
            guiTexture.color = m_colorOver;//m_base * 2;
            if (m_icono != null) m_icono.color = m_colorOver;
        }
        else
            guiTexture.texture = m_over;
        if (m_icono != null && m_IconOver!=null) m_icono.texture = m_IconOver;*/
    }

    protected void SetHover(bool mode)
    {
        m_hoverState = mode;
        if (m_selected) return;
        if (m_over == null)
        {
            GetComponent<GUITexture>().color = mode ? m_colorOver : m_base;//m_base * 2;
              if (m_icono != null) m_icono.color = mode ? m_colorOver : m_base;
        }
        else
            GetComponent<GUITexture>().texture = mode ? m_over : m_current;
        if (m_icono != null && m_IconOver!=null) m_icono.texture = mode ? m_IconOver : m_IconCurrent;

        if(m_zoom != 1f)
        {
          if(mode && ! m_zoomed)
          {
            GetComponent<GUITexture>().pixelInset = ifcBase.ScaleRect(GetComponent<GUITexture>().pixelInset, m_zoom);
            if(m_icono != null) m_icono.pixelInset = ifcBase.ScaleRect(m_icono.pixelInset, m_zoom);
            m_zoomed = true;
          }
          else if(m_zoomed)
          {
            GetComponent<GUITexture>().pixelInset = ifcBase.ScaleRect(GetComponent<GUITexture>().pixelInset, 1f/m_zoom);
            if(m_icono != null) m_icono.pixelInset = ifcBase.ScaleRect(m_icono.pixelInset, 1f/m_zoom);
            m_zoomed = false;
          }
        }
    }

    protected virtual void OnMouseExit()
    {
        SetHover(false);
        /*if (m_selected) return;
        if (m_over == null)
        {
            guiTexture.color = m_base;
            if (m_icono != null) m_icono.color = m_base;
        }
        else
            guiTexture.texture = m_current;
        if (m_icono != null ) m_icono.texture = m_IconCurrent;*/
    }

    public virtual void Select()
    {
        GetComponent<GUITexture>().texture = m_select;
        if (m_icono != null && m_IconSelect!=null) m_icono.texture = m_IconSelect;
        GetComponent<GUITexture>().color = new Color( 0.5f, 0.5f, 0.5f, 0.5f);
        m_selected = true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_forceBaseColor">Indica si al deseleccionar este boton hay que devolverlo a su color original o su color resaltado</param>
    public virtual void Deselect()
    {
        m_selected = false;
        GetComponent<GUITexture>().texture = m_current;
        GetComponent<GUITexture>().color = m_base; //  m_colorOver
        if (m_icono != null) m_icono.texture = m_IconCurrent;
        
    }

    void OnMouseUpAsButton()
    {
        if (m_disabled)  return;
        if (m_selected && !Toggle) return;
        if (action != null) action(name);
    }

    public virtual void SetEnabled(bool _show = true)
    {
        float a = _show ? 0.5f : 0.3f;
        float at = _show ? 1.0f : 0.3f;
        Color tmp;
        GameObject obj;
        obj = gameObject; tmp = obj.GetComponent<GUITexture>().color; tmp.a = a; obj.GetComponent<GUITexture>().color = tmp;
        obj.layer = _show?0:1 << 1;
        Transform t = transform.Find("Icono");
        if (t) { obj = t.gameObject; tmp = obj.GetComponent<GUITexture>().color; tmp.a = a; obj.GetComponent<GUITexture>().color = tmp; }
        t = transform.Find("Text");
        if (t) { obj = t.gameObject; tmp = obj.GetComponent<GUIText>().color; tmp.a = at; obj.GetComponent<GUIText>().color = tmp; }
    }

        public virtual void SetDisabled(bool b)
        {
            m_disabled = b;
        }

        
}
