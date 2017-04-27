//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

interface IUserMessages {

  void ShowMessageWindow(string _title, string _body);

}


// Class to be implemented in each game as this has to show our error window during the Purchase flow
public class ShowMessages : IUserMessages {

  public void ShowMessageWindow(string _title, string _body) {
    UnityEngine.Debug.Log(_body);
  }

}