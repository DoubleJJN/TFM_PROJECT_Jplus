using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class JuegoDePuzzle : MonoBehaviour
{
    // Variables públicas (se mantienen para no romper el Inspector de Unity)
    public Button[] piezas1 = new Button[4];
    public Button[] piezas2 = new Button[6];
    public Button[] piezas3 = new Button[9];

    public RectTransform[] posiciones1 = new RectTransform[4];
    public RectTransform[] posiciones2 = new RectTransform[6];
    public RectTransform[] posiciones3 = new RectTransform[9];
    
    public RectTransform posOut1; 
    public RectTransform posOut2; 
    public RectTransform posOut3; 
    
    public static bool[] ocupado1 = { false, true, true, true };
    public static bool[] ocupado2 = { true, true, true, false, true, true };
    public static bool[] ocupado3 = { true, true, true, true, false, true, true, true, true };

    public TextMeshProUGUI textoNivel;
    public int nivelActual = 1;
    public GameObject[] ComponentesNivel = new GameObject[3]; 
    public PopUpGanar popUpGanar; 
    public GameObject imagenBien, imagenMal;
    
    public static int[] estadoGanador1;
    public static int[] estadoGanador2;
    public static int[] estadoGanador3;
    
    public static int[] posiciones1_actual = { 3, 2, -1, 1 }; 
    public static int[] posiciones2_actual = { 0, 1, 5 , 2, 4, -1 };
    public static int[] posiciones3_actual = { 0, 5, 1, 3, 7, 2, 6, 8, -1 };

    // --- NUEVAS VARIABLES MAESTRAS (Arrays de Arrays) ---
    private Button[][] piezasMaestro;
    private RectTransform[][] posicionesMaestro;
    private RectTransform[] posOutMaestro;
    private bool[][] ocupadoMaestro;
    private int[][] posicionesActualesMaestro;
    private int[][] estadoGanadorMaestro;
    private int[][] estadoInicialMaestro;
    private int[][][] adyacentesMaestro;
    private int restartCounter = 0;
    private bool bloqueado = false; // NUESTRO CANDADO VISUAL

    // Lógica de adyacentes (se mantiene intacta)
    private int[][] adyacentes1 = { new int[] { 1, 2 }, new int[] { 0, 3 }, new int[] { 0, 3 }, new int[] { 1, 2 } };
    private int[][] adyacentes2 = { new int[] { 1, 3 }, new int[] { 0, 2, 4 }, new int[] { 1, 5 }, new int[] { 0, 4 }, new int[] { 1, 3, 5 }, new int[] { 2, 4 } };
    private int[][] adyacentes3 = { new int[] { 1, 3 }, new int[] { 0, 2, 4 }, new int[] { 1, 5 }, new int[] { 0, 4, 6 }, new int[] { 1, 3, 5, 7 }, new int[] { 2, 4, 8 }, new int[] { 3, 7 }, new int[] { 4, 6, 8 }, new int[] { 5, 7 } };

    public void SetValor(bool valor)
    {
        if (valor)
        {
            restartCounter = 0;
            bloqueado = false;

            // 2. Restauramos de fábrica los 3 niveles de golpe
            for (int nivelIndex = 0; nivelIndex < 3; nivelIndex++)
            {
                // Restaurar los arrays de posiciones iniciales
                posicionesActualesMaestro[nivelIndex] = (int[])estadoInicialMaestro[nivelIndex].Clone();
                
                // Restaurar los huecos y ocupaciones originales
                if (nivelIndex == 0) ocupadoMaestro[0] = new bool[] { false, true, true, true };
                if (nivelIndex == 1) ocupadoMaestro[1] = new bool[] { true, true, true, false, true, true };
                if (nivelIndex == 2) ocupadoMaestro[2] = new bool[] { true, true, true, true, false, true, true, true, true };

                // Devolver visualmente TODAS las piezas a sus sitios de inicio en los 3 paneles
                for (int i = 0; i < piezasMaestro[nivelIndex].Length; i++)
                {
                    if (posicionesActualesMaestro[nivelIndex][i] != -1)
                    {
                        piezasMaestro[nivelIndex][i].transform.position = posicionesMaestro[nivelIndex][posicionesActualesMaestro[nivelIndex][i]].position;
                    }
                    else if (posOutMaestro[nivelIndex] != null)
                    {
                        piezasMaestro[nivelIndex][i].transform.position = posOutMaestro[nivelIndex].position;
                    }
                }
            }
            // 3. Volvemos al Nivel 1
            CambiarNivel(1);
        }
    }
    void Start()
    {
        // 1. Configurar estados ganadores
        estadoGanador1 = new int[] { 0, 1, 2, 3 };
        estadoGanador2 = new int[] { 0, 1, 2, 3, 4, 5 };
        estadoGanador3 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // 2. AGRUPAR TODO EN LOS ARRAYS MAESTROS
        piezasMaestro = new Button[][] { piezas1, piezas2, piezas3 };
        posicionesMaestro = new RectTransform[][] { posiciones1, posiciones2, posiciones3 };
        posOutMaestro = new RectTransform[] { posOut1, posOut2, posOut3 };
        ocupadoMaestro = new bool[][] { ocupado1, ocupado2, ocupado3 };
        posicionesActualesMaestro = new int[][] { posiciones1_actual, posiciones2_actual, posiciones3_actual };
        estadoGanadorMaestro = new int[][] { estadoGanador1, estadoGanador2, estadoGanador3 };
        adyacentesMaestro = new int[][][] { adyacentes1, adyacentes2, adyacentes3 };
        
        // Clonar estados iniciales
        estadoInicialMaestro = new int[][] { 
            (int[])posiciones1_actual.Clone(), 
            (int[])posiciones2_actual.Clone(), 
            (int[])posiciones3_actual.Clone() 
        };

        // 3. Configurar listeners de forma dinámica y limpia
        for (int nivel = 0; nivel < 3; nivel++)
        {
            for (int p = 0; p < piezasMaestro[nivel].Length; p++)
            {
                piezasMaestro[nivel][p].onClick.RemoveAllListeners();
                int nivelLocal = nivel;
                int piezaLocal = p;
                piezasMaestro[nivel][p].onClick.AddListener(() => ClickearBotonMaestro(nivelLocal, piezaLocal));
            }
        }
        SetValor(true); // Iniciar el juego con la configuración inicial
        CambiarNivel(1);
        ColocarPiezasIniciales();
    }

    void ColocarPiezasIniciales()
    {
        for (int nivel = 0; nivel < 3; nivel++)
        {
            for (int i = 0; i < piezasMaestro[nivel].Length; i++)
            {
                if (posicionesActualesMaestro[nivel][i] != -1)
                    piezasMaestro[nivel][i].transform.position = posicionesMaestro[nivel][posicionesActualesMaestro[nivel][i]].position;
            }
        }
    }

    void ActualizarTextoNivel()
    {
        if (textoNivel != null) textoNivel.text = "Nivel " + nivelActual;
    }

    void CambiarNivel(int nuevoNivel)
    {
        nivelActual = nuevoNivel;
        ActualizarTextoNivel();
        
        for (int i = 0; i < ComponentesNivel.Length; i++)
        {
            if (ComponentesNivel[i] != null) ComponentesNivel[i].SetActive(i == nivelActual - 1);
        }
    }

    // --- FUNCIÓN UNIFICADA DE CLICS ---
    void ClickearBotonMaestro(int nivelIndex, int indiceBoton)
    {
        if (bloqueado) return; // CANDADO

        int posicionActual = posicionesActualesMaestro[nivelIndex][indiceBoton];
        int indexHueco = System.Array.IndexOf(ocupadoMaestro[nivelIndex], false); // Forma rápida de buscar el 'false'

        if (indexHueco == -1) return;

        // Si viene de fuera
        if (posicionActual == -1)
        {
            MoverPieza(nivelIndex, indiceBoton, posicionActual, indexHueco);
            StartCoroutine(RutinaVerificarNivelCompleto(nivelIndex));
            return;
        }

        // Comprobar adyacencia
        bool esAdyacente = System.Array.Exists(adyacentesMaestro[nivelIndex][indexHueco], element => element == posicionActual);

        if (esAdyacente)
        {
            MoverPieza(nivelIndex, indiceBoton, posicionActual, indexHueco);
            StartCoroutine(RutinaVerificarNivelCompleto(nivelIndex));
        }
    }

    void MoverPieza(int nivelIndex, int indiceBoton, int posActual, int indexHueco)
    {
        if (posActual != -1) ocupadoMaestro[nivelIndex][posActual] = false;
        
        ocupadoMaestro[nivelIndex][indexHueco] = true;
        posicionesActualesMaestro[nivelIndex][indiceBoton] = indexHueco;
        piezasMaestro[nivelIndex][indiceBoton].transform.position = posicionesMaestro[nivelIndex][indexHueco].position;
    }

    // --- CORRUTINA UNIFICADA DE VERIFICACIÓN CON IMÁGENES ---
    IEnumerator RutinaVerificarNivelCompleto(int nivelIndex)
    {
        // 1. ¿Está todo ocupado?
        if (System.Array.Exists(ocupadoMaestro[nivelIndex], element => element == false)) 
            yield break; // Aún hay un hueco, el jugador sigue jugando

        bloqueado = true; // CERRAMOS CANDADO

        // 2. ¿Están en orden ganador?
        bool nivelCompletado = true;
        for (int i = 0; i < posicionesActualesMaestro[nivelIndex].Length; i++)
        {
            if (posicionesActualesMaestro[nivelIndex][i] != estadoGanadorMaestro[nivelIndex][i])
            {
                nivelCompletado = false;
                break;
            }
        }

        if (nivelCompletado)
        {
            // GANAR
            yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenBien, 1f));
            
            if (nivelActual < 3)
            {
                CambiarNivel(nivelActual + 1);
            }
            else
            {
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }
        }
        else
        {
            // PERDER INTENTO
            yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenMal, 1f));
            ResetearNivel(nivelIndex, true); // True indica que la pieza vuelve afuera
        }

        bloqueado = false; // ABRIMOS CANDADO
    }

    // --- RESETEO UNIFICADO ---
    void ResetearNivel(int nivelIndex = -1, bool castigo = false)
    {
        // Si no se le pasa nivel (por el botón externo), asume el nivel actual
        if (nivelIndex == -1) nivelIndex = nivelActual - 1;

        restartCounter++;

        // Restaurar arrays
        posicionesActualesMaestro[nivelIndex] = (int[])estadoInicialMaestro[nivelIndex].Clone();
        
        // Restaurar ocupados según nivel
        if (nivelIndex == 0) ocupadoMaestro[0] = new bool[] { false, true, true, true };
        if (nivelIndex == 1) ocupadoMaestro[1] = new bool[] { true, true, true, false, true, true };
        if (nivelIndex == 2) ocupadoMaestro[2] = new bool[] { true, true, true, true, false, true, true, true, true };

        // Devolver piezas a sus sitios
        for (int i = 0; i < piezasMaestro[nivelIndex].Length; i++)
        {
            if (posicionesActualesMaestro[nivelIndex][i] != -1)
                piezasMaestro[nivelIndex][i].transform.position = posicionesMaestro[nivelIndex][posicionesActualesMaestro[nivelIndex][i]].position;
            else if (posOutMaestro[nivelIndex] != null)
                piezasMaestro[nivelIndex][i].transform.position = posOutMaestro[nivelIndex].position;
        }

        if (EstasBloqueado(nivelIndex) && restartCounter > 4)
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
        }
    }

    bool EstasBloqueado(int nivelIndex)
    {
        for (int i = 0; i < piezasMaestro[nivelIndex].Length; i++)
        {
            int posActual = posicionesActualesMaestro[nivelIndex][i];
            if (posActual == -1) continue; 
            
            foreach (int adj in adyacentesMaestro[nivelIndex][posActual])
            {
                if (!ocupadoMaestro[nivelIndex][adj]) return false; // Puede moverse
            }
        }
        return true; // No puede moverse
    }

    void MostrarPopUpGanadoConRetraso()
    {
        if (popUpGanar != null)
        {
            popUpGanar.SetNombreJuego("Puzzle");
            string mensajeGanado = "";
        if (restartCounter == 0)
        {
            mensajeGanado = "¡Felicidades! ¡Has completado todos los niveles sin errores!";
        }
        else if (restartCounter == 1)
        {
            mensajeGanado = "¡Bien hecho! ¡Has completado todos los niveles con solo 1 error!";
        }
        else if (restartCounter == 2)
        {
            mensajeGanado = "¡Lo lograste! Completaste todos los niveles con " + restartCounter + " errores.";
        }
        else
        {
            mensajeGanado = "Cometiste " + restartCounter + " errores. ¡Intenta mejorar!";
        }
        popUpGanar.MostrarPopUpGanado(restartCounter, mensajeGanado);
        }
    }

    IEnumerator MostrarImagenTiempoLimitado(GameObject imagen, float duracion)
    {
        if (imagen == null) yield break;
        imagen.SetActive(true);
        yield return new WaitForSeconds(duracion);
        imagen.SetActive(false);
    }
}