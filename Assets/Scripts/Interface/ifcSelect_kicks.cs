using UnityEngine;
using System.Collections;

public class ifcSelect_kicks : ifcBase {

  // ------------------------------------------------------------------------------
  // ---  PROPIEDADES  ------------------------------------------------------------
  // ------------------------------------------------------------------------------


  public static ifcSelect_kicks instance { get { return m_instance; } }
  private static ifcSelect_kicks m_instance;

  // escudos de los equipos de futbol
  public Texture2D[] escudos;

  // shaders para poner a los jugadores en blanco y negro (asignarlos desde la interfaz de Unity)
  public Shader m_shaderDiffuseBw;
  public Shader m_shaderDiffuseDetailBw;


  // ------------------------------------------------------------------------------
  // ---  METODOS  ----------------------------------------------------------------
  // ------------------------------------------------------------------------------


  void Awake() {
      m_instance = this;
  }


  void Start()
  {
    transform.Find("encuesta/btnVotar").GetComponent<btnButton>().action = (_name) => {
            Interfaz.ClickFX();
#if UNITY_WEBPLAYER
            Application.ExternalEval("window.open('http://opin.at/vli','Votar')");
#else
            Application.OpenURL("http://opin.at/vli");
#endif
    };

    transform.Find("encuesta2/btnVotar").GetComponent<btnButton>().action = (_name) => {
            Interfaz.ClickFX();
#if UNITY_WEBPLAYER
            Application.ExternalEval("window.open('http://opin.at/vli','Votar')");
#else
            Application.OpenURL("http://opin.at/vli");
#endif
    };


    // botones para paginar los porteros
    transform.Find("selectGoalkeeper/btnRight").GetComponent<btnButton>().action = (_name) => {
        Interfaz.ClickFX();
        Interfaz.instance.Goalkeeper++;
        MostrarPorteroSeleccionado();
    };
    transform.Find("selectGoalkeeper/btnLeft").GetComponent<btnButton>().action = (_name) => {
        Interfaz.ClickFX();
        Interfaz.instance.Goalkeeper--;
        MostrarPorteroSeleccionado();
    };
    /*
    // botones para paginar los tiradores
    transform.Find("selectThrower/btnRight").GetComponent<btnButton>().action = (_name) => {
        Interfaz.ClickFX();
        Interfaz.instance.Thrower++;
        MostrarTiradorSeleccionado();
    };
    transform.Find("selectThrower/btnLeft").GetComponent<btnButton>().action = (_name) => {
        Interfaz.ClickFX();
        Interfaz.instance.Thrower--;
        MostrarTiradorSeleccionado();
    };

    // crear el control para mostrar una alerta sobre los tiradores
    GameObject go;
    m_cntInfoTiradorDisponible = go.GetComponent<cntInfoJugadorDisponible>();
    m_cntInfoTiradorDisponible.Inicializar(this.transform, "infoTiradorDisponible", new Vector3(5.9051f, -1.07f, 0.0f));
    Scale(m_cntInfoTiradorDisponible.gameObject);

    // crear el control para mostrar una alerta sobre los porteros
    m_cntInfoPorteroDisponible = go.GetComponent<cntInfoJugadorDisponible>();
    m_cntInfoPorteroDisponible.Inicializar(this.transform, "infoPorteroDisponible", new Vector3(6.1751f, -1.07f, 0.0f));
    Scale(m_cntInfoPorteroDisponible.gameObject);

    // obtener la referencia de las texturas de los candados
    m_texturaCandadoTirador = transform.FindChild("infoTiradorDisponible/candado").GetComponent<GUITexture>();
    m_texturaCandadoPortero = transform.FindChild("infoPorteroDisponible/candado").GetComponent<GUITexture>();

    // mostrar los jugadores seleccionados
    MostrarTiradorSeleccionado();
    MostrarPorteroSeleccionado();
     */
  }


