using UnityEngine;
using System.Collections;

/// <summary>
/// Clase para representar una fila con la informacion de un jugador del listado de favoritos
/// </summary>
public class cntJugadorFavorito : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // datos almacenados en este control
    public string nombreJugador { get { return m_nombreJugador; } }
    private string m_nombreJugador;

    // elementos de esta interfaz
    private GUIText m_txtUsuario;
    private GUIText m_txtVictoriasDerrotas;
    private btnButton m_collider;               // <== boton que funciona como collider de este control


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Inicializa la informacion a mostrar en este control
    /// </summary>
    /// <param name="_usuario"></param>
    /// <param name="_listadoJugadoresFavoritos">Listado de jugadores (GameObject) al que pertenece este control</param>
    public void SetInfoJugador(Usuario _usuario, ifcFavoritos _listadoJugadoresFavoritos = null) {
        // obtener la referencia a los elementos de esta interfaz
        if (m_txtUsuario == null)
            m_txtUsuario = (GUIText) transform.FindChild("txtUsuario").GetComponent("GUIText");
        if (m_txtVictoriasDerrotas == null)
            m_txtVictoriasDerrotas = (GUIText) transform.FindChild("txtVictoriasDerrotas").GetComponent("GUIText");

        if (_usuario == null) {
            // si no se especifica usuario => no mostrar nada en el control
            m_txtUsuario.text = "";
            m_txtVictoriasDerrotas.text = "";
        } else {
            // guardar el nombre del usuario
            m_nombreJugador = _usuario.alias;

            // calcular el porcentaje de victorias
            float porcentajeVictorias;
            if (_usuario.numVictorias + _usuario.numDerrotas == 0)
                porcentajeVictorias = 100.0f;
            else
                porcentajeVictorias = 100.0f * _usuario.numVictorias / (_usuario.numVictorias + _usuario.numDerrotas);

            // inicializar los textos
            m_txtUsuario.text = "<color=#ffffff>" + _usuario.alias + "</color>" + " <color=#ffd200>" + (int) porcentajeVictorias + "%</color>";
            m_txtVictoriasDerrotas.text = "<color=#87befe>" + _usuario.numVictorias + "/" + _usuario.numDerrotas + "</color>";
        }
        // si el elemento pertenece a un listado
        if (_listadoJugadoresFavoritos != null) {
            // inicializar el collider
            if (m_collider == null)
                m_collider = (btnButton) transform.FindChild("collider").GetComponent("btnButton");
            m_collider.action = (_name) => {
                // si el control tiene un nombre de usuario => seleccionarlo
                if (m_txtUsuario.text != "")
                    _listadoJugadoresFavoritos.SeleccionarJugadorEnListado(this);
            };
        }

        // por defecto deseleccionar este elemento 
        m_collider.Deselect();
    }


    /// <summary>
    /// Marca o desmarca este elemento como seleccionado
    /// </summary>
    /// <param name="_selected"></param>
    public void SetSelected(bool _selected) {
        if (m_collider != null) {
            if (_selected) {

                // modificar los colores
                m_txtUsuario.text = m_txtUsuario.text.Replace("<color=#ffffff>", "<color=#000000>");
                m_txtUsuario.text = m_txtUsuario.text.Replace("<color=#ffd200>", "<color=#5a3c00>");
                m_txtVictoriasDerrotas.text = m_txtVictoriasDerrotas.text.Replace("<color=#87befe>", "<color=#000000>");

                // actualizar el estado del colider
                m_collider.Select();
            } else {
                // modificar los colores del texto
                m_txtUsuario.text = m_txtUsuario.text.Replace("<color=#000000>", "<color=#ffffff>");
                m_txtUsuario.text = m_txtUsuario.text.Replace("<color=#5a3c00>", "<color=#ffd200>");
                m_txtVictoriasDerrotas.text = m_txtVictoriasDerrotas.text.Replace("<color=#000000>", "<color=#87befe>");

                // actualizar el estado del colider
                m_collider.Deselect();
            }
        }
    }




}
