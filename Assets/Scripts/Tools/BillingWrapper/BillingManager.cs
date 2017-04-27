using UnityEngine;
//using System;
//using System.Text;
//using System.Collections;
using System.Collections.Generic;
#if !UNITY_IPHONE && !UNITY_ANDROID
using MiniJSON;
#endif



public class BillingManager {

    private static BillingManager m_instance;
    public static BillingManager instance { get { if (m_instance == null) m_instance = new BillingManager(); return m_instance; } }
#if UNITY_ANDROID
    private string[] copiedSkus = null;
#endif
    //public  string JsonPurchaseHardCurrency( string itemCode, string platform, string transactionId, string value = "") {
    //    var keyValues = new Dictionary<string, object>
    //        {
    //            { "iid", itemCode },
    //            { "platform", platform },
    //            { "value", value}, 
    //            { "transactionId", transactionId }
    //        };
    //    Debug.Log("JsonPurchase() => " + Json.Serialize(keyValues));
    //    return Json.Serialize(keyValues);
    //}

    //static void useCoins(int hardCurrency, int softCurrency, int externalCurrency) {
    //        User.UserHardCurrency =     hardCurrency;
    //        User.UserSoftcurrency =     softCurrency;
    //        User.UserExternalCurrency = externalCurrency;
    //        UI_SoftCoins.Instance.updateCoins();
    //        UI_CompraMonedaPremium.Instance.update();
    //        }

    //This is the function you should call to bye InApp currency
    //Convert.ToBase64String(Encoding.UTF8.GetBytes(value))


    //public void purchaseHardCurrency(string itemCode, string platform, string url, string transactionId, string value = "") {

    //    string js_purchase = JsonPurchaseHardCurrency(itemCode, platform, transactionId, value);
    //    Debug.Log("PurchaseHardCurrency()" + js_purchase);
    //    DownloadDaemonJSON.instance.callWS(
    //        url + "/" + User.UserToken,
    //        // callback si todo OK
    //            (object _ret) => {

    //                Debug.Log("PurchaseHardCurrency()    >>>>     OK");

    //                Dictionary<string, System.Object> infoItem = _ret as Dictionary<string, System.Object>;

    //                if (infoItem != null) {

    //                    if (infoItem.ContainsKey("code")) {

    //                        Debug.Log("ERROR !!! code => " + infoItem["code"] + " , desc => " + infoItem["desc"] + "    >>>   Market  >>  PurchaseHardCurrency()   >>>  KO  !!!!");

    //                    } else {

    //                        int hardCurrency = (int) System.Convert.ToInt32(infoItem["hardCurrency"]);
    //                        int softCurrency = (int) System.Convert.ToInt32(infoItem["softCurrency"]);
    //                        int externalCurrency = (int) System.Convert.ToInt32(infoItem["externalCurrency"]);
    //                        useCoins(hardCurrency, softCurrency, externalCurrency);
    //                    }
    //                }
    //            },
    //        // callback si error
    //            (string _ret) => { Debug.LogError("ER >>>> Error => " + _ret); },
    //        // parametros de la llamada
    //    js_purchase);
    //}

//  public static void CallVerificationBackendService(string _url, StoreKitTransaction _transaction, DownloadDaemonJSON.callBack _onSuccedd, DownloadDaemonJSON.stringCallBack _onError) {

    //public void CallVerificationBackendService(string _url, GooglePurchase _purchase, DownloadDaemonJSON.callBack _onSuccedd, DownloadDaemonJSON.stringCallBack _onError) {
    public void CallVerificationBackendService(string _url, string _productId, string _purchaseToken, string _platform, DownloadDaemonJSON.callBack _onSucceed, DownloadDaemonJSON.stringCallBack _onError) {

        UnityEngine.Debug.Log("CallVerificationBackendService --> productName: " + _productId + " purchaseToken: " + _purchaseToken);
        // Calling the Verification WebService  

        //string productIdFake = "com.bitoon.soccerdudestcg.booster.2";
        //string purchaseTokenFake = "bnipeefpgddpanibmlpnpgbn.AO-J1OwimxtQxUzesd-kAWY5aSM8KxnEqc1rnBSZBPsNI99nAUdzOiuXAu3JX_t2I0tzc4lg8_PHNSOSGbtWCG3cMvBDMxypCwElFlrIdekOxvVUVVYomLbGkWUjEsLNLz53-qoHO7fiXbT8DzpnEVz61W7V09Emxg";

        string param = "{\"productId\":\"" + /*productIdFake*/_productId + "\", \"purchaseToken\":\"" + /*purchaseTokenFake*/_purchaseToken + "\", \"platform\":" + _platform + "}";
        /*for (int i=0; i<5; ++i)*/ DownloadDaemonJSON.instance.callWS(_url, _onSucceed, _onError, param);
        
  }


