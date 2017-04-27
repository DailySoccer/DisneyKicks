using UnityEngine;
using System.Collections.Generic;
using System;
using Prime31;

using ListItem = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, InAppPersistenceManager.InAppPersistenceState>>;


public class initClass : MonoBehaviourGUI {

  public static String[] skus = new string[] { "com.bitoon.soccerdudestcg.paco1", "com.bitoon.soccerdudestcg.paco2", "com.bitoon.soccerdudestcg.booster.1", "com.bitoon.soccerdudestcg.booster.2" };

	void OnGUI() {
		beginColumn();


    if (GUILayout.Button("Initialize InApp System")) {
#if UNITY_ANDROID || UNITY_IPHONE
      InAppPersistenceManager.instance.Init(skus);
      BillingManager.instance.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArl1eY+yE+wBG4NAfbXRnQ5A+Io1kBtmZseYB1pAJAjRqmT7wtDbEIETKsSI8Bd0VRI3HgJbaPo7pCpZlm60pPnVf012YFIVx0C+WQEHFVDHO8a8/AMCUpoplzOiq3gP8/hn7qUKoxYi1SQGin81bvKoVZmv6T5T/COoRsLM2VKpcMXEOFyAhWP0fKlohkncZTe3NSVBEjB4xjNmgLGEmxNUG2PP+FY8MW5UwL/82XhIL2anDBUCCp+9l3j4EX9Ge2lTxWLzqHAuyV+opxiUwjj9/e5tfyUsB/vHoUtxrOVIAB3OVFqxen3JGH+MNEnzTxEybHvS1wOqsrmV5CS8LmQIDAQAB", skus);
#endif
    }


		if (GUILayout.Button( "Purchase Booster 1")) {
			Debug.Log( "Purchase Booster 1 button pressed ");
      BillingManager.instance.purchaseProduct(skus[2]);  // Item Booster 1
      //InAppPersistenceManager.instance.SetPurchaseInfo(skus[2], InAppPersistenceManager.InAppPersistenceState.purchase_pending);
		}


    if (GUILayout.Button("Purchase Booster 2")) {
      Debug.Log("Purchase Booster 2 button pressed ");
      BillingManager.instance.purchaseProduct(skus[3]);  // Item Booster 2
      //InAppPersistenceManager.instance.SetPurchaseInfo(skus[3], InAppPersistenceManager.InAppPersistenceState.purchase_pending);
    }


		if (GUILayout.Button("Android only: Consume Purchases")) {
      Debug.Log("ANDROID only: Consume purchases button pressed");
#if UNITY_ANDROID
      foreach (string sku in skus) {
        GoogleIAB.consumeProduct(sku);
      }
#endif
		}


		if (GUILayout.Button("List Pending Purchases. Android consume them too" )) {
      Debug.Log("List Pending Purchases. ANDROID only: consume them too");
      ListItem purchases = InAppPersistenceManager.instance.GetPurchasesInfo();
      Debug.Log("pending purchases: " + purchases.Count);
      foreach (KeyValuePair<string, InAppPersistenceManager.InAppPersistenceState> purchase in purchases) {
        Debug.Log(purchase.Key + " in " + (InAppPersistenceManager.InAppPersistenceState)purchase.Value);
#if UNITY_ANDROID
        GoogleIAB.consumeProduct(purchase.Key);
#endif
      }
		}
    

		endColumn( true );


    // This just can be called after press the "Initialize InApp System" button, i.o.c. it will return a null list
    if (GUILayout.Button("Query Inventory. After Init Process please")) {      
      BillingManager.instance.QueryInventory();
    }


    if (GUILayout.Button("List Pending Purchases. Android only consume consume_pending")) {
      ListItem purchases = InAppPersistenceManager.instance.GetPurchasesInfo();
      Debug.Log("pending purchases: " + purchases.Count);
      foreach (KeyValuePair<string, InAppPersistenceManager.InAppPersistenceState> purchase in purchases) {
        Debug.Log(purchase.Key + " in " + (InAppPersistenceManager.InAppPersistenceState)purchase.Value);
        if ((InAppPersistenceManager.InAppPersistenceState)purchase.Value == InAppPersistenceManager.InAppPersistenceState.consume_pending) {
#if UNITY_ANDROID
          GoogleIAB.consumeProduct(purchase.Key);
#endif
        }
      }
    }


		if( GUILayout.Button( "FB Login?" ) ) {
      // FACEBOOK CODE ----------------
      //string pass = SHA1Util.SHA1HashStringForUTF8String("123");
      //string token = SystemInfo.deviceUniqueIdentifier + pass;

      ////DownloadDaemonJSON.instance.callWS("/user/signup", (json) => {
      ////Dictionary<string,object> resp = (Dictionary<string,object>)json;
      ////int code = (int)(System.Int64)resp["code"];
      ////Debug.Log(">>> signup " + code);
      ////if (code < 0) {
      ////    throw (new Exception("Error al logarse"));
      ////}
      ////}, (error) => {
      ////    Debug.Log("[signup] Error " + error);
      ////}, "{\"userName\":\"" + token + "\", \"password\":\"\"}");

      ////DownloadDaemonJSON.instance.callWS("/user/login", (json) => {
      ////    Dictionary<string, object> resp = (Dictionary<string, object>)json;
      ////    int code = (int)(System.Int64)resp["code"];
      ////    Debug.Log(">>> Log " + code);
      ////    if (code < 0) {
      ////        throw (new Exception("Error al logarse"));
      ////    }
      ////}, (error) => {
      ////    Debug.Log("[signup] Error " + error);
      ////}, "{\"userName\":\"" + token + "\", \"password\":\"\", \"type\":1}");

      ////  string token_fb = "CAAI0KbKYMcIBAAAEyfPhRhplJVcPYJRr3KYCPtbllZCZBMKnakooEdfiuV4zCglFqWVczqxQ1nVvGiDiN2ALwOKm55t7xWSUuG7RekdzZA3qByMAweOvDWhgej9NkEH0lNvFNFhhZCxtK5CjdfjeVzhNVxL8BGHnvEVQQUZBCrxCuNPekT6y5H3arswJWW2Fz9gDDn1rcBHKv76uUgCPIonQMbSVsuMOaWafMzC4PcAZDZD";
      ////  int id_fb = 542609263;
      ////  DownloadDaemonJSON.instance.callWS("/user/link_facebook", (json) => {
      ////    Dictionary<string, object> resp = (Dictionary<string, object>)json;
      ////    int code = (int)(System.Int64)resp["code"];
      ////    Debug.Log(">>> link_facebook " + code);
      ////    if (code < 0) {
      ////      throw (new Exception("Error al logarse"));
      ////    }
      ////  }, (error) => {
      ////    Debug.Log("[signup] Error " + error);
      ////  }, "{\"userName\":\"" + token + "\", \"password\":\"" + token_fb + "\"}");
      //////}, "{\"userName\":\"" + token + "\", \"password\":\"" + token_fb + "\", \"facebookId\":"+id_fb+"}");

      string token_fb = "CAAI0KbKYMcIBAAAEyfPhRhplJVcPYJRr3KYCPtbllZCZBMKnakooEdfiuV4zCglFqWVczqxQ1nVvGiDiN2ALwOKm55t7xWSUuG7RekdzZA3qByMAweOvDWhgej9NkEH0lNvFNFhhZCxtK5CjdfjeVzhNVxL8BGHnvEVQQUZBCrxCuNPekT6y5H3arswJWW2Fz9gDDn1rcBHKv76uUgCPIonQMbSVsuMOaWafMzC4PcAZDZD";
      int id_fb = 542609263;
      DownloadDaemonJSON.instance.callWS("/user/login", (json) =>
      {
          Dictionary<string, object> resp = (Dictionary<string, object>)json;
          int code = (int)(System.Int64)resp["code"];
          Debug.Log(">>> Log " + code);
          if (code < 0) {
            throw (new Exception("Error al logarse"));
          } else {
            //IABEventListener.userToken = (string)(System.String)resp["token"];
            Debug.Log(">>> usertoken " + Trunk.userToken);
          }
      }, (error) => {
          Debug.Log("[signup] Error " + error);
      //}, "{\"userName\":\"" + id_fb + "\", \"password\":\"" + token_fb + "\", \"type\":2}");
      }, "{\"userName\":\"" + id_fb + "\", \"password\":\"" + token_fb + "\", \"type\":0}");			
		}


		if (GUILayout.Button("Purchase Paco 1")) {
      Debug.Log("Purchase Paco 1 button pressed ");
      BillingManager.instance.purchaseProduct(skus[0]);  // Item Paco 1
      //InAppPersistenceManager.instance.SetPurchaseInfo(skus[0], InAppPersistenceManager.InAppPersistenceState.purchase_pending);
		}


    if (GUILayout.Button("Purchase Paco 2")) {
      Debug.Log("Purchase Paco 2 button pressed ");
      BillingManager.instance.purchaseProduct(skus[1]);  // Item Paco 2
      //InAppPersistenceManager.instance.SetPurchaseInfo(skus[1], InAppPersistenceManager.InAppPersistenceState.purchase_pending);
		}

#if UNITY_ANDROID
    if (GUILayout.Button("List Pending Purchases")) {
      Debug.Log("ANDROID only -> List Pending Purchases.");
      ListItem purchases = InAppPersistenceManager.instance.GetPurchasesInfo();
      Debug.Log("Pending purchases: " + purchases.Count);
      foreach (KeyValuePair<string, InAppPersistenceManager.InAppPersistenceState> purchase in purchases) {
        Debug.Log(purchase.Key + " in " + (InAppPersistenceManager.InAppPersistenceState)purchase.Value);
      }
    }
#endif


		endColumn();
	}

}
