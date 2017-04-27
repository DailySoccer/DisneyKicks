using UnityEngine;
using System.Collections;

public class ifcPopUp : ifcBase
{
    public static ifcPopUp instance { get; protected set; }
    public Texture2D[] porteros;
    public Texture2D[] lanzadores;

    ifcBase m_interfazAnterior;
    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        transform.position = new Vector3(-1, -0.23f, 0);
    }

    void Start()
    {
        this.m_backMethod = Cerrar;

        // cargar el avatar
        if (GameplayService.initialGameMode == GameMode.Shooter) {
            // en modo lanzador => mostrar un portero
            getComponentByName("avatar").GetComponent<GUITexture>().texture = porteros[FieldControl.localGoalkeeper.idModelo];
        } else {
            // en modo portero => mostrar un lanzador
            getComponentByName("avatar").GetComponent<GUITexture>().texture = lanzadores[FieldControl.localThrower.idModelo];
        }

        getComponentByName("Cerrar").GetComponent<btnButton>().action = (_name) => {
            Cerrar();
        };

        // inicializar el collider
        transform.FindChild("collider").GetComponent<btnButton>().action = (_name) => {
            Cerrar();
        };
    }

    /// <summary>
    /// Cierra este popUp
    /// </summary>
    private void Cerrar(string _entry = "") {
        ifcBase.activeIface = m_interfazAnterior;
        GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOffClip);
        new SuperTweener.move(gameObject, 0.33f, new Vector3(-1, -0.23f, 0), SuperTweener.SinusoidalIn);
        gameObject.SetActive(false);

        // si estamos en modo time attack => activar una cuenta atras y tras esta activar el cronometro y los tiros del jugador
        if (GameplayService.modoJuego.tipoModo == ModoJuego.TipoModo.TIME_ATTACK) {
            // mostrar una cuenta atras
            cntCuentaAtras.instance.Activar(
                (_name) => {
                    // inicializar el cronometro
                    cntCronoTimeAttack.instance.SetActivo(true);

                    // desbloquear el jugador para que empiece a lanzar balones
                    InputManager.instance.Blocked = false;
                }, Stats.TIME_TIME_ATTACK_CUENTA_ATRAS);
        } else
            InputManager.instance.Blocked = false;
    }


    /// <summary>
    /// Muestra un popup
    /// </summary>
    /// <param name="_title"></param>
    /// <param name="_text"></param>
    /// <param name="_fixPosition">Si es true hace que el PopUp se muestre directamente en su posicion de pantalla (sin ser desplazado por el supertweener) en los casos de mostrar un popUp se pueda solapar con la cortinilla (que hace un flush del supertweener)</param>
    public void Show(string _title, string _text) {
        m_interfazAnterior = ifcBase.activeIface;
        ifcBase.activeIface = this;

        GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.popupOnClip);

        InputManager.instance.Blocked = true;
        gameObject.SetActive(true);
        transform.Find("Titulo").GetComponent<GUIText>().text = _title;
        GUIText gt = transform.Find("Texto").GetComponent<GUIText>();
        int lines = 0;
        gt.text = ifcTooltip.warp(_text, 270.0f, gt.font, gt.fontSize, out lines);
        transform.Find("Texto").GetComponent<txtText>().Fix();


        transform.position = new Vector3(-1, -0.2f, 0);
        new SuperTweener.move(gameObject, 0.33f, new Vector3(0, -0.2f, 0));
    }

}