  /// <summary>
  /// Muestra el jugador tirador seleccionado
  /// </summary>
  private void MostrarTiradorSeleccionado() {
      // obtener la informacion del tirador seleccionado
      Jugador tirador = InfoJugadores.instance.GetTirador(Interfaz.instance.Thrower);

      // si el jugador no existe mostrar la info de la encuesta
      if (tirador == null) {
          getComponentByName("encuesta").SetActive(true);
          transform.Find("selectThrower/Text").GetComponent<GUIText>().text = "¡PARTICIPA!";
          transform.Find("selectThrower/Text/Shadow").GetComponent<GUIText>().text = "¡PARTICIPA!";
      } else {
          getComponentByName("encuesta").SetActive(false);
          transform.Find("selectThrower/Text").GetComponent<GUIText>().text = tirador.nombre;
          transform.Find("selectThrower/Text/Shadow").GetComponent<GUIText>().text = tirador.nombre;

          // mostrar / ocultar controles en funcion del estado del jugador
          switch (tirador.estado) {
              case Jugador.Estado.ADQUIRIDO:
                  break;

              case Jugador.Estado.DISPONIBLE:
                  // pintar el jugador en blanco y negro
                  DecolorarInstanciaJugador(Interfaz.instance.throwerModel.transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>());
                  DecolorarInstanciaJugador(Interfaz.instance.throwerModel.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Head").GetComponent<MeshRenderer>());

                  break;

              case Jugador.Estado.BLOQUEADO:
                  // pintar el jugador en blanco y negro
                  DecolorarInstanciaJugador(Interfaz.instance.throwerModel.transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>());
                  DecolorarInstanciaJugador(Interfaz.instance.throwerModel.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Head").GetComponent<MeshRenderer>());

                  break;
          }
      }

      // pintar el escudo del jugador
      transform.Find("selectThrower/imgEscudo").GetComponent<GUITexture>().texture = escudos[Interfaz.instance.Thrower];
  }


  /// <summary>
  /// Muestra el jugador portero seleccionado
  /// </summary>
  private void MostrarPorteroSeleccionado() {
      // obtener la informacion del tirador seleccionado
      Jugador portero = InfoJugadores.instance.GetPortero(Interfaz.instance.Goalkeeper);

      // si el jugador no existe mostrar la info de la encuesta
      if (portero == null) {
          getComponentByName("encuesta2").SetActive(true);
          transform.Find("selectGoalkeeper/Text").GetComponent<GUIText>().text = "¡PARTICIPA!";
          transform.Find("selectGoalkeeper/Text/Shadow").GetComponent<GUIText>().text = "¡PARTICIPA!";
      } else {
          getComponentByName("encuesta2").SetActive(false);
          transform.Find("selectGoalkeeper/Text").GetComponent<GUIText>().text = portero.nombre;
          transform.Find("selectGoalkeeper/Text/Shadow").GetComponent<GUIText>().text = portero.nombre;

          // mostrar / ocultar controles en funcion del estado del jugador
          switch (portero.estado) {
              case Jugador.Estado.ADQUIRIDO:
                  break;

              case Jugador.Estado.DISPONIBLE:

                  // pintar el jugador en blanco y negro
                  DecolorarInstanciaJugador(Interfaz.instance.goalkeeperModel.transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>());
                  DecolorarInstanciaJugador(Interfaz.instance.goalkeeperModel.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Head").GetComponent<MeshRenderer>());
                break;

              case Jugador.Estado.BLOQUEADO:

                  // pintar el jugador en blanco y negro
                  DecolorarInstanciaJugador(Interfaz.instance.goalkeeperModel.transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>());
                  DecolorarInstanciaJugador(Interfaz.instance.goalkeeperModel.transform.FindChild("Bip01/Bip01 Pelvis/Bip0  1 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Head").GetComponent<MeshRenderer>());
                break;
         }
      }
      
      // pintar el escudo del jugador
      transform.Find("selectGoalkeeper/imgEscudo").GetComponent<GUITexture>().texture = escudos[Interfaz.instance.Goalkeeper];
  }


  /// <summary>
  /// Metodo para decolorar una instancia de jugador a partir de su SkinedMeshRenderer
  /// </summary>
  /// <param name="_smr"></param>
  private void DecolorarInstanciaJugador(SkinnedMeshRenderer _smr) {
      if (_smr != null) {
          // recorrer cada material del modelo instanciado del jugador
          for (int i = 0; i < _smr.materials.Length; ++i) {
              // substituir los shaders por su correspondiente en blanco y negro
              if (_smr.materials[i].shader.name == "Diffuse Detail")
                  _smr.materials[i].shader = m_shaderDiffuseDetailBw;
              else
                  if (_smr.materials[i].shader.name == "Diffuse")
                      _smr.materials[i].shader = m_shaderDiffuseBw;
          }
      }
  }

  /// <summary>
  /// Metodo para decolorar una instancia de jugador a partir de su SkinedMeshRenderer
  /// </summary>
  /// <param name="_smr"></param>
  private void DecolorarInstanciaJugador(MeshRenderer _smr) {
      if (_smr != null) {
          // recorrer cada material del modelo instanciado del jugador
          for (int i = 0; i < _smr.materials.Length; ++i) {
              // substituir los shaders por su correspondiente en blanco y negro
              if (_smr.materials[i].shader.name == "Diffuse Detail")
                  _smr.materials[i].shader = m_shaderDiffuseDetailBw;
              else
                  if (_smr.materials[i].shader.name == "Diffuse")
                      _smr.materials[i].shader = m_shaderDiffuseBw;
          }
      }
  }


}
