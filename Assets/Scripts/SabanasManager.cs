using UnityEngine;
using System.Collections;

/// <summary>
/// Clase para gestionar las sabanas que se muestran en la porteria
/// </summary>
public class SabanasManager : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Numero maximo de sabanas a mostrar
    /// </summary>
    public const int NUM_MAX_SABANAS = 3;

    /// <summary>
    /// Posicion de la primera sabana (el resto se posicionan respecto a esta con un offset)
    /// </summary>
    private float X_INICAL = 2.38f;


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Tipos posibles de sabanas individuales existentes
    /// </summary>
    public enum TipoSabanaInidividual { NONE, BLANK, L, M, S}


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// instancia de esta clase
    /// </summary>
    public static SabanasManager instance { get { return m_instance; } }
    private static SabanasManager m_instance;

    /// <summary>
    /// True si hay sabanas delante de la porteria, false en caso contrario
    /// </summary>
    public bool haySabanas { get { return m_haySabanas; } }
    private bool m_haySabanas = false;

    // prefabs para generar instancias de las sabanas (asignarles valor desde la interfaz de unity)
    public Sabana m_prefBullSheet_Blank;
    public Sabana m_prefBullSheet_L;
    public Sabana m_prefBullSheet_M;
    public Sabana m_prefBullSheet_MM;
    public Sabana m_prefBullSheet_MS;
    public Sabana m_prefBullSheet_S;
    public Sabana m_prefBullSheet_SS;

    // sabanas a mostrar en la porteria
    private Sabana[] m_sabanas;


    // ------------------------------------------------------------------------------
    // ---  METODOS PUBLICOS  ------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Muestra sobre la porteria la configuracion de sabanas recibidas como parametro
    /// </summary>
    /// <param name="_tiposDeSabana"></param>
    public void ShowSabanas(TipoSabanaInidividual[] _tiposDeSabana) {
        // eliminar las sabanas actuales
        ClearSabanas();

        // generar las nuevas instancias de las sabanas
        if (_tiposDeSabana != null) {
            for (int i = 0; (i < NUM_MAX_SABANAS) && (i < _tiposDeSabana.Length); ++i) {
                m_sabanas[i] = GenerarInstanciaSabana(_tiposDeSabana[i]);
                if (m_sabanas[i] != null) {
                    m_sabanas[i].transform.position = new Vector3(X_INICAL - (2.38f * i), 0.0f, -50.51f);

                    // indicar que si que hay sabanas
                    m_haySabanas = true;
                }
            }
        }
    }


    /// <summary>
    /// Elimina las sabanas actuales
    /// </summary>
    public void ClearSabanas() {
        for (int i = 0; i < NUM_MAX_SABANAS; ++i) {
            if (m_sabanas[i] != null)
                GameObject.Destroy( m_sabanas[i].gameObject );
        }

        // indicar que no hay sabanas
        m_haySabanas = false;
    }


    /// <summary>
    /// Devuelve los puntos correspondientes al punto de impacto con las sabanas de la porteria
    /// </summary>
    /// <param name="_posicionImpactoPorteria">Punto de impacto sobre el plano en el que se ubica la porteria. 
    /// Nota: la coordenada Z se desprecia</param>
    /// <returns></returns>
    public int GetScore (Vector3 _posicionImpactoPorteria) {
        int score = 0;

        // crear un Vector2 a partir del Vactor3 recibido (despreciando la z)
        Vector2 posicionImpactoPorteriaV2 = new Vector2(_posicionImpactoPorteria.x, _posicionImpactoPorteria.y);

        // devolver el maximo multiplicador
        for ( int i = 0; i < m_sabanas.Length; ++i ) {
            if ( m_sabanas[ i ] != null ) {
                score = Mathf.Max( score, m_sabanas[ i ].GetScore( posicionImpactoPorteriaV2 ) );
            }
        }

        return score;
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS PRIVADOS  ------------------------------------------------------
    // ----------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    void Start() {
        // inicializar el array de sabanas
        m_sabanas = new Sabana[NUM_MAX_SABANAS];
    }

    
    /// <summary>
    /// Devuelve una instancia de la sabana pedida por parametro
    /// </summary>
    /// <param name="_tipoSabana"></param>
    /// <param name="_poscion"></param>
    /// <returns></returns>
    private Sabana GenerarInstanciaSabana(TipoSabanaInidividual _tipoSabana) {
       
        switch (_tipoSabana) {
            case TipoSabanaInidividual.BLANK:
                return GameObject.Instantiate(m_prefBullSheet_Blank) as Sabana;

            case TipoSabanaInidividual.L:
                return GameObject.Instantiate(m_prefBullSheet_L) as Sabana;

            case TipoSabanaInidividual.M:
                return GameObject.Instantiate(m_prefBullSheet_M) as Sabana;

//            case TipoSabanaInidividual.MM:
//                return GameObject.Instantiate(m_prefBullSheet_MM) as Sabana;

//            case TipoSabanaInidividual.MS:
//                return GameObject.Instantiate(m_prefBullSheet_MS) as Sabana;

            case TipoSabanaInidividual.S:
                return GameObject.Instantiate(m_prefBullSheet_S) as Sabana;

//            case TipoSabanaInidividual.SS:
//                return GameObject.Instantiate(m_prefBullSheet_SS) as Sabana;

            default:
                return null;
        }
    }


}
