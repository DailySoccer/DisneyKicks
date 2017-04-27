using UnityEngine;
using System.Collections;

public class AvataresManager : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ---------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static AvataresManager instance { get { return m_instance; } }
    private static AvataresManager m_instance;


    // texturas con las imagenes de los jugadores (de medio cuerpo)
    // NOTA: asignar valor a estas propiedades desde la interfaz de Unity
    public Texture[] m_texturasImgsLanzador;
    public Texture[] m_texturasImgsPortero;

    // texturas con las imagenes de los jugadores (de cuerpo entero)
    // NOTA: asignar valor a estas propiedades desde la interfaz de Unity
    public Texture[] m_texturasImgsLanzadorCuerpoEntero;
    public Texture[] m_texturasImgsPorteroCuerpoEntero;

    // texturas de los escudos
    public Texture[] m_texturasEscudos;

    // texturas de los escudos para su compra en la tienda
    // NOTA: el la posicion 0 no hay textura xq corresponde al escudo por defecto que no se compra
    public Texture[] m_texturasEscudosTienda;
    public Texture[] m_texturasEscudosTiendaEquipados;

    // textura de los escudos cuando se desbloquean
    public Texture[] m_texturasEscudosDesbloqueados;

    // textura para la compra de powerup
    public Texture m_texturaCompraPowerUp;

    // texturas para los powerups
    public Texture[] m_texturasPowerUp;

    // texturas para mostrar las habilidades de los jugadores
    public Texture[] m_texturasHabilidadades;

    // texturas de las equipaciones conseguidas (
    public Texture[] m_texturasAvatarEquipacionesLanzador;
    public Texture[] m_texturasAvatarEquipacionesPortero;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ---------------------------------------------------------------------------

    void Awake() {
        m_instance = this;
    }


    void Start() {
        // comprobar si se han inicializado los arrays de texturas y si no mostrar un warning
        if (m_texturasImgsLanzador == null || m_texturasImgsLanzador.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasImgsLanzador' de 'AvataresManager' no tiene texturas");

        if (m_texturasImgsPortero == null || m_texturasImgsPortero.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasImgsPortero' de 'AvataresManager' no tiene texturas");

        if (m_texturasImgsLanzadorCuerpoEntero == null || m_texturasImgsLanzadorCuerpoEntero.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasImgsLanzadorCuerpoEntero' de 'AvataresManager' no tiene texturas");

        if (m_texturasImgsPorteroCuerpoEntero == null || m_texturasImgsPorteroCuerpoEntero.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasImgsPorteroCuerpoEntero' de 'AvataresManager' no tiene texturas");

        if (m_texturasEscudos == null || m_texturasEscudos.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasEscudos' de 'AvataresManager' no tiene texturas");

        if (m_texturasEscudosTienda == null || m_texturasEscudosTienda.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasCompraEscudos' de 'AvataresManager' no tiene texturas");

        if (m_texturasEscudosTiendaEquipados == null || m_texturasEscudosTiendaEquipados.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasEscudosTiendaEquipados' de 'AvataresManager' no tiene texturas");

        if (m_texturasEscudosDesbloqueados == null || m_texturasEscudosDesbloqueados.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasEscudosDesbloqueados' de 'AvataresManager' no tiene texturas");

        if (m_texturasPowerUp == null || m_texturasPowerUp.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasPowerUp' de 'AvataresManager' no tiene texturas");

        if (m_texturasHabilidadades == null || m_texturasHabilidadades.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasHabilidadades' de 'AvataresManager' no tiene texturas");

        if (m_texturasAvatarEquipacionesLanzador == null || m_texturasAvatarEquipacionesLanzador.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasAvatarEquipaciones' de 'AvataresManager' no tiene texturas");

        if (m_texturasAvatarEquipacionesPortero == null || m_texturasAvatarEquipacionesPortero.Length == 0)
            Debug.LogWarning("Atencion: el array 'm_texturasAvatarEquipacionesPortero' de 'AvataresManager' no tiene texturas");
    }


    /// <summary>
    /// Devuelve la textura asociada al lanzador "_numLanzador"
    /// </summary>
    /// <param name="_numLanzador"></param>
    /// <returns></returns>
    public Texture GetTexturaLanzador(int _numLanzador) {
        if (m_texturasImgsLanzador != null && _numLanzador >= 0 && _numLanzador < m_texturasImgsLanzador.Length) {
            return m_texturasImgsLanzador[_numLanzador];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura asociada al portero "_numLanzador"
    /// </summary>
    /// <param name="_numPortero"></param>
    /// <returns></returns>
    public Texture GetTexturaPortero(int _numPortero) {
        if (m_texturasImgsPortero != null && _numPortero >= 0 && _numPortero < m_texturasImgsPortero.Length) {
            return m_texturasImgsPortero[_numPortero];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de jugador de cuerpo entero asociada al lanzador "_numLanzador"
    /// </summary>
    /// <param name="_numLanzador"></param>
    /// <returns></returns>
    public Texture GetTexturaLanzadorCuerpoEntero(int _numLanzador) {
        if (m_texturasImgsLanzadorCuerpoEntero != null && _numLanzador >= 0 && _numLanzador < m_texturasImgsLanzadorCuerpoEntero.Length) {
            return m_texturasImgsLanzadorCuerpoEntero[_numLanzador];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de jugador de cuerpo entero asociada al portero "_numLanzador"
    /// </summary>
    /// <param name="_numPortero"></param>
    /// <returns></returns>
    public Texture GetTexturaPorteroCuerpoEntero(int _numPortero) {
        if (m_texturasImgsPorteroCuerpoEntero != null && _numPortero >= 0 && _numPortero < m_texturasImgsPorteroCuerpoEntero.Length) {
            return m_texturasImgsPorteroCuerpoEntero[_numPortero];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura asociada al escudo "_numEscudo"
    /// </summary>
    /// <param name="_numEscudo"></param>
    /// <returns></returns>
    public Texture GetTexturaEscudo(int _numEscudo) {
        if (m_texturasEscudos != null && _numEscudo >= 0 && _numEscudo < m_texturasEscudos.Length) {
            return m_texturasEscudos[_numEscudo];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de escudo en la tienda asociada al escudo "_numEscudo"
    /// </summary>
    /// <param name="_numEscudo"></param>
    /// <returns></returns>
    public Texture GetTexturaEscudoTienda(int _numEscudo) {
        if (m_texturasEscudosTienda != null && _numEscudo >= 0 && _numEscudo < m_texturasEscudosTienda.Length) {
            return m_texturasEscudosTienda[_numEscudo];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de escudo en la tienda  de escudo asociada al escudo "_numEscudo"
    /// </summary>
    /// <param name="_numEscudo"></param>
    /// <returns></returns>
    public Texture GetTexturaEscudoTiendaEquipado(int _numEscudo) {
        if (m_texturasEscudosTiendaEquipados != null && _numEscudo >= 0 && _numEscudo < m_texturasEscudosTiendaEquipados.Length) {
            return m_texturasEscudosTiendaEquipados[_numEscudo];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de escudo en la tienda  de escudo asociada al escudo "_numEscudo"
    /// </summary>
    /// <param name="_numEscudo"></param>
    /// <returns></returns>
    public Texture GetTexturasEscudosDesbloqueados(int _numEscudo) {
        if (m_texturasEscudosDesbloqueados != null && _numEscudo >= 0 && _numEscudo < m_texturasEscudosDesbloqueados.Length) {
            return m_texturasEscudosDesbloqueados[_numEscudo];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura del powerup "_numPowerUp"
    /// </summary>
    /// <param name="_numPowerUp"></param>
    /// <returns></returns>
    public Texture GetTexturaPowerUp(int _numPowerUp) {
        if (m_texturasPowerUp != null && _numPowerUp >= 0 && _numPowerUp < m_texturasPowerUp.Length) {
            return m_texturasPowerUp[_numPowerUp];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de la habilidad "_numHabilidad"
    /// </summary>
    /// <param name="_numHabilidad"></param>
    /// <returns></returns>
    public Texture GetTexturaHabilidad(int _numHabilidad) {
        if (m_texturasHabilidadades != null && _numHabilidad >= 0 && _numHabilidad < m_texturasHabilidadades.Length) {
            return m_texturasHabilidadades[_numHabilidad];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de la equipacion de lanzador "_numEquipacion"
    /// </summary>
    /// <param name="_numEquipacion"></param>
    /// <returns></returns>
    public Texture GetTexturaAvatarEquipacionesLanzador(int _numEquipacion) {
        if (m_texturasAvatarEquipacionesLanzador != null && _numEquipacion >= 0 && _numEquipacion < m_texturasAvatarEquipacionesLanzador.Length) {
            return m_texturasAvatarEquipacionesLanzador[_numEquipacion];
        } else
            return null;
    }


    /// <summary>
    /// Devuelve la textura de la equipacion de portero "_numEquipacion"
    /// </summary>
    /// <param name="_numEquipacion"></param>
    /// <returns></returns>
    public Texture GetTexturaAvatarEquipacionesPortero(int _numEquipacion) {
        if (m_texturasAvatarEquipacionesPortero != null && _numEquipacion >= 0 && _numEquipacion < m_texturasAvatarEquipacionesPortero.Length) {
            return m_texturasAvatarEquipacionesPortero[_numEquipacion];
        } else
            return null;
    }

}
