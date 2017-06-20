using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipacionData {
    /*
     * TIRADORES
     */

    // int _idTextura, string _assetName, int _precioSoft, int _precioHard, Color _colorDorsal, int _faseDesbloqueo = 0, 
    // int _precioEarlyBuy = 0, Estado _estado = Estado.BLOQUEADA
    public static List<Equipacion> Tiradores = new List<Equipacion> {
        new Equipacion { 
            idTextura = 14, 
            assetName = "IT_EQU_ST_0015", 
            precioSoft = 0, 
            precioHard = 0, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 0, 
            estado = Equipacion.Estado.ADQUIRIDA
        },
        new Equipacion {
            idTextura = 17, 
            assetName = "IT_EQU_ST_0018", 
            precioSoft = 0, 
            precioHard = 0, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 0, 
            estado = Equipacion.Estado.ADQUIRIDA
        },
        new Equipacion {
            idTextura = 20, 
            assetName = "IT_EQU_ST_0021", 
            precioSoft = 0, 
            precioHard = 0, 
            colorDorsal = new Color(169.0f / 255.0f, 29.0f / 255.0f, 31.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 0, 
            estado = Equipacion.Estado.ADQUIRIDA
        },
        new Equipacion {
            idTextura = 30, 
            assetName = "IT_EQU_ST_0031", 
            precioSoft = 4200, 
            precioHard = 120, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 195
        },
        new Equipacion {
            idTextura = 26, 
            assetName = "IT_EQU_ST_0027", 
            precioSoft = 3600, 
            precioHard = 100, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 180
        },
        new Equipacion {
            idTextura = 28, 
            assetName = "IT_EQU_ST_0029", 
            precioSoft = 5600, 
            precioHard = 180, 
            colorDorsal = new Color(64.0f / 255.0f, 83.0f / 255.0f, 18.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 325
        },
        new Equipacion {
            idTextura = 25, 
            assetName = "IT_EQU_ST_0026", 
            precioSoft = 6300, 
            precioHard = 180, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 310
        },
        new Equipacion {
            idTextura = 23, 
            assetName = "IT_EQU_ST_0024", 
            precioSoft = 8000, 
            precioHard = 190, 
            colorDorsal = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 300
        },
        new Equipacion {
            idTextura = 21, 
            assetName = "IT_EQU_ST_0022", 
            precioSoft = 8400, 
            precioHard = 200, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 400
        },
        new Equipacion {
            idTextura = 19, 
            assetName = "IT_EQU_ST_0020", 
            precioSoft = 9280, 
            precioHard = 200, 
            colorDorsal = new Color(80.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 250
        },
        new Equipacion {
            idTextura = 32, 
            assetName = "IT_EQU_ST_0033", 
            precioSoft = 10400, 
            precioHard = 210, 
            colorDorsal = new Color(160.0f / 255.0f, 122.0f / 255.0f, 3.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 360
        },
        new Equipacion {
            idTextura = 34, 
            assetName = "IT_EQU_ST_0035", 
            precioSoft = 12750, 
            precioHard = 210, 
            colorDorsal = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 350
        },
        new Equipacion {
            idTextura = 29, 
            assetName = "IT_EQU_ST_0030", 
            precioSoft = 14790, 
            precioHard = 250, 
            colorDorsal = new Color(64.0f / 255.0f, 83.0f / 255.0f, 18.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 425
        },
        new Equipacion {
            idTextura = 27, 
            assetName = "IT_EQU_ST_0028", 
            precioSoft = 18000, 
            precioHard = 200, 
            colorDorsal = new Color(8.0f / 255.0f, 105.0f / 255.0f, 169.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 400
        },
        new Equipacion {
            idTextura = 22, 
            assetName = "IT_EQU_ST_0023", 
            precioSoft = 16560, 
            precioHard = 250, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 600
        }
    };

    /*
    */

    /*
     * PORTEROS
     */

    public static List<Equipacion> Porteros = new List<Equipacion> {
        new Equipacion {
            idTextura = 0, 
            assetName = "IT_EQU_GK_0001", 
            precioSoft = 0, 
            precioHard = 0, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 0, 
            estado = Equipacion.Estado.ADQUIRIDA
        },
        new Equipacion { 
            idTextura = 1, 
            assetName = "IT_EQU_GK_0002", 
            precioSoft = 0, 
            precioHard = 0, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 0, 
            estado = Equipacion.Estado.ADQUIRIDA
        },
        new Equipacion {
            idTextura = 2, 
            assetName = "IT_EQU_GK_0003", 
            precioSoft = 2500, 
            precioHard = 75, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 100
        },
        new Equipacion {
            idTextura = 3, 
            assetName = "IT_EQU_GK_0004", 
            precioSoft = 7700, 
            precioHard = 175, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 275
        },
        new Equipacion {
            idTextura = 5, 
            assetName = "IT_EQU_GK_0006", 
            precioSoft = 15750, 
            precioHard = 275, 
            colorDorsal = new Color(221.0f / 255.0f, 217.0f / 255.0f, 221.0f / 255.0f, 1.0f), 
            precioEarlyBuy = 375
        }
    };
        
}
