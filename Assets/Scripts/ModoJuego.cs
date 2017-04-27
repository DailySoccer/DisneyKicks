using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Clase para representar un modo de juego
/// </summary>
public class ModoJuego {

    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // estados en los que puede estar el modo de juego
    public enum Estado { ADQUIRIDO, DISPONIBLE, BLOQUEADO };

    // posibles modos de juego
    public enum TipoModo { NORMAL, EXPERTO, LEYENDA, TIME_ATTACK };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// codigo del modo de juego (para su identificacion contra los servicios web)
    /// </summary>
    public string codigo { get { return m_codigo; } }
    private string m_codigo;

    /// <summary>
    /// tipo del modo seleccionado
    /// </summary>
    public TipoModo tipoModo { get { return m_tipoModo; } }
    private TipoModo m_tipoModo;

    /// <summary>
    /// nombre del modo de juego
    /// </summary>
    public string nombre { get { return m_nombre; } }
    private string m_nombre;

    /// <summary>
    /// estado en el que se encuentra el modo de juego
    /// </summary>
    public Estado estado { get { return m_estado; } set { m_estado = value; } }
    private Estado m_estado;

    /// <summary>
    /// Dinero por el que se desbloquea este jugador
    /// </summary>
    public int precioDesbloqueo { get { return m_precioDesbloqueo; } }
    private int m_precioDesbloqueo;

    /// <summary>
    /// Nombre del logro que desbloquea a este jugador
    /// </summary>
    public string nombreLogroDesbloqueo { get { return m_nombreLogroDesbloqueo; } }
    private string m_nombreLogroDesbloqueo;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public ModoJuego(string _codigo, TipoModo _tipoModo, Estado _estado = Estado.BLOQUEADO, int _precioDesbloqueo = 1000, string _nombreLogroDesbloqueo = "Logro") {
        m_codigo = _codigo;
        m_tipoModo = _tipoModo;
        m_estado = _estado;
        m_precioDesbloqueo = _precioDesbloqueo;
        m_nombreLogroDesbloqueo = _nombreLogroDesbloqueo;

        // calcular el nombre del modo en funcion de su tipo
        switch (_tipoModo) {
            case TipoModo.NORMAL:
                m_nombre = "modo normal";
                break;
            case TipoModo.EXPERTO:
                m_nombre = "modo experto";
                break;
            case TipoModo.LEYENDA:
                m_nombre = "modo leyenda";
                break;
            case TipoModo.TIME_ATTACK:
                m_nombre = "modo time attack";
                break;
        }
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


}



/// <summary>
/// Clase con la informacion de los jugadores
/// </summary>
public class InfoModosJuego {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static InfoModosJuego instance {
        get {
            if (m_instance == null)
                m_instance = new InfoModosJuego();
            return m_instance;
        }
    }
    private static InfoModosJuego m_instance;

    // listas con los modos de juego posibles del juego
    private List<ModoJuego> m_listaModosJuegoTirador;
    private List<ModoJuego> m_listaModosJuegoPortero;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public InfoModosJuego() {
        m_listaModosJuegoTirador = new List<ModoJuego>();
        m_listaModosJuegoPortero = new List<ModoJuego>();

        // F4KE: añadir los modos de juego a cholon
        m_listaModosJuegoTirador.Add(new ModoJuego("SHOOTER_NORMAL_MODE", ModoJuego.TipoModo.NORMAL, ModoJuego.Estado.ADQUIRIDO));
        m_listaModosJuegoTirador.Add(new ModoJuego("SHOOTER_EXPERT_MODE", ModoJuego.TipoModo.EXPERTO, ModoJuego.Estado.ADQUIRIDO));
        m_listaModosJuegoTirador.Add(new ModoJuego("SHOOTER_LEGEND_MODE", ModoJuego.TipoModo.LEYENDA, ModoJuego.Estado.ADQUIRIDO));
        m_listaModosJuegoTirador.Add(new ModoJuego("SHOOTER_TIME_ATTACK_MODE", ModoJuego.TipoModo.TIME_ATTACK, ModoJuego.Estado.ADQUIRIDO));

        m_listaModosJuegoPortero.Add(new ModoJuego("GOALKEEPER_NORMAL_MODE", ModoJuego.TipoModo.NORMAL, ModoJuego.Estado.ADQUIRIDO));
        m_listaModosJuegoPortero.Add(new ModoJuego("GOALKEEPER_TIME_ATTACK_MODE", ModoJuego.TipoModo.TIME_ATTACK, ModoJuego.Estado.ADQUIRIDO));
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Busca un modo de juego (independientemente de que sea de tirador o portero) y devuelve su informacion.
    /// NOTA: si no se encuentra el modo de juego se devuelve "null"
    /// </summary>
    /// <param name="_codigoModoJuego"></param>
    /// <returns></returns>
    public ModoJuego GetModoJuego(string _codigoModoJuego) {
        // buscar en la lista de modos de juego de tirador
        for (int i = 0; i < m_listaModosJuegoTirador.Count; ++i) {
            if (m_listaModosJuegoTirador[i].codigo == _codigoModoJuego)
                return m_listaModosJuegoTirador[i];
        }

        // buscar en la lista de modos de juego de portero
        for (int i = 0; i < m_listaModosJuegoPortero.Count; ++i) {
            if (m_listaModosJuegoPortero[i].codigo == _codigoModoJuego)
                return m_listaModosJuegoPortero[i];
        }

        return null;
    }

}
