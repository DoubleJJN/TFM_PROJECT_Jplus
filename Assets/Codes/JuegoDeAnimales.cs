using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class JuegoDeAnimales : MonoBehaviour
{
    public int nivelActual = 1;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoPregunta;
    public Button btnValidar;
    public GameObject[] panelesNiveles = new GameObject[3]; // Para los 3 niveles
    public PopUpGanar popUpGanar;
    public TotalStarsCounter totalStarsCounter;
    
    // Tracking de selecciones (jagged arrays)
    private bool[][] seleccionados = new bool[3][];
    private Image[][] marcadoresSeleccionado = new Image[3][];
    private Image[][] imagenesBoton = new Image[3][]; // Para cambiar color del botón
    private Color[][] coloresOriginales = new Color[3][]; // Guardar colores originales

    // Nombres de animales por nivel (asignable desde Inspector)
    public string[][] nombresAnimales = new string[3][];
    private Button[][] botonesAnimales = new Button[3][]; // Se buscan automáticamente
    
    // Nivel 1 - Solo una pregunta por ahora
    private string[] preguntasNivel1 = { "¿Cuáles son aves?" };
    private int[][] respuestasNivel1 = { new int[] { 1, 3 } }; // Pájaro (1) y Pingüino (3)

    // Nivel 2 - Reptiles
    private string[] preguntasNivel2 = { "¿Cuáles son reptiles?" };
    private int[][] respuestasNivel2 = { new int[] { 0, 4 } }; // Serpiente (0) y Tortuga (4)

    // Nivel 3 - Mamíferos
    private string[] preguntasNivel3 = { "¿Cuáles son mamíferos?" };
    private int[][] respuestasNivel3 = { new int[] { 1, 2, 3, 4 } }; // Vaca (1), León (2), Perro (3), Delfín (4)

    private int preguntaActual = 0;
    private int[] contadorFallos = new int[3];

    void Start()
    {
        InicializarJagged();
        BuscarComponentes();
        ConfigurarBotones();
        CambiarNivel(1);
    }

    void InicializarJagged()
    {
        // Buscar botones automáticamente dentro de cada panel de nivel
        for (int nivel = 0; nivel < 3; nivel++)
        {
            if (panelesNiveles[nivel] != null)
            {
                // Buscar todos los botones dentro del panel (exceptuando btnValidar)
                Button[] botonesEnPanel = panelesNiveles[nivel].GetComponentsInChildren<Button>();
                System.Collections.Generic.List<Button> botonesFiltrados = new System.Collections.Generic.List<Button>();
                
                foreach (Button btn in botonesEnPanel)
                {
                    if (btn != btnValidar) // Excluir el botón Validar
                        botonesFiltrados.Add(btn);
                }
                
                if (botonesFiltrados.Count > 0)
                {
                    // Inicializar arrays
                    botonesAnimales[nivel] = botonesFiltrados.ToArray();
                    seleccionados[nivel] = new bool[botonesAnimales[nivel].Length];
                    marcadoresSeleccionado[nivel] = new Image[botonesAnimales[nivel].Length];
                    imagenesBoton[nivel] = new Image[botonesAnimales[nivel].Length];
                    coloresOriginales[nivel] = new Color[botonesAnimales[nivel].Length];
                    nombresAnimales[nivel] = new string[botonesAnimales[nivel].Length];
                    
                    Debug.Log("✅ Nivel " + (nivel + 1) + ": Se encontraron " + botonesAnimales[nivel].Length + " botones");
                }
                else
                {
                    Debug.LogWarning("⚠️ Nivel " + (nivel + 1) + ": No se encontraron botones en el panel");
                    botonesAnimales[nivel] = new Button[0];
                    seleccionados[nivel] = new bool[0];
                    marcadoresSeleccionado[nivel] = new Image[0];
                    imagenesBoton[nivel] = new Image[0];
                    coloresOriginales[nivel] = new Color[0];
                    nombresAnimales[nivel] = new string[0];
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Panel del Nivel " + (nivel + 1) + " no asignado en el Inspector");
                botonesAnimales[nivel] = new Button[0];
                seleccionados[nivel] = new bool[0];
                marcadoresSeleccionado[nivel] = new Image[0];
                imagenesBoton[nivel] = new Image[0];
                coloresOriginales[nivel] = new Color[0];
                nombresAnimales[nivel] = new string[0];
            }
        }
    }

    void BuscarComponentes()
    {
        // Asignar nombres por defecto para cada nivel si no están asignados
        if (nombresAnimales[0] == null || nombresAnimales[0].Length == 0)
            nombresAnimales[0] = new string[] { "Perro", "Pájaro", "León", "Pingüino" };
        
        if (nombresAnimales[1] == null || nombresAnimales[1].Length == 0)
            nombresAnimales[1] = new string[] { "Serpiente", "Águila", "Vaca", "Perro", "Tortuga" };
        
        if (nombresAnimales[2] == null || nombresAnimales[2].Length == 0)
            nombresAnimales[2] = new string[] { "Rana", "Vaca", "León", "Perro", "Delfín", "Cocodrilo" };
        
        // Buscar marcadores "Seleccionado" e imágenes en cada botón de cada nivel
        for (int nivel = 0; nivel < 3; nivel++)
        {
            if (botonesAnimales[nivel] != null && botonesAnimales[nivel].Length > 0)
            {
                for (int i = 0; i < botonesAnimales[nivel].Length; i++)
                {
                    if (botonesAnimales[nivel][i] != null)
                    {
                        // Buscar marcador "Seleccionado"
                        Transform marcador = botonesAnimales[nivel][i].transform.Find("Seleccionado");
                        if (marcador != null)
                        {
                            marcadoresSeleccionado[nivel][i] = marcador.GetComponent<Image>();
                            if (marcadoresSeleccionado[nivel][i] != null)
                                marcadoresSeleccionado[nivel][i].enabled = false; // Oculto por defecto
                        }

                        // Buscar y guardar la imagen del botón y su color original
                        imagenesBoton[nivel][i] = botonesAnimales[nivel][i].GetComponent<Image>();
                        if (imagenesBoton[nivel][i] != null)
                            coloresOriginales[nivel][i] = imagenesBoton[nivel][i].color;
                        
                        Debug.Log("Nivel " + (nivel + 1) + " Botón " + i + ": " + (nombresAnimales[nivel] != null && i < nombresAnimales[nivel].Length ? nombresAnimales[nivel][i] : "Sin nombre"));
                    }
                }
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontraron botones en Nivel " + (nivel + 1));
            }
        }
    }

    void ConfigurarBotones()
    {
        // Botones de animales por nivel
        for (int nivel = 0; nivel < 3; nivel++)
        {
            if (botonesAnimales[nivel] != null)
            {
                for (int i = 0; i < botonesAnimales[nivel].Length; i++)
                {
                    int nivelRef = nivel;
                    int indiceRef = i;
                    if (botonesAnimales[nivel][i] != null)
                        botonesAnimales[nivel][i].onClick.AddListener(() => SeleccionarAnimal(nivelRef, indiceRef));
                }
            }
        }

        // Botón Validar
        if (btnValidar != null)
            btnValidar.onClick.AddListener(ValidarRespuesta);
    }

    void SeleccionarAnimal(int nivel, int indice)
    {
        if (botonesAnimales[nivel] == null || indice >= botonesAnimales[nivel].Length)
        {
            Debug.LogError("❌ Índice inválido en Nivel " + (nivel + 1));
            return;
        }
        
        seleccionados[nivel][indice] = !seleccionados[nivel][indice];
        
        // Cambiar color del botón
        if (imagenesBoton[nivel][indice] != null)
        {
            if (seleccionados[nivel][indice])
                imagenesBoton[nivel][indice].color = new Color(0.7f, 1f, 0.7f); // Verde claro cuando está seleccionado
            else
                imagenesBoton[nivel][indice].color = coloresOriginales[nivel][indice]; // Volver al color original
        }
        
        // Mostrar/ocultar marcador
        if (marcadoresSeleccionado[nivel][indice] != null)
            marcadoresSeleccionado[nivel][indice].enabled = seleccionados[nivel][indice];
        
        string nombreAnimal = (indice < nombresAnimales[nivel].Length) ? nombresAnimales[nivel][indice] : "Desconocido";
        Debug.Log((seleccionados[nivel][indice] ? "✓" : "✗") + " " + nombreAnimal);
    }

    void ValidarRespuesta()
    {
        int nivelIndex = nivelActual - 1; // Convertir a índice 0-based
        
        if (botonesAnimales[nivelIndex] == null || botonesAnimales[nivelIndex].Length == 0)
        {
            Debug.LogError("❌ No hay botones en Nivel " + nivelActual);
            return;
        }
        
        // Recopilar los índices seleccionados del nivel actual
        System.Collections.Generic.List<int> seleccionadosList = new System.Collections.Generic.List<int>();
        for (int i = 0; i < seleccionados[nivelIndex].Length; i++)
        {
            if (seleccionados[nivelIndex][i])
                seleccionadosList.Add(i);
        }

        // Print de lo que ha seleccionado
        string seleccionadosStr = "";
        for (int i = 0; i < seleccionadosList.Count; i++)
        {
            if (i > 0) seleccionadosStr += ", ";
            if (seleccionadosList[i] < nombresAnimales[nivelIndex].Length)
                seleccionadosStr += nombresAnimales[nivelIndex][seleccionadosList[i]];
        }
        Debug.Log("📌 Animales seleccionados: " + (seleccionadosStr != "" ? seleccionadosStr : "Ninguno"));

        // Obtener la respuesta correcta según el nivel
        int[] respuestaCorrecta = null;
        if (nivelActual == 1 && preguntaActual < respuestasNivel1.Length)
            respuestaCorrecta = respuestasNivel1[preguntaActual];
        else if (nivelActual == 2 && preguntaActual < respuestasNivel2.Length)
            respuestaCorrecta = respuestasNivel2[preguntaActual];
        else if (nivelActual == 3 && preguntaActual < respuestasNivel3.Length)
            respuestaCorrecta = respuestasNivel3[preguntaActual];
        
        if (respuestaCorrecta == null)
            return;

        // Print de la respuesta correcta esperada
        string respuestaCorrectaStr = "";
        for (int i = 0; i < respuestaCorrecta.Length; i++)
        {
            if (i > 0) respuestaCorrectaStr += ", ";
            if (respuestaCorrecta[i] < nombresAnimales[nivelIndex].Length)
                respuestaCorrectaStr += nombresAnimales[nivelIndex][respuestaCorrecta[i]];
        }
        Debug.Log("✔️ Respuesta correcta esperada: " + respuestaCorrectaStr);

        // Verificar si la selección es correcta
        bool esCorrect = seleccionadosList.Count == respuestaCorrecta.Length;
        if (esCorrect)
        {
            for (int i = 0; i < respuestaCorrecta.Length; i++)
            {
                if (!seleccionadosList.Contains(respuestaCorrecta[i]))
                {
                    esCorrect = false;
                    break;
                }
            }
        }

        if (esCorrect)
        {
            Debug.Log("✅ ¡Respuesta correcta!");
            PasarAlSiguienteNivel();
        }
        else
        {
            Debug.Log("❌ Respuesta incorrecta");
            contadorFallos[nivelIndex]++;
            LimpiarSeleccion();
        }
    }

    void PasarAlSiguienteNivel()
    {
        if (nivelActual < 3)
        {
            nivelActual++;
            preguntaActual = 0;
            LimpiarSeleccion();
            CambiarNivel(nivelActual);
        }
        else
        {
            // Juego completado
            Debug.Log("🎉 ¡JUEGO COMPLETADO!");
            Invoke("MostrarPopUpGanador", 0.3f);
        }
    }

    void LimpiarSeleccion()
    {
        int nivelIndex = nivelActual - 1;
        if (botonesAnimales[nivelIndex] != null && botonesAnimales[nivelIndex].Length > 0)
        {
            for (int i = 0; i < seleccionados[nivelIndex].Length; i++)
            {
                seleccionados[nivelIndex][i] = false;
                
                // Restaurar color original
                if (imagenesBoton[nivelIndex][i] != null)
                    imagenesBoton[nivelIndex][i].color = coloresOriginales[nivelIndex][i];
                
                // Ocultar marcador
                if (marcadoresSeleccionado[nivelIndex][i] != null)
                    marcadoresSeleccionado[nivelIndex][i].enabled = false;
            }
        }
    }

    void CambiarNivel(int nivel)
    {
        // Ocultar todos los paneles
        for (int i = 0; i < 3; i++)
        {
            if (panelesNiveles[i] != null)
                panelesNiveles[i].SetActive(false);
        }

        nivelActual = nivel;

        // Mostrar panel del nivel actual
        if (nivel > 0 && nivel <= 3 && panelesNiveles[nivel - 1] != null)
            panelesNiveles[nivel - 1].SetActive(true);
        else
            Debug.LogError("❌ Panel para Nivel " + nivel + " no existe");

        // Actualizar textos
        if (textoNivel != null)
            textoNivel.text = "Nivel " + nivel;
        else
            Debug.LogWarning("⚠️ textoNivel no asignado");

        MostrarPreguntaActual();
    }

    void MostrarPreguntaActual()
    {
        if (textoPregunta == null)
        {
            Debug.LogWarning("⚠️ textoPregunta no asignado");
            return;
        }
        
        if (nivelActual == 1 && preguntasNivel1.Length > 0)
        {
            if (preguntaActual < preguntasNivel1.Length)
                textoPregunta.text = preguntasNivel1[preguntaActual];
            else
                textoPregunta.text = "Pregunta no encontrada";
        }
        else if (nivelActual == 2 && preguntasNivel2.Length > 0)
        {
            if (preguntaActual < preguntasNivel2.Length)
                textoPregunta.text = preguntasNivel2[preguntaActual];
            else
                textoPregunta.text = "Pregunta no encontrada";
        }
        else if (nivelActual == 3 && preguntasNivel3.Length > 0)
        {
            if (preguntaActual < preguntasNivel3.Length)
                textoPregunta.text = preguntasNivel3[preguntaActual];
            else
                textoPregunta.text = "Pregunta no encontrada";
        }
        else
        {
            textoPregunta.text = "Nivel " + nivelActual + " no implementado";
        }
    }

    void MostrarPopUpGanador()
    {
        if (popUpGanar == null)
        {
            Debug.LogError("❌ PopUpGanar no asignado");
            return;
        }

        popUpGanar.SetNombreJuego("Animales");
        int fallosTotales = contadorFallos[0] + contadorFallos[1] + contadorFallos[2];
        string mensaje = "";

        if (fallosTotales == 0)
            mensaje = "¡Felicidades! ¡Perfecto sin errores!";
        else if (fallosTotales <= 3)
            mensaje = "¡Muy bien! Solo " + fallosTotales + " error/es";
        else
            mensaje = "¡Completado! Cometiste " + fallosTotales + " errores";

        popUpGanar.MostrarPopUpGanado(fallosTotales, mensaje);

        if (totalStarsCounter != null)
        {
            int estrellas = (fallosTotales == 0) ? 3 : (fallosTotales <= 3) ? 2 : 1;
            totalStarsCounter.AgregarEstrellas(estrellas, "Animales");
        }
        else
        {
            Debug.LogWarning("⚠️ TotalStarsCounter no asignado");
        }
    }
}
