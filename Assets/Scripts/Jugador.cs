using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase para representar a un jugador (tirador, portero...) de este juego
/// </summary>
public class Jugador {


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// estados en los que puede estar el jugador para el usuario
    /// </summary>
    public enum Estado { ADQUIRIDO, DISPONIBLE, BLOQUEADO };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// id del modelo (indica la posicion del modelo de este jugador dentro de los arrays "m_Goalkeepers" y "m_Throwers" de "Interfaz") que sera que el modelo ingame
    /// </summary>
    public int idModelo { get { return m_idModelo; } }
    private int m_idModelo;

    /// <summary>
    /// nombre del asset del jugador (para su identificacion contra los servicios web o con arte)
    /// </summary>
    public string assetName { get { return m_assetName; } }
    private string m_assetName;

    /// <summary>
    /// nombre del jugador
    /// </summary>
    public string nombre { get { return m_nombre; } }
    private string m_nombre;

    /// <summary>
    /// Pais del jugador
    /// </summary>
    public string pais { get { return m_pais; } }
    private string m_pais;

    /// <summary>
    /// estado en el que se encuentra el jugador para el usuario de la aplicacion
    /// </summary>
    public Estado estado { get { return m_estado; } set { m_estado = value; } }
    private Estado m_estado;

    /// <summary>
    /// dinero SOFT con el que se desbloquea este jugador
    /// </summary>
    public int precioSoft { get { return m_precioSoft; } }
    private int m_precioSoft;

    /// <summary>
    /// dinero HARD con el que se desbloquea este jugador
    /// </summary>
    public int precioHard { get { return m_precioHard; } }
    private int m_precioHard;

    /// <summary>
    /// fase a partir de la cual se desbloquea el jugador
    /// </summary>
    public int faseDesbloqueo { get { return m_faseDesbloqueo; } }
    private int m_faseDesbloqueo;

    /// <summary>
    /// dinero HARD que vale el jugador si se quiere comprar antes de haber sido desbloqueado
    /// </summary>
    public int precioEarlyBuy { get { return m_precioEarlyBuy; } }
    private int m_precioEarlyBuy;

    /// <summary>
    /// devuelve la posicion del jugador en su correspondiente array (de tirador o portero)
    /// </summary>
    public int index {
        get {
            return InfoJugadores.instance.GetJugadorIndex(this);
        }
    }

    
    /// <summary>
    /// Numero de camiseta de este jugador
    /// </summary>
    public int numDorsal { get { return m_numDorsal; } }
    private int m_numDorsal;

    // Habilidades de este jugador
    public Habilidades.Skills[] habilidades { get { return m_habilidades; } }
    private Habilidades.Skills[] m_habilidades;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="_idModelo">Indice del modelo del jugador en el array "Goalkeepers" o "Throwers" de "ifcInterfaz"</param>
    /// <param name="_assetName">Identificador del asset (el utilizado por backend y arte)</param>
    /// <param name="_nombre">Nombre del jugador</param>
    /// <param name="_pais"></param>
    /// <param name="_precioSoft"></param>
    /// <param name="_precioHard"></param>
    /// <param name="_faseDesbloqueo">fase en la que, al conseguir los 4 objetivos, se desbloquea el jugador</param>
    /// <param name="_precioEarlyBuy"></param>
    /// <param name="_numDorsal"></param>
    /// <param name="_habilidades">Habilidades de este jugador</param>
    /// <param name="_estado"></param>
    public Jugador(int _idModelo, string _assetName, string _nombre, string _pais, int _precioSoft, int _precioHard, int _faseDesbloqueo = 0, int _precioEarlyBuy = 0, int _numDorsal = 7, Habilidades.Skills[] _habilidades = null, Estado _estado = Estado.BLOQUEADO) {
        m_idModelo = _idModelo;
        m_assetName = _assetName;
        m_nombre = _nombre;
        m_pais = _pais;
        m_precioSoft = _precioSoft;
        m_precioHard = _precioHard;
        m_faseDesbloqueo = _faseDesbloqueo;
        m_precioEarlyBuy = _precioEarlyBuy;
        m_numDorsal = _numDorsal;
        m_habilidades = _habilidades;
        m_estado = _estado;
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Comprueba si este jugador tiene una determinada habilidad
    /// </summary>
    /// <param name="_habilidad"></param>
    /// <returns></returns>
    public bool TieneHabilidad(Habilidades.Skills _habilidad) {
        if (m_habilidades == null)
            return false;
        else {
            for (int i = 0; i < m_habilidades.Length; ++i) {
                if (m_habilidades[i] == _habilidad)
                    return true;
            }

            // no se ha encontrado la habilidad
            return false;
        }
    }


    /// <summary>
    /// Muestra toda la info de esta clase
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        string info = "Jugador";
        info += "   idModelo=" + m_idModelo;
        info += "   assetName=" + m_assetName;
        info += "   nombre=" + m_nombre;
        info += "   pais=" + m_pais;
        info += "   precioSoft=" + m_precioSoft;
        info += "   precioHard=" + m_precioHard;
        info += "   faseDesbloqueo=" + m_faseDesbloqueo;
        info += "   precioEarlyBuy=" + m_precioEarlyBuy;
        info += "   estado=" + m_estado;

        if (m_habilidades != null)
            info += "   habilidades=" + m_habilidades.Length;
        else
            info += "   habilidades=NULL";
        return info;
    }
}


