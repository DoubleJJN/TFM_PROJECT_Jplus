using UnityEngine;
using TMPro;
using System.IO;
using System.Collections;

public class TotalStarsCounter : MonoBehaviour
{
    public int puntuacionRana, puntuacionBolas, puntuacionMochila, puntuacionLaberinto, puntuacionQuiz, puntuacionReina, puntuacionTresEnRaya, puntuacionPuzzle, puntuacionAnimales, puntuacionRutina;
    public static int totalPuntacion;
    public static TotalStarsCounter instance;
    public TextMeshProUGUI totalPointsText;
    private UserManager userManager;
    private string usuarioLogueado;
    private string usersDatabasePath;
    private float tiempoUltimaActualizacion = 0f;
    private const float INTERVALO_ACTUALIZACION = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        usersDatabasePath = Path.Combine(Application.streamingAssetsPath, "users.json");
        BuscarTextoPuntuacion();
        ActualizarTotalPuntacion();
    }

    void Update()
    {
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
        }
        
        // ¡MAGIA 2!: Si cambiamos de escena y la estrella se pierde, la buscamos automáticamente
        if (totalPointsText == null)
        {
            BuscarTextoPuntuacion();
        }
        
        tiempoUltimaActualizacion += Time.deltaTime;
        if (tiempoUltimaActualizacion >= INTERVALO_ACTUALIZACION)
        {
            tiempoUltimaActualizacion = 0f;
            ActualizarTotalPuntacion();
        }
    }

    private void BuscarTextoPuntuacion()
    {
        GameObject totalPointsObj = GameObject.Find("TotalPoints");
        if (totalPointsObj != null)
        {
            totalPointsText = totalPointsObj.GetComponent<TextMeshProUGUI>();
            totalPointsText.GetComponent<RectTransform>().SetAsLastSibling();
            Color textColor = totalPointsText.color;
            textColor.a = 1f;
            totalPointsText.color = textColor;
            totalPointsText.text = totalPuntacion.ToString();
        }
    }

    public void AgregarEstrellas(int estrellas, string nombreJuego)
    {
        if (userManager == null) userManager = FindFirstObjectByType<UserManager>();
        if (userManager == null) return;
        
        string usuarioActual = userManager.GetCurrentUser();
        if (string.IsNullOrEmpty(usuarioActual)) return;
        
        userManager.UpdateGameScore(usuarioActual, nombreJuego, estrellas);
        ActualizarTotalPuntacion();
    }

    void ActualizarTotalPuntacion()
    {
        if (userManager == null) userManager = FindFirstObjectByType<UserManager>();
        if (userManager == null) return;
        
        string usuarioActual = userManager.GetCurrentUser();
        if (string.IsNullOrEmpty(usuarioActual)) return;
        
        User user = null;
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "users.json");
        string persistentPath = Path.Combine(Application.persistentDataPath, "users.json");
        
        try
        {
            if (File.Exists(persistentPath))
            {
                string json = File.ReadAllText(persistentPath, System.Text.Encoding.UTF8);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                if (database != null && database.users != null)
                {
                    foreach (User u in database.users)
                    {
                        if (u.username == usuarioActual) { user = u; break; }
                    }
                }
            }
        }
        catch { }
        
        if (user == null)
        {
            try
            {
                if (File.Exists(streamingPath))
                {
                    string json = File.ReadAllText(streamingPath, System.Text.Encoding.UTF8);
                    UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                    if (database != null && database.users != null)
                    {
                        foreach (User u in database.users)
                        {
                            if (u.username == usuarioActual) { user = u; break; }
                        }
                    }
                }
            }
            catch { }
        }
        
        if (user == null) user = userManager.GetUser(usuarioActual);
        
        if (user == null)
        {
            totalPuntacion = 0;
            return;
        }
        
        puntuacionRana = user.ranasScore;
        puntuacionBolas = user.bolasScore;
        puntuacionMochila = user.mochilaScore;
        puntuacionLaberinto = user.laberintoScore;
        puntuacionQuiz = user.quizScore;
        puntuacionReina = user.reinaScore;
        puntuacionTresEnRaya = user.tresEnRayaScore;
        puntuacionPuzzle = user.puzzleScore;
        puntuacionAnimales = user.animalesScore;
        puntuacionRutina = user.rutinaScore;
        totalPuntacion = user.puntuacion;
        
        if (totalPointsText != null)
        {
            totalPointsText.text = totalPuntacion.ToString();
        }
    }

    public void SincronizarPuntuacionUsuario()
    {
        usuarioLogueado = PlayerPrefs.GetString("UsuarioLogueado", "");
        if (userManager == null) userManager = FindFirstObjectByType<UserManager>();
        ActualizarTotalPuntacion();
    }
    
    // --- MÉTODOS ORIGINALES DE JSON INTACTOS ---
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
                        if (user.username == username) return user.puntuacion;
                    }
                }
            }
        }
        catch (System.Exception e) { Debug.LogError("Error: " + e.Message); }
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
                    string updatedJson = JsonUtility.ToJson(database, true);
                    File.WriteAllText(usersDatabasePath, updatedJson);
                    return;
                }
            }
        }
        catch (System.Exception e) { Debug.LogError("Error: " + e.Message); }
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
                            int puntuacionActual = ObtenerPuntuacionJuego(user, nombreJuego);
                            if (nuevaPuntuacion > puntuacionActual)
                            {
                                ActualizarScoreJuego(user, nombreJuego, nuevaPuntuacion);
                            }
                            user.puntuacion = user.GetTotalScore();
                            break;
                        }
                    }
                    string updatedJson = JsonUtility.ToJson(database, true);
                    File.WriteAllText(usersDatabasePath, updatedJson);
                    return;
                }
            }
        }
        catch (System.Exception e) { Debug.LogError("Error: " + e.Message); }
    }
    
    private int ObtenerPuntuacionJuego(User user, string nombreJuego)
    {
        switch (nombreJuego.ToLower())
        {
            case "ranas": return user.ranasScore;
            case "bolas": return user.bolasScore;
            case "mochila": return user.mochilaScore;
            case "laberinto": return user.laberintoScore;
            case "quiz": return user.quizScore;
            case "reina": return user.reinaScore;
            case "tresenraya": return user.tresEnRayaScore;
            case "puzzle": return user.puzzleScore;
            case "animales": return user.animalesScore;
            case "rutina": return user.rutinaScore;
            default: return 0;
        }
    }
    
    private void ActualizarScoreJuego(User user, string nombreJuego, int nuevaPuntuacion)
    {
        switch (nombreJuego.ToLower())
        {
            case "ranas": user.ranasScore = nuevaPuntuacion; break;
            case "bolas": user.bolasScore = nuevaPuntuacion; break;
            case "mochila": user.mochilaScore = nuevaPuntuacion; break;
            case "laberinto": user.laberintoScore = nuevaPuntuacion; break;
            case "quiz": user.quizScore = nuevaPuntuacion; break;
            case "reina": user.reinaScore = nuevaPuntuacion; break;
            case "tresenraya": user.tresEnRayaScore = nuevaPuntuacion; break;
            case "puzzle": user.puzzleScore = nuevaPuntuacion; break;
            case "animales": user.animalesScore = nuevaPuntuacion; break;
            case "rutina": user.rutinaScore = nuevaPuntuacion; break;
        }
    }
}