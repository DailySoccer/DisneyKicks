using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// Clase para gestionar los logros del juego
/// </summary>
public class LogrosManager : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // ----------------------------------------------------------------------------

    /// <summary>
    /// Tipos de listas de logros
    /// </summary>
    public enum TipoLogros { LANZADOR, PORTERO, DUELO };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static LogrosManager instance {
        get {
            return m_instance;
        }
    }
    private static LogrosManager m_instance;

    /// <summary>
    /// Lista de logros de lanzador
    /// </summary>
    public static List<GrupoLogros> logrosLanzador { get { return m_logrosLanzador; } }
    private static List<GrupoLogros> m_logrosLanzador;

    /// <summary>
    /// Lista de logros de portero
    /// </summary>
    public static List<GrupoLogros> logrosPortero { get { return m_logrosPortero; } }
    private static List<GrupoLogros> m_logrosPortero;

    /// <summary>
    /// Lista de logros de multijugador
    /// </summary>
    public static List<GrupoLogros> logrosDuelo { get { return m_logrosDuelo; } }
    private static List<GrupoLogros> m_logrosDuelo;

    /// <summary>
    /// Devuelve true si se ha conseguido nuevos logros
    /// </summary>
    public bool hayNuevosLogrosConseguidos { get { return m_hayNuevosLogrosConseguidos; } }
    private bool m_hayNuevosLogrosConseguidos = false;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Genera toda la informacion asociada a los logros del juego
    /// </summary>
    private void InicializarListasLogros() {
        // F4KE: crear los grupos de logros de LANZADOR
        if (m_logrosLanzador == null) {
            List<Logro> logrosDianas = new List<Logro>();
            logrosDianas.Add(new Logro("ACH_KICK_DIA_01", 65, 5));
            logrosDianas.Add(new Logro("ACH_KICK_DIA_02", 225, 25));
            logrosDianas.Add(new Logro("ACH_KICK_DIA_03", 1250, 60));
            logrosDianas.Add(new Logro("ACH_KICK_DIA_04", 2750, 125));
            GrupoLogros grupoDianas = new GrupoLogros("ACH_KICK_DIA", LocalizacionManager.instance.GetTexto(166), LocalizacionManager.instance.GetTexto(167), logrosDianas, GrupoLogros.Magnitud.DIANAS_ACERTADAS);

            List<Logro> logrosPuntuacionLanzamientos = new List<Logro>();
            logrosPuntuacionLanzamientos.Add(new Logro("ACH_KICK_TPU_01", 6500, 5));
            logrosPuntuacionLanzamientos.Add(new Logro("ACH_KICK_TPU_02", 27500, 30));
            logrosPuntuacionLanzamientos.Add(new Logro("ACH_KICK_TPU_03", 95000, 75));
            logrosPuntuacionLanzamientos.Add(new Logro("ACH_KICK_TPU_04", 235000, 120));
            logrosPuntuacionLanzamientos.Add(new Logro("ACH_KICK_TPU_05", 750000, 250));
            GrupoLogros grupoPuntuacionLanzamientos = new GrupoLogros("ACH_KICK_TPU", LocalizacionManager.instance.GetTexto(168), LocalizacionManager.instance.GetTexto(169), logrosPuntuacionLanzamientos, GrupoLogros.Magnitud.PUNTUACION_TOTAL_LANZADOR);

            List<Logro> logrosDianasPerfectas = new List<Logro>();
            logrosDianasPerfectas.Add(new Logro("ACH_KICK_PER_01", 80, 10));
            logrosDianasPerfectas.Add(new Logro("ACH_KICK_PER_02", 250, 35));
            logrosDianasPerfectas.Add(new Logro("ACH_KICK_PER_03", 750, 60));
            logrosDianasPerfectas.Add(new Logro("ACH_KICK_PER_04", 1800, 125));
            logrosDianasPerfectas.Add(new Logro("ACH_KICK_PER_05", 6000, 240));
            GrupoLogros grupoDianasPerfectas = new GrupoLogros("ACH_KICK_PER", LocalizacionManager.instance.GetTexto(170), LocalizacionManager.instance.GetTexto(171), logrosDianasPerfectas, GrupoLogros.Magnitud.DIANAS_PERFECTAS);

            // crear la lista de logros de lanzador
            m_logrosLanzador = new List<GrupoLogros>();
            m_logrosLanzador.Add(grupoDianas);
            m_logrosLanzador.Add(grupoPuntuacionLanzamientos);
            m_logrosLanzador.Add(grupoDianasPerfectas);
        }

        // F4KE: crear los grupos de logros de PORTERO
        if (m_logrosPortero == null) {
            List<Logro> logrosPuntuacionParadas = new List<Logro>();
            logrosPuntuacionParadas.Add(new Logro("ACH_KICK_PPU_01", 5500, 5));
            logrosPuntuacionParadas.Add(new Logro("ACH_KICK_PPU_02", 28000, 30));
            logrosPuntuacionParadas.Add(new Logro("ACH_KICK_PPU_03", 85000, 80));
            logrosPuntuacionParadas.Add(new Logro("ACH_KICK_PPU_04", 245000, 150));
            logrosPuntuacionParadas.Add(new Logro("ACH_KICK_PPU_05", 650000, 250));
            GrupoLogros grupoPuntuacionParadas = new GrupoLogros("ACH_KICK_PPU", LocalizacionManager.instance.GetTexto(172), LocalizacionManager.instance.GetTexto(173), logrosPuntuacionParadas, GrupoLogros.Magnitud.PUNTUACION_TOTAL_PORTERO);

            List<Logro> logrosDespejes = new List<Logro>();
            logrosDespejes.Add(new Logro("ACH_KICK_DES_01", 70, 5));
            logrosDespejes.Add(new Logro("ACH_KICK_DES_02", 280, 25));
            logrosDespejes.Add(new Logro("ACH_KICK_DES_03", 825, 65));
            logrosDespejes.Add(new Logro("ACH_KICK_DES_04", 2250, 120));
            logrosDespejes.Add(new Logro("ACH_KICK_DES_05", 6500, 215));
            GrupoLogros grupoDespejes = new GrupoLogros("ACH_KICK_DES", LocalizacionManager.instance.GetTexto(174), LocalizacionManager.instance.GetTexto(175), logrosDespejes, GrupoLogros.Magnitud.DESPEJES);

            List<Logro> logrosParadas = new List<Logro>();
            logrosParadas.Add(new Logro("ACH_KICK_ATR_01", 20, 15));
            logrosParadas.Add(new Logro("ACH_KICK_ATR_02", 100, 50));
            logrosParadas.Add(new Logro("ACH_KICK_ATR_03", 500, 175));
            logrosParadas.Add(new Logro("ACH_KICK_ATR_04", 1500, 300));
            logrosParadas.Add(new Logro("ACH_KICK_ATR_05", 3000, 500));
            GrupoLogros grupoParadas = new GrupoLogros("ACH_KICK_ATR", LocalizacionManager.instance.GetTexto(176), LocalizacionManager.instance.GetTexto(177), logrosParadas, GrupoLogros.Magnitud.PARADAS);

            // crear la lista de logros de portero
            m_logrosPortero = new List<GrupoLogros>();
            m_logrosPortero.Add(grupoPuntuacionParadas);
            m_logrosPortero.Add(grupoDespejes);
            m_logrosPortero.Add(grupoParadas);
        }

        // F4KE: crear los grupos de logro de DUELO
        if (m_logrosDuelo == null) {
            List<Logro> logrosJugarDuelos = new List<Logro>();
            logrosJugarDuelos.Add(new Logro("ACH_KICK_MUL_DUEL_01", 20, 5));
            logrosJugarDuelos.Add(new Logro("ACH_KICK_MUL_DUEL_02", 125, 35));
            logrosJugarDuelos.Add(new Logro("ACH_KICK_MUL_DUEL_03", 400, 80));
            logrosJugarDuelos.Add(new Logro("ACH_KICK_MUL_DUEL_04", 700, 150));
            GrupoLogros grupoJugarDuelos = new GrupoLogros("ACH_KICK_MUL_DUEL", LocalizacionManager.instance.GetTexto(178), LocalizacionManager.instance.GetTexto(179), logrosJugarDuelos, GrupoLogros.Magnitud.DUELOS_JUGADOS);

            List<Logro> logrosVencerDuelos = new List<Logro>();
            logrosVencerDuelos.Add(new Logro("ACH_KICK_MUL_WON_01", 10, 10));
            logrosVencerDuelos.Add(new Logro("ACH_KICK_MUL_WON_02", 50, 45));
            logrosVencerDuelos.Add(new Logro("ACH_KICK_MUL_WON_03", 125, 110));
            logrosVencerDuelos.Add(new Logro("ACH_KICK_MUL_WON_04", 600, 225));
            logrosVencerDuelos.Add(new Logro("ACH_KICK_MUL_WON_05", 2000, 450));
            GrupoLogros grupoVencerDuelos = new GrupoLogros("ACH_KICK_MUL_WON", LocalizacionManager.instance.GetTexto(180), LocalizacionManager.instance.GetTexto(181), logrosVencerDuelos, GrupoLogros.Magnitud.DUELOS_GANADOS);

            List<Logro> logrosDueloPerfecto = new List<Logro>();
            logrosDueloPerfecto.Add(new Logro("ACH_KICK_MUL_PFCT_01", 1, 20));
            GrupoLogros grupoDueloPerfecto = new GrupoLogros("ACH_KICK_MUL_PFCT", LocalizacionManager.instance.GetTexto(182), LocalizacionManager.instance.GetTexto(183), logrosDueloPerfecto, GrupoLogros.Magnitud.DUELOS_PERFECTOS);

            // crear la lista de logros de portero
            m_logrosDuelo = new List<GrupoLogros>();
            m_logrosDuelo.Add(grupoJugarDuelos);
            m_logrosDuelo.Add(grupoVencerDuelos);
            m_logrosDuelo.Add(grupoDueloPerfecto);
        }
    }


    void Awake() {
        if (m_instance == null) {
            m_instance = this;
            m_instance.InicializarListasLogros();
        }
    }

    void Start() {
        // crear las listas de logros
        //m_instance.InicializarListasLogros();
    }


    /// <summary>
    /// Ordena las listas de logros en funcion de su progreso
    /// </summary>
    public void OrdenarListasLogrosPorProgreso() {
        m_logrosLanzador.Sort(GrupoLogros.CompararPorPorcentajeSuperadoLogro);
        m_logrosPortero.Sort(GrupoLogros.CompararPorPorcentajeSuperadoLogro);
        m_logrosDuelo.Sort(GrupoLogros.CompararPorPorcentajeSuperadoLogro);
    }


    /// <summary>
    /// Devuelve la informacion de un determinado logro (null si el logro solicitado no existe)
    /// </summary>
    /// <param name="_idLogro"></param>
    /// <param name="_grupoContenedor">Grupo contenedor de este logro</param>
    /// <returns></returns>
    public Logro GetLogro(string _idLogro, ref GrupoLogros _grupoContenedor) {
        Logro logro = null;

        // comprobar si el logro es un logro de lanzador
        if (m_logrosLanzador != null) {
            for (int i = 0; (i < m_logrosLanzador.Count) && (logro == null); ++i) {
                _grupoContenedor = m_logrosLanzador[i];
                logro = m_logrosLanzador[i].GetLogro(_idLogro);
            }
        }

        // comprobar si el logro es un logro de portero
        if (m_logrosPortero != null) {
            for (int i = 0; (i < m_logrosPortero.Count) && (logro == null); ++i) {
                _grupoContenedor = m_logrosPortero[i];
                logro = m_logrosPortero[i].GetLogro(_idLogro);
            }
        }

        // comprobar si el logro es un logro de duelo
        if (m_logrosDuelo != null) {
            for (int i = 0; (i < m_logrosDuelo.Count) && (logro == null); ++i) {
                _grupoContenedor = m_logrosDuelo[i];
                logro = m_logrosDuelo[i].GetLogro(_idLogro);
            }
        }

        // si no se ha encontrado el logro => indicar que no esta contenido en ningun grupo
        if (logro == null)
            _grupoContenedor = null;

        return logro;
    }


    /// <summary>
    /// Devuelve la cantidad total que hay de un determinado tipo
    /// </summary>
    /// <param name="_tipoLogros"></param>
    /// <returns></returns>
    public int GetNumTotalLogros(TipoLogros _tipoLogros) {
        // comprobar de que lista se deben contar los logros
        List<GrupoLogros> listaGrupoLogros = null;
        switch (_tipoLogros) {
            case TipoLogros.LANZADOR:
                listaGrupoLogros = m_logrosLanzador;
                break;
            case TipoLogros.PORTERO:
                listaGrupoLogros = m_logrosPortero;
                break;
            case TipoLogros.DUELO:
                listaGrupoLogros = m_logrosDuelo;
                break;
        }

        // calcular el numero de logros de la lista
        int numTotalLogros = 0;
        if (listaGrupoLogros != null) {
            foreach (GrupoLogros grupoLogros in listaGrupoLogros) {
                numTotalLogros += grupoLogros.numTotalLogros;
            }
        }

        return numTotalLogros;
    }


    /// <summary>
    /// Devuelve la cantidad de logros que se ha conseguido de un determinado tipo
    /// </summary>
    /// <param name="_tipoLogros"></param>
    /// <returns></returns>
    public int GetNumLogrosConseguidos(TipoLogros _tipoLogros) {
        // comprobar de que lista se deben contar los logros
        List<GrupoLogros> listaGrupoLogros = null;
        switch (_tipoLogros) {
            case TipoLogros.LANZADOR:
                listaGrupoLogros = m_logrosLanzador;
                break;
            case TipoLogros.PORTERO:
                listaGrupoLogros = m_logrosPortero;
                break;
            case TipoLogros.DUELO:
                listaGrupoLogros = m_logrosDuelo;
                break;
        }

        // calcular el numero de logros de la lista
        int numTotalLogrosConseguidos = 0;
        if (listaGrupoLogros != null) {
            foreach (GrupoLogros grupoLogros in listaGrupoLogros) {
                numTotalLogrosConseguidos += grupoLogros.nivelAlcanzado;
            }
        }

        return numTotalLogrosConseguidos;
    }


}


