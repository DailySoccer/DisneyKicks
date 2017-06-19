using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JugadorData {

    /*
     * TIRADORES
     */

    public static List<Jugador> Tiradores = new List<Jugador> {
        new Jugador {
            idModelo        = 2,
            assetName       = "IT_PLY_ST_0003",
            nombre          = "Vincent Lacombe",
            pais = LocalizacionManager.instance.GetTexto(261),
            precioSoft      = 0,
            precioHard      = 0,
            precioEarlyBuy  = 0,
            numDorsal       = 4,
            habilidades     = null,
            powerups        = null,
            quality         = CardQuality.COMMON,
            estado          = Jugador.Estado.ADQUIRIDO
        },
        new Jugador {
            idModelo = 3, 
            assetName = "IT_PLY_ST_0004", 
            nombre = "Mauro Tankara", 
            pais = LocalizacionManager.instance.GetTexto(262), 
            precioSoft = 0, 
            precioHard = 0, 
            precioEarlyBuy = 0, 
            numDorsal = 5, 
            habilidades = null,
            powerups = null,
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 4, 
            assetName = "IT_PLY_ST_0005", 
            nombre = "Hans Fritzlang", 
            pais = LocalizacionManager.instance.GetTexto(263), 
            precioSoft = 3750, 
            precioHard = 100, 
            precioEarlyBuy = 125, 
            numDorsal = 6,
            habilidades = null,
            powerups = null,
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 1, 
            assetName = "IT_PLY_ST_0002", 
            nombre = "Andrew Crowley", 
            pais = LocalizacionManager.instance.GetTexto(264), 
            precioSoft = 8800, 
            precioHard = 200, 
            precioEarlyBuy = 420, 
            numDorsal = 3, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Prima },
            powerups = new Powerup[] { Powerup.Concentracion },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 0, 
            assetName = "IT_PLY_ST_0001", 
            nombre = "Manuel Villalba", 
            pais = LocalizacionManager.instance.GetTexto(265), 
            precioSoft = 9000, 
            precioHard = 150, 
            precioEarlyBuy = 380, 
            numDorsal = 2, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Destello },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 16, 
            assetName = "IT_PLY_ST_0108", 
            nombre = "Laszlo Pionescu", 
            pais = LocalizacionManager.instance.GetTexto(266), 
            precioSoft = 11040, 
            precioHard = 220, 
            precioEarlyBuy = 500, 
            numDorsal = 12, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador },
            powerups = new Powerup[] { Powerup.Sharpshooter },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 11, 
            assetName = "IT_PLY_ST_0101", 
            nombre = "Andre Van Der Moor", 
            pais = LocalizacionManager.instance.GetTexto(267), 
            precioSoft = 10800, 
            precioHard = 220, 
            precioEarlyBuy = 550, 
            numDorsal = 7, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Prima, Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Resbaladiza },
            quality = CardQuality.COMMON
        }, 
        new Jugador {
            idModelo = 19, 
            assetName = "IT_PLY_ST_0111", 
            nombre = "Olaf Larrson", 
            pais = LocalizacionManager.instance.GetTexto(268), 
            precioSoft = 21000, 
            precioHard = 240, 
            precioEarlyBuy = 575, 
            numDorsal = 15, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Prima, Habilidades.Skills.VIP },
            powerups = new Powerup[] { Powerup.Phase },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 22, 
            assetName = "IT_PLY_ST_0114", 
            nombre = "Franz Raissenberg", 
            pais = LocalizacionManager.instance.GetTexto(269), 
            precioSoft = 19250, 
            precioHard = 195, 
            precioEarlyBuy = 450, 
            numDorsal = 2, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.VIP, Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Concentracion, Powerup.Destello },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 13, 
            assetName = "IT_PLY_ST_0103", 
            nombre = "Ben Massala", 
            pais = LocalizacionManager.instance.GetTexto(270), 
            precioSoft = 27200, 
            precioHard = 210, 
            precioEarlyBuy = 480, 
            numDorsal = 9, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Mago_balon },
            powerups = new Powerup[] { Powerup.Sharpshooter, Powerup.Resbaladiza },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 21, 
            assetName = "IT_PLY_ST_0113", 
            nombre = "Joao Soares", 
            pais = LocalizacionManager.instance.GetTexto(271), 
            precioSoft = 33600, 
            precioHard = 250, 
            precioEarlyBuy = 600, 
            numDorsal = 10, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Mago_balon, Habilidades.Skills.VIP },
            powerups = new Powerup[] { Powerup.Phase, Powerup.Concentracion },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 12,
            assetName = "IT_PLY_ST_0102", 
            nombre = "Mario Barrenchi", 
            pais = LocalizacionManager.instance.GetTexto(272), 
            precioSoft = 33745, 
            precioHard = 250, 
            precioEarlyBuy = 675, 
            numDorsal = 8, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.VIP },
            powerups = new Powerup[] { Powerup.Destello, Powerup.Sharpshooter, Powerup.Resbaladiza },
            quality = CardQuality.COMMON
        },
        new Jugador { 
            idModelo = 23, 
            assetName = "IT_PLY_ST_0115", 
            nombre = "Hassan Nagala", 
            pais = LocalizacionManager.instance.GetTexto(273), 
            precioSoft = 59500, 
            precioHard = 550, 
            precioEarlyBuy = 900, 
            numDorsal = 3, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Mago_balon, Habilidades.Skills.Goleador },
            powerups = new Powerup[] { Powerup.Phase, Powerup.Sharpshooter, Powerup.Destello },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 15, 
            assetName = "IT_PLY_ST_0106", 
            nombre = "Yanis Paidopoulos", 
            pais = LocalizacionManager.instance.GetTexto(274), 
            precioSoft = 56160, 
            precioHard = 500, 
            precioEarlyBuy = 1000, 
            numDorsal = 11, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.Prima },
            powerups = new Powerup[] { Powerup.Phase, Powerup.Sharpshooter, Powerup.Destello, Powerup.Concentracion },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 24, 
            assetName = "IT_PLY_ST_0116", 
            nombre = "Kurt Mulligan", 
            pais = LocalizacionManager.instance.GetTexto(275), 
            precioSoft = 50040, 
            precioHard = 400, 
            precioEarlyBuy = 850, 
            numDorsal = 4, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Resbaladiza, Powerup.Sharpshooter, Powerup.Destello, Powerup.Concentracion },
            quality = CardQuality.COMMON
        }
    };

    /*
     * PORTEROS
     */

    public static List<Jugador> Porteros = new List<Jugador> {
        new Jugador {
            idModelo = 2, 
            assetName = "IT_PLY_GK_0003", 
            nombre = "Jack Donovan", 
            pais = LocalizacionManager.instance.GetTexto(276), 
            precioSoft = 0, 
            precioHard = 0, 
            precioEarlyBuy = 0, 
            numDorsal = 1, 
            habilidades = null,
            powerups = null,
            quality = CardQuality.COMMON,
            estado = Jugador.Estado.ADQUIRIDO
        },
        new Jugador {
            idModelo = 1, 
            assetName = "IT_PLY_GK_0002", 
            nombre = "Alfredo Del Valle", 
            pais = LocalizacionManager.instance.GetTexto(277), 
            precioSoft = 3500, 
            precioHard = 75, 
            precioEarlyBuy = 100, 
            numDorsal = 1,
            habilidades = null,
            powerups = new Powerup[] { Powerup.Intuicion },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 5, 
            assetName = "IT_PLY_GK_0103", 
            nombre = "Santiago Resquicio", 
            pais = LocalizacionManager.instance.GetTexto(278), 
            precioSoft = 5200, 
            precioHard = 120, 
            precioEarlyBuy = 225, 
            numDorsal = 1, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Practico },
            powerups = new Powerup[] { Powerup.Manoplas },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 4, 
            assetName = "IT_PLY_GK_0102", 
            nombre = "Pietro Capiente", 
            pais = LocalizacionManager.instance.GetTexto(279), 
            precioSoft = 30600, 
            precioHard = 350, 
            precioEarlyBuy = 650, 
            numDorsal = 1, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Practico, Habilidades.Skills.Barrera },
            powerups = new Powerup[] { Powerup.Reflejo },
            quality = CardQuality.COMMON
        },
        new Jugador {
            idModelo = 3, 
            assetName = "IT_PLY_GK_0101", 
            nombre = "Matsuhiro Shintao", 
            pais = LocalizacionManager.instance.GetTexto(280), 
            precioSoft = 56000, 
            precioHard = 600, 
            precioEarlyBuy = 1050, 
            numDorsal = 1, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Premonicion },
            powerups = new Powerup[] { Powerup.TiempoBala },
            quality = CardQuality.COMMON
        }
    };
}
