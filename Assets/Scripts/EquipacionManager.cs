using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Clase para gestionar las equipaciones que llevan los jugadores
/// </summary>
public class EquipacionManager: MonoBehaviour {
    const string KEY_PORTEROS = "porteros";
    const string KEY_TIRADORES = "tiradores";

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
    public Material[] m_materialsLanzador;
    public Material[] m_materialsPortero;

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


    public string SaveData {
        get {
            Dictionary<string, List<Dictionary<string, object>>> data = new Dictionary<string, List<Dictionary<string, object>>>();
            data.Add(KEY_PORTEROS, SaveDataFromList(m_equipacionesPortero));
            data.Add(KEY_TIRADORES, SaveDataFromList(m_equipacionesLanzador));
            return MiniJSON.Json.Serialize(data);
        }

        set {
            Dictionary<string, object> data = (Dictionary<string, object>) MiniJSON.Json.Deserialize(value);
            SaveDataToList(data[KEY_PORTEROS] as List<object>, m_equipacionesPortero);
            SaveDataToList(data[KEY_TIRADORES] as List<object>, m_equipacionesLanzador);
        }
    }

    private List<Dictionary<string, object>> SaveDataFromList(List<Equipacion> lista) {
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
        foreach(Equipacion equipacion in lista) {
            result.Add( equipacion.SaveData );
        }
        return result;
    }

    private void SaveDataToList(List<object> data, List<Equipacion> lista) {
        foreach(object equipacionSaved in data) {
            Dictionary<string, object> saveData = equipacionSaved as Dictionary<string, object>;
            Equipacion equipacion = lista.Find( el => el.ID == saveData[Equipacion.KEY_ID].ToString() );
            if (equipacion != null) {
                equipacion.SaveData = saveData;
            }
            else {
                Debug.LogError("SaveDataToList: Not Exists " + saveData["id"]);
            }
        }
    }

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
    
            // F4KE: crear la informacion de las equipaciones de PORTERO
            m_equipacionesPortero = new List<Equipacion>();
            m_equipacionesPortero.AddRange( EquipacionData.Porteros );

            // F4KE: crear la informacion de las equipaciones de LANZADOR
            m_equipacionesLanzador = new List<Equipacion>();
            m_equipacionesLanzador.AddRange( EquipacionData.Tiradores );

            DontDestroyOnLoad(gameObject);

            // TEST SaveData --
            /*
            string dataBefore = SaveData;
            Debug.Log("SaveData BEFORE: " + dataBefore);
            SaveData = dataBefore;
            Debug.Log("SaveData SAVED: " + SaveData);
            string dataRestored = SaveData;
            Debug.Log("SaveData RESTORED: " + dataRestored);
            Debug.Assert(dataBefore == dataRestored, "SaveData ERROR");
            */
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

            Material[] mats = bodyModeloJugador.GetComponent<Renderer>().materials;
            mats[0] = m_materialsLanzador[idTextura]; 
            bodyModeloJugador.GetComponent<Renderer>().materials = mats;
            /*
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasLanzador[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;
            */

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

            Material[] mats = bodyModeloJugador.GetComponent<Renderer>().materials;
            mats[0] = m_materialsPortero[idTextura]; 
            bodyModeloJugador.GetComponent<Renderer>().materials = mats;
            /*
            bodyModeloJugador.GetComponent<Renderer>().materials[0] = m_materialsPortero[idTextura];
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasPortero[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;
            */

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
            Material[] mats = bodyModeloJugador.GetComponent<Renderer>().materials;
            mats[0] = m_materialsPortero[idTextura]; 
            bodyModeloJugador.GetComponent<Renderer>().materials = mats;
            /*
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasPortero[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;
            */

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
            Material[] mats = bodyModeloJugador.GetComponent<Renderer>().materials;
            mats[0] = m_materialsLanzador[idTextura]; 
            bodyModeloJugador.GetComponent<Renderer>().materials = mats;
            /*
            bodyModeloJugador.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", m_texturasLanzador[idTextura]);
            bodyModeloJugador.GetComponent<Renderer>().materials[0].color = Color.grey;
            */

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

    public int GetPositionLanzador(string key) {
        int position = -1;
        if (m_equipacionesLanzador != null) {
            for (int i=0; i<m_texturasLanzador.Length; i++) {
                if (m_texturasLanzador[i] != null && m_texturasLanzador[i].name.Contains(key)) {
                    position = i;
                    break;
                }
            }
        }
        return position;
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

    public int GetPositionPortero(string key) {
        int position = -1;
        if (m_equipacionesPortero != null) {
            for (int i=0; i<m_texturasPortero.Length; i++) {
                if (m_texturasPortero[i] != null && m_texturasPortero[i].name.Contains(key)) {
                    position = i;
                    break;
                }
            }
        }
        return position;
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

    public void CHEAT_ChangeAllToState(Equipacion.Estado estado) {
        for (int i = 1; i < m_equipacionesPortero.Count; ++i) {
            m_equipacionesPortero[i].estado = estado;
        }

        for (int i = 1; i < m_equipacionesLanzador.Count; ++i) {
            m_equipacionesLanzador[i].estado = estado;
        }
    }
}
