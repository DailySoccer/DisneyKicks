using System;
using UnityEngine;

public struct PowerupUsage
{
    public int Id { get; set; } //id del powerup en portero o lanzador
    public int AbsId { get; set; } //id absoluto del powerup
    public Powerup Value { get; set; } //anum del powerup
    public bool Own { get; set; } //es un powerup propio o del otro jugador?
    public GameMode Mode { get; set; } //es un powerup del protero o del lanzador?
}

// NOTA: si se añade algun power up:
// 1) Actualizar las funciones "PowerupToIdWs" y "IdWsToPowerup" de la clase "PowerupInventory" para que se adapten a estos nuevos valores
// 2) Actualizar la funcion "InicializarDescriptores" de "PowerupInventory"
public enum Powerup {
    // powerups de lanzador
    Concentracion = 0,
    Destello = 1,
    Sharpshooter = 2,
    Resbaladiza = 3,
    Phase = 4,
    // powerups de portero
    Manoplas = 5,
    Reflejo = 6,
    Intuicion = 7,
    TiempoBala = 8
}


/// <summary>
/// Descriptor con la informacion asociada a cada tipo de power up
/// </summary>
public class PowerUpDescriptor {


    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Numero máximo de unidades que puede tener el usuario de cada escudo
    /// </summary>
    public const int LIMITE_UNIDADES = 99;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Identificador utilizado para identificar este item contra los web services
    /// Nota: existen los metodos "PowerupToIdWs" y "IdWsToPowerup" de la clase "PowerupInventory" para realizar la conversion de este id con las posiciones en las listas locales
    /// </summary>
    public string idWs { get { return m_idWs; } }
    private string m_idWs;

    public string nombre { get { return m_nombre; } }
    private string m_nombre;

    public string _descripcion { get { return m_descripcion; } }
    private string m_descripcion;

    public int precioSoft { get { return m_precioSoft; } }
    private int m_precioSoft;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    /// <summary>
    /// Identificador utilizado para identificar este item contra los web services
    /// </summary>
    /// <param name="_idWs"></param>
    /// <param name="_nombre"></param>
    /// <param name="_descripcion"></param>
    /// <param name="_preciosoft"></param>
    public PowerUpDescriptor(string _idWs, string _nombre, string _descripcion, int _preciosoft) {
        m_idWs = _idWs;
        m_nombre = _nombre;
        m_descripcion = _descripcion;
        m_precioSoft = _preciosoft;
    }


}



public class PowerupInventory
{

    public static PowerUpDescriptor[] descriptoresLanzador {
        get {
            if (m_descriptoresLanzador == null)
                InicializarDescriptores();
            return m_descriptoresLanzador;
        } }
    private static PowerUpDescriptor[] m_descriptoresLanzador;

    public static PowerUpDescriptor[] descriptoresPortero {
        get {
            if (m_descriptoresPortero == null)
                InicializarDescriptores();
            return m_descriptoresPortero;
        }
    }
    private static PowerUpDescriptor[] m_descriptoresPortero;



    int[] GoalkeeperPowerups;
    int[] ThrowerPowerups;

    bool fakedInfinite = false;

    public PowerupInventory(bool _faked = false)
    {
        GoalkeeperPowerups = new int[PowerupService.MAXPOWERUPSPORTERO];
        ThrowerPowerups = new int[PowerupService.MAXPOWERUPSTIRADOR];
        if(_faked) fakedInfinite = true;
    }

    public PowerupInventory(int[] _throwerInventory, int[] _goalkeeperInventory)
    {
        GoalkeeperPowerups = _goalkeeperInventory;
        ThrowerPowerups = _throwerInventory;
    }

    public bool UsePowerup(int _id, GameMode _mode)
    {
        if(fakedInfinite) return true;
        int[] inventory = (_mode == GameMode.Shooter) ? ThrowerPowerups : GoalkeeperPowerups;
        if(inventory[_id] <= 0)
        {
            return false;
        }
        else
        {
            // indicar a los webservices que he consumido el powerup
            // Interfaz.instance.consumirItem(PowerupToIdWs((Powerup)_id));

            // decrementar la cantidad de power up
            inventory[_id]--;
            return true;
        }
    }


