using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase para generar un elemento de interfaz contenedor de "cntCompraPowerUps"
/// </summary>
public class cntCompraItemsContainer: MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Numero de power ups que se muestran por pagina
    /// </summary>
    private const int NUM_ITEMS_PAGINA = 3;

    /// <summary>
    /// Separacion en el eje X que hay entre los items a comprar
    /// </summary>
    private const float SEPARACION_X_ENTRE_ITEMS = 0.138f;


    // ------------------------------------------------------------------------------
    // ---  ENUMERADOS  -------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Tipo de item que se va a mostrar en este control
    /// </summary>
    public enum TipoItem { POWER_UP_LANZADOR, POWER_UP_PORTERO, ESCUDO };


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // numero de pagina / grupo de items que se esta mostrando
    private int m_numPaginaActual = 0;

    // elementos de este componente
    private btnButton m_btnIzda;
    private btnButton m_btnDcha;
    private cntCompraItem[] m_cntCompraItem;

    private Jugador m_jugador;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // Use this for initialization
    void Awake() {
        // ocultar este control
        transform.gameObject.SetActive(false);
    }


    /// <summary>
    /// obtener la referencia a los elementos de esta interfaz
    /// </summary>
    private void ObtenerReferencias() {
        if (m_btnIzda == null) 
            m_btnIzda = transform.FindChild("btnIzda").GetComponent<btnButton>();
        if (m_btnDcha == null) 
            m_btnDcha = transform.FindChild("btnDcha").GetComponent<btnButton>();
        if (m_cntCompraItem == null) {
            m_cntCompraItem = new cntCompraItem[NUM_ITEMS_PAGINA];
            for (int i = 0; i < NUM_ITEMS_PAGINA; ++i)
                m_cntCompraItem[i] = transform.FindChild("compraItem" + i).GetComponent<cntCompraItem>();
        }
    }


    /// <summary>
    /// Inicializa este control para mostrar el tipo de item al que se le asocia
    /// </summary>
    /// <param name="_tipoItem"></param>
    /// <param name="_posicionOrigen"></param>
    public void Inicializar(Jugador _jugador, TipoItem _tipoItem, Vector2 _posicionOrigen) {
        ObtenerReferencias();
        /*
        // reposicionar los items a comprar
        for (int i = 0; i < m_cntCompraItem.Length; ++i) {
            Vector3 posicion = m_cntCompraItem[i].transform.localPosition;
            m_cntCompraItem[i].transform.localPosition = new Vector3(_posicionOrigen.x + (i * SEPARACION_X_ENTRE_ITEMS), _posicionOrigen.y, 0.0f);
        }
         */

        m_jugador = _jugador;
        m_numPaginaActual = 0;

        // mostrar la pagina
        ShowPagina(m_numPaginaActual, _tipoItem);
    }


    /// <summary>
    /// Muestra los elementos del numero de pagina especificado
    /// </summary>
    /// <param name="_numPagina"></param>
    /// <param name="_tipoItem"></param>
    private void ShowPagina(int _numPagina, TipoItem _tipoItem) {
        // guardar el numero de pagina actual
        m_numPaginaActual = _numPagina;

        // calcular el numero maximo de paginas a mostrar
        int numTotalPaginas = 0;
        switch (_tipoItem) {
            case TipoItem.POWER_UP_LANZADOR:
                List<PowerUpDescriptor> descriptoresLanzador = PowerupInventory.descriptoresLanzadorFiltered(m_jugador.powerups);
                numTotalPaginas = 1 + (Mathf.Max(1, descriptoresLanzador.Count - 1) / NUM_ITEMS_PAGINA);

                // actualizar los elementos del container
                for (int i = 0; i < NUM_ITEMS_PAGINA; ++i)
                    if ((_numPagina * NUM_ITEMS_PAGINA) + i < descriptoresLanzador.Count)
                        m_cntCompraItem[i].ShowAsPowerUp(descriptoresLanzador[(_numPagina * NUM_ITEMS_PAGINA) + i]);
                    else
                        m_cntCompraItem[i].ShowAsPowerUp(null);
                break;

            case TipoItem.POWER_UP_PORTERO:
                List<PowerUpDescriptor> descriptoresPortero = PowerupInventory.descriptoresPorteroFiltered(m_jugador.powerups);
                numTotalPaginas = 1 + (Mathf.Max(1, descriptoresPortero.Count - 1) / NUM_ITEMS_PAGINA);

                // actualizar los elementos del container
                for (int i = 0; i < NUM_ITEMS_PAGINA; ++i)
                    if ((_numPagina * NUM_ITEMS_PAGINA) + i < descriptoresPortero.Count)
                        m_cntCompraItem[i].ShowAsPowerUp(descriptoresPortero[(_numPagina * NUM_ITEMS_PAGINA) + i]);
                    else
                        m_cntCompraItem[i].ShowAsPowerUp(null);
                break;

            case TipoItem.ESCUDO:
                numTotalPaginas = 1 + (Mathf.Max(1, EscudosManager.instance.GetNumEscudos() - 1) / NUM_ITEMS_PAGINA);

                // asegurarse de que la pagina actual queda dentro de rango
                _numPagina = Mathf.Clamp(_numPagina, 0, numTotalPaginas);

                // actualizar los elementos del container
                for (int i = 0; i < NUM_ITEMS_PAGINA; ++i)
                    m_cntCompraItem[i].ShowAsEscudo(EscudosManager.instance.GetEscudo((_numPagina * NUM_ITEMS_PAGINA) + i));
                break;
        }      

        // boton paginar izquierda
        m_btnIzda.gameObject.SetActive(_numPagina > 0);
        m_btnIzda.action = (_name) => {
            GeneralSounds_menu.instance.select();
            ShowPagina(--m_numPaginaActual, _tipoItem);
        };

        // boton paginar dcha
        m_btnDcha.gameObject.SetActive(_numPagina < (numTotalPaginas - 1));
        m_btnDcha.action = (_name) => {
            GeneralSounds_menu.instance.select();
            ShowPagina(++m_numPaginaActual, _tipoItem);
        };
    }


}
