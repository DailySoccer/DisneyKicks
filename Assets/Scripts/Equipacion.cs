using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para gestionar la informacion asociada a una equipacion
/// </summary>
public class Equipacion {

    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // estados en los que puede estar la equipacion
    public enum Estado { ADQUIRIDA, BLOQUEADA };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


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
