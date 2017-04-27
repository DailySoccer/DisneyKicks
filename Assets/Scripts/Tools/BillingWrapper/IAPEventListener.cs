using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class IAPEventListener: MonoBehaviour {
#if UNITY_IPHONE
  private static string productID;


    void OnEnable() {
        // Listens to all the StoreKit events. All event listeners MUST be removed before this object is disposed!
        StoreKitManager.transactionUpdatedEvent += transactionUpdatedEvent;
        StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
        StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
        StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
        StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
        StoreKitManager.productListReceivedEvent += productListReceivedEvent;
        StoreKitManager.productListRequestFailedEvent += productListRequestFailedEvent;
        StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
        StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
        StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;
    }


    void OnDisable() {
        // Remove all the event handlers
        StoreKitManager.transactionUpdatedEvent -= transactionUpdatedEvent;
        StoreKitManager.productPurchaseAwaitingConfirmationEvent -= productPurchaseAwaitingConfirmationEvent;
        StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
        StoreKitManager.purchaseCancelledEvent -= purchaseCancelledEvent;
        StoreKitManager.purchaseFailedEvent -= purchaseFailedEvent;
        StoreKitManager.productListReceivedEvent -= productListReceivedEvent;
        StoreKitManager.productListRequestFailedEvent -= productListRequestFailedEvent;
        StoreKitManager.restoreTransactionsFailedEvent -= restoreTransactionsFailedEvent;
        StoreKitManager.restoreTransactionsFinishedEvent -= restoreTransactionsFinishedEvent;
        StoreKitManager.paymentQueueUpdatedDownloadsEvent -= paymentQueueUpdatedDownloadsEvent;
    }



    void transactionUpdatedEvent(StoreKitTransaction transaction) {
        Debug.Log("transactionUpdatedEvent: " + transaction);
    }


#region Query Inventory --------------------------
    void productListReceivedEvent(List<StoreKitProduct> productList) {
        Debug.Log("productListReceivedEvent. total products received: " + productList.Count);

        // print the products to the console
        foreach (StoreKitProduct product in productList)
            Debug.Log(product.ToString() + "\n");

        // añadir los productos al inventario de la tienda
        PurchaseManager.InventarioTienda = productList;
    }


    void productListRequestFailedEvent(string error) {
        Debug.Log("productListRequestFailedEvent: " + error);
    }
#endregion


  #region Purchase ---------------------------------
    void purchaseFailedEvent(string error) {
        Debug.Log("purchaseFailedEvent: " + error);
        ShowMessages sm = new ShowMessages();
        sm.ShowMessageWindow("Error Purchasing", "Error purchasing");
    }


    void purchaseCancelledEvent(string error) {
        Debug.Log("purchaseCancelledEvent: " + error);
        //ShowMessages sm = new ShowMessages();
        //sm.ShowMessageWindow("Warning", "Purchase canceled by user");
    }


    void productPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction) {
        Debug.Log("productPurchaseAwaitingConfirmationEvent: " + transaction);
        InAppPersistenceManager.instance.SetPurchaseInfo(transaction.productIdentifier, InAppPersistenceManager.InAppPersistenceState.purchase_verification_pending);
        productID = transaction.productIdentifier;
        BillingManager.instance.HandlePurchase(null, productID); //modificado porque no tenemos backend
        //BillingManager.instance.CallVerificationBackendService(Trunk.verificationURL + Trunk.userToken, productID, transaction.base64EncodedTransactionReceipt, "2", onVerificationSucceeded, onVerificationError);
    }


    void purchaseSuccessfulEvent(StoreKitTransaction transaction) {
        Debug.Log("purchaseSuccessfulEvent: " + transaction);        
        //InAppPersistenceManager.instance.SetPurchaseInfo(transaction.productIdentifier, InAppPersistenceManager.InAppPersistenceState.purchase_verification_pending);
        //productID = transaction.productIdentifier;
        //BillingManager.instance.CallVerificationBackendService(Trunk.verificationURL + Trunk.userToken, productID, transaction.base64EncodedTransactionReceipt, "2", onVerificationSucceeded, onVerificationError);
    }

    public static void onVerificationSucceeded(object _ret) {
      Dictionary<string, object> resp = (Dictionary<string, object>)_ret;
      if (resp != null && resp.ContainsKey("code")) {
        int code = (int)(System.Int64)resp["code"];
        switch (code) {
          case 0:// All was right
            Debug.Log("Verification succeeded");
            InAppPersistenceManager.instance.ErasePurchaseInfo(productID);
            BillingManager.instance.HandlePurchase(resp, productID);
            break;

          case -10: { // Purchase ID not matching the purchase token
              Debug.Log("Verification failed: PurchaseID incorrect. Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Purchase Id incorrect");
            }
            break;

          case -14: // Purchase duplicated
          case -32: {
              Debug.Log("Verification failed: PurchaseID duplicated. Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Purchase Id duplicated");
            }
            break;

          case 400: { // Purchase TOKEN not valid (doesn't match the ProductID)
              Debug.Log("Verification failed: Purchase Token incorrect. Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Purchase Token incorrect");
            }
            break;

          default: {
              Debug.Log("Verification failed: Generic Error Code: " + code);
              ShowMessages sm = new ShowMessages();
              sm.ShowMessageWindow("Error: purchase failed", "Generic Purchase Error");
            }
            break;
        }
      }
    }


    public static void onVerificationError(string _ret) {
      ShowMessages sm = new ShowMessages();
      sm.ShowMessageWindow("Error: confirmation failed. ErrorInfo: ", _ret);
    }
  #endregion


    void restoreTransactionsFailedEvent(string error) {
        Debug.Log("restoreTransactionsFailedEvent: " + error);
    }


    void restoreTransactionsFinishedEvent() {
        Debug.Log("restoreTransactionsFinished");
    }


    void paymentQueueUpdatedDownloadsEvent(List<StoreKitDownload> downloads) {
        Debug.Log("paymentQueueUpdatedDownloadsEvent: ");
        foreach (var dl in downloads)
            Debug.Log(dl);
    }

#endif
}