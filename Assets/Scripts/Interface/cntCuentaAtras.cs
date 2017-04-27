using UnityEngine;
using System.Collections;


public class cntCuentaAtras : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------

    // instancia de esta clase
    public static cntCuentaAtras instance { get { return m_instance; } }
    private static cntCuentaAtras m_instance;

    // texto para mostrar el contador
    private GUIText m_textoContador;

    // Indica si este control esta activado o no
    private bool m_activado = false;

    // tiempo restante para que se ejecute el callback
    private float m_tiempoRestante;

    // callback a realizar si se termina el tiempo
    private btnButton.guiAction m_timeLimitCallback = null;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    void Awake() {
        m_instance = this;

        // por defecto ocultar el cronometro
        m_textoContador = transform.FindChild("txtTiempo").GetComponent<GUIText>();
        m_textoContador.gameObject.SetActive(false);
    }


    /// <summary>
    /// Activa la cuenta atras del temporizador y le asigna el callback al que se llamara si se termina la cuenta atras
    /// </summary>
    /// <param name="_timeLimitCallBack">Accion a realizar si se termina el tiempo</param>
    /// <param name="_tiempo">tiempo en segundos a esperar</param>
    public void Activar(btnButton.guiAction _timeLimitCallBack, float _tiempo = Stats.CUENTA_ATRAS_TIRO_TIEMPO_TOTAL) {
        m_textoContador.gameObject.SetActive(false);
        m_timeLimitCallback = _timeLimitCallBack;
        m_tiempoRestante = _tiempo;
        m_activado = true;
    }


    /// <summary>
    /// Detiene la cuenta atras del cronometro
    /// </summary>
    public void Detener() {
        m_activado = false;
        m_textoContador.gameObject.SetActive(false);
    }


	// Update is called once per frame
	void Update () {
        if (m_activado) {
            m_tiempoRestante = Mathf.Max(m_tiempoRestante - Time.deltaTime, 0.0f);

            // comprobar si hay que mostrar la cuenta atras
            if (m_tiempoRestante <= Stats.CUENTA_ATRAS_TIRO_MOSTRAR) {
                // mostrar el temporizador
                m_textoContador.text = ((int) (m_tiempoRestante + 1)).ToString();
                m_textoContador.gameObject.SetActive(true);
            } else {
                // ocultar el temporizador
                m_textoContador.gameObject.SetActive(false);
            }

            // si se ha agotado el tiempo
            if (m_tiempoRestante == 0.0f) {
                // ocultar este control
                m_textoContador.gameObject.SetActive(false);

                // detener el cronometro
                m_activado = false;

                // realizar la accion de callback
                if (m_timeLimitCallback != null)
                    m_timeLimitCallback("");
            }
        }
	}


}
