using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Clase para representar a un usuario de esta aplicacion
/// </summary>
public class Usuario {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public string uid { get { return m_uid; } set { m_uid = value; } }
    private string m_uid;

    public string alias { get { return m_alias; } set { m_alias = value; } }
    private string m_alias;

    public int numVictorias { get { return m_numVictorias; } set { m_numVictorias = value; } }
    private  int m_numVictorias;

    public int numDerrotas { get { return m_numDerrotas; } set { m_numDerrotas = value; } }
    public int m_numDerrotas;

    public bool initMode { get { return m_initMode; } set { m_initMode = value; } }
    private  bool m_initMode;

    public Jugador charThrower { get { return m_charThrower; } set { m_charThrower = value; } }
    private  Jugador m_charThrower;

    public Jugador charGoalkeeper { get { return m_charGoalkeeper; } set { m_charGoalkeeper = value; } }
    private Jugador m_charGoalkeeper;

    public Equipacion equipacionGoalkeeper { get { return m_equipacionGoalkeeper; } set { m_equipacionGoalkeeper = value; } }
    private Equipacion m_equipacionGoalkeeper;

    public Equipacion equipacionShooter { get { return m_equipacionShooter; } set { m_equipacionShooter = value; } }
    private Equipacion m_equipacionShooter;

    /// <summary>
    /// Escudo seleccionado por el usuario para su partida (puede ser null)
    /// </summary>
    public Escudo escudo { get { return m_escudo; } set { m_escudo = value; } }
    private Escudo m_escudo;

    /// <summary>
    /// Devuelve el jugador por defecto
    /// </summary>
    public Jugador DefaultCharacter { get{return (m_initMode ? m_charGoalkeeper : m_charThrower);} set {}}

    /// <summary>
    /// Devuelve el jugador secundario (si el usuario juega en modo portero => el lanzador, si el usuario juega en modo lanzador => el portero)
    /// </summary>
    public Jugador secondaryCharacter { get { return (m_initMode ? m_charThrower : m_charGoalkeeper); } }

    public bool yoRobot { get { return m_yoRobot; } set { m_yoRobot = value; } }
    private bool m_yoRobot;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="_alias"></param>
    /// <param name="_numVictorias">Numero de victorias</param>
    /// <param name="_numDerrotas">Numero de derrotas</param>
    /// <param name="_uid">Id de juego del usuario</param>
    /// <param name="_yoRobot">Indica si el usuario es un bot</param>
    public Usuario(string _alias, int _numVictorias, int _numDerrotas, string _uid, bool _yoRobot = false) {
        m_alias = _alias;
        m_numVictorias = _numVictorias;
        m_numDerrotas = _numDerrotas;
        m_uid = _uid;
        m_yoRobot = _yoRobot;
    }


    public Usuario(UsuarioNet _usuario) {
        m_uid = _usuario.uid;
        m_alias = _usuario.alias;
        m_numVictorias = _usuario.numVictorias;
        m_numDerrotas = _usuario.numDerrotas;
        m_initMode = _usuario.initMode;
        m_charGoalkeeper = InfoJugadores.instance.GetJugador(_usuario.characterGoalkeeper);
        m_charThrower = InfoJugadores.instance.GetJugador(_usuario.characterThrower);
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------



    /// <summary>
    /// 
    /// </summary>
    /// <param name="_alias"></param>
    /// <param name="_numVictorias"></param>
    /// <param name="_numDerrotas"></param>
    /// <param name="uid"></param>
    public void SetValues(string _alias, int _numVictorias, int _numDerrotas, string uid) {
        m_alias = _alias;
        m_numVictorias = _numVictorias;
        m_numDerrotas = _numDerrotas;
        m_uid = uid;
    }


    /// <summary>
    /// Devuelve el ratio de victorias de este usuario
    /// </summary>
    /// <returns></returns>
    public int getRatio() {
        if (m_numVictorias + m_numDerrotas == 0)
            return 0;
        else {
            return (int) ((float) m_numVictorias / (float) (m_numVictorias + m_numDerrotas)*100f);
        }
    }
}


/// <summary>
/// Clase para gestionar los usuarios favoritos
/// </summary>
public class UsuariosFavoritos {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static UsuariosFavoritos instance { 
        get { 
            if (m_instance == null)
                m_instance = new UsuariosFavoritos();
                return m_instance; 
        } 
    }
    private static UsuariosFavoritos m_instance;


    /// <summary>
    /// lista de usuarios favoritos reales
    /// </summary>
    private List<Usuario> m_listaFavoritos;

