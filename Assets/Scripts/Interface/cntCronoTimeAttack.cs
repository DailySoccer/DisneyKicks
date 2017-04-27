using UnityEngine;
using System.Collections;


/// <summary>
/// Comportamiento del cronometro para el modo TimeAttack
/// </summary>
public class cntCronoTimeAttack : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------

    // instancia de esta clase
    public static cntCronoTimeAttack instance { get { return m_instance; } }
    private static cntCronoTimeAttack m_instance;
 
    // tiempo restante que le queda al cronometro
    public float tiempoRestante { get { return m_tiempoRestante; } }
    private float m_tiempoRestante = 0.0f;

    // indica si el cronometro esta contando o no
    private bool m_activado = false;

    // accion a realizar cuando finaliza el tiempo
    private btnButton.guiAction m_callbackTiempoAgotado = null;

    // referencias a los textos
    private GUIText m_textoCronometro;
    private GUIText m_textoTiempoAdicional;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


	void Awake() {
        m_instance = this;

        // obtener las referencias a los elementos de esta interfaz
        m_textoCronometro = transform.FindChild("txtCronometro").GetComponent<GUIText>();
        m_textoTiempoAdicional = transform.FindChild("txtTiempoAdicional").GetComponent<GUIText>();
	}


    /// <summary>
    /// Inicializa el cronometro
    /// NOTA: esta funcion lo inicializa pero no lo activa, para que el cronometro empiece a contar llamar a la funcion "SetActivo(true)"
    /// </summary>
    /// <param name="_tiempoCronometro"></param>
    /// <param name="_callbackTiempoAgotado"></param>
    public void Inicializar(float _tiempoCronometro, btnButton.guiAction _callbackTiempoAgotado) {
        m_tiempoRestante = _tiempoCronometro;
        m_callbackTiempoAgotado = _callbackTiempoAgotado;
        m_activado = false;

        // mostrar este control
        transform.gameObject.SetActive(true);

        // ocultar el marcador de tiempo adicional
        m_textoTiempoAdicional.gameObject.SetActive(false);

        // pintar el tiempo como corresponda
        PintarTiempo();
    }


    /// <summary>
    /// Indica si este control debe estar visible o no
    /// </summary>
    /// <param name="_visible"></param>
    public void SetVisible(bool _visible) {
        transform.gameObject.SetActive(false);
    }


    /// <summary>
    /// Sirve para arrancar o detener la cuenta del cronometro
    /// </summary>
    /// <param name="_activo"></param>
    public void SetActivo(bool _activo) {
        m_activado = _activo;
    }


    /// <summary>
    /// Añade una cantidad de tiempo adicional al cronometro
    /// </summary>
    /// <param name="_tiempo"></param>
    public void AddTiempo(float _tiempo) {
        // si el cronometro esta activado
        if (m_activado) {
            // mostrar el texto con el tiempo adicional
            m_textoTiempoAdicional.text = ((int) _tiempo).ToString();

            if (_tiempo > 0.0f) {
                m_textoTiempoAdicional.color = new Color(0.726f, 0.929f, 0.371f, 1.0f); // verde
                m_textoTiempoAdicional.text = "+" + m_textoTiempoAdicional.text;
            } else {
                m_textoTiempoAdicional.color = new Color(1.0f, 0.0f, 0.0f, 1.0f); // rojo
            }

            // colorear el texto y mostrarlo
            m_textoTiempoAdicional.transform.gameObject.SetActive(true);

            // actualizar el tiempo
            m_tiempoRestante = Mathf.Max(m_tiempoRestante + _tiempo, 0.0f);
        }
    }

	
	// Update is called once per frame
	void Update () {
	    // si el cronometro esta activado => actualizar el tiempo restante
        if (m_activado) {
            // si se esta mostrando un popUp => NO actualizar el tiempo hasta que se oculte el popup
            if (ifcPopUp.instance != null) {
                GameObject go = ifcPopUp.instance.gameObject;
                if (go != null && go.activeInHierarchy) {
                    return;
                }
            }

            // actualizar el cronometro
            float escalaTiempo = (Time.timeScale != 0.0f) ? Time.timeScale : 1.0f; // <= para evitar divisiones por 0
            m_tiempoRestante -= Mathf.Max(Time.deltaTime / escalaTiempo, 0.0f); // nota: la escala de tiempo se utiliza porque para el lanzador y el portero el tiempo no transcurre igual

            // pintar el tiempo restante
            PintarTiempo();

            // si se ha agotado el tiempo => llamar a la funcion de callback
            if (m_tiempoRestante <= 0.0f) {
                // mostrar el texto sin opacidad
                m_textoCronometro.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);

                // detener el cronometro
                m_activado = false;

                // ejecutar el callback correspondiente
                if (m_callbackTiempoAgotado != null)
                    m_callbackTiempoAgotado("");
            }
        }

        // si el texto del tiempo adicional esta visible
        if (m_textoTiempoAdicional.gameObject.activeInHierarchy) {
            // reducir la opacidad del texto
            m_textoTiempoAdicional.color = new Color(
                m_textoTiempoAdicional.color.r,
                m_textoTiempoAdicional.color.g,
                m_textoTiempoAdicional.color.b,
                Mathf.Max(0.0f, m_textoTiempoAdicional.color.a - (Time.deltaTime / 2)));
        } else {
            // ocultar el texto
            m_textoTiempoAdicional.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Metodo para pintar el texto del tiempo como corresponda
    /// </summary>
    private void PintarTiempo() {
        // calcular la transparencia del texto
        float transparencia = 1.0f;
        if (m_tiempoRestante < 5.0f) {
            transparencia = ((m_tiempoRestante * 1000) % 1000) / 1000;
        }

        // colorear el texto del cronometro
        if (m_tiempoRestante < 10.0f) {
            // <color=#ff0000>
            m_textoCronometro.color = new Color(1.0f, 0.0f, 0.0f, transparencia);
            // sonido del corazon
            GeneralSounds.instance.chronobeat(m_tiempoRestante / 10f);
        } else if (m_tiempoRestante < 30.0f)
            // <color=#feda1d>
            m_textoCronometro.color = new Color(0.99f, 0.85f, 0.11f, transparencia);
        else
            // <color=#baee5f>
            m_textoCronometro.color = new Color(0.72f, 0.92f, 0.37f, transparencia);

        m_textoCronometro.text = ((int) m_tiempoRestante / 60).ToString("D2") + ":" + ((int) m_tiempoRestante % 60).ToString("D2");
    }

}