/// <summary>
/// Clase para gestionar un grupo de logros asociados a una determinada magnitud
/// </summary>
public class GrupoLogros {

    // ------------------------------------------------------------------------------
    // ---  ENUMERADO  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Magnitudes posibles que miden los logros
    /// </summary>
    public enum Magnitud {
        DIANAS_ACERTADAS,
        PUNTUACION_TOTAL_LANZADOR,
        DIANAS_PERFECTAS,
        PUNTUACION_TOTAL_PORTERO,
        DESPEJES,
        PARADAS,
        DUELOS_JUGADOS,
        DUELOS_GANADOS,
        DUELOS_PERFECTOS
    };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------

    /// <summary>
    /// Magnitud que mide este grupo de logros
    /// </summary>
    private Magnitud m_magnitud;

    /// <summary>
    /// Logros de este grupo
    /// </summary>
    private List<Logro> m_listaLogros;

    /// <summary>
    /// Nivel alcanzado dentro de este grupo de logros
    /// Nota: sirve de indice dentro de la lista de logros para apuntar al logro actual en base al progreso
    /// </summary>
    public int nivelAlcanzado { get { return CalcularNivelAlcanzado(progreso); } }

    /// <summary>
    /// Nombre de este grupo de logros
    /// </summary>
    public string nombre { get { return m_nombre; } }
    private string m_nombre = "";

