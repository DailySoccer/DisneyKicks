#define MONEY_FREE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase para generar una caja de dialogo de compra de hard cash
/// </summary>
public class ifcBuyHardCashDialogBox: ifcBase {

   
    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------
    

    #region Singleton

    private static ifcBuyHardCashDialogBox _instance;
    public static ifcBuyHardCashDialogBox Instance {
        get { return _instance; }
    }

    #endregion


    // texturas para mostrar packs de hard/soft cash
    // NOTA: asignarles valor desde la interfaz de unity
    public Texture m_texturaHardCoins;
    public Texture m_texturaSoftCoins;

    // elementos del dialogo
    private btnButton m_btnCerrar;
    private btnButton m_btnVelo;       // <= el velo funciona como el boton aceptar
    private List<btnButton> _hardCashPackButtons = new List<btnButton>();

    //interfaz subyacente
    private ifcBase m_interfazAnterior;


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    #region MonoBehaviour

    void Awake () {
        _instance = this;
        GameObject.DontDestroyOnLoad( this.gameObject );
    }

    void Start () {

        this.m_backMethod = Back;

        // obtener las referencias a los elementos del dialogbox
        Transform cajaTransform = transform.FindChild( "caja" );

        m_btnCerrar = cajaTransform.FindChild( "btnCancelar" ).gameObject.GetComponent<btnButton>();
        m_btnCerrar.action = (_name) => {
            // hacer sonar el click y ocultar este dialogo
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
            gameObject.SetActive( false );
        };

        m_btnVelo = transform.FindChild("velo").GetComponent<btnButton>();
        m_btnVelo.action = (_name) => {
            // hacer sonar el click y ocultar este dialogo
            GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
            gameObject.SetActive(false);
        };

        this.gameObject.SetActive( false ); // ocultar este control
    }

    #endregion

    
    /// <summary>
    /// Muestra el dialogo
    /// </summary>
    /// <param name="_showHardCoinPacks">Indica si en el dialogo hay que mostrar packs de hardcoins o de softcoins</param>
    public void Show (bool _showHardCoinPacks) {
#if UNITY_IPHONE || UNITY_ANDROID
        string[] precios = PurchaseManager.GetPreciosOrdenados();
#else
        //string[] precios = {"0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €","0.0 €"};
        string[] precios = { "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?" };
#endif

#if !UNITY_EDITOR
        // verificar que se haya obtenido los precios de la tienda
        if (precios == null || precios[0] == "?") {
            ifcDialogBox.instance.ShowOneButtonDialog(ifcDialogBox.OneButtonType.POSITIVE, LocalizacionManager.instance.GetTexto(84).ToUpper(), LocalizacionManager.instance.GetTexto(290), LocalizacionManager.instance.GetTexto(45).ToUpper());
            return;
        }
#endif

        m_interfazAnterior = ifcBase.activeIface; // guardar la interfaz anterior para recuperarla
        GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOnClip);

        // inicializar los botones de compra de monedas
        for (int i = 0; i < PurchaseManager.HARDCASH_PACKS; ++i) {
            int idx = (i + 1);
            int index = i;
            int skuIndex = index + ((_showHardCoinPacks) ? 0 : PurchaseManager.HARDCASH_PACKS);

            // accion al pulsar el boton
            btnButton btn = transform.FindChild("caja/pack_" + idx).GetComponent<btnButton>();
            btn.GetComponent<GUITexture>().texture = (_showHardCoinPacks) ? m_texturaHardCoins : m_texturaSoftCoins;
            btn.m_current = (_showHardCoinPacks) ? m_texturaHardCoins : m_texturaSoftCoins;
            btn.action = (_name) => {
#if MONEY_FREE
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.compraMonedaClip);
                Debug.Log("Money free " + skuIndex);
                // BillingManager.instance.purchaseProduct(PurchaseManager.skus[skuIndex]);

                // incrementar los diferentes tipos de moneda
                PurchaseManager.PerformPurchase(PurchaseManager.skus[skuIndex]);

                // ocultar este control
                this.gameObject.SetActive(false);
                cntBarraSuperior.instance.ActualizarDinero();
#else
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.compraMonedaClip);
                Debug.Log("Comprar paquete " + skuIndex);
                BillingManager.instance.purchaseProduct(PurchaseManager.skus[skuIndex]);


                // incrementar los diferentes tipos de moneda
                //Interfaz.MonedasHard += cantidadMonedasHard;
                //Interfaz.MonedasSoft += cantidadMonedasSoft;

                // ocultar este control
                //this.gameObject.SetActive(false);
                //cntBarraSuperior.instance.ActualizarDinero();
#endif
            };

            // actualizar el texto con la cantidad de moneda del pack
            btn.transform.FindChild("Amount").GetComponent<GUIText>().text = ((_showHardCoinPacks) ? PurchaseManager.m_valoresPackMonedasHard[index] : PurchaseManager.m_valoresPackMonedasSoft[index]).ToString();
            btn.transform.FindChild("Amount/Shadow").GetComponent<GUIText>().text = ((_showHardCoinPacks) ? PurchaseManager.m_valoresPackMonedasHard[index] : PurchaseManager.m_valoresPackMonedasSoft[index]).ToString();

            // actualizar el texto con el precio del pack
            btn.transform.FindChild("HardCashPrice/Text").GetComponent<GUIText>().text = precios[skuIndex];
            btn.transform.FindChild("HardCashPrice/TextSombra").GetComponent<GUIText>().text = precios[skuIndex];

            _hardCashPackButtons.Add(btn);
        }

        this.gameObject.SetActive( true ); // mostrar este control
    }

    /// <summary>
    /// Oculta el control
    /// </summary>
    public void Hide () {
        ifcBase.activeIface = m_interfazAnterior;
        gameObject.SetActive( false );
    }

    void Back (string _input) {
        Hide();        
    }


}