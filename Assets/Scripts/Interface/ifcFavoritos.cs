using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Control para mostrar los favoritos del usuario actual
/// </summary>
public class ifcFavoritos : ifcBase {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Numero de jugadores favoritos por cada pagina del listado
    /// Nota NUM_JUGADORES_PAGINA tiene que ser > 0
    /// </summary>
    private const int NUM_JUGADORES_PAGINA = 7;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // instancia de esta clase
    public static ifcFavoritos instance { get; protected set; }

    // objeto prefab para generar instancias
    public cntJugadorFavorito m_prefJugadorFavorito;

    // controles para mostrar los usuarios favoritos
    private cntJugadorFavorito[] m_jugadoresFavoritos;

    // botones
    private btnButton m_btnCerrar;
    private btnButton m_btnPaginarAnterior;
    private btnButton m_btnPaginarSiguiente;
    private btnButton m_btnAgregar;
    private btnButton m_btnEliminar;

    // texto para mostrar el numero de pagina actual
    private GUIText m_txtNumPagina;

    // numero de pagina del listado de jugadores favoritos que se esta mostrando
    private int m_numPagina = 0;

    // indica que jugador favorito esta seleccionado en la pagina actual (un valor negativo indica que ninguno)
    private int m_numJugadorFavoritoSeleccionadoEnPagina = -1;
    

    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        instance = this;
    }


	// Use this for initialization
	void Start () {
        m_backMethod = Back;

        // boton atras
        transform.FindChild("btnAtras").GetComponent<btnButton>().action = (_name) => {
            Back();
        };

        // boton paginar anterior
        m_btnPaginarAnterior = (btnButton) transform.FindChild("btnPaginarAnterior").GetComponent<btnButton>();
        m_btnPaginarAnterior.action = (_name) => {
            // refrescar la pagina
            UpdatePaginaListadoFavoritos(m_numPagina - 1);
        };

        // boton paginar siguiente
        m_btnPaginarSiguiente = (btnButton) transform.FindChild("btnPaginarSiguiente").GetComponent<btnButton>();
        m_btnPaginarSiguiente.action = (_name) => {
            // refrescar la pagina
            UpdatePaginaListadoFavoritos(m_numPagina + 1);
        };

        // texto para mostrar el numer de pagina
        m_txtNumPagina = (GUIText) transform.FindChild("numPagina").GetComponent<GUIText>();

        // boton agregar
        m_btnAgregar = (btnButton) transform.FindChild("btnAgregar").GetComponent<btnButton>();
        m_btnAgregar.action = (_name) => { AgregarUsuario(); };

        // boton eliminar
        m_btnEliminar = (btnButton) transform.FindChild("btnEliminar").GetComponent<btnButton>();
        m_btnEliminar.action = (_name) => { EliminarUsuario(); };

        // generar los elementos del listado de favoritos
        Transform listadoJugadores = transform.FindChild("ListadoJugadoresFavoritos");
        m_jugadoresFavoritos = new cntJugadorFavorito[NUM_JUGADORES_PAGINA];
        for (int i = 0; i < m_jugadoresFavoritos.Length; ++i) {
            m_jugadoresFavoritos[i] = (cntJugadorFavorito)Instantiate(m_prefJugadorFavorito);
            m_jugadoresFavoritos[i].transform.parent = listadoJugadores;
            m_jugadoresFavoritos[i].name = "jugadorFavorito" + i;
            m_jugadoresFavoritos[i].transform.localPosition = new Vector3(0.0f, -0.06f * i, 0.0f);
        }

        // reescalar los elementos creados dinamicamente de esta interfaz
        /*
        ifcBase.Scale(m_btnPaginarAnterior.gameObject);
        ifcBase.Scale(m_btnPaginarSiguiente.gameObject);
        for (int i = 0; i < m_jugadoresFavoritos.Length; ++i) {
            ifcBase.Scale(m_jugadoresFavoritos[i].gameObject);
        }
         */
        ifcBase.Scale(transform.FindChild("ListadoJugadoresFavoritos").gameObject);


        // desmarcar los elementos del listado
        SeleccionarJugadorEnListado(null);
	}


    public void Back(string _name = "") {
        // volver a la interfaz de perfil
        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.5f, 0.5f, 0.0f), SuperTweener.CubicOut);
        new SuperTweener.move(ifcPerfil.instance.gameObject, 0.25f, new Vector3(0.0f, 0.0f, 0.0f), SuperTweener.CubicOut);
        ifcBase.activeIface = ifcPerfil.instance;
        GeneralSounds_menu.instance.back();

    }


    /// <summary>
    /// Actualiza el listado de favoritos
    /// </summary>
    public void UpdatePaginaListadoFavoritos() {
        UpdatePaginaListadoFavoritos(m_numPagina);
    }


    /// <summary>
    /// Muestra la pagina del listado de favoritos recibida como parametro
    /// </summary>
    /// <param name="_numPagina"></param>
    public void UpdatePaginaListadoFavoritos(int _numPagina) {
        // indicar que no hay favorito seleccionado
        m_numJugadorFavoritoSeleccionadoEnPagina = -1;

        // obtener la lista de favoritos
        List<Usuario> listadoFavoritos = UsuariosFavoritos.instance.GetFavoritos();
        if (listadoFavoritos == null)
            listadoFavoritos = new List<Usuario>(); // <= si no hay lista crear una nueva con 0 elementos para que el resto del codigo de este metodo se comporte igual

        // calcular el numero de paginas del listado
        int numPaginasListado = (listadoFavoritos.Count % NUM_JUGADORES_PAGINA == 0) ? (int) Mathf.Max(0, (listadoFavoritos.Count / NUM_JUGADORES_PAGINA) - 1) : (int) (listadoFavoritos.Count / NUM_JUGADORES_PAGINA);

        // comprobar que el numero de pagina recibido quede dentro de rango
        if (_numPagina < 0)
            _numPagina = 0;
        if (_numPagina > numPaginasListado)
            _numPagina = numPaginasListado;

        // actualizar el numero de pagina
        m_numPagina = _numPagina;
        m_txtNumPagina.text = "<color=#ffd200>" + (m_numPagina + 1).ToString() + "/" + (numPaginasListado + 1) + "</color>";

        // mostrar / ocultar los botones de paginar
        m_btnPaginarAnterior.gameObject.SetActive(_numPagina > 0);
        m_btnPaginarSiguiente.gameObject.SetActive(_numPagina < numPaginasListado);

        // calcular el primer jugador favorito a listar
        int primerJugadorListado = _numPagina * NUM_JUGADORES_PAGINA;

        // actualizar el listado de elementos
        for (int i = 0; i < NUM_JUGADORES_PAGINA; ++i) {
            if (primerJugadorListado + i < listadoFavoritos.Count)
                m_jugadoresFavoritos[i].SetInfoJugador(listadoFavoritos[primerJugadorListado + i], this);
            else
                m_jugadoresFavoritos[i].SetInfoJugador(null, this);
        }
    }


    /// <summary>
    /// Destaca un elemento del listado de jugadores y lo marca como seleccionado (y el resto los deselecciona)
    /// </summary>
    /// <param name="_elementoSeleccionado"></param>
    public void SeleccionarJugadorEnListado(cntJugadorFavorito _jugadorSeleccionado) {
        // valor por defecto
        m_numJugadorFavoritoSeleccionadoEnPagina = -1;

        // marcar o desmarcar los jugadores del listado
        for (int i = 0; i < m_jugadoresFavoritos.Length; ++i) {
            if (m_jugadoresFavoritos[i] == _jugadorSeleccionado) {
                m_numJugadorFavoritoSeleccionadoEnPagina = i;
                m_jugadoresFavoritos[i].SetSelected(true);
                Debug.Log(">>> Seleccionado el elemento " + i + " del listado");
            } else
                m_jugadoresFavoritos[i].SetSelected(false);
        }
    }


    /// <summary>
    /// Acciones a realizar cuando se agrega a un usuario
    /// </summary>
    public void AgregarUsuario() {
        /*
        ifcDialogBox.instance.ShowTextInput(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "INTRODUCE EL NICK A AGREGAR", "",
            // accion a realizar al aceptar al usuario
            (_name) => {
                if (ifcDialogBox.instance.textoEditado == "") {
                    // si el nick es vacio avisar de error y volver a mostrar al dialogo de "agregar al usuario"
                    ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "INTRODUCE EL NICK A AGREGAR", "El nombre de usuario no debe estar vacio",
                        (_name1) => { AgregarUsuario(); },
                        (_name2) => { AgregarUsuario(); });
                } else if (ifcDialogBox.instance.textoEditado.ToUpper() == Interfaz.m_uname.ToUpper()) {
                    // si el nick es vacio avisar de error y volver a mostrar al dialogo de "agregar al usuario"
                    ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "INTRODUCE EL NICK A AGREGAR", "No puedes agregarte a ti mismo como amigo",
                        (_name1) => { AgregarUsuario(); },
                        (_name2) => { AgregarUsuario(); });
                } else if (UsuariosFavoritos.instance.ExisteAlias(ifcDialogBox.instance.textoEditado)) {
                    // comprobar si el nick ya pertenece a tu lista de favoritos
                    ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "INTRODUCE EL NICK A AGREGAR", "No puedes añadr a <color=#feda1d>" + ifcDialogBox.instance.textoEditado + "</color> porque ya pertenece a tus favoritos",
                        (_name1) => { AgregarUsuario(); },
                        (_name2) => { AgregarUsuario(); });
                }
            });
         */
    }


    /// <summary>
    /// Acciones a realizar cuando se elimina un usuario
    /// </summary>
    private void EliminarUsuario() {
        /*
        if (m_numJugadorFavoritoSeleccionadoEnPagina < 0) {
            ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "ELIMINAR FAVORITO", "Pulsa sobre uno de tus favoritos para eliminarlo");
        } else {
            // obtener el nombre del usuario seleccionado y enviar el mensaje al server para que lo elimine de favoritos
            //Interfaz.instance.deleteContacts(m_jugadoresFavoritos[m_numJugadorFavoritoSeleccionadoEnPagina].nombreJugador);
        }
         */
    }


}