    /// <summary>
    /// Descripcion asociada a este grupo de logros
    /// </summary>
    public string descripcion { 
        get {
            if (m_descripcion.Contains("{0}"))  // <= si en la descripcion existe este formato, mostrar en la descripcion el valor para superar el logro
                return string.Format(m_descripcion, m_listaLogros[Mathf.Clamp(nivelAlcanzado, 0, m_listaLogros.Count - 1)].valorSuperarLogro);
            else
                return m_descripcion;
        } }
    private string m_descripcion = "";

    /// <summary>
    /// Descripcion asociada a este grupo de logros CON EL FORMATO ADECUADO PARA INTERCALARLE EL VALOR QUE SE DESEE
    /// </summary>
    public string descriptionConFormato { get { return m_descripcion; } }

    /// <summary>
    /// Progreso del jugador dentro de este grupo de logros
    /// </summary>
    public int progreso { get { return GetValorMagnitud(m_magnitud); } }

    /// <summary>
    /// Progreso que hay que alcanzar para superar el nivel actual del logro
    /// </summary>
    public int valorSuperarLogro { get { return m_listaLogros[Mathf.Clamp(nivelAlcanzado, 0, m_listaLogros.Count - 1)].valorSuperarLogro; } }

    /// <summary>
    /// Devuelve un valor entre [0.0f ... 1.0f] correspondiente al porcentaje superado del nivel actual de este logro
    /// </summary>
    public float porcentajeSuperadoLogro {
        get {
            if (valorSuperarLogro == 0)
                return 0.0f;
            else
                return Mathf.Clamp01((float)progreso / (float)valorSuperarLogro);
        }
    }