    public void purchaseProduct(string product) {
#if UNITY_ANDROID
        GoogleIAB.purchaseProduct(product);
#elif UNITY_IPHONE
        StoreKitBinding.purchaseProduct(product, 1);
#else
/*        string url = UI.Instance.getURL(product);
        FB.Canvas.Pay(url, "purchaseitem", 1, null, null, null, null, null, delegate(FBResult response) {
            object resp = Json.Deserialize(response.Text);
            Dictionary<string, object> infoItem = resp as Dictionary<string, object>;
            if (infoItem != null) {//HACKKKAKAKAKKAKKA
                //if (product == "com.bitoon.classicgolf.punadogemas") { //joyero
                //    //purchaseHardCurrency(product, "facebook", LoadXmlData.baseUrl + "/golfws/rest/market/exchange", "", "eyJwYXltZW50X2lkIjo0NDE0ODg0ODI2NDgwMTUsImFtb3VudCI6IjEwLjAwIiwiY3VycmVuY3kiOiJFVVIiLCJxdWFudGl0eSI6IjEiLCJzdGF0dXMiOiJjb21wbGV0ZWQiLCJzaWduZWRfcmVxdWVzdCI6Imx2Zkg3Smt2ajY3MjJPMTVVRE9KNkU5UjdpNlFNczRTajJLUVRQdU9JYVEuZXlKaGJHZHZjbWwwYUcwaU9pSklUVUZETFZOSVFUSTFOaUlzSW1GdGIzVnVkQ0k2SWpFd0xqQXdJaXdpWTNWeWNtVnVZM2tpT2lKRlZWSWlMQ0pwYzNOMVpXUmZZWFFpT2pFek9UZ3lOVEV5T0RBc0luQmhlVzFsYm5SZmFXUWlPalEwTVRRNE9EUTRNalkwT0RBeE5Td2ljWFZoYm5ScGRIa2lPaUl4SWl3aWMzUmhkSFZ6SWpvaVkyOXRjR3hsZEdWa0luMCJ9");
                //} else if (product == "com.bitoon.classicgolf.bolsagemas") {  //saco
                //    //purchaseHardCurrency(product, "facebook", LoadXmlData.baseUrl + "/golfws/rest/market/exchange", "", "eyJwYXltZW50X2lkIjo0NDE0ODg0ODkzMTQ2ODEsImFtb3VudCI6IjI1LjAwIiwiY3VycmVuY3kiOiJFVVIiLCJxdWFudGl0eSI6IjEiLCJzdGF0dXMiOiJjb21wbGV0ZWQiLCJzaWduZWRfcmVxdWVzdCI6IjZyMW8yX1IxQ1pOdHlHeUdvZXdGYjlKbTZ3WUxxV3FIajYzRWl1SHlBLW8uZXlKaGJHZHZjbWwwYUcwaU9pSklUVUZETFZOSVFUSTFOaUlzSW1GdGIzVnVkQ0k2SWpJMUxqQXdJaXdpWTNWeWNtVnVZM2tpT2lKRlZWSWlMQ0pwYzNOMVpXUmZZWFFpT2pFek9UZ3lOVEV6TkRZc0luQmhlVzFsYm5SZmFXUWlPalEwTVRRNE9EUTRPVE14TkRZNE1Td2ljWFZoYm5ScGRIa2lPaUl4SWl3aWMzUmhkSFZ6SWpvaVkyOXRjR3hsZEdWa0luMCJ9");
                //} else if (product == "com.bitoon.classicgolf.joyerogemas") {//cofre
                //    //purchaseHardCurrency(product, "facebook", LoadXmlData.baseUrl + "/golfws/rest/market/exchange", "", "eyJwYXltZW50X2lkIjo0NDE0ODg0OTI2NDgwMTQsImFtb3VudCI6IjUwLjAwIiwiY3VycmVuY3kiOiJFVVIiLCJxdWFudGl0eSI6IjEiLCJzdGF0dXMiOiJjb21wbGV0ZWQiLCJzaWduZWRfcmVxdWVzdCI6Ill3eEJTM2p2WWlrVjdQbVhJemM5STluSUZoMklwaWw1Z3p4Y0UwNUNJTEUuZXlKaGJHZHZjbWwwYUcwaU9pSklUVUZETFZOSVFUSTFOaUlzSW1GdGIzVnVkQ0k2SWpVd0xqQXdJaXdpWTNWeWNtVnVZM2tpT2lKRlZWSWlMQ0pwYzNOMVpXUmZZWFFpT2pFek9UZ3lOVEUwTWpJc0luQmhlVzFsYm5SZmFXUWlPalEwTVRRNE9EUTVNalkwT0RBeE5Dd2ljWFZoYm5ScGRIa2lPaUl4SWl3aWMzUmhkSFZ6SWpvaVkyOXRjR3hsZEdWa0luMCJ9");
                //} else if (product == "com.bitoon.classicgolf.sacogemas") { // camion
                //    //purchaseHardCurrency(product, "facebook", LoadXmlData.baseUrl + "/golfws/rest/market/exchange", "", "eyJwYXltZW50X2lkIjo0NDE0ODg1MzkzMTQ2NzYsImFtb3VudCI6IjEwMC4wMCIsImN1cnJlbmN5IjoiRVVSIiwicXVhbnRpdHkiOiIxIiwic3RhdHVzIjoiY29tcGxldGVkIiwic2lnbmVkX3JlcXVlc3QiOiJWUWR6eXBKMnBETGoyU0lBZ0NoaGd5NThwTUtFdW9IeWVFOW52cTROcjkwLmV5SmhiR2R2Y21sMGFHMGlPaUpJVFVGRExWTklRVEkxTmlJc0ltRnRiM1Z1ZENJNklqRXdNQzR3TUNJc0ltTjFjbkpsYm1ONUlqb2lSVlZTSWl3aWFYTnpkV1ZrWDJGMElqb3hNems0TWpVeU16UXpMQ0p3WVhsdFpXNTBYMmxrSWpvME5ERTBPRGcxTXprek1UUTJOellzSW5GMVlXNTBhWFI1SWpvaU1TSXNJbk4wWVhSMWN5STZJbU52YlhCc1pYUmxaQ0o5In0=");
                //} else if (product == "com.bitoon.classicgolf.cofregemas") { //000000
                //    //purchaseHardCurrency(product, "facebook", LoadXmlData.baseUrl + "/golfws/rest/market/exchange", "", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
                //} else ;
                ////purchaseHardCurrency(product, "facebook", LoadXmlData.baseUrl + "/golfws/rest/market/exchange", "",Convert.ToBase64String(Encoding.UTF8.GetBytes(response.Text)));
            }
       });
 */
#endif
    }


