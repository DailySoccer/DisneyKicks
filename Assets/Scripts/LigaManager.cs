using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigaManager : MonoBehaviour {

    public static LigaManager instance;

    public List<Liga> Ligas;

    /// <summary>
    /// Devuelve la liga correspondiente al skillLevel (teniendo en cuenta la liga actual)
    /// </summary>
    /// <returns>The liga.</returns>
    /// <param name="currentLiga">Current liga.</param>
    /// <param name="skillLevel">Skill level.</param>
    public int CalculateLiga(int currentLiga, int skillLevel) {
        int result = currentLiga;

        // Comprobar si bajamos de liga
        while (result > 0 && Ligas[result].SkillLevelDown > skillLevel) {
            result--;
        }
        // Comprobar si subimos de liga
        while (result+1 < Ligas.Count && Ligas[result+1].SkillLevelUp <= skillLevel) {
            result++;
        }

        Debug.Log(string.Format("CalculateLiga: CurrentLiga: {0} SkillLevel: {1} => Liga: {2}", currentLiga, skillLevel, result));
        return result;
    }

    void Awake () {
        if (instance == null) {
            instance = this;
        }

        Ligas = new List<Liga>();
        Ligas.AddRange( LigaData.SkillLevels );
    }

	void Start () {
	}
	
	void Update () {
	}
}
