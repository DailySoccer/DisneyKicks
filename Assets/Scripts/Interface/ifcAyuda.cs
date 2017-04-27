using UnityEngine;
using System.Collections;

public class ifcAyuda : ifcBase
{
    public enum mode
    {
        none,
        casillas,
        iniesta,
        logros
    };

    mode currentMode = mode.none;

    public static ifcAyuda instance { get; protected set; }
    int m_page;
    GameObject m_current;
    GameObject m_currentPage;
    GameObject m_casillas;
    GameObject m_iniesta;
    GameObject m_logros;
    GameObject m_btnLeft;
    GameObject m_btnRigth;


    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        m_backMethod = Back;

        m_casillas = transform.Find("Casillas").gameObject;
        m_iniesta = transform.Find("Iniesta").gameObject;
        m_logros = transform.Find("Logros").gameObject;

        m_btnLeft = transform.Find("flecha_izq").gameObject;
        m_btnRigth = transform.Find("flecha_dcha").gameObject;


        getComponentByName("btnAtras").GetComponent<btnButton>().action = Back;

        getComponentByName("btnCasillas").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            ActivateButton(_name);
        };

        getComponentByName("btnIniesta").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            ActivateButton(_name);
        };
        getComponentByName("btnLogros").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            ActivateButton(_name);
        };

        m_btnLeft.GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            setPage(m_page - 1);
        };
        m_btnRigth.GetComponent<btnButton>().action = (_name) => {
            Interfaz.ClickFX();
            setPage(m_page + 1);
        };

        ActivateButton("btnIniesta");
    }

    void Back(string _target = "")
    {
        GeneralSounds_menu.instance.back();
        for (int i = 0; i < (currentMode == mode.logros ? 3 : 4); ++i)
            if (i != m_page)
                m_current.transform.Find("Step" + (i + 1)).position = new Vector3(-1.0f, 100.0f, 0.0f);
        
        /*
        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        new SuperTweener.move(ifcJugar.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        ifcBase.activeIface = ifcJugar.instance;
        */
        new SuperTweener.move( gameObject, 0.25f, new Vector3( 1.0f, 0.0f, 0.0f ), SuperTweener.CubicOut );
        new SuperTweener.move( ifcMainMenu.instance.gameObject, 0.25f, new Vector3( 0.0f, 0.0f, 0.0f ), SuperTweener.CubicOut );
        ifcBase.activeIface = ifcMainMenu.instance;
    }

    void ActivateButton(string _name)
    {
        getComponentByName("btnCasillas").GetComponent<btnButton>().Deselect();
        getComponentByName("btnIniesta").GetComponent<btnButton>().Deselect();
        getComponentByName("btnLogros").GetComponent<btnButton>().Deselect();
        getComponentByName(_name).GetComponent<btnButton>().Select();

        if (_name == "btnCasillas") setAyuda(mode.casillas);
        else if (_name == "btnIniesta") setAyuda(mode.iniesta);
        else setAyuda(mode.logros);
    }

    public void setAyuda(mode _mode) {
        currentMode = _mode;
        GameObject next = null;
        switch (_mode) {
            case mode.iniesta: next = m_iniesta; break;
            case mode.casillas: next = m_casillas; break;
            case mode.logros: next = m_logros; break;
        }
        if (m_current != next)
        {
            m_casillas.SetActive(false);
            m_iniesta.SetActive(false);
            m_logros.SetActive(false);
            m_current = next;
            m_current.SetActive(true);
            setPage(0);
        }
    }

    void setPage(int _page)
    {
        GameObject next = m_current.transform.Find("Step" + (_page + 1)).gameObject;

        if (m_page < _page)
        {
            next.transform.localPosition = new Vector3(1.5f, 0.55f, 0);
            new SuperTweener.moveLocal(next, 0.25f, new Vector3(0.5f, 0.55f, 0.0f), SuperTweener.CubicOut);
            if (m_currentPage != null)
                new SuperTweener.moveLocal(m_currentPage, 0.25f, new Vector3(-0.5f, 0.55f, 0.0f), SuperTweener.CubicOut);
        }
        else
        {
            next.transform.localPosition = new Vector3(-0.5f, 0.55f, 0);
            new SuperTweener.moveLocal(next, 0.25f, new Vector3(0.5f, 0.55f, 0.0f), SuperTweener.CubicOut);
            if (m_currentPage != null)
                new SuperTweener.moveLocal(m_currentPage, 0.25f, new Vector3(1.5f, 0.55f, 0.0f), SuperTweener.CubicOut);
        }
        m_page = _page;
        if (m_page == 0)
            m_btnLeft.SetActive(false);
        else
            m_btnLeft.SetActive(true);
        if (m_page == 3 || (m_page == 2 && currentMode == mode.logros))
            m_btnRigth.SetActive(false);
        else
            m_btnRigth.SetActive(true);
        m_currentPage = next;

    }
}
