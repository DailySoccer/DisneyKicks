using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ifcBottomBar : ifcBase {


	//SINGLETON
	public static ifcBottomBar instance { get; protected set; }



	//BOTONES
	private btnButton m_btnTienda;
	private btnButton m_btnEquipo;
	private btnButton m_btnHome;
	private btnButton m_btnPerfil;
	private btnButton m_btnRanking;


	//CASO DE PANTALLA
	public int NumPantalla;
	public ifcBase m_pantallaAnterior;

	void Awake() {
		instance = this;
	}

	void Start () {

		m_btnTienda = transform.FindChild("btnTienda").GetComponent<btnButton>();
		m_btnEquipo = transform.FindChild("btnEquipo").GetComponent<btnButton>();
		m_btnHome = transform.FindChild("btnHome").GetComponent<btnButton>();
		m_btnPerfil = transform.FindChild("btnPerfil").GetComponent<btnButton>();
		m_btnRanking = transform.FindChild("btnRanking").GetComponent<btnButton>();

		NumPantalla = 2;
		m_btnHome.Select ();
	}
	
	// Update is called once per frame
	void Update () {

		//ACION DEL BOTON TIENDA
		m_btnTienda.action = (_name) => {

			Debug.Log("VOY A TIENDA");
			OcultarEscenaAnterior();
			NumPantalla = 0;
			MostrarEscenaNueva();
		};


		//ACCION DEL BOTON EQUIPO/VESTUARIO
		m_btnEquipo.action = (_name) => {

			Debug.Log("VOY A EQUIPO");
			OcultarEscenaAnterior();
			NumPantalla = 1;
			MostrarEscenaNueva();

		};


		//ACCION DEL BOTON JUGAR
		m_btnHome.action = (_name) => {

			Debug.Log("VOY A HOME/JUGAR");
			OcultarEscenaAnterior();
			NumPantalla = 2;
			MostrarEscenaNueva();
		};

		//ACCION DEL BOTON PERFIL
		m_btnPerfil.action = (_name) => {

			Debug.Log("VOY A PERFIL");
			OcultarEscenaAnterior();
			NumPantalla = 3;
			MostrarEscenaNueva();
		};

		//ACCION DEL BOTON RANKING
		m_btnRanking.action = (_name) => {

			Debug.Log("VOY A RANKING");
			OcultarEscenaAnterior();
			NumPantalla = 4;
			MostrarEscenaNueva();
		};
		
	}

	public void MostrarEscenaNueva()
	{
		switch (NumPantalla) 
		{
		case 0:
			Debug.Log ("Estoy en TIENDA");
			m_btnTienda.Select ();
		break;

		case 1:
			Debug.Log ("Estoy en EQUIPO");
			m_btnEquipo.Select ();
			ifcBase.activeIface = ifcVestuario.instance;
			Interfaz.ClickFX();
			ifcVestuario.instance.SetPantallaBack(this);
			ifcVestuario.instance.SetVisible(true);
			Interfaz.instance.RefrescarModelosJugadores(true, false);
			ifcVestuario.instance.ShowAs(ifcVestuario.instance.m_tipoVestuario);
			new SuperTweener.move(ifcVestuario.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut, (_target) => { });
			Interfaz.recompensaAcumulada = 0;
			Interfaz.instance.CleanPlayers();

			break;
		case 2:
			Debug.Log ("Estoy en HOME");
			m_btnHome.Select ();
			new SuperTweener.move (ifcMainMenu.instance.gameObject, 0.25f, new Vector3 (0f, 0f, 0.0f), SuperTweener.CubicOut, (_target) => {
			});
			//NUEVO
			// al salir de esta pantalla verificar que los jugadores y las equipaciones seleccionadas estan adquiridas (sino cambiarlas)
			//ComprobarJugadoresYEquipacionesAdquiridos();
			GeneralSounds_menu.instance.back ();
			m_pantallaAnterior = ifcMainMenu.instance;
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
			}
			break;
		case 3:
			Debug.Log ("Estoy en PLAYER");
			new SuperTweener.move (ifcPerfil.instance.gameObject, 0.25f, new Vector3 (0f, 0f, 0.0f), SuperTweener.CubicOut, (_target) => {
			});
			m_btnPerfil.Select ();
			// si ya se esta mostrando la pantalla de perfil => no hacer nada
			if (ifcBase.activeIface == ifcPerfil.instance)
				return;

			ifcPerfil.instance.SetVisible(true);
			cntBarraSuperior.instance.ShowPantalla(ifcPerfil.instance, Vector3.zero);

			// actualizar la pantalla de perfil
			ifcPerfil.instance.Refresh();
			break;
		case 4:
			Debug.Log ("Estoy en LEADERBOARD");
			m_btnRanking.Select ();
			break;
		}
	}

	public void OcultarEscenaAnterior()
	{
		switch (NumPantalla) 
		{
		case 0:
			Debug.Log ("Oculto TIENDA");
			m_btnTienda.Deselect ();
			break;

		case 1:
			Debug.Log ("Oculto EQUIPO");
			m_btnEquipo.Deselect ();
			GeneralSounds_menu.instance.back();
			new SuperTweener.move(ifcVestuario.instance.gameObject, 0.25f, new Vector3(1.5f, 0.5f, 0.0f), SuperTweener.CubicOut);
			break;
		case 2:
			Debug.Log ("Oculto HOME");
			m_btnHome.Deselect ();
			new SuperTweener.move(ifcMainMenu.instance.gameObject, 0.25f, new Vector3(1.5f, 0f, 0.0f), SuperTweener.CubicOut);
			break;
		case 3:
			Debug.Log ("Oculto PLAYER");
			m_btnPerfil.Deselect ();
			new SuperTweener.move (ifcPerfil.instance.gameObject, 0.25f, new Vector3 (3f, 0f, 0.0f), SuperTweener.CubicOut, (_target) => {
			});
			break;
		case 4:
			Debug.Log ("Oculto LEADERBOARD");
			m_btnRanking.Deselect ();
			break;
		}
	}
}
