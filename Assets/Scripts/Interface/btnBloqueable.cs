using UnityEngine;
using System.Collections;


/// <summary>
/// Boton que tiene dos modos de funcionamiento; como boton normal y bloqueado
/// </summary>
public class btnBloqueable : btnButton {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Textura a mostrar sobre este boton cuando esta bloqueado el boton esta bloqueado
    /// </summary>
    private GUITexture m_iconoBotonBloqueado;
    protected GUITexture _iconBlockedButton {
        get {
            // obtener la referencia a la textura
            if ( m_iconoBotonBloqueado == null ) {
                m_iconoBotonBloqueado = transform.Find( "IconoBotonBloqueado" ).GetComponent<GUITexture>();
            }

            return m_iconoBotonBloqueado;
        }
    }

    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------

    private void GetLockIcon () {
    }

    /// <summary>
    /// Metodo para modificar el estado de bloqueo de este boton
    /// </summary>
    /// <param name="_locked">True para bloquear el boton</param>
    public void SetLock(bool _locked) {
        // mostrar / ocultar la imagen para bloquear el boton
        _iconBlockedButton.gameObject.SetActive( _locked );
    }

    override public void SetEnabled (bool _show = true) {
        base.SetEnabled( _show );

        float a = _show ? 0.5f : 0.3f;

        Color tmp;
        GameObject obj;

        Transform t = _iconBlockedButton.transform;
        if ( t ) { obj = t.gameObject; tmp = obj.GetComponent<GUITexture>().color; tmp.a = a; obj.GetComponent<GUITexture>().color = tmp; }
    }


}
