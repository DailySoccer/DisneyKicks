using UnityEngine;
using System.Collections;


/// <summary>
/// Control para mostrar un tooltip de una misión
/// </summary>
public class cntTooltipLevelSelection : MonoBehaviour {

    private const int NUM_MISSION_ACHIEVEMENTS = 4;

    // texturas de las primas cuando estan conseguidas y cuando no
    // Nota: asignar valor a estas variables desde la interfaz de Unity
    public Texture[] m_texturasPrimasNoConseguidas;
    public Texture[] m_texturasPrimasConseguidas;

    // elementos de esta interfaz
    private GUIText m_missionNameLabel;
    private GUIText m_missionNameLabelSombra;
    private GUIText[] m_missionAchievementLabels = new GUIText[ NUM_MISSION_ACHIEVEMENTS ];
    private GUIText[] m_missionAchievementLabelsSombra = new GUIText[NUM_MISSION_ACHIEVEMENTS];
    private GUITexture[] m_iconoObjetivoConseguido = new GUITexture[NUM_MISSION_ACHIEVEMENTS];
    private btnButton m_playButton;


    /// <summary>
    /// Obtiene las referencias a los elementos graficos de este control
    /// </summary>
    private void BuscarReferencias() {
        if (m_missionNameLabel == null)
            m_missionNameLabel = transform.FindChild("Mission Name").GetComponent<GUIText>();
        if (m_missionNameLabelSombra == null)
            m_missionNameLabelSombra = transform.FindChild("Mission Name/sombra").GetComponent<GUIText>();

        //if (m_missionModeIcon == null)
        //    m_missionModeIcon = transform.FindChild("Mission Type").guiTexture;

        for (int i = 0; i < NUM_MISSION_ACHIEVEMENTS; ++i) {
            if (m_missionAchievementLabels[i] == null) {
                m_missionAchievementLabels[i] = transform.FindChild("Objetivo_" + (i + 1) + "/Label").GetComponent<GUIText>();
                m_missionAchievementLabelsSombra[i] = transform.FindChild("Objetivo_" + (i + 1) + "/Label/Sombra").GetComponent<GUIText>();
                m_iconoObjetivoConseguido[i] = transform.FindChild("Objetivo_" + (i + 1) + "/iconoConseguido").GetComponent<GUITexture>();
            }
        }

        if (m_playButton == null)
            m_playButton = transform.FindChild("PlayBtn").GetComponent<btnButton>();
    }


    /// <summary>
    /// Actualiza la informacion de la mision a mostrar
    /// </summary>
    /// <param name="gameLevel"></param>
    /// <param name="_misionDesbloqueada"></param>
    public void SetInfo (GameLevelMission gameLevel, bool _misionDesbloqueada) {
        BuscarReferencias();

        // guardar en el "ifcCarrera" si la mision es de lanzador o de portero
        ifcCarrera.instance.estoyMostrandoMisionDeLanzador = (gameLevel.MissionGameMode == GameMode.Shooter);

        // mostrar el nombre de la mision 
        m_missionNameLabel.text = LocalizacionManager.instance.GetTexto(11).ToUpper() + " " + (gameLevel.Index+1);//int.Parse(partesNombreMision[2]);// + " (" + gameLevel.GetRoundsCount() + " " + LocalizacionManager.instance.GetTexto(145) + ")";
        m_missionNameLabelSombra.text = m_missionNameLabel.text;

        //SetMissionIcon( gameLevel.MissionGameMode );

        // mostrar los objetivos de mision
        int idx = 0;
        foreach (var achievement in gameLevel.GetAchievements()) {
            bool objetivoConseguido = achievement.IsAchieved();

            // pintar el texto del objetivo de mision con la opacidad que corresponda
            m_missionAchievementLabels[idx].text = achievement.DescriptionID;
            Color colorTexto = m_missionAchievementLabels[idx].color;
            colorTexto.a = objetivoConseguido ? 1.0f : 0.5f;
            m_missionAchievementLabels[idx].color = colorTexto;

            // pintar la sombra del texto
            m_missionAchievementLabelsSombra[idx].text = achievement.DescriptionID;
            m_missionAchievementLabelsSombra[idx].gameObject.SetActive(objetivoConseguido);

            // pintar la estrella con la opacidad que corresponda
            Color colorEstrella = m_iconoObjetivoConseguido[idx].color;
            colorEstrella.a = achievement.IsAchieved() ? 1.0f : 0.2f;
            m_iconoObjetivoConseguido[idx].color = colorEstrella;
            m_iconoObjetivoConseguido[idx].texture = objetivoConseguido ? m_texturasPrimasConseguidas[idx] : m_texturasPrimasNoConseguidas[idx];

            idx++;
        }

        // actualizar el estado del boton jugar
        m_playButton.gameObject.SetActive(_misionDesbloqueada);
        //m_missionModeIcon.gameObject.SetActive(_misionDesbloqueada);
        if (_misionDesbloqueada) {
            m_playButton.action =
                (_name) => {
                    GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.confirmClip);

                    // almacenar el modo de juego que se ha seleccionado
                    GameplayService.initialGameMode = gameLevel.MissionGameMode;
                    GameplayService.modoJuego = InfoModosJuego.instance.GetModoJuego("SHOOTER_NORMAL_MODE");
                    GameplayService.gameLevelMission = gameLevel;

                    // inicializar la pantalla de vestuario
                    ifcBase.activeIface = ifcVestuario.instance;
                    ifcVestuario.instance.SetPantallaBack(ifcCarrera.instance);

                    ifcVestuario.instance.SetVisible(true);
                    ifcVestuario.instance.ShowAs(ifcCarrera.instance.estoyMostrandoMisionDeLanzador ? ifcVestuario.TipoVestuario.LANZADOR : ifcVestuario.TipoVestuario.PORTERO);

                    // mostrar el vestuario
                    new SuperTweener.move(ifcCarrera.instance.gameObject, 0.25f, new Vector3(-1.0f, 0.0f, 0.0f), SuperTweener.CubicOut, (_target) => { ifcCarrera.instance.SetVisible(false); });
                    new SuperTweener.move(ifcVestuario.instance.gameObject, 0.25f, new Vector3(0.5f, 0.5f, 0.0f), SuperTweener.CubicOut, (_target) => { });

                    //Cortinilla.instance.Play();
                };
        }
    }

    /*private void SetMissionIcon (GameMode mode) {
        switch ( mode ) {
            case GameMode.GoalKeeper: m_missionModeIcon.texture = goalkeeperIcon; break;
            case GameMode.Shooter: m_missionModeIcon.texture = shooterIcon; break;
        }
    }
     */
}
