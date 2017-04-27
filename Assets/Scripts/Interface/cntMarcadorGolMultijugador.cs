using UnityEngine;
using System.Collections;


/// <summary>
/// Control para mostrar el estado de un tiro (si aun no se ha lanzado, se ha marcado o fallado)
/// </summary>
public class cntMarcadorGolMultijugador : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // estados posibles de este control
    public enum Estado { SIN_TIRAR = 0, MARCADO = 1, FALLADO = 2 }


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // texturas para mostrar los diferentes estados
    public Texture m_texturaGolMarcado;
    public Texture m_texturaGolFallado;
    public Texture m_texturaAunNoDisparado;

    // elementos visuales de este control
    private GUITexture m_guiTextura;

    // estado de este control
    public Estado estado { get { return m_estado; } }
    private Estado m_estado = Estado.SIN_TIRAR;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Start() {
        // por defecto establecer el tiro como no realizado
        SetEstado(Estado.SIN_TIRAR);
    }


    /// <summary>
    /// Modifica el estado de este control
    /// </summary>
    /// <param name="_estado"></param>
    public void SetEstado(Estado _estado) {
        // obtener la referencia a la textura
        if (m_guiTextura == null)
            m_guiTextura = transform.GetComponent<GUITexture>();

        // en funcion del estado actualizar la textura
        switch (_estado) {
            case Estado.SIN_TIRAR:
                m_guiTextura.texture = m_texturaAunNoDisparado;
                m_guiTextura.gameObject.SetActive(true);
                break;

            case Estado.MARCADO:
                m_guiTextura.texture = m_texturaGolMarcado;
                m_guiTextura.gameObject.SetActive(true);
                break;

            case Estado.FALLADO:
                m_guiTextura.texture = m_texturaGolFallado;
                m_guiTextura.gameObject.SetActive(true);
                break;
        }

        // actualizar el estado
        m_estado = _estado;
    }


}
