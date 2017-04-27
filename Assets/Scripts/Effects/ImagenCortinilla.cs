using UnityEngine;
using System.Collections;
using System;

public class ImagenCortinilla : MonoBehaviour {
  String m_url = "https://s3-eu-west-1.amazonaws.com/kicksdemo/cortinilla.ogv";

  // Use this for initialization
  void Start ()
  {
    Reload();
  }

  public void Reload()
  {
    StartCoroutine("LoadCortinilla");
  }

  private IEnumerator LoadCortinilla()
  {
      WWW www = new WWW("http://s3-eu-west-1.amazonaws.com/bitoonkicks-pro/cortinillas/cortinilla.ini");

      while(!www.isDone)
      {
          yield return www;
      }
  
      try
      {           
          if (www.error != null)
              Debug.Log("DownloadFile(): WWW Error - " + www.error);
          else
          {
              int totalCortinillas = 0;
              int.TryParse(www.data, out totalCortinillas);
              //GetComponent<GUITexture>().pixelInset = new Rect(-628f * ifcBase.scaleFactor, -353f * ifcBase.scaleFactor, 1256f * ifcBase.scaleFactor, 706f * ifcBase.scaleFactor);
              StartCoroutine("LoadCortinillaNum", totalCortinillas);
              Debug.Log ("Publi ready. " + www.data + " cortinillas.");
          }
      }
      catch(Exception ex)
      {
          Cortinilla.instance.ShowRandomImage();
          Debug.Log("DownloadFile(): Exception - " + ex.Message);
          Exception innerEx = ex.InnerException;
          while (innerEx != null)
          {
              Debug.Log("DownloadFile(): Inner Exception - " + innerEx.Message);
              innerEx = innerEx.InnerException;
          }
      }
  }

  private IEnumerator LoadCortinillaNum(int _total)
  {
      WWW www = new WWW("http://s3-eu-west-1.amazonaws.com/bitoonkicks-pro/cortinillas/cortinilla" + UnityEngine.Random.Range(1, _total + 1) + ".jpg");

      while(!www.isDone)
      {
          yield return www;
      }
  
      try
      {
          if (www.error != null)
              Debug.Log("DownloadFile(): WWW Error - " + www.error);
          else
          {
              GetComponent<GUITexture>().texture = www.texture;
              GetComponent<GUITexture>().pixelInset = new Rect(-628f * ifcBase.scaleFactor, -353f * ifcBase.scaleFactor, 1256f * ifcBase.scaleFactor, 706f * ifcBase.scaleFactor);
          }
      }
      catch(Exception ex)
      {
          Cortinilla.instance.ShowRandomImage();
          Debug.LogError("DownloadFile(): Exception - " + ex.Message);
          Exception innerEx = ex.InnerException;
          while (innerEx != null)
          {
              Debug.Log("DownloadFile(): Inner Exception - " + innerEx.Message);
              innerEx = innerEx.InnerException;
              }
      }
  }

  /*private IEnumerator DownloadFile()
  {
      WWW www = new WWW(m_url);

      while(!www.movie.isReadyToPlay)
      {
          yield return www;
      }
  
      try
      {           
          if (www.error != null)
              Debug.Log("DownloadFile(): WWW Error - " + www.error);
          else
          {
              GetComponent<GUITexture>().texture = www.movie;
              ((MovieTexture)GetComponent<GUITexture>().texture).loop = true;
              ((MovieTexture)GetComponent<GUITexture>().texture).Play();
                GetComponent<GUITexture>().pixelInset = new Rect(-470f * ifcBase.scaleFactor, -352.5f * ifcBase.scaleFactor, 940f * ifcBase.scaleFactor, 705f * ifcBase.scaleFactor);
              Debug.Log ("Publi ready.");
          }
      }
      catch(Exception ex)
      {
          Debug.LogError("DownloadFile(): Exception - " + ex.Message);
          Exception innerEx = ex.InnerException;
          while (innerEx != null)
          {
              Debug.Log("DownloadFile(): Inner Exception - " + innerEx.Message);
              innerEx = innerEx.InnerException;
              }
      }
  }*/
}
