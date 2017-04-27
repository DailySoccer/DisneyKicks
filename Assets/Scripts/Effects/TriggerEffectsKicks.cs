using UnityEngine;
using System.Collections;

public class TriggerEffectsKicks : MonoBehaviour
{
  public GameObject[] whenShoot;
  public GameObject[] whenGoal;
  public GameObject[] whenReset;

  public void Start()
  {
    ServiceLocator.Request<IDifficultyService>().RegisterListener(OnReset);
    ServiceLocator.Request<IShotService>().RegisterListener(OnShoot);
    ServiceLocator.Request<IShotResultService>().RegisterListener(OnGoal);
  }

  void OnShoot(ShotInfo _info)
  {
    SetActive(whenShoot, true);
  }

  void OnGoal(ShotResult _info)
  {
    SetActive(whenGoal, true);
    SetActive(whenReset, false);
  }

  void OnReset(ShotConfiguration _info)
  {
    SetActive (whenGoal, false);
    SetActive (whenShoot, false);
    SetActive (whenReset, true);
  }

  void SetActive(GameObject[] _objects, bool _mode)
  {
    foreach(GameObject go in _objects)
    {
      if(go != null) go.SetActive(_mode);
    }
  }
}
