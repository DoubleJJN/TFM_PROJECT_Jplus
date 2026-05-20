using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AyudaJugar : MonoBehaviour
{
    public Button comoJugar;
    public MoverRana resetearRana;
    public JuegoDeLaMochila resetearMochila;
    public MovimientoEnLaberinto resetearLaberinto;
    public JuegoDeQuiz resetearQuiz;
    public OrdenarBolasListas resetearOrdenar;
    public JuegoDeReinas resetearAjedrez;
    // Start is called before the first frame update
    void Start()
    {
        comoJugar.onClick.AddListener(Ayuda);
    }

    void Ayuda()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "GameRanas")
        {
            int id = 0; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            resetearRana.SetValor(true);
            SceneManager.LoadScene("AyudaRana"); // Cambia a la siguiente escena
        }
        else if (sceneName == "GameMochila")
        {
            int id = 1; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            JuegoDeLaMochila.darComoJugar();
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
        }
        else if (sceneName == "GameAjedrez")
        {
            int id = 5; // Tu ID aquí
            PlayerPrefs.SetInt("ID", id); // Almacena el ID en PlayerPrefs
            resetearAjedrez.SetValor(true);
            SceneManager.LoadScene("AyudaAjedrez"); // Cambia a la siguiente escena
        }
    }
}
