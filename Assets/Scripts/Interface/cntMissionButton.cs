using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cntMissionButton : MonoBehaviour {
    
    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    private const int NUM_OBJETIVOS = 4;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    // texturas para este control
    // NOTA: asignar valor a estas texturas desde la interfaz de Unity
    public Texture m_texturaLanzador;
    public Texture m_texturaPortero;
    public Texture m_texturaLanzadorOff;
    public Texture m_texturaPorteroOff;
    public Texture m_texturaBotonBlock;
    public Texture m_texturaBotonUp;

    // elementos graficos de este componente grafico
    private GUITexture[] m_imgsObjetivos;
    private GUITexture m_imgIcono;
    private GUIText m_txtNumMision;
    private btnButton m_boton;

    /// <summary>
    /// Indica si la fase asociada a este boton ha sido desbloqueada por el usuario
    /// </summary>
    private bool m_misionDesbloqueada = false;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Obtiene las referencias a los elementos de esta interfaz
    /// </summary>
    private void GetReferencias() {
        if (m_imgsObjetivos == null) {
            m_imgsObjetivos = new GUITexture[NUM_OBJETIVOS];
            for (int i = 0; i < m_imgsObjetivos.Length; ++i) {
                m_imgsObjetivos[i] = transform.FindChild("objetivo" + (i + 1)).GetComponent<GUITexture>();
            }
        }

        if (m_imgIcono == null)
            m_imgIcono = transform.FindChild("icono").GetComponent<GUITexture>();
        if (m_txtNumMision == null)
            m_txtNumMision = transform.FindChild("numMision").GetComponent<GUIText>();
        if (m_boton == null)
            m_boton = transform.FindChild("boton").GetComponent<btnButton>();
    }


	// Use this for initialization
	void Start () {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();
	}


    /// <summary>
    /// Inicializa este control
    /// </summary>
    /// <param name="_numMision">Número de misison (de 0 a 9)</param>
    /// <param name="_glm"></param>
    /// <param name="_misionDesbloqueada">Indica si esta mision ha sido desbloqueada o no</param>
    public void Init(int _numMision, GameLevelMission _glm, bool _misionDesbloqueada) {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        m_misionDesbloqueada = _misionDesbloqueada;

        // pintar el numero de mision
        m_txtNumMision.text = (_numMision + 1).ToString();

        // asignarle el icono que corresponda en funcion del estado y tipo de mision
        if (m_misionDesbloqueada)
            m_imgIcono.texture = (_glm.MissionGameMode == GameMode.Shooter) ? m_texturaLanzador : m_texturaPortero;
        else
            m_imgIcono.texture = (_glm.MissionGameMode == GameMode.Shooter) ? m_texturaLanzadorOff : m_texturaPorteroOff;

        // pintar los objetivos conseguidos en esta mision
        List<MissionAchievement> logros = _glm.GetAchievements();
        for (int i = 0; i < logros.Count; ++i) {
            m_imgsObjetivos[i].gameObject.SetActive(logros[i].IsAchieved());
        }

        // boton
        m_boton.action = (_name) => {
            Debug.Log(">>> _glm=" + _glm + "   _numMision=" + _numMision);

            cntMissions.instance.ShowLevelMission(_glm, _numMision);
            cntMissions.instance.MostrarBotonSeleccionado(_numMision % 10);
            GeneralSounds_menu.instance.select();
        };

        // fondo del boton
        m_boton.m_current = (_misionDesbloqueada) ? m_texturaBotonUp : m_texturaBotonBlock;
    }


    /// <summary>
    /// Marca este control como seleccionado
    /// </summary>
    public void Select() {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        //if (m_misionDesbloqueada)
            m_boton.Select();
    }


    /// <summary>
    /// Deselecciona este control
    /// </summary>
    public void Deselect() {
        // obtener las referencias a los elementos de esta interfaz
        GetReferencias();

        //if (m_misionDesbloqueada)
            m_boton.Deselect();
    }

}
