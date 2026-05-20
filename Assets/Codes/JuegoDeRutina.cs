using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class JuegoDeRutina : MonoBehaviour
{
    public int nivelActual = 1;
    public TextMeshProUGUI Pregunta;
    public TextMeshProUGUI nivelTexto; 
    public Button[][] botonesPoner = new Button[3][]; /*3 niveles x 5 botones cada uno*/
    public Button[][] botonesPuesto = new Button[3][]; /*3 niveles con 3, 4 y 5 botones respectivamente*/
    public GameObject[] panelesNiveles = new GameObject[3]; // Paneles para cada nivel (Nivel 1, 2, 3)
    public PopUpGanar popUpGanar; // PopUp que se muestra cuando se completa el juego
    public TotalStarsCounter totalStarsCounter; // Para guardar estrellas
    
    // Trackear cuál botón de "Poner" está en cada posición de "Puesto" (-1 = vacío)
    private int[][] seleccionesActuales = new int[3][]; // Ej: nivel 1: [-1, -1, -1]
    
    // Contador de fallos
    private int[] contadorFallos = new int[3]; // Para los 3 niveles
    
    // Control de Invoke
    private bool popUpPendiente = false;
    
    // Cache de componentes Image para optimizar performance
    private Image[][] imagenesPonerCache = new Image[3][]; // Cache de componentes Image de botonesPoner
    private Image[][] imageniesPuestoCache = new Image[3][]; // Cache de componentes Image de botonesPuesto
    
    // Guardar las imágenes originales de los botones de respuesta
    private Sprite[][] imagenesOriginalesRespuestas = new Sprite[3][];
    
    // Definir las órdenes correctas para cada nivel (puede haber múltiples respuestas válidas)
    private int[][][] ordenesCorrectas = new int[][][]
    {
        // Nivel 1: Una respuesta correcta
        new int[][]
        {
            new int[] { 3, 4, 2 }
        },
        // Nivel 2: Dos respuestas correctas
        new int[][]
        {
            new int[] { 4, 1, 0, 3 },  // Preparar → Duchar → Cepillar → Dormir
            new int[] { 1, 0, 4, 3 }   // Duchar → Cepillar → Preparar → Dormir
        },
        // Nivel 3: Dos respuestas correctas
        new int[][]
        {
            new int[] { 3, 4, 0, 1, 2 },  // Despertar → Desayunar → Cepillar → Cambiar ropa → Salir
            new int[] { 3, 4, 1, 0, 2 }   // Despertar → Desayunar → Cambiar ropa → Cepillar → Salir
        }
    };
    
    // Preguntas de cada nivel
    private string[] textosPreguntas = new string[]
    {
        "¿Cuál es el orden correcto de las cosas que se debe hacer por la mañana?",
        "¿Cuál es el orden correcto de las cosas que se debe hacer por la noche?", 
        "¿Cuál es el orden completo de las cosas que se debe hacer por la mañana?"
    };
    
    void Start()
    {
        InicializarArrays();
        BuscarBotonesAutomaticamente();
        ActualizarPaneles();
        ConfigurarBotonesNivel1();
    }
    
    void InicializarArrays()
    {
        // Inicializar botonesPoner: 3 niveles x 5 botones cada uno
        botonesPoner = new Button[3][];
        botonesPoner[0] = new Button[5]; // Nivel 1: 5 botones para elegir
        botonesPoner[1] = new Button[5]; // Nivel 2: 5 botones para elegir
        botonesPoner[2] = new Button[5]; // Nivel 3: 5 botones para elegir
        
        // Inicializar botonesPuesto: 3 niveles con diferente cantidad de espacios
        botonesPuesto = new Button[3][];
        botonesPuesto[0] = new Button[3]; // Nivel 1: 3 espacios (solo 3 correctos)
        botonesPuesto[1] = new Button[4]; // Nivel 2: 4 espacios
        botonesPuesto[2] = new Button[5]; // Nivel 3: 5 espacios
        
        // Inicializar selecciones actuales (qué botón está en cada posición de puesto)
        seleccionesActuales = new int[3][];
        seleccionesActuales[0] = new int[] { -1, -1, -1 }; // Nivel 1
        seleccionesActuales[1] = new int[] { -1, -1, -1, -1 }; // Nivel 2
        seleccionesActuales[2] = new int[] { -1, -1, -1, -1, -1 }; // Nivel 3
        
        // Inicializar imágenes originales
        imagenesOriginalesRespuestas = new Sprite[3][];
        imagenesOriginalesRespuestas[0] = new Sprite[3]; // Nivel 1
        imagenesOriginalesRespuestas[1] = new Sprite[4]; // Nivel 2
        imagenesOriginalesRespuestas[2] = new Sprite[5]; // Nivel 3
        
        // Inicializar caches de componentes Image
        imagenesPonerCache = new Image[3][];
        imagenesPonerCache[0] = new Image[5];
        imagenesPonerCache[1] = new Image[5];
        imagenesPonerCache[2] = new Image[5];
        
        imageniesPuestoCache = new Image[3][];
        imageniesPuestoCache[0] = new Image[3]; // Nivel 1
        imageniesPuestoCache[1] = new Image[4]; // Nivel 2
        imageniesPuestoCache[2] = new Image[5]; // Nivel 3
        
        // Inicializar contador de fallos
        contadorFallos = new int[3];
        contadorFallos[0] = 0;
        contadorFallos[1] = 0;
        contadorFallos[2] = 0;
    }

    void BuscarBotonesAutomaticamente()
    {
        // Buscar botones en TODOS los niveles - OPTIMIZADO
        for (int nivel = 0; nivel < 3; nivel++)
        {
            if (panelesNiveles[nivel] == null)
            {
                Debug.LogError("Panel del Nivel " + (nivel + 1) + " no está asignado");
                continue;
            }

            // Cache del Transform del panel para evitar búsquedas repetidas
            Transform panelTransform = panelesNiveles[nivel].transform;

            // Buscar botones de "Poner" (siempre 5 por nivel)
            for (int i = 0; i < 5; i++)
            {
                Transform boton = panelTransform.Find("Button" + i);
                if (boton != null)
                {
                    Button btnComponent = boton.GetComponent<Button>();
                    if (btnComponent != null)
                    {
                        botonesPoner[nivel][i] = btnComponent;
                        // Cache el componente Image
                        imagenesPonerCache[nivel][i] = boton.GetComponent<Image>();
                    }
                }
            }

            // Buscar botones de "Puesto" (cantidad variable por nivel)
            int cantidadRespuestas = (nivel == 0) ? 3 : (nivel == 1) ? 4 : 5;
            for (int i = 0; i < cantidadRespuestas; i++)
            {
                Transform respuesta = panelTransform.Find("Respuesta" + i);
                if (respuesta != null)
                {
                    Button btnComponent = respuesta.GetComponent<Button>();
                    if (btnComponent != null)
                    {
                        botonesPuesto[nivel][i] = btnComponent;
                        
                        // Cache el componente Image
                        Image imagenRespuesta = respuesta.GetComponent<Image>();
                        imageniesPuestoCache[nivel][i] = imagenRespuesta;

                        // Guardar la imagen original
                        if (imagenRespuesta != null)
                        {
                            imagenesOriginalesRespuestas[nivel][i] = imagenRespuesta.sprite;
                        }
                    }
                }
            }
        }
    }

    void ActualizarPaneles()
    {
        // Activar solo el panel del nivel actual, desactivar los otros
        for (int i = 0; i < panelesNiveles.Length; i++)
        {
            if (panelesNiveles[i] != null)
            {
                panelesNiveles[i].SetActive(i == nivelActual - 1);
            }
        }
        
        // Actualizar el texto del nivel y la pregunta
        if (nivelTexto != null)
        {
            nivelTexto.text = "Nivel " + nivelActual;
        }
        
        if (Pregunta != null)
        {
            Pregunta.text = textosPreguntas[nivelActual - 1];
        }
    }
    
    void CambiarNivel(int nuevoNivel)
    {
        nivelActual = nuevoNivel;
        ActualizarPaneles();
        
        // Configurar los botones del nuevo nivel
        if (nivelActual == 1)
        {
            ConfigurarBotonesNivel1();
        }
        else if (nivelActual == 2)
        {
            ConfigurarBotonesNivel2();
        }
        else if (nivelActual == 3)
        {
            ConfigurarBotonesNivel3();
        }
    }

    void ConfigurarBotonesNivel1()
    {
        // Limpiar listeners anteriores de botonesPoner
        for (int i = 0; i < 5; i++)
        {
            botonesPoner[0][i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para los 5 botones del Nivel 1
        for (int i = 0; i < 5; i++)
        {
            int indice = i;
            botonesPoner[0][i].onClick.AddListener(() => ClickEnBotonPonerNivel1(indice));
        }

        // Limpiar listeners anteriores de botonesPuesto
        for (int i = 0; i < 3; i++)
        {
            botonesPuesto[0][i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para los 3 botones de respuesta del Nivel 1
        for (int i = 0; i < 3; i++)
        {
            int posicion = i;
            botonesPuesto[0][i].onClick.AddListener(() => ClickEnBotonPuestoNivel1(posicion));
        }
    }

    void ClickEnBotonPuestoNivel1(int posicion)
    {
        // Quitar solo la imagen en esa posición
        if (seleccionesActuales[0][posicion] != -1)
        {
            LimpiarPosicion(0, posicion);
            Debug.Log("Imagen en Respuesta " + posicion + " fue eliminada");
        }
        else
        {
            Debug.Log("No hay nada en la posición " + posicion);
        }
    }

    void LimpiarPosicion(int nivel, int posicion)
    {
        // Restaurar la imagen original del botón de "Puesto"
        Image imagenDestino = botonesPuesto[nivel][posicion].GetComponent<Image>();
        if (imagenDestino != null)
        {
            // Restaurar la imagen original que se guardó al principio
            imagenDestino.sprite = imagenesOriginalesRespuestas[nivel][posicion];
            imagenDestino.enabled = true;
        }

        // Marcar como vacío
        seleccionesActuales[nivel][posicion] = -1;
    }

    void ClickEnBotonPonerNivel1(int indiceBotonPoner)
    {
        // Verificar si este botón ya está seleccionado
        int posicionEnPuesto = -1;
        for (int i = 0; i < seleccionesActuales[0].Length; i++)
        {
            if (seleccionesActuales[0][i] == indiceBotonPoner)
            {
                posicionEnPuesto = i;
                break;
            }
        }

        if (posicionEnPuesto != -1)
        {
            // El botón ya está seleccionado, lo removemos
            QuitarSeleccion(0, posicionEnPuesto);
            Debug.Log("Botón " + indiceBotonPoner + " removido de la posición " + posicionEnPuesto);
        }
        else
        {
            // El botón no está seleccionado, lo añadimos en el siguiente espacio libre
            int siguienteLivre = -1;
            for (int i = 0; i < seleccionesActuales[0].Length; i++)
            {
                if (seleccionesActuales[0][i] == -1)
                {
                    siguienteLivre = i;
                    break;
                }
            }

            if (siguienteLivre != -1)
            {
                AñadirSeleccion(0, siguienteLivre, indiceBotonPoner);
                Debug.Log("Botón " + indiceBotonPoner + " añadido en la posición " + siguienteLivre);
            }
            else
            {
                Debug.Log("No hay más espacios disponibles en Nivel 1");
            }
        }
    }

    void AñadirSeleccion(int nivel, int posicion, int indiceBotonPoner)
    {
        // Guardar qué botón está en esta posición
        seleccionesActuales[nivel][posicion] = indiceBotonPoner;

        // Obtener la imagen del botón de "Poner" desde el cache
        Image imagenOrigen = imagenesPonerCache[nivel][indiceBotonPoner];

        if (imagenOrigen != null)
        {
            // Asignar la imagen al botón de "Puesto" desde el cache
            Image imagenDestino = imageniesPuestoCache[nivel][posicion];
            if (imagenDestino != null)
            {
                imagenDestino.sprite = imagenOrigen.sprite;
                imagenDestino.enabled = true;
            }
        }

        // Verificar si el nivel está completamente lleno
        bool nivelLleno = true;
        for (int i = 0; i < seleccionesActuales[nivel].Length; i++)
        {
            if (seleccionesActuales[nivel][i] == -1)
            {
                nivelLleno = false;
                break;
            }
        }

        if (nivelLleno)
        {
            // Verificar si es el orden correcto
            VerificarNivel(nivel);
        }
    }

    void QuitarSeleccion(int nivel, int posicion)
    {
        // Limpiar la imagen del botón de "Puesto" usando cache
        Image imagenDestino = imageniesPuestoCache[nivel][posicion];
        if (imagenDestino != null)
        {
            imagenDestino.sprite = null;
            imagenDestino.enabled = false;
        }

        // Desplazar las selecciones posteriores hacia atrás
        for (int i = posicion; i < seleccionesActuales[nivel].Length - 1; i++)
        {
            seleccionesActuales[nivel][i] = seleccionesActuales[nivel][i + 1];

            if (seleccionesActuales[nivel][i] != -1)
            {
                // Mover la imagen hacia la posición anterior
                int indiceBoton = seleccionesActuales[nivel][i];
                Image imagenOrigen = imagenesPonerCache[nivel][indiceBoton];
                Image imagenDest = imageniesPuestoCache[nivel][i];
                
                if (imagenOrigen != null && imagenDest != null)
                {
                    imagenDest.sprite = imagenOrigen.sprite;
                }
            }
            else
            {
                // Limpiar la imagen si no hay nada
                Image imagenDest = imageniesPuestoCache[nivel][i];
                if (imagenDest != null)
                {
                    imagenDest.sprite = null;
                    imagenDest.enabled = false;
                }
            }
        }

        // Limpiar la última posición
        seleccionesActuales[nivel][seleccionesActuales[nivel].Length - 1] = -1;
        Image imagenUltima = imageniesPuestoCache[nivel][seleccionesActuales[nivel].Length - 1];
        if (imagenUltima != null)
        {
            imagenUltima.sprite = null;
            imagenUltima.enabled = false;
        }
    }

    void VerificarNivel(int nivel)
    {
        // Verificar si las selecciones actuales coinciden con alguna de las respuestas válidas
        bool esCorrect = false;

        for (int respuestaIdx = 0; respuestaIdx < ordenesCorrectas[nivel].Length; respuestaIdx++)
        {
            int[] respuestaValida = ordenesCorrectas[nivel][respuestaIdx];
            bool coincide = true;

            for (int i = 0; i < respuestaValida.Length; i++)
            {
                if (seleccionesActuales[nivel][i] != respuestaValida[i])
                {
                    coincide = false;
                    break;
                }
            }

            if (coincide)
            {
                esCorrect = true;
                break;
            }
        }

        if (esCorrect)
        {
            
            // Cambiar al siguiente nivel
            if (nivel + 1 < 3)
            {
                CambiarNivel(nivel + 2); // nivel + 2 porque nivel es 0-indexed pero nivelActual es 1-indexed
            }
            else
            {
                // Mostrar PopUpGanar
                if (!popUpPendiente)
                {
                    popUpPendiente = true;
                    Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
                }
            }
        }
        else
        {
            contadorFallos[nivel]++;
            ResetearNivel(nivel);
        }
    }

    void ResetearNivel(int nivel)
    {
        // Limpiar todas las selecciones del nivel
        for (int i = 0; i < seleccionesActuales[nivel].Length; i++)
        {
            seleccionesActuales[nivel][i] = -1;
            
            // Restaurar la imagen original usando cache
            Image imagenDestino = imageniesPuestoCache[nivel][i];
            if (imagenDestino != null)
            {
                imagenDestino.sprite = imagenesOriginalesRespuestas[nivel][i];
                imagenDestino.enabled = true;
            }
        }
    }

    void MostrarPopUpGanadoConRetraso()
    {
        if (popUpGanar == null)
        {
            Debug.LogError("❌ PopUpGanar no está asignado en el inspector");
            return;
        }
        
        popUpGanar.SetNombreJuego("Rutina");
        string mensajeGanado = "";
        
        // Calcular fallos totales
        int fallosTotales = contadorFallos[0] + contadorFallos[1] + contadorFallos[2];
        
        if (fallosTotales == 0)
        {
            mensajeGanado = "¡Felicidades! ¡Lo hiciste perfecto sin errores!";
        }
        else if (fallosTotales <= 2)
        {
            mensajeGanado = "¡Muy bien! Cometiste solo algunos errores. ¡Sigue practicando!";
        }
        else
        {
            mensajeGanado = "¡Lo intentaste! Prueba nuevamente para mejorar.";
        }
        
        popUpGanar.MostrarPopUpGanado(fallosTotales, mensajeGanado);
    }

    // ==================== NIVEL 2 ====================
    void ConfigurarBotonesNivel2()
    {
        // Limpiar listeners anteriores de botonesPoner
        for (int i = 0; i < 5; i++)
        {
            botonesPoner[1][i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para los 5 botones del Nivel 2
        for (int i = 0; i < 5; i++)
        {
            int indice = i;
            botonesPoner[1][i].onClick.AddListener(() => ClickEnBotonPonerNivel2(indice));
        }

        // Limpiar listeners anteriores de botonesPuesto
        for (int i = 0; i < 4; i++)
        {
            botonesPuesto[1][i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para los 4 botones de respuesta del Nivel 2
        for (int i = 0; i < 4; i++)
        {
            int posicion = i;
            botonesPuesto[1][i].onClick.AddListener(() => ClickEnBotonPuestoNivel2(posicion));
        }
    }

    void ClickEnBotonPonerNivel2(int indiceBotonPoner)
    {
        // Verificar si este botón ya está seleccionado
        int posicionEnPuesto = -1;
        for (int i = 0; i < seleccionesActuales[1].Length; i++)
        {
            if (seleccionesActuales[1][i] == indiceBotonPoner)
            {
                posicionEnPuesto = i;
                break;
            }
        }

        if (posicionEnPuesto != -1)
        {
            // El botón ya está seleccionado, lo removemos
            QuitarSeleccion(1, posicionEnPuesto);
            Debug.Log("Botón " + indiceBotonPoner + " removido de la posición " + posicionEnPuesto);
        }
        else
        {
            // El botón no está seleccionado, lo añadimos en el siguiente espacio libre
            int siguienteLivre = -1;
            for (int i = 0; i < seleccionesActuales[1].Length; i++)
            {
                if (seleccionesActuales[1][i] == -1)
                {
                    siguienteLivre = i;
                    break;
                }
            }

            if (siguienteLivre != -1)
            {
                AñadirSeleccion(1, siguienteLivre, indiceBotonPoner);
                Debug.Log("Botón " + indiceBotonPoner + " añadido en la posición " + siguienteLivre);
            }
            else
            {
                Debug.Log("No hay más espacios disponibles en Nivel 2");
            }
        }
    }

    void ClickEnBotonPuestoNivel2(int posicion)
    {
        // Quitar solo la imagen en esa posición
        if (seleccionesActuales[1][posicion] != -1)
        {
            LimpiarPosicion(1, posicion);
            Debug.Log("Imagen en Respuesta " + posicion + " fue eliminada");
        }
        else
        {
            Debug.Log("No hay nada en la posición " + posicion);
        }
    }

    // ==================== NIVEL 3 ====================
    void ConfigurarBotonesNivel3()
    {
        // Limpiar listeners anteriores de botonesPoner
        for (int i = 0; i < 5; i++)
        {
            botonesPoner[2][i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para los 5 botones del Nivel 3
        for (int i = 0; i < 5; i++)
        {
            int indice = i;
            botonesPoner[2][i].onClick.AddListener(() => ClickEnBotonPonerNivel3(indice));
        }

        // Limpiar listeners anteriores de botonesPuesto
        for (int i = 0; i < 5; i++)
        {
            botonesPuesto[2][i].onClick.RemoveAllListeners();
        }
        
        // Configurar listeners para los 5 botones de respuesta del Nivel 3
        for (int i = 0; i < 5; i++)
        {
            int posicion = i;
            botonesPuesto[2][i].onClick.AddListener(() => ClickEnBotonPuestoNivel3(posicion));
        }
    }

    void ClickEnBotonPonerNivel3(int indiceBotonPoner)
    {
        // Verificar si este botón ya está seleccionado
        int posicionEnPuesto = -1;
        for (int i = 0; i < seleccionesActuales[2].Length; i++)
        {
            if (seleccionesActuales[2][i] == indiceBotonPoner)
            {
                posicionEnPuesto = i;
                break;
            }
        }

        if (posicionEnPuesto != -1)
        {
            // El botón ya está seleccionado, lo removemos
            QuitarSeleccion(2, posicionEnPuesto);
            Debug.Log("Botón " + indiceBotonPoner + " removido de la posición " + posicionEnPuesto);
        }
        else
        {
            // El botón no está seleccionado, lo añadimos en el siguiente espacio libre
            int siguienteLivre = -1;
            for (int i = 0; i < seleccionesActuales[2].Length; i++)
            {
                if (seleccionesActuales[2][i] == -1)
                {
                    siguienteLivre = i;
                    break;
                }
            }

            if (siguienteLivre != -1)
            {
                AñadirSeleccion(2, siguienteLivre, indiceBotonPoner);
                Debug.Log("Botón " + indiceBotonPoner + " añadido en la posición " + siguienteLivre);
            }
            else
            {
                Debug.Log("No hay más espacios disponibles en Nivel 3");
            }
        }
    }

    void ClickEnBotonPuestoNivel3(int posicion)
    {
        // Quitar solo la imagen en esa posición
        if (seleccionesActuales[2][posicion] != -1)
        {
            LimpiarPosicion(2, posicion);
            Debug.Log("Imagen en Respuesta " + posicion + " fue eliminada");
        }
        else
        {
            Debug.Log("No hay nada en la posición " + posicion);
        }
    }
}
