using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JugadorData {

    /*
     * TIRADORES
     */

    public static List<Jugador> Tiradores = new List<Jugador> {
        new Jugador {
            assetName       = "IT_PLY_ST_0003",
            nombre          = "Vincent Lacombe",
            pais = LocalizacionManager.instance.GetTexto(261),
            numDorsal       = 4,
            habilidades     = null,
            powerups        = null,
            quality         = CardQuality.COMMON,
            nivel           = 1,
            liga            = 0
        },
        new Jugador {
            assetName = "IT_PLY_ST_0004", 
            nombre = "Mauro Tankara", 
            pais = LocalizacionManager.instance.GetTexto(262), 
            numDorsal = 5, 
            habilidades = null,
            powerups = null,
            quality = CardQuality.COMMON,
            liga = 0
        },
        new Jugador {
            assetName = "IT_PLY_ST_0005", 
            nombre = "Hans Fritzlang", 
            pais = LocalizacionManager.instance.GetTexto(263), 
            numDorsal = 6,
            habilidades = null,
            powerups = null,
            quality = CardQuality.COMMON,
            liga = 0
        },
        new Jugador {
            assetName = "IT_PLY_ST_0002", 
            nombre = "Andrew Crowley", 
            pais = LocalizacionManager.instance.GetTexto(264), 
            numDorsal = 3, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Prima },
            powerups = new Powerup[] { Powerup.Concentracion },
            quality = CardQuality.COMMON,
            liga = 0
        },
        new Jugador {
            assetName = "IT_PLY_ST_0001", 
            nombre = "Manuel Villalba", 
            pais = LocalizacionManager.instance.GetTexto(265), 
            numDorsal = 2, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Destello },
            quality = CardQuality.COMMON,
            liga = 1
        },
        new Jugador {
            assetName = "IT_PLY_ST_0108", 
            nombre = "Laszlo Pionescu", 
            pais = LocalizacionManager.instance.GetTexto(266), 
            numDorsal = 12, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador },
            powerups = new Powerup[] { Powerup.Sharpshooter },
            quality = CardQuality.COMMON,
            liga = 1
        },
        new Jugador {
            assetName = "IT_PLY_ST_0101", 
            nombre = "Andre Van Der Moor", 
            pais = LocalizacionManager.instance.GetTexto(267), 
            numDorsal = 7, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Prima, Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Resbaladiza },
            quality = CardQuality.COMMON,
            liga = 1
        }, 
        new Jugador {
            assetName = "IT_PLY_ST_0111", 
            nombre = "Olaf Larrson", 
            pais = LocalizacionManager.instance.GetTexto(268), 
            numDorsal = 15, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Prima, Habilidades.Skills.VIP },
            powerups = new Powerup[] { Powerup.Phase },
            quality = CardQuality.COMMON,
            liga = 1
        },
        new Jugador {
            assetName = "IT_PLY_ST_0114", 
            nombre = "Franz Raissenberg", 
            pais = LocalizacionManager.instance.GetTexto(269), 
            numDorsal = 2, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.VIP, Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Concentracion, Powerup.Destello },
            quality = CardQuality.COMMON,
            liga = 2
        },
        new Jugador {
            assetName = "IT_PLY_ST_0103", 
            nombre = "Ben Massala", 
            pais = LocalizacionManager.instance.GetTexto(270), 
            numDorsal = 9, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Mago_balon },
            powerups = new Powerup[] { Powerup.Sharpshooter, Powerup.Resbaladiza },
            quality = CardQuality.COMMON,
            liga = 2
        },
        new Jugador {
            assetName = "IT_PLY_ST_0113", 
            nombre = "Joao Soares", 
            pais = LocalizacionManager.instance.GetTexto(271), 
            numDorsal = 10, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Mago_balon, Habilidades.Skills.VIP },
            powerups = new Powerup[] { Powerup.Phase, Powerup.Concentracion },
            quality = CardQuality.COMMON,
            liga = 2
        },
        new Jugador {
            assetName = "IT_PLY_ST_0102", 
            nombre = "Mario Barrenchi", 
            pais = LocalizacionManager.instance.GetTexto(272), 
            numDorsal = 8, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.VIP },
            powerups = new Powerup[] { Powerup.Destello, Powerup.Sharpshooter, Powerup.Resbaladiza },
            quality = CardQuality.COMMON,
            liga = 2
        },
        new Jugador { 
            assetName = "IT_PLY_ST_0115", 
            nombre = "Hassan Nagala", 
            pais = LocalizacionManager.instance.GetTexto(273), 
            numDorsal = 3, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Mago_balon, Habilidades.Skills.Goleador },
            powerups = new Powerup[] { Powerup.Phase, Powerup.Sharpshooter, Powerup.Destello },
            quality = CardQuality.COMMON,
            liga = 2
        },
        new Jugador {
            assetName = "IT_PLY_ST_0106", 
            nombre = "Yanis Paidopoulos", 
            pais = LocalizacionManager.instance.GetTexto(274), 
            numDorsal = 11, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.Prima },
            powerups = new Powerup[] { Powerup.Phase, Powerup.Sharpshooter, Powerup.Destello, Powerup.Concentracion },
            quality = CardQuality.COMMON,
            liga = 3
        },
        new Jugador {
            assetName = "IT_PLY_ST_0116", 
            nombre = "Kurt Mulligan", 
            pais = LocalizacionManager.instance.GetTexto(275), 
            numDorsal = 4, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Goleador, Habilidades.Skills.Vista_halcon },
            powerups = new Powerup[] { Powerup.Resbaladiza, Powerup.Sharpshooter, Powerup.Destello, Powerup.Concentracion },
            quality = CardQuality.COMMON,
            liga = 3
        }
    };

    /*
     * PORTEROS
     */

    public static List<Jugador> Porteros = new List<Jugador> {
        new Jugador {
            assetName = "IT_PLY_GK_0003", 
            nombre = "Jack Donovan", 
            pais = LocalizacionManager.instance.GetTexto(276), 
            numDorsal = 1, 
            habilidades = null,
            powerups = null,
            quality = CardQuality.COMMON,
            nivel = 1,
            liga = 0
        },
        new Jugador {
            assetName = "IT_PLY_GK_0002", 
            nombre = "Alfredo Del Valle", 
            pais = LocalizacionManager.instance.GetTexto(277), 
            numDorsal = 1,
            habilidades = null,
            powerups = new Powerup[] { Powerup.Intuicion },
            quality = CardQuality.COMMON,
            liga = 1
        },
        new Jugador {
            assetName = "IT_PLY_GK_0103", 
            nombre = "Santiago Resquicio", 
            pais = LocalizacionManager.instance.GetTexto(278), 
            numDorsal = 1, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Practico },
            powerups = new Powerup[] { Powerup.Manoplas },
            quality = CardQuality.COMMON,
            liga = 1
        },
        new Jugador {
            assetName = "IT_PLY_GK_0102", 
            nombre = "Pietro Capiente", 
            pais = LocalizacionManager.instance.GetTexto(279), 
            numDorsal = 1, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Practico, Habilidades.Skills.Barrera },
            powerups = new Powerup[] { Powerup.Reflejo },
            quality = CardQuality.COMMON,
            liga = 2
        },
        new Jugador {
            assetName = "IT_PLY_GK_0101", 
            nombre = "Matsuhiro Shintao", 
            pais = LocalizacionManager.instance.GetTexto(280), 
            numDorsal = 1, 
            habilidades = new Habilidades.Skills[] { Habilidades.Skills.Premonicion },
            powerups = new Powerup[] { Powerup.TiempoBala },
            quality = CardQuality.COMMON,
            liga = 3
        }
    };
}
