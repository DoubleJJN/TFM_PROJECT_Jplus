using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Escenas : MonoBehaviour
{
    public Button b1, b2, b3, b4, b5, b6, b7, b8, b9, b10;
    public GameObject popUpAntesJugar;
    
    private UserManager userManager;
    private string usuarioLogueado;
    private string usuarioAnterior = ""; // Para detectar cambios
    private float tiempoUltimaActualizacion = 0f;
    private const float INTERVALO_ACTUALIZACION = 0.5f; // Actualizar cada 0.5 segundos

    void Start()
    {
        // Obtener usuario logueado
        usuarioLogueado = PlayerPrefs.GetString("UsuarioLogueado", "");
        
        // Buscar UserManager en la escena o en cualquier lugar
        userManager = FindFirstObjectByType<UserManager>();
        
        // Configurar botones
        b1.onClick.AddListener(Button1);
        b2.onClick.AddListener(Button2);
        b3.onClick.AddListener(Button3);
        b4.onClick.AddListener(Button4);
        b5.onClick.AddListener(Button5);
        b6.onClick.AddListener(Button6);
        b7.onClick.AddListener(Button7);
        b8.onClick.AddListener(Button8);
        b9.onClick.AddListener(Button9);
        b10.onClick.AddListener(Button10);
        
        // Cargar datos frescos del archivo para asegurar que ve los cambios más recientes
        
        // Mostrar estrellas basadas en puntuaciones
        MostrarEstrellas();
    }
    
    void Update()
    {
        // Si aún no hemos encontrado UserManager, intenta nuevamente
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
        }
        
        // Obtener usuario actual
        usuarioLogueado = PlayerPrefs.GetString("UsuarioLogueado", "");
        
        // Si el usuario cambió, actualizar estrellas inmediatamente
        if (usuarioLogueado != usuarioAnterior)
        {
            usuarioAnterior = usuarioLogueado;
            MostrarEstrellas(); // Actualizar inmediatamente si cambió el usuario
        }
        
        // Actualizar estrellas cada X segundos (no cada frame)
        tiempoUltimaActualizacion += Time.deltaTime;
        if (tiempoUltimaActualizacion >= INTERVALO_ACTUALIZACION)
        {
            tiempoUltimaActualizacion = 0f;
            if (!string.IsNullOrEmpty(usuarioLogueado))
            {
                MostrarEstrellas();
            }
        }
    }
    
    private User ObtenerUsuarioActual()
    {
        if (string.IsNullOrEmpty(usuarioLogueado))
            return null;
        
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "users.json");
        string persistentPath = Path.Combine(Application.persistentDataPath, "users.json");
        
        // Intento 1: persistentDataPath (donde UserManager GUARDA en Editor)
        try
        {
            if (File.Exists(persistentPath))
            {
                string json = System.IO.File.ReadAllText(persistentPath, System.Text.Encoding.UTF8);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == usuarioLogueado)
                        {
                            return user;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("⚠️ Error al leer persistentDataPath: " + e.Message);
        }
        
        // Intento 2: StreamingAssets (fallback)
        try
        {
            if (File.Exists(streamingPath))
            {
                string json = System.IO.File.ReadAllText(streamingPath, System.Text.Encoding.UTF8);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == usuarioLogueado)
                        {
                            return user;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("⚠️ Error al leer persistentDataPath: " + e.Message);
        }
        
        // FALLBACK 3: UserManager
        if (userManager != null)
        {
            Debug.Log("🔍 Intentando UserManager");
            User userFromManager = userManager.GetUser(usuarioLogueado);
            if (userFromManager != null)
            {
                Debug.Log("✓ Usuario encontrado en UserManager - tresEnRayaScore=" + userFromManager.tresEnRayaScore + ", total=" + userFromManager.puntuacion);
                return userFromManager;
            }
        }
        
        Debug.LogWarning("⚠️ No se encontró usuario '" + usuarioLogueado + "' en ninguna fuente");
        return null;
    }
    
    private void MostrarEstrellas()
    {
        // Si no hay usuario logueado, no hacer nada
        if (string.IsNullOrEmpty(usuarioLogueado))
        {
            Debug.LogWarning("⚠️ MostrarEstrellas() - No hay usuario logueado en PlayerPrefs");
            return;
        }
        
        // En WebGL (compilado), necesitamos UserManager
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (userManager == null)
            return;
        #endif
        
        User usuarioActual = ObtenerUsuarioActual();
        
        if (usuarioActual == null)
        {
            Debug.LogError("❌ MostrarEstrellas() - No se pudo obtener datos del usuario: " + usuarioLogueado);
            return;
        }
        
        // Actualizar estrellas
        
        // Botón 1 - Ranas
        ActualizarEstrellas(b1.transform, usuarioActual.ranasScore);
        
        // Botón 2 - Mochila
        ActualizarEstrellas(b2.transform, usuarioActual.mochilaScore);
        
        // Botón 3 - Laberinto
        ActualizarEstrellas(b3.transform, usuarioActual.laberintoScore);
        
        // Botón 4 - Hardware/Software
        ActualizarEstrellas(b4.transform, usuarioActual.hardwareSoftwareScore);
        
        // Botón 5 - Bolas
        ActualizarEstrellas(b5.transform, usuarioActual.bolasScore);
        
        // Botón 6 - Reina/Ajedrez
        ActualizarEstrellas(b6.transform, usuarioActual.reinaScore);
        
        // Botón 7 - Tres en Raya
        ActualizarEstrellas(b7.transform, usuarioActual.tresEnRayaScore);

        // Botón 8 - Puzzle
        ActualizarEstrellas(b8.transform, usuarioActual.puzzleScore);
    }
    
    private void ActualizarEstrellas(Transform buttonTransform, int puntuacion)
    {
        // Buscar las estrellas dentro del botón
        Transform starWin1 = buttonTransform.Find("StarWin1");
        Transform starWin2 = buttonTransform.Find("StarWin2");
        Transform starWin3 = buttonTransform.Find("StarWin3");
        
        // Ocultar todas primero
        if (starWin1 != null) starWin1.gameObject.SetActive(false);
        if (starWin2 != null) starWin2.gameObject.SetActive(false);
        if (starWin3 != null) starWin3.gameObject.SetActive(false);
        
        // Mostrar según la puntuación
        if (puntuacion >= 1 && starWin1 != null)
            starWin1.gameObject.SetActive(true);
        
        if (puntuacion >= 2 && starWin2 != null)
            starWin2.gameObject.SetActive(true);
        
        if (puntuacion >= 3 && starWin3 != null)
            starWin3.gameObject.SetActive(true);
    }

    private bool UsuarioLogueado()
    {
        return PlayerPrefs.GetInt("EstaLogueado", 0) == 1;
    }

    private void IntentarCargarEscena(string escenaNombre)
    {
        if (UsuarioLogueado())
        {
            SceneManager.LoadScene(escenaNombre);
        }
        else
        {
            if (popUpAntesJugar != null)
            {
                popUpAntesJugar.SetActive(true);
            }
            else
            {
                Debug.LogWarning("⚠️ popUpAntesJugar no asignado en Escenas");
            }
        }
    }

    void Button1()
    {
        IntentarCargarEscena("GameRanas");
    }

    void Button2()
    {
        IntentarCargarEscena("GameMochila");
    }

    void Button3()
    {
        IntentarCargarEscena("GameLaberinto");
    }

    void Button4()
    {
        IntentarCargarEscena("GameAdivinarHardwareOrSoftware");
    }

    void Button5()
    {
        IntentarCargarEscena("GameOrdenar");
    }

    void Button6()
    {
        IntentarCargarEscena("GameAjedrez");
    }  

    void Button7()
    {
        Debug.Log("🎮 Botón 7 presionado - intentando cargar GameTresEnRaya");
        IntentarCargarEscena("GameTresEnRaya");
    }

    void Button8()
    {
        Debug.Log("🎮 Botón 8 presionado - intentando cargar GamePuzzle");
        IntentarCargarEscena("GamePuzzle");
    }

    void Button9()
    {
        Debug.Log("🎮 Botón 9 presionado - intentando cargar GameAnimales");
        IntentarCargarEscena("GameAnimales");
    }

    void Button10()
    {
        Debug.Log("🎮 Botón 10 presionado - intentando cargar GameRutina");
        IntentarCargarEscena("GameRutina");
    }
}
