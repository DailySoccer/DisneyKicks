using UnityEngine;
using System.Collections;

public class ifcSplash : ifcBase
{
    public float m_spalshTime = 5.0f;
    public static ifcSplash instance { get; protected set; }
    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        /* 
        // cargar la pantalla de login del Bitoon Kicks
        if (Interfaz.m_firstTime) {
            new SuperTweener.none(gameObject, m_spalshTime, (_obj) => {
                new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.4f, 0.0f, 0.0f), SuperTweener.CubicOut, (_target) => { });
                new SuperTweener.move(ifcIdentificacion.instance.gameObject, 0.25f, Stats.POS_PANTALLA_VISIBLE, SuperTweener.CubicOut, (_target) => { });
            });
        }
         */

        if(Interfaz.m_firstTime)
        {
          new SuperTweener.none(gameObject, m_spalshTime, (_obj) => {
              new SuperTweener.move(gameObject, 0.25f, new Vector3(-1.4f, 0.0f, 0.0f), SuperTweener.CubicOut, (_target) => { gameObject.SetActive(false); });
              new SuperTweener.move(ifcMainMenu.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f),
                  SuperTweener.CubicOut, (_target) => {
                      // ocultar los elementos de interfaz
                      ifcMainMenu.instance.OcultarElementosDeInterfazNoVisibles();

                      // mostrar los logros pendientes que pudiera haber
                      DialogManager.instance.MostrarDialogosRegistradosPendientes(null);
                  });
          });
        }
        else
        {
          // mostrar los logros pendientes que pudiera haber
          DialogManager.instance.MostrarDialogosRegistradosPendientes(null);
          gameObject.transform.position = new Vector3(-1.4f, 0.0f, 0.0f);
          ifcMainMenu.instance.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
          //cntMenuDesplegableOpciones.instance.Show();
        }      
	}

    /*public void NoSplash()
    {
        gameObject.transform.position = new Vector3(-1.4f, 0.0f, 0.0f);
        transform.parent.Find("bitoon_logo").gameObject.guiTexture.color = Color.white;
        ifcMainMenu.instance.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }*/
}
