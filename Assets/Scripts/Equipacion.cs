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
    public enum Estado { ADQUIRIDA, DISPONIBLE, BLOQUEADA };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// id de textura (indica la posicion del modelo de este jugador dentro de los arrays "m_texturasLanzador" y "m_texturasPortero" del componente "Equipacion Manager" de "AnilloUnico")
    /// </summary>
    public int idTextura { get { return m_idTextura; } set { m_idTextura = value; } }
    private int m_idTextura;

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
    /// dinero SOFT con el que se desbloquea esta equipacion
    /// </summary>
    public int precioSoft { get { return m_precioSoft; } set { m_precioSoft = value; } }
    private int m_precioSoft;

    /// <summary>
    /// dinero HARD con el que se desbloquea esta equipacion
    /// </summary>
    public int precioHard { get { return m_precioHard; } set { m_precioHard = value; } }
    private int m_precioHard;

    /// <summary>
    /// fase en la que, al conseguir los 4 objetivos, se desbloquea el jugador
    /// </summary>
    public int faseDesbloqueo { get { return m_faseDesbloqueo; } set { m_faseDesbloqueo = value; } }
    private int m_faseDesbloqueo;

    /// <summary>
    /// dinero HARD que vale el jugador si se quiere comprar antes de haber sido desbloqueado
    /// </summary>
    public int precioEarlyBuy { get { return m_precioEarlyBuy; } set { m_precioEarlyBuy = value; } }
    private int m_precioEarlyBuy;

    /// <summary>
    /// color del dorsal que corresponde a esta equipacion
    /// </summary>
    public Color colorDorsal { get { return m_colorDorsal; } set { m_colorDorsal = value; } }
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
    public Equipacion() {
    }

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
