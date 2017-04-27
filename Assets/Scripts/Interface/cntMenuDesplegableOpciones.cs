using UnityEngine;
using System.Collections;

public class cntMenuDesplegableOpciones : MonoBehaviour
{


    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // coordenadas en el eje x de este control cuando esta plegado / desplegado
    private float POS_X_PLEGADO = -0.5f;
    private float POS_X_DESPLEGADO = 0.0f;

    public static bool firstTime = true;
    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static cntMenuDesplegableOpciones instance { get { return m_instance; } }
    private static cntMenuDesplegableOpciones m_instance;

    // elementos de esta interfaz
    private btnButton m_btnDesplegar;
    private btnButton m_btnFx;
    private btnButton m_btnMusica;
    private btnButton m_btnCalidadAlta;
    private btnButton m_btnCalidadMedia;
    private btnButton m_btnCalidadBaja;
    private bool m_plegado = true;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake()
    {
        m_instance = this;
    }


    /// <summary>
    /// obtener las referencias a los elementos de esta interfaz
    /// </summary>
    private void GetReferencias()
    {
        // boton plegar
        if (m_btnDesplegar == null)
        {
            m_btnDesplegar = transform.FindChild("btnDesplegar").GetComponent<btnButton>();
            m_btnDesplegar.action = (_name) =>
            {
                Interfaz.ClickFX();
                Plegar();
            };
        }

        // boton fx
        if (m_btnFx == null)
        {
            m_btnFx = transform.FindChild("btnSonidoFx").GetComponent<btnButton>();
            m_btnFx.Toggle = true;
            m_btnFx.action = (_name) =>
            {
                ifcOpciones.fx = !ifcOpciones.fx;
                PlayerPrefs.SetInt("fx", ifcOpciones.fx ? 1 : 0);
                Interfaz.ClickFX();
                RefrescarBotones();
            };
        }

        // boton musica
        if (m_btnMusica == null)
        {
            m_btnMusica = transform.FindChild("btnSonidoMusica").GetComponent<btnButton>();
            m_btnMusica.Toggle = true;
            m_btnMusica.action = (_name) =>
            {
                Interfaz.ClickFX();
                ifcOpciones.music = !ifcOpciones.music;
                PlayerPrefs.SetInt("music", ifcOpciones.music ? 1 : 0);
                if (ifcOpciones.music)
                    ifcMusica.instance.musicOn();
                else
                    ifcMusica.instance.musicOff();
                RefrescarBotones();
            };
        }


        // boton calidad alta
        if (m_btnCalidadAlta == null)
        {
            m_btnCalidadAlta = transform.FindChild("btnCalidadAlta").GetComponent<btnButton>();
            m_btnCalidadAlta.Toggle = true;
            m_btnCalidadAlta.action = (_name) =>
            {
                Interfaz.ClickFX();
                SetCalidad(2, "cntMenuDesplegableOpciones => btnCalidadAlta");
                RefrescarBotones();
            };
        }

        // boton calidad media
        if (m_btnCalidadMedia == null)
        {
            m_btnCalidadMedia = transform.FindChild("btnCalidadMedia").GetComponent<btnButton>();
            m_btnCalidadMedia.Toggle = true;
            m_btnCalidadMedia.action = (_name) =>
            {
                Interfaz.ClickFX();
                SetCalidad(1, "cntMenuDesplegableOpciones => btnCalidadMedia");
                RefrescarBotones();
            };
        }

        // boton calidad baja
        if (m_btnCalidadBaja == null)
        {
            m_btnCalidadBaja = transform.FindChild("btnCalidadBaja").GetComponent<btnButton>();
            m_btnCalidadBaja.Toggle = true;
            m_btnCalidadBaja.action = (_name) =>
            {
                Interfaz.ClickFX();
                SetCalidad(0, "cntMenuDesplegableOpciones => btnCalidadBaja");
                RefrescarBotones();
            };
        }
    }


    public void onStart()
    {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        ifcOpciones.fx = PlayerPrefs.GetInt("fx", 1) != 0;
        ifcOpciones.music = PlayerPrefs.GetInt("music", 1) != 0;
        if (ifcOpciones.music)
        {
            ifcMusica.instance.musicOn();
        }

        if (firstTime)
        {
            SetCalidad(PlayerPrefs.GetInt("calidad", 1), "cntMenuDesplegableOpciones => Start()");
            firstTime = false;
        }
        //QualitySettings.SetQualityLevel(ifcOpciones.calidad);

        // por defecto dejar plegado este control
        transform.localPosition = new Vector3(POS_X_PLEGADO, transform.position.y, transform.position.z);
        RefrescarBotones();
    }


    /// <summary>
    /// Despliega este menu
    /// </summary>
    public void Desplegar()
    {
        m_plegado = false;
        // obtener las referencias a los elementos de esta interfaz
        //GetReferencias();

        // mostrar este control
        SetVisible(true);

        // actualizar el estado de los botones
        RefrescarBotones();
        new SuperTweener.move(gameObject, 0.25f, new Vector3(POS_X_DESPLEGADO * ifcBase.scaleFactor, transform.position.y, transform.position.z), SuperTweener.CubicOut);
    }


