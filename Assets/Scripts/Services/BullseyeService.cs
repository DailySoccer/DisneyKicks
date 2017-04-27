using System;

public class BullseyeService : IBullseyeService, IDisposable  {

  public BullseyeService(){
    ServiceLocator.Register<IBullseyeService>( this );
  }

  private event Action<BullseyeImpactInfo> bullseyeImpacted = null;

  public void RegisterListener(Action<BullseyeImpactInfo> listener) {
    bullseyeImpacted += listener;
  }

  public void UnregisterListener(Action<BullseyeImpactInfo> listener) {
    bullseyeImpacted -= listener;
  }

  public void OnBullseyeImpacted(BullseyeImpactInfo bullseyeImpactInfo) {
    if (bullseyeImpacted != null) {
      bullseyeImpacted( bullseyeImpactInfo );
    }
  }

  public void Dispose() {
    ServiceLocator.Remove<IBullseyeService>();
  }	
}
