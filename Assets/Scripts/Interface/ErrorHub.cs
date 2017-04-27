using UnityEngine;
using System.Collections;

public class ErrorHub
{
    /* CODIGOS DE ERROR
        0 - desconocido
        1 - sin respuesta del servidor
        2 - cliente retado desconectado
        3 - cliente retado en reto
        4 - cliente retado en partida
        5 - propio jugador en reto
        6 - kickeado por AFK
        7 - imposible conectar
        8 - se ha perdido la conexion
        9 - version incompatible
        10 - ya esta logueado con ese usuario
        11 - el servidor no encuentra el usuario en los WS
    */

    static int[] errMsg = { 194,195,196,197,198,199,200,201,202,203,204,205 };
    static int[] errTitle = { 206,207,208,209,210,211,212,207,207,213, 214, 215 };


    public static void ThrowError(int _code, btnButton.guiAction _action = null, string _extraInfo = "")
    {
        string msg = string.Format(LocalizacionManager.instance.GetTexto(errMsg[_code]), _extraInfo);
        /*switch(_code)
        {
            case 0:
                
        }*/

        ifcDialogBox.instance.ShowOneButtonDialog(ifcDialogBox.OneButtonType.POSITIVE, LocalizacionManager.instance.GetTexto(errTitle[_code]), msg, LocalizacionManager.instance.GetTexto(45).ToUpper(), _action);
        //ifcDialogBox.instance.Show(ifcDialogBox.TipoBotones.ACEPTAR_CERRAR, "ERROR " + _code, msg, _action);
    }

    public static void ProcessError(int _code)
    {
        switch(_code)
        {
            case 2:
            case 3:
            case 4:
                ThrowError(_code, null, ifcDuelo.m_rival.alias);
                break;
            case 5:
                ThrowError(_code);
                break;
            case 6:
            case 9:
            case 10:
            case 11:
                ThrowError(_code, (_name) => {
                    Application.LoadLevel("Menus");
                });
                break;
            default:
                ThrowError(_code);
                break;
        }
    }
}
