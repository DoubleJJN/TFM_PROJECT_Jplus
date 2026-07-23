using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class JuegoDeTresEnRaya : MonoBehaviour
{
    public TMP_InputField inputRivalUsername;
    public Button buttonAceptar;
    public Button buttonPrueba;
    public GameObject popUpRivalUsername; // Popup para ingresar rival
    public GameObject panelPrincipal; // Panel principal del juego
    public PopUpGanar popUpGanar; // Popup para mostrar estrella ganada
    
    // Para WebGL: URL del servidor (configurable en Inspector)
    public string serverURL = "http://localhost:3002";
    
    // Marcadores de turno
    public GameObject marcador1; // Marcador para turno del jugador "Yo"
    public GameObject marcador2; // Marcador para turno del rival
    
    private UserManager userManager;
    private string usuarioActual;
    private string usuarioRival = "";
    
    private User usuarioActualObj = null;
    private User usuarioRivalObj = null;
    // aquí lo manejo yo
    public TextMeshProUGUI yo, rival, ronda, noExiste;
    public Button[] cuadros = new Button[9];
    int[] estadoCuadros = new int[9];//0 vacio, 1 X, 2 O
    
    private Queue<int> posicionesJugador = new Queue<int>(); // O del jugador actual
    private Queue<int> posicionesRival = new Queue<int>(); // X del rival
    
    private int rondasActuales = 0;
    private bool estuvoAceptado = false;

    void Start()
    {
        Debug.Log("═══ INICIANDO JUEGO TRES EN RAYA ═══");
        // Detectar plataforma
        #if UNITY_EDITOR
        Debug.Log("💻 Plataforma: Editor (siempre usa archivos locales)");
        #elif UNITY_WEBGL
        Debug.Log("🌐 Plataforma: WebGL Build");
        #else
        Debug.Log("💻 Plataforma: Desktop Standalone");
        #endif
        
        // Obtener usuario logueado
        usuarioActual = PlayerPrefs.GetString("UsuarioLogueado", "");
        Debug.Log("👤 Usuario actual de PlayerPrefs: '" + usuarioActual + "'");
        
        if (string.IsNullOrEmpty(usuarioActual))
        {
            Debug.LogError("❌ CRÍTICO: No hay usuario logueado en PlayerPrefs");
        }
        
        // Buscar UserManager en la escena
        userManager = FindFirstObjectByType<UserManager>();
        if (userManager != null)
        {
            Debug.Log("✓ UserManager encontrado");
        }
        
        // Si no están asignados en el Inspector, intentar encontrarlos
        if (inputRivalUsername == null)
        {
            inputRivalUsername = FindFirstObjectByType<TMP_InputField>();
            Debug.Log("🔍 InputField encontrado automáticamente: " + (inputRivalUsername != null ? "✓" : "❌"));
        }
        
        if (buttonAceptar == null)
        {
            Button[] botones = FindObjectsByType<Button>(FindObjectsSortMode.None);
            if (botones.Length > 0)
                buttonAceptar = botones[0];
            Debug.Log("🔍 Botón Aceptar encontrado automáticamente: " + (buttonAceptar != null ? "✓" : "❌"));
        }
        
        if (buttonPrueba == null && buttonAceptar != null)
        {
            Button[] botones = FindObjectsByType<Button>(FindObjectsSortMode.None);
            if (botones.Length > 1)
                buttonPrueba = botones[1];
            Debug.Log("🔍 Botón Prueba encontrado automáticamente: " + (buttonPrueba != null ? "✓" : "❌"));
        }
        
        Debug.Log("🔍 Referencias encontradas:");
        Debug.Log("   InputField: " + (inputRivalUsername != null ? "✓" : "❌"));
        Debug.Log("   ButtonAceptar: " + (buttonAceptar != null ? "✓" : "❌"));
        Debug.Log("   ButtonPrueba: " + (buttonPrueba != null ? "✓" : "❌"));
        
        // Conectar botones
        if (buttonAceptar != null)
        {
            buttonAceptar.onClick.AddListener(ValidarRival);
            Debug.Log("✓ Botón Aceptar conectado");
        }
        
        if (buttonPrueba != null)
        {
            buttonPrueba.onClick.AddListener(BotonPrueba);
            Debug.Log("✓ Botón Prueba conectado");
        }
        
        // Mostrar popup de rival y mostrar panel principal al inicio
        // El popup estará encima del panel, obstruyendo la vista hasta que se valide el rival
        if (popUpRivalUsername != null)
        {
            popUpRivalUsername.SetActive(true);
            Debug.Log("✓ Popup de rival mostrado (quedará encima)");
        }
        
        if (panelPrincipal != null)
        {
            panelPrincipal.SetActive(true);
            Debug.Log("✓ Panel principal mostrado (debajo del popup)");
        }
        
        Debug.Log("═══ JUEGO LISTO ═══");
    }
    void ShowFeedback(string message)
    {
        noExiste.text = message;
        StartCoroutine(ClearFeedbackAfterDelay());
    }

    IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        noExiste.text = "";
    }
    private void ValidarRival()
    {
        if (inputRivalUsername == null)
        {
            Debug.LogError("❌ InputField es NULL");
            return;
        }
        
        string nombreRival = inputRivalUsername.text.Trim();
        Debug.Log("📝 Texto ingresado: '" + nombreRival + "'");
        
        if (string.IsNullOrEmpty(nombreRival))
        {
            Debug.LogWarning("⚠️ Nombre de rival vacío");
            return;
        }
        
        // Buscar al usuario rival y guardar referencia
        usuarioRivalObj = ObtenerUsuarioPorNombre(nombreRival);
        
        if (usuarioRivalObj != null)
        {
            usuarioRival = nombreRival;
            Debug.Log("✓ Rival encontrado: " + usuarioRival);
            
            // Mostrar nombres en los TextMeshPro
            if (yo != null)
                yo.text = usuarioActual;
            if (rival != null)
                rival.text = usuarioRival;
            
            // Inicializar ronda en 0
            rondasActuales = 0;
            ActualizarTextoRonda();
            
            // Inicializar colas de posiciones
            posicionesJugador = new Queue<int>();
            posicionesRival = new Queue<int>();
            
            // Actualizar marcadores para mostrar el primer turno (del jugador actual)
            ActualizarMarcadores();
            
            // Conectar botones a sus métodos de click
            for (int i = 0; i < cuadros.Length; i++)
            {
                int indice = i; // Capturar para closure
                if (cuadros[i] != null)
                {
                    cuadros[i].onClick.AddListener(() => OnClickCuadro(indice));
                }
            }
            
            estuvoAceptado = true;
            
            // Ocultar popup de rival
            if (popUpRivalUsername != null)
            {
                popUpRivalUsername.SetActive(false);
                Debug.Log("✓ Popup ocultado");
            }
            
            // Mostrar panel principal del juego
            if (panelPrincipal != null)
            {
                panelPrincipal.SetActive(true);
                Debug.Log("✓ Panel principal mostrado");
            }
        }
        else
        {
            Debug.LogWarning("❌ Usuario rival no encontrado: " + nombreRival);
            ShowFeedback("No existe este usuario");
        }
    }
    
    private void ActualizarTextoRonda()
    {
        if (ronda != null)
        {
            ronda.text = "Ronda: " + rondasActuales + " / 30";
        }
    }
    
    /// <summary>
    /// Actualiza la visibilidad de los marcadores según el turno actual
    /// </summary>
    private void ActualizarMarcadores()
    {
        int turnoActual = rondasActuales % 2; // 0 = Yo, 1 = Rival
        
        if (turnoActual == 0)
        {
            // Turno del jugador actual (O) → Mostrar Marcador1, ocultar Marcador2
            if (marcador1 != null)
                marcador1.SetActive(true);
            if (marcador2 != null)
                marcador2.SetActive(false);
            
            Debug.Log("🎯 Marcador: Turno de YO");
        }
        else
        {
            // Turno del rival (X) → Ocultar Marcador1, mostrar Marcador2
            if (marcador1 != null)
                marcador1.SetActive(false);
            if (marcador2 != null)
                marcador2.SetActive(true);
            
            Debug.Log("🎯 Marcador: Turno del RIVAL");
        }
    }
    
    /// <summary>
    /// Se llama cuando el usuario hace click en un cuadro del grid
    /// </summary>
    private void OnClickCuadro(int indice)
    {
        if (!estuvoAceptado)
        {
            Debug.LogWarning("⚠️ El juego aún no ha comenzado");
            return;
        }
        
        if (indice < 0 || indice >= 9)
        {
            Debug.LogError("❌ Índice de cuadro inválido: " + indice);
            return;
        }
        
        // Si el cuadro ya está ocupado, no hacer nada
        if (estadoCuadros[indice] != 0)
        {
            Debug.LogWarning("⚠️ Cuadro " + indice + " ya está ocupado");
            return;
        }
        
        // Determinar turno (0→1 es O "Yo", 1→0 es X "Rival")
        int turno = rondasActuales % 2; // 0 = Yo, 1 = Rival
        
        // Marcar el cuadro
        if (turno == 0) // Turno del jugador actual (O)
        {
            estadoCuadros[indice] = 2; // 2 = O
            ActualizarVisualCuadro(indice, "O");
            
            // Agregar a la cola de posiciones del jugador
            posicionesJugador.Enqueue(indice);
            
            // Si tiene más de 3 símbolos, remover el primero
            if (posicionesJugador.Count > 3)
            {
                int indiceARemover = posicionesJugador.Dequeue();
                RemoverSimbolo(indiceARemover);
                Debug.Log("🗑️ Símbolo O removido del cuadro " + indiceARemover + " (límite de 3 alcanzado)");
            }
        }
        else // Turno del rival (X)
        {
            estadoCuadros[indice] = 1; // 1 = X
            ActualizarVisualCuadro(indice, "X");
            
            // Agregar a la cola de posiciones del rival
            posicionesRival.Enqueue(indice);
            
            // Si tiene más de 3 símbolos, remover el primero
            if (posicionesRival.Count > 3)
            {
                int indiceARemover = posicionesRival.Dequeue();
                RemoverSimbolo(indiceARemover);
                Debug.Log("🗑️ Símbolo X removido del cuadro " + indiceARemover + " (límite de 3 alcanzado)");
            }
        }
        
        // Incrementar rondas
        rondasActuales++;
        ActualizarTextoRonda();
        ActualizarMarcadores(); // Cambiar el marcador al siguiente turno
        
        Debug.Log("🎮 Jugada en cuadro " + indice + " | Turno: " + (turno == 0 ? "Yo (O)" : "Rival (X)") + " | Ronda: " + rondasActuales);
        
        // Verificar si hay victoria
        string ganador = DetectarVictoria();
        if (ganador != null)
        {
            // Hay ganador
            string ganadorNombre = (ganador == "O") ? usuarioActual : usuarioRival;
            Debug.Log("🎉 ¡" + ganadorNombre + " GANÓ! (" + ganador + ")");
            TerminarPartida(ganadorNombre, rondasActuales);
            return;
        }
        
        // Verificar si es empate (30 rondas completadas)
        if (rondasActuales >= 30)
        {
            Debug.Log("🤝 ¡EMPATE! Se completaron las 30 rondas");
            EmpatePartida(rondasActuales);
            return;
        }
    }
    
    /// <summary>
    /// Actualiza el visual del cuadro con el símbolo (X u O)
    /// </summary>
    private void ActualizarVisualCuadro(int indice, string simbolo)
    {
        if (cuadros[indice] == null)
        {
            Debug.LogError("❌ Cuadro " + indice + " es null");
            return;
        }
        
        // Buscar el componente TextMeshProUGUI llamado "Symbol" en el botón
        TextMeshProUGUI symbolText = cuadros[indice].GetComponentInChildren<TextMeshProUGUI>();
        
        if (symbolText != null)
        {
            symbolText.text = simbolo;
            Debug.Log("✓ Cuadro " + indice + " actualizado con: " + simbolo);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró componente TextMeshProUGUI en cuadro " + indice);
        }
        
        // Desactivar el botón para que no se pueda hacer click de nuevo
        cuadros[indice].interactable = false;
    }
    
    /// <summary>
    /// Detecta si hay victoria verificando filas, columnas y diagonales
    /// Retorna el ganador ("O" = usuarioActual, "X" = usuarioRival) o null si no hay victoria
    /// </summary>
    private string DetectarVictoria()
    {
        // Combinaciones ganadoras (índices del grid 3x3)
        int[][] combinacionesGanadoras = new int[][]
        {
            // Filas
            new int[] {0, 1, 2},
            new int[] {3, 4, 5},
            new int[] {6, 7, 8},
            
            // Columnas
            new int[] {0, 3, 6},
            new int[] {1, 4, 7},
            new int[] {2, 5, 8},
            
            // Diagonales
            new int[] {0, 4, 8},
            new int[] {2, 4, 6}
        };
        
        // Verificar cada combinación
        foreach (int[] combinacion in combinacionesGanadoras)
        {
            int val1 = estadoCuadros[combinacion[0]];
            int val2 = estadoCuadros[combinacion[1]];
            int val3 = estadoCuadros[combinacion[2]];
            
            // Si los tres valores son iguales y no están vacíos
            if (val1 != 0 && val1 == val2 && val2 == val3)
            {
                // val1 == 1 → X (usuarioRival gana)
                // val1 == 2 → O (usuarioActual gana)
                return (val1 == 1) ? "X" : "O";
            }
        }
        
        return null; // No hay victoria
    }
    
    void Update()
    {
        // Aquí podrías verificar victoria después de cada jugada
        // Pero lo ideal es hacerlo directamente en OnClickCuadro()
    }
    
    private User ObtenerUsuarioPorNombre(string username)
    {
        // En Editor: intentar persistentDataPath primero, fallback a StreamingAssets
        // En Desktop: intentar StreamingAssets primero
        
        #if UNITY_EDITOR
        string persistentPath = Path.Combine(Application.persistentDataPath, "users.json");
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "users.json");
        
        // Intento 1: StreamingAssets (archivo principal)
        try
        {
            if (File.Exists(streamingPath))
            {
                string json = File.ReadAllText(streamingPath, System.Text.Encoding.UTF8);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == username)
                        {
                            return user;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error leyendo StreamingAssets: " + e.Message);
        }
        
        // Intento 2: persistentDataPath fallback
        try
        {
            if (File.Exists(persistentPath))
            {
                string json = File.ReadAllText(persistentPath, System.Text.Encoding.UTF8);
                UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
                
                if (database != null && database.users != null)
                {
                    foreach (User user in database.users)
                    {
                        if (user.username == username)
                        {
                            return user;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error leyendo persistentDataPath: " + e.Message);
        }
        
        #else
        // En WebGL: buscar desde UserManager (datos en memoria del servidor)
        if (userManager != null)
        {
            User user = userManager.GetUser(username);
            if (user != null)
            {
                return user;
            }
        }
        #endif
        
        // Fallback final a UserManager si el archivo no funciona
        if (userManager != null)
        {
            User user = userManager.GetUser(username);
            if (user != null)
            {
                return user;
            }
        }
        
        return null;
    }
    
    private void BotonPrueba()
    {
        // Este método ya no se usa (buttonPrueba fue removido en el rediseño del juego)
        Debug.LogWarning("⚠️ BotonPrueba() llamado pero ya no se usa en el nuevo sistema");
    }
    
    /// <summary>
    /// Llamar cuando la partida termina con un ganador
    /// </summary>
    public void TerminarPartida(string ganador, int rondas)
    {
        Debug.Log("🎮 TerminarPartida() - Ganador: " + ganador + ", Rondas: " + rondas);
        
        // Desactivar todos los botones para que no se pueda seguir jugando
        DesactivarTodosBotones();
        
        if (string.IsNullOrEmpty(ganador))
        {
            Debug.LogError("❌ TerminarPartida: ganador vacío");
            return;
        }
        
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
        }
        
        if (userManager == null)
        {
            Debug.LogError("❌ UserManager no encontrado");
            return;
        }
        
        // Calcular puntos según rondas
        int puntosGanador = CalcularPuntos(rondas);
        int puntosPerdedor = 0; // El perdedor no recibe puntos
        
        Debug.Log("📊 Rondas: " + rondas + " | Puntos ganador: " + puntosGanador + " | Puntos perdedor: " + puntosPerdedor);
        
        // Determinar quién gana y quién pierde
        string perdedor = (ganador == usuarioActual) ? usuarioRival : usuarioActual;
        
        // Actualizar ambos jugadores con UserManager
        userManager.UpdateGameScore(ganador, "TresEnRaya", puntosGanador);
        userManager.UpdateGameScore(perdedor, "TresEnRaya", puntosPerdedor);
        string mensajeTerminado = "Partida terminada. Ganador: " + ganador + " | Rondas: " + rondas;
        // Mostrar popup de victoria
        if (popUpGanar == null)
        {
            popUpGanar = FindFirstObjectByType<PopUpGanar>();
        }
        
        if (popUpGanar != null)
        {
            // Convertir puntos a "estrellas" para mostrar
            // 3 puntos = 3 estrellas, 2 puntos = 2 estrellas, 1 punto = 1 estrella
            popUpGanar.SetNombreJuego("TresEnRaya");
            popUpGanar.MostrarPopUpGanado(3 - puntosGanador, mensajeTerminado); // 0→3 stars, 1→2 stars, 2→1 star
        }
        else
        {
            Debug.LogWarning("⚠️ PopUpGanar no encontrado, volviendo a menú");
            SceneManager.LoadScene("Escenas");
        }
    }
    
    /// <summary>
    /// Llamar cuando la partida termina en empate (30 rondas completadas)
    /// </summary>
    public void EmpatePartida(int rondas)
    {
        Debug.Log("🎮 EmpatePartida() - Rondas: " + rondas);
        
        // Desactivar todos los botones para que no se pueda seguir jugando
        DesactivarTodosBotones();
        
        if (rondas != 30)
        {
            Debug.LogWarning("⚠️ EmpatePartida llamado pero rondas != 30: " + rondas);
        }
        
        if (userManager == null)
        {
            userManager = FindFirstObjectByType<UserManager>();
        }
        
        if (userManager == null)
        {
            Debug.LogError("❌ UserManager no encontrado");
            return;
        }
        
        // En empate: 1 punto a cada jugador
        int puntosEmpate = 1;
        
        Debug.Log("📊 Empate - Ambos reciben: " + puntosEmpate + " punto");
        
        // Actualizar ambos jugadores
        userManager.UpdateGameScore(usuarioActual, "TresEnRaya", puntosEmpate);
        userManager.UpdateGameScore(usuarioRival, "TresEnRaya", puntosEmpate);
        
        // Mostrar popup de empate (1 estrella)
        if (popUpGanar == null)
        {
            popUpGanar = FindFirstObjectByType<PopUpGanar>();
        }
        
        if (popUpGanar != null)
        {
            popUpGanar.SetNombreJuego("TresEnRaya");
            popUpGanar.MostrarPopUpGanado(2, "La partida ha terminado en empate."); // 2→1 star
        }
        else
        {
            Debug.LogWarning("⚠️ PopUpGanar no encontrado, volviendo a menú");
            SceneManager.LoadScene("Escenas");
        }
    }
    
    /// <summary>
    /// Calcula los puntos basado en el número de rondas
    /// </summary>
    private int CalcularPuntos(int rondas)
    {
        if (rondas <= 10)
            return 3;
        else if (rondas <= 20)
            return 2;
        else
            return 1;
    }
    
    private void VerificarGuardoLocal()
    {
        // Verificar que se guardó correctamente leyendo del archivo
        string verificacionPath = Path.Combine(Application.streamingAssetsPath, "users.json");
        try
        {
            string jsonVerif = File.ReadAllText(verificacionPath);
            UserDatabase dbVerif = JsonUtility.FromJson<UserDatabase>(jsonVerif);
            foreach (User u in dbVerif.users)
            {
                if (u.username == usuarioActual || u.username == usuarioRival)
                {
                    Debug.Log("📊 Verificación archivo - " + u.username + ": tresEnRayaScore=" + u.tresEnRayaScore + ", puntuacion=" + u.puntuacion);
                }
            }
        }
        catch
        {
            Debug.LogWarning("⚠️ No se pudo verificar el archivo guardado");
        }
    }
    
    private bool GuardarCambios()
    {
        // El Editor siempre usa archivos locales, sin importar el build target
        #if UNITY_EDITOR
        return GuardarCambiosLocal();
        #elif UNITY_WEBGL
        // WebGL compilado (NO en editor): usar servidor HTTP
        GuardarCambiosHTTP();
        return true; // Asíncrono
        #else
        // Desktop standalone
        return GuardarCambiosLocal();
        #endif
    }
    
    private bool GuardarCambiosLocal()
    {
        try
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "users.json");
            string persistentPath = Path.Combine(Application.persistentDataPath, "users.json");
            
            // Leer desde donde sea que esté el archivo
            string json;
            
            // Intentar leer de StreamingAssets primero
            if (File.Exists(streamingPath))
            {
                json = File.ReadAllText(streamingPath, System.Text.Encoding.UTF8);
            }
            // Si no existe en StreamingAssets, intentar persistentDataPath
            else if (File.Exists(persistentPath))
            {
                json = File.ReadAllText(persistentPath, System.Text.Encoding.UTF8);
            }
            else
            {
                Debug.LogError("❌ No se encontró users.json en StreamingAssets ni persistentDataPath");
                return false;
            }
            
            UserDatabase database = JsonUtility.FromJson<UserDatabase>(json);
            
            if (database == null || database.users == null)
            {
                Debug.LogError("❌ No se pudo parsear la base de datos JSON");
                return false;
            }
            
            // Actualizar los usuarios en la base de datos
            int usuariosActualizados = 0;
            
            for (int i = 0; i < database.users.Count; i++)
            {
                if (database.users[i].username == usuarioActual && usuarioActualObj != null)
                {
                    database.users[i].tresEnRayaScore = usuarioActualObj.tresEnRayaScore;
                    database.users[i].puntuacion = usuarioActualObj.puntuacion;
                    database.users[i].ranasScore = usuarioActualObj.ranasScore;
                    database.users[i].bolasScore = usuarioActualObj.bolasScore;
                    database.users[i].mochilaScore = usuarioActualObj.mochilaScore;
                    database.users[i].laberintoScore = usuarioActualObj.laberintoScore;
                    database.users[i].quizScore = usuarioActualObj.quizScore;
                    database.users[i].reinaScore = usuarioActualObj.reinaScore;
                    
                    usuariosActualizados++;
                }
                
                if (database.users[i].username == usuarioRival && usuarioRivalObj != null)
                {
                    database.users[i].tresEnRayaScore = usuarioRivalObj.tresEnRayaScore;
                    database.users[i].puntuacion = usuarioRivalObj.puntuacion;
                    database.users[i].ranasScore = usuarioRivalObj.ranasScore;
                    database.users[i].bolasScore = usuarioRivalObj.bolasScore;
                    database.users[i].mochilaScore = usuarioRivalObj.mochilaScore;
                    database.users[i].laberintoScore = usuarioRivalObj.laberintoScore;
                    database.users[i].quizScore = usuarioRivalObj.quizScore;
                    database.users[i].reinaScore = usuarioRivalObj.reinaScore;
                    
                    usuariosActualizados++;
                }
            }
            
            if (usuariosActualizados < 2)
            {
                Debug.LogError("❌ Solo se actualizaron " + usuariosActualizados + " usuarios. Se esperaban 2.");
                return false;
            }
            
            // Serializar
            string jsonActualizado = JsonUtility.ToJson(database, true);
            
            // PRIMARY: Guardar a StreamingAssets/users.json (el archivo VISIBLE que tú ves)
            bool guardoExitoso = false;
            try
            {
                // Intentar escribir
                File.WriteAllText(streamingPath, jsonActualizado, System.Text.Encoding.UTF8);
                guardoExitoso = true;
                
                #if UNITY_EDITOR
                // Refrescar Asset Database para que Unity vea el cambio inmediatamente
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ ERROR al escribir en StreamingAssets/users.json: " + e.Message);
                
                // FALLBACK: Si falla, guardar en persistentDataPath como respaldo
                try
                {
                    File.WriteAllText(persistentPath, jsonActualizado, System.Text.Encoding.UTF8);
                    guardoExitoso = true;
                }
                catch (System.Exception fallbackErr)
                {
                    Debug.LogError("❌ CRÍTICO: Fallo ambos intentos de guardar: " + fallbackErr.Message);
                    guardoExitoso = false;
                }
            }
            
            return guardoExitoso;
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error CRÍTICO al guardar cambios: " + e.Message);
            Debug.LogError("   StackTrace: " + e.StackTrace);
            return false;
        }
    }
    
    private void GuardarCambiosHTTP()
    {
        Debug.Log("📡 Preparando datos para enviar al servidor HTTP...");
        
        // JSON para enviar al servidor
        string postData = JsonUtility.ToJson(new SaveScoresRequest
        {
            username = usuarioActual,
            puntuacion = usuarioActualObj.puntuacion,
            nivel = 1,
            ranasScore = usuarioActualObj.ranasScore,
            bolasScore = usuarioActualObj.bolasScore,
            mochilaScore = usuarioActualObj.mochilaScore,
            laberintoScore = usuarioActualObj.laberintoScore,
            quizScore = usuarioActualObj.quizScore,
            reinaScore = usuarioActualObj.reinaScore,
            tresEnRayaScore = usuarioActualObj.tresEnRayaScore,
            puzzleScore = usuarioActualObj.puzzleScore,
            animalesScore = usuarioActualObj.animalesScore,
            rutinaScore = usuarioActualObj.rutinaScore
        });
        
        StartCoroutine(EnviarScoresAlServidor(usuarioActual, postData));
        
        // También guardar rival
        if (usuarioRivalObj != null)
        {
            string postDataRival = JsonUtility.ToJson(new SaveScoresRequest
            {
                username = usuarioRival,
                puntuacion = usuarioRivalObj.puntuacion,
                nivel = 1,
                ranasScore = usuarioRivalObj.ranasScore,
                bolasScore = usuarioRivalObj.bolasScore,
                mochilaScore = usuarioRivalObj.mochilaScore,
                laberintoScore = usuarioRivalObj.laberintoScore,
                quizScore = usuarioRivalObj.quizScore,
                reinaScore = usuarioRivalObj.reinaScore,
                tresEnRayaScore = usuarioRivalObj.tresEnRayaScore,
                puzzleScore = usuarioRivalObj.puzzleScore,
                animalesScore = usuarioRivalObj.animalesScore,
                rutinaScore = usuarioRivalObj.rutinaScore
            });
            
            StartCoroutine(EnviarScoresAlServidor(usuarioRival, postDataRival));
        }
    }
    
    private System.Collections.IEnumerator EnviarScoresAlServidor(string username, string jsonData)
    {
        string url = serverURL + "/api/save-all-scores";
        Debug.Log("📡 Enviando scores a: " + url);
        Debug.Log("   Data: " + jsonData);
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✓ Scores guardados en servidor para: " + username);
                Debug.Log("   Respuesta: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Error al guardar scores para " + username);
                Debug.LogError("   Error: " + request.error);
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("   Respuesta: " + request.downloadHandler.text);
                }
            }
        }
    }
    
    /// <summary>
    /// Desactiva todos los botones del grid para que no se pueda seguir jugando
    /// </summary>
    private void DesactivarTodosBotones()
    {
        for (int i = 0; i < cuadros.Length; i++)
        {
            if (cuadros[i] != null)
            {
                cuadros[i].interactable = false;
            }
        }
    }
    
    /// <summary>
    /// Remueve un símbolo del tablero (utilizado cuando se excede el límite de 3 símbolos por jugador)
    /// </summary>
    private void RemoverSimbolo(int indice)
    {
        if (indice < 0 || indice >= 9)
        {
            Debug.LogError("❌ Índice inválido para remover: " + indice);
            return;
        }
        
        // Marcar la celda como vacía
        estadoCuadros[indice] = 0;
        
        // Actualizar el visual
        if (cuadros[indice] != null)
        {
            // Limpiar el texto del símbolo
            TextMeshProUGUI symbolText = cuadros[indice].GetComponentInChildren<TextMeshProUGUI>();
            if (symbolText != null)
            {
                symbolText.text = "";
            }
            
            // Reactivar el botón para que se pueda volver a usar
            cuadros[indice].interactable = true;
        }
    }
    
    // Clase auxiliar para serializar la solicitud HTTP
    [System.Serializable]
    public class SaveScoresRequest
    {
        public string username;
        public int puntuacion;
        public int nivel;
        public int ranasScore;
        public int bolasScore;
        public int mochilaScore;
        public int laberintoScore;
        public int quizScore;
        public int reinaScore;
        public int tresEnRayaScore;
        public int puzzleScore;
        public int animalesScore;
        public int rutinaScore;
    }
}
