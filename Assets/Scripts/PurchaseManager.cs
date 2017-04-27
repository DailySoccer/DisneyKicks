using UnityEngine;
using System.Collections.Generic;

public static class PurchaseManager
{
//un manager dedicado a Santi :D

    public static string[] skus = new string[] 
    {
        "com.bitoon.kicksfootballwarriors.gembooster.1",   // 
        "com.bitoon.kicksfootballwarriors.gembooster.2",   // 
        "com.bitoon.kicksfootballwarriors.gembooster.3",   // 
        "com.bitoon.kicksfootballwarriors.gembooster.4",   // 
        "com.bitoon.kicksfootballwarriors.gembooster.5",   // 
        "com.bitoon.kicksfootballwarriors.gembooster.6",   // 
        "com.bitoon.kicksfootballwarriors.coinbooster.1",  // 
        "com.bitoon.kicksfootballwarriors.coinbooster.2",  // 
        "com.bitoon.kicksfootballwarriors.coinbooster.3",  // 
        "com.bitoon.kicksfootballwarriors.coinbooster.4",  // 
        "com.bitoon.kicksfootballwarriors.coinbooster.5",  // 
        "com.bitoon.kicksfootballwarriors.coinbooster.6"   // 
    };

    // datos referentes a los packs de monedas
    public const int HARDCASH_PACKS = 6;
    public static int[] m_valoresPackMonedasHard = { 150, 300, 600, 1500, 3000, 6000 };
    public static int[] m_valoresPackMonedasSoft = { 1000, 2500, 5000, 10000, 15000, 20000 };

    public static void InitStore()
    {
        InAppPersistenceManager.instance.Init(skus);
        BillingManager.instance.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqke5E4wdSbtxFRGTHNH5HoPcj/t7wSwxNSIGODE72Rpp1PoU378SRTUD2i0LotYyQeRtQRDEcivcmUsAYRA23RzXPaRUL0aLAfoaDIw2LbXk7cxHGzyrBVdtqj7+a6+uTLoSo7/WKZAxiWitjDcsE93WTL5d60M/SAcEcfQah1opzMXPGvEGbKb2l+c90P4mjuvckbzvtNzk57UWh76MJrGZCFveU9AcBP4MpJNM57fANGamS83cPzbElrUUyzO/4LKLpkvLu2y4vZipHM89p4RnTIL07SFl4Fon42BNuwkVe/XqOY3luj70UHlzsTakBD7kaRFDSyJRshtQyCup1QIDAQAB", skus);
#if UNITY_ANDROID
        foreach(string s in skus) GoogleIAB.consumeProduct(s);
#endif
    }

#if UNITY_ANDROID
    public static List<GoogleSkuInfo> InventarioTienda;
    
    public static string[] GetPreciosOrdenados()
    {
        
        if(InventarioTienda == null)
        {
            string[] res = {"?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?"};
            return res;
        }
        
        GoogleSkuInfo[] aux = new GoogleSkuInfo[skus.Length];
        
        for(int i = 0 ; i < skus.Length ; i++)
        {
            string id = skus[i];
            foreach(GoogleSkuInfo info in InventarioTienda)
            {
                if(info.productId == id)
                {
                    aux[i] = info;
                    break;
                }
            }
        }
        
        string[] results = new string[skus.Length];
        for(int j = 0; j < aux.Length ; j++)
        {
            if(aux[j] != null)
            {
                results[j] = aux[j].price;
            }
            else
            {
                results[j] = "?";
            }
        }
        return results;
    }
#elif UNITY_IPHONE
    public static List<StoreKitProduct> InventarioTienda;
    
    public static string[] GetPreciosOrdenados() {
        if(InventarioTienda == null)
        {
            string[] res = {"?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?", "?"};
            return res;
        }
        
        StoreKitProduct[] aux = new StoreKitProduct[skus.Length];
        
        for (int i = 0; i < skus.Length; i++) {
            string id = skus[i];
            foreach (StoreKitProduct info in InventarioTienda) {
                if (info.productIdentifier == id) { 
                    aux[i] = info;
                    break;
                }
            }
        }
        
        string[] results = new string[skus.Length];
        for (int j = 0; j < aux.Length; j++) {
            if (aux[j] != null) {
                results[j] = aux[j].price;
                results[j] += " " + aux[j].currencySymbol;
            } else {
                results[j] = "?";
            }
        }
        
        return results;
    }
#endif

    public static void PerformPurchase(string _id)
    {
        Debug.LogWarning("COMPRA: " + _id);
        // FPA (04/01/17): Eliminado GameAnalitics de momento. 
        // GA.API.Design.NewEvent("Compra:"+_id, 0f, Vector3.zero);
        for ( int i = 0 ; i < skus.Length ; i++)
        {
            if(_id == skus[i])
            {
                if(i < HARDCASH_PACKS)
                {
                    Interfaz.MonedasHard += (m_valoresPackMonedasHard[i]);
                }
                else
                {
                    Interfaz.MonedasSoft += (m_valoresPackMonedasSoft[i - HARDCASH_PACKS]);
                }
            }
        }
        cntBarraSuperior.instance.ActualizarDinero();
    }

}