/// <summary>
/// Clase con la informacion de los jugadores
/// </summary>
public class InfoJugadores {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// instancia de esta clase
    /// </summary>
    public static InfoJugadores instance { 
        get {
            if (m_instance == null)
                m_instance = new InfoJugadores();
            return m_instance; 
        } 
    }
    private static InfoJugadores m_instance;

    // lista de jugadores tiradores
    private List<Jugador> m_listaTiradores;

    // lista de jugadores portero
    private List<Jugador> m_listaPorteros;

    /// <summary>
    /// Numero de lanzadores
    /// </summary>
    public int numLanzadores { get { return (m_listaTiradores == null) ? 0 : m_listaTiradores.Count; } }

    /// <summary>
    /// Numero de porteros
    /// </summary>
    public int numPorteros { get { return (m_listaPorteros == null) ? 0 : m_listaPorteros.Count; } }


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public InfoJugadores() {
        m_listaTiradores = new List<Jugador>();
        m_listaPorteros = new List<Jugador>();

        // F4KE: crear la lista de lanzadores a cholon
        m_listaTiradores.Add(new Jugador(2, "IT_PLY_ST_0003", "Vincent Lacombe", LocalizacionManager.instance.GetTexto(261), 0, 0, 0, 0, 4, null, Jugador.Estado.ADQUIRIDO));
        m_listaTiradores.Add(new Jugador(3, "IT_PLY_ST_0004", "Mauro Tankara", LocalizacionManager.instance.GetTexto(262), 0, 0, 0, 0, 5, null, Jugador.Estado.ADQUIRIDO));
        m_listaTiradores.Add(new Jugador(4, "IT_PLY_ST_0005", "Hans Fritzlang", LocalizacionManager.instance.GetTexto(263), 3750, 100, 3, 125, 6));
        m_listaTiradores.Add(new Jugador(1, "IT_PLY_ST_0002", "Andrew Crowley", LocalizacionManager.instance.GetTexto(264), 8800, 200, 6, 420, 3, new Habilidades.Skills[] { Habilidades.Skills.Prima }));
        m_listaTiradores.Add(new Jugador(0, "IT_PLY_ST_0001", "Manuel Villalba", LocalizacionManager.instance.GetTexto(265), 9000, 150, 8, 380, 2, new Habilidades.Skills[] { Habilidades.Skills.Vista_halcon }));
        m_listaTiradores.Add(new Jugador(16, "IT_PLY_ST_0108", "Laszlo Pionescu", LocalizacionManager.instance.GetTexto(266), 11040, 220, 13, 500, 12, new Habilidades.Skills[] { Habilidades.Skills.Goleador }));
        m_listaTiradores.Add(new Jugador(11, "IT_PLY_ST_0101", "Andre Van Der Moor", LocalizacionManager.instance.GetTexto(267), 10800, 220, 15, 550, 7, new Habilidades.Skills[] { Habilidades.Skills.Prima, Habilidades.Skills.Vista_halcon }));
        m_listaTiradores.Add(new Jugador(19, "IT_PLY_ST_0111", "Olaf Larrson", LocalizacionManager.instance.GetTexto(268), 21000, 240, 19, 575, 15, new Habilidades.Skills[] { Habilidades.Skills.Prima, Habilidades.Skills.VIP }));
        m_listaTiradores.Add(new Jugador(22, "IT_PLY_ST_0114", "Franz Raissenberg", LocalizacionManager.instance.GetTexto(269), 19250, 195, 24, 450, 2, new Habilidades.Skills[] { Habilidades.Skills.VIP, Habilidades.Skills.Vista_halcon }));
        m_listaTiradores.Add(new Jugador(13, "IT_PLY_ST_0103", "Ben Massala", LocalizacionManager.instance.GetTexto(270), 27200, 210, 29, 480, 9, new Habilidades.Skills[] { Habilidades.Skills.Mago_balon }));
        m_listaTiradores.Add(new Jugador(21, "IT_PLY_ST_0113", "Joao Soares", LocalizacionManager.instance.GetTexto(271), 33600, 250, 30, 600, 10, new Habilidades.Skills[] { Habilidades.Skills.Mago_balon, Habilidades.Skills.VIP }));
        m_listaTiradores.Add(new Jugador(12, "IT_PLY_ST_0102", "Mario Barrenchi", LocalizacionManager.instance.GetTexto(272), 33745, 250, 37, 675, 8, new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.VIP }));
        m_listaTiradores.Add(new Jugador(23, "IT_PLY_ST_0115", "Hassan Nagala", LocalizacionManager.instance.GetTexto(273), 59500, 550, 49, 900, 3, new Habilidades.Skills[] { Habilidades.Skills.Mago_balon, Habilidades.Skills.Goleador }));
        m_listaTiradores.Add(new Jugador(15, "IT_PLY_ST_0105", "Yanis Paidopoulos", LocalizacionManager.instance.GetTexto(274), 56160, 500, 55, 1000, 11, new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.Prima }));
        m_listaTiradores.Add(new Jugador(24, "IT_PLY_ST_0116", "Kurt Mulligan", LocalizacionManager.instance.GetTexto(275), 50040, 400, 57, 850, 4, new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.Vista_halcon }));

        // F4KE: Crear la lista de porteros a cholon
        m_listaPorteros.Add(new Jugador(2, "IT_PLY_GK_0003", "Jack Donovan", LocalizacionManager.instance.GetTexto(276), 0, 0, 0, 0, 1, null, Jugador.Estado.ADQUIRIDO));
        m_listaPorteros.Add(new Jugador(1, "IT_PLY_GK_0002", "Alfredo Del Valle", LocalizacionManager.instance.GetTexto(277), 3500, 75, 4, 100, 1));
        m_listaPorteros.Add(new Jugador(5, "IT_PLY_GK_0103", "Santiago Resquicio", LocalizacionManager.instance.GetTexto(278), 5200, 120, 7, 225, 1, new Habilidades.Skills[] { Habilidades.Skills.Practico }));
        m_listaPorteros.Add(new Jugador(4, "IT_PLY_GK_0102", "Pietro Capiente", LocalizacionManager.instance.GetTexto(279), 30600, 350, 35, 650, 1, new Habilidades.Skills[] { Habilidades.Skills.Practico, Habilidades.Skills.Barrera }));
        m_listaPorteros.Add(new Jugador(3, "IT_PLY_GK_0101", "Matsuhiro Shintao", LocalizacionManager.instance.GetTexto(280), 56000, 600, 47, 1050, 1, new Habilidades.Skills[] { Habilidades.Skills.Premonicion }));
        
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Devuelve el tirador en la posicion "_posicion" de la lista de tiradores (o null si no existe)
    /// </summary>
    /// <param name="_posicion"></param>
    /// <returns></returns>
    public Jugador GetTirador(int _posicion) {
        if (_posicion < 0 || _posicion >= m_listaTiradores.Count)
            return null;
        else
            return m_listaTiradores[_posicion];
    }


    /// <summary>
    /// Devuelve el portero en la posicion "_posicion" de la lista de porteros (o null si no existe)
    /// </summary>
    /// <param name="_posicion"></param>
    /// <returns></returns>
    public Jugador GetPortero(int _posicion) {
        if (_posicion < 0 || _posicion >= m_listaPorteros.Count)
            return null;
        else
            return m_listaPorteros[_posicion];
    }


    /// <summary>
    /// Busca un jugador (independientemente de que juegue como tirador o portero) y devuelve su informacion.
    /// NOTA: devuelve null en caso de que no exista
    /// </summary>
    /// <param name="_codigo"></param>
    /// <returns></returns>
    public Jugador GetJugador(string _codigo) {
        // buscar en la lista de tiradores
        for (int i = 0; i < m_listaTiradores.Count; ++i) {
            if (m_listaTiradores[i].assetName == _codigo)
                return m_listaTiradores[i];
        }

        // buscar en la lista de porteros
        for (int i = 0; i < m_listaPorteros.Count; ++i) {
            if (m_listaPorteros[i].assetName == _codigo)
                return m_listaPorteros[i];
        }

        return null;
    }


    /// <summary>
    /// Devuelve la posicion del proximo jugador tirador (a partir de la posicion "_posicion") que este en estado de ADQUIRIDO
    /// Nota: en caso de no encontrar jugador devuelve "_posicion"
    /// </summary>
    /// <param name="_posicion">Posicion desde la que iniciar la busqueda</param>
    /// <returns></returns>
    public int GetPosicionSiguienteTiradorAdquirido(int _posicion = 0) {
        // buscar desde la "_posicion" hasta el final
        for (int i = _posicion; i < m_listaTiradores.Count; ++i) {
            if (m_listaTiradores[i].estado == Jugador.Estado.ADQUIRIDO)
                return i;
        }

        // buscar desde el principio hasta la "_posicion"
        for (int i = 0; i < _posicion; ++i) {
            if (m_listaTiradores[i].estado == Jugador.Estado.ADQUIRIDO)
                return i;
        }

        // devolver por defecto la _posicion
        return _posicion;
    }


    /// <summary>
    /// Devuelve la posicion del proximo jugador portero (a partir de la posicion "_posicion") que este en estado de ADQUIRIDO
    /// Nota: en caso de no encontrar jugador devuelve "_posicion"
    /// </summary>
    /// <param name="_posicion">Posicion desde la que iniciar la busqueda</param>
    /// <returns></returns>
    public int GetPosicionSiguientePorteroAdquirido(int _posicion = 0) {
        // buscar desde la "_posicion" hasta el final
        for (int i = _posicion; i < m_listaPorteros.Count; ++i) {
            if (m_listaPorteros[i].estado == Jugador.Estado.ADQUIRIDO)
                return i;
        }

        // buscar desde el principio hasta la "_posicion"
        for (int i = 0; i < _posicion; ++i) {
            if (m_listaPorteros[i].estado == Jugador.Estado.ADQUIRIDO)
                return i;
        }

        // devolver por defecto la _posicion
        return _posicion;
    }


    /// <summary>
    /// Devuelve la posicion del jugador en el array de tiradores o porteros (segun donde lo encuentre)
    /// Nota: en caso de no encontrar jugador devuelve "-1"
    /// </summary>
    /// <param name="_jugador">Jugador del que se quiere obtener el indice</param>
    /// <returns></returns>
    public int GetJugadorIndex(Jugador _jugador) {
        for (int i = 0; i < m_listaPorteros.Count; ++i) {
            if (m_listaPorteros[i].assetName == _jugador.assetName)
                return i;
        }
        
        for (int i = 0; i < m_listaTiradores.Count; ++i) {
            if (m_listaTiradores[i].assetName == _jugador.assetName)
                return i;
        }
        
        // devolver -1 si no se ha encotnrado
        return -1;
    }


    /// <summary>
    /// Metodo que en funcion de la "_ultimaFaseDesbloqueada" actualiza el estado de los jugadores para que pasen de "BLOQUEADO" a "DISPONIBLE" si procede
    /// </summary>
    /// <param name="_ultimaFaseDesbloqueada"></param>
    public void RefreshJugadoresDesbloqueados(int _ultimaFaseDesbloqueada) {
        // comprobar los lanzadores
        if (m_listaTiradores != null) {
            foreach (Jugador jugador in m_listaTiradores) {
                if (jugador.estado == Jugador.Estado.BLOQUEADO && jugador.faseDesbloqueo <= _ultimaFaseDesbloqueada)
                    jugador.estado = Jugador.Estado.DISPONIBLE;
            }
        }

        // comprobar los porteros
        if (m_listaPorteros != null) {
            foreach (Jugador jugador in m_listaPorteros) {
                if (jugador.estado == Jugador.Estado.BLOQUEADO && jugador.faseDesbloqueo <= _ultimaFaseDesbloqueada)
                    jugador.estado = Jugador.Estado.DISPONIBLE;
            }
        }
    }


    /// <summary>
    /// Devuelve el jugador que se desbloquea en la fase "_numFase"
    /// NOTA: devuelve null si no se ha encontrado ningun jugador
    /// </summary>
    /// <param name="_numFaseSuperada"></param>
    /// <returns></returns>
    public Jugador GetJugadorDesbloqueableEnFase(int _numFaseSuperada) {
        ++_numFaseSuperada; // <= se suma 1 por como se definen las fases de desbloqueo de los jugadores

        // buscar en la lista de lanzadores
        if (m_listaTiradores != null) {
            for (int i = 0; i < m_listaTiradores.Count; ++i) {
                if (m_listaTiradores[i].estado == Jugador.Estado.BLOQUEADO && m_listaTiradores[i].faseDesbloqueo == _numFaseSuperada)
                    return m_listaTiradores[i];
            }
        }

        // buscar en la lista de porteros
        if (m_listaPorteros != null) {
            for (int i = 0; i < m_listaPorteros.Count; ++i) {
                if (m_listaPorteros[i].estado == Jugador.Estado.BLOQUEADO && m_listaPorteros[i].faseDesbloqueo == _numFaseSuperada)
                    return m_listaPorteros[i];
            }
        }

        // no se ha encontrado ningun jugador
        return null;
    }


    /// <summary>
    /// Comprueba si un determinado jugador pertenece a la lista de lanzadores
    /// </summary>
    /// <param name="_assetName"></param>
    /// <returns></returns>
    public bool EsLanzador(string _assetName) {
        if (m_listaTiradores != null) {
            for (int i = 0; i < m_listaTiradores.Count; ++i) {
                if (m_listaTiradores[i].assetName == _assetName) {
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Devuelve true si el usuario ha adquirido al menos un jugador LANZADOR
    /// </summary>
    /// <returns></returns>
    public bool HayAlgunLanzadorAdquirido() {
        if (m_listaTiradores != null) {
            for (int i = 0; i < m_listaTiradores.Count; ++i)
                if (m_listaTiradores[i].estado == Jugador.Estado.ADQUIRIDO)
                    return true;
        }

        return false;
    }


    /// <summary>
    /// Devuelve true si el usuario ha adquirido al menos un jugador PORTERO
    /// </summary>
    /// <returns></returns>
    public bool HayAlgunPorteroAdquirido() {
        if (m_listaPorteros != null) {
            for (int i = 0; i < m_listaPorteros.Count; ++i) {
                if (m_listaPorteros[i].estado == Jugador.Estado.ADQUIRIDO) {
                    Debug.LogWarning(">>> El jugador " + m_listaPorteros[i].nombre + " esta ADQUIRIDO");
                    return true;
                }
            }
        }

        return false;
    }


}