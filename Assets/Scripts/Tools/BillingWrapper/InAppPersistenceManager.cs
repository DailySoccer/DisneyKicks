using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

// To access PlayerPrefs in Android device you need to root it and find this file: /data/data/<yourPackageName>/shared_prefs/<youPackageName>.xml


public class ModifDict<K, V> : Dictionary<K, V> {

  new public void Add(K _key, V _val) {
    if (!ContainsKey(_key)) base.Add(_key, _val);
    else this[_key] = (V)(object)_val;
  }
}



public class InAppPersistenceManager {
  private static InAppPersistenceManager m_instance;
  public static InAppPersistenceManager instance { get { if (m_instance == null) m_instance = new InAppPersistenceManager(); return m_instance; } }

  public enum InAppPersistenceState : int { none = -1, purchase_pending = 0, purchase_verification_pending, consume_pending };  // -1 value means no pending operation
  public ModifDict<string, InAppPersistenceState> pendingOperations = new ModifDict<string, InAppPersistenceState>();


  public List<KeyValuePair<string, InAppPersistenceState>> Init(String[] _skus) {
    Load(_skus); // Prepare the local system to handle incompleted purchases 

    List<KeyValuePair<string, InAppPersistenceState>> purchases = GetPurchasesInfo();
    Debug.Log(">> >> >> >> >> PendingPurchases: " + purchases.Count);

    foreach (KeyValuePair<string, InAppPersistenceState> purchase in purchases) {
      switch (purchase.Value) {
        case InAppPersistenceState.purchase_pending: // The user has try the purchase but the answer from Google Play hasn't arrive
          Debug.Log("Purchase: " + purchase.Key + " in state: " + purchase.Value);
          break;

        case InAppPersistenceState.purchase_verification_pending: // The user tries the verification of his purchase but our Backend servers don't answer
          Debug.Log("Purchase: " + purchase.Key + " in state: " + purchase.Value);
          break;

        case InAppPersistenceState.consume_pending: // The user has try the purchase but the answer from Google Play
          Debug.Log("Purchase: " + purchase.Key + " in state: " + purchase.Value);
          break;
      }
    }
    return purchases; // useful if you don't handle the previous purchases inside this method
  }


  public void Load(string[] _skus) {
    Debug.Log(">>> >>> >>> Load: " + _skus.Length);
    if (pendingOperations != null || pendingOperations.Count > 0) pendingOperations.Clear();
    pendingOperations = new ModifDict<string, InAppPersistenceState>();
    foreach (string sku in _skus){
      if (PlayerPrefs.HasKey(sku)) {
        pendingOperations.Add(sku, (InAppPersistenceState)PlayerPrefs.GetInt(sku));
      }
    }
  }


  public void Save() {
    PlayerPrefs.Save();
  }


  public void SetPurchaseInfo(string _productId, InAppPersistenceState _value, bool _save = true) {
    Debug.Log(">>> >>> >>> SetPurchaseInfo: " + _productId + " value: "  + _value);
    //Debug.Log(">>> >>> >>> SetPurchaseInfo: pendingOperations items: " + pendingOperations.Count);
    pendingOperations.Add(_productId, _value);
    Debug.Log(">>> >>> >>> SetPurchaseInfo: pendingOperations items: " + pendingOperations.Count + " key: " + _productId + " value" + (InAppPersistenceState)pendingOperations[_productId]);
    PlayerPrefs.SetInt(_productId, (int)_value);

    Debug.Log(">>> >>> >>> SetPurchaseInfo: PlayerPrefs has key: " + _productId + "? " + PlayerPrefs.HasKey(_productId) + " with value: " + (InAppPersistenceState)PlayerPrefs.GetInt(_productId));

    if (_save) Save();
  }


  public InAppPersistenceState GetPurchaseInfo(string _productId) {
    if (PlayerPrefs.HasKey(_productId)) {
      return (InAppPersistenceState)PlayerPrefs.GetInt(_productId);
    }
    return InAppPersistenceState.none;
  }


  public List<KeyValuePair<string, InAppPersistenceState>> GetPurchasesInfo() {
    return pendingOperations.ToList();
  }


  public void ErasePurchaseInfo(string _productId, bool _save = true) {
    if (PlayerPrefs.HasKey(_productId)) {
      PlayerPrefs.DeleteKey(_productId);
      pendingOperations.Remove(_productId);
      if (_save) Save();
    }
  }

}
