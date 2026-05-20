using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class JuegoDePuzzle : MonoBehaviour
{
    public Button[] piezas1 = new Button[4];
    public Button[] piezas2 = new Button[6];
    public Button[] piezas3 = new Button[9];

    public RectTransform[] posiciones1 = new RectTransform[4];
    public RectTransform[] posiciones2 = new RectTransform[6];
    public RectTransform[] posiciones3 = new RectTransform[9];
    
    public RectTransform posOut1; // Posición inicial de la pieza fuera del tablero - Nivel 1
    public RectTransform posOut2; // Posición inicial de la pieza fuera del tablero - Nivel 2
    public RectTransform posOut3; // Posición inicial de la pieza fuera del tablero - Nivel 3
    
    public static bool[] ocupado1 = { false, true, true, true };
    public static bool[] ocupado2 = { true, true, true, false, true, true };
    public static bool[] ocupado3 = { true, true, true, true, false, true, true, true, true };

    public TextMeshProUGUI textoNivel;
    public int nivelActual = 1;
    public GameObject[] ComponentesNivel = new GameObject[3]; // Para activar/desactivar componentes según el nivel
    public PopUpGanar popUpGanar; // PopUp que se muestra cuando se completa el juego
    
    // Array para guardar el estado ganador de cada nivel
    public static int[] estadoGanador1;
    public static int[] estadoGanador2;
    public static int[] estadoGanador3;
    
    // Estados iniciales para resetear
    private int[] estadoInicial1;
    private int[] estadoInicial2;
    private int[] estadoInicial3;
    
    // Contador de fallos por nivel
    private int[] contadorFallos = new int[3]; // Para los 3 niveles
    private int fallosActuales = 0;
    
    // Última pieza clickeada (para trackear cual se debe devolver a posOut)
    private int ultimaPiezaClickeada = -1;
    
    // Lista de adyacentes para Nivel 1 (2x2)
    // Grid:
    // [0] [1]
    // [2] [3]
    private int[][] adyacentes1 = {
        new int[] { 1, 2 },    // Pos 0: conecta con 1 (derecha) y 2 (abajo)
        new int[] { 0, 3 },    // Pos 1: conecta con 0 (izquierda) y 3 (abajo)
        new int[] { 0, 3 },    // Pos 2: conecta con 0 (arriba) y 3 (derecha)
        new int[] { 1, 2 }     // Pos 3: conecta con 1 (arriba) y 2 (izquierda)
    };
    private int[][] adyacentes2 = {
        new int[] { 1, 3 },    // Pos 0: conecta con 1 (derecha) y 3 (abajo)
        new int[] { 0, 2, 4 }, // Pos 1: conecta con 0 (izquierda), 2 (derecha) y 4 (abajo)
        new int[] { 1, 5 },    // Pos 2: conecta con 1 (izquierda) y 5 (abajo)
        new int[] { 0, 4 },    // Pos 3: conecta con 0 (arriba) y 4 (derecha)
        new int[] { 1, 3, 5 }, // Pos 4: conecta con 1 (arriba), 3 (izquierda) y 5 (derecha)
        new int[] { 2, 4 }     // Pos 5: conecta con 2 (arriba) y 4 (izquierda)
    };
    private int[][] adyacentes3 = {
        new int[] { 1, 3 },    // Pos 0: conecta con 1 (derecha) y 3 (abajo)
        new int[] { 0, 2, 4 }, // Pos 1: conecta con 0 (izquierda), 2 (derecha) y 4 (abajo)
        new int[] { 1, 5 },    // Pos 2: conecta con 1 (izquierda) y 5 (abajo)
        new int[] { 0, 4, 6 }, // Pos 3: conecta con 0 (arriba), 4 (derecha) y 6 (abajo)
        new int[] { 1, 3, 5, 7 }, // Pos 4: conecta con 1 (arriba), 3 (izquierda), 5 (derecha) y 7 (abajo)
        new int[] { 2, 4, 8 }, // Pos 5: conecta con 2 (arriba), 4 (izquierda) y 8 (abajo)
        new int[] { 3, 7 },    // Pos 6: conecta con 3 (arriba) y 7 (derecha)
        new int[] { 4, 6, 8 }, // Pos 7: conecta con 4 (arriba), 6 (izquierda) y 8 (derecha)
        new int[] { 5, 7 }     // Pos 8: conecta con 5 (arriba) y 7 (izquierda)
    };
    // Índice = número de botón, Valor = posición donde está ese botón
    public static int[] posiciones1_actual = { 3, 2, -1, 1 }; // -1 significa que la pieza 0 (hueco) no está en el tablero, las otras piezas están en posiciones 2, 3 y 1 respectivamente
    public static int[] posiciones2_actual = { 0, 1, 5 , 2, 4, -1 };
    public static int[] posiciones3_actual = { 0, 5, 1, 3, 7, 2, 6, 8, -1 };
    void Start()
    {
        // El estado ganador es siempre: botón i en posición i
        estadoGanador1 = new int[] { 0, 1, 2, 3 };
        estadoGanador2 = new int[] { 0, 1, 2, 3, 4, 5 };
        estadoGanador3 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // Reiniciar estados a su valor original
        posiciones1_actual = new int[] { 3, 2, -1, 1 };
        posiciones2_actual = new int[] { 0, 1, 5, 2, 4, -1 };
        posiciones3_actual = new int[] { 0, 5, 1, 3, 7, 2, 6, 8, -1 };
        
        ocupado1 = new bool[] { false, true, true, true };
        ocupado2 = new bool[] { true, true, true, false, true, true };
        ocupado3 = new bool[] { true, true, true, true, false, true, true, true, true };

        // Guardar los estados iniciales para poder resetear
        estadoInicial1 = (int[])posiciones1_actual.Clone();
        estadoInicial2 = (int[])posiciones2_actual.Clone();
        estadoInicial3 = (int[])posiciones3_actual.Clone();
        
        // Inicializar contador de fallos
        contadorFallos = new int[3];
        fallosActuales = 0;
        
        // Reiniciar nivel a 1
        nivelActual = 1;

        // Actualizar el texto del nivel
        ActualizarTextoNivel();
        
        // Activar solo el componente del nivel actual, desactivar los otros
        for (int i = 0; i < ComponentesNivel.Length; i++)
        {
            if (ComponentesNivel[i] != null)
            {
                ComponentesNivel[i].SetActive(i == nivelActual - 1);
            }
        }
        
        // Limpiar listeners anteriores
        for (int i = 0; i < 4; i++)
        {
            piezas1[i].onClick.RemoveAllListeners();
        }
        for (int i = 0; i < 6; i++)
        {
            piezas2[i].onClick.RemoveAllListeners();
        }
        for (int i = 0; i < 9; i++)
        {
            piezas3[i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para todos los botones del Nivel 1
        for (int i = 0; i < 4; i++)
        {
            int indice = i;
            piezas1[i].onClick.AddListener(() => ClickearBotonNivel1(indice));
        }

        // Configurar listeners para todos los botones del Nivel 2
        for (int i = 0; i < 6; i++)
        {
            int indice = i;
            piezas2[i].onClick.AddListener(() => ClickearBotonNivel2(indice));
        }

        // Configurar listeners para todos los botones del Nivel 3
        for (int i = 0; i < 9; i++)
        {
            int indice = i;
            piezas3[i].onClick.AddListener(() => ClickearBotonNivel3(indice));
        }
        
        // Posicionar todos los botones en sus posiciones iniciales
        for (int i = 0; i < 4; i++)
        {
            if (posiciones1_actual[i] != -1)
                piezas1[i].transform.position = posiciones1[posiciones1_actual[i]].position;
        }
        for (int i = 0; i < 6; i++)
        {
            if (posiciones2_actual[i] != -1)
                piezas2[i].transform.position = posiciones2[posiciones2_actual[i]].position;
        }
        for (int i = 0; i < 9; i++)
        {
            if (posiciones3_actual[i] != -1)
                piezas3[i].transform.position = posiciones3[posiciones3_actual[i]].position;
        }
    }

    void ActualizarTextoNivel()
    {
        if (textoNivel != null)
        {
            textoNivel.text = "Nivel " + nivelActual;
        }
    }

    void CambiarNivel(int nuevoNivel)
    {
        nivelActual = nuevoNivel;
        ActualizarTextoNivel();
        
        // Activar solo el componente del nuevo nivel
        for (int i = 0; i < ComponentesNivel.Length; i++)
        {
            if (ComponentesNivel[i] != null)
            {
                ComponentesNivel[i].SetActive(i == nivelActual - 1);
            }
        }
    }

    void ResetearNivel()
    {
        if (nivelActual == 1)
        {
            posiciones1_actual = (int[])estadoInicial1.Clone();
            ocupado1 = new bool[] { false, true, true, true };
            for (int i = 0; i < 4; i++)
            {
                if (posiciones1_actual[i] != -1)
                    piezas1[i].transform.position = posiciones1[posiciones1_actual[i]].position;
            }
            fallosActuales++;
            contadorFallos[0] = fallosActuales;
            Debug.Log("Nivel 1 reseteado. Fallos: " + fallosActuales);
        }
        else if (nivelActual == 2)
        {
            posiciones2_actual = (int[])estadoInicial2.Clone();
            ocupado2 = new bool[] { true, true, true, false, true, true };
            for (int i = 0; i < 6; i++)
            {
                if (posiciones2_actual[i] != -1)
                    piezas2[i].transform.position = posiciones2[posiciones2_actual[i]].position;
            }
            fallosActuales++;
            contadorFallos[1] = fallosActuales;
            Debug.Log("Nivel 2 reseteado. Fallos: " + fallosActuales);
        }
        else if (nivelActual == 3)
        {
            posiciones3_actual = (int[])estadoInicial3.Clone();
            ocupado3 = new bool[] { true, true, true, true, false, true, true, true, true };
            for (int i = 0; i < 9; i++)
            {
                if (posiciones3_actual[i] != -1)
                    piezas3[i].transform.position = posiciones3[posiciones3_actual[i]].position;
            }
            fallosActuales++;
            contadorFallos[2] = fallosActuales;
            Debug.Log("Nivel 3 reseteado. Fallos: " + fallosActuales);
        }
        
        // Verificar si está bloqueado después del reset
        if (EstasBloqueado())
        {
            if (fallosActuales > 4)
            {
                Debug.Log("😢 ¡Estás bloqueado y superaste 4 fallos! Mostrando PopUp...");
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }
        }
    }

    public void SetValor(bool valor)
    {
        if (valor)
        {
            ResetearNivel();
        }
    }

    void VerificarNivelCompleto()
    {
        if (nivelActual == 1)
        {
            // Verificar si todas las posiciones están ocupadas
            bool todoOcupado = true;
            foreach (bool ocupada in ocupado1)
            {
                if (!ocupada)
                {
                    todoOcupado = false;
                    break;
                }
            }

            if (!todoOcupado) return;

            // Verificar si las piezas están en el mismo orden que el estado ganador
            bool nivelCompletado = true;
            for (int i = 0; i < 4; i++)
            {
                if (posiciones1_actual[i] != estadoGanador1[i])
                {
                    nivelCompletado = false;
                    break;
                }
            }

            if (nivelCompletado)
            {
                Debug.Log("🎉 ¡Nivel 1 completado!");
                CambiarNivel(nivelActual + 1);
            }
            else if (todoOcupado && !nivelCompletado)
            {
                Debug.Log("❌ Solución incorrecta en Nivel 1. Reseteando...");
                ResetearNivelConPiezaEnOut(1);
            }
        }
        else if (nivelActual == 2)
        {
            // Verificar si todas las posiciones están ocupadas
            bool todoOcupado = true;
            foreach (bool ocupada in ocupado2)
            {
                if (!ocupada)
                {
                    todoOcupado = false;
                    break;
                }
            }

            if (!todoOcupado) return;

            // Verificar si las piezas están en el mismo orden que el estado ganador
            bool nivelCompletado = true;
            for (int i = 0; i < 6; i++)
            {
                if (posiciones2_actual[i] != estadoGanador2[i])
                {
                    nivelCompletado = false;
                    break;
                }
            }

            if (nivelCompletado)
            {
                Debug.Log("🎉 ¡Nivel 2 completado!");
                CambiarNivel(nivelActual + 1);
            }
            else if (todoOcupado && !nivelCompletado)
            {
                Debug.Log("❌ Solución incorrecta en Nivel 2. Reseteando...");
                ResetearNivelConPiezaEnOut(2);
            }
        }
        else if (nivelActual == 3)
        {
            // Verificar si todas las posiciones están ocupadas
            bool todoOcupado = true;
            foreach (bool ocupada in ocupado3)
            {
                if (!ocupada)
                {
                    todoOcupado = false;
                    break;
                }
            }

            if (!todoOcupado) return;

            // Verificar si las piezas están en el mismo orden que el estado ganador
            bool nivelCompletado = true;
            for (int i = 0; i < 9; i++)
            {
                if (posiciones3_actual[i] != estadoGanador3[i])
                {
                    nivelCompletado = false;
                    break;
                }
            }

            if (nivelCompletado)
            {
                Debug.Log("🎉 ¡Nivel 3 completado!");
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }
            else if (todoOcupado && !nivelCompletado)
            {
                Debug.Log("❌ Solución incorrecta en Nivel 3. Reseteando...");
                ResetearNivelConPiezaEnOut(3);
            }
        }
    }

    void ResetearNivelConPiezaEnOut(int nivel)
    {
        fallosActuales++;
        
        if (nivel == 1)
        {
            // Primero: resetear TODO al estado inicial
            posiciones1_actual = (int[])estadoInicial1.Clone();
            ocupado1 = new bool[] { false, true, true, true };
            for (int i = 0; i < 4; i++)
            {
                if (posiciones1_actual[i] != -1)
                    piezas1[i].transform.position = posiciones1[posiciones1_actual[i]].position;
                else if (posOut1 != null)
                    piezas1[i].transform.position = posOut1.position;
            }
            
            contadorFallos[0] = fallosActuales;
            Debug.Log("Nivel 1 reseteado por solución incorrecta. Fallos: " + fallosActuales);
        }
        else if (nivel == 2)
        {
            posiciones2_actual = (int[])estadoInicial2.Clone();
            ocupado2 = new bool[] { true, true, true, false, true, true };
            for (int i = 0; i < 6; i++)
            {
                if (posiciones2_actual[i] != -1)
                    piezas2[i].transform.position = posiciones2[posiciones2_actual[i]].position;
                else if (posOut2 != null)
                    piezas2[i].transform.position = posOut2.position;
            }
            
            contadorFallos[1] = fallosActuales;
            Debug.Log("Nivel 2 reseteado por solución incorrecta. Fallos: " + fallosActuales);
        }
        else if (nivel == 3)
        {
            posiciones3_actual = (int[])estadoInicial3.Clone();
            ocupado3 = new bool[] { true, true, true, true, false, true, true, true, true };
            for (int i = 0; i < 9; i++)
            {
                if (posiciones3_actual[i] != -1)
                    piezas3[i].transform.position = posiciones3[posiciones3_actual[i]].position;
                else if (posOut3 != null)
                    piezas3[i].transform.position = posOut3.position;
            }
            
            contadorFallos[2] = fallosActuales;
            Debug.Log("Nivel 3 reseteado por solución incorrecta. Fallos: " + fallosActuales);
        }
    }

    void MostrarPopUpGanadoConRetraso()
    {
        if (popUpGanar == null)
        {
            Debug.LogError("❌ PopUpGanar no está asignado en el inspector");
            return;
        }
        
        popUpGanar.SetNombreJuego("Puzzle");
        string mensajeGanado = "";
        
        if (fallosActuales == 0)
        {
            mensajeGanado = "¡Felicidades! Has ganado sin cometer errores.";
        }
        else if (fallosActuales == 1)
        {
            mensajeGanado = "¡Bien hecho! Has ganado con solo un error.";
        }
        else
        {
            mensajeGanado = "Has ganado, pero cometiste algunos errores. ¡Sigue practicando!";
        }
        
        popUpGanar.MostrarPopUpGanado(fallosActuales, mensajeGanado);
    }

    bool EstasBloqueado()
    {
        if (nivelActual == 1)
        {
            // Verificar si hay movimientos posibles para todas las piezas
            for (int i = 0; i < 4; i++)
            {
                int posActual = posiciones1_actual[i];
                if (posActual == -1) continue; // Pieza fuera del tablero puede entrar
                
                // Obtener adyacentes disponibles
                foreach (int adj in adyacentes1[posActual])
                {
                    if (!ocupado1[adj])
                        return false; // Hay un movimiento disponible
                }
            }
            return true; // No hay movimientos disponibles
        }
        else if (nivelActual == 2)
        {
            for (int i = 0; i < 6; i++)
            {
                int posActual = posiciones2_actual[i];
                if (posActual == -1) continue;
                
                foreach (int adj in adyacentes2[posActual])
                {
                    if (!ocupado2[adj])
                        return false;
                }
            }
            return true;
        }
        else if (nivelActual == 3)
        {
            for (int i = 0; i < 9; i++)
            {
                int posActual = posiciones3_actual[i];
                if (posActual == -1) continue;
                
                foreach (int adj in adyacentes3[posActual])
                {
                    if (!ocupado3[adj])
                        return false;
                }
            }
            return true;
        }
        return false;
    }

    void GuardarPuntuacion()
    {
        // La puntuación se guarda automáticamente cuando se muestra el PopUp
        // TotalStarsCounter se llama desde PopUpGanar.MostrarPopUpGanado()
        Debug.Log("Puntuación de Puzzle: " + fallosActuales + " fallos");
    }

    void ClickearBotonNivel1(int indiceBotón)
    {
        ultimaPiezaClickeada = indiceBotón;
        // Obtener posición actual del botón
        int posicionActual = posiciones1_actual[indiceBotón];
        
        // Encontrar dónde está el hueco
        int indexHueco = -1;
        for (int i = 0; i < 4; i++)
        {
            if (!ocupado1[i])
            {
                indexHueco = i;
                break;
            }
        }

        if (indexHueco == -1)
        {
            Debug.LogError("❌ No se encontró el hueco");
            return;
        }

        // Si el botón está fuera del tablero (-1), puede moverse directamente al hueco
        if (posicionActual == -1)
        {
            //Debug.Log("✅ El botón " + indiceBotón + " estaba fuera del tablero. Entrando al hueco en posición " + indexHueco);
            ocupado1[indexHueco] = true;
            posiciones1_actual[indiceBotón] = indexHueco;
            piezas1[indiceBotón].transform.position = posiciones1[indexHueco].position;
            //Debug.Log("Posiciones: " + string.Join(", ", posiciones1_actual));
            //Debug.Log("Ocupado: " + string.Join(", ", ocupado1));
            VerificarNivelCompleto();
            return;
        }

        // Si el botón está en el tablero, verificar si es adyacente al hueco
        bool esAdyacente = false;
        foreach (int adj in adyacentes1[indexHueco])
        {
            if (adj == posicionActual)
            {
                esAdyacente = true;
                break;
            }
        }

        if (!esAdyacente)
        {
            //Debug.Log("❌ El botón " + indiceBotón + " en posición " + posicionActual + " NO está adyacente al hueco en " + indexHueco);
            return;
        }

        // Si es adyacente, intercambiar
        //Debug.Log("✅ El botón " + indiceBotón + " en posición " + posicionActual + " está adyacente al hueco. Moviendo...");
        
        // Intercambiar posiciones
        ocupado1[posicionActual] = false;
        ocupado1[indexHueco] = true;
        posiciones1_actual[indiceBotón] = indexHueco;
        
        // Mover el botón visualmente
        piezas1[indiceBotón].transform.position = posiciones1[indexHueco].position;

        //Debug.Log("Posiciones: " + string.Join(", ", posiciones1_actual));
        //Debug.Log("Ocupado: " + string.Join(", ", ocupado1));
        VerificarNivelCompleto();
    }

    void ClickearBotonNivel2(int indiceBotón)
    {
        ultimaPiezaClickeada = indiceBotón;
        // Obtener posición actual del botón
        int posicionActual = posiciones2_actual[indiceBotón];
        
        // Encontrar dónde está el hueco
        int indexHueco = -1;
        for (int i = 0; i < 6; i++)
        {
            if (!ocupado2[i])
            {
                indexHueco = i;
                break;
            }
        }

        if (indexHueco == -1)
        {
            Debug.LogError("❌ No se encontró el hueco");
            return;
        }

        // Si el botón está fuera del tablero (-1), puede moverse directamente al hueco
        if (posicionActual == -1)
        {
            Debug.Log("✅ El botón " + indiceBotón + " estaba fuera del tablero. Entrando al hueco en posición " + indexHueco);
            ocupado2[indexHueco] = true;
            posiciones2_actual[indiceBotón] = indexHueco;
            piezas2[indiceBotón].transform.position = posiciones2[indexHueco].position;
            Debug.Log("Posiciones: " + string.Join(", ", posiciones2_actual));
            Debug.Log("Ocupado: " + string.Join(", ", ocupado2));
            VerificarNivelCompleto();
            return;
        }

        // Si el botón está en el tablero, verificar si es adyacente al hueco
        bool esAdyacente = false;
        foreach (int adj in adyacentes2[indexHueco])
        {
            if (adj == posicionActual)
            {
                esAdyacente = true;
                break;
            }
        }

        if (!esAdyacente)
        {
            //Debug.Log("❌ El botón " + indiceBotón + " en posición " + posicionActual + " NO está adyacente al hueco en " + indexHueco);
            return;
        }

        // Si es adyacente, intercambiar
        //Debug.Log("✅ El botón " + indiceBotón + " en posición " + posicionActual + " está adyacente al hueco. Moviendo...");
        
        // Intercambiar posiciones
        ocupado2[posicionActual] = false;
        ocupado2[indexHueco] = true;
        posiciones2_actual[indiceBotón] = indexHueco;
        
        // Mover el botón visualmente
        piezas2[indiceBotón].transform.position = posiciones2[indexHueco].position;

        //Debug.Log("Posiciones: " + string.Join(", ", posiciones2_actual));
        //Debug.Log("Ocupado: " + string.Join(", ", ocupado2));
        VerificarNivelCompleto();
    }

    void ClickearBotonNivel3(int indiceBotón)
    {
        ultimaPiezaClickeada = indiceBotón;
        // Obtener posición actual del botón
        int posicionActual = posiciones3_actual[indiceBotón];
        
        // Encontrar dónde está el hueco
        int indexHueco = -1;
        for (int i = 0; i < 9; i++)
        {
            if (!ocupado3[i])
            {
                indexHueco = i;
                break;
            }
        }

        if (indexHueco == -1)
        {
            Debug.LogError("❌ No se encontró el hueco");
            return;
        }

        // Si el botón está fuera del tablero (-1), puede moverse directamente al hueco
        if (posicionActual == -1)
        {
            Debug.Log("✅ El botón " + indiceBotón + " estaba fuera del tablero. Entrando al hueco en posición " + indexHueco);
            ocupado3[indexHueco] = true;
            posiciones3_actual[indiceBotón] = indexHueco;
            piezas3[indiceBotón].transform.position = posiciones3[indexHueco].position;
            Debug.Log("Posiciones: " + string.Join(", ", posiciones3_actual));
            Debug.Log("Ocupado: " + string.Join(", ", ocupado3));
            VerificarNivelCompleto();
            return;
        }

        // Si el botón está en el tablero, verificar si es adyacente al hueco
        bool esAdyacente = false;
        foreach (int adj in adyacentes3[indexHueco])
        {
            if (adj == posicionActual)
            {
                esAdyacente = true;
                break;
            }
        }

        if (!esAdyacente)
        {
            return;
        }

        ocupado3[posicionActual] = false;
        ocupado3[indexHueco] = true;
        posiciones3_actual[indiceBotón] = indexHueco;
        
        // Mover el botón visualmente
        piezas3[indiceBotón].transform.position = posiciones3[indexHueco].position;

        //Debug.Log("Posiciones: " + string.Join(", ", posiciones3_actual));
        //Debug.Log("Ocupado: " + string.Join(", ", ocupado3));
        VerificarNivelCompleto();
    }
}

