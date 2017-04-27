using UnityEngine;
//using System;
//using System.Collections;
using System.Collections.Generic;


#region Google Server Response Codes
//BILLING_RESPONSE_RESULT_OK 	                  0 	Success
//BILLING_RESPONSE_RESULT_USER_CANCELED 	      1 	User pressed back or canceled a dialog
//BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE 	3 	Billing API version is not supported for the type requested
//BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE 	    4 	Requested product is not available for purchase
//BILLING_RESPONSE_RESULT_DEVELOPER_ERROR 	    5 	Invalid arguments provided to the API. This error can also indicate that the application was not correctly signed or properly set up for In-app Billing in Google Play, or does not have the necessary permissions in its manifest
//BILLING_RESPONSE_RESULT_ERROR 	              6 	Fatal error during the API action
//BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED 	  7 	Failure to purchase since item is already owned
//BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED 	      8 	Failure to consume since item is not owned
#endregion


public class IABEventListener : MonoBehaviour {
#if UNITY_ANDROID
  private static string productID;

  void OnEnable() {
    // Listen to all events for illustration purposes
    GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
    GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
    GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
    GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
    GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
    GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
    GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
    GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
    GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
  }


  void OnDisable() {
    // Remove all event handlers
    GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
    GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
    GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
    GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
    GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
    GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
    GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
    GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
    GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
  }

#region Billing -----------------------------------
  void billingSupportedEvent() {
    Debug.Log("billingSupportedEvent");
    BillingManager.instance.QueryInventory();
  }


  void billingNotSupportedEvent(string error) {
    Debug.Log("billingNotSupportedEvent: " + error);
    ShowMessages sm = new ShowMessages();
    sm.ShowMessageWindow("Error: billingNotSupportedEvent", error);
  }
#endregion


  #region Query Inventory --------------------------
  void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus) {
    Debug.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));
    Prime31.Utils.logObject(purchases);
    Prime31.Utils.logObject(skus);
  }


  void queryInventoryFailedEvent(string error) {
    Debug.Log("queryInventoryFailedEvent: " + error);
    ShowMessages sm = new ShowMessages();
    sm.ShowMessageWindow("Error: queryInventoryFailedEvent", error);
  }
#endregion


  #region Purchase ---------------------------------
  void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature) {
    Debug.Log(string.Format("purchaseCompleteAwaitingVerificationEvent. purchaseData: {0}, signature: {1}", purchaseData, signature));    
  }


  void purchaseSucceededEvent(GooglePurchase purchase) {
    Debug.Log("purchaseSucceededEvent: " + purchase.packageName + " | " + purchase.orderId + " | " + purchase.productId + ", token: " + purchase.purchaseToken);
    InAppPersistenceManager.instance.SetPurchaseInfo(purchase.productId, InAppPersistenceManager.InAppPersistenceState.purchase_verification_pending);
    productID = purchase.productId;
    BillingManager.instance.HandlePurchase(null, productID); //modificado porque no tenemos backend
    //BillingManager.instance.CallVerificationBackendService(Trunk.verificationURL + Trunk.userToken, productID, purchase.purchaseToken, "1", onVerificationSucceeded, onVerificationError);
  }


  public static void onVerificationSucceeded(object _ret) {
      Dictionary<string, object> resp = (Dictionary<string, object>) _ret;
      if (resp != null && resp.ContainsKey("code")) {
          int code = (int) (System.Int64) resp["code"];
          switch (code) {
            case 0:
              Debug.Log("Verification succeeded");
              InAppPersistenceManager.instance.SetPurchaseInfo(productID, InAppPersistenceManager.InAppPersistenceState.consume_pending);
              BillingManager.instance.HandlePurchase(resp, productID);
              break;

            case -10: { // Purchase ID not matching the purchase token
              Debug.Log("Verification failed: PurchaseID incorrect. Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Purchase Id incorrect");
              }
              break;

            case -14: { // Purchase duplicated
              Debug.Log("Verification failed: PurchaseID duplicated. Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Purchase Id duplicated");
              }
              break;

            case -29: { // Purchase TOKEN not valid (doesn't match the ProductID)
              Debug.Log("Verification failed: Purchase Token incorrect. Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Purchase Token incorrect");
              }
              break;

            default: {
              // 400 -> Wrong Purchase Token
              // 404 -> Purchase Token doesn't match the ProductId
              Debug.Log("Verification failed: Generic Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Generic Purchase Error");
              }
              break;
          }
      }
  }

  public static void onVerificationError(string _ret) {
    Debug.Log("Verification failed: " + _ret);
    ShowMessages sm = new ShowMessages();
    sm.ShowMessageWindow("Error: verification failed. ErrorInfo: ", _ret);
  }


  void purchaseFailedEvent(string error) {
    Debug.Log("purchaseFailedEvent: " + error);
    string response = "response: ";
    int size = error.LastIndexOf(":") - (error.IndexOf(response) + response.Length);    

    ShowMessages sm = new ShowMessages();
    switch (error.Substring(error.IndexOf(response) + response.Length, size)){
      case "-1005":
        sm.ShowMessageWindow("Error Purchasing", "USER HAS CANCELLED THIS PURCHASE");
        //InAppPersistenceManager.instance.ErasePurchaseInfo(ProductID);
        break;

      case "7":
        sm.ShowMessageWindow("Error Purchasing", "YOU OWN THIS OBJECT!!");
        //InAppPersistenceManager.instance.ErasePurchaseInfo(ProductID);
        break;
    }
  }
#endregion


  #region Consume Purchase -------------------------
  void consumePurchaseSucceededEvent(GooglePurchase purchase) {
    Debug.Log("consumePurchaseSucceededEvent: " + purchase);
    InAppPersistenceManager.instance.ErasePurchaseInfo(purchase.productId);
  }


  void consumePurchaseFailedEvent(string error) {
    Debug.Log("consumePurchaseFailedEvent: " + error);
  }
#endregion

#endif
}