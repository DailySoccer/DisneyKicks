using UnityEngine;
using System.Collections;

public class GeneralSounds_menu : MonoBehaviour {
  public static GeneralSounds_menu instance;
  public AudioClip normalClickClip;
  public AudioClip popupOnClip;
  public AudioClip popupOffClip;
  public AudioClip selectorClip;
  public AudioClip compraClip;
  public AudioClip equiparEscudoClip;
  public AudioClip desequiparEscudoClip;
  public AudioClip confirmClip;
  public AudioClip backClip;
  public AudioClip compraMonedaClip;
  public AudioClip logroDesbloqueadoClip;

  float originalVolume = 0f;

  void Awake()
  {
    // TODO: Se intenta inicializar varias veces (después de volver de un partido)
    if (instance == null) {
        instance = this;
    }
    originalVolume = GetComponent<AudioSource>().volume;
  }

    void Start()
    {
        GetComponent<AudioSource>().volume = 0f;
        SetVolume();
    }

    void SetVolume()
    {
        if(ifcOpciones.fx)
        {
            GetComponent<AudioSource>().volume = originalVolume;
        }
        else
        {
            GetComponent<AudioSource>().volume = 0f;
        }
    }

    public void click()
    {
        playOneShot(normalClickClip);
    }

    public void back()
    {
        playOneShot(backClip);
    }

    public void comprar()
    {
        playOneShot(compraClip);
    }

    public void select()
    {
        playOneShot(selectorClip);
    }

    public void playOneShot(AudioClip _clip)
    {
        if(ifcOpciones.fx) GetComponent<AudioSource>().PlayOneShot(_clip);
        Debug.Log("playing sound: " + _clip);
    }
}