    public void init(string key, string[] skus) {
#if UNITY_ANDROID
        GoogleIAB.init( key);
        //copiedSkus = new string[skus.Length];
        copiedSkus = skus;
        //GoogleIAB.queryInventory(skus);
#elif UNITY_IPHONE
      bool canMakePayments = StoreKitBinding.canMakePayments();
      if (canMakePayments)
        StoreKitBinding.requestProductData(skus);
      else {
        ShowMessages sm = new ShowMessages();
        sm.ShowMessageWindow("ERROR ACCESSING APPLE STORE", "You can't connect to Apple Store, network error detected");
      }
#endif
    }

    public void HandlePurchase(Dictionary<string, object> resp, string productID) {
        //User.UserHardCurrency =     (int) System.Convert.ToInt32(resp["hardCurrency"]);
        //User.UserSoftcurrency =     (int) System.Convert.ToInt32(resp["softCurrency"]);
        //User.UserExternalCurrency = (int) System.Convert.ToInt32(resp["externalCurrency"]);
        //UI_SoftCoins.Instance.updateCoins();
        //UI_CompraMonedaPremium.Instance.update();
#if UNITY_ANDROID
        GoogleIAB.consumeProduct(productID);
#endif
        PurchaseManager.PerformPurchase(productID);
    }

    public void QueryInventory() {
#if UNITY_ANDROID
      GoogleIAB.queryInventory(copiedSkus);
#endif
    }

}