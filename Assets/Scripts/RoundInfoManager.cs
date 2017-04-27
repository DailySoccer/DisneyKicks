using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Clase para almacenar la informacion asociada a los valores acumulados durantelas rondas de una partida
/// (con la finalidad de que luego se envie a los servicios web)
/// </summary>
public class RoundInfoManager {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static RoundInfoManager instance {
        get {
            if (m_instance == null)
                m_instance = new RoundInfoManager();
            return m_instance;
        }
    }
    private static RoundInfoManager m_instance;

    /// <summary>
    /// "a" => Goles atrapados
    /// </summary>
    public int numAtrapados { get { return m_numAtrapados; } set { m_numAtrapados = value; } }
    private int m_numAtrapados;

    /// <summary>
    /// "d" => Goles despejados
    /// </summary>
    public int numDespejados { get { return m_numDespejados; } set { m_numDespejados = value; } }
    private int m_numDespejados;

    /// <summary>
    /// "e" => Goles encajados o marcados (segun se este jugando como lanzador o portero)
    /// </summary>
    public int numEncajados { get { return m_numEncajados; } set { m_numEncajados = value; } }
    private int m_numEncajados;

    /// <summary>
    /// "f" => Balones tirados fuera
    /// </summary>
    public int numFueras { get { return m_numFueras; } set { m_numFueras = value; } }
    private int m_numFueras;

    /// <summary>
    /// "t" => Dianas acertadas
    /// </summary>
    public int numTargets { get { return m_numTargets; } set { m_numTargets = value; } }
    private int m_numTargets;

    /// <summary>
    /// Puntos acumulados durantes estas rondas
    /// </summary>
    public int puntos { get { return m_puntos; } set { m_puntos = value; } }
    private int m_puntos;

    /// <summary>
    /// Perfectos acumulados durantes estas rondas
    /// </summary>
    public int perfectos { get { return m_perfectos; } set { m_perfectos = value; } }
    private int m_perfectos;

    /// <summary>
    /// Tiempo total (en segundos) que ha durado esta ronda (devuelve la diferencia entre los tiempos final e inicial de la ronda)
    /// NOTA: (usar los metodos "SetInstanteInicio()" y "SetInstanteFin()" para almacenar los instantes de inicio y fin de la ronda)
    /// </summary>
    public int time { get { return (int) (m_instanteFin - m_instanteInicio); } }
    private float m_instanteInicio;
    private float m_instanteFin;

    // lista de objetivos al inicio de la mision
    public List<MissionAchievement> listaObjetivosInicioMision { get { return m_listaObjetivosIncioMision; } }
    private List<MissionAchievement> m_listaObjetivosIncioMision = new List<MissionAchievement>();


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public RoundInfoManager() {
        ResetInfo();
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS PUBLICOS  -------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Establece toda la informacion almacenada en esta clase a sus valores por defecto
    /// </summary>
    public void ResetInfo() {
        m_numAtrapados = 0;
        m_numDespejados = 0;
        m_numEncajados = 0;
        m_numFueras = 0;
        m_numTargets = 0;
        m_puntos = 0;
        m_instanteInicio = 0.0f;
        m_instanteFin = 0.0f;
        m_listaObjetivosIncioMision.Clear();
    }

    /// <summary>
    /// Inicio de la partida
    /// </summary>
    public void Inicializar() {
        ResetInfo();

        // instante de inicio de la mision
        m_instanteInicio = Time.timeSinceLevelLoad;

        // calcular los objetivos que ya hay cumplidos al inicio de la mision
        GameLevelMission mision = GameplayService.gameLevelMission;
        if (mision == null) {
            Debug.LogWarning("No hay mision cargada");
        } else {
            List<MissionAchievement> objetivosMision = mision.GetAchievements();
            if (objetivosMision == null) {
                Debug.LogWarning("La mision " + GameplayService.gameLevelMission.MissionName + " no tiene objetivos definidos.");
            } else {
                for (int i = 0; i < objetivosMision.Count; ++i) {
                    if (objetivosMision[i].IsAchieved()) {
                        Debug.Log("En la mision " + GameplayService.gameLevelMission.MissionName + " el objetivo " + objetivosMision[i].Code + " SI esta conseguido.");
                        m_listaObjetivosIncioMision.Add(objetivosMision[i]);
                    } else {
                        Debug.Log("En la mision " + GameplayService.gameLevelMission.MissionName + " el objetivo " + objetivosMision[i].Code + " aun NO esta conseguido.");
                    }
                }
            }   
        }

        //misionInfo.SetAchivementState(objetivosConseguido.ToString(), true);
    }


    /// <summary>
    /// Acumula los datos asociados a una ronda
    /// </summary>
    /// <param name="_result"></param>
    /// <param name="_points"></param>
    public void AcumularRonda(Result _result, int _points, bool _perfect) {
        switch (_result) {
            case Result.Saved:
                ++m_numAtrapados;
                break;
            case Result.Stopped:
                ++m_numDespejados;
                break;
            case Result.Goal:
                ++m_numEncajados;
                break;
            case Result.OutOfBounds:
                ++m_numFueras;
                break;
            case Result.Target:
                ++m_numTargets;
                break;
        }

        if(_perfect)
        {
            ++m_perfectos;
        }

        m_puntos +=_points;

        m_instanteFin = Time.timeSinceLevelLoad; // <= guardar el instante de tiempo actual

        PersistenciaManager.instance.AcumularRondaMision(_result, _perfect, _points, !GameplayService.networked);
    }


    /// <summary>
    /// Muestra los datos contenidos en esta clase
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        //return base.ToString();
        string texto = "";
        texto += "   m_numAtrapados=" + m_numAtrapados;
        texto += "   m_numDespejados=" + m_numDespejados;
        texto += "   m_numEncajados=" + m_numEncajados;
        texto += "   m_numFueras=" + m_numFueras;
        texto += "   m_numTargets=" + m_numTargets;
        texto += "   m_puntos=" + m_puntos;
        texto += "   time=" + time;
        return texto;
    }
}
