using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

public class UserManager : MonoBehaviour
{
    private UserDatabase database;
    private string filePath;
    private string fallbackPath;
    private bool isWebGL = false;
    private string serverURL;

    // Función para hashear contraseña con SHA256
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    void Awake()
    {
        // Hacer que UserManager persista entre escenas (como TotalStarsCounter)
        DontDestroyOnLoad(gameObject);
        
        // Detectar si estamos en WebGL
        #if UNITY_WEBGL && !UNITY_EDITOR
        isWebGL = true;
        PrintDebug("MODO WebGL detectado");
        #else
        isWebGL = false;
        PrintDebug("MODO Desktop detectado");
        #endif

        // CONFIGURAR URL DEL SERVIDOR
        #if UNITY_EDITOR
        // En editor: intentar localhost (para desarrollo)
        serverURL = "http://localhost:3002/api";
        #elif UNITY_WEBGL
        // En WebGL: usar la misma URL base que la página actual
        string currentURL = Application.absoluteURL;
        // Si está en localhost, usar localhost:3000. Si no, usar el servidor remoto
        if (currentURL.Contains("localhost"))
        {
            serverURL = "http://localhost:3002/api";
        }
        else
        {
            // Extraer el dominio de la URL actual
            System.Uri uri = new System.Uri(currentURL);
            string domain = uri.Scheme + "://" + uri.Host + (uri.Port != 80 && uri.Port != 443 ? ":" + uri.Port : "");
            serverURL = domain + ":3002/api";  // Asume que el servidor está en puerto 3002
            // O si está en la misma URL: serverURL = domain + "/api";
        }
        #else
        // Desktop build: usar localhost
        serverURL = "http://localhost:3002/api";
        #endif
        
        PrintDebug("URL Servidor: " + serverURL);

        // Inicializar database
        if (database == null)
        {
            database = new UserDatabase();
        }
        if (database.users == null)
        {
            database.users = new List<User>();
        }

        // EN EDITOR: guardar en persistentDataPath (escribible), leer de StreamingAssets (actualizado)
        // EN BUILD WebGL: usar servidor
        // EN BUILD Desktop: guardar en persistentDataPath
        #if UNITY_EDITOR
        filePath = Path.Combine(Application.persistentDataPath, "users.json");  // ← GUARDAR aquí (escribible)
        fallbackPath = Path.Combine(Application.streamingAssetsPath, "users.json");  // ← LEER de aquí (el que ves)
        #elif UNITY_WEBGL
        filePath = "localStorage"; 
        fallbackPath = "";
        #else
        filePath = Path.Combine(Application.persistentDataPath, "users.json");
        fallbackPath = Path.Combine(Application.streamingAssetsPath, "users.json");
        #endif
        
        PrintDebug("═══ INICIANDO USERMANAGER ═══");
        PrintDebug("Ruta GUARDAR: " + filePath);
        PrintDebug("Ruta LEER: " + fallbackPath);

        if (isWebGL)
        {
            PrintDebug("Cargando datos del servidor...");
            StartCoroutine(LoadUsersFromServer());
        }
        else
        {
            // Desktop: carga desde fallbackPath (StreamingAssets)
            if (File.Exists(fallbackPath))
            {
                PrintDebug("Cargando usuarios desde StreamingAssets");
                LoadUsers(fallbackPath);
            }
            else
            {
                PrintDebug("Archivo de usuarios no encontrado");
                database = new UserDatabase();
            }
            
            // LUEGO: Si existe archivo guardado con cambios (diferente de fallbackPath), cargarlo sobre la BD en memoria
            #if !UNITY_EDITOR
            if (File.Exists(filePath) && filePath != fallbackPath)
            {
                PrintDebug("Encontrado archivo guardado, cargando cambios...");
                try
                {
                    string json = File.ReadAllText(filePath);
                    UserDatabase savedDatabase = JsonUtility.FromJson<UserDatabase>(json);
                    if (savedDatabase != null && savedDatabase.users != null && savedDatabase.users.Count > 0)
                    {
                        // Actualizar puntuaciones de usuarios en memoria con los datos guardados
                        foreach (User savedUser in savedDatabase.users)
                        {
                            foreach (User memoryUser in database.users)
                            {
                                if (memoryUser.username == savedUser.username)
                                {
                                    // Copiar TODOS los scores desde el archivo guardado
                                    memoryUser.puntuacion = savedUser.puntuacion;
                                    memoryUser.ranasScore = savedUser.ranasScore;
                                    memoryUser.bolasScore = savedUser.bolasScore;
                                    memoryUser.mochilaScore = savedUser.mochilaScore;
                                    memoryUser.laberintoScore = savedUser.laberintoScore;
                                    memoryUser.quizScore = savedUser.quizScore;
                                    memoryUser.reinaScore = savedUser.reinaScore;
                                    break;
                                }
                            }
                        }
                        PrintDebug("Puntuaciones actualizadas desde archivo guardado");
                    }
                }
                catch (System.Exception e)
                {
                    PrintDebug("Error al cargar cambios guardados: " + e.Message);
                }
            }
            #endif
            
            PrintDebug("═══ USUARIOS CARGADOS: " + (database.users != null ? database.users.Count : 0) + " ═══\n");
        }
    }

