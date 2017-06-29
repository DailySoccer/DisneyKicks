using UnityEngine;
using System.Collections;

public class LocalizacionManager : MonoBehaviour {


    // ------------------------------------------------------------------------------
    // ---  PROPIEDADES  ------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Instancia de esta clase
    /// </summary>
    public static LocalizacionManager instance {
        get {
            if (m_instance == null) {
                GameObject go = GameObject.Find("AnilloUnico");
                if (go != null)
                    m_instance = go.transform.GetComponent<LocalizacionManager>();
            }

            return m_instance;
        }
    }
    private static LocalizacionManager m_instance;

    // traducciones de los textos del juego a un determinado idioma
    private string[] m_textos;


    // ------------------------------------------------------------------------------
    // ---  CONSTRUCTOR  ------------------------------------------------------------
    // ------------------------------------------------------------------------------

    void Awake() {
        m_instance = this;
    }


    public LocalizacionManager() {
        // cargar el fichero de idioma por defecto
        LoadIdiomaFile("ES");
    }


    // ------------------------------------------------------------------------------
    // ---  METODOS  ----------------------------------------------------------------
    // ------------------------------------------------------------------------------


    /// <summary>
    /// Metodo para cargar un determinado fichero de idioma
    /// </summary>
    /// <param name="_idIdioma"></param>
    private void LoadIdiomaFile(string _idIdioma) {
        // F4KE: inicializar las variables de idioma
        string[] textos = {
            "sound",                                        // id = 1       PANTALLA OPCIONES
            "fx",
            "music",
            "graphic quality",
            "high",                                         // id = 5
            "medium",
            "low",
            "career",                                       // id = 8       PANTALLA MAIN_MENU
            "JUGAR",
            "select mission",                               // id = 10      PANTALLA CARRERA
            "mission",
            "level",
            "go!!",
            "Lanzador",                                      // id = 14      PANTALLA VESTUARIO
            "Portero",                                   // id = 15
            "info",
            "powerups",
            "multipliers",
            "ready!!",
            "player locked",                                // id = 20
            "Finish mission{0} to unlock this player",
            "early buy",
            "player available",
            "Add{0} to your squad",
            "",                                             // id = 25
            "kit locked",
            "Finish level{0} to unlock this kit",
            "kit available",
            "You can buy this kit",
            "buy",                                          // id = 30
            "level",                                        // id = 31      PANTALLA LOGROS
            "high score",                                   // id = 32      PANTALLA PERFIL
            "scored goals",
            "bullseye hits", // dianas acertadas
            "missed shots", // balones fallados             // id = 35
            "cleared shots", // balones despejados
            "Perfect saves",
            "conceded goals", // balones encajados
            "played duels",
            "won duels",                                    // id = 40
            "perfect duels",
            "win percentage",
            "buy coins",                                    // id = 43     DIALOGO DE COMPRA DE MONEDAS
            "Choose a pack",
            "ok",                                           // id = 45
            "¤",    // coins
            "§",    // bitoons
            "cancel",
            "reject",
            "{1} goals {0}",                                // id = 50      OBJETIVOS DE MISION
            "Link",
            "Achieve ",      // se refiere a logros
            "{1} {2}{0} to a {3}",
            "shots",
            "shot",                                         // id = 55
            "training net",  // sabana
            "bullseye",
            "high ",
            "medium ",
            "low ",                                         // id = 60
            "bonus",
            "bonuses",
            "{1} {2}effect {3}{0}",
            "Make",
            "save",                                         // id = 65
            "saves",
            "Perfect ",
            "{1} {2}{3}{0}",
            "life",
            "lives",                                        // id = 70
            "Finish with {0} {1}",
            "point",
            "points",
            "Score {0} {1}",
            "Concede no goal",                              // id = 75
            "Perfect",
            "Perfects",
            "shield locked",                                // id = 78      DIALOGOS COMPRA ESCUDOS / POWER_UPS
            "You can only buy and use this shield after finishing level{0}",
            "buy {0}",                                      // id = 80
            "You have{0} {1} {2} in your inventory.",
            "enable",
            "disable",
            "warning",
            "You have reached the maximum number of{0} units",     // id = 85
            "You don't have enough{0} to buy this item",
            "Do you want to early buy this kit?",           // id = 87      TOOLTIP DE JUGADOR / EQUIPACION DISPONIBLE
            "buy kit",
            "Do you want to buy this kit? To proceed, please select a currency to make the payment.",
            "You don't have enough{0} to buy{1}",         // id = 90
            "buy player",
            "The termination clause of{0} is{1}. Do you want to pay it?",
            "Do you want to add{0} to your squad? To proceed, please select a currency to make the payment.",
            "You need at least one striker to play this game mode. You should buy one to continue.",    // id = 94       PANTALLA VESTUARIO
            "You need at least one goalkeeper to play this game mode. You should buy one to continue.", // id = 95
            "Connecting",                                   // id = 96      CONEXION
            "Please wait...",
            "There are no available servers. Please, try again later.",
            "Server not found. Please, try again later.",
            "duel rejected",                                // id = 100     DUELOS
            "{0} has rejected the duel.",
            "player challenge",
            "Do you want to challenge{0} ({1} victories)?",
            "waiting for the opponent",
            "Waiting for your opponent accepts the duel...",     //id = 105
            "you have been challenged!!",
            "{0} ({1} victories) challenges you to a duel.",
            "challenge!!",
            "duel accepted!!",
            "Cargando...",                                       // id = 110
            "Score",    // se refiere a goles
            "Get",      // se refiere a puntos
            "repeat",                                           // id = 113     PANTALLA DE GAME OVER
            "continue",
            "you win",                                          // id = 115
            "game over",
            "exit",                                             // id = 117     PANTALLA DE PAUSA
            "retry",
            "Focus",
            "Flash",                                            // id = 120
            "Sharpshooter",
            "Greasy Ball",
            "Phase Ball",
            "Power Glove",
            "Reflection",                                       // id = 125
            "Instinct",
            "Bullet Time",
            "Increases shot drawing time.",
            "Produces a blinding flash of light, confusing the goalie.",
            "Sharpens the striker's sight, providing a precision zoom to draw the shot.", // id = 130
            "The ball is covered with grease, the ball may slip from the goalie's hands.",
            "The ball blinks in the air, confusing the goalkeeper.",
            "The goalie's gloves become charged, greatly increasing the Perfect ratio.",
            "The goalkeeper divides into 9 copies of himself, covering a wider area.",
            "The goalie uses his instinct to foresee where the ball is going to hit.",//id = 135
            "Time slows down, giving the goalkeeper extra time to defend the goal.",
            "round",
            "powerup",
            "x1.5 Multiplier",
            "x1.8 Multiplier",                                  // id = 140
            "x2.0 Multiplier",
            "Multiplies your score by 1.5.",
            "Multiplies your score by 1.8.",
            "Multiplies your score by 2.0.",
            "rounds",                                           // id = 145
            "Wall",
            "PRO Wall",
            "Scorer",
            "Heroic",
            "Tricky",                                           // id = 150
            "Practical",
            "Premonition",
            "Incentive",
            "V.I.P.",
            "Hawk Eye",                                         // id = 155
            "Summons a two-player wall.",
            "Summons a three-player wall.",
            "Shooting to a goalkeeper rewards double points.",
            "Clearing a ball rewards double points.",
            "Effect shots increase the probability of scoring to a goalkeeper.", //id = 160
            "Increases ball clearing skill, but decreases Perfect chances.",
            "After failing a shot, precisely predicts the next one.",
            "Bullseye hits reward double points.",
            "Perfect shots restore all lives.",
            "Bullseyes are bigger.",                           // id = 165
            "Bullseyes",
            "Hit {0} bullseyes",
            "Striker score",
            "Gather {0} points as striker",
            "Perfects",                                         // id = 170
            "Get {0} Perfects",
            "Goalkeeper score",
            "Gather {0} points as goalkeeper",
            "Clears",
            "Clear {0} shots",                                  // id = 175
            "Perfect saves",
            "Make {0} Perfect saves",
            "Duels played",
            "Play {0} duels",
            "Duel victories",                                   // id = 180
            "Win {0} duels",
            "Perfect duels",
            "Score all goals and receive none during a duel",
            "victory!",
            "game over",                                        // id = 185
            "result",
            "reward",
            "rematch",
            "achievements",
            "stopped shots",                                    // id = 190
            "goals scored",
            "you got new achievement rewards!",
            "Your total reward is",
            "",
            "Server is not responding",                         // id = 195
            "Player {0} disconnected.",
            "Right now player {0} is already being challenged.\nTry again in a minute.",
            "Player {0} is already playing a match.\nUpdating lobby...",
            "You are being challenged.",
            "Exceeded wait time.\nYou have been disconnected for inactivity.", // id = 200
            "Could not connect to multiplayer server.",
            "Conection was lost.",
            "Your version of the game is not compatible.\nPlease, update the game.",
            "Your profile is already playing, possibly on another device.\nMake a new user or log out on the other device.",
            "The server cannot find your user profile.",        // id = 205
            "Unknown error",
            "Conection problem",
            "Player disconnected",
            "Player busy",
            "Player already playing",                           // id = 210
            "Already in a challenge",
            "Inactivity disconection",
            "Incorrect version",
            "Login conflict",
            "Unregistered player",                              // id = 215
            "keep using multiplier?",
            "You had a {0} enabled. Do you want to keep using it? Another one will be deducted ( {1} left).",
            "Draw a line from the ball to your target to shoot.",//"Traza una línea desde el balón a tu objetivo para disparar."
            "Hit the center of the bullseye to make a Perfect.",//"Golpea en el centro de la diana para conseguir un Perfecto."
            "Each Perfect shot gives you one life, up to 3 lives.",//"Cada lanzamiento Perfecto te otorgará una vida, hasta un máximo de 3." // id = 220
            "You can draw curved shots, which give effect to the ball and extra points.",//"También puedes trazar tiros curvos, que tendrán mayor efecto y puntuación."
            "There are 3 different point bonuses, rewarded according to the amount of effect given to the shot.",//"Existen 3 bonificadores de la puntuación, otorgados en función de la cantidad de efecto del disparo."
            "Draw a line from the goalkeeper to the red point to perform a defense.",//"Traza una línea desde el portero al punto rojo para realizar una parada."
            "Wait for the ball to be at the proper distance. Don't jump too fast!",//"Espera a que el balón esté a la distancia correcta. ¡No te adelantes!"
            "If you forsee the shot position and perform the defense at the right time, you will make a Perfect.",//"Si prevés la posición del tiro y realizas el trazo en el momento adecuado, lograrás un Perfecto." // id = 225
            "Each Perfect save gives you a life.",//"Cada parada Perfecta te otorgará una vida."
            "You will be rewarded according to the precision of your defense.",//"En función de la precisión de la parada, recibirás diferente puntuación."
            "A different challenge for strikers! Now you have to hit the bullseye or the yellow zone.",//"¡Una prueba distinta de Lanzador! Ahora tienes que acertar en la diana o en la zona amarilla."
            "Try to complete the mission with at least one life remaining. Game over, otherwise!",//"Procura llegar al final de la misión con al menos una vida, ¡o serás eliminado!"
            "You will be rewarded with a different amount of points, according to the kind of target you hit.",//"Obtendrás diferente puntuación en función del tipo de objetivo sobre el que aciertes." // id = 230
            "Achieve the 4 skill challenges of each mission to get incentive rewards. ¡Money!",//"Supera los 4 retos de habilidad propuestos en cada misión para conseguir primas. ¡Monedas!"
            "Complete a mission to unlock the next one. Keep progressing to unlock new players, kits and more!",//"Desbloquea elementos del vestuario y nuevas misiones llegando al final de la misión con 1 vida o más."
            "Difficulty of the shots will increase during the game. Prepare for crazy effect shots!",//"Los tiros incrementarán su dificultad durante el juego. ¡Prepárate para endiablados efectos!"
            "Now you play without help. Use what you learned to become a living wall.",//"Ahora juegas sin ayuda. Utiliza lo aprendido para convertirte en una muralla."
            "Help yourself with powerups to overcome harder rounds. You may use one per round!",//"Puedes ayudarte de power-ups para superar las rondas más difíciles. ¡Puedes usar uno por ronda!" // id = 235
            "You use a powerup by clicking on its icon at the top bar, always before the striker hits the ball.",//"Para usar un power-up, pincha sobre él en la barra superior antes de que se produzca el tiro."
            "Learn the benefits of each powerup to make the most of them!",//"¡Aprende las ventajas de los power-ups para sacarles el máximo partido!"
            "early!", //pronto
            "late!", //tarde
            "nice!", //paradon                                  // id = 240
            "awesome!!", //magnifico
            "good", //bien
            "perfect!!", //perfecto
            "fail!", //fallaste
            "out", //fuera                                      // id = 245
            "ball slipped!",
            "amazing!", //increible
            "hit!", //golazo
            "effect",
            "zone", //en zona                                   // id = 250
            "post!", //poste
            "crossbar!", //larguero
            "effect shots", //tiros con efecto
            "congratulations!!",
            "You unlocked {0}.",                           // id= 255
            "a new kit",
            "a new shield",
            "blocked duel",
            "You must beat {0} before playing duels",
            "score", // puntuación                             // id = 260
            "France",                                          // id = 261                 PAISES
            "South Africa",
            "Germany",
            "U.S.A.",
            "Spain",                                          // id = 265
            "Romania",
            "Holland",
            "Sweden",
            "Austria",
            "Jamaica",                                      // id = 270
            "Brazil",
            "Argentina",
            "Nigeria",
            "Greece",
            "Ireland",                                      // id = 275
            "England",
            "Mexico",
            "Chile",
            "Italy",
            "Japan",                                        // id = 280
            "yes",
            "no",
            "new player",
            "Choose a nickname:",
            "A nickname needs at least 6 characters.",      // id = 285
            "wall",
            "you achieved {0}!!",
            "completed!!",
            "points",
            "Couldn't get item prices from the store. Please try again later.",  // id = 290    No se pudo obtener los precios de la tienda. Por favor, vuelva a intentarlo más tarde.
            "net",
            " in a row",
            "Successfully defend {0} times{1}",
            "Stopped",
            "Balones de Oro",                               // id = 295
    };

        m_textos = textos;
    }


    /// <summary>
    /// Devuelve el texto con el identificador "_idTexto" traducido al idioma seleccionado actualmente
    /// </summary>
    /// <param name="_idTexto">Nota: los ids empiezan en 1</param>
    /// <returns></returns>
    public string GetTexto(int _idTexto) {
        if (m_textos != null)
            if (_idTexto > 0 && _idTexto <= m_textos.Length)
                return m_textos[(_idTexto- 1)];

        return "???";   // <= texto por defecto
    }
}