    /// <summary>
    /// Devuelve el id (utilizado por los webservices) asociado a este tipo de powerup
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public static string PowerupToIdWs(Powerup _powerUp) {
        switch(_powerUp) {
            case Powerup.Concentracion: 
                return "IT_POWUP_ST_01";
            case Powerup.Destello: 
                return "IT_POWUP_ST_02";
            case Powerup.Sharpshooter: 
                return "IT_POWUP_ST_03";
            case Powerup.Resbaladiza:
                return "IT_POWUP_ST_04";
            case Powerup.Phase:
                return "IT_POWUP_ST_05";
            case Powerup.Manoplas:
                return "IT_POWUP_GK_01";
            case Powerup.Reflejo:
                return "IT_POWUP_GK_02";
            case Powerup.Intuicion:
                return "IT_POWUP_GK_03";
            case Powerup.TiempoBala:
                return "IT_POWUP_GK_04";
            default:
                Debug.LogWarning(">>> Atencion: PowerupToIdWs() ha recibido un valor inesperado: " + _powerUp);
                return "";
        }
    }


    /// <summary>
    /// Devuelve el tipo de powerup asociado al id (utilizado por los webservices) recibido como parametro
    /// </summary>
    /// <param name="_string"></param>
    /// <returns></returns>
    public static Powerup IdWsToPowerup(string _idWs) {
        switch (_idWs) {
            case "IT_POWUP_ST_01":
                return Powerup.Concentracion;
            case "IT_POWUP_ST_02":
                return Powerup.Destello;
            case "IT_POWUP_ST_03":
                return Powerup.Sharpshooter;
            case "IT_POWUP_ST_04":
                return Powerup.Resbaladiza;
            case "IT_POWUP_ST_05":
                return Powerup.Phase;
            case "IT_POWUP_GK_01":
                return Powerup.Manoplas;
            case "IT_POWUP_GK_02":
                return Powerup.Reflejo;
            case "IT_POWUP_GK_03":
                return Powerup.Intuicion;
            case "IT_POWUP_GK_04":
                return Powerup.TiempoBala;
            default:
                Debug.LogWarning(">>> Atencion: IdWsToPowerup() ha recibido un valor inesperado: "+ _idWs);
                return Powerup.Concentracion;
        }
    }

    
    /// <summary>
    /// Inicializa los descriptores de los power ups
    /// </summary>
    public static void InicializarDescriptores() {
        // F4KE: generar los descriptores de los powerups de LANZADOR
        if (m_descriptoresLanzador == null) {
            m_descriptoresLanzador = new PowerUpDescriptor[PowerupService.MAXPOWERUPSTIRADOR];

            m_descriptoresLanzador[(int) Powerup.Concentracion] = new PowerUpDescriptor("IT_POWUP_ST_01", LocalizacionManager.instance.GetTexto(119), LocalizacionManager.instance.GetTexto(128), 50);
            m_descriptoresLanzador[(int) Powerup.Destello] = new PowerUpDescriptor("IT_POWUP_ST_02", LocalizacionManager.instance.GetTexto(120), LocalizacionManager.instance.GetTexto(129), 50);
            m_descriptoresLanzador[(int) Powerup.Sharpshooter] = new PowerUpDescriptor("IT_POWUP_ST_03", LocalizacionManager.instance.GetTexto(121), LocalizacionManager.instance.GetTexto(130), 50);
            m_descriptoresLanzador[(int) Powerup.Resbaladiza] = new PowerUpDescriptor("IT_POWUP_ST_04", LocalizacionManager.instance.GetTexto(122), LocalizacionManager.instance.GetTexto(131), 50);
            m_descriptoresLanzador[(int) Powerup.Phase] = new PowerUpDescriptor("IT_POWUP_ST_05", LocalizacionManager.instance.GetTexto(123), LocalizacionManager.instance.GetTexto(132), 50);
        }

        // F4KE: generar los descriptores de los powerups de PORTERO
        if (m_descriptoresPortero == null) {
            m_descriptoresPortero = new PowerUpDescriptor[PowerupService.MAXPOWERUPSPORTERO];

            m_descriptoresPortero[(int) Powerup.Manoplas - PowerupService.MAXPOWERUPSTIRADOR] = new PowerUpDescriptor("IT_POWUP_GK_01", LocalizacionManager.instance.GetTexto(124), LocalizacionManager.instance.GetTexto(133), 50);
            m_descriptoresPortero[(int) Powerup.Reflejo - PowerupService.MAXPOWERUPSTIRADOR] = new PowerUpDescriptor("IT_POWUP_GK_02", LocalizacionManager.instance.GetTexto(125), LocalizacionManager.instance.GetTexto(134), 50);
            m_descriptoresPortero[(int) Powerup.Intuicion - PowerupService.MAXPOWERUPSTIRADOR] = new PowerUpDescriptor("IT_POWUP_GK_03", LocalizacionManager.instance.GetTexto(126), LocalizacionManager.instance.GetTexto(135), 50);
            m_descriptoresPortero[(int) Powerup.TiempoBala - PowerupService.MAXPOWERUPSTIRADOR] = new PowerUpDescriptor("IT_POWUP_GK_04", LocalizacionManager.instance.GetTexto(127), LocalizacionManager.instance.GetTexto(136), 50);
        }
    }