    public bool Register(string username, string password)
    {
        PrintDebug("\nREGISTRANDO: " + username);
        
        if (isWebGL)
        {
            // En WebGL: enviar al servidor
            StartCoroutine(RegisterToServer(username, password));
            return true;
        }
        else
        {
            // En Desktop: guardar localmente
            if (UserExists(username))
            {
                PrintDebug("Usuario ya existe");
                return false;
            }

            if (database == null)
            {
                database = new UserDatabase();
            }
            if (database.users == null)
            {
                database.users = new List<User>();
            }

            User newUser = new User();
            newUser.username = username;
            newUser.password = HashPassword(password);
            newUser.nivel = 1;
            newUser.puntuacion = 0;
            newUser.ranasScore = 0;
            newUser.bolasScore = 0;
            newUser.mochilaScore = 0;
            newUser.laberintoScore = 0;
            newUser.quizScore = 0;
            newUser.reinaScore = 0;

            database.users.Add(newUser);
            PrintDebug("Agregado a lista. Total: " + database.users.Count);
            
            SaveUsers();
            PrintDebug("USUARIO REGISTRADO EXITOSAMENTE\n");
            return true;
        }
    }

    public bool Login(string username, string password)
    {
        PrintDebug("Login intento: " + username);
        
        if (isWebGL)
        {
            // En WebGL: verificar en el servidor
            StartCoroutine(LoginToServer(username, password));
            return true;
        }
        else
        {
            // En Desktop: verificar localmente
            if (database == null || database.users == null)
            {
                PrintDebug("Database es null");
                return false;
            }

            string hashedPassword = HashPassword(password);
            foreach (User user in database.users)
            {
                if (user.username == username && user.password == hashedPassword)
                {
                    PlayerPrefs.SetString("UsuarioLogueado", username);
                    PrintDebug("LOGIN EXITOSO");
                    return true;
                }
            }
            PrintDebug("Credenciales incorrectas");
            return false;
        }
    }

    public void LoginAsync(string username, string password, Action<bool> onComplete)
    {
        if (isWebGL)
        {
            StartCoroutine(LoginToServer(username, password, onComplete));
            return;
        }

        bool success = Login(username, password);
        onComplete?.Invoke(success);
    }

    public void RegisterAsync(string username, string password, Action<bool> onComplete)
    {
        if (isWebGL)
        {
            StartCoroutine(RegisterToServer(username, password, onComplete));
            return;
        }

        bool success = Register(username, password);
        onComplete?.Invoke(success);
    }

