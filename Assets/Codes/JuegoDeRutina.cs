using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
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
    public GameObject imagenBien, imagenMal; // Imágenes para feedback de respuestas correctas/incorrectas
    private bool bloqueado = false;
    // Trackear cuál botón de "Poner" está en cada posición de "Puesto" (-1 = vacío)
    private int[][] seleccionesActuales = new int[3][]; // Ej: nivel 1: [-1, -1, -1]
    private int restartCounter = 0; // Contador de reinicios para guardar estrellas
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
        ConfigurarBotonesNivel(0);
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
        ConfigurarBotonesNivel(nivelActual - 1);
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
            // Cambiamos la llamada normal por la Corrutina
            StartCoroutine(RutinaVerificarNivel(nivel));
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
            restartCounter++;
            ResetearNivel(nivel);
            if(restartCounter == 3)
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
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
void ConfigurarBotonesNivel(int nivelIndex)
    {
        // 1. Limpiar y configurar botones Poner (siempre son 5)
        for (int i = 0; i < 5; i++)
        {
            botonesPoner[nivelIndex][i].onClick.RemoveAllListeners();
            int indice = i;
            // Le pasamos el nivelIndex además del botón pulsado
            botonesPoner[nivelIndex][i].onClick.AddListener(() => ClickEnBotonPoner(nivelIndex, indice));
        }

        // 2. Limpiar y configurar botones Puesto (la longitud se detecta sola)
        int cantidadPuestos = botonesPuesto[nivelIndex].Length;
        for (int i = 0; i < cantidadPuestos; i++)
        {
            botonesPuesto[nivelIndex][i].onClick.RemoveAllListeners();
            int posicion = i;
            // Le pasamos el nivelIndex además de la posición
            botonesPuesto[nivelIndex][i].onClick.AddListener(() => ClickEnBotonPuesto(nivelIndex, posicion));
        }
    }

    void ClickEnBotonPuesto(int nivelIndex, int posicion)
    {
        if (bloqueado) return; 
        
        // Quitar solo la imagen en esa posición usando el nivel correcto
        if (seleccionesActuales[nivelIndex][posicion] != -1)
        {
            LimpiarPosicion(nivelIndex, posicion);
            Debug.Log("Imagen en Respuesta " + posicion + " del Nivel " + (nivelIndex + 1) + " fue eliminada");
        }
        else
        {
            Debug.Log("No hay nada en la posición " + posicion);
        }
    }

    void ClickEnBotonPoner(int nivelIndex, int indiceBotonPoner)
    {
        if(bloqueado) return; 

        // Verificar si este botón ya está seleccionado en el nivel actual
        int posicionEnPuesto = -1;
        for (int i = 0; i < seleccionesActuales[nivelIndex].Length; i++)
        {
            if (seleccionesActuales[nivelIndex][i] == indiceBotonPoner)
            {
                posicionEnPuesto = i;
                break;
            }
        }

        if (posicionEnPuesto != -1)
        {
            Debug.Log("No vale repetir la misma acción, botón " + indiceBotonPoner + " ya está en la posición " + posicionEnPuesto);
        }
        else
        {
            // El botón no está seleccionado, lo añadimos en el siguiente espacio libre
            int siguienteLivre = -1;
            for (int i = 0; i < seleccionesActuales[nivelIndex].Length; i++)
            {
                if (seleccionesActuales[nivelIndex][i] == -1)
                {
                    siguienteLivre = i;
                    break;
                }
            }

            if (siguienteLivre != -1)
            {
                AñadirSeleccion(nivelIndex, siguienteLivre, indiceBotonPoner);
                Debug.Log("Botón " + indiceBotonPoner + " añadido en la posición " + siguienteLivre);
            }
            else
            {
                Debug.Log("No hay más espacios disponibles en Nivel " + (nivelIndex + 1));
            }
        }
    }
    IEnumerator RutinaVerificarNivel(int nivel)
    {
        bloqueado = true; // 1. Cerramos el candado para que no toquen nada

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

        // EVALUACIÓN VISUAL
        if (esCorrect)
        {
            // Mostramos imagen de Bien durante 1 segundo
            yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenBien, 1f));
            
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
            // Mostramos imagen de Mal durante 1 segundo
            yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenMal, 1f));
            
            restartCounter++;
            ResetearNivel(nivel);
            
            if(restartCounter >= 3)
            {
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }
        }

        bloqueado = false; // 4. Abrimos el candado para que puedan volver a jugar
    }
    IEnumerator MostrarImagenTiempoLimitado(GameObject imagen, float duracion)
    {
        if (imagen == null) yield break;
        
        imagen.SetActive(true);
        yield return new WaitForSeconds(duracion);
        imagen.SetActive(false);
    }
    public void SetValor(bool valor)
    {
        if (valor)
        {
            nivelActual = 1;
            ActualizarPaneles();
            ConfigurarBotonesNivel(0);
            restartCounter = 0; 
            for (int i = 0; i < 3; i++)
            {
                ResetearNivel(i);
            }
        }
    }
}