    /// <summary>
    /// Recompensa que se obtiene al superar el nivel actual del logro
    /// </summary>
    public int recompensa { get { return m_listaLogros[Mathf.Clamp(nivelAlcanzado, 0, m_listaLogros.Count - 1)].recompensa; } }

    /// <summary>
    /// Prefijo comun que tienen los identificadores de los logros de este grupo (sirve de identificador de este grupo de logros)
    /// </summary>
    public string prefijoComunIdLogros { get { return m_prefijoComunIdLogros; } }
    private string m_prefijoComunIdLogros;

    /// <summary>
    /// Indica si en este grupo de logros ha habido una subida de nivel recientemente
    /// </summary>
    public bool subidaNivelReciente { get { return m_subidaNivelReciente; } set { m_subidaNivelReciente = value; } }
    private bool m_subidaNivelReciente = false;

    /// <summary>
    /// Devuelve el numero TOTAL de logros de este grupo
    /// </summary>
    public int numTotalLogros { get { return (m_listaLogros == null ? 0 : m_listaLogros.Count); } }

    /// <summary>
    /// Devuelve "true" se han superado todos los logros de este grupo
    /// </summary>
    public bool superadosTodosLosLogros { 
        get { 
            if (m_listaLogros == null || m_listaLogros.Count == 0)
                return true;
            else
                return (GetValorMagnitud(m_magnitud) >= m_listaLogros[m_listaLogros.Count - 1].valorSuperarLogro);
        } 
    }


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="_prefijoComunIdLogros"></param>
    /// <param name="_nombre"></param>
    /// <param name="_descripcion">Descripción del logro. Nota: si en la descripcion se quiere mostrar el "valorSuperarLogro" del logro actual</param>
    /// <param name="_listaLogros"></param>
    /// <param name="_magnitud"></param>
    public GrupoLogros(string _prefijoComunIdLogros, string _nombre, string _descripcion, List<Logro> _listaLogros, Magnitud _magnitud) {
        m_prefijoComunIdLogros = _prefijoComunIdLogros;
        m_nombre = _nombre;
        m_descripcion = _descripcion;
        m_listaLogros = _listaLogros;
        m_magnitud = _magnitud;

        // ordenar los logros de la lista por su propiedad "valorSuperarLogro"
        if (m_listaLogros != null)
            m_listaLogros.Sort(Logro.CompararPorValorSuperarLogro);
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Devuelve el valor correspondiente a la magnitud solicitada
    /// </summary>
    /// <param name="_magnitud"></param>
    /// <returns></returns>
    private int GetValorMagnitud(Magnitud _magnitud) {
        switch (_magnitud) {
            case Magnitud.DIANAS_ACERTADAS:
                return Interfaz.m_asThrower.targets;
            case Magnitud.PUNTUACION_TOTAL_LANZADOR:
                return Interfaz.m_asThrower.totalPoints;
            case Magnitud.DIANAS_PERFECTAS:
                return Interfaz.m_asThrower.perfects;
            case Magnitud.PUNTUACION_TOTAL_PORTERO:
                return Interfaz.m_asKeeper.totalPoints;
            case Magnitud.DESPEJES:
                return Interfaz.m_asKeeper.deflected;
            case Magnitud.PARADAS:
                return Interfaz.m_asKeeper.goalsStopped;
            case Magnitud.DUELOS_JUGADOS:
                return Interfaz.m_duelsPlayed;
            case Magnitud.DUELOS_GANADOS:
                return Interfaz.m_duelsWon;
            case Magnitud.DUELOS_PERFECTOS:
                return Interfaz.m_perfectDuels;
        }

        return 0;    // <= valor por defecto
    }


    /// <summary>
    /// Devuelve la informacion un logro que pertenece a este grupo (null si no se encuentra)
    /// </summary>
    /// <param name="_idLogro">Identificador del logro</param>
    /// <returns></returns>
    public Logro GetLogro(string _idLogro) {
        // buscar el logro en la lista
        if (m_listaLogros != null) {
            for (int i = 0; i < m_listaLogros.Count; ++i)
                if (m_listaLogros[i].id == _idLogro)
                    return m_listaLogros[i];
        }

        // si no se ha encontrado el logro
        return null;
    }


    /// <summary>
    /// Devuelve la informacion un logro que pertenece a este grupo (null si no se encuentra)
    /// </summary>
    /// <param name="_posicionLogro">Posicion del logro</param>
    /// <returns></returns>
    public Logro GetLogro(int _posicionLogro) {
        // buscar el logro en la lista
        if (m_listaLogros != null && _posicionLogro >= 0 && _posicionLogro < m_listaLogros.Count)
            return m_listaLogros[_posicionLogro];
        else
            // si no se ha encontrado el logro
            return null;
    }


    /// <summary>
    /// Calcula la recompensa acumulada desde el nivel "_fromLevel" al nivel "_toLevel"
    /// </summary>
    /// <param name="_fromLevel"></param>
    /// <param name="_toLevel"></param>
    /// <returns></returns>
    public int GetRecompensaAcumulada(int _fromLevel, int _toLevel) {
        // comprobar que los limites esten en rango
        if (_fromLevel < 0)
            _fromLevel = 0;
        if (_toLevel >= m_listaLogros.Count)
            _toLevel = m_listaLogros.Count - 1;

        // caso directo => devolver 0
        if (_fromLevel >= _toLevel)
            return 0;
        else {
            // calcular las recompensas acumuladas
            int recompensaAcumulada = 0;
            for (int i = _fromLevel; i < _toLevel; ++i) {
                recompensaAcumulada += m_listaLogros[i].recompensa;
            }

            return recompensaAcumulada;
        }
    }


    /// <summary>
    /// Calcula el nivel alcanzado dentro de este grupo de logros con el "_progreso" recibido como parametro
    /// </summary>
    /// <param name="_progreso"></param>
    /// <returns></returns>
    private int CalcularNivelAlcanzado(int _progreso) {
        if (m_listaLogros != null) {
            for (int i = 0; i < m_listaLogros.Count; ++i) {
                if (m_listaLogros[i] != null)
                {
                    if (_progreso < m_listaLogros[i].valorSuperarLogro)
                    {
                        return i;
                    }
                }
            }

            // el progreso es mayor que todos los logros del grupo => se devuelve el ultimo
            return (m_listaLogros.Count );
        }

        return 0;   // <= devolver por defecto 0
    }


    /// <summary>
    /// Metodo para comparar dos grupos de logros por su porcentaje de logro superado
    /// NOTA: como se va a utilizar para ordenar grupos de logros de mayor a menor, el resultado devuelto esta invertido
    /// </summary>
    /// <param name="_grupo1"></param>
    /// <param name="_grupo2"></param>
    /// <returns></returns>
    public static int CompararPorPorcentajeSuperadoLogro(GrupoLogros _grupo1, GrupoLogros _grupo2) {
        if (_grupo1 == null) {
            if (_grupo2 == null)
                return 0;
            else
                return -1;
        } else {
            if (_grupo2 == null)
                return 1;
            else
                // NOTA1: multiplico por 100 los valores para convertirlos en enteros
                return (int)(_grupo2.porcentajeSuperadoLogro * 100 - _grupo1.porcentajeSuperadoLogro * 100);    // <= devuelve el resultado inverito: grupo2 - grupo1
        }
    }

}


/// <summary>
/// Clase para representar un logro
/// </summary>
public class Logro {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // Identificador del logro (para identificar el logro contra los webservices)
    public string id { get { return m_id; } }
    private string m_id = "";

