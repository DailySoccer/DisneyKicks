
using UnityEngine;
using System.Collections;
using System;
using Chartboost;


public class ChartboostPersistentObject : MonoBehaviour {

    public string charboostAppID_Android = "5305d5252d42da07656491d3";
    public string charboostAppSignature_Android = "94c6b300f342d95300306354d1fe8f214b3d548d";
    public string charboostAppID_IOS = "5304dac42d42da5a1a7565c9";
    public string charboostAppSignature_IOS = "c6e0540dbda4fa0b22702666d26b270bfc8fab4d";
	// Use this for initialization
	void Start () {
        Debug.Log("ChartboostPersistentObject => Start()");
#if UNITY_ANDROID
        CBBinding.init(charboostAppID_Android, charboostAppSignature_Android);
        Debug.Log("ChartboostPersistentObject => Start() => CBBinding.init("+charboostAppID_Android+", "+charboostAppSignature_Android+") => CBBinding.cacheInterstitial(null)");
#elif UNITY_IPHONE
        CBBinding.init(charboostAppID_IOS, charboostAppSignature_IOS);
        Debug.Log("ChartboostPersistentObject => Start() => CBBinding.init(" + charboostAppID_IOS + ", " + charboostAppSignature_IOS + ") => CBBinding.cacheInterstitial(null)");
#endif
        //CBBinding.cacheInterstitial(null);
        //ShowChartboostAdd();
	}
	
    public void ShowChartboostAdd() {
        Debug.Log("ChartboostPersistentObject => ShowChartboostAdd() => CBBinding.showInterstitial(null)");
        CBBinding.showInterstitial(null);
    }


    void OnEnable()
    {
        // Initialize the Chartboost plugin
#if UNITY_ANDROID
        // Replace these with your own Android app ID and signature from the Chartboost web portal
        CBBinding.init(charboostAppID_Android, charboostAppSignature_Android);
        Debug.Log("ChartboostPersistentObject => OnEnable() => CBBinding.init(" + charboostAppID_Android + ", " + charboostAppSignature_Android + ")");
#elif UNITY_IPHONE
        // Replace these with your own iOS app ID and signature from the Chartboost web portal
        CBBinding.init(charboostAppID_IOS, charboostAppSignature_IOS);
        Debug.Log("ChartboostPersistentObject => OnEnable() => CBBinding.init(" + charboostAppID_IOS + ", " + charboostAppSignature_IOS + ")");
#endif
    }


#if UNITY_ANDROID
    void OnApplicationPause(bool paused) {
        // Manage Chartboost plugin lifecycle
        CBBinding.pause(paused);
    }

    void OnDisable() {
        // Shut down the Chartboost plugin
        CBBinding.destroy();
    }
#endif


#if UNITY_ANDROID
    public void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            if (CBBinding.onBackPressed())
                return;
            else
                Application.Quit();
        }
    }
#endif
}
