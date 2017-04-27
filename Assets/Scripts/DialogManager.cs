using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Clase que permite generar una cola de dialogos y mostrarlos todos cuando se le pida
/// </summary>
public class DialogManager {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static DialogManager instance { 
        get {
            if (m_instance == null)
                m_instance = new DialogManager();
            return m_instance; 
        } 
    }
    private static DialogManager m_instance;

    // cola para alamacenar los dialogos registrados
    private Queue<DialogDefinition> m_colaDialogos;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    public DialogManager() {
        m_colaDialogos = new Queue<DialogDefinition>();
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------
    

    /// <summary>
    /// Elimina todos los dialogos pendientes de ser mostrados
    /// </summary>
    public void ClearDialogosRegistrados() {
        m_colaDialogos.Clear();
    }


    /// <summary>
    /// Registra un dialogo a mostrar
    /// </summary>
    /// <param name="_dialogo"></param>
    public void RegistrarDialogo(DialogDefinition _dialogo) {
        if (_dialogo == null)
            return;
        else {
            m_colaDialogos.Enqueue(_dialogo);
        }
    }


    public delegate void Action();

    /// <summary>
    /// Muestra en cadena todos los dialogos registrados
    /// </summary>
    /// <param name="_onEndCallback">Accion a realizar cuando se muestren todos los dialogos</param>
    public void MostrarDialogosRegistradosPendientes(Action _onEndCallback) {
        Debug.Log(">>> MOSTRAR DIALOGOS PENDIENTES: " + m_colaDialogos.Count);

        if (m_colaDialogos.Count > 0) {
            // desencolar el primer dialogo y mostrarlo
            DialogDefinition dialogo = m_colaDialogos.Dequeue();
            MostrarDialogo(dialogo, _onEndCallback);
        } else {
            // realizar la accion tras haber mostrado todos los dialogos
            if (_onEndCallback != null)
                _onEndCallback();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_dialogo"></param>
    /// <param name="_onEndCallback"></param>
    private void MostrarDialogo(DialogDefinition _dialogo, Action _onEndCallback) {
        if (_dialogo == null)
            return;
        else {
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.logroDesbloqueadoClip);
            switch (_dialogo.tipoDialogo) {
                case DialogDefinition.TipoDialogo.JUGADOR_DESBLOQUEADO:
                    ifcItemRevealDialog.instance.ShowJugadorDesbloqueado((Jugador) _dialogo.parametro, (_name) => { MostrarDialogosRegistradosPendientes(_onEndCallback); });
                    break;

                case DialogDefinition.TipoDialogo.EQUIPACION_DESBLOQUEADA:
                    ifcItemRevealDialog.instance.ShowEquipacionDesbloqueada((Equipacion) _dialogo.parametro, (_name) => { MostrarDialogosRegistradosPendientes(_onEndCallback); });
                    break;

                case DialogDefinition.TipoDialogo.LOGRO_DESBLOQUEADO:
                    ifcItemRevealDialog.instance.ShowLogroDesbloqueado((string) _dialogo.parametro, (_name) => { MostrarDialogosRegistradosPendientes(_onEndCallback); });
                    break;

                case DialogDefinition.TipoDialogo.ESCUDO_DESBLOQUEADO:
                    ifcItemRevealDialog.instance.ShowEscudoDesbloqueado((Escudo) _dialogo.parametro, (_name) => { MostrarDialogosRegistradosPendientes(_onEndCallback); });
                    break;
            }
        }
    }

}


/// <summary>
/// Clase para almacenar la definicion de un dialogo a mostrar
/// </summary>
public class DialogDefinition {

    // -----------------------------------------------------------------------------
    // ---  ENUMERADOS  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public enum TipoDialogo { 
        EQUIPACION_DESBLOQUEADA, 
        JUGADOR_DESBLOQUEADO, 
        LOGRO_DESBLOQUEADO,
        ESCUDO_DESBLOQUEADO
    };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    public TipoDialogo tipoDialogo { get { return m_tipoDialogo; } }
    private TipoDialogo m_tipoDialogo;

    /// <summary>
    /// Parametro que admite el dialogo en su llamada
    /// </summary>
    public System.Object parametro { get { return m_parametro; } }
    private System.Object m_parametro;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Constructor de la clase
    /// TIPOS - parametros_esperados:
    /// EQUIPACION_DESBLOQUEADA - Equipacion
    /// JUGADOR_DESBLOQUEADO - Jugador
    /// LOGRO_DESBLOQUEADO - (int)Recompensa_acumulada
    /// </summary>
    /// <param name="_tipoDialogo"></param>
    /// <param name="_parametro"></param>
    public DialogDefinition(TipoDialogo _tipoDialogo, System.Object _parametro) {
        m_tipoDialogo = _tipoDialogo;
        m_parametro = _parametro;
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------
}

