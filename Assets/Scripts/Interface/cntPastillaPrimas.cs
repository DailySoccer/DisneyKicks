using UnityEngine;
using System.Collections;

public class cntPastillaPrimas : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static cntPastillaPrimas instance { get { return m_instance; } }
    private static cntPastillaPrimas m_instance;

    /// <summary>
    /// controles de esta interfaz
    /// </summary>
    private cntPrima[] m_primas;


    // ------------------------------------------------------------------------------
    // ---  METOODOS  ---------------------------------------------------------------
    // ----------------------------------------------------------------------------


    void Awake() {      
        // guardar la instancia a esta clase
        m_instance = this;
    }


	// Use this for initialization
    void Start() {
        if(!MissionManager.instance.HasCurrentMission())
        {
            return;
        }
        if (m_primas == null) {
            // obtener la referencia a los elementos de esta interfaz
            m_primas = new cntPrima[4];
            for (int i = 0; i < m_primas.Length; ++i) {
                m_primas[i] = transform.FindChild("prima" + (i + 1)).GetComponent<cntPrima>();
            }

            // registrar el refresco de los objetivos
            ServiceLocator.Request<IShotResultService>().RegisterListener(RefreshEstadoObjetivos);
        }

        // inicializar los controles para mostrar las primas
        {
            int i = 0;
            for (; (i < MissionManager.instance.GetMission().Achievements.Count) && (i < m_primas.Length); ++i) {
                MissionAchievement objetivo = MissionManager.instance.GetMission().Achievements[i];
                MissionAchievement objetivoCargado = GameplayService.gameLevelMission.GetAchievements()[i];
                m_primas[i] = transform.FindChild("prima" + (i + 1)).GetComponent<cntPrima>();
                m_primas[i].Inicializar(objetivo.DescriptionID, objetivo.IsAchieved() || objetivoCargado.IsAchieved());
                m_primas[i].gameObject.SetActive(true);
            }

            // ocultar los controles restantes
            for (; i < m_primas.Length; ++i) {
                m_primas[i].gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Refrescar el estado de los objetivos de esta mision
    /// </summary>
    /// <param name="_shotResul"></param>
    public void RefreshEstadoObjetivos(ShotResult _shotResul) {
        for (int i = 0; (i < MissionManager.instance.GetMission().Achievements.Count) && (i < m_primas.Length); ++i) {
            MissionAchievement objetivoCargado = GameplayService.gameLevelMission.GetAchievements()[i];
            m_primas[i].RefreshConseguido(MissionManager.instance.GetMission().Achievements[i].IsAchieved() || objetivoCargado.IsAchieved());
        }
    }

	
}
