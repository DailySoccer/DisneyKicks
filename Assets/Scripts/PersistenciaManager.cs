using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager que se encarga de persistir / recuperar la informacion del juego
/// </summary>
public class PersistenciaManager {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static PersistenciaManager instance {
        get {
            if (m_instance == null)
                m_instance = new PersistenciaManager();
            return m_instance;
        }
    }
    private static PersistenciaManager m_instance;

    // Caracter separador para almacenar varios strings en una unica variable string
    private char[] separadores = { '|' };
    private char[] separadoresIgual = { '=' };

    // Indica si ya se ha realizado alguna carga de datos
    private bool m_cargaDatosRealizada = false;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Carga la informacion almacenada en la preferencias del juego (si no se ha cargado ya antes)
    /// </summary>
    /// <param name="_forzarCarga">Fuerza la carga de las preferencias aunque ya se haya realizado antes</param>
    public void LoadAllData(bool _forzarCarga = false) {
        if (_forzarCarga || !m_cargaDatosRealizada) {
            Debug.LogWarning(">>> Recupero informacion de las preferencias");

            // comprobar si existen las claves asociadas a los logros (es caso de que no crearlas)
            if (!EncryptedPlayerPrefs.HasKey("logrosLanzador")) {
                CreateListaLogrosPlayerPrefs(LogrosManager.logrosLanzador, "logrosLanzador");
                PlayerPrefs.Save();
            }
            if (!EncryptedPlayerPrefs.HasKey("logrosPortero")) {
                CreateListaLogrosPlayerPrefs(LogrosManager.logrosPortero, "logrosPortero");
                PlayerPrefs.Save();
            }
            if (!EncryptedPlayerPrefs.HasKey("logrosDuelo")) {
                CreateListaLogrosPlayerPrefs(LogrosManager.logrosDuelo, "logrosDuelo");
                PlayerPrefs.Save();
            }

            // obtener el dinero del juego y actualizar el dinero en la barra superior
			Interfaz.SetMonedaSoft_SinPersistencia(EncryptedPlayerPrefs.HasKey("softCurrency") ? EncryptedPlayerPrefs.GetInt("softCurrency") : 0);
			Interfaz.SetMonedaHard_SinPersistencia(EncryptedPlayerPrefs.GetInt("hardCurrency", 0));
            cntBarraSuperior.instance.ActualizarDinero();

            // obtener las mejores puntuaciones
            //Player.record_keeper = (PlayerPrefs.HasKey("goalkeeperRecord")) ? PlayerPrefs.GetInt("goalkeeperRecord") : 0;
            //Player.record_thrower = (PlayerPrefs.HasKey("shooterRecord")) ? PlayerPrefs.GetInt("shooterRecord") : 0;
            Interfaz.m_uname = EncryptedPlayerPrefs.GetString ("alias", "");

            // obtener el tiempo de juego
            Interfaz.m_nextTryTime = (EncryptedPlayerPrefs.HasKey("nextTryTime")) ? EncryptedPlayerPrefs.GetInt("nextTryTime") : 0;

            // obtener el avance como portero
            Interfaz.m_asKeeper.record = EncryptedPlayerPrefs.GetInt("goalkeeperRecord", 0);
            Interfaz.m_asKeeper.targets = EncryptedPlayerPrefs.GetInt("goalkeeperTargets", 0);
            Interfaz.m_asKeeper.goals = EncryptedPlayerPrefs.GetInt("goalkeeperGoals", 0);
            Interfaz.m_asKeeper.goalsStopped = EncryptedPlayerPrefs.GetInt("goalkeeperGoalsStopped", 0);
            Interfaz.m_asKeeper.throwOut = EncryptedPlayerPrefs.GetInt("goalkeeperThrowOut", 0);
            Interfaz.m_asKeeper.totalPoints = EncryptedPlayerPrefs.GetInt("goalkeeperTotalPoints", 0);
            Interfaz.m_asKeeper.deflected = EncryptedPlayerPrefs.GetInt("goalkeeperDeflected", 0);
            Interfaz.m_asKeeper.perfects = EncryptedPlayerPrefs.GetInt("goalkeeperPerfects", 0);

            // obtener el avance como lanzador
            Interfaz.m_asThrower.record = EncryptedPlayerPrefs.GetInt("shooterRecord", 0);
            Interfaz.m_asThrower.targets = EncryptedPlayerPrefs.GetInt("shooterTargets", 0);
            Interfaz.m_asThrower.goals = EncryptedPlayerPrefs.GetInt("shooterGoals", 0);
            Interfaz.m_asThrower.goalsStopped = EncryptedPlayerPrefs.GetInt("shooterGoalsStopped", 0);
            Interfaz.m_asThrower.throwOut = EncryptedPlayerPrefs.GetInt("shooterThrowOut", 0);
            Interfaz.m_asThrower.totalPoints = EncryptedPlayerPrefs.GetInt("shooterTotalPoints", 0);
            Interfaz.m_asThrower.deflected = EncryptedPlayerPrefs.GetInt("shooterDeflected", 0);
            Interfaz.m_asThrower.perfects = EncryptedPlayerPrefs.GetInt("shooterPerfects", 0);

            // obtener el avance en duelos
            Interfaz.m_duelsPlayed = EncryptedPlayerPrefs.GetInt("duelsPlayed",0);
            Interfaz.m_duelsWon = EncryptedPlayerPrefs.GetInt("duelsWon",0);
            Interfaz.m_perfectDuels = EncryptedPlayerPrefs.GetInt("perfectDuels",0);

            // obtener la ultima mision desbloqueada
            Interfaz.ultimaMisionDesbloqueada = EncryptedPlayerPrefs.GetInt("ultimaMisionDesbloqueada", 0);

            // actualizar el estado de desbloqueo de los escudos en funcion de la ultima fase desbloqueada
            EscudosManager.instance.ActualizarEstadoDesbloqueoEscudos(Interfaz.ultimaMisionDesbloqueada);

            // obtener los lanzadores comprados
            if (PlayerPrefs.HasKey("shootersComprados")) {
                string strShootersComprados = EncryptedPlayerPrefs.GetString("shootersComprados");
                if (strShootersComprados != null) {
                    string[] shootersComprados = strShootersComprados.Split(separadores);
                    if (shootersComprados != null) {
                        for (int i = 0; i < shootersComprados.Length; ++i) {
                            Jugador jugador = InfoJugadores.instance.GetJugador(shootersComprados[i]);
                            if (jugador != null)
                                jugador.estado = Jugador.Estado.ADQUIRIDO;
                        }
                    }
                }
            }

            // obtener los porteros comprados
            if (PlayerPrefs.HasKey("goalkeepersComprados")) {
                string strGoalkeepersComprados = EncryptedPlayerPrefs.GetString("goalkeepersComprados");
                if (strGoalkeepersComprados != null) {
                    string[] goalkeepersComprados = strGoalkeepersComprados.Split(separadores);
                    if (goalkeepersComprados != null) {
                        for (int i = 0; i < goalkeepersComprados.Length; ++i) {
                            Jugador jugador = InfoJugadores.instance.GetJugador(goalkeepersComprados[i]);
                            if (jugador != null) {
                                jugador.estado = Jugador.Estado.ADQUIRIDO;
                                Debug.LogWarning(">>> El jugador " + jugador.nombre + " pasa a estar ADQUIRIDO");
                            }
                        }
                    }
                }
            }

            // obtener equipaciones de lanzador compradas
            if (PlayerPrefs.HasKey("shooterEquipaciones")) {
                string strEquipacionesLanzador = EncryptedPlayerPrefs.GetString("shooterEquipaciones");
                if (strEquipacionesLanzador != null) {
                    string[] equipacionesLanzador = strEquipacionesLanzador.Split(separadores);
                    if (equipacionesLanzador != null) {
                        for (int i = 0; i < equipacionesLanzador.Length; ++i) {
                            Equipacion equipacion = EquipacionManager.instance.GetEquipacion(equipacionesLanzador[i]);
                            if (equipacion != null)
                                equipacion.estado = Equipacion.Estado.ADQUIRIDA;
                        }
                    }
                }
            }

            // obtener equipaciones de portero compradas
            if (PlayerPrefs.HasKey("goalkeeperEquipaciones")) {
                string strEquipacionesPortero = EncryptedPlayerPrefs.GetString("goalkeeperEquipaciones");
                if (strEquipacionesPortero != null) {
                    string[] equipacionesPortero= strEquipacionesPortero.Split(separadores);
                    if (equipacionesPortero != null) {
                        for (int i = 0; i < equipacionesPortero.Length; ++i) {
                            Equipacion equipacion = EquipacionManager.instance.GetEquipacion(equipacionesPortero[i]);
                            if (equipacion != null)
                                equipacion.estado = Equipacion.Estado.ADQUIRIDA;
                        }
                    }
                }
            }

            // obtener los powerups de lanzador comprados
            if (PlayerPrefs.HasKey("pwrUpsLanzador")) {
                string strPowerUpsLanzador = EncryptedPlayerPrefs.GetString("pwrUpsLanzador");
                if (strPowerUpsLanzador != null) {
                    string[] powerUps = strPowerUpsLanzador.Split(separadores);
                    if (powerUps != null) {
                        for (int i = 0; i < powerUps.Length; ++i) {
                            string[] infoPowerUp = powerUps[i].Split(separadoresIgual);
                            if (infoPowerUp != null && infoPowerUp.Length == 2) {
                                PowerupService.ownInventory.SetCantidadPowerUp(infoPowerUp[0], int.Parse(infoPowerUp[1]));
                            }
                        }
                    }
                }
            }

            // obtener los powerups de portero comprados
            if (PlayerPrefs.HasKey("pwrUpsPortero")) {
                string strPowerUpsPortero = EncryptedPlayerPrefs.GetString("pwrUpsPortero");
                if (strPowerUpsPortero != null) {
                    string[] powerUps = strPowerUpsPortero.Split(separadores);
                    if (powerUps != null) {
                        for (int i = 0; i < powerUps.Length; ++i) {
                            string[] infoPowerUp = powerUps[i].Split(separadoresIgual);
                            if (infoPowerUp != null && infoPowerUp.Length == 2) {
                                PowerupService.ownInventory.SetCantidadPowerUp(infoPowerUp[0], int.Parse(infoPowerUp[1]));
                            }
                        }
                    }
                }
            }

            // obtener los escudos comprados
            if (PlayerPrefs.HasKey("escudos")) {
                string strEscudos = EncryptedPlayerPrefs.GetString("escudos");
                if (strEscudos != null) {
                    string[] escudos = strEscudos.Split(separadores);
                    if (escudos != null) {
                        for (int i = 0; i < escudos.Length; ++i) {
                            string[] infoEscudo = escudos[i].Split(separadoresIgual);
                            if (infoEscudo != null && infoEscudo.Length == 2) {
                                Escudo escudo = EscudosManager.instance.GetEscudo(infoEscudo[0]);
                                if (escudo != null) {
                                    escudo.numUnidades = int.Parse(infoEscudo[1]);
                                }
                            }
                        }
                    }
                }
            }

            // fuerza a que si los jugadores y equipaciones seleccionadas actualmente no estan ADQUIRIDOS, sean substidos por unos que si
            ifcVestuario.instance.ComprobarJugadoresYEquipacionesAdquiridos(true);

            // cargar el avance del jugador
            CargarObjetivosMision();

            // actualizar el estado de los jugadores
            EquipacionManager.instance.RefreshEquipacionesDesbloqueadas(Interfaz.ultimaMisionDesbloqueada);

            // indicar que la carga de datos ya se ha realizado una vez
            m_cargaDatosRealizada = true;
        }
        // comprobar si existen logros que aun no han sido registrados
        int recompensaPendiente = 0;
        List<string> idsLogros = new List<string>();
        if (PersistenciaManager.instance.CheckHayLogrosSinRecompensar(ref recompensaPendiente, ref idsLogros)) {
            // mostrar en la barra de opciones una "exclamacion"
            if (cntBarraSuperior.instance != null)
                cntBarraSuperior.instance.MostrarQueHayNuevosLogros();

            // actualizar el dinero
            Interfaz.MonedasHard += recompensaPendiente;

            // actualizar el progreso de los logros para que esta alerta no se dispare mas
            SaveLogros();

            // crear los dialogos para mostrar cada uno de los logros desbloqueados
            for (int i = 0; i < idsLogros.Count; ++i) {
                DialogManager.instance.RegistrarDialogo(new DialogDefinition(DialogDefinition.TipoDialogo.LOGRO_DESBLOQUEADO, idsLogros[i]));
            }
        }
    }


    /*
    /// <summary>
    /// Almacena la informacion del juego
    /// </summary>
    public void Save() {
        Debug.LogWarning(">>> Almaceno informacion de las preferencias");

        // almacenar el tiempo de juego
        PlayerPrefs.SetInt("nextTryTime", Interfaz.m_nextTryTime);

        // almacenar el avance como portero
        PlayerPrefs.SetInt("goalkeeperRecord", Interfaz.m_asKeeper.record);
        PlayerPrefs.SetInt("goalkeeperTargets", Interfaz.m_asKeeper.targets);
        PlayerPrefs.SetInt("goalkeeperGoals", Interfaz.m_asKeeper.goals);
        PlayerPrefs.SetInt("goalkeeperGoalsStopped", Interfaz.m_asKeeper.goalsStopped);
        PlayerPrefs.SetInt("goalkeeperThrowOut", Interfaz.m_asKeeper.throwOut);
        PlayerPrefs.SetInt("goalkeeperTotalPoints", Interfaz.m_asKeeper.totalPoints);
        PlayerPrefs.SetInt("goalkeeperDeflected", Interfaz.m_asKeeper.deflected);
        PlayerPrefs.SetInt("goalkeeperPerfects", Interfaz.m_asKeeper.perfects);

        // almacenar el avance como lanzador
        PlayerPrefs.SetInt("shooterRecord", Interfaz.m_asThrower.record);
        PlayerPrefs.SetInt("shooterTargets", Interfaz.m_asThrower.targets);
        PlayerPrefs.SetInt("shooterGoals", Interfaz.m_asThrower.goals);
        PlayerPrefs.SetInt("shooterGoalsStopped", Interfaz.m_asThrower.goalsStopped);
        PlayerPrefs.SetInt("shooterThrowOut", Interfaz.m_asThrower.throwOut);
        PlayerPrefs.SetInt("shooterTotalPoints", Interfaz.m_asThrower.totalPoints);
        PlayerPrefs.SetInt("shooterDeflected", Interfaz.m_asThrower.deflected);
        PlayerPrefs.SetInt("shooterPerfects", Interfaz.m_asThrower.perfects);

        // almacenar la ultima mision desbloqueada
        PlayerPrefs.SetInt("ultimaMisionDesbloqueada", Interfaz.ultimaMisionDesbloqueada);

        PlayerPrefs.Save();
    }
    */

    public void ActualizarUltimoNivelDesbloqueado(int _nivel)
    {
        int lastSaved = EncryptedPlayerPrefs.GetInt("ultimaMisionDesbloqueada", 0);
        if(_nivel > lastSaved)
        {
            Interfaz.ultimaMisionDesbloqueada = _nivel;
            EncryptedPlayerPrefs.SetInt("ultimaMisionDesbloqueada", _nivel);
            PlayerPrefs.Save();
        }
    }

    public void SaveMoney()
    {
        EncryptedPlayerPrefs.SetInt("softCurrency", Interfaz.MonedasSoft);
        PlayerPrefs.Save();
    }

    public void SaveAlias(string alias)
    {
        EncryptedPlayerPrefs.SetString("alias", alias);
        PlayerPrefs.Save();
        Interfaz.m_uname = alias;
    }

    public void SaveHardMoney()
    {
        EncryptedPlayerPrefs.SetInt("hardCurrency", Interfaz.MonedasHard);
        PlayerPrefs.Save();
    }


    /// <summary>
    /// Almacena los jugadores adquiridos actualmente en las preferencias
    /// </summary>
    public void SaveJugadoresComprados() {
        Debug.Log(">>> GUARDAR JUGADORES");

        // almacenar los lanzadores comprados
        string strLanzadoresComprados = "";
        for (int i = 0; i < InfoJugadores.instance.numLanzadores; ++i) {
            Jugador jugador = InfoJugadores.instance.GetTirador(i);
            if (jugador != null && jugador.estado == Jugador.Estado.ADQUIRIDO) {
                // añadir el separador antes del id del jugador
                if (strLanzadoresComprados != "")
                    strLanzadoresComprados += separadores[0];

                strLanzadoresComprados += jugador.assetName;
            }
        }
        EncryptedPlayerPrefs.SetString("shootersComprados", strLanzadoresComprados);

        // almacenar los porteros comprados
        string strPorterosComprados = "";
        for (int i = 0; i < InfoJugadores.instance.numPorteros; ++i) {
            Jugador jugador = InfoJugadores.instance.GetPortero(i);
            if (jugador != null && jugador.estado == Jugador.Estado.ADQUIRIDO) {
                // añadir el separador antes del id del jugador
                if (strPorterosComprados != "")
                    strPorterosComprados += separadores[0];

                strPorterosComprados += jugador.assetName;
            }
        }
        EncryptedPlayerPrefs.SetString("goalkeepersComprados", strPorterosComprados);

        PlayerPrefs.Save();
    }


    /// <summary>
    /// Almacena las equipaciones adquiridas actualmente en las preferencias
    /// </summary>
    public void SaveEquipacionesCompradas() {
        Debug.Log(">>> GUARDAR EQUIPACIONES");

        // almacenar las equipaciones de lanzador compradas
        string strEquipacionesLanzadorCompradas = "";
        for (int i = 0; i < EquipacionManager.instance.numEquipacionesLanzador; ++i) {
            Equipacion equipacion = EquipacionManager.instance.GetEquipacionLanzador(i);
            if (equipacion != null && equipacion.estado == Equipacion.Estado.ADQUIRIDA) {
                // añadir el separador antes del id de la equipacion
                if (strEquipacionesLanzadorCompradas != "")
                    strEquipacionesLanzadorCompradas += separadores[0];

                strEquipacionesLanzadorCompradas += equipacion.assetName;
            }
        }
        EncryptedPlayerPrefs.SetString("shooterEquipaciones", strEquipacionesLanzadorCompradas);

        // almacenar las equipaciones de portero compradas
        string strEquipacionesPorteroCompradas = "";
        for (int i = 0; i < EquipacionManager.instance.numEquipacionesPortero; ++i) {
            Equipacion equipacion = EquipacionManager.instance.GetEquipacionPortero(i);
            if (equipacion != null && equipacion.estado == Equipacion.Estado.ADQUIRIDA) {
                // añadir el separador antes del id de la equipacion
                if (strEquipacionesPorteroCompradas != "")
                    strEquipacionesPorteroCompradas += separadores[0];

                strEquipacionesPorteroCompradas += equipacion.assetName;
            }
        }
        EncryptedPlayerPrefs.SetString("goalkeeperEquipaciones", strEquipacionesPorteroCompradas);

        PlayerPrefs.Save();
    }


    /// <summary>
    /// Almacena el numero de escudos que ha adquirido actualmente el usuario en las preferencias
    /// </summary>
    public void SaveEscudos() {
        Debug.Log(">>> GUARDAR ESCUDOS");

        // almacenar los escudos
        string strEscudos = "";
        for (int i = 0; i < EscudosManager.instance.GetNumEscudos(); ++i) {
            Escudo escudo = EscudosManager.instance.GetEscudo(i);
            if (escudo != null && escudo.numUnidades > 0) {
                // añadir el separador antes del id de la equipacion
                if (strEscudos != "")
                    strEscudos += separadores[0];

                strEscudos += escudo.id + separadoresIgual[0] + escudo.numUnidades;
            }
        }
        EncryptedPlayerPrefs.SetString("escudos", strEscudos);

        PlayerPrefs.Save();
    }


    /// <summary>
    /// Almacena las preferencias el numero de power ups que ha adquirido actualmente el usuario
    /// </summary>
    public void SavePowerUps() {
        Debug.Log(">>> GUARDAR POWER_UPS");

        // almacenar los power ups de Lanzador
        string strPowerUpsLanzador = "";

        for (int i = 0; i < PowerupInventory.descriptoresLanzador.Length; ++i) {
            int cantidad = PowerupService.ownInventory.GetCantidadPowerUp(PowerupInventory.descriptoresLanzador[i].idWs);
            if (cantidad > 0) {
                // añadir separador antes del powerup
                if (strPowerUpsLanzador != "")
                    strPowerUpsLanzador += " |";

                strPowerUpsLanzador += PowerupInventory.descriptoresLanzador[i].idWs + separadoresIgual[0] + cantidad.ToString();
            }
        }
        EncryptedPlayerPrefs.SetString("pwrUpsLanzador", strPowerUpsLanzador);

        // almacenar los power ups de Portero
        string strPowerUpsPortero = "";
        for (int i = 0; i < PowerupInventory.descriptoresPortero.Length; ++i) {
            int cantidad = PowerupService.ownInventory.GetCantidadPowerUp(PowerupInventory.descriptoresPortero[i].idWs);
            if (cantidad > 0) {
                // añadir separador antes del powerup
                if (strPowerUpsPortero != "")
                    strPowerUpsPortero += " |";

                strPowerUpsPortero += PowerupInventory.descriptoresPortero[i].idWs + separadoresIgual[0] + cantidad.ToString();
            }
        }
        EncryptedPlayerPrefs.SetString("pwrUpsPortero", strPowerUpsPortero);

        PlayerPrefs.Save();
    }


    /// <summary>
    /// Almacena el avance que el usuario ha realizado con los logros
    /// </summary>
    public void SaveLogros() {
        Debug.Log(">>> GUARDAR LOGROS");

        // almacenar el avance en los logros de lanzador
        CreateListaLogrosPlayerPrefs(LogrosManager.logrosLanzador, "logrosLanzador");
        CreateListaLogrosPlayerPrefs(LogrosManager.logrosPortero, "logrosPortero");
        CreateListaLogrosPlayerPrefs(LogrosManager.logrosDuelo, "logrosDuelo");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Crea las claves en las playerprefs de una determinada lista de logros ( lista para guardar cuando se haga un playerPrefs.save() )
    /// </summary>
    /// <param name="_listaLogros"></param>
    /// <param name="_playerPrefsKey"></param>
    private void CreateListaLogrosPlayerPrefs(List<GrupoLogros> _listaLogros, string _playerPrefsKey) {
        string strLogros = "";

        if (_listaLogros != null) {
            foreach (GrupoLogros grupoLogros in _listaLogros) {
                // añadir un separador si procede
                if (strLogros != "")
                    strLogros += separadores[0];

                strLogros += grupoLogros.prefijoComunIdLogros + "=" + grupoLogros.nivelAlcanzado;
            }
        }
        EncryptedPlayerPrefs.SetString(_playerPrefsKey, strLogros);
    }


    /// <summary>
    /// Comprueba si se ha conseguido algun logro nuevo desde la ultima vez que se consultaron los logros en las playerPrefs
    /// </summary>
    /// <param name="_recompensaAcumulada"></param>
    /// <param name="_idsLogros">Lista en la que se van a añadir los ids de los logros conseguidos recientemente</param>
    /// <returns></returns>
    public bool CheckHayLogrosSinRecompensar(ref int _recompensaAcumulada, ref List<string> _idsLogros) {
        int recompensaLogrosLanzador = 0, recompensaLogrosPortero = 0, recompensaLogrosDuelo = 0;
        bool hayLogrosNuevos = false;

        hayLogrosNuevos |= CheckHayLogrosSinRecompensarEnLista(LogrosManager.logrosLanzador, "logrosLanzador", ref recompensaLogrosLanzador, ref _idsLogros);
        hayLogrosNuevos |= CheckHayLogrosSinRecompensarEnLista(LogrosManager.logrosPortero, "logrosPortero", ref recompensaLogrosPortero, ref _idsLogros);
        hayLogrosNuevos |= CheckHayLogrosSinRecompensarEnLista(LogrosManager.logrosDuelo, "logrosDuelo", ref recompensaLogrosDuelo, ref _idsLogros);

        _recompensaAcumulada = recompensaLogrosLanzador + recompensaLogrosPortero + recompensaLogrosDuelo;

        return hayLogrosNuevos;
    }


    /// <summary>
    /// Comprueba si se ha subido de nivel en alguno de los logros de la lista "_listaLogros" con respecto a lo que hay almacenado en las preferencias
    /// </summary>
    /// <param name="_listaLogros"></param>
    /// <param name="_playerPrefKey"></param>
    /// <param name="_recompensaAcumulada"></param>
    /// <param name="_listIdLogros">Lista a la que se añaden los identificadores de los logros que han subido de nivel recientemente</param>
    /// <returns></returns>
    private bool CheckHayLogrosSinRecompensarEnLista(List<GrupoLogros> _listaLogros, string _playerPrefKey, ref int _recompensaAcumulada, ref List<string> _listIdLogros) {
        bool hayLogrosNuevos = false;

        if (EncryptedPlayerPrefs.HasKey(_playerPrefKey) && _listaLogros != null) {
            string strLogros = EncryptedPlayerPrefs.GetString(_playerPrefKey);
            if (strLogros != null) {
                string[] logros = strLogros.Split(separadores);
                if (logros != null) {
                    for (int i = 0; i < logros.Length; ++i) {
                        string[] infoLogro = logros[i].Split(separadoresIgual);
                        if (infoLogro != null && infoLogro.Length == 2) {
                            bool logroEncontrado = false;
                            for (int j = 0; (j < _listaLogros.Count) && (!logroEncontrado); ++j) {
                                if (_listaLogros[j].prefijoComunIdLogros == infoLogro[0]) {
                                    logroEncontrado = true;
                                    int nivelAlmacenado = int.Parse(infoLogro[1]);
                                    if (_listaLogros[j].nivelAlcanzado > nivelAlmacenado) {
                                        // indicar que hay nuevos logros en la lista
                                        hayLogrosNuevos = true;

                                        // acumular la recompensa
                                        _recompensaAcumulada += _listaLogros[j].GetRecompensaAcumulada(nivelAlmacenado, _listaLogros[j].nivelAlcanzado);

                                        // meter en la lista de dialogos los dialogos conseguidos
                                        for (int k = nivelAlmacenado; k < _listaLogros[j].nivelAlcanzado; k++)
                                            _listIdLogros.Add(_listaLogros[j].GetLogro(k).id);

                                        _listaLogros[j].subidaNivelReciente = true; // <= indicar que en este grupo de logros ha habido una subida de nivel
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return hayLogrosNuevos;
    }


    public void GuardarPartidaSinglePlayer()
    {
        int bitMapProgreso  = 0;
        //TODO convertir esto a guardar objetivos de mision
        GameLevelMission gameLevel = MissionManager.instance.GetGameLevelMission(GameplayService.gameLevelMission.MissionName);
        GameplayService.gameLevelMission.UnFreeze();
        MissionAchievement[] retosAntes = GameplayService.gameLevelMission.GetAchievements().ToArray();
        //MissionAchievement[] retosDespues = MissionManager.instance.GetMission().Achievements.ToArray();

        if (gameLevel == null) {
            Debug.LogWarning(">>> No hay informacion del nivel " + GameplayService.gameLevelMission.MissionName);
        } else {
            string claveNivel = "prog" + gameLevel.MissionName;
            bitMapProgreso = EncryptedPlayerPrefs.GetInt(claveNivel, 0);

            for(int i = 0 ; i < retosAntes.Length ; i++)
            {
                int flag = (bitMapProgreso % ((int)Mathf.Pow(10, i+1)))/(int)Mathf.Pow(10, i);
                if(retosAntes[i].IsAchieved() && (flag == 0)) //importante el isAchieved() primero para que lo haga, evitar el lazy
                {
                    bitMapProgreso += (int)Mathf.Pow(10, retosAntes[i].Code);
                }
            }

            EncryptedPlayerPrefs.SetInt(claveNivel, bitMapProgreso);
        }

        // guardar las preferencias
        PlayerPrefs.Save();
    }

    public void GuardarPartidaMultiPlayer(bool _ganador, bool _perfect)
    {
        EncryptedPlayerPrefs.SetInt("duelsPlayed", PlayerPrefs.GetInt("duelsPlayed", 0) + 1);
        Interfaz.m_duelsPlayed++;
        if(_ganador)
        {
            EncryptedPlayerPrefs.SetInt("duelsWon", PlayerPrefs.GetInt("duelsWon", 0) + 1);
            Interfaz.m_duelsWon++;
        }

        if(_perfect)
        {
            EncryptedPlayerPrefs.SetInt("perfectDuels", PlayerPrefs.GetInt("perfectDuels", 0) + 1);
            Interfaz.m_perfectDuels++;
        }

        // guardar las preferencias
        PlayerPrefs.Save();
    }

    public void AcumularRondaMision(Result _result, bool _perfect, int _points, bool _guardarProgresoMision)
    {
        bool goalkeeper = GameplayService.initialGameMode == GameMode.GoalKeeper;
        if(!goalkeeper)
        {
            int record = EncryptedPlayerPrefs.GetInt("shooterRecord", 0);
            if(record < RoundInfoManager.instance.puntos)
            {
                EncryptedPlayerPrefs.SetInt("shooterRecord", RoundInfoManager.instance.puntos);
                Interfaz.m_asThrower.record = RoundInfoManager.instance.puntos;
            }

            if(_points > 0)
            {
                EncryptedPlayerPrefs.SetInt("shooterTotalPoints", PlayerPrefs.GetInt("shooterTotalPoints", 0) + _points);
                Interfaz.m_asThrower.totalPoints += _points;
            }

            if(_perfect)
            {
                EncryptedPlayerPrefs.SetInt("shooterPerfects", PlayerPrefs.GetInt("shooterPerfects", 0) + 1); 
                Interfaz.m_asThrower.perfects++;
            }

            switch (_result)
            {
                case Result.Saved:
                    EncryptedPlayerPrefs.SetInt("shooterGoalsStopped", EncryptedPlayerPrefs.GetInt("shooterGoalsStopped", 0) + 1); Interfaz.m_asThrower.goalsStopped++; break;
                case Result.Stopped:
                    EncryptedPlayerPrefs.SetInt("shooterDeflected", EncryptedPlayerPrefs.GetInt("shooterDeflected", 0) + 1); Interfaz.m_asThrower.deflected++; break;
                case Result.Goal:
                    EncryptedPlayerPrefs.SetInt("shooterGoals", EncryptedPlayerPrefs.GetInt("shooterGoals", 0) + 1); Interfaz.m_asThrower.goals++; break;
                case Result.OutOfBounds:
                    EncryptedPlayerPrefs.SetInt("shooterThrowOut", EncryptedPlayerPrefs.GetInt("shooterThrowOut", 0) + 1); Interfaz.m_asThrower.throwOut++; break;
                case Result.Target:
                    EncryptedPlayerPrefs.SetInt("shooterTargets", EncryptedPlayerPrefs.GetInt("shooterTargets", 0) + 1); Interfaz.m_asThrower.targets++; break;
            }
        }
        else
        {
            int record = EncryptedPlayerPrefs.GetInt("goalkeeperRecord", 0);
            if(record < RoundInfoManager.instance.puntos)
            {
                EncryptedPlayerPrefs.SetInt("goalkeeperRecord", RoundInfoManager.instance.puntos);
                Interfaz.m_asKeeper.record = RoundInfoManager.instance.puntos;
            }

            if(_points > 0)
            {
                EncryptedPlayerPrefs.SetInt("goalkeeperTotalPoints", EncryptedPlayerPrefs.GetInt("goalkeeperTotalPoints", 0) + _points);
                Interfaz.m_asKeeper.totalPoints += _points;
            }

            if(_perfect)
            {
                EncryptedPlayerPrefs.SetInt("goalkeeperPerfects", EncryptedPlayerPrefs.GetInt("goalkeeperPerfects", 0) + 1); 
                Interfaz.m_asKeeper.perfects++;
            }

            switch (_result)
            {
                case Result.Saved:
                    EncryptedPlayerPrefs.SetInt("goalkeeperGoalsStopped", EncryptedPlayerPrefs.GetInt("goalkeeperGoalsStopped", 0) + 1); Interfaz.m_asKeeper.goalsStopped++; break;
                case Result.Stopped:
                    EncryptedPlayerPrefs.SetInt("goalkeeperDeflected", EncryptedPlayerPrefs.GetInt("goalkeeperDeflected", 0) + 1); Interfaz.m_asKeeper.deflected++; break;
                case Result.Goal:
                    EncryptedPlayerPrefs.SetInt("goalkeeperGoals", EncryptedPlayerPrefs.GetInt("goalkeeperGoals", 0) + 1); Interfaz.m_asKeeper.goals++; break;
                case Result.OutOfBounds:
                    EncryptedPlayerPrefs.SetInt("goalkeeperThrowOut", EncryptedPlayerPrefs.GetInt("goalkeeperThrowOut", 0) + 1); Interfaz.m_asKeeper.throwOut++; break;
                case Result.Target:
                    EncryptedPlayerPrefs.SetInt("goalkeeperTargets", EncryptedPlayerPrefs.GetInt("goalkeeperTargets", 0) + RoundInfoManager.instance.numTargets); Interfaz.m_asKeeper.targets++; break;
            }
        }

        if(_guardarProgresoMision)
        {
            MissionAchievement[] retosActuales = MissionManager.instance.GetMission().Achievements.ToArray();
            string claveNivel = "prog" + GameplayService.gameLevelMission.MissionName;
            int bitMapProgreso = EncryptedPlayerPrefs.GetInt(claveNivel, 0);
            
            for(int i = 0 ; i < retosActuales.Length ; i++)
            {
                int flag = (bitMapProgreso % ((int)Mathf.Pow(10, i+1)))/(int)Mathf.Pow(10, i);
                if((flag == 0) && retosActuales[i].IsAchieved())
                {
                    bitMapProgreso += (int)Mathf.Pow(10, retosActuales[i].Code);
                }
            }
            EncryptedPlayerPrefs.SetInt(claveNivel, bitMapProgreso);
        }

        PlayerPrefs.Save();
    }


    void CargarObjetivosMision()
    {
        foreach(GameLevelMission mision in MissionManager.instance.ListaLevelMissions)
        {
            int bitMap = EncryptedPlayerPrefs.GetInt("prog" + mision.MissionName, 0);

            List<MissionAchievement> listaObjetivos = mision.GetAchievements();
            foreach(MissionAchievement ma in listaObjetivos)
            {
                int flag = (bitMap % ((int)Mathf.Pow(10, ma.Code+1)))/(int)Mathf.Pow(10, ma.Code);
                if(flag > 0)
                {
                    ma.SetAchieved(true);
                }
            }
        }
    }

    
}
