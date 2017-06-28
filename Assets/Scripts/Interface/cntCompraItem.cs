using UnityEngine;
using System.Collections;


/// <summary>
/// Clase para generar un elemento de interfaz que permite comprar un PoweUp
/// </summary>
public class cntCompraItem: MonoBehaviour {

    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    // elementos de la interfaz
    private btnButton m_btnCompra; // <= sirve como collider

    private GUIText m_txtCantidad;
    private GUIText m_txtCantidadSombra;
    private GUIText m_txtBoost;
    private GUIText m_txtPrecio;
    private GUIText m_txtPrecioSombra;



    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ----------------------------------------------------------------------------


    /// <summary>
    /// Obtiene la referencias a los elementos de esta interfaz
    /// NOTA: este codigo no lo incluyo en el Start() o el Awake() xq si no a Unity se le va la pinza
    /// </summary>
    private void ObtenerReferencias() {
        // obtener referencias a los elementos de la interfaz
        if (m_btnCompra == null)
            m_btnCompra = transform.FindChild("btnCompra").GetComponent<btnButton>();

        if (m_txtCantidad == null)
            m_txtCantidad = transform.FindChild("txtCantidad").GetComponent<GUIText>();
        if (m_txtCantidadSombra == null)
            m_txtCantidadSombra = m_txtCantidad.transform.FindChild("sombra").GetComponent<GUIText>();
        if (m_txtBoost == null)
            m_txtBoost = transform.FindChild("txtBoost").GetComponent<GUIText>();
        if (m_txtPrecio == null)
            m_txtPrecio = transform.FindChild("txtPrecio").GetComponent<GUIText>();
        if (m_txtPrecioSombra == null)
            m_txtPrecioSombra = transform.FindChild("txtPrecio/sombra").GetComponent<GUIText>();
    }


    /// <summary>
    /// Muestra u oculta los elementos de esta interfaz
    /// </summary>
    /// <param name="_visible"></param>
    private void SetVisible(bool _visible) {
        // ocultar los elementos de la interfaz
        m_txtCantidad.gameObject.SetActive(_visible);
        m_txtCantidadSombra.gameObject.SetActive(_visible);
        m_txtPrecio.gameObject.SetActive(_visible);

        // inhabilitar el boton para la compra
        m_btnCompra.SetEnabled(_visible);
    }


    /// <summary>
    /// Muestra la informacion asociada a un escudo
    /// </summary>
    /// <param name="_escudo"></param>
    public void ShowAsEscudo(Escudo _escudo) {
        ObtenerReferencias();

        if (_escudo == null) {
            // ocultar este control
            SetVisible(false);
        } else {
            // mostrar este control
            SetVisible(true);

            // actualizar textos
            m_txtCantidad.text = _escudo.numUnidades.ToString();
            m_txtCantidadSombra.text = m_txtCantidad.text;
            m_txtPrecio.text = "§ " + _escudo.precioHard.ToString();
            m_txtPrecioSombra.text = m_txtPrecio.text;
            m_txtBoost.text = _escudo.boost.ToString("f1");
            m_txtBoost.gameObject.SetActive(true);

            // textura del escudo
            Texture texturaEscudo;
            if (EscudosManager.escudoEquipado.boost == _escudo.boost)
                texturaEscudo = AvataresManager.instance.GetTexturaEscudoTiendaEquipado(_escudo.idTextura);
            else
                texturaEscudo = AvataresManager.instance.GetTexturaEscudoTienda(_escudo.idTextura);
            m_btnCompra.GetComponent<GUITexture>().texture = texturaEscudo;
            m_btnCompra.m_current = texturaEscudo;

            // boton
            // XIMO: 19/06/2017: Hack para adquirir un jugador/equipación
            m_btnCompra.action = (_name) => {
                Interfaz.ClickFX();
                ifcVestuario.instance.SubirNivelJugador();
                ifcVestuario.instance.AdquirirEquipacion();
            };

            /*
            if (_escudo.faseDesbloqueo < Interfaz.ultimaMisionDesbloqueada) {
                // boton
                m_btnCompra.action = (_name) => {
                    Interfaz.ClickFX();
                    AccionComprarEscudo(_escudo);
                };
            } else {
                // boton
                m_btnCompra.action = (_name) => {
                    Interfaz.ClickFX();
                    // mostrar el dialogo de compra / equipar escudo
                    ifcDialogBox.instance.ShowOneButtonDialog(
                        ifcDialogBox.OneButtonType.POSITIVE,
                        LocalizacionManager.instance.GetTexto(78).ToUpper(),
                        string.Format(LocalizacionManager.instance.GetTexto(79), "<color=#ddf108> " + (int) (_escudo.faseDesbloqueo + 1) + "</color>"),
                        LocalizacionManager.instance.GetTexto(45).ToUpper());
                };
            }
            */
        }
    }


