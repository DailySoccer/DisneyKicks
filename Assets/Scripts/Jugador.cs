using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Clase para representar a un jugador (tirador, portero...) de este juego
/// </summary>
public class Jugador {
    public const string KEY_ID = "id";

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

    public string ID {
        get { return assetName; }
    }

    /// <summary>
    /// id del modelo (indica la posicion del modelo de este jugador dentro de los arrays "m_Goalkeepers" y "m_Throwers" de "Interfaz") que sera que el modelo ingame
    /// </summary>
    public int idModelo { 
        get { 
            if (m_idModelo == -1) {
                m_idModelo = Interfaz.instance.GetPositionInGoalkeepers(assetName);
                if (m_idModelo == -1) {
                    m_idModelo = Interfaz.instance.GetPositionInThrowers(assetName);
                }
            }
            return m_idModelo; 
        } 
    }
    private int m_idModelo = -1;

    /// <summary>
    /// nombre del asset del jugador (para su identificacion contra los servicios web o con arte)
    /// </summary>
    public string assetName { get { return m_assetName; } set { m_assetName = value; } }
    private string m_assetName;

    /// <summary>
    /// nombre del jugador
    /// </summary>
    public string nombre { get { return m_nombre; } set { m_nombre = value; } }
    private string m_nombre;

    /// <summary>
    /// Pais del jugador
    /// </summary>
    public string pais { get { return m_pais; } set { m_pais = value; }}
    private string m_pais;

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
    public int numDorsal { get { return m_numDorsal; } set { m_numDorsal = value; } }
    private int m_numDorsal;

    // Habilidades de este jugador
    public Habilidades.Skills[] habilidades { get { return m_habilidades; } set { m_habilidades = value; } }
    private Habilidades.Skills[] m_habilidades;

    /// <summary>
    /// Calidad del jugador (común, raro, épico)
    /// </summary>
    public CardQuality quality { get { return m_quality; } set { m_quality = value; } }
    private CardQuality m_quality;

    // Powerups de este jugador
    public Powerup[] powerups { get { return m_powerups; } set { m_powerups = value; } }
    private Powerup[] m_powerups;

    /*
     * DATOS A REGISTRAR EN PREFS
     */

    /// <summary>
    /// estado en el que se encuentra el jugador para el usuario de la aplicacion
    /// </summary>
    public Estado estado { get { return m_estado; } set { m_estado = value; } }
    private Estado m_estado;

    /// <summary>
    /// Nivel alcanzado del jugador
    /// </summary>
    public int nivel { get { return m_nivel; } set { m_nivel = value; } }
    private int m_nivel;

    /// <summary>
    /// Número de veces que se ha conseguido este jugador / carta
    /// </summary>
    public int cartas { get { return m_cartas; } set { m_cartas = value; } }
    private int m_cartas;


    public Dictionary<string, object> SaveData {
        get {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(KEY_ID, ID);
            data.Add("estado", estado);
            data.Add("nivel", nivel);
            data.Add("cartas", cartas);
            return data;
        }

        set {
            Debug.Assert(value[KEY_ID].ToString() == assetName, string.Format("SaveData: {0} != {1}", value[KEY_ID].ToString(), assetName));
            estado = value.ContainsKey("estado") ? (Estado) Enum.Parse(typeof(Estado), value["estado"].ToString()) : Estado.BLOQUEADO;
            nivel = value.ContainsKey("nivel") ? int.Parse(value["nivel"].ToString()) : 0;
            cartas = value.ContainsKey("cartas") ? int.Parse(value["cartas"].ToString()) : 0;
        }
    }

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
    public Jugador() {
        m_estado = Estado.BLOQUEADO;
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
        info += "   quality=" + m_quality.ToString();
        info += "   nivel=" + m_nivel.ToString();
        info += "   cards=" + m_cartas.ToString();
        info += "   estado=" + m_estado;

        if (m_habilidades != null)
            info += "   habilidades=" + m_habilidades.Length;
        else
            info += "   habilidades=NULL";

        if (m_powerups != null)
            info += "   powerups=" + m_powerups.Length;
        else
            info += "   powerups=NULL";
        
        return info;
    }
}


/// <summary>
/// Clase con la informacion de los jugadores
/// </summary>
public class InfoJugadores {

    const string KEY_PORTEROS = "porteros";
    const string KEY_TIRADORES = "tiradores";

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
        // F4KE: Crear la lista de porteros a cholon
        m_listaPorteros = new List<Jugador>();
        m_listaPorteros.AddRange( JugadorData.Porteros );

        // F4KE: crear la lista de lanzadores a cholon
        m_listaTiradores = new List<Jugador>();
        m_listaTiradores.AddRange( JugadorData.Tiradores );

        string dataBefore = SaveData;
        Debug.Log("SaveData BEFORE: " + dataBefore);
        SaveData = dataBefore;
        Debug.Log("SaveData SAVED: " + SaveData);
        string dataRestored = SaveData;
        Debug.Log("SaveData RESTORED: " + dataRestored);
        Debug.Assert(dataBefore == dataRestored, "SaveData ERROR");
    }

    public string SaveData {
        get {
            Dictionary<string, List<Dictionary<string, object>>> data = new Dictionary<string, List<Dictionary<string, object>>>();
            data.Add(KEY_PORTEROS, SaveDataFromList(m_listaPorteros));
            data.Add(KEY_TIRADORES, SaveDataFromList(m_listaTiradores));
            return MiniJSON.Json.Serialize(data);
        }

        set {
            Dictionary<string, object> data = (Dictionary<string, object>) MiniJSON.Json.Deserialize(value);
            SaveDataToList(data[KEY_PORTEROS] as List<object>, m_listaPorteros);
            SaveDataToList(data[KEY_TIRADORES] as List<object>, m_listaTiradores);
        }
    }

    private List<Dictionary<string, object>> SaveDataFromList(List<Jugador> lista) {
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
        foreach(Jugador jugador in lista) {
            result.Add( jugador.SaveData );
        }
        return result;
    }

    private void SaveDataToList(List<object> data, List<Jugador> lista) {
        foreach(object jugadorSaved in data) {
            Dictionary<string, object> saveData = jugadorSaved as Dictionary<string, object>;
            Jugador jugador = lista.Find( el => el.ID == saveData[Jugador.KEY_ID].ToString() );
            if (jugador != null) {
                jugador.SaveData = saveData;
            }
            else {
                Debug.LogError("SaveDataToList: Not Exists " + saveData["id"]);
            }
        }
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

    public void ChangeAllToState(Jugador.Estado estado) {
        for (int i = 1; i < m_listaPorteros.Count; ++i) {
            m_listaPorteros[i].estado = estado;
        }

        for (int i = 1; i < m_listaTiradores.Count; ++i) {
            m_listaTiradores[i].estado = estado;
        }
    }

}