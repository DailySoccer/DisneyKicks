using UnityEngine;
using System.Collections;

/// <summary>
/// Pantalla para mostrar las posibles opciones de identificacion del usuario
/// </summary>
public class ifcIdentificacion: ifcBase {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    public static ifcIdentificacion instance { get; protected set; }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        instance = this;
    }


	void Start () {
        // posicionar esta pantalla por defecto oculta por la dcha
        transform.position = Stats.POS_PANTALLA_OCULTA_DCHA;

        // ajustar las dimensiones y posiciones de los elementos de esta interfaz
        getComponentByName("Fondo").GetComponent<GUITexture>().pixelInset = Stats.FONDO_PIXEL_INSET;

	    // Crear acciones asociadas a los botones de esta interfaz
        getComponentByName("btnLoginFacebook").GetComponent<btnButton>().action = OnLoginFacebook;
        getComponentByName("btnLoginKicks").GetComponent<btnButton>().action = OnLoginKicks;
        getComponentByName("btnLoginInvitado").GetComponent<btnButton>().action = OnLoginInvitado;
	}
	

	/* 
    //Update is called once per frame
	void Update () {
	}
     */


    void OnLoginFacebook(string _name) {
        Debug.Log(">>> PULSO EN LOGIN FACEBOOK");
        Interfaz.ClickFX();
    }


    void OnLoginKicks(string _name) {
        Debug.Log(">>> PULSO EN LOGIN KICKS");
        Interfaz.ClickFX();
    }


    void OnLoginInvitado(string _name) {
        Debug.Log(">>> PULSO EN LOGIN INVITADO");
        Interfaz.ClickFX();

        // ocultar esta pantalla por la izquierda y mostrar la siguiente
        new SuperTweener.move(gameObject, 0.25f, Stats.POS_PANTALLA_OCULTA_IZDA, SuperTweener.CubicOut, (_target) => { });
        new SuperTweener.move(ifcMainMenu.instance.gameObject, 0.25f, Vector3.zero, SuperTweener.CubicOut, (_target) => { });
    }

}
