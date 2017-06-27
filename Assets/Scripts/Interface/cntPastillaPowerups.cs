using UnityEngine;
using System.Collections;


/// <summary>
/// GUI control para seleccionar power ups
/// </summary>
public class cntPastillaPowerups : MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  CONSTANTES  -------------------------------------------------------------
    // ------------------------------------------------------------------------------

    // numero maximo de botones que se pintan simultaneamente (es el valor maximo entre NUM_POWERUPS_LANZADOR y NUM_POWER_UPS_PORTERO)
    public const int NUM_BOTONES_PINTADOS = 5;

    // numero de powerups de cada tipo
    public const int NUM_POWERUPS_LANZADOR = 5;
    public const int NUM_POWER_UPS_PORTERO = 4;

    // altura a la que debe mostrarse la pastilla en la pantalla en funcion del modo de juego seleccionado
    private const float Y_PASTILLA_MODO_SINGLE = 0.84f;
    private const float Y_PASTILLA_MODO_MULTI = 0.795f;


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static cntPastillaPowerups instance { get { return m_instance; } }
    private static cntPastillaPowerups m_instance;

    /// <summary>
    /// Devuelve "true" si la pastilla de power ups esta visible
    /// </summary>
    public bool estaVisible { get { return m_estaVisible; } }
    private bool m_estaVisible;

    // componentes graficos de esta interfaz
    private GameObject m_goGrupoPowerupsLanzador;
    private GameObject m_goGrupoPowerupsPortero;

    private btnButton[] m_btnPowerupLanzador;
    private btnButton[] m_btnPowerupPortero;

    private GUIText[] m_txtCantidadesPowerUp;
    private GUIText[] m_txtCantidadesPowerUpSombra;



    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    void Awake() {
        m_instance = this;
        m_estaVisible = false;
    }


    /// <summary>
    /// Obtiene las referencias a los elementos graficos de esta interfaz
    /// </summary>
    private void ObtenerReferencias() {
        // obtener las referencias a los gameObjects que engloban los botones
        if (m_goGrupoPowerupsLanzador == null)
            m_goGrupoPowerupsLanzador = transform.FindChild("lanzador").gameObject;
        if (m_goGrupoPowerupsPortero == null)
            m_goGrupoPowerupsPortero = transform.FindChild("portero").gameObject;

        // obtener las referencias a los botones de lanzador si procede
        if (m_btnPowerupLanzador == null) {
            m_btnPowerupLanzador = new btnButton[NUM_POWERUPS_LANZADOR];
            for (int i = 0; i < m_btnPowerupLanzador.Length; ++i) {
                int numPowerup = i;
                m_btnPowerupLanzador[numPowerup] = transform.FindChild("lanzador/btnPowerup" + i).GetComponent<btnButton>();
                m_btnPowerupLanzador[numPowerup].action = (_name) => {
                    Debug.Log("Has pulsado en powerup LANZADOR " + numPowerup);
                    PowerupService.instance.UsePowerup((Powerup) numPowerup);
                    Hide();
                };
            }
        }

        // obtener las referencias a los botones de portero si procede
        if (m_btnPowerupPortero == null) {
            m_btnPowerupPortero = new btnButton[NUM_POWER_UPS_PORTERO];
            for (int i = 0; i < m_btnPowerupPortero.Length; ++i) {
                int numPowerup = i;
                m_btnPowerupPortero[numPowerup] = transform.FindChild("portero/btnPowerup" + i).GetComponent<btnButton>();
                m_btnPowerupPortero[numPowerup].action = (_name) => {
                    Debug.Log("Has pulsado en powerup PORTERO " + numPowerup);
                    GeneralSounds.instance.usePowerup();
                    PowerupService.instance.UsePowerup((Powerup) numPowerup + PowerupService.MAXPOWERUPSTIRADOR);
                    Hide();
                };
            }
        }

        // obtener las referencias a las cantidades de los power ups
        if (m_txtCantidadesPowerUp == null) {
            m_txtCantidadesPowerUp = new GUIText[NUM_BOTONES_PINTADOS];
            m_txtCantidadesPowerUpSombra = new GUIText[NUM_BOTONES_PINTADOS];
            for (int i = 0; i < m_txtCantidadesPowerUp.Length; ++i) {
                m_txtCantidadesPowerUp[i] = transform.FindChild("cantidades/txtPowerup" + i).GetComponent<GUIText>();
                m_txtCantidadesPowerUpSombra[i] = transform.FindChild("cantidades/txtPowerup" + i + "/sombra").GetComponent<GUIText>();
            }
        }
    }


    /// <summary>
    /// Muestra la pastilla de power ups en el modo recibido como parametro
    /// </summary>
    public void Show() {
        if(m_estaVisible) return;
        // indicar que la pastilla esta visible
        m_estaVisible = true;

        ObtenerReferencias();

        // comprobar si hay que mostrar la pastilla de lanzador o de portero
        bool modoLanzador = !GameplayService.IsGoalkeeper();

        // mostrar / ocultar los botones de lanzador
        m_goGrupoPowerupsLanzador.SetActive(modoLanzador);
        m_goGrupoPowerupsPortero.SetActive(!modoLanzador);

        // mostrar las cantidades de cada tipo de powerup
        int numPowerups = modoLanzador ? NUM_POWERUPS_LANZADOR : NUM_POWER_UPS_PORTERO;
        GameMode gameMode = modoLanzador ? GameMode.Shooter : GameMode.GoalKeeper;
        for (int i = 0; i < numPowerups; ++i) {
            // Activamos o desactivamos los powerups dependiendo del tirador y portero actualmente seleccionados
            int cantidadPowerup = PowerupService.ownInventory.GetCantidadPowerUp(gameMode, i);
            if (gameMode == GameMode.Shooter) {
                cantidadPowerup = FieldControl.localThrower.HasPowerup(i) ? 10 : 0;
            }
            else {
                cantidadPowerup = FieldControl.localGoalkeeper.HasPowerup((int)Powerup.Manoplas + i) ? 10 : 0;
            }
            m_txtCantidadesPowerUp[i].text = cantidadPowerup.ToString();
            m_txtCantidadesPowerUpSombra[i].text = m_txtCantidadesPowerUp[i].text;

            m_txtCantidadesPowerUpSombra[i].gameObject.SetActive(cantidadPowerup > 0);
            m_txtCantidadesPowerUp[i].gameObject.SetActive(cantidadPowerup > 0);

            // comprobar si el boton debe estar habilitado o no
            if (gameMode == GameMode.Shooter) {
                m_btnPowerupLanzador[i].SetEnabled(cantidadPowerup > 0);
                m_btnPowerupLanzador[i].gameObject.SetActive(cantidadPowerup > 0);
            }
            else {
                m_btnPowerupPortero[i].SetEnabled(cantidadPowerup > 0);
                m_btnPowerupPortero[i].gameObject.SetActive(cantidadPowerup > 0);
            }
        }
        for (int i = numPowerups; i < m_txtCantidadesPowerUp.Length; ++i) {
            m_txtCantidadesPowerUp[i].gameObject.SetActive(false);
        }

        // mostrar la pastilla de power ups
        new SuperTweener.move(gameObject, 0.25f, new Vector3(1.0f, (GameplayService.networked ? Y_PASTILLA_MODO_MULTI : Y_PASTILLA_MODO_SINGLE), 0.0f));
        GeneralSounds.instance.powerupBarOn();
    }


    /// <summary>
    /// Oculta la pastilla de powerups
    /// </summary>
    public void Hide() {
        if(!m_estaVisible) return;
        m_estaVisible = false;

        new SuperTweener.move(gameObject, 0.25f, new Vector3(2.0f, (GameplayService.networked ? Y_PASTILLA_MODO_MULTI : Y_PASTILLA_MODO_SINGLE), 0.0f), null,
            // on end callback
            (_name) => {
            });

        GeneralSounds.instance.powerupBarOff();
    }


}
