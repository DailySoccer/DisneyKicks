using UnityEngine;
using System.Collections;

/// <summary>
/// Pantalla para mostrar el menu de Single Player
/// </summary>
public class ifcSinglePlayer: ifcBase {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public Vector3 m_posicionVisible = new Vector3(1.0f, 0.0f, 0.0f);
    public Vector3 m_posicionHide = new Vector3(2.25f, 0.0f, 0.0f);

    public static ifcSinglePlayer instance { get; protected set; }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------


    void Awake() {
        instance = this;
    }


	// Use this for initialization
	void Start () {
        // ajustar las dimensiones y posiciones de los elementos de esta interfaz
        getComponentByName("Fondo").GetComponent<GUITexture>().pixelInset = Stats.FONDO_PIXEL_INSET;

        // Crear acciones asociadas a los botones de esta interfaz
        getComponentByName("btnAtras").GetComponent<btnButton>().action = (_name) => {
            Interfaz.ClickFX();
            new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
            new SuperTweener.move(ifcMainMenu.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        };
	}


	/*
	// Update is called once per frame
	void Update () {
	}
     */
}
