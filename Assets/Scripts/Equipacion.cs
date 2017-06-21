using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para gestionar la informacion asociada a una equipacion
/// </summary>
public class Equipacion {

    public const string KEY_ID = "id";

    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // estados en los que puede estar la equipacion
    public enum Estado { ADQUIRIDA, BLOQUEADA };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    public string ID {
        get { return assetName; }
    }

    /// <summary>
    /// id de textura (indica la posicion del modelo de este jugador dentro de los arrays "m_texturasLanzador" y "m_texturasPortero" del componente "Equipacion Manager" de "AnilloUnico")
    /// </summary>
    public int idTextura { 
        get { 
            if (m_idTextura == -1) {
                m_idTextura = EquipacionManager.instance.GetPositionPortero(assetName);
                if (m_idTextura == -1) {
                    m_idTextura = EquipacionManager.instance.GetPositionLanzador(assetName);
                }
            }
            return m_idTextura; 
        } 
    }
    private int m_idTextura = -1;

    /// <summary>
    /// Estado en el que se encuentra esta equipacion
    /// </summary>
    public Estado estado { get { return m_estado; } set { m_estado = value; } }
    private Estado m_estado;

    /// <summary>
    /// nombre del asset del jugador (para su identificacion contra los servicios web o con arte)
    /// </summary>
    public string assetName { get { return m_assetName; } set { m_assetName = value; } }
    private string m_assetName;

    /// <summary>
    /// color del dorsal que corresponde a esta equipacion
    /// </summary>
    public Color colorDorsal { get { return m_colorDorsal; } set { m_colorDorsal = value; } }
    private Color m_colorDorsal;

    /// <summary>
    /// Calidad de la equipación (común, raro, épico)
    /// </summary>
    public CardQuality quality { get { return m_quality; } set { m_quality = value; } }
    private CardQuality m_quality;

    /// <summary>
    /// Liga en la que es posible recibir como recompensa la equipación
    /// </summary>
    public int liga { get { return m_liga; } set { m_liga = value; } }
    private int m_liga;

    /// <summary>
    /// Una equipación estará disponible si el player está en una liga igual o superior
    /// </summary>
    public bool isDisponible(int playerLiga) {
        return m_liga <= playerLiga;
    }

    /// <summary>
    /// Adquirir una equipación
    /// </summary>
    public void Adquirir() {
        m_estado = Estado.ADQUIRIDA; 
        PersistenciaManager.instance.SaveEquipaciones();
    }

    /// <summary>
    /// Información de un usuario para ser registrada (en PlayerPrefs) o recuperada (de PlayerPrefs)
    /// </summary>
    public Dictionary<string, object> SaveData {
        get {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(KEY_ID, ID);
            data.Add("estado", m_estado.ToString());
            return data;
        }

        set {
            Debug.Assert(value[KEY_ID].ToString() == assetName, string.Format("SaveData: {0} != {1}", value[KEY_ID].ToString(), assetName));
            estado = value.ContainsKey("estado") ? (Estado) Enum.Parse(typeof(Estado), value["estado"].ToString()) : 0;
        }
    }

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
    public Equipacion() {
        estado = Estado.BLOQUEADA;
    }

    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------
}
