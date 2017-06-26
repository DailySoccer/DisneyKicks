using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigaManager : MonoBehaviour {

    public static LigaManager instance;

    public Liga[] Ligas;

    /// <summary>
    /// Devuelve la liga correspondiente al skillLevel (teniendo en cuenta la liga actual)
    /// </summary>
    /// <returns>The liga.</returns>
    /// <param name="currentLiga">Current liga.</param>
    /// <param name="skillLevel">Skill level.</param>
    public int CalculateLiga(int currentLiga, int skillLevel) {
        int result = currentLiga;
        // Comprobar si en la liga actual estamos para "bajar"
        if (Ligas[currentLiga].SkillLevelDown > skillLevel) {
            result = currentLiga - 1;
        }
        else if (currentLiga+1 < Ligas.Length) {
            // Comprobar si en la liga siguiente estamos para "subir"
            if (Ligas[currentLiga+1].SkillLevelUp <= skillLevel) {
                result = currentLiga + 1;
            }    
        }
        Debug.Log(string.Format("CalculateLiga: CurrentLiga: {0} SkillLevel: {1} => Liga: {2}", currentLiga, skillLevel, result));
        return result;
    }

    void Awake () {
        if (instance == null) {
            instance = this;
        }
    }

	void Start () {
	}
	
	void Update () {
	}
}
