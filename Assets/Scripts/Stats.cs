using UnityEngine;
using System.Collections;

public class Stats {
    public const int version = 11;
    public const int zona = 1;
    public const string tipo = "BITOONKICKS_MULTIPLAYER";
    


    // dimensiones de la pantalla
    public const float SCREEN_WIDTH = 1024; //940;
    public const float SCREEN_HEIGHT = 705;

    // ratios para calcular el desplazamiento lateral de las pantallas
    public const float SCREEN_X_RATIO = SCREEN_WIDTH / 940;

    // Propiedades para elementos de la interfaz
    public static Rect FONDO_PIXEL_INSET = new Rect(-SCREEN_WIDTH / 2, -SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT);      // rectangulo para definir la textura de fondo de pantalla

    // posiciones de la pantalla cuando se encuentra oculta
    public static Vector3 POS_PANTALLA_OCULTA_IZDA = new Vector3(-SCREEN_X_RATIO, 0.0f, 0.0f);      // pantalla oculta por la izquierda
    public static Vector3 POS_PANTALLA_OCULTA_DCHA = new Vector3(SCREEN_X_RATIO, 0.0f, 0.0f);       // pantalla oculta por la derecha

    // tiempo (en SEGUNDOS) que se muestra el mensaje de VS en la pantalla de duelo antes de pasar a jugarlo
    public const float TIEMPO_ESPERA_MOSTRAR_PANTALLA_VS = 2.0f;

	// tiempo (en SEGUNDOS) que se muestra la pantalla de BUSCANDO RIVAL
	public const float TIEMPO_ESPERA_MOSTRAR_PANTALLA_BUSCANDORIVAL = 6.0f;

    // tiempo (en SEGUNDOS) maximo que la alerta de "HAS RECIBIDO UN RETO" permanece abierta si el jugador local no la acepta
    public const float TIEMPO_ESPERA_RIVAL_ACEPTAR_RETO = 10.0f;

    // tiempo (en SEGUNDOS) maximo que el usuario local espera a que el remoto acepte el duelo
    // NOTA: "TIEMPO_ESPERA_RIVAL_ACEPTAR_RETO" debe ser <= que "TIEMPO_ESPERA_CONFIRMACION_RETO_RIVAL"
    public const float TIEMPO_ESPERA_CONFIRMACION_RETO_RIVAL = 15.0f;

    // tiempo (en SEGUNDOS)  que se muestra el tooltip sobre un boton de modo de juego
    public const float TIEMPO_TOOLTIP_MODO_JUEGO = 5.0f;

    // tiempo (en SEGUNDOS) total que puede estar un jugador en modo multijugador sin tirar
    public const float CUENTA_ATRAS_TIRO_TIEMPO_TOTAL = 10.0f;

    // ultimos segunos que se muestran de la cuenta atras en modo multijugador
    public const float CUENTA_ATRAS_TIRO_MOSTRAR = 3.0f;

    // tiempo que tarda en arrancar el time_attack
    public const float TIME_TIME_ATTACK_CUENTA_ATRAS = 3.0f;

    // tiempo base de una partida en modo time_attack
    public const float TIME_ATTACK_TIEMPO_BASE = 60.0f;

    // precio que cuesta enviar un duelo a otro jugador
    public const int PRECIO_RETO = 100;

    // posicion horizontal del jugador cuando esta en el vestuario
    public const float PLAYER_VESTUARIO_COORDENADA_X = 0.22f;

    
}