    /// <summary>
    /// Oculta este menu
    /// </summary>
    public void Plegar()
    {
        if(bloquearCambiosCalidad || m_plegado) return;
        m_plegado = true;
        // obtener las referencias a los elementos de esta interfaz
        //GetReferencias();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(POS_X_PLEGADO , transform.position.y, transform.position.z), SuperTweener.CubicOut,
            // onEndCallback
            (_name) =>
            {
                // ocultar este control
                SetVisible(false);

                // mostrar el boton de opciones en el main menu
                ifcMainMenu.instance.SetVisibleBotonOpciones(true);
            });
    }


    /// <summary>
    /// Refrescar el estado de los botones
    /// </summary>
    public void RefrescarBotones()
    {
        // obtener las referencias a los elementos de esta interfaz
        //GetReferencias();

        // boton FX
        if (ifcOpciones.fx)
            m_btnFx.Select();
        else
            m_btnFx.Deselect();

        // boton musica
        if (ifcOpciones.music)
            m_btnMusica.Select();
        else
            m_btnMusica.Deselect();

        // botones de calidad
        m_btnCalidadAlta.Deselect();
        m_btnCalidadMedia.Deselect();
        m_btnCalidadBaja.Deselect();

        m_btnCalidadAlta.SetDisabled(false);
        m_btnCalidadMedia.SetDisabled(false);
        m_btnCalidadBaja.SetDisabled(false);

        switch (QualitySettings.GetQualityLevel())
        {
            case 0:
                m_btnCalidadBaja.Select();
                m_btnCalidadBaja.SetDisabled(true);
                break;
            case 1:
                m_btnCalidadMedia.Select();
                m_btnCalidadMedia.SetDisabled(true);
                break;
            case 2:
                m_btnCalidadAlta.Select();
                m_btnCalidadAlta.SetDisabled(true);
                break;
        }
    }

    public void SetCalidad(int _level, string _str = "")
    {
        if(bloquearCambiosCalidad) return;
        bloquearCambiosCalidad = true;
        ifcOpciones.calidad = _level;
        PlayerPrefs.SetInt("calidad", _level);


#if UNITY_ANDROID
            float prevScaleFactor = ifcBase.scaleFactor;
            float factor = 1f;

            switch (_level)
            {
                case 0: factor = 0.5f; break;
                case 1: factor = 0.75f; break;
                case 2: factor = 1f; break;
            }

            //Screen.SetResolution(Mathf.RoundToInt(ifcBase.origWResolution * factor), Mathf.RoundToInt(ifcBase.origResolution * factor), true);
            StartCoroutine(changeResolution(Mathf.RoundToInt(ifcBase.origWResolution * factor), Mathf.RoundToInt(ifcBase.origResolution * factor), true));

            ifcBase.scaleFactor = ifcBase.origFactor * factor;
            ifcBase.scaleWFactor = ifcBase.origWFactor * factor;

            ifcBase.Scale(Interfaz.instance.gameObject, prevScaleFactor);
            ifcBase.Scale(Cortinilla.instance.gameObject, prevScaleFactor);
            //ifcBase.Scale(ifcDialogBox.instance.gameObject, prevScaleFactor);
#endif
        

#if UNITY_IPHONE
		StartCoroutine(changeResolutionIOS());
#endif
    }

    bool bloquearCambiosCalidad = false;
    IEnumerator changeResolution(int _a, int _b, bool _c)
    {

        Camera cam = Camera.main;
        Vector3 cam_position = cam.transform.position;
        GUILayer guiLayer;
        guiLayer = Camera.main.GetComponent<GUILayer>();
        guiLayer.enabled = false;

        Vector3 temp_cam_position = new Vector3(-1000, -1000, -1000);
        cam.transform.position = temp_cam_position;
        //cam.enabled = false;
        QualitySettings.SetQualityLevel(ifcOpciones.calidad);
        Screen.SetResolution(_a, _b, _c);
        yield return new WaitForSeconds(0.5f);
        //cam.enabled = true;
        cam.transform.position = cam_position;
        guiLayer.enabled = true;
        bloquearCambiosCalidad = false;
        yield return null;

    }

	IEnumerator changeResolutionIOS()
	{
		
		Camera cam = Camera.main;
		Vector3 cam_position = cam.transform.position;
		GUILayer guiLayer;
		guiLayer = Camera.main.GetComponent<GUILayer>();
		guiLayer.enabled = false;
		
		Vector3 temp_cam_position = new Vector3(-1000, -1000, -1000);
		cam.transform.position = temp_cam_position;
		//cam.enabled = false;
		QualitySettings.SetQualityLevel(ifcOpciones.calidad);
		yield return new WaitForSeconds(0.5f);
		//cam.enabled = true;
		cam.transform.position = cam_position;
		guiLayer.enabled = true;
		bloquearCambiosCalidad = false;
		yield return null;
		
	}

    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible)
    {
        gameObject.SetActive(_visible);
    }



}