    public bool ChangePassword(string username, string newPassword)
    {
        PrintDebug("Cambio de contraseña: " + username);
        
        if (database == null || database.users == null)
        {
            PrintDebug("Database es null");
            return false;
        }

        foreach (User user in database.users)
        {
            if (user.username == username)
            {
                user.password = HashPassword(newPassword);
                SaveUsers();
                PrintDebug("✓ Contraseña actualizada para: " + username);
                return true;
            }
        }
        PrintDebug("Usuario no encontrado");
        return false;
    }

    private bool UserExists(string username)
    {
        if (database == null || database.users == null)
        {
            return false;
        }

        foreach (User user in database.users)
        {
            if (user.username == username)
                return true;
        }
        return false;
    }

    void SaveUsers()
    {
        try
        {
            if (database == null)
                database = new UserDatabase();
            if (database.users == null)
                database.users = new List<User>();

            if (isWebGL)
            {
                // En WebGL: guardar SOLO el usuario logueado (el único que cambia)
                string usuarioActual = GetCurrentUser();
                if (!string.IsNullOrEmpty(usuarioActual))
                {
                    User usuario = GetUser(usuarioActual);
                    if (usuario != null)
                    {
                        StartCoroutine(SaveUserToServer(usuario));
                    }
                }
            }
            else
            {
                // En Desktop: guardar en archivo
                string usuarioActual = GetCurrentUser();
                User usuario = GetUser(usuarioActual);
                if (usuario != null)
                {
                    // Primero lee el archivo actual
                    string json = File.ReadAllText(filePath);
                    UserDatabase fileDatabase = JsonUtility.FromJson<UserDatabase>(json);
                    
                    // Busca y actualiza solo el usuario logueado
                    foreach (User u in fileDatabase.users)
                    {
                        if (u.username == usuarioActual)
                        {
                            u.puntuacion = usuario.puntuacion;
                            u.ranasScore = usuario.ranasScore;
                            u.bolasScore = usuario.bolasScore;
                            u.mochilaScore = usuario.mochilaScore;
                            u.laberintoScore = usuario.laberintoScore;
                            u.quizScore = usuario.quizScore;
                            u.reinaScore = usuario.reinaScore;
                            u.tresEnRayaScore = usuario.tresEnRayaScore;
                            u.puzzleScore = usuario.puzzleScore;
                            u.animalesScore = usuario.animalesScore;
                            u.rutinaScore = usuario.rutinaScore;
                            break;
                        }
                    }
                    
                    // Guarda todo el archivo (pero solo cambió 1 usuario)
                    string jsonOut = JsonUtility.ToJson(fileDatabase, true);
                    File.WriteAllText(filePath, jsonOut);
                    
                    // BONUS en EDITOR: copiar a StreamingAssets para que veas los cambios
                    #if UNITY_EDITOR
                    try
                    {
                        string streamingPath = Path.Combine(Application.streamingAssetsPath, "users.json");
                        File.WriteAllText(streamingPath, jsonOut);
                        UnityEditor.AssetDatabase.Refresh();
                    }
                    catch (System.Exception copyErr)
                    {
                        // Silent fail - StreamingAssets copy is optional
                    }
                    #endif
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al guardar: " + e.Message);
        }
    }

    private IEnumerator SaveUserToServer(User user)
    {
        // Crear un JSON con los datos del usuario
        string jsonData = JsonUtility.ToJson(user, false);
        
        using (UnityWebRequest request = new UnityWebRequest(serverURL + "/save-all-scores", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                PrintDebug("Usuario guardado en servidor: " + user.username);
            }
            else
            {
                PrintDebug("Error guardando usuario en servidor: " + request.error);
            }
        }
    }

    void LoadUsersFromPlayerPrefs()
    {
        try
        {
            if (PlayerPrefs.HasKey("UsersDatabase"))
            {
                string json = PlayerPrefs.GetString("UsersDatabase");
                database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database == null)
                {
                    database = new UserDatabase();
                }
                if (database.users == null)
                {
                    database.users = new List<User>();
                }
                
                PrintDebug("Cargados " + database.users.Count + " usuarios desde localStorage");
            }
            else
            {
                database = new UserDatabase();
                PrintDebug(" localStorage vacío, base de datos nueva");
            }
        }
        catch (System.Exception e)
        {
            PrintDebug("Error al cargar de localStorage: " + e.Message);
            database = new UserDatabase();
        }
    }

    // Coroutines para servidor
    private IEnumerator RegisterToServer(string username, string password, Action<bool> onComplete = null)
    {
        // Crear usuario con todos los campos inicializados
        User newUser = new User();
        newUser.username = username;
        newUser.password = password;
        newUser.nivel = 1;
        newUser.puntuacion = 0;
        newUser.ranasScore = 0;
        newUser.bolasScore = 0;
        newUser.mochilaScore = 0;
        newUser.laberintoScore = 0;
        newUser.quizScore = 0;
        newUser.reinaScore = 0;

        string jsonData = JsonUtility.ToJson(newUser, false);
        
        using (UnityWebRequest request = new UnityWebRequest(serverURL + "/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PrintDebug("Registrado en servidor");
                // Agregar el usuario a la BD en memoria
                if (database == null)
                    database = new UserDatabase();
                if (database.users == null)
                    database.users = new List<User>();
                
                database.users.Add(newUser);
                PrintDebug("Usuario agregado a BD local");
                
                yield return StartCoroutine(LoadUsersFromServer());
                onComplete?.Invoke(true);
            }
            else
            {
                PrintDebug("Error en registro: " + request.error);
                onComplete?.Invoke(false);
            }
        }
    }

    private IEnumerator LoginToServer(string username, string password, Action<bool> onComplete = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(serverURL + "/login", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerPrefs.SetString("UsuarioLogueado", username);
                yield return StartCoroutine(LoadUsersFromServer());
                PrintDebug("✓ LOGIN EXITOSO");
                onComplete?.Invoke(true);
            }
            else
            {
                PrintDebug("Error en login: " + request.error);
                onComplete?.Invoke(false);
            }
        }
    }

