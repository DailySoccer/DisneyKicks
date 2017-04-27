using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Clase para gestionar las barreras
/// </summary>
public class BarreraManager: MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  ------------------------------------------------------------
    // ----------------------------------------------------------------------------


    // separacion entre los jugadores de la barrera
    private const float SEPARACION_ENTRE_JUGADORES = 0.65f;

    // ancho del collider de cada jugador de la barrera (mirar este valor en el prefab)
    private const float ANCHO_COLLIDER_JUGADOR = 0.81f;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static BarreraManager instance { get { return m_instance; } }
    private static BarreraManager m_instance;

    // prefab para generar instancias de los jugadores que componen la barrera 
    // NOTA: asignarle valor desde la interfaz
    public GameObject prefJugadorBarrera;

    // gameObjects para mostrar los jugadores de la barrera
    private List<GameObject> m_listaJugadoresBarrera;

    // referencia al boxcollider de esta clase
    private BoxCollider m_boxCollider;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
        m_boxCollider = transform.GetComponent<BoxCollider>();

        // inicializar la lista de jugadores de la barrera
        m_listaJugadoresBarrera = new List<GameObject>();

        // por defecto ocultar este objeto
        Habilitar(false);
    }


    /// <summary>
    /// Metodo para crear una barrera de "_numBarrierPlayers" jugadores defensores
    /// </summary>
    /// <param name="_numBarrierPlayers">Numero de jugadores defensores de los que se compone la barrera</param>
    /// <param name="_shooterPosition">Posicion del jugador que va a efectuar el tiro</param>
    public void Create(int _numBarrierPlayers, Vector3 _shooterPosition) {
        // si la barrera es de 0 jugadores o menos => la oculto
        if (_numBarrierPlayers <= 0) {
            Habilitar(false);
        } else {
            // mostrar la barrera
            Habilitar(true);

            // comprobar si el numero de jugadores de la barrera ha cambiado 
            if (m_listaJugadoresBarrera.Count != _numBarrierPlayers) {
                // si la nueva barrera tiene menos jugadores
                if (_numBarrierPlayers < m_listaJugadoresBarrera.Count) {
                    // eliminar los jugadores que sobren
                    for (int i = m_listaJugadoresBarrera.Count - 1; i >= _numBarrierPlayers; --i) {
                        Destroy(m_listaJugadoresBarrera[i]);
                        m_listaJugadoresBarrera.RemoveAt(i);
                    }
                } else {
                    // añadir los jugadores que falten
                    for (int i = m_listaJugadoresBarrera.Count; i < _numBarrierPlayers; ++i) {
                        GameObject goJugador = GameObject.Instantiate(prefJugadorBarrera) as GameObject;
                        goJugador.transform.parent = transform;
                        goJugador.name = "jugadorBarrera" + i;
                        m_listaJugadoresBarrera.Add(goJugador);
                    }
                }
            }

            // posicionar (localmente) los jugadores de la barrera
            for (int i = 0; i < m_listaJugadoresBarrera.Count; ++i) {
                m_listaJugadoresBarrera[i].transform.localPosition = new Vector3((SEPARACION_ENTRE_JUGADORES * (_numBarrierPlayers - 1) / 2) - (i * SEPARACION_ENTRE_JUGADORES), 0.0f, 0.0f);
                m_listaJugadoresBarrera[i].transform.localRotation = Quaternion.identity;
            }

            // calcular el tamaño del colider de la barrera
            m_boxCollider.size = new Vector3((SEPARACION_ENTRE_JUGADORES * (_numBarrierPlayers - 1)) + ANCHO_COLLIDER_JUGADOR, m_boxCollider.size.y, m_boxCollider.size.z);

            // calcular cual de los dos postes de la porteria esta mas cercano a la posicion de tiro
            Vector3 vectorPosTiroPoste;
            Vector3 vectorPosTiroPosteIzdo = new Vector3(Porteria.instance.position.x - (7.16f / 2), Porteria.instance.position.y, Porteria.instance.position.z) - _shooterPosition;
            Vector3 vectorPosTiroPosteDcho = new Vector3(Porteria.instance.position.x + (7.16f / 2), Porteria.instance.position.y, Porteria.instance.position.z) - _shooterPosition;
            if (vectorPosTiroPosteIzdo.sqrMagnitude < vectorPosTiroPosteDcho.sqrMagnitude) {
                vectorPosTiroPoste = vectorPosTiroPosteIzdo;
            } else {
                vectorPosTiroPoste = vectorPosTiroPosteDcho;
            }

            // calcular el punto donde colocar la barrera
            transform.position = _shooterPosition + (vectorPosTiroPoste / 2); // <= NOTA: si se quiere acercar o alejar la barrera modificar este "2"

            // desplazar la barrera para que uno de sus extremos quede alineado con el poste elegido
            float ajuste = 0.9f;    // <= NOTA: sirve para que la barrera no se ajuste completamente al poste
            if (vectorPosTiroPoste == vectorPosTiroPosteIzdo)
                transform.localPosition = new Vector3(transform.localPosition.x + (m_boxCollider.size.x / 2) * ajuste, transform.localPosition.y, transform.localPosition.z);
            else
                transform.localPosition = new Vector3(transform.localPosition.x - (m_boxCollider.size.x / 2) * ajuste, transform.localPosition.y, transform.localPosition.z);

            // orientar la barrera hacia el balon
            Debug.Log(">>> FORWARD: " + transform.forward);
            transform.LookAt(_shooterPosition);
        }
    }


    /// <summary>
    /// Muestra el contol y habilita sus colliders o lo oculta y deshabilita sus colliders
    /// </summary>
    /// <param name="_enabled"></param>
    public void Habilitar(bool _enabled) {
        if (_enabled) {
            // collider general de la barrera
            transform.gameObject.SetActive(true);
            m_boxCollider.enabled = true;

            // colliders de los jugadores
            for (int i = 0; i < m_listaJugadoresBarrera.Count; ++i)
                m_listaJugadoresBarrera[i].transform.FindChild("barrera").GetComponent<BoxCollider>().enabled = true;

        } else {
            // primero habilito el control xq si no modifica bien los colliders
            transform.gameObject.SetActive(true);

            for (int i = 0; i < m_listaJugadoresBarrera.Count; ++i) {
                // coliders de las cabezas de los jugadores
                m_listaJugadoresBarrera[i].transform.FindChild("barrera").GetComponent<BoxCollider>().enabled = false;
            }

            // collider general de la barrera
            m_boxCollider.enabled = false;
            transform.gameObject.SetActive(false);
        }
    }

}
