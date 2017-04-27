using UnityEngine;
using System;

public struct BullseyeImpactInfo{
	
	public int Points {get; set;}
	public Vector3 Position {get; set;}
	public float Distance {get; set;}
  public int Ring {get; set;}
}

public interface IBullseyeService {
	
  void RegisterListener(Action<BullseyeImpactInfo> listener);
  void UnregisterListener(Action<BullseyeImpactInfo> listener);

  void OnBullseyeImpacted(BullseyeImpactInfo bullEyeImpactInfo);
}
