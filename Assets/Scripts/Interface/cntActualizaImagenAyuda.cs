using UnityEngine;
using System.Collections;


/// <summary>
/// Actualiza la imagen de esta pantalla de ayuda en funcion del dispositivo sobre el que se ejecuta la aplicacion
/// </summary>
public class cntActualizaImagenAyuda: MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------

    /// <summary>
    /// Textura si el control es tactil
    /// NOTA: asignar valor a esta propiedad desde la interfaz de Unity
    /// </summary>
    public Texture m_texturaControlTactil;

    /// <summary>
    /// Textura si el control es con el raton
    /// NOTA: asignar valor a esta propiedad desde la interfaz de Unity
    /// </summary>
    public Texture m_texturaControlRaton;

    /// <summary>
    /// Texto a amostrar en caso de que el control sea tactil
    /// NOTA: asignar valor a esta propiedad desde la interfaz de Unity
    /// </summary>
    public string m_textoControlTactil;

    /// <summary>
    /// Texto a amostrar en caso de que el control sea con el raton
    /// NOTA: asignar valor a esta propiedad desde la interfaz de Unity
    /// </summary>
    public string m_textoControlRaton;

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    // Use this for initialization
    void Start() {
        // asignar para el paso 2 de tirador en funcion de la plataforma sobre la que se ejecuta la aplicacion
#if UNITY_WEBPLAYER
        // textura manejando con el raton
        transform.GetComponent<GUITexture>().texture = m_texturaControlRaton;
        transform.FindChild("Cuerpo").GetComponent<GUIText>().text = m_textoControlRaton;
#else
        // textura manejando con el dedo
        transform.GetComponent<GUITexture>().texture = m_texturaControlTactil;
        transform.FindChild("Cuerpo").GetComponent<GUIText>().text = m_textoControlTactil;
#endif

    }

}