    /// <summary>
    /// lista de usuarios favoritos bots (nota: los bots se pierden al cerrar el juego)
    /// </summary>
    private List<Usuario> m_listaFavoritosBots;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public UsuariosFavoritos() {
        m_listaFavoritos = new List<Usuario>();
        m_listaFavoritosBots = new List<Usuario>();
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Asigna la lista de favoritos
    /// </summary>
    /// <param name="_listaFavoritos"></param>
    public void SetFavoritos(List<Usuario> _listaFavoritos) {
        if (_listaFavoritos == null)
            m_listaFavoritos.Clear();
        else
            m_listaFavoritos = _listaFavoritos;
    }


    /// <summary>
    /// Devuelve la lista de favoritos
    /// </summary>
    /// <returns></returns>
    public List<Usuario> GetFavoritos() {
        List<Usuario> listaGeneral = new List<Usuario>();

        foreach (Usuario usuario in m_listaFavoritos)
            listaGeneral.Add(usuario);

        foreach(Usuario usuario in m_listaFavoritosBots)
            listaGeneral.Add(usuario);

        return listaGeneral;
    }


    /// <summary>
    /// Comprueba si en los favoritos existe un usuario con el alias recibido como parametro
    /// </summary>
    /// <param name="_alias"></param>
    /// <param name="_caseSensitive">True si en la comparacion del alias son relevantes las mayusculas y minusculas, false en caso contrario</param>
    /// <returns></returns>
    public bool ExisteAlias(string _alias, bool _caseSensitive = false) {
        if (m_listaFavoritos != null) {

            if (_caseSensitive) {
                // comprobacion sensible a mayusculas
                for (int i = 0; i < m_listaFavoritos.Count; ++i) {
                    if (m_listaFavoritos[i].alias == _alias)
                        return true;
                }
                for (int i = 0; i < m_listaFavoritosBots.Count; ++i) {
                    if (m_listaFavoritosBots[i].alias == _alias)
                        return true;
                }
            } else {
                // comprobacion NO sensible a mayusculas
                for (int i = 0; i < m_listaFavoritos.Count; ++i) {
                    if (m_listaFavoritos[i].alias.ToUpper() == _alias.ToUpper())
                        return true;
                }
                for (int i = 0; i < m_listaFavoritosBots.Count; ++i) {
                    if (m_listaFavoritosBots[i].alias.ToUpper() == _alias.ToUpper())
                        return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Añade un usuario favorito al listado
    /// </summary>
    /// <param name="_usuario"></param>
    /// <returns>Devuelve true si el usuario se ha añadido correctamente, false si no (por ejemplo si ya existe un usuario con el mismo alias en el listado)</returns>
    public bool AddFavorito(Usuario _usuario) {
        if (m_listaFavoritos != null && _usuario != null) {
            if (!ExisteAlias(_usuario.alias)) {
                m_listaFavoritos.Add(_usuario);
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Añade un bot al listado de favoritos
    /// </summary>
    /// <param name="_usuarioBot"></param>
    /// <returns></returns>
    public bool AddFavoritoBot(Usuario _usuarioBot) {
        if (m_listaFavoritosBots != null && _usuarioBot != null) {
            if (!ExisteAlias(_usuarioBot.alias)) {
                m_listaFavoritosBots.Add(_usuarioBot);
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Elimina del listado de alias al usuario con el alias recibido como parametro
    /// </summary>
    /// <param name="_alias"></param>
    /// <returns></returns>
    public bool DeleteFavorito(string _alias) {
        // buscar en la lista de usuarios favoritos
        if (m_listaFavoritos != null) {
            for (int i = 0; i < m_listaFavoritos.Count; ++i) {
                if (m_listaFavoritos[i].alias == _alias) {
                    m_listaFavoritos.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Elimina del listado de bots favoritos  al usuario con el alias recibido como parametro
    /// </summary>
    /// <param name="_alias"></param>
    /// <returns></returns>
    public bool DeleteFavoritoBot(string _alias) {
        // buscar en la lista de bots favoritos
        if (m_listaFavoritosBots != null) {
            for (int i = 0; i < m_listaFavoritosBots.Count; ++i) {
                if (m_listaFavoritosBots[i].alias == _alias) {
                    m_listaFavoritosBots.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }


    /*
    /// <summary>
    /// Metodo fake para crear una lista de usuarios favoritos para pruebas
    /// </summary>
    /// <param name="_numFavoritos"></param>
    /// <returns></returns>
    public void F4KE_GenerarInstanciaParaPruebas(int _numFavoritos) {
        m_listaFavoritos.Clear();

        for (int i = 0; i < _numFavoritos; ++i) {
            Usuario usuario = new Usuario("Alias_" + (i + 1), (int) (Random.value * 100), (int) (Random.value * 100), (uint) i);
            m_listaFavoritos.Add(usuario);
        }

        Debug.Log(">>> Genero una lista de " + m_listaFavoritos.Count + " usuarios favoritos F4KE");
    }
    */

}
