using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AyudaJugar : MonoBehaviour
{
    public Button comoJugar;
    
    // Paneles de ayuda
    public GameObject panelAyudaRana;
    public GameObject panelAyudaMochila;
    public GameObject panelAyudaLaberinto;
    public GameObject panelAyudaQuiz;
    public GameObject panelAyudaOrdenar;
    public GameObject panelAyudaAjedrez;
    public GameObject panelAyudaTresEnRaya;
    public GameObject panelAyudaPuzzle;
    public GameObject panelAyudaAnimales;
    public GameObject panelAyudaRutina;
    
    // Botones para cerrar ayuda
    public Button btnQuitarAyudaRana;
    public Button btnQuitarAyudaMochila;
    public Button btnQuitarAyudaLaberinto;
    public Button btnQuitarAyudaQuiz;
    public Button btnQuitarAyudaOrdenar;
    public Button btnQuitarAyudaAjedrez;
    public Button btnQuitarAyudaTresEnRaya;
    public Button btnQuitarAyudaPuzzle;
    public Button btnQuitarAyudaAnimales;
    public Button btnQuitarAyudaRutina;
    
    // Start is called before the first frame update
    void Start()
    {
        comoJugar.onClick.AddListener(Ayuda);
        
        // Agregar listeners a los botones de cerrar ayuda
        if (btnQuitarAyudaRana != null)
            btnQuitarAyudaRana.onClick.AddListener(() => OcultarAyuda(panelAyudaRana));
        if (btnQuitarAyudaMochila != null)
            btnQuitarAyudaMochila.onClick.AddListener(() => OcultarAyuda(panelAyudaMochila));
        if (btnQuitarAyudaLaberinto != null)
            btnQuitarAyudaLaberinto.onClick.AddListener(() => OcultarAyuda(panelAyudaLaberinto));
        if (btnQuitarAyudaQuiz != null)
            btnQuitarAyudaQuiz.onClick.AddListener(() => OcultarAyuda(panelAyudaQuiz));
        if (btnQuitarAyudaOrdenar != null)
            btnQuitarAyudaOrdenar.onClick.AddListener(() => OcultarAyuda(panelAyudaOrdenar));
        if (btnQuitarAyudaAjedrez != null)
            btnQuitarAyudaAjedrez.onClick.AddListener(() => OcultarAyuda(panelAyudaAjedrez));
        if (btnQuitarAyudaTresEnRaya != null)
            btnQuitarAyudaTresEnRaya.onClick.AddListener(() => OcultarAyuda(panelAyudaTresEnRaya));
        if (btnQuitarAyudaPuzzle != null)
            btnQuitarAyudaPuzzle.onClick.AddListener(() => OcultarAyuda(panelAyudaPuzzle));
        if (btnQuitarAyudaAnimales != null)
            btnQuitarAyudaAnimales.onClick.AddListener(() => OcultarAyuda(panelAyudaAnimales));
        if (btnQuitarAyudaRutina != null)
            btnQuitarAyudaRutina.onClick.AddListener(() => OcultarAyuda(panelAyudaRutina));
    }

    void Ayuda()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "GameRanas")
        {
            // Solo mostrar el panel, no resetear
            if (panelAyudaRana != null)
                panelAyudaRana.SetActive(true);
        }
        else if (sceneName == "GameMochila")
        {
            // JuegoDeLaMochila.darComoJugar();  // Solo mostrar el panel, no ejecutar acciones
            if (panelAyudaMochila != null)
                panelAyudaMochila.SetActive(true);
        }
        else if (sceneName == "GameLaberinto")
        {
            // Solo mostrar el panel, no resetear
            if (panelAyudaLaberinto != null)
                panelAyudaLaberinto.SetActive(true);
        }
        else if (sceneName == "GameQuiz")
        {
            // Solo mostrar el panel, no resetear
            if (panelAyudaQuiz != null)
                panelAyudaQuiz.SetActive(true);
        }
        else if (sceneName == "GameOrdenar")
        {
            if (panelAyudaOrdenar != null)
                panelAyudaOrdenar.SetActive(true);
        }
        else if (sceneName == "GameAjedrez")
        {
            // Solo mostrar el panel, no resetear
            if (panelAyudaAjedrez != null)
                panelAyudaAjedrez.SetActive(true);
        }
        else if (sceneName == "GameTresEnRaya")
        {
            if (panelAyudaTresEnRaya != null)
                panelAyudaTresEnRaya.SetActive(true);
        }
        else if (sceneName == "GamePuzzle")
        {
            if (panelAyudaPuzzle != null)
                panelAyudaPuzzle.SetActive(true);
        }
        else if (sceneName == "GameAnimales")
        {
            if (panelAyudaAnimales != null)
                panelAyudaAnimales.SetActive(true);
        }
        else if (sceneName == "GameRutina")
        {
            if (panelAyudaRutina != null)
                panelAyudaRutina.SetActive(true);
        }
    }
    
    void OcultarAyuda(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        
        // Log para debug
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Panel cerrado. Escena actual: " + currentScene.name);
    }
}
