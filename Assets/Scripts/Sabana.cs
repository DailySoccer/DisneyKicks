using UnityEngine;
using System.Collections;

public class Sabana : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  ENUMERADO  --------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public enum HoleSize { LARGE = 0, MEDIUM = 1, SHORT = 2 };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // valor del del radio correspondiente a cada tamaño de agujero
    private static float[] m_radioAgujero;

    // puntuacion correspondiente a cada tamaño de agujero
    private static int[] m_PuntuacionAgujero;

    // variables para indicar el tamaño y la posicion de los agujeros de esta sabana
    // Nota: a estos arrays se les asigna valor desde la interfaz de Unity
    public Vector2[] m_holePositions;
    public HoleSize[] m_holeSize;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


	// Use this for initialization
	void Start () {
        // si no se han inicializado los radios de los agujeros => inicializarlos
        if (m_radioAgujero == null) {
            m_radioAgujero = new float[3];
            m_radioAgujero[(int) HoleSize.LARGE] = 1.22f;
            m_radioAgujero[(int) HoleSize.MEDIUM] = 0.91f;
            m_radioAgujero[(int) HoleSize.SHORT] = 0.71f;
        }

        // si no se han inicializado los multiplicadores de puntucion => inicializarlos
        if ( m_PuntuacionAgujero == null ) {
            m_PuntuacionAgujero = new int[ 3 ];
            m_PuntuacionAgujero[ (int)HoleSize.LARGE ] = (int)ScoreManager.SheetScore.L;
            m_PuntuacionAgujero[ (int)HoleSize.MEDIUM ] = (int)ScoreManager.SheetScore.M;
            m_PuntuacionAgujero[ (int)HoleSize.SHORT ] = (int)ScoreManager.SheetScore.S;
        }

        // comprobar si los arrays de agujeros tienen la misma longitud
        if (m_holePositions.Length != m_holeSize.Length) {
            Debug.LogWarning(">>> Atención, en la sabána '" + transform.name + "' los arrays 'm_holePositions' y 'm_holeSize' no tienen la misma longitud");
        }
	}


    /// <summary>
    /// Devuelve los puntos correspondientes para una posicion determinada de la sabana
    /// </summary>
    /// <param name="_posicionImpactoSabana"></param>
    /// <returns></returns>
    public int GetScore (Vector2 _posicion)
        {
        // comprobar si la posicion corresponde a alguno de los agujeros

        for ( int i = 0; i < m_holePositions.Length; ++i ) {
            // calcular la posicion del agujero en el mundo real
            Vector2 posicionAgujeroEnMundo = new Vector2(
                transform.position.x + m_holePositions[i].x,
                transform.position.y + m_holePositions[i].y);

            // comprobar si la posicion queda dentro del agujero
            int iHoleSize = (int)m_holeSize[ i ];
            if ( Vector2.Distance( posicionAgujeroEnMundo, _posicion ) < m_radioAgujero[ iHoleSize ] ) {
                return m_PuntuacionAgujero[ iHoleSize ];
            }
        }

        return 0; // valor por defecto
    }
}
