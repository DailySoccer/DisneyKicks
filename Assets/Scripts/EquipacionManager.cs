using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Clase para gestionar las equipaciones que llevan los jugadores
/// </summary>
public class EquipacionManager: MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // instancia de esta clase
    public static EquipacionManager instance { get { return m_instance; } }
    private static EquipacionManager m_instance;

    // texturas con las equipaciones de los jugadores
    // NOTA: asignar valor a estas propiedades desde la interfaz de Unity
    public Texture[] m_texturasLanzador;
    public Texture[] m_texturasPortero;

    // informacion de las equipaciones de los jugadores
    private List<Equipacion> m_equipacionesLanzador;
    private List<Equipacion> m_equipacionesPortero;

    /// <summary>
    /// id de la equipacion de lanzador seleccionada
    /// </summary>
    public int idEquipacionLanzadorSeleccionada
    {
        get { return (ifcBase.activeIface == ifcVestuario.instance) ? m_idEquipacionLanzadorVisible : m_idEquipacionLanzadorEquipada; }
        set
        {
            m_idEquipacionLanzadorVisible = value;
            if(m_equipacionesLanzador[value].estado == Equipacion.Estado.ADQUIRIDA)
            {
                m_idEquipacionLanzadorEquipada = value;
            }
        }
    }

    /// <summary>
    /// id de la equipacion de portero seleccionada
    /// </summary>
    public int idEquipacionPorteroSeleccionada
    {
        get { return (ifcBase.activeIface == ifcVestuario.instance) ? m_idEquipacionPorteroVisible : m_idEquipacionPorteroEquipada; }
        set
        {
            m_idEquipacionPorteroVisible = value;
            if(m_equipacionesPortero[value].estado == Equipacion.Estado.ADQUIRIDA)
            {
                m_idEquipacionPorteroEquipada = value;
            }
        }
    }

    /// <summary>
    /// id de la equipacion de lanzador visible
    /// </summary>
    private int m_idEquipacionLanzadorVisible;

    /// <summary>
    /// id de la equipacion de lanzador equipada
    /// </summary>
    private int m_idEquipacionLanzadorEquipada;
    
    /// <summary>
    /// id de la equipacion de portero visible
    /// </summary>
    private int m_idEquipacionPorteroVisible;

    /// <summary>
    /// id de la equipacion de portero equipada
    /// </summary>
    private int m_idEquipacionPorteroEquipada;

    /// <summary>
    /// numero de equipaciones de lanzador disponibles
    /// </summary>
    public int numEquipacionesLanzador { get { return (m_equipacionesLanzador == null) ? 0 : m_equipacionesLanzador.Count; } }

    /// <summary>
    /// numero de equipaciones de lanzador disponibles
    /// </summary>
    public int numEquipacionesPortero { get { return (m_equipacionesPortero == null) ? 0 : m_equipacionesPortero.Count; } }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    void Awake() {
        if(m_instance == null)
        {
            m_instance = this;

            // obtener la equipacion de lanzador de las preferencias
            if (PlayerPrefs.HasKey("throwerEquipationInd"))
                m_idEquipacionLanzadorEquipada = PlayerPrefs.GetInt("throwerEquipationInd");
            else
                m_idEquipacionLanzadorEquipada = 3; // <= equipacion por defecto
    
            // obtener la equipacion de portero de las preferencias
            if (PlayerPrefs.HasKey("goalKeeperEquipationInd"))
                m_idEquipacionPorteroEquipada = PlayerPrefs.GetInt("goalKeeperEquipationInd");
            else
                m_idEquipacionPorteroEquipada = 0; // <= equipacion por defecto
    
            // inicializar la informacion de las equipaciones
            m_equipacionesLanzador = new List<Equipacion>();
            m_equipacionesPortero = new List<Equipacion>();
    
            // F4KE: crear la informacion de las equipaciones de LANZADOR
            m_equipacionesLanzador.Add(new Equipacion(14, "IT_EQU_ST_0015", 0, 0, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 0, 0, Equipacion.Estado.ADQUIRIDA));
            m_equipacionesLanzador.Add(new Equipacion(17, "IT_EQU_ST_0018", 0, 0, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 0, 0, Equipacion.Estado.ADQUIRIDA));
            m_equipacionesLanzador.Add(new Equipacion(20, "IT_EQU_ST_0021", 0, 0, new Color(169.0f / 255.0f, 29.0f / 255.0f, 31.0f / 255.0f, 1.0f), 0, 0, Equipacion.Estado.ADQUIRIDA));
            m_equipacionesLanzador.Add(new Equipacion(30, "IT_EQU_ST_0031", 4200, 120, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 10, 195));
            m_equipacionesLanzador.Add(new Equipacion(26, "IT_EQU_ST_0027", 3600, 100, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 11, 180));
            m_equipacionesLanzador.Add(new Equipacion(28, "IT_EQU_ST_0029", 5600, 180, new Color(64.0f / 255.0f, 83.0f / 255.0f, 18.0f / 255.0f, 1.0f), 17, 325));
            m_equipacionesLanzador.Add(new Equipacion(25, "IT_EQU_ST_0026", 6300, 180, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 22, 310));
            m_equipacionesLanzador.Add(new Equipacion(23, "IT_EQU_ST_0024", 8000, 190, new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 1.0f), 26, 300));
            m_equipacionesLanzador.Add(new Equipacion(21, "IT_EQU_ST_0022", 8400, 200, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 31, 400));
            m_equipacionesLanzador.Add(new Equipacion(19, "IT_EQU_ST_0020", 9280, 200, new Color(80.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 1.0f), 32, 250));
            m_equipacionesLanzador.Add(new Equipacion(32, "IT_EQU_ST_0033", 10400, 210, new Color(160.0f / 255.0f, 122.0f / 255.0f, 3.0f / 255.0f, 1.0f), 34, 360));
            m_equipacionesLanzador.Add(new Equipacion(34, "IT_EQU_ST_0035", 12750, 210, new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 1.0f), 40, 350));
            m_equipacionesLanzador.Add(new Equipacion(29, "IT_EQU_ST_0030", 14790, 250, new Color(64.0f / 255.0f, 83.0f / 255.0f, 18.0f / 255.0f, 1.0f), 43, 425));
            m_equipacionesLanzador.Add(new Equipacion(27, "IT_EQU_ST_0028", 18000, 200, new Color(8.0f / 255.0f, 105.0f / 255.0f, 169.0f / 255.0f, 1.0f), 52, 400));
            m_equipacionesLanzador.Add(new Equipacion(22, "IT_EQU_ST_0023", 16560, 250, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 60, 600));
    
            // F4KE: crear la informacion de las equipaciones de PORTERO
            m_equipacionesPortero.Add(new Equipacion(0, "IT_EQU_GK_0001", 0, 0, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 0, 0, Equipacion.Estado.ADQUIRIDA));
            m_equipacionesPortero.Add(new Equipacion(1, "IT_EQU_GK_0002", 0, 0, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 0, 0, Equipacion.Estado.ADQUIRIDA));
            m_equipacionesPortero.Add(new Equipacion(2, "IT_EQU_GK_0003", 2500, 75, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 2, 100));
            m_equipacionesPortero.Add(new Equipacion(3, "IT_EQU_GK_0004", 7700, 175, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 25, 275));
            m_equipacionesPortero.Add(new Equipacion(5, "IT_EQU_GK_0006", 15750, 275, new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 50, 375));
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate (gameObject);
        }
    }

    public void ResetEquipacion(bool _portero)
    {
        if(_portero)
        {
            m_idEquipacionPorteroVisible = m_idEquipacionPorteroEquipada;
        }
        else
        {
            m_idEquipacionLanzadorVisible = m_idEquipacionLanzadorEquipada;
        }
    }


    /// <summary>
    /// Comprueba si existe una determinada equipacion por su nombre de asset
    /// </summary>
    /// <param name="_assetName"></param>
    /// <returns></returns>
    public Equipacion GetEquipacion(string _assetName) {
        // buscar en la lista de tiradores
        for (int i = 0; i < m_equipacionesLanzador.Count; ++i) {
            if (m_equipacionesLanzador[i].assetName == _assetName)
                return m_equipacionesLanzador[i];
        }

        // buscar en la lista de porteros
        for (int i = 0; i < m_equipacionesPortero.Count; ++i) {
            if (m_equipacionesPortero[i].assetName == _assetName)
                return m_equipacionesPortero[i];
        }

        return null;
    }


    /// <summary>
    /// Modifica la equipacion del MODELO lanzador de la interfaz
    /// </summary>
    /// <param name="_offsetEquipacion"></param>
    /// <returns>Devuelve la informacion de la equipacion seleccionada</returns>
    public Equipacion CambiarEquipacionLanzador(int _offsetEquipacion = 0) {
        if (m_equipacionesLanzador == null || m_equipacionesLanzador.Count == 0) {
            Debug.LogWarning("Atención. No se han definido equipaciones de lanzador");
            return null;
        }

        // calcular el numero de la nueva equipacion
        int nuevoNumEquipacion = (idEquipacionLanzadorSeleccionada + _offsetEquipacion) % m_equipacionesLanzador.Count;
        if (nuevoNumEquipacion < 0)
            nuevoNumEquipacion += m_equipacionesLanzador.Count;

        idEquipacionLanzadorSeleccionada = nuevoNumEquipacion;

        // actualizar la textura del jugador
        if (Interfaz.instance.throwerModel) {
            int idTextura = m_equipacionesLanzador[idEquipacionLanzadorSeleccionada].idTextura;
            GameObject bodyModeloJugador = Interfaz.instance.throwerModel.transform.FindChild("Body").gameObject;
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasLanzador[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;

            // colorear el dorsal del jugador en funcion de la equipacion que lleve
            Color colorDorsal = Color.white;    // <= por defecto blanco
            Equipacion equipacion = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada();
            if (equipacion != null)
                colorDorsal = equipacion.colorDorsal;
            Numbers numbersComponent = bodyModeloJugador.GetComponent<Numbers>();
            if (numbersComponent != null)
                numbersComponent.color = colorDorsal;
        }

        PlayerPrefs.SetInt("throwerEquipationInd", m_idEquipacionLanzadorEquipada);
        return m_equipacionesLanzador[idEquipacionLanzadorSeleccionada];
    }


    /// <summary>
    /// Modifica la equipacion del MODELO portero de la interfaz
    /// </summary>
    /// <param name="_offsetEquipacion"></param>
    /// <returns>Devuelve la informacion de la equipacion seleccionada</returns>
    public Equipacion CambiarEquipacionPortero(int _offsetEquipacion = 0) {
        if (m_equipacionesPortero == null || m_equipacionesPortero.Count == 0) {
            Debug.LogWarning("Atención. No se han definido equipaciones de portero");
            return null;
        }

        // calcular el numero de la nueva equipacion
        int nuevoNumEquipacion = (idEquipacionPorteroSeleccionada + _offsetEquipacion) % m_equipacionesPortero.Count;
        if (nuevoNumEquipacion < 0)
            nuevoNumEquipacion += m_equipacionesPortero.Count;

        idEquipacionPorteroSeleccionada = nuevoNumEquipacion;

        // actualizar la textura del jugador
        if (Interfaz.instance.goalkeeperModel != null) {
            int idTextura = m_equipacionesPortero[idEquipacionPorteroSeleccionada].idTextura;

            GameObject bodyModeloJugador = Interfaz.instance.goalkeeperModel.transform.FindChild("Body").gameObject;
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasPortero[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;


            // colorear el dorsal del jugador en funcion de la equipacion que lleve
            Color colorDorsal = Color.white;    // <= por defecto blanco
            Equipacion equipacion = EquipacionManager.instance.GetEquipacionPorteroSeleccionada();
            if (equipacion != null)
                colorDorsal = equipacion.colorDorsal;
            Numbers numbersComponent = bodyModeloJugador.GetComponent<Numbers>();
            if (numbersComponent != null)
                numbersComponent.color = colorDorsal;
        }
        
        PlayerPrefs.SetInt("goalKeeperEquipationInd", m_idEquipacionPorteroEquipada);
        return m_equipacionesPortero[idEquipacionPorteroSeleccionada];
    }


    /// <summary>
    /// devuelve la informacion de la equipacion de lanzador seleccionada
    /// </summary>
    /// <returns></returns>
    public Equipacion GetEquipacionLanzadorSeleccionada() {
        if (m_equipacionesLanzador == null)
            return null;
        else
            return m_equipacionesLanzador[idEquipacionLanzadorSeleccionada];
    }


    /// <summary>
    /// devuelve la informacion de la equipacion de lanzador seleccionada
    /// </summary>
    /// <returns></returns>
    public Equipacion GetEquipacionPorteroSeleccionada() {
        if (m_equipacionesPortero == null)
            return null;
        else
            return m_equipacionesPortero[idEquipacionPorteroSeleccionada];
    }

    public void PintarEquipacionesIngame(bool _portero, GameObject _modelo)
    {
        int idTextura =0;
        if(!GameplayService.networked)
        {
            if(_portero)
            {
                idTextura = m_equipacionesPortero[idEquipacionPorteroSeleccionada].idTextura;
            }
            else
            {
                idTextura = m_equipacionesLanzador[idEquipacionLanzadorSeleccionada].idTextura;
            }
        }
        else
        {
            if(_portero)
            {
                idTextura = GameplayService.IsGoalkeeper() ? m_equipacionesPortero[idEquipacionPorteroSeleccionada].idTextura : ifcDuelo.m_rival.equipacionGoalkeeper.idTextura;
            }
            else
            {
                
                idTextura = !GameplayService.IsGoalkeeper() ? m_equipacionesLanzador[idEquipacionLanzadorSeleccionada].idTextura : ifcDuelo.m_rival.equipacionShooter.idTextura;
            }
        }

        if(_portero)
        {
            GameObject bodyModeloJugador = _modelo.transform.FindChild("Body").gameObject;
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasPortero[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;

            // colorear el dorsal del jugador en funcion de la equipacion que lleve
            Color colorDorsal = Color.white;    // <= por defecto blanco
            Equipacion equipacion = EquipacionManager.instance.GetEquipacionPorteroSeleccionada();
            if (equipacion != null)
                colorDorsal = equipacion.colorDorsal;
            Numbers numbersComponent = bodyModeloJugador.GetComponent<Numbers>();
            if (numbersComponent != null) {
                numbersComponent.color = colorDorsal;
                numbersComponent.number = FieldControl.localGoalkeeper.numDorsal;
            }
        }
        else
        {
            GameObject bodyModeloJugador = _modelo.transform.FindChild("Body").gameObject;
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasLanzador[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;

            // colorear el dorsal del jugador en funcion de la equipacion que lleve
            Color colorDorsal = Color.white;    // <= por defecto blanco
            Equipacion equipacion = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada();
            if (equipacion != null)
                colorDorsal = equipacion.colorDorsal;
            Numbers numbersComponent = bodyModeloJugador.GetComponent<Numbers>();
            if (numbersComponent != null)
                numbersComponent.color = colorDorsal;
            numbersComponent.number = FieldControl.localThrower.numDorsal;
        }
    }


    /// <summary>
    /// Devuelve la proxima equipacion de LANZADOR (a partir de la posicion "_posicion") que este en estado de ADQUIRIDA
    /// </summary>
    /// <returns></returns>
    public void CambiarASiguienteEquipacionLanzadorAdquirida() {
        // buscar desde la "_posicion" hasta el final
        for (int i = idEquipacionLanzadorSeleccionada; i < m_equipacionesLanzador.Count; ++i) {
            if (m_equipacionesLanzador[i].estado == Equipacion.Estado.ADQUIRIDA) {
                idEquipacionLanzadorSeleccionada = i;
                return;
            }
        }

        // buscar desde el principio hasta la "_posicion"
        for (int i = 0; i < idEquipacionLanzadorSeleccionada; ++i) {
            if (m_equipacionesLanzador[i].estado == Equipacion.Estado.ADQUIRIDA) {
                idEquipacionLanzadorSeleccionada = i;
                return;
            }
        }
    }


    /// <summary>
    /// Devuelve la proxima equipacion de PORTERO (a partir de la posicion "_posicion") que este en estado de ADQUIRIDA
    /// </summary>
    /// <returns></returns>
    public void CambiarASiguienteEquipacionPorteroAdquirida() {
        // buscar desde la "_posicion" hasta el final
        for (int i = idEquipacionPorteroSeleccionada; i < m_equipacionesPortero.Count; ++i) {
            if (m_equipacionesPortero[i].estado == Equipacion.Estado.ADQUIRIDA) {
                idEquipacionPorteroSeleccionada = i;
                return;
            }
        }

        // buscar desde el principio hasta la "_posicion"
        for (int i = 0; i < idEquipacionPorteroSeleccionada; ++i) {
            if (m_equipacionesPortero[i].estado == Equipacion.Estado.ADQUIRIDA) {
                idEquipacionPorteroSeleccionada = i;
                return;
            }
        }
    }


    /// <summary>
    /// Devuelve la equipacion de lanzador que ocupa la posicion "_posicion"
    /// </summary>
    /// <param name="_posicion"></param>
    /// <returns></returns>
    public Equipacion GetEquipacionLanzador(int _posicion) {
        if (m_equipacionesLanzador != null && _posicion >= 0 && _posicion < m_equipacionesLanzador.Count)
            return m_equipacionesLanzador[_posicion];
        else
            return null; // <= valor por defecto
    }


    /// <summary>
    /// Devuelve la equipacion de portero que ocupa la posicion "_posicion"
    /// </summary>
    /// <param name="_posicion"></param>
    /// <returns></returns>
    public Equipacion GetEquipacionPortero(int _posicion) {
        if (m_equipacionesPortero != null && _posicion >= 0 && _posicion < m_equipacionesPortero.Count)
            return m_equipacionesPortero[_posicion];
        else
            return null; // <= valor por defecto
    }


    /// <summary>
    /// Metodo que en funcion de la "_ultimaFaseDesbloqueada" actualiza el estado de las equipaciones para que pasen de "BLOQUEADA" a "DISPONIBLE" si procede
    /// </summary>
    /// <param name="_ultimaFaseDesbloqueada"></param>
    public void RefreshEquipacionesDesbloqueadas(int _ultimaFaseDesbloqueada) {
        // comprobar las equipaciones de lanzador
        if (m_equipacionesLanzador != null) {
            foreach (Equipacion equipacion in m_equipacionesLanzador) {
                if (equipacion.estado == Equipacion.Estado.BLOQUEADA && equipacion.faseDesbloqueo <= _ultimaFaseDesbloqueada)
                    equipacion.estado = Equipacion.Estado.DISPONIBLE;
            }
        }

        // comprobar las equipaciones de portero
        if (m_equipacionesPortero != null) {
            foreach (Equipacion equipacion in m_equipacionesPortero) {
                if (equipacion.estado == Equipacion.Estado.BLOQUEADA && equipacion.faseDesbloqueo <= _ultimaFaseDesbloqueada)
                    equipacion.estado = Equipacion.Estado.DISPONIBLE;
            }
        }
    }


    /// <summary>
    /// Devuelve la equipacion que se desbloquea en la fase "_numFase"
    /// NOTA: devuelve null si no se ha encontrado ninguna equipacion
    /// </summary>
    /// <param name="_numFaseSuperada"></param>
    /// <returns></returns>
    public Equipacion GetEquipacionDesbloqueableEnFase(int _numFaseSuperada) {
        ++_numFaseSuperada; // <= se suma 1 por como se definen las fases de desbloqueo de las equipaciones

        // buscar en las equipaciones de lanzador
        if (m_equipacionesLanzador != null) {
            for (int i = 0; i < m_equipacionesLanzador.Count; ++i) {
                if (m_equipacionesLanzador[i].estado == Equipacion.Estado.BLOQUEADA && m_equipacionesLanzador[i].faseDesbloqueo == _numFaseSuperada)
                    return m_equipacionesLanzador[i];
            }
        }

        // buscar en las equipaciones de portero
        if (m_equipacionesPortero != null) {
            for (int i = 0; i < m_equipacionesPortero.Count; ++i) {
                if (m_equipacionesPortero[i].estado == Equipacion.Estado.BLOQUEADA && m_equipacionesPortero[i].faseDesbloqueo == _numFaseSuperada)
                    return m_equipacionesPortero[i];
            }
        }

        return null;
    }


    /// <summary>
    /// Comprueba si existe una equipacion de lanzador con un determinado "_assetName"
    /// </summary>
    /// <param name="_assetName"></param>
    /// <returns></returns>
    public bool EsEquipacionDeLanzador(string _assetName) {
        if (m_equipacionesLanzador != null) {
            for (int i = 0; i < m_equipacionesLanzador.Count; ++i) {
                if (m_equipacionesLanzador[i].assetName == _assetName)
                    return true;
            }
        }

        return false;
    }
}


/// <summary>
/// Clase para gestionar la informacion asociada a una equipacion
/// </summary>
public class Equipacion {


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // estados en los que puede estar la equipacion
    public enum Estado { ADQUIRIDA, DISPONIBLE, BLOQUEADA };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// id de textura (indica la posicion del modelo de este jugador dentro de los arrays "m_texturasLanzador" y "m_texturasPortero" del componente "Equipacion Manager" de "AnilloUnico")
    /// </summary>
    public int idTextura { get { return m_idTextura; } }
    private int m_idTextura;

    /// <summary>
    /// Estado en el que se encuentra esta equipacion
    /// </summary>
    public Estado estado { get { return m_estado; } set { m_estado = value; } }
    private Estado m_estado;

    /// <summary>
    /// nombre del asset del jugador (para su identificacion contra los servicios web o con arte)
    /// </summary>
    public string assetName { get { return m_assetName; } }
    private string m_assetName;

    /// <summary>
    /// dinero SOFT con el que se desbloquea esta equipacion
    /// </summary>
    public int precioSoft { get { return m_precioSoft; } }
    private int m_precioSoft;

    /// <summary>
    /// dinero HARD con el que se desbloquea esta equipacion
    /// </summary>
    public int precioHard { get { return m_precioHard; } }
    private int m_precioHard;

    /// <summary>
    /// fase en la que, al conseguir los 4 objetivos, se desbloquea el jugador
    /// </summary>
    public int faseDesbloqueo { get { return m_faseDesbloqueo; } }
    private int m_faseDesbloqueo;

    /// <summary>
    /// dinero HARD que vale el jugador si se quiere comprar antes de haber sido desbloqueado
    /// </summary>
    public int precioEarlyBuy { get { return m_precioEarlyBuy; } }
    private int m_precioEarlyBuy;

    /// <summary>
    /// color del dorsal que corresponde a esta equipacion
    /// </summary>
    public Color colorDorsal { get { return m_colorDorsal; } }
    private Color m_colorDorsal;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="_idTextura">id de textura (indica la posicion del modelo de este jugador dentro de los arrays "m_texturasLanzador" y "m_texturasPortero" del componente "Equipacion Manager" de "AnilloUnico")</param>
    /// <param name="_assetName">nombre del asset del jugador (para su identificacion contra los servicios web o con arte)</param>
    /// <param name="_precioSoft"></param>
    /// <param name="_precioHard"></param>
    /// <param name="_colorDorsal">color para teñir el numero de esta camiseta</param>
    /// <param name="_faseDesbloqueo"></param>
    /// <param name="_precioEarlyBuy">Fase en la que, al conseguir los 4 objetivos, se desbloquea el jugador</param>
    /// <param name="_estado"></param>
    public Equipacion(int _idTextura, string _assetName, int _precioSoft, int _precioHard, Color _colorDorsal, int _faseDesbloqueo = 0, int _precioEarlyBuy = 0, Estado _estado = Estado.BLOQUEADA) {
        m_idTextura = _idTextura;
        m_assetName = _assetName;
        m_precioSoft = _precioSoft;
        m_precioHard = _precioHard;
        m_colorDorsal = _colorDorsal;
        m_faseDesbloqueo = _faseDesbloqueo;
        m_precioEarlyBuy = _precioEarlyBuy;
        m_estado = _estado;
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


}
