using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ManejoLogin : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;
    public Button loginButton;
    public Button registerButton;
    public Button forgotButton;
    public TMP_Text titleText;

    private UserManager userManager;
    private bool isRegisterMode = false; // Variable para saber si estamos en modo registro
    private bool isForgotPasswordMode = false; // Variable para saber si estamos en modo olvidé contraseña
    private Vector3 loginButtonOriginalPos;
    private Vector3 registerButtonOriginalPos;
    private Vector3 forgotButtonOriginalPos;

    void Start()
    {
        // Establecer el booleano de login a falso al cargar la pantalla
        PlayerPrefs.SetInt("EstaLogueado", 0);
        
        // Guardas las posiciones originales de los botones
        loginButtonOriginalPos = loginButton.transform.localPosition;
        registerButtonOriginalPos = registerButton.transform.localPosition;
        forgotButtonOriginalPos = forgotButton.transform.localPosition;
        
        // Establecer el título inicial
        if (titleText != null)
            titleText.text = "Iniciar sesión";
        
        // Buscar el UserManager en la escena
        userManager = FindFirstObjectByType<UserManager>();

        // Conectar botones
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        forgotButton.onClick.AddListener(OnForgotPasswordClicked);
    }

    void ShowFeedback(string message)
    {
        feedbackText.text = message;
        StartCoroutine(ClearFeedbackAfterDelay());
    }

    IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        feedbackText.text = "";
    }

    void OnLoginClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        // Si estamos en modo olvidé contraseña
        if (isForgotPasswordMode)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowFeedback("Rellena todos los campos.");
                return;
            }

            // Cambiar la contraseña del usuario
            bool success = userManager.ChangePassword(username, password);

            if (success)
            {
                ShowFeedback("Contraseña actualizada. Vuelve a iniciar sesión.");
                
                // Restaurar la vista original
                isForgotPasswordMode = false;
                loginButton.gameObject.SetActive(true);
                registerButton.gameObject.SetActive(true);
                loginButton.transform.localPosition = loginButtonOriginalPos;
                registerButton.transform.localPosition = registerButtonOriginalPos;
                forgotButton.transform.localPosition = forgotButtonOriginalPos;
                
                // Restaurar el título
                if (titleText != null)
                    titleText.text = "Iniciar sesión";
                
                // Limpiar campos
                usernameInput.text = "";
                passwordInput.text = "";
            }
            else
            {
                ShowFeedback("Usuario no encontrado.");
            }
            return;
        }

        // Login normal
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Rellena todos los campos.";
            return;
        }

        if (userManager.Login(username, password))
        {
            ShowFeedback("Cargando juego...");
            // Guardar el nombre de usuario y establecer el booleano a verdadero
            PlayerPrefs.SetString("UsuarioLogueado", username);
            PlayerPrefs.SetInt("EstaLogueado", 1);
            
            // Sincronizar la puntuación del usuario
            if (TotalStarsCounter.instance != null)
            {
                TotalStarsCounter.instance.SincronizarPuntuacionUsuario();
            }
            
            // Cambio de escena al menú principal después del login exitoso
            SceneManager.LoadScene("MenuInicial"); 
        }
        else
        {
            ShowFeedback("Usuario o contraseña incorrectos.");
        }
    }

    void OnRegisterClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (!isRegisterMode)
        {
            // Primera vez que presiona Registrar: cambiar a modo registro
            isRegisterMode = true;
            loginButton.gameObject.SetActive(false);
            forgotButton.gameObject.SetActive(false);
            
            // Limpiar campos
            usernameInput.text = "";
            passwordInput.text = "";
            
            // Mover el botón de registrar al medio (entre ambas posiciones)
            registerButton.transform.localPosition = (loginButtonOriginalPos + registerButtonOriginalPos) / 2;
            
            // Cambiar el título
            if (titleText != null)
                titleText.text = "Registrar cuenta";
            
            ShowFeedback("Ingresa tu usuario y contraseña");
            return;
        }

        // Segunda vez que presiona Registrar: procesar el registro
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowFeedback("Rellena todos los campos.");
            return;
        }

        bool success = userManager.Register(username, password);

        if (success)
        {
            ShowFeedback("Usuario registrado correctamente.");
            
            // Restaurar la vista original
            isRegisterMode = false;
            loginButton.gameObject.SetActive(true);
            forgotButton.gameObject.SetActive(true);
            loginButton.transform.localPosition = loginButtonOriginalPos;
            registerButton.transform.localPosition = registerButtonOriginalPos;
            
            // Restaurar el título
            if (titleText != null)
                titleText.text = "Iniciar sesión";
            
            // Limpiar campos
            usernameInput.text = "";
            passwordInput.text = "";
            ShowFeedback("Ahora puedes iniciar sesión.");
        }
        else
        {
            ShowFeedback("El usuario ya existe.");
        }
    }

    void OnForgotPasswordClicked()
    {
        if (!isForgotPasswordMode)
        {
            // Primera vez que presiona Olvidado: cambiar a modo olvidé contraseña
            isForgotPasswordMode = true;
            registerButton.gameObject.SetActive(false);
            forgotButton.gameObject.SetActive(false);
            
            // Limpiar campos
            usernameInput.text = "";
            passwordInput.text = "";
            
            // Mover el botón login al medio
            loginButton.transform.localPosition = (loginButtonOriginalPos + registerButtonOriginalPos) / 2;
            
            // Cambiar el título
            if (titleText != null)
                titleText.text = "Recuperar cuenta";
            
            ShowFeedback("Ingresa tu usuario y nueva contraseña");
            return;
        }
    }
}