//#define DEBUG_MULTI

using UnityEngine;
using System.Collections;




/// <summary>
/// Clase para mostrar el vestuario; donde se puede equipar a los jugadores y realizar compras
/// TODO: como posible mejora, utiliaz un flag para que solo se llame a "PersistenciaManager.instance.SaveVestuario();" cuando se haya realizado alguna compra
/// </summary>
public class ifcVestuario : ifcBase {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // -----------------------------------------------------------------------------

    /// <summary>
    /// Posicion de esta pantalla cuando esta en reposo
    /// </summary>
    private Vector3 POSICION_REPOSO = new Vector3(0.5f, 0.5f, 0.0f);


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Modos en los que se puede mostrar el vestuario
    /// </summary>
    public enum TipoVestuario { LANZADOR, PORTERO };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static ifcVestuario instance {
        get {
            if (m_instance == null) {
                if (Interfaz.instance != null) {
                    Transform tr = Interfaz.instance.transform.FindChild("Vestuario");
                    if (tr != null) {
                        m_instance = tr.GetComponent<ifcVestuario>();
                        m_instance.Start();
                    }
                }
            }
            return m_instance;
        }
    }
    private static ifcVestuario m_instance;

    // pantalla desde la que se ha llegado a esta interfaz (para saber cuando se ejecute un back a donde hay que volver)
	private ifcBase m_pantallaAnterior;

    // referencias a elementos de esta interfaz
    private GameObject m_goJugadorLanzador;
    private GameObject m_goJugadorPortero;
    private GameObject m_goEquipacionLanzador;
    private GameObject m_goEquipacionPortero;
    private btnButton m_btnBack;
    private btnButton m_btnInfo;
    private btnButton m_btnPortero;
    private btnButton m_btnLanzador;
    private btnButton m_btnListo;

    private GUITexture m_imgEscudo;
    private GUIText m_txtMultiplicador;

    // grupos de items para comprar
    private cntCompraItemsContainer m_cntCompraPowerUpsLanzador;
    private cntCompraItemsContainer m_cntCompraPowerUpsPortero;
    private cntCompraItemsContainer m_cntCompraEscudos;

    // tooltip para mostrar que un jugador o una equipacion esta disponible
    private cntTooltipItemDisponible m_tooltipItemDisponible;