    /// <summary>
    /// Valor que debe conseguir el usuario para superar este logro
    /// </summary>
    public int valorSuperarLogro { get { return m_valorSuperarLogro; } }
    private int m_valorSuperarLogro = 0;

    /// <summary>
    /// Recompensa que obtiene el usuario al superar este logro
    /// </summary>
    public int recompensa { get { return m_recompensa; } }
    private int m_recompensa = 0;

    /*
    /// <summary>
    /// Lista de logros
    /// </summary>
    public static List<Logro> logros { get { return m_logros; } }
    private static List<Logro> m_logros = new List<Logro>();
    */

    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="_id">Identificador del logro (para identificar el logro contra los webservices)</param>
    /// <param name="_valorSuperarLogro">Valor que debe conseguir el usuario para superar este logro</param>
    /// <param name="_recompensa">Recompensa que obtiene el usuario al superar este logro</param>
    public Logro(string _id, int _valorSuperarLogro, int _recompensa) {
        m_id = _id;
        m_valorSuperarLogro = _valorSuperarLogro;
        m_recompensa = _recompensa;
        //logros.Add(this);
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Metodo para comparar dos logros por su "ValorSuperarLogro"
    /// </summary>
    /// <param name="_logro1"></param>
    /// <param name="_logro2"></param>
    /// <returns></returns>
    public static int CompararPorValorSuperarLogro(Logro _logro1, Logro _logro2) {
        if (_logro1 == null) {
            if (_logro2 == null)
                return 0;
            else
                return -1;
        } else {
            if (_logro2 == null)
                return 1;
            else
                return (_logro1.valorSuperarLogro - _logro2.valorSuperarLogro);
        }
    }

    /*
    public static void Clear() {
        if(logros == null) {
            m_logros = new List<Logro>();
        }
        logros.Clear();
    }
    */

}