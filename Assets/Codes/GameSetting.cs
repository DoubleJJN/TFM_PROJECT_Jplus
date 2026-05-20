using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    public GameObject popUp;
    public MoverRana resetear;
    public JuegoDeLaMochila resetearMochila;
    public MovimientoEnLaberinto resetearLaberinto;
    public JuegoDeQuiz resetearQuiz;
    public OrdenarBolasListas resetearOrdenar;
    public JuegoDeReinas resetearAjedrez;
    public Button btnReiniciar; // Haz estos campos públicos
   // public Button btnAyuda;
    public Button btnCancelar;
    public Button btnSalir;

    public Button CerrarSesion;
    void Start()
    {
        // Asigna las funciones a los botones (con verificación)
        if (btnReiniciar != null) btnReiniciar.onClick.AddListener(Reiniciar);
        if (btnCancelar != null) btnCancelar.onClick.AddListener(Cancelar);
        if (btnSalir != null) btnSalir.onClick.AddListener(Salir);
        if (CerrarSesion != null) CerrarSesion.onClick.AddListener(CerrarSesionClick);
    }

    void Cancelar(){
        OcultarPopUp();
    }

    void Ayuda(){
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "GameRanas")
        {
            int id = 0; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            resetear.SetValor(true);
            SceneManager.LoadScene("AyudaRana"); // Cambia a la siguiente escena
        }
        else if (sceneName == "GameMochila")
        {
            int id = 1; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            resetearMochila.SetValor1(true);
            SceneManager.LoadScene("AyudaMochila"); // Cambia a la siguiente escena
        }
        else if (sceneName == "GameLaberinto")
        {
            int id = 2; // Tu ID aquí
            resetearLaberinto.SetValor(true);
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            
            SceneManager.LoadScene("AyudaLaberinto"); // Cambia a la siguiente escena
        }
        else if (sceneName == "GameQuiz")
        {
            int id = 3; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            resetearQuiz.SetValor(true);
            SceneManager.LoadScene("AyudaQuiz"); // Cambia a la siguiente escena
        }
        else if (sceneName == "GameOrdenar")
        {
            int id = 4; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            SceneManager.LoadScene("AyudaOrdenar"); // Cambia a la siguiente escena
        } else if (sceneName == "GameAjedrez")
        {
            int id = 5; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            resetearAjedrez.SetValor(true);
            SceneManager.LoadScene("AyudaAjedrez"); // Cambia a la siguiente escena
        }
            
    }

    public void Reiniciar(){
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if(sceneName == "GameRanas"|| sceneName == "AyudaRana")
            resetear.SetValor(true);
        if (sceneName == "GameMochila")
            resetearMochila.SetValor(true);
        if(sceneName == "GameLaberinto")
            resetearLaberinto.SetValor(true);
        if(sceneName == "GameQuiz")
            resetearQuiz.SetValor(true);
        if(sceneName == "GameOrdenar")
            resetearOrdenar.SetValor(true);
        if(sceneName == "GameAjedrez")
            resetearAjedrez.SetValor(true);
            
        /*else if(sceneName == "GameMochila")
            Debug.Log("Reiniciar mochila");
            //resetearMochila.SetValor(true);*/
        OcultarPopUp();
    }

    void Salir(){
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "GameRanas" || sceneName == "AyudaRana")
            resetear.SetValor(true);
        if (sceneName == "GameMochila")
            resetearMochila.SetValor(true);
        if (sceneName == "GameLaberinto")
            resetearLaberinto.SetValor(true);
        if (sceneName == "GameQuiz")
            resetearQuiz.SetValor(true);
        if (sceneName == "GameOrdenar")
            resetearOrdenar.SetValor(true);
        if (sceneName == "GameAjedrez")
            resetearAjedrez.SetValor(true);
        SceneManager.LoadScene("MenuJuego");
    }

    public void MostrarPopUp(){
        if (popUp != null)
            popUp.SetActive(true);
        else
            Debug.LogWarning("⚠️ PopUp no asignado en GameSetting");
    }

    void OcultarPopUp(){
        if (popUp != null)
            popUp.SetActive(false);
        else
            Debug.LogWarning("⚠️ PopUp no asignado en GameSetting");
    }

    void CerrarSesionClick()
    {
        // Cerrar el popup
        OcultarPopUp();
        
        // Limpiar los datos de sesión
        PlayerPrefs.DeleteKey("EstaLogueado");
        PlayerPrefs.DeleteKey("UsuarioLogueado");
        PlayerPrefs.Save();
        
        // Recargar la escena actual para refrescar los datos
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