    private IEnumerator LoadUsersFromServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverURL + "/users"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database == null)
                {
                    database = new UserDatabase();
                }
                if (database.users == null)
                {
                    database.users = new List<User>();
                }
                
                // Inicializar campos faltantes en usuarios cargados del servidor
                InitializeAllUserFields();
                
                PrintDebug("Cargados " + database.users.Count + " usuarios desde servidor");
                PrintDebug("═══ USUARIOS CARGADOS: " + database.users.Count + " ═══\n");
                
                // Notificar a TotalStarsCounter que actualice los datos
                TotalStarsCounter totalStars = FindFirstObjectByType<TotalStarsCounter>();
                if (totalStars != null)
                {
                    totalStars.SincronizarPuntuacionUsuario();
                }
            }
            else
            {
                PrintDebug("Error cargando usuarios: " + request.error);
                database = new UserDatabase();
            }
        }
    }

    void LoadUsers(string ruta)
    {
        try
        {
            if (File.Exists(ruta))
            {
                string json = File.ReadAllText(ruta);
                database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database == null)
                {
                    database = new UserDatabase();
                }
                if (database.users == null)
                {
                    database.users = new List<User>();
                }
                
                // Inicializar campos faltantes en usuarios cargados
                InitializeAllUserFields();
                
                PrintDebug("Cargados " + database.users.Count + " usuarios desde: " + ruta);
            }
            else
            {
                PrintDebug(" Archivo no existe: " + ruta);
                database = new UserDatabase();
            }
        }
        catch (System.Exception e)
        {
            PrintDebug("Error al cargar: " + e.Message);
            database = new UserDatabase();
        }
    }

    private void InitializeAllUserFields()
    {
        if (database == null || database.users == null)
            return;

        PrintDebug("Inicializando " + database.users.Count + " usuarios en BD...");
        foreach (User user in database.users)
        {
            PrintDebug("Usuario en BD: " + user.username + " | Puntuación: " + user.puntuacion + " | Ranas: " + user.ranasScore);
            // Los campos ya están inicializados en 0 por defecto en la clase User
        }
    }

    private void PrintDebug(string mensaje)
    {
        Debug.Log("<color=white>" + mensaje + "</color>");
    }

    public int GetUserScore(string username)
    {
        // En WebGL: buscar directamente en la BD en memoria (ya cargada del servidor)
        if (isWebGL)
        {
            if (database == null || database.users == null)
                return 0;

            foreach (User user in database.users)
            {
                if (user.username == username)
                    return user.puntuacion;
            }
            return 0;
        }

        // En Desktop: primero intenta cargar desde el archivo guardado (cambios recientes)
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                UserDatabase tempDatabase = JsonUtility.FromJson<UserDatabase>(json);
                if (tempDatabase != null && tempDatabase.users != null)
                {
                    foreach (User user in tempDatabase.users)
                    {
                        if (user.username == username)
                            return user.puntuacion;
                    }
                }
            }
            catch { }
        }
        
        // Si no encuentra en el archivo guardado, busca en la BD en memoria
        if (database == null || database.users == null)
            return 0;

        foreach (User user in database.users)
        {
            if (user.username == username)
                return user.puntuacion;
        }
        return 0;
    }

    public User GetUser(string username)
    {
        // En WebGL: buscar directamente en la BD en memoria (ya cargada del servidor)
        if (isWebGL)
        {
            if (database == null || database.users == null)
                return null;

            foreach (User user in database.users)
            {
                if (user.username == username)
                    return user;
            }
            return null;
        }

        // En Desktop: primero intenta cargar desde el archivo guardado
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                UserDatabase tempDatabase = JsonUtility.FromJson<UserDatabase>(json);
                if (tempDatabase != null && tempDatabase.users != null)
                {
                    foreach (User user in tempDatabase.users)
                    {
                        if (user.username == username)
                            return user;
                    }
                }
            }
            catch { }
        }
        
        // Si no encuentra, busca en la BD en memoria
        if (database == null || database.users == null)
            return null;

        foreach (User user in database.users)
        {
            if (user.username == username)
                return user;
        }
        return null;
    }

    public void UpdateUserScore(string username, int newScore)
    {
        if (database == null || database.users == null)
            return;

        foreach (User user in database.users)
        {
            if (user.username == username)
            {
                user.puntuacion = newScore;
                SaveUsers();
                PrintDebug("Puntuación guardada para " + username + ": " + newScore);
                return;
            }
        }
        PrintDebug("Usuario no encontrado para actualizar puntuación: " + username);
    }
    
    public string GetCurrentUser()
    {
        return PlayerPrefs.GetString("UsuarioLogueado", "");
    }
    
    public void UpdateGameScore(string username, string gameName, int gameScore)
    {
        if (database == null || database.users == null)
            return;

        foreach (User user in database.users)
        {
            if (user.username == username)
            {
                int puntuacionActual = ObtenerPuntuacionJuego(user, gameName);
                
                if (gameScore > puntuacionActual)
                {
                    ActualizarScoreJuego(user, gameName, gameScore);
                }
                
                // Recalcular total
                user.puntuacion = user.GetTotalScore();
                SaveUsers();
                return;
            }
        }
        Debug.LogError("Usuario no encontrado para actualizar juego: " + username);
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
            case "quiz":
                return user.quizScore;
            case "reina":
                return user.reinaScore;
            case "tresenraya":
                return user.tresEnRayaScore;
            case "puzzle":
                return user.puzzleScore;
            case "animales":
                return user.animalesScore;
            case "rutina":
                return user.rutinaScore;
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
            case "quiz":
                user.quizScore = nuevaPuntuacion;
                break;
            case "reina":
                user.reinaScore = nuevaPuntuacion;
                break;
            case "tresenraya":
                user.tresEnRayaScore = nuevaPuntuacion;
                break;
            case "puzzle":
                user.puzzleScore = nuevaPuntuacion;
                break;
            case "animales":
                user.animalesScore = nuevaPuntuacion;
                break;
            case "rutina":
                user.rutinaScore = nuevaPuntuacion;
                break;
        }
    }
}