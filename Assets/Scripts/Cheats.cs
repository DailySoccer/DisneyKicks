using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour {

    static public Cheats Instance {
        get {
            return _instance;
        }
    }
    static private Cheats _instance;

    [Header("Jugadores")]
    public bool UnlockAllJugadores = false;
    public bool DeleteSavedJugadores = false;

    void Awake () {
        if (_instance == null) {
            _instance = this;
        }
    }

	void Start () {
        InfoJugadores.instance.CHEAT_ChangeAllToState( UnlockAllJugadores ? Jugador.Estado.ADQUIRIDO : Jugador.Estado.BLOQUEADO );

        if (DeleteSavedJugadores) {
            PlayerPrefs.DeleteKey("jugadores");
        }
	}
	
	void Update () {
	}
}
