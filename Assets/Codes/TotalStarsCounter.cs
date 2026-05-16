using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

public class TotalStarsCounter : MonoBehaviour
{
    public int puntuacionRana, puntuacionBolas, puntuacionMochila, puntuacionLaberinto, puntuacionHS, puntuacionReina, puntuacionTresEnRaya;
    public static int totalPuntacion;
    public static TotalStarsCounter instance;
    public TextMeshProUGUI totalPointsText;
    private UserManager userManager;
    private string usuarioLogueado;
    private string usersDatabasePath;
    private float tiempoUltimaActualizacion = 0f;
    private const float INTERVALO_ACTUALIZACION = 0.5f; // Actualizar cada 0.5 segundos

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject.transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("═══ INICIANDO TOTALSTARSCOUNTER ═══");
        
        // Inicializar ruta de la base de datos
        usersDatabasePath = Path.Combine(Application.streamingAssetsPath, "users.json");
        Debug.Log("📂 Ruta de database: " + usersDatabasePath);
        
        // Si totalPointsText no está asignado, intenta buscarlo
        if (totalPointsText == null)
        {
            GameObject totalPointsObj = GameObject.Find("TotalPoints");
            if (totalPointsObj != null)
            {
                totalPointsText = totalPointsObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ TotalPointsText encontrado automáticamente");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró objeto TotalPoints en la escena");
            }
        }
        else
        {
            Debug.Log("✓ TotalPointsText ya estaba asignado");
        }
        
        // Asegurar que totalPointsText se dibuje encima y sea visible
        if (totalPointsText != null)
        {
            totalPointsText.GetComponent<RectTransform>().SetAsLastSibling();
            Color textColor = totalPointsText.color;
            textColor.a = 1f;
            totalPointsText.color = textColor;
            Debug.Log("✓ TotalPointsText configurado (visible y encima)");
        }
        
        // Actualizar puntuación inicial
        ActualizarTotalPuntacion();
        
        Debug.Log("═══ TOTALSTARSCOUNTER LISTO ═══");
    }

    void Update()
    {
        // Si no tenemos UserManager, intentar encontrarlo
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
            if (userManager != null)
            {
                Debug.Log("✓ UserManager encontrado en Update()");
            }
        }
        
        // Actualizar puntuación cada X segundos (no cada frame)
        tiempoUltimaActualizacion += Time.deltaTime;
        if (tiempoUltimaActualizacion >= INTERVALO_ACTUALIZACION)
        {
            tiempoUltimaActualizacion = 0f;
            ActualizarTotalPuntacion();
        }
    }
    
    private void BuscarUserManager()
    {
        if (userManager == null)
        {
            // Buscar en la escena actual
            userManager = FindFirstObjectByType<UserManager>();
            
            if (userManager != null)
            {
                Debug.Log("✓ UserManager encontrado en Start()");
            }
            else
            {
                Debug.LogWarning("⚠️ UserManager NO encontrado en Start(). Se buscará luego cuando sea necesario");
            }
        }
    }

    public void AgregarEstrellas(int estrellas, string nombreJuego)
    {
        Debug.Log("🌟 AgregarEstrellas() iniciado - Juego: " + nombreJuego + ", Estrellas: " + estrellas);
        
        // Buscar UserManager si no lo tenemos
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
            Debug.Log("   UserManager encontrado: " + (userManager != null ? "✓" : "❌"));
        }
        
        if (userManager == null)
        {
            Debug.LogError("❌ UserManager no encontrado - no se puede guardar la puntuación");
            return;
        }
        
        // Obtener el usuario actual desde UserManager
        string usuarioActual = userManager.GetCurrentUser();
        Debug.Log("   Usuario actual: " + usuarioActual);
        
        if (string.IsNullOrEmpty(usuarioActual))
        {
            Debug.LogWarning("⚠️ No hay usuario logueado");
            return;
        }
        
        Debug.Log("   Llamando a UpdateGameScore...");
        // Actualizar el máximo del juego específico (solo si es mayor)
        userManager.UpdateGameScore(usuarioActual, nombreJuego, estrellas);
        
        Debug.Log("   Llamando a ActualizarTotalPuntacion...");
        // Recargar el total
        ActualizarTotalPuntacion();
        
        Debug.Log("   Total puntuación: " + totalPuntacion);
        
        // Actualizar TextMesh
        if (totalPointsText != null)
        {
            totalPointsText.text = totalPuntacion.ToString();
            Debug.Log("   TextMesh actualizado a: " + totalPuntacion);
        }
        else
        {
            Debug.LogWarning("⚠️ totalPointsText es null");
        }
        
        Debug.Log("✓ AgregarEstrellas() completado");
    }

    void ActualizarTotalPuntacion()
    {
        // Si no tenemos UserManager, buscarlo
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
        }
        
        if (userManager == null)
        {
            Debug.LogError("❌ UserManager es null en ActualizarTotalPuntacion");
            return;
        }
        
        // Obtener el usuario actual
        string usuarioActual = userManager.GetCurrentUser();
        // Debug.Log("   Usuario actual en ActualizarTotalPuntacion: " + usuarioActual);
        
        if (string.IsNullOrEmpty(usuarioActual))
        {
            //Debug.LogError("❌ Usuario current es null/empty");
            return;
        }
        
        // Intenta obtener datos del archivo users.json directamente
        User user = null;
        
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "users.json");
        string persistentPath = Path.Combine(Application.persistentDataPath, "users.json");
        
        //Debug.Log("   persistentPath: " + persistentPath + " | Existe: " + File.Exists(persistentPath));
        //Debug.Log("   streamingPath: " + streamingPath + " | Existe: " + File.Exists(streamingPath));
        
        // Intento 1: persistentDataPath (donde UserManager GUARDA en Editor)
        try
        {
            if (File.Exists(persistentPath))
            {
                //Debug.Log("   Leyendo persistentPath...");
                string json = File.ReadAllText(persistentPath, System.Text.Encoding.UTF8);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                //Debug.Log("   BD cargada con " + (database?.users?.Count ?? 0) + " usuarios");
                
                if (database != null && database.users != null)
                {
                    foreach (User u in database.users)
                    {
                        if (u.username == usuarioActual)
                        {
                            user = u;                           
                            break;
                        }
                    }
                }
                
                if (user == null)
                    Debug.Log("   ❌ Usuario " + usuarioActual + " NO encontrado en persistentPath");
            }
            else
            {
                Debug.Log("   persistentPath no existe");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error leyendo persistentPath: " + e.Message);
        }
        
        // Intento 2: StreamingAssets (fallback)
        if (user == null)
        {
            try
            {
                if (File.Exists(streamingPath))
                {
                    Debug.Log("   Leyendo streamingPath (fallback)...");
                    string json = File.ReadAllText(streamingPath, System.Text.Encoding.UTF8);
                    UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                    
                    if (database != null && database.users != null)
                    {
                        foreach (User u in database.users)
                        {
                            if (u.username == usuarioActual)
                            {
                                user = u;
                                Debug.Log("   ✓ Usuario encontrado en streamingPath");
                                break;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ Error leyendo StreamingAssets: " + e.Message);
            }
        }
        
        // Intento 3: UserManager (fallback en memoria)
        if (user == null)
        {
            Debug.Log("   Intentando fallback con UserManager...");
            user = userManager.GetUser(usuarioActual);
            if (user != null)
                Debug.Log("   ✓ Usuario encontrado en UserManager en memoria");
        }
        
        if (user == null)
        {
            Debug.LogError("❌ CRÍTICO: Usuario NO encontrado en ninguna fuente: " + usuarioActual);
            totalPuntacion = 0;
            return;
        }
        
        // Cargar todos los scores
        puntuacionRana = user.ranasScore;
        puntuacionBolas = user.bolasScore;
        puntuacionMochila = user.mochilaScore;
        puntuacionLaberinto = user.laberintoScore;
        puntuacionHS = user.hardwareSoftwareScore;
        puntuacionReina = user.reinaScore;
        puntuacionTresEnRaya = user.tresEnRayaScore;
        totalPuntacion = user.puntuacion;
        
        /*Debug.Log("✓ Puntuaciones cargadas:");
        Debug.Log("  - Ranas: " + puntuacionRana);
        Debug.Log("  - Bolas: " + puntuacionBolas);
        Debug.Log("  - Mochila: " + puntuacionMochila);
        Debug.Log("  - Laberinto: " + puntuacionLaberinto);
        Debug.Log("  - HS: " + puntuacionHS);
        Debug.Log("  - Reina: " + puntuacionReina);
        Debug.Log("  - TresEnRaya: " + puntuacionTresEnRaya);
        Debug.Log("  - TOTAL: " + totalPuntacion);*/
        
        // Mostrar en UI
        if (totalPointsText != null)
        {
            totalPointsText.text = totalPuntacion.ToString();
        }
    }

    public void SincronizarPuntuacionUsuario()
    {
        // Actualizar el usuario actual logueado
        usuarioLogueado = PlayerPrefs.GetString("UsuarioLogueado", "");
        Debug.Log("🔄 SincronizarPuntuacionUsuario() para usuario: " + usuarioLogueado);
        
        // Forzar búsqueda de UserManager
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
        }
        
        ActualizarTotalPuntacion();
        Debug.Log("✓ Puntuación sincronizada al cambiar usuario");
    }
    
    // Métodos para leer/escribir directamente del archivo users.json
    private int CargarPuntuacionDirecta(string username)
    {
        try
        {
            if (File.Exists(usersDatabasePath))
            {
                string json = File.ReadAllText(usersDatabasePath);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == username)
                        {
                            return user.puntuacion;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error al cargar puntuación directamente: " + e.Message);
        }
        
        return 0;
    }
    
    private void GuardarPuntuacionDirecta(string username, int newScore)
    {
        try
        {
            if (File.Exists(usersDatabasePath))
            {
                string json = File.ReadAllText(usersDatabasePath);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == username)
                        {
                            user.puntuacion = newScore;
                            break;
                        }
                    }
                    
                    // Guardar cambios
                    string updatedJson = JsonUtility.ToJson(database, true);
                    File.WriteAllText(usersDatabasePath, updatedJson);
                    Debug.Log("💾 Puntuación guardada directamente: " + username + " = " + newScore);
                    return;
                }
            }
            
            Debug.LogWarning("⚠️ No se pudo guardar la puntuación: archivo no encontrado");
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error al guardar puntuación directamente: " + e.Message);
        }
    }
    
    private void ActualizarPuntuacionJuegoDirecta(string username, string nombreJuego, int nuevaPuntuacion)
    {
        try
        {
            if (File.Exists(usersDatabasePath))
            {
                string json = File.ReadAllText(usersDatabasePath);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == username)
                        {
                            // Actualizar solo si la nueva puntuación es mayor
                            int puntuacionActual = ObtenerPuntuacionJuego(user, nombreJuego);
                            
                            if (nuevaPuntuacion > puntuacionActual)
                            {
                                ActualizarScoreJuego(user, nombreJuego, nuevaPuntuacion);
                                Debug.Log("🎮 " + nombreJuego + ": " + puntuacionActual + " → " + nuevaPuntuacion);
                            }
                            else
                            {
                                Debug.Log("🎮 " + nombreJuego + ": No se actualiza (" + nuevaPuntuacion + " ≤ " + puntuacionActual + ")");
                            }
                            
                            // Recalcular total
                            user.puntuacion = user.GetTotalScore();
                            break;
                        }
                    }
                    
                    // Guardar cambios
                    string updatedJson = JsonUtility.ToJson(database, true);
                    File.WriteAllText(usersDatabasePath, updatedJson);
                    return;
                }
            }
            
            Debug.LogWarning("⚠️ No se pudo actualizar puntuación del juego: archivo no encontrado");
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error al actualizar puntuación del juego: " + e.Message);
        }
    }
    
    private int ObtenerPuntuacionJuego(User user, string nombreJuego)
    {
        switch (nombreJuego.ToLower())
        {
            case "ranas":
                return user.ranasScore;
            case "bolas":
                return user.bolasScore;
            case "mochila":
                return user.mochilaScore;
            case "laberinto":
                return user.laberintoScore;
            case "hardwaresoftware":
                return user.hardwareSoftwareScore;
            case "reina":
                return user.reinaScore;
            case "tresEnraya":
            case "tresenraya":
                return user.tresEnRayaScore;
            default:
                return 0;
        }
    }
    
    private void ActualizarScoreJuego(User user, string nombreJuego, int nuevaPuntuacion)
    {
        switch (nombreJuego.ToLower())
        {
            case "ranas":
                user.ranasScore = nuevaPuntuacion;
                break;
            case "bolas":
                user.bolasScore = nuevaPuntuacion;
                break;
            case "mochila":
                user.mochilaScore = nuevaPuntuacion;
                break;
            case "laberinto":
                user.laberintoScore = nuevaPuntuacion;
                break;
            case "hardwaresoftware":
                user.hardwareSoftwareScore = nuevaPuntuacion;
                break;
            case "reina":
                user.reinaScore = nuevaPuntuacion;
                break;
            case "tresEnraya":
            case "tresenraya":
                user.tresEnRayaScore = nuevaPuntuacion;
                break;
        }
    }
}
