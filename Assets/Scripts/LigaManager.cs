using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigaManager : MonoBehaviour {

    public static LigaManager instance;

    public Liga[] Ligas;
    public Liga CurrentLiga {
        get {
            return Ligas[CurrentLigaIndex-1];
        }
    }

    public Texture2D CurrentImageLiga {
        get {
            return currentImage2D;
        }
    }

    public GameObject currentModel3D;
    private Texture2D currentImage2D;

    private int CurrentLigaIndex = -1;

    /// <summary>
    /// Devuelve la liga correspondiente al skillLevel (teniendo en cuenta la liga actual)
    /// </summary>
    /// <returns>The liga.</returns>
    /// <param name="currentLiga">Current liga.</param>
    /// <param name="skillLevel">Skill level.</param>
    public int CalculateLiga(int currentLiga, int skillLevel) {
        int result = currentLiga;
        // Comprobar si en la liga actual estamos para "bajar"
        if (GetLiga(currentLiga).SkillLevelDown > skillLevel) {
            result = currentLiga - 1;
        }
        else if (currentLiga+1 <= Ligas.Length) {
            // Comprobar si en la liga siguiente estamos para "subir"
            if (GetLiga(currentLiga+1).SkillLevelUp <= skillLevel) {
                result = currentLiga + 1;
            }    
        }
        Debug.Log(string.Format("CalculateLiga: CurrentLiga: {0} SkillLevel: {1} => Liga: {2}", currentLiga, skillLevel, result));
        return result;
    }

    private Liga GetLiga(int value) {
        return Ligas[value-1];
    }

    void Awake () {
        instance = this;
        ChangeLiga( PlayerPrefs.GetInt("currentLiga", 1) );
    }

	void Start () {
	}
	
	void Update () {
	}

    public void ChangeLiga(int ligaIndex) {
        if (ligaIndex == CurrentLigaIndex) {
            return;
        }

        CurrentLigaIndex = ligaIndex;
        PlayerPrefs.SetInt("currentLiga", CurrentLigaIndex);
        PlayerPrefs.Save();

        if (currentModel3D != null) {
            GameObject.Destroy(currentModel3D);
        }

        Liga liga = GetLiga(CurrentLigaIndex);
        RenderSettings.ambientLight = liga.Light;
        RenderSettings.fogDensity = liga.FogDensity;
        RenderSettings.fogColor = liga.Fog;
        currentModel3D = GameObject.Instantiate(liga.Model3D) as GameObject;
        currentImage2D = liga.Image2D;
    }
}