    // modo en el que se muestra la tienda
    public TipoVestuario m_tipoVestuario = TipoVestuario.LANZADOR;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
    }


    /// <summary>
    /// Obtiene las referencias a los elementos de esta interfaz
    /// </summary>
    private void GetReferencias() {
        // boton back
        if (m_btnBack == null) {
            m_btnBack = transform.FindChild("btnAtras").GetComponent<btnButton>();
            m_backMethod = Back;
            m_btnBack.action = Back;
        }

        // tooltip para mostrar si un jugador o una equipacion esta disponible
        if (m_tooltipItemDisponible == null)
            m_tooltipItemDisponible = transform.FindChild("tooltipItemDisponible").GetComponent<cntTooltipItemDisponible>();

        // boton lanzador
        if (m_btnLanzador == null)
            m_btnLanzador = transform.FindChild("botones/btnLanzador").GetComponent<btnButton>();
        m_btnLanzador.Toggle = true;
        m_btnLanzador.action = (_name) => {
            Interfaz.ClickFX();

            EquipacionManager.instance.ResetEquipacion(true);
            Interfaz.instance.ResetJugador(true);

            m_tipoVestuario = TipoVestuario.LANZADOR;

            // mostrar el modelo que corresponda
            Interfaz.instance.RefrescarModelosJugadores(true, false);

            // actualizar los controles
            ShowAs(m_tipoVestuario);
        };

        // boton portero
        if (m_btnPortero == null)
            m_btnPortero = transform.FindChild("botones/btnPortero").GetComponent<btnButton>();
        m_btnPortero.Toggle = true;
        m_btnPortero.action = (_name) => {
            Interfaz.ClickFX();

            EquipacionManager.instance.ResetEquipacion(false);
            Interfaz.instance.ResetJugador(false);

            m_tipoVestuario = TipoVestuario.PORTERO;

            // mostrar el modelo que corresponda
            Interfaz.instance.RefrescarModelosJugadores(false, true);

            // actualizar los controles
            ShowAs(m_tipoVestuario);
        };

        // grupos de items a comprar
        if (m_cntCompraPowerUpsLanzador == null)
            m_cntCompraPowerUpsLanzador = transform.FindChild("CompraPowerUpsLanzador").GetComponent<cntCompraItemsContainer>();
        if (m_cntCompraPowerUpsPortero == null)
            m_cntCompraPowerUpsPortero = transform.FindChild("CompraPowerUpsPortero").GetComponent<cntCompraItemsContainer>();
        if (m_cntCompraEscudos == null)
            m_cntCompraEscudos = transform.FindChild("CompraEscudos").GetComponent<cntCompraItemsContainer>();

        // boton info lanzador
        if (m_btnInfo == null) {
            m_btnInfo = transform.FindChild("btnInfo").GetComponent<btnButton>();
            m_btnInfo.action = (_name) => {
                Interfaz.ClickFX();
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOnClip);

                // desplegar la info
                if (m_tipoVestuario == TipoVestuario.LANZADOR)
                    ifcDialogoInfoJugador.instance.Show(InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower));
                else
                    ifcDialogoInfoJugador.instance.Show(InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper));
                ifcDialogoInfoJugador.instance.Desplegar();

                // ocultar este boton
                m_btnInfo.gameObject.SetActive(false);
            };
        }


        // boton listo
        if (m_btnListo == null) {
            m_btnListo = transform.FindChild("btnListo").GetComponent<btnButton>();
            m_btnListo.action = (_name) => {
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.confirmClip);
                // si la pantalla anterior era el "MainMenu" (estoy en modo multiplayer)
                bool estoyJugandoDuelo = (m_pantallaAnterior == ifcMainMenu.instance);

                // comprobar la fase desbloqueada
                int faseMinima = 4;
                if (estoyJugandoDuelo && Interfaz.ultimaMisionDesbloqueada < faseMinima) {
                    ifcDialogBox.instance.ShowOneButtonDialog(
                        ifcDialogBox.OneButtonType.POSITIVE,
                        LocalizacionManager.instance.GetTexto(258).ToUpper(),
                        string.Format(LocalizacionManager.instance.GetTexto(259), "<color=#ddf108>" + LocalizacionManager.instance.GetTexto(11) + " " + ((int) (faseMinima / 10) + 1) + "-" + (faseMinima % 10) + "</color>"),
                        LocalizacionManager.instance.GetTexto(45).ToUpper());
                    return;
                }

                // si el usuario necesita al menos un lanzador para jugar => comprobar que lo tenga
                if ((estoyJugandoDuelo || (!estoyJugandoDuelo && GameplayService.initialGameMode == GameMode.Shooter))
                    && !InfoJugadores.instance.HayAlgunLanzadorAdquirido()) {

                    ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(84).ToUpper(),
                            LocalizacionManager.instance.GetTexto(94),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());

                    return;
                }

                // si el usuario necesita al menos un portero para jugar => comprobar que lo tenga
                if ((estoyJugandoDuelo || (!estoyJugandoDuelo && GameplayService.initialGameMode == GameMode.GoalKeeper))
                    && !InfoJugadores.instance.HayAlgunPorteroAdquirido()) {

                    ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(84).ToUpper(),
                            LocalizacionManager.instance.GetTexto(95),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());

                    return;
                }

                Interfaz.ClickFX();

                // pedir el alias al usuario
                if (estoyJugandoDuelo && Interfaz.m_uname == "") {
                    PedirAlias(Interfaz.m_uname);
                    return;
                }

                // al salir de esta pantalla verificar que los jugadores y las equipaciones seleccionadas estan adquiridas
                ComprobarJugadoresYEquipacionesAdquiridos();

                // comprobar si se esta jugando en modo "single" o "multi"
                if (m_pantallaAnterior == ifcMainMenu.instance) {
                    // estoy jugando Duelo => ir al "Lobby"

                    // ocultar la barra superior
                    //cntBarraSuperior.instance.SetVisible(false);
#if DEBUG_MULTI
                GoDuelo();
#else
                    Interfaz.instance.getServerIP(Stats.tipo, Stats.zona);
#endif
                    ifcDialogBox.instance.ShowZeroButtonDialog(
                        LocalizacionManager.instance.GetTexto(96).ToUpper(),
                        LocalizacionManager.instance.GetTexto(97));
                    //ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.NONE, "Conectando...", "Por favor, espere.");

                } else if (m_pantallaAnterior == ifcCarrera.instance) {
                    // estoy jugando Single => jugar mision
                    Cortinilla.instance.Play();
                }
            };
        }

        // escudo multiplicador de puntuacion
        if (m_imgEscudo == null)
            m_imgEscudo = transform.FindChild("escudo").GetComponent<GUITexture>();
        if (m_txtMultiplicador == null)
            m_txtMultiplicador = transform.FindChild("escudo/txtMultiplicador").GetComponent<GUIText>();
    }


    // Use this for initialization
    void Start () {
        // Obtiene las referencias a los elementos de esta interfaz
        GetReferencias();

        // refrescar la informacion de los controles
        //RefreshInfo();

        // escudo multiplicador de puntuacion
        //RefreshMultiplicadorPuntuacion();

        // mostrar la pagina
        //ShowAs(m_tipoVestuario);
    }


    void Update() {
        // el boton de "INFO" unicamente se muestra si esta interfaz esta en su posicion de reposo
        // m_btnInfo.transform.gameObject.SetActive(transform.position == POSICION_REPOSO);
    }


    public void PedirAlias(string _aliasActual = "")
    {
        ifcDialogBox.instance.ShowOneButtonDialog(
            ifcDialogBox.OneButtonType.POSITIVE, 
            LocalizacionManager.instance.GetTexto(283).ToUpper(),
            LocalizacionManager.instance.GetTexto(284).ToUpper(),
            LocalizacionManager.instance.GetTexto(45).ToUpper(),
            (_name) => {
                if(ifcDialogBox.instance.textoEditado.Length < 6)
                {
                    ifcDialogBox.instance.ShowOneButtonDialog(
                        ifcDialogBox.OneButtonType.POSITIVE, 
                        LocalizacionManager.instance.GetTexto(283).ToUpper(),
                        LocalizacionManager.instance.GetTexto(285).ToUpper(),
                        LocalizacionManager.instance.GetTexto(45).ToUpper(),
                        (_name2) => {PedirAlias(ifcDialogBox.instance.textoEditado);}
                    );
                }
                else
                {
                    PersistenciaManager.instance.SaveAlias(ifcDialogBox.instance.textoEditado);
                    transform.FindChild("btnListo").GetComponent<btnButton>().action("");
                }
            }, 
            true
        );
        ifcDialogBox.instance.ShowTextInput(_aliasActual);
    }


    /// <summary>
    /// Actualiza o refresca la equipacion seleccionado
    /// </summary>
    /// <param name="_desplazamiento">Offset respecto al id de la equipacion actual que se quiere mostrar (si es 0, sirve para refrescar la equipacion actual)</param>
    /// <param name="_mostrarTooltip">Indica si hay que mostrar el tooltip asociado a la equipacion (si procede)</param>
    public void CambiarEquipacionSeleccionada(int _desplazamiento, bool _mostrarTooltip = false) {
        Equipacion equipacion;
        if (m_tipoVestuario == TipoVestuario.LANZADOR) {
            // verificar que el LANZADOR seleccionado este adquirido (sino cambiarlo por otro que si lo este)
            Jugador tirador = InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower);
            if (tirador.estado != Jugador.Estado.ADQUIRIDO) {
                Interfaz.instance.Thrower = InfoJugadores.instance.GetPosicionSiguienteTiradorAdquirido();
                //Interfaz.instance.Thrower = Interfaz.instance.Thrower; // <= actualiza la equipacion del lanzador
            }

            equipacion = EquipacionManager.instance.CambiarEquipacionLanzador(_desplazamiento);
        } else {
            // verificar que el PORTERO seleccionado este adquirido (sino cambiarlo por otro que si lo este)
            Jugador portero = InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper);
            if (portero.estado != Jugador.Estado.ADQUIRIDO) {
                Interfaz.instance.Goalkeeper = InfoJugadores.instance.GetPosicionSiguienteTiradorAdquirido();
                //Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper; // <= actualiza la equipacion del portero
            }

            equipacion = EquipacionManager.instance.CambiarEquipacionPortero(_desplazamiento);
        }

        // mostrar el tooltip si procede
        if (_mostrarTooltip)
            m_tooltipItemDisponible.Show(equipacion);
    }

    ///<summary>
    /// Actualiza o refresca el jugador seleccionado
    /// </summary>
    /// <param name="_desplazamiento">Offset respecto al id del jugador actual del jugador que se quiere mostrar (si es 0, sirve para refrescar el jugador actual)</param>
    /// <param name="_mostrarTooltip">Indica si hay que mostrar el tooltip asociado al jugador (si procede)</param>
    public void CambiarJugadorSeleccionado(int _desplazamiento, bool _mostrarTooltip = false) {
        Jugador jugador;
        if (m_tipoVestuario == TipoVestuario.LANZADOR) {
            // comprobar si la equipacion de LANZADOR seleccionada ha sido ADQUIRIDA
            Equipacion equipacionLanzador = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada();
            if (equipacionLanzador.estado != Equipacion.Estado.ADQUIRIDA) {
                EquipacionManager.instance.CambiarASiguienteEquipacionLanzadorAdquirida();
            }

            Interfaz.instance.Thrower += _desplazamiento;
            jugador = InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower);
            m_tooltipItemDisponible.Show(jugador);
        } else {
            // comprobar si la equipacion de PORTERO seleccionada ha sido ADQUIRIDA
            Equipacion equipacionPortero = EquipacionManager.instance.GetEquipacionPorteroSeleccionada();
            if (equipacionPortero.estado != Equipacion.Estado.ADQUIRIDA) {
                EquipacionManager.instance.CambiarASiguienteEquipacionPorteroAdquirida();
            }

            Interfaz.instance.Goalkeeper += _desplazamiento;
            jugador = InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper);
            m_tooltipItemDisponible.Show(jugador);
        }

        //mostrar el tooltip si procede
        if(_mostrarTooltip)
            m_tooltipItemDisponible.Show(jugador);
    }


    /// <summary>
    /// Actualiza el multiplicador de puntuacion
    /// </summary>
    public void RefreshMultiplicadorPuntuacion() {
        m_imgEscudo.texture = AvataresManager.instance.GetTexturaEscudo(EscudosManager.escudoEquipado.idTextura);
        m_txtMultiplicador.text = EscudosManager.escudoEquipado.boost.ToString("f1");
    }


    /// <summary>
    /// Actualiza los controles del vestuario para mostrarlo en el modo que corresponda
    /// </summary>
    /// <param name="_tipoVestuario"></param>
    public void ShowAs(TipoVestuario _tipoVestuario) {
        // Obtiene las referencias a los elementos de esta interfaz
        GetReferencias();
        // FPA (04/01/17): Eliminado GameAnalitics de momento. 
        // GA.API.Design.NewEvent("EntradaTienda:"+(_tipoVestuario == TipoVestuario.LANZADOR ? "Lanzador":"Portero"), Interfaz.ultimaMisionDesbloqueada, new Vector3(Interfaz.MonedasSoft, Interfaz.MonedasHard, 0f));

        Debug.Log(">>> START: m_tipoVestuario=" + m_tipoVestuario);

        m_tipoVestuario = _tipoVestuario;

        // destacar el boton del modo seleccionado
        m_btnLanzador.Deselect();
        m_btnPortero.Deselect();
        if (_tipoVestuario == TipoVestuario.LANZADOR)
            m_btnLanzador.Select();
        else
            m_btnPortero.Select();

        // mostrar / ocultar los controles
        m_cntCompraEscudos.gameObject.SetActive(true);
        m_cntCompraPowerUpsLanzador.gameObject.SetActive(m_tipoVestuario == TipoVestuario.LANZADOR);
        m_cntCompraPowerUpsPortero.gameObject.SetActive(m_tipoVestuario == TipoVestuario.PORTERO);

        // refrescar la informacion de los controles
        RefreshInfo();

        // escudo multiplicador de puntuacion
        RefreshMultiplicadorPuntuacion();

        // mostrar un tooltip sobre el jugador si procede
        if (_tipoVestuario == TipoVestuario.LANZADOR)
            m_tooltipItemDisponible.Show(InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower));
        else
            m_tooltipItemDisponible.Show(InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper));
    }


    /// <summary>
    /// comprueba si los jugadores seleccionados han sido adquiridos y si no, selecciona otros que si que lo esten
    /// </summary>
    /// <param name="_forzarRefresco">fuerza el refresco de los elementos de esta interfaz</param>
    public void ComprobarJugadoresYEquipacionesAdquiridos(bool _forzarRefresco = false) {

        bool actualizarLanzador = false;
        bool actualizadaEquipacionLanzador = false;
        bool actualizarPortero = false;
        bool actualizadaEquipacionPortero = false;

        // comprobar si el jugador TIRADOR seleccionado no ha sido ADQUIRIDO => buscar el siguiente jugador ADQUIRIDO
        Jugador tirador = InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower);
        if (tirador.estado != Jugador.Estado.ADQUIRIDO)
            actualizarLanzador = true;

        // comprobar si el jugador PORTERO seleccionado no ha sido ADQUIRIDO => buscar el siguiente jugador ADQUIRIDO
        Jugador portero = InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper);
        if (portero.estado != Jugador.Estado.ADQUIRIDO)
            actualizarPortero = true;

        // comprobar si la equipacion de lanzador seleccionada ha sido ADQUIRIDA
        Equipacion equipacionLanzador = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada();
        if (equipacionLanzador.estado != Equipacion.Estado.ADQUIRIDA) {
            //EquipacionManager.instance.CambiarASiguienteEquipacionLanzadorAdquirida(); //esto podria ya no ser necesario
            actualizadaEquipacionLanzador = true;
        }

        // comprobar si la equipacion de lanzador seleccionada ha sido ADQUIRIDA
        Equipacion equipacionPortero = EquipacionManager.instance.GetEquipacionPorteroSeleccionada();
        if (equipacionPortero.estado != Equipacion.Estado.ADQUIRIDA) {
            //EquipacionManager.instance.CambiarEquipacionPortero(); //esto podria ya no ser necesario
            actualizadaEquipacionPortero = true;
        }

        // actualizar el modelo de lanzador
        if (actualizarLanzador)
            Interfaz.instance.Thrower = InfoJugadores.instance.GetPosicionSiguienteTiradorAdquirido();
        else if (actualizadaEquipacionLanzador)
            Interfaz.instance.Thrower = Interfaz.instance.Thrower;
        
        // actualizar el modelo de portero
        if (actualizarPortero)
            Interfaz.instance.Goalkeeper = InfoJugadores.instance.GetPosicionSiguientePorteroAdquirido();
        else if (actualizadaEquipacionPortero)
            Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper;

        // comprobar si hay que actualizar la info de la interfaz
        if (_forzarRefresco || actualizarLanzador || actualizarPortero || actualizadaEquipacionLanzador || actualizadaEquipacionPortero)
            RefreshInfo();
    }

    /*
    /// <summary>
    /// Garantiza que el jugador seleccionado esta adquirido (y si no lo cambia por uno que si que lo este)
    /// </summary>
    /// <param name="_forzarRefresco">fuerza el refresco de los elementos de esta interfaz</param>
    public void GarantizarJugadoresSeleccionadosAdquiridos(bool _forzarRefresco = false) {
        bool refrescarInfo = false;

        // comprobar si el jugador TIRADOR seleccionado no ha sido ADQUIRIDO => buscar el siguiente jugador ADQUIRIDO
        Jugador tirador = InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower);
        if (tirador.estado != Jugador.Estado.ADQUIRIDO) {
            Interfaz.instance.Thrower = InfoJugadores.instance.GetPosicionSiguienteTiradorAdquirido();
            Interfaz.instance.Thrower = Interfaz.instance.Thrower; // <= actualiza la equipacion del lanzador
            refrescarInfo = true;
        }

        // comprobar si el jugador PORTERO seleccionado no ha sido ADQUIRIDO => buscar el siguiente jugador ADQUIRIDO
        Jugador portero = InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper);
        if (portero.estado != Jugador.Estado.ADQUIRIDO) {
            Interfaz.instance.Goalkeeper = InfoJugadores.instance.GetPosicionSiguientePorteroAdquirido();
            Interfaz.instance.Goalkeeper = Interfaz.instance.Goalkeeper; // <= actualiza la equipacion del portero
            refrescarInfo = true;
        }

        if (_forzarRefresco || refrescarInfo)
            RefreshInfo();
    }


    /// <summary>
    /// Garantiza que la equipacion seleccionada esta adquirida (y si no la cambia por una que si que lo este)
    /// </summary>
    /// <param name="_forzarRefresco">fuerza el refresco de los elementos de esta interfaz</param>
    public void GarantizarEquipacionSeleccionadAdquirida(bool _forzarRefresco = false) {
        bool refrescarInfo = false;

        // comprobar si la equipacion de lanzador seleccionada ha sido ADQUIRIDA
        Equipacion equipacionLanzador = EquipacionManager.instance.GetEquipacionLanzadorSeleccionada();
        if (equipacionLanzador.estado != Equipacion.Estado.ADQUIRIDA) {
            EquipacionManager.instance.CambiarASiguienteEquipacionLanzadorAdquirida();
            refrescarInfo = true;
        }

        // comprobar si la equipacion de lanzador seleccionada ha sido ADQUIRIDA
        Equipacion equipacionPortero = EquipacionManager.instance.GetEquipacionPorteroSeleccionada();
        if (equipacionPortero.estado != Equipacion.Estado.ADQUIRIDA) {
            EquipacionManager.instance.CambiarASiguienteEquipacionPorteroAdquirida();
            refrescarInfo = true;
        }

        if (_forzarRefresco || refrescarInfo)
            RefreshInfo();
    }
    */

    /// <summary>
    /// Refresca los controles para que actualicen los datos que estan mostrando
    /// </summary>
    public void RefreshInfo() {
        // Obtiene las referencias a los elementos de esta interfaz
        GetReferencias();

        // por defecto ocultar el tooltip de compra de items
        m_tooltipItemDisponible.gameObject.SetActive(false);

        // grupos de items a comprar
        m_cntCompraPowerUpsLanzador.Inicializar(cntCompraItemsContainer.TipoItem.POWER_UP_LANZADOR, new Vector2(-0.033f, 0.07f));
        m_cntCompraPowerUpsPortero.Inicializar(cntCompraItemsContainer.TipoItem.POWER_UP_PORTERO, new Vector2(-0.033f, 0.07f));
        m_cntCompraEscudos.Inicializar(cntCompraItemsContainer.TipoItem.ESCUDO, new Vector2(-0.033f, -0.03f));
    }


    /// <summary>
    /// Pantalla desde la que se llama a esta
    /// </summary>
    /// <param name="_pantalla"></param>
    public void SetPantallaBack(ifcBase _pantalla) {
        m_pantallaAnterior = _pantalla;
    }


    /// <summary>
    /// Volver a la pantalla anterior
    /// </summary>
    /// <param name="_name"></param>
    public void Back(string _name = "") {

        // al salir de esta pantalla verificar que los jugadores y las equipaciones seleccionadas estan adquiridas (sino cambiarlas)
        //ComprobarJugadoresYEquipacionesAdquiridos();
        GeneralSounds_menu.instance.back();

        // si se ha especificado la pantalla que ha llamado a esta => volver a esa pantalla
        if (m_pantallaAnterior != null) {
            ifcBase.activeIface = m_pantallaAnterior;
            // si la pantalla anterior es el menu principal
            if (m_pantallaAnterior == ifcMainMenu.instance) {
                // habilitar la pantalla "mainMenu"
                ifcMainMenu.instance.SetVisible(true);

                // mostrar los modelos del lanzador y del portero
                Interfaz.instance.RefrescarModelosJugadores(true, true, true);
                cntMenuDesplegableOpciones.instance.Plegar();
            }

            // si la pantalla anterior es la de seleccion de misiones
            if (m_pantallaAnterior == ifcCarrera.instance) {
                // habilitar la pantalla de seleccion de misiones
                ifcCarrera.instance.SetVisible(true);

                // comprobar que modelo de jugador hay que pintar
                Interfaz.instance.RefrescarModelosJugadores(ifcCarrera.instance.estoyMostrandoMisionDeLanzador, !ifcCarrera.instance.estoyMostrandoMisionDeLanzador, true);
            }

            new SuperTweener.move(gameObject, 0.25f, new Vector3(1.5f, 0.5f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
			//ADRIAN he duplicado y cambiado la frase de abajo poniendole las coordenadas de mi barra tambien he añadido otras dos frases para que los modelos vuelvan a aparecer
            //new SuperTweener.move(m_pantallaAnterior.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
			new SuperTweener.move(m_pantallaAnterior.gameObject, 0.25f, new Vector3(0.5f, 0.1f, 0.0f), SuperTweener.CubicOut);
			Interfaz.instance.RefrescarModelosJugadores(true, true, true);
			cntMenuDesplegableOpciones.instance.Plegar();
        }
    }


    /// <summary>
    /// Oculta el pop up de informacion asociado a la compra de jugadores y equipaciones
    /// </summary>
    public void HideTooltipInfoJugadorEquipacion() {
        m_tooltipItemDisponible.gameObject.SetActive(false);
    }


    /// <summary>
    /// Ir a la pantalla de duelo
    /// </summary>
    public void GoDuelo() {

        ifcMainMenu.goDuelo = false;

        // indicar un modo de juego por defecto => (NOTA: es importante no usar un TIME_ATTACK porque afecta al gameplay)
        GameplayService.modoJuego = InfoModosJuego.instance.GetModoJuego("SHOOTER_NORMAL_MODE");

        ifcBase.activeIface = ifcDuelo.instance;

        // indicar un modo de juego por defecto => (NOTA: es importante no usar un TIME_ATTACK porque afecta al gameplay)
        GameplayService.modoJuego = InfoModosJuego.instance.GetModoJuego("SHOOTER_NORMAL_MODE");

        //ocultar jugadores
        if (Interfaz.instance.goalkeeperModel != null)
            Interfaz.instance.goalkeeperModel.SetActive(false);
        if (Interfaz.instance.throwerModel != null)
            Interfaz.instance.throwerModel.SetActive(false);

        // mostrar la interfaz de duelo
        ifcDuelo.instance.SetVisible(true);
        cntBarraSuperior.instance.SetVisible(false);
        new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.0f, 0.5f, 0.0f), SuperTweener.CubicOut, (_name) => { SetVisible(false); });
        new SuperTweener.move(ifcDuelo.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => {
            Debug.Log("Conectando...");
            GameplayService.networked = true;
            Shark.instance.Conectar();
        });
    }


    /// <summary>
    /// Muestra / oculta el boton de info
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisibleInfoButton(bool _visible) {
        m_btnInfo.gameObject.SetActive(_visible);
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }

}