    void AccionComprarEscudo(Escudo _escudo)
    {
        // comprobar como debe funcionar el boton "equipar/desequipar"
        btnButton.guiAction onEquiparCallback;

        bool equipar = _escudo.boost != EscudosManager.escudoEquipado.boost;
        if (equipar) {
            // el boton funciona como equipar
            onEquiparCallback = (_name) => {
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.equiparEscudoClip);
                EscudosManager.escudoEquipado = _escudo;
                ifcVestuario.instance.RefreshMultiplicadorPuntuacion();
                ifcVestuario.instance.RefreshInfo(); // <= refrescar el estado de los escudos
            };
        } else {
            // el boton funciona como desequipar
            onEquiparCallback = (_name) => {
                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.desequiparEscudoClip);
                EscudosManager.escudoEquipado = EscudosManager.instance.escudoPorDefecto;   // <= al desequipar asigno el escudo por defecto
                ifcVestuario.instance.RefreshMultiplicadorPuntuacion();
                ifcVestuario.instance.RefreshInfo(); // <= refrescar el estado de los escudos
            };
        }

        if(_escudo.numUnidades == 0)
        {
            // mostrar el dialogo de compra / equipar escudo
            ifcDialogBox.instance.ShowOneButtonDialog(
                ifcDialogBox.OneButtonType.BITOONS,
                string.Format(LocalizacionManager.instance.GetTexto(80).ToUpper(), _escudo.nombre.ToUpper()),
                _escudo._descripcion + " " + string.Format(LocalizacionManager.instance.GetTexto(81), "<color=#ddf108> " + _escudo.numUnidades + "</color>", _escudo.nombre, (_escudo.numUnidades == 1) ? LocalizacionManager.instance.GetTexto(136) : LocalizacionManager.instance.GetTexto(145)),
                _escudo.precioHard.ToString(),
                // callback para la compra de escudo
                (_name) => {
                    // para que el usuario no se pase comprando
                    if (_escudo.numUnidades >= 99) {
                        ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(84).ToUpper(),
                            string.Format(LocalizacionManager.instance.GetTexto(85), "<color=#ddf108> " + _escudo.nombre + "</color>"),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
                        return;
                    }

                    // comprobar que el usuario tenga suficiente dinero para la compra
                    if (Interfaz.MonedasHard < _escudo.precioHard) {
                        ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(84).ToUpper(),
                            string.Format(LocalizacionManager.instance.GetTexto(86), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(47) + "</color>"),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
                        return;
                    }

                    // pagar el escudo y actualizar el dinero mostrado
                    Interfaz.MonedasHard -= _escudo.precioHard;
                    cntBarraSuperior.instance.ActualizarDinero();

                    GeneralSounds_menu.instance.comprar();

                    // incrementar la cantidad de escudos y repintar la tienda
                    ++_escudo.numUnidades;
                    if (ifcVestuario.instance != null)
                        ifcVestuario.instance.RefreshInfo();

                    // persistir la cantidad de escudos
                    PersistenciaManager.instance.SaveEscudos();
                    ShowAsEscudo(_escudo);
    
                    // volver a la compra de escudos
                    AccionComprarEscudo(_escudo);
                },
                true, null, true);
        }
        else
        {
            // mostrar el dialogo de compra / equipar escudo
            ifcDialogBox.instance.ShowTwoButtonDialog(
                (equipar) ? ifcDialogBox.TwoButtonType.BITOONS_EQUIPAR : ifcDialogBox.TwoButtonType.BITOONS_DESEQUIPAR,
                string.Format(LocalizacionManager.instance.GetTexto(80).ToUpper(), _escudo.nombre.ToUpper()),
                string.Format(LocalizacionManager.instance.GetTexto(81), "<color=#ddf108> " + _escudo.numUnidades + "</color>", _escudo.nombre, 
                (_escudo.numUnidades == 1) ? LocalizacionManager.instance.GetTexto(137) : LocalizacionManager.instance.GetTexto(145)),
                _escudo.precioHard.ToString(),
                (equipar ? LocalizacionManager.instance.GetTexto(82) : LocalizacionManager.instance.GetTexto(83)).ToUpper(),
                // callback para la compra de escudo
                (_name) => {
                    // para que el usuario no se pase comprando
                    if (_escudo.numUnidades >= 99) {
                        ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(84).ToUpper(),
                            string.Format(LocalizacionManager.instance.GetTexto(85), "<color=#ddf108> " + _escudo.nombre + "</color>"),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
                        return;
                    }

                    // comprobar que el usuario tenga suficiente dinero para la compra
                    if (Interfaz.MonedasHard < _escudo.precioHard) {
                        ifcDialogBox.instance.ShowOneButtonDialog(
                            ifcDialogBox.OneButtonType.POSITIVE,
                            LocalizacionManager.instance.GetTexto(84).ToUpper(),
                            string.Format(LocalizacionManager.instance.GetTexto(86), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(47) + "</color>"),
                            LocalizacionManager.instance.GetTexto(45).ToUpper());
                        return;
                    }

                    // pagar el escudo y actualizar el dinero mostrado
                    Interfaz.MonedasHard -= _escudo.precioHard;
                    cntBarraSuperior.instance.ActualizarDinero();

                    GeneralSounds_menu.instance.comprar();

                    // incrementar la cantidad de escudos y repintar la tienda
                    ++_escudo.numUnidades;
                    if (ifcVestuario.instance != null)
                        ifcVestuario.instance.RefreshInfo();

                    // persistir la cantidad de escudos
                    PersistenciaManager.instance.SaveEscudos();
                    ShowAsEscudo(_escudo);

                    // volver a la compra de escudos
                    AccionComprarEscudo(_escudo);
                },
                // callback para equipar / desequipar el escudo
                onEquiparCallback,
                true, null, true);
        }
    }



    /// <summary>
    /// Inicializa los valores de este control para mostrar un power up
    /// </summary>
    /// <param name="_powerUpDescriptor"></param>
    public void ShowAsPowerUp(PowerUpDescriptor _powerUpDescriptor) {
        ObtenerReferencias();

        if (_powerUpDescriptor == null) {
            // ocultar este control
            SetVisible(false);

            m_txtBoost.gameObject.SetActive(false);

            // mostrar en el boton la textura por defecto
            m_btnCompra.GetComponent<GUITexture>().texture = AvataresManager.instance.m_texturaCompraPowerUp;
            m_btnCompra.m_current = AvataresManager.instance.m_texturaCompraPowerUp;
        } else {
            // mostrar este control
            SetVisible(false);
            m_btnCompra.SetEnabled(true);

            /*
            // actualizar la cantidad de items
            if (PowerupService.ownInventory == null)
                m_txtCantidad.text = "0";
            else
                m_txtCantidad.text = PowerupService.ownInventory.GetCantidadPowerUp(_powerUpDescriptor.idWs).ToString();
            m_txtCantidadSombra.text = m_txtCantidad.text;
          
            //m_icono.texture = AvataresManager.instance.GetTexturaPowerUp((int) PowerupInventory.IdWsToPowerup(_powerUpDescriptor.idWs));
            //m_icono.gameObject.SetActive(true);

            // actualizar el resto de textos
            m_txtPrecio.text = "¤ " + _powerUpDescriptor.precioSoft.ToString();
            m_txtPrecioSombra.text = m_txtPrecio.text;
            */
            m_txtBoost.gameObject.SetActive(false);

            // boton compra
            Texture textura = AvataresManager.instance.GetTexturaPowerUp((int) PowerupInventory.IdWsToPowerup(_powerUpDescriptor.idWs));
            m_btnCompra.GetComponent<GUITexture>().texture = textura;
            m_btnCompra.m_current = textura;

            /*
            m_btnCompra.action = (_name) => {
                Interfaz.ClickFX();
                AccionComprarPowerup(_powerUpDescriptor);
            };
            */
        }
    }


    void AccionComprarPowerup(PowerUpDescriptor _powerUpDescriptor)
    {
        int cantidad = PowerupService.ownInventory.GetCantidadPowerUp(_powerUpDescriptor.idWs);

        ifcDialogBox.instance.ShowOneButtonDialog(
            ifcDialogBox.OneButtonType.COINS,
            string.Format(LocalizacionManager.instance.GetTexto(80), _powerUpDescriptor.nombre).ToUpper(),
            string.Format(_powerUpDescriptor._descripcion + "\n" + LocalizacionManager.instance.GetTexto(81), "<color=#ddf108> " + cantidad.ToString() + "</color>", "<color=#ddf108>" + _powerUpDescriptor.nombre + "</color>", cantidad == 1 ? LocalizacionManager.instance.GetTexto(138) : LocalizacionManager.instance.GetTexto(17)),
            _powerUpDescriptor.precioSoft.ToString(),
            // callback al comprar el powerup
            (_name) => {
                // para que el usuario no se pase comprando
                if (PowerupService.ownInventory.GetCantidadPowerUp(_powerUpDescriptor.idWs) >= PowerUpDescriptor.LIMITE_UNIDADES) {
                    ifcDialogBox.instance.ShowOneButtonDialog(
                        ifcDialogBox.OneButtonType.POSITIVE,
                        LocalizacionManager.instance.GetTexto(84).ToUpper(),
                        string.Format(LocalizacionManager.instance.GetTexto(85), "<color=#ddf108> " + _powerUpDescriptor.nombre + "</color>"),
                        LocalizacionManager.instance.GetTexto(45).ToUpper());
                    return;
                }

                // comprobar que el usuario tenga suficiente dinero para la compra
                if (Interfaz.MonedasSoft < _powerUpDescriptor.precioSoft) {
                    ifcDialogBox.instance.ShowOneButtonDialog(
                        ifcDialogBox.OneButtonType.POSITIVE,
                        LocalizacionManager.instance.GetTexto(84).ToUpper(),
                        string.Format(LocalizacionManager.instance.GetTexto(86), "<color=#ddf108> " + LocalizacionManager.instance.GetTexto(46) + "</color>"),
                        LocalizacionManager.instance.GetTexto(45).ToUpper());
                    return;
                }

                // pagar el powerup y actualizar el dinero mostrado
                Interfaz.MonedasSoft -= _powerUpDescriptor.precioSoft;
                cntBarraSuperior.instance.ActualizarDinero();

                // incrementar la cantidad de powerup y repintar la tienda
                PowerupService.ownInventory.IncrementarCantidadPowerUp(_powerUpDescriptor.idWs, 1);

                GeneralSounds_menu.instance.playOneShot(GeneralSounds_menu.instance.compraClip);

                if (ifcVestuario.instance != null)
                    ifcVestuario.instance.RefreshInfo();

                // persistir la cantidad de powerups
                PersistenciaManager.instance.SavePowerUps();

                // volver a mostrar este dialogo
                AccionComprarPowerup(_powerUpDescriptor);
            },
            true,
            null, true);
    }

}
