using UnityEngine;
using System.Collections;

public class ifcOpciones : ifcBase
{    
    public static bool fx = true;
    public static bool music = true;
    public static int calidad = 1;
    public static ifcOpciones instance { get; protected set; }
    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        m_backMethod = Back;

#if UNITY_IPHONE || UNITY_ANDROID

        transform.Find("btnAlta").gameObject.SetActive(false);
        transform.Find("btnMedia").gameObject.SetActive(false);
        transform.Find("btnBaja").gameObject.SetActive(false);
        transform.Find ("SubtituloCalidad").gameObject.SetActive(false);
        Vector3 tempPos;
        tempPos = transform.Find("SubtituloAudio").position;
        tempPos.y -= 0.2f;
        transform.Find("SubtituloAudio").position = tempPos;

        tempPos = transform.Find("btnEfectos").position;
        tempPos.y -= 0.2f;
        transform.Find("btnEfectos").position = tempPos;

        tempPos = transform.Find("btnMusica").position;
        tempPos.y -= 0.2f;
        transform.Find("btnMusica").position = tempPos;

        calidad = 2;

#else

        if (!PlayerPrefs.HasKey("calidad"))
        {
            calidad = 1;
            PlayerPrefs.SetInt("calidad", calidad);
        }
        else
        {
            calidad = PlayerPrefs.GetInt("calidad");
        }

#endif

        if (!PlayerPrefs.HasKey("fx"))
        {
            fx = true;
            PlayerPrefs.SetInt("fx", fx ? 1 : 0);
        }
        else
        {
            fx = PlayerPrefs.GetInt("fx") == 1;
        }
        if (!PlayerPrefs.HasKey("music"))
        {
            music = true;
            PlayerPrefs.SetInt("music", music ? 1 : 0);            
        }
        else
        {
            music = PlayerPrefs.GetInt("music") == 1;
        }

        getComponentByName("btnEfectos").GetComponent<btnButton>().Toggle = true;
        getComponentByName("btnMusica").GetComponent<btnButton>().Toggle = true;

        getComponentByName("btnEfectos").GetComponent<btnButton>().action = (_name) =>
        {
            fx = !fx;
            PlayerPrefs.SetInt("fx", fx ? 1 : 0);
            setFX();
            Interfaz.ClickFX();
        };
        getComponentByName("btnMusica").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            music = !music;
            PlayerPrefs.SetInt("music", music ? 1 : 0);

            setMusic();
        };

        getComponentByName("btnAlta").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            if (calidad != 2)
            {
                calidad = 2;
                PlayerPrefs.SetInt("calidad", calidad);
                setCalidad();
            }
        };
        getComponentByName("btnMedia").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            if (calidad != 1)
            {
                calidad = 1;
                PlayerPrefs.SetInt("calidad", calidad);
                setCalidad();
            }
        };
        getComponentByName("btnBaja").GetComponent<btnButton>().action = (_name) =>
        {
            Interfaz.ClickFX();
            if (calidad != 0)
            {
                calidad = 0;
                PlayerPrefs.SetInt("calidad", calidad);
                setCalidad();
            }
        };

        getComponentByName("btnAtras").GetComponent<btnButton>().action = Back;

        setFX();
        setMusic();
        setCalidad();
    }

    void Back(string _target = "")
    {
        GeneralSounds_menu.instance.back();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        new SuperTweener.move(ifcMainMenu.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        ifcBase.activeIface = ifcMainMenu.instance;
    }

    void Update()
    {
        if(Input.GetKeyUp("escape") && ifcBase.activeIface == this)
        {
            Back();
        }
    }

    void setFX()
    {
        if (!fx)
        {
            transform.Find("btnEfectos/Text").GetComponent<GUIText>().text = "EFECTOS <color=#575757>NO</color>";
            transform.Find("btnEfectos/Text/Shadow").GetComponent<GUIText>().text = "EFECTOS NO";
            getComponentByName("btnEfectos").GetComponent<btnButton>().Deselect();
        }
        else
        {
            transform.Find("btnEfectos/Text").GetComponent<GUIText>().text = "EFECTOS <color=#d2ff5a>SI</color>";
            transform.Find("btnEfectos/Text/Shadow").GetComponent<GUIText>().text = "EFECTOS SI";
            getComponentByName("btnEfectos").GetComponent<btnButton>().Select();
        }
    }

    void setMusic()
    {
        if (!music)
        {
            ifcMusica.instance.musicOff();
            transform.Find("btnMusica/Text").GetComponent<GUIText>().text = "MÚSICA <color=#575757>NO</color>";
            transform.Find("btnMusica/Text/Shadow").GetComponent<GUIText>().text = "MÚSICA NO";
            getComponentByName("btnMusica").GetComponent<btnButton>().Deselect();
        }
        else
        {
            ifcMusica.instance.musicOn();
            transform.Find("btnMusica/Text").GetComponent<GUIText>().text = "MÚSICA <color=#d2ff5a>SI</color>";
            transform.Find("btnMusica/Text/Shadow").GetComponent<GUIText>().text = "MÚSICA SI";
            getComponentByName("btnMusica").GetComponent<btnButton>().Select();
        }
    }

    void setCalidad()
    {
        QualitySettings.SetQualityLevel(calidad);

        getComponentByName("btnAlta").GetComponent<btnButton>().Deselect();
        getComponentByName("btnMedia").GetComponent<btnButton>().Deselect();
        getComponentByName("btnBaja").GetComponent<btnButton>().Deselect();

        switch (calidad)
        {
            case 0:
                getComponentByName("btnBaja").GetComponent<btnButton>().Select();
                break;
            case 1:
                getComponentByName("btnMedia").GetComponent<btnButton>().Select();
                break;
            case 2:
                getComponentByName("btnAlta").GetComponent<btnButton>().Select();
                break;
        }

    }
}
