using UnityEngine;
using System.Collections;

public class stadium_control : MonoBehaviour
{
    public GameObject estadioModel;
    public GameObject[] estadioModels;
    public Color[] fogs;
    public Color[] lights;
    public float[] fogDensity;
    public static int estadioIndex = 0;
    public static stadium_control instance;

    void Awake()
    {
        instance = this;
        estadioIndex = PlayerPrefs.GetInt("estadio", 0);
        SetScenario();
    }

    public void SetScenario()
    {
        PlayerPrefs.SetInt("estadio", estadioIndex);
        PlayerPrefs.Save();
        GameObject.Destroy(estadioModel);
        RenderSettings.ambientLight = lights[estadioIndex];
        RenderSettings.fogDensity = fogDensity[estadioIndex];
        RenderSettings.fogColor = fogs[estadioIndex];
        estadioModel = GameObject.Instantiate(estadioModels[estadioIndex]) as GameObject;
    }
}
