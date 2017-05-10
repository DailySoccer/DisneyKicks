﻿//#define DEBUG_MULTI

using UnityEngine;
using System.Collections;

public class ifcMainMenu : ifcBase {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static ifcMainMenu instance { get; protected set; }

    // para indicar si hay que saltar de esta interfaz a carrera (singleplayer) o a duelo (multiplayer)
    public static bool goCarrera { set { m_goCarrera = value; if (value) m_showCarrera = value; } get { return m_goCarrera; } }
    private static bool m_goCarrera = false;
    public static bool goDuelo { set { m_goDuelo = value; if (value) m_showDuelo = value; } get { return m_goDuelo; } }
    private static bool m_goDuelo = false;

    // variables para indicar que hay que pintar una determinada interfaz
    private static bool m_showCarrera = false;
    private static bool m_showDuelo = false;

    // elementos graficos de esta interfaz
    private btnButton m_btnOpciones;
    private btnButton m_btnVelo;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------

    void Awake() {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
        ifcBase.activeIface = this;

        GameplayService.networked = false;

        /*
        // boton para Facebook 
        transform.FindChild("btnFacebook").GetComponent<btnButton>().action = (_name) => {
            Debug.Log(">>> Has pulsado en el boton de Facebook");
        };
         */

        // botones duelo y carrera
        transform.FindChild("botones/btnCarrera").GetComponent<btnButton>().action = OnCarrera;
        transform.FindChild("botones/btnDuelo").GetComponent<btnButton>().action = OnDuelo;


        // boton para mostrar las opciones
        m_btnOpciones = transform.FindChild("btnOpciones").GetComponent<btnButton>();
        m_btnOpciones.action = (_name) => {
            Interfaz.ClickFX();

            // mostrar el menu de opciones y el velo
            m_btnOpciones.gameObject.SetActive(false);
            cntMenuDesplegableOpciones.instance.Desplegar();
            m_btnVelo.gameObject.SetActive(true);
        };

        // velo (que se muestra cuando aparecen las opciones y que funciona como boton de plegar)
        m_btnVelo = transform.FindChild("btnVelo").GetComponent<btnButton>();
        m_btnVelo.action = (_name) => {
            Interfaz.ClickFX();

            // ocultar el menu de opciones y el velo
            cntMenuDesplegableOpciones.instance.Plegar();
            m_btnVelo.gameObject.SetActive(false);
        };
        m_btnVelo.gameObject.SetActive(false);  // <= ocultar por defecto el velo

        // esto no es hack, el otro jugador (IA o persona) tiene siempre el inventario falso
        PowerupService.rivalInventory = new PowerupInventory(true);
    }


    void Update()
    {
        if(Input.GetKeyUp("escape"))
        {
            if(ifcBase.activeIface == this)
            {
                Application.Quit();
            }
            else
            {
                ifcBase.activeIface.m_backMethod("");
            }
        }
    }


    void LateUpdate()
    {
        if(m_goCarrera)
        {
            OnCarrera(null);
        }
        if(m_goDuelo)
        {
            OnDuelo(null);
            ifcVestuario.instance.GoDuelo();
        }
    }

   
    private void OnDuelo (string _name) {
        ifcBase.activeIface = ifcVestuario.instance;
        Interfaz.ClickFX();

        ifcVestuario.instance.SetPantallaBack(this);
        
        // mostrar por defecto el vestuario en modo lanzador
        ifcVestuario.instance.SetVisible(true);
        Interfaz.instance.RefrescarModelosJugadores(true, false);
        ifcVestuario.instance.ShowAs(ifcVestuario.instance.m_tipoVestuario);

        // ocultar esta interfaz y mostrar el vestuario
        new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
        new SuperTweener.move(ifcVestuario.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut, (_target) => { });

        // establecer el valor de recompena acumulada en la partida a "0"
        Interfaz.recompensaAcumulada = 0;

        Interfaz.instance.CleanPlayers();
    }


    private void OnCarrera (string _name) {
        ifcCarrera.instance.SetVisible(true);
        ifcBase.activeIface = ifcCarrera.instance;
        m_goCarrera = false;
        Interfaz.ClickFX();
        Interfaz.instance.CleanPlayers();

        new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_name2) => { SetVisible(false); });
        new SuperTweener.move( ifcCarrera.instance.gameObject, 0.25f, new Vector3( 0.0f, 0.0f, 0.0f ), SuperTweener.CubicOut, (_target) => { } );

        // hacer que se muestre la ultima mision desbloqueada del modo carrera
        ifcCarrera.instance.MostrarUltimaMision();

        // establecer el valor de recompena acumulada en la partida a "0"
        Interfaz.recompensaAcumulada = 0;
    }


    /// <summary>
    /// Muestra / oculta el boton de opciones en funcion del valor del parametro "_visible"
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisibleBotonOpciones(bool _visible) {
        m_btnOpciones.gameObject.SetActive(_visible);
        m_btnVelo.gameObject.SetActive(!_visible);
    }


    /// <summary>
    /// Muestra / oculta esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        gameObject.SetActive(_visible);
    }


    /// <summary>
    /// Oculta los elementos de interfaz que no deberian estar visibles (para optimizar el rendimiento de la aplicacion)
    /// </summary>
    public void OcultarElementosDeInterfazNoVisibles() {
        // pantallas
        if (ifcPerfil.instance != null)
            ifcPerfil.instance.SetVisible(false);
        if (ifcLogros.instance != null)
            ifcLogros.instance.SetVisible(false);
        if (ifcVestuario.instance != null)
            ifcVestuario.instance.SetVisible(false);
        if (ifcCarrera.instance != null)
            ifcCarrera.instance.SetVisible(m_showCarrera);
        if (ifcDuelo.instance != null)
            ifcDuelo.instance.SetVisible(m_showDuelo);

        // controles
        if (cntMenuDesplegableOpciones.instance != null)
            cntMenuDesplegableOpciones.instance.SetVisible(false);

        // comprobar si hay que ocultar esta pantalla
        if (m_showCarrera || m_showDuelo) {
            if (m_showCarrera) {
                ifcBase.activeIface = ifcCarrera.instance;
                m_showCarrera = false;
            }
            if (m_showDuelo) {
                ifcBase.activeIface = ifcDuelo.instance;
                m_showDuelo = false;
                cntBarraSuperior.instance.SetVisible(false);
            }

            SetVisible(false);
        } else {
            // indicar que esta es la pantalla activa y mostrarla
            ifcBase.activeIface = ifcMainMenu.instance;
            SetVisible(true);
        }
    }

}