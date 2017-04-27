using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class edtLogros
{
    [MenuItem("Kicks/CreateLogros")]
    public static void CreateLogros(){
        LogrosDescription.descLogro[] logrosTmp = new LogrosDescription.descLogro[]{

// LOGROS PORTERO ---------------------------------------------------------------------------------

// Logros de parada (despejes)
        new LogrosDescription.descLogro( "ACH_KICK_DES_01", "Avispado","Consigue 25 despejes", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_DES_02", "Reflejos felinos","Consigue 100 despejes", 100 ),
        new LogrosDescription.descLogro( "ACH_KICK_DES_03", "Muralla humana","Consigue 250 despejes", 300 ),
        new LogrosDescription.descLogro( "ACH_KICK_DES_04", "Trofeo Zamora","Consigue 500 despejes", 600 ),
        new LogrosDescription.descLogro( "ACH_KICK_DES_05", "Imbatible","Consigue 1000 despejes", 1000 ),
        new LogrosDescription.descLogro( "ACH_KICK_DES_06", "Cancerbero","Consigue 2000 despejes", 1500 ),
// Logros dec parada (atrapadas)
        new LogrosDescription.descLogro( "ACH_KICK_ATR_01", "Seguridad plena","Realiza 25 paradas", 150 ),
        new LogrosDescription.descLogro( "ACH_KICK_ATR_02", "La garra humana","Realiza 100 paradas", 400 ),
        new LogrosDescription.descLogro( "ACH_KICK_ATR_03", "Guante de oro","Realiza 500 paradas", 700 ),
        new LogrosDescription.descLogro( "ACH_KICK_ATR_04", "Sin resquicios","Realiza 1000 paradas", 1000 ),
        new LogrosDescription.descLogro( "ACH_KICK_ATR_05", "Portería impenetrable","Realiza 3000 paradas", 1500 ),
        new LogrosDescription.descLogro( "ACH_KICK_ATR_06", "El Santo","Realiza 5000 paradas", 2000 ),
// Logros de parada (puntuación)
        new LogrosDescription.descLogro( "ACH_KICK_PPU_01", "Aprendiz","Acumula 5000 puntos en parada", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_02", "Iniciado","Acumula 10000 puntos en parada", 200 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_03", "Pro","Acumula 50000 puntos en parada", 500 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_04", "Veterano","Acumula 100000 puntos en parada", 700 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_05", "Ídolo","Acumula 500000 puntos en parada", 800 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_06", "Estrella","Acumula 700000 puntos en parada", 1000 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_07", "Mítico","Acumula 1000000 puntos en parada", 1200 ),
        new LogrosDescription.descLogro( "ACH_KICK_PPU_08", "Legendario","Acumula 1500000 puntos en parada", 1300 ),

      // LOGROS TIRADOR ---------------------------------------------------------------------------------

// Logros de chut (dianas)
        new LogrosDescription.descLogro( "ACH_KICK_DIA_01", "Goleador en ciernes","Consigue 25 dianas", 10 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_02", "Promesa","Consigue 100 dianas", 25 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_03", "Ojo de halcón","Consigue 500 dianas", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_04", "Goleador","Consigue 1000 dianas", 100 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_05", "Artillero","Consigue 2000 dianas", 200 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_06", "Astro del balón","Consigue 5000 dianas", 500 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_07", "Bota de oro","Consigue 7000 dianas", 900 ),
        new LogrosDescription.descLogro( "ACH_KICK_DIA_08", "Legendario","Consigue 10000 dianas", 1500 ),
// Logros de chut (puntuación)
        new LogrosDescription.descLogro( "ACH_KICK_TPU_01", "Diploma","Acumula 10000 puntos en tiro a portería", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_02", "Medalla","Acumula 50000 puntos en tiro a portería", 100 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_03", "Título","Acumula 100000 puntos en tiro a portería", 200 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_04", "Galardón","Acumula 500000 puntos en tiro a portería", 300 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_05", "Trofeo","Acumula 700000 puntos en tiro a portería", 400 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_06", "Copa","Acumula 900000 puntos en tiro a portería", 600 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_07", "Súper Copa","Acumula 1000000 puntos en tiro a portería", 1000 ),
        new LogrosDescription.descLogro( "ACH_KICK_TPU_08", "Hall of Fame","Acumula 1500000 puntos en tiro a portería", 1200 ),
// Logros de chut (fallos)
        new LogrosDescription.descLogro( "ACH_KICK_TFA_01", "Aprende de tus fallos","Falla 100 lanzamientos", 10 ),
        new LogrosDescription.descLogro( "ACH_KICK_TFA_02", "Mira desviada","Falla 500 lanzamientos", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_TFA_03", "Una mala tarde","Falla 800 lanzamientos", 100 ),
        new LogrosDescription.descLogro( "ACH_KICK_TFA_04", "No pasa nada","Falla 1000 lanzamientos", 500 ),

// LOGROS MULTIJUGADOR -----------------------------------------------------------------------------

// Logros por actividad
        new LogrosDescription.descLogro( "ACH_KICK_MUL_DUEL_01", "Neófito", "Juega tu primer duelo", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_MUL_DUEL_02", "La docena","Juega 12 duelos", 200 ),
        new LogrosDescription.descLogro( "ACH_KICK_MUL_DUEL_03", "Experimentado","Juega 100 duelos", 1000 ),
// Logros por victorias
        new LogrosDescription.descLogro( "ACH_KICK_MUL_WON_01", "Con buen pie","Vence tu primer duelo", 50 ),
        new LogrosDescription.descLogro( "ACH_KICK_MUL_WON_02", "¡De 10!","Gana 10 duelos", 250 ),
        new LogrosDescription.descLogro( "ACH_KICK_MUL_WON_03", "Centenario","Vence en 100 duelos", 1000 ),
// Logros por derrotas
        new LogrosDescription.descLogro( "ACH_KICK_MUL_LOST_01", "La otra mejilla", "Pierde 10 duelos multijugador", 50 ),
// Logors por perfeccion
        new LogrosDescription.descLogro( "ACH_KICK_MUL_PFCT_01", "Precisión de cirujano", "Marca todos los goles en un duelo", 100 ),
        new LogrosDescription.descLogro( "ACH_KICK_MUL_PFCT_02", "Infranqueable", "Para o despeja todos los balones en un duelo", 100 ),
        };

        LogrosDescription tmp = ScriptableObject.CreateInstance<LogrosDescription>();
        tmp.m_lista = logrosTmp;
        AssetDatabase.CreateAsset(tmp, "Assets/Resources/Logros.asset");

        Debug.Log("Se ha actualizado la lista de logros.");
        Debug.LogWarning("NOTA 1: No olvides acceder a \"Interfaz/Logros/cntLogros\" y verificar que la propiedad \"Logros\" tiene valor, si no la ejecución fallará. (asígnale el asset \"Logros.asset\") que se acaba de generar");
        Debug.LogWarning("NOTA 2: Si el número de logros de cada tipo (tirador, portero, multijugador) ha cambiado, no olvides acceder a la clase \"cntLogros\" y actualizar las constantes: NUM_LOGROS_TIRADOR, NUM_LOGROS_PORTERO y NUM_LOGROS_MULTIJUGADOR)");
    }
}