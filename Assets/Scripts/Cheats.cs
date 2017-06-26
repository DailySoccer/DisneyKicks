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

    [Header("[** Jugadores **]")]
    public bool UnlockAllJugadores = false;
    public bool DeleteSavedJugadores = false;

    [Header("[** Equipaciones **]")]
    public bool UnlockAllEquipaciones = false;
    public bool DeleteSavedEquipaciones = false;

    [Header("[** Partido **]")]
    public bool GameOver = false;
    public int OwnerScore = 0;
    public int OpponentScore = 0;
    public int OwnerELO = -1;
    public int OpponentELOMod = 0;

    bool firstTime = true;

    void Awake () {
        if (_instance == null) {
            _instance = this;
        }

        if (DeleteSavedJugadores) {
            PlayerPrefs.DeleteKey("jugadores");
        }

        if (DeleteSavedEquipaciones) {
            PlayerPrefs.DeleteKey("equipaciones");
        }
    }

    public void Unlock() {
        if (UnlockAllJugadores) {
            InfoJugadores.instance.CHEAT_ChangeAllToState( Jugador.Estado.ADQUIRIDO );
        }

        if (UnlockAllEquipaciones) {
            EquipacionManager.instance.CHEAT_ChangeAllToState( Equipacion.Estado.ADQUIRIDA );
        }
    }

	void Start () {
    }
	
	void Update () {
        if (firstTime) {
            Unlock();
            firstTime = false;
        }
	}
}
