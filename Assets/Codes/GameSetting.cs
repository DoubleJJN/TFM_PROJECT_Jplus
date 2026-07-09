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
    public JuegoDeAnimales resetearAnimales;
    public JuegoDeRutina resetearRutina;
    public JuegoDePuzzle resetearPuzzle;
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

    public void Reiniciar(){
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if(sceneName == "GameRanas")
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
            resetearAjedrez.SetValor(true, true);
        if(sceneName == "GameAnimales")
            resetearAnimales.SetValor(true);
        if(sceneName == "GameRutina")
            resetearRutina.SetValor(true);
        if(sceneName == "GamePuzzle")
            resetearPuzzle.SetValor(true);
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
            resetearAjedrez.SetValor(true, true);
        if (sceneName == "GameAnimales")
            resetearAnimales.SetValor(true);
        if(sceneName == "GameRutina")
            resetearRutina.SetValor(true);
        if(sceneName == "GamePuzzle")
            resetearPuzzle.SetValor(true);
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
