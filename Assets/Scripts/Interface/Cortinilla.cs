using UnityEngine;
using System.Collections;


/// <summary>
/// Clase para mostrar la publicidad entre escenas
/// </summary>
public class Cortinilla : ifcBase
{

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // -----------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static Cortinilla instance { get; private set; }

    // imagenes para mostrar en la pantalla de publicidad
    public Texture[] m_texturasPublicidad;

    // audios
    public AudioClip cortinillaIn;
    public AudioClip cortinillaOut;

    WWW m_publi;

    // elementos de esta interfaz
    private GUITexture m_imgPublicidad;

    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // -----------------------------------------------------------------------------

    
    void Awake()
    {
        // asegurarse de que unicamente hay una instancia de esta clase => para utilizar el patron singleton
        if (instance == null) {
            instance = this;
        } else
            DestroyImmediate(gameObject);

        // mostrar un warning si no hay imagenes de publicidad
        if (m_texturasPublicidad == null || m_texturasPublicidad.Length == 0)
            Debug.LogWarning("El array de texturas 'm_texturasPublicidad' no esta inicializado");
    }


    
    // Use this for initialization
    void Start()
    {
        // hacer que este dialogo persista entre escenas
        DontDestroyOnLoad(gameObject);

        m_imgPublicidad = transform.FindChild("Publicidad").GetComponent<GUITexture>();
    }


    // Use this for initialization
    // Update is called once per frame

    public void Play()
    {
        //ShowRandomImage();

        GetComponent<AudioSource>().clip = cortinillaIn;
        Vector3 pos = transform.position;
        pos.x = -0.6f;
        transform.position = pos;
        if(ifcOpciones.fx) GetComponent<AudioSource>().Play();
        SuperTweener.Flush();
        ifcMusica.instance.musicOff(0.25f);
        new SuperTweener.move(gameObject, 0.25f, new Vector3(0.5f, 0.5f, 50f), SuperTweener.CubicOut, (_target) =>
        {
            Application.LoadLevel("unload");
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
            new SuperTweener.move(gameObject, 1.0f, new Vector3(0.5f, 0.5f, 50f), null, (_target2) =>
            {
                Application.LoadLevel("BBVAKicks");
                new SuperTweener.move(gameObject, 1.0f, new Vector3(0.5f, 0.5f, 50f), null, (_target5) =>
                {
                    new SuperTweener.move(gameObject, 0.25f, new Vector3(2.0f, 0.5f, 50f), SuperTweener.CubicIn, (_target3) =>
                    {
                        GetComponent<AudioSource>().clip = cortinillaOut;
                        if(ifcOpciones.fx)GetComponent<AudioSource>().Play();
                        if(GameplayService.networked)
                        {
                            MsgPlayerReady msg = Shark.instance.mensaje<MsgPlayerReady>();
                            msg.send();
                        }
                        m_imgPublicidad.gameObject.GetComponent<ImagenCortinilla>().Reload();
                    });
                });
            });
        });
    }


    public void Return(bool _gotoMenu = true)
    {
        //ShowRandomImage();

        GetComponent<AudioSource>().clip = cortinillaIn;
        Vector3 pos = transform.position;
        pos.x = 1.6f;
        transform.position = pos;
        if(ifcOpciones.fx)GetComponent<AudioSource>().Play();

        //SuperTweener.Flush();
        new SuperTweener.move(gameObject, 0.25f, new Vector3(0.5f, 0.5f, 50f), SuperTweener.CubicOut, (_target) =>
        {
            Application.LoadLevel("unload");
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
            new SuperTweener.move(gameObject, 1.0f, new Vector3(0.5f, 0.5f, 50f), null, (_target2) =>
            {
                Application.LoadLevel(_gotoMenu ? "Menus" : "BBVAKicks");
                new SuperTweener.move(gameObject, 1.0f, new Vector3(0.5f, 0.5f, 50f), null, (_target3) =>
                {
                    new SuperTweener.move(gameObject, 0.25f, new Vector3(-2.0f, 0.5f, 50f), SuperTweener.CubicIn, (_target4) =>
                    {
                        if(_gotoMenu) ifcMusica.instance.musicOn(0.25f);
                        if(ifcOpciones.fx)GetComponent<AudioSource>().Play();
                        // optimizar el rendimiento eliminando partes de interfaz no visibles
                        if(_gotoMenu) ifcMainMenu.instance.OcultarElementosDeInterfazNoVisibles();
                        GetComponent<AudioSource>().clip = cortinillaOut;
                        m_imgPublicidad.gameObject.GetComponent<ImagenCortinilla>().Reload();
                    });
                });
            });
        });
    }


    /// <summary>
    /// Muestra una imagen de publicidad al azar
    /// </summary>
    public void ShowRandomImage() {
        m_imgPublicidad.texture = m_texturasPublicidad[Random.Range(0, m_texturasPublicidad.Length)];
    }
}
