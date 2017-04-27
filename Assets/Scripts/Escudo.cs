using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EscudosManager {
    

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static EscudosManager instance {
        get {
            if (m_instance == null)
                m_instance = new EscudosManager();
            return m_instance;
        }
    }
    private static EscudosManager m_instance;


    /// <summary>
    /// Lista con los posibles tipos de escudos disponibles
    /// </summary>
    private List<Escudo> m_listaEscudos;

    /// <summary>
    /// Escudo por defecto (el que se utiliza para indicar que el jugador tiene un multiplicador x1)
    /// </summary>
    public Escudo escudoPorDefecto { get { return m_EscudoPorDefecto; } }
    private Escudo m_EscudoPorDefecto;

    /// <summary>
    /// Escudo (multiplicador de puntuacion) equipado actualmente
    /// </summary>
    public static Escudo escudoEquipado = EscudosManager.instance.escudoPorDefecto;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public EscudosManager() {
        m_listaEscudos = new List<Escudo>();

        // crear el escudo por defecto
        m_EscudoPorDefecto = new Escudo("", "", 0, 1.0f, 0, 0, "", int.MaxValue, false);

        // F4KE: generar la lista de escudos
        m_listaEscudos.Add(new Escudo("IT_SHIELD_01", LocalizacionManager.instance.GetTexto(139), 1, 1.5f, 12, 0, LocalizacionManager.instance.GetTexto(142), 0, false));
        m_listaEscudos.Add(new Escudo("IT_SHIELD_02", LocalizacionManager.instance.GetTexto(140), 2, 1.8f, 18, 20, LocalizacionManager.instance.GetTexto(143), 0));
        m_listaEscudos.Add(new Escudo("IT_SHIELD_03", LocalizacionManager.instance.GetTexto(141), 3, 2.0f, 20, 45, LocalizacionManager.instance.GetTexto(144), 0));
    }


    /// <summary>
    /// Devuelve el numero de escudos registrados
    /// </summary>
    /// <returns></returns>
    public int GetNumEscudos() {
        if (m_listaEscudos == null)
            return 0;
        else
            return m_listaEscudos.Count;
    }

    
    /// <summary>
    /// Devuelve un escudo a partir de su "_id" (o null si no existe)
    /// </summary>
    /// <param name="_id">Identificador del escudo (es el que se usa para identificar el escudo contra los web services)</param>
    /// <returns></returns>
    public Escudo GetEscudo(string _id) {
        if (m_listaEscudos != null) {
            for (int i = 0; i < m_listaEscudos.Count; ++i) {
                if (string.Compare(m_listaEscudos[i].id, _id) == 0)
                    return m_listaEscudos[i];
            }
        }

        return null;
    }




    /// <summary>
    /// Devuelve un escudo a partir de su "_posicionEnListaEscudos" (o null si no existe)
    /// </summary>
    /// <param name="_posicionEnListaEscudos">Posicion (orden) dentro de la lista de escudos</param>
    /// <returns></returns>
    public Escudo GetEscudo(int _posicionEnListaEscudos) {
        if (m_listaEscudos == null)
            return null;
        
        if (_posicionEnListaEscudos < 0 || _posicionEnListaEscudos >= m_listaEscudos.Count)
            return null;

        return m_listaEscudos[_posicionEnListaEscudos];
    }


    public void DecrementaEscudoActual()
    {
        if(escudoEquipado == m_EscudoPorDefecto) return;
        if(escudoEquipado.numUnidades > 0)
        {
            escudoEquipado.numUnidades--;
            PersistenciaManager.instance.SaveEscudos();
        }
    }

    public void ComprobarEscudosConsumidos()
    {
        if(escudoEquipado.numUnidades <= 0)
        {
            escudoEquipado = m_EscudoPorDefecto;
        }
    }


    /// <summary>
    /// Devuelve el escudo que se desbloquea en la fase "_numFase"
    /// NOTA: devuelve null si no se ha encontrado ningun escudo
    /// </summary>
    /// <param name="_numFaseSuperada"></param>
    /// <returns></returns>
    public Escudo GetEscudoDesbloqueableEnFase(int _numFaseSuperada) {
        ++_numFaseSuperada; // <= se suma 1 por como se definen las fases de desbloqueo de los escudos

        Debug.LogWarning(">>> Busco escudo desbloqueable en fase: " + _numFaseSuperada);

        if (m_listaEscudos != null) {
            for (int i = 0; i < m_listaEscudos.Count; ++i) {
                if (m_listaEscudos[i].bloqueado && m_listaEscudos[i].faseDesbloqueo == _numFaseSuperada)
                    return m_listaEscudos[i];
            }
        }

        return null;
    }


    /// <summary>
    /// Actualizar el estado de desbloqueo de los escudos en funcion del numero de fase actual
    /// </summary>
    /// <param name="_numFase"></param>
    public void ActualizarEstadoDesbloqueoEscudos(int _numFaseActual) {
        if (m_listaEscudos != null) {
            for (int i = 0; i < m_listaEscudos.Count; ++i) {
                if (m_listaEscudos[i].bloqueado && m_listaEscudos[i].faseDesbloqueo < _numFaseActual)
                    m_listaEscudos[i].bloqueado = false;
            }
        }
    }
}


public class Escudo {


    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Numero máximo de unidades que puede tener el usuario de cada escudo
    /// </summary>
    private const int LIMITE_UNIDADES = 50;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Identificador del escudo (es el que se usa para identificar el escudo contra los web services)
    /// </summary>
    public string id { get { return m_id; } }
    private string m_id;

    /// <summary>
    /// Nombre del escudo
    /// </summary>
    public string nombre { get { return m_nombre; } }
    private string m_nombre;

    /// <summary>
    /// Descripcion del escudo
    /// </summary>
    public string _descripcion { get { return m_descripcion; } }
    private string m_descripcion;

    /// <summary>
    /// Identificador de la textura asociada a este escudo
    /// </summary>
    public int idTextura { get { return m_idTextura; } }
    private int m_idTextura;

    /// <summary>
    /// dinero HARD con el que se desbloquea este escudo
    /// </summary>
    public int precioHard { get { return m_precioHard; } }
    private int m_precioHard;

    /// <summary>
    /// multiplicador de puntuacion que se obtiene por este escudo
    /// </summary>
    public float boost { get { return m_boost; } }
    private float m_boost;

    /// <summary>
    /// fase a partir de la cual se desbloquea el escudo
    /// </summary>
    public int faseDesbloqueo { get { return m_faseDesbloqueo; } }
    private int m_faseDesbloqueo;

    /// <summary>
    /// Unidades adquiridas por el usuario de este escudo
    /// </summary>
    public int numUnidades { get { return m_numUnidades; } set { m_numUnidades = value; } }
    private int m_numUnidades;

    /// <summary>
    /// Indica si el escudo esta desbloqueado o no
    /// </summary>
    public bool bloqueado { get { return m_bloqueado; } set { m_bloqueado = value; } }
    private bool m_bloqueado;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="_id">Identificador unico del escudo</param>
    /// <param name="_nombre">Nombre amigable del escudo</param>
    /// <param name="_boost"></param>
    /// <param name="_precioHard"></param>
    /// <param name="_faseDesbloqueo"></param>
    /// <param name="_descripcion"></param>
    /// <param name="_numUnidades"></param>
    /// <param name="_bloqueado"></param>
    public Escudo(string _id, string _nombre, int _idTextura, float _boost, int _precioHard, int _faseDesbloqueo, string _descripcion, int _numUnidades = 0, bool _bloqueado = true) {
        m_id = _id;
        m_nombre = _nombre;
        m_idTextura = _idTextura;
        m_precioHard = _precioHard;
        m_boost = _boost;
        m_faseDesbloqueo = _faseDesbloqueo;
        m_descripcion = _descripcion;
        m_numUnidades = _numUnidades;
        m_bloqueado = _bloqueado;
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


}


