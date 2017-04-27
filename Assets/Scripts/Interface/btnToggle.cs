using UnityEngine;
using System.Collections;


/// <summary>
/// Boton que al ser presionado pasa a funcionar en modo B, y al volver a ser presionado vuelve a funcionar en modo A
/// Nota: en vez de asignar directamente el callback al "action" de este boton utilizar el metodo: "Init()"
/// </summary>
public class btnToggle : btnButton {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // indica el modo en el que esta trabajando este boton
    private bool m_estoyEnModoA = true;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Inicializa este control (define las acciones que tiene que realizar en cada uno de sus estados)
    /// </summary>
    /// <param name="_onClickOnModoA">Accion a realizar si se pulsa el boton en modo A</param>
    /// <param name="_onClickOnModoB">Accion a realizar si se pulsa el boton en modo B</param>
    public void Init(btnButton.guiAction _onClickOnModoA = null, btnButton.guiAction _onClickOnModoB = null) {
        this.action = (_name) => {
            // realizar una accion en funcion del modo en el que se encuentra actualmente el boton
            if (m_estoyEnModoA) {
                if (_onClickOnModoA != null)
                    _onClickOnModoA(_name);
            } else {
                if (_onClickOnModoB != null)
                    _onClickOnModoB(_name);
            }

            // cambiar el estado del boton a su contrario
            SetModoA(!m_estoyEnModoA);
        };
    }


    /// <summary>
    /// Indica si el boton se esta manteniendo mantener o no personado
    /// </summary>
    /// <param name="_estoyEnModoA"></param>
    /// <returns></returns>
    public void SetModoA(bool _estoyEnModoA) {
        m_estoyEnModoA = _estoyEnModoA;

        if (m_estoyEnModoA) {
            // deseleccionar el boton
            Deselect();
        } else {
            // resaltar el boton
            SetHover(true);
        }
    }


    protected override void OnMouseExit() {
        // si estoy en modoA el OnMouseExit se comporta de manera normal, en caso contrario no se hace nada
        if (m_estoyEnModoA)
            base.OnMouseExit();
    }



}