    /// <summary>
    /// Modifica la cantidad de un determinado tipo de powerup
    /// </summary>
    /// <param name="_idWs">Id del power up utilizado por los webservices</param>
    /// <param name="_cantidad"></param>
    public void SetCantidadPowerUp(string _idWs, int _cantidad) {
        // comprobar si el power up es de lanzador o de portero
        int intPowerup = (int)IdWsToPowerup(_idWs);
        if (intPowerup < PowerupService.MAXPOWERUPSTIRADOR) {
            // power ups de lanzador => actualizar su cantidad
            UnityEngine.Debug.LogWarning(">>> Modifico la cantidad del power up de LANZADOR " + _idWs + " a " + _cantidad);
            ThrowerPowerups[intPowerup] = _cantidad;
        } else {
            // power ups de portero => actualizar su cantidad
            UnityEngine.Debug.LogWarning(">>> Modifico la cantidad del power up de PORTERO " + _idWs + " a " + _cantidad);
            GoalkeeperPowerups[intPowerup - PowerupService.MAXPOWERUPSTIRADOR] = _cantidad;
        }
    }


    /// <summary>
    /// Incrementa la cantidad de un determinado tipo de powerup
    /// </summary>
    /// <param name="_idWs">Id del power up utilizado por los webservices</param>
    /// <param name="_cantidad"></param>
    public void IncrementarCantidadPowerUp(string _idWs, int _incremento) {
        // comprobar si el power up es de lanzador o de portero
        int intPowerup = (int) IdWsToPowerup(_idWs);
        if (intPowerup < PowerupService.MAXPOWERUPSTIRADOR) {
            // power ups de lanzador => actualizar su cantidad
            UnityEngine.Debug.LogWarning(">>> Incremento la cantidad del power up de LANZADOR " + _idWs + " a " + _incremento);
            ThrowerPowerups[intPowerup] += _incremento;
            ThrowerPowerups[intPowerup] = Math.Max(ThrowerPowerups[intPowerup], 0); // <= asegurarse de que el power up siempre es como minimo 0
        } else {
            // power ups de portero => actualizar su cantidad
            UnityEngine.Debug.LogWarning(">>> Incremento la cantidad del power up de PORTERO " + _idWs + " a " + _incremento);
            GoalkeeperPowerups[intPowerup - PowerupService.MAXPOWERUPSTIRADOR] += _incremento;
            ThrowerPowerups[intPowerup - PowerupService.MAXPOWERUPSTIRADOR] = Math.Max(ThrowerPowerups[intPowerup - PowerupService.MAXPOWERUPSTIRADOR], 0); // <= asegurarse de que el power up siempre es como minimo 0
        }
    }

    /// <summary>
    /// Devuelve la cantidad de un determinado tipo de powerup
    /// </summary>
    /// <param name="_idWs">Identificador del power up que utilizan los web services</param>
    /// <returns></returns>
    public int GetCantidadPowerUp(string _idWs) {
        if(fakedInfinite) return 1;

        // comprobar si el power up es de lanzador o de portero
        int intPowerup = (int) IdWsToPowerup(_idWs);

        if (intPowerup < PowerupService.MAXPOWERUPSTIRADOR) {
            return ThrowerPowerups[intPowerup];
        } else {
            // power ups de portero => actualizar su cantidad
            return GoalkeeperPowerups[intPowerup - PowerupService.MAXPOWERUPSTIRADOR];
        }
    }

    /// <summary>
    /// Devuelve la cantidad de un determinado tipo de powerup
    /// </summary>
    /// <param name="_gameMode">Modo lanzador / portero</param>
    /// <param name="_idPowerup">Identificador numerico / orden del power up</param>
    /// <returns></returns>
    public int GetCantidadPowerUp(GameMode _gameMode, int _idPowerup) {
        if(fakedInfinite) return 1;
        // si el id es incorrecto
        if (_idPowerup < 0)
            return 0;

        if (_gameMode == GameMode.Shooter && _idPowerup < ThrowerPowerups.Length)
            return ThrowerPowerups[_idPowerup];

        if (_gameMode == GameMode.GoalKeeper && _idPowerup < GoalkeeperPowerups.Length)
            return GoalkeeperPowerups[_idPowerup];

        // valor por defecto
        return 0;
    }

}

public interface IPowerupService
{
    void RegisterListener(Action<PowerupUsage> listener);
    void UnregisterListener(Action<PowerupUsage> listener);

    void OnPowerUpUsed(PowerupUsage _info);
}
