using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ControlBotones : MonoBehaviour
{
    public Button jugar;
    public Button ayuda;
    public Button salir;
    public GameObject popUpAntesJugar;
    public Button btnOkPopUp;
    
    void Start()
    {
        jugar = GameObject.Find("Jugar").GetComponent<Button>();
        ayuda = GameObject.Find("Acerca de").GetComponent<Button>();
        salir = GameObject.Find("Salir").GetComponent<Button>();
        jugar.onClick.AddListener(Jugar);
        ayuda.onClick.AddListener(Ayuda);
        salir.onClick.AddListener(Salir);
        
        // Asignar el botón Ok del popUp
        if (btnOkPopUp != null)
        {
            btnOkPopUp.onClick.AddListener(OcultarPopUp);
        }
    }

    void Jugar(){
        // Verificar si el usuario está logueado
        int estaLogueado = PlayerPrefs.GetInt("EstaLogueado", 0);
        
        if (estaLogueado == 1)
        {
            // Si está logueado, cargar la escena del menú de juego
            SceneManager.LoadScene("MenuJuego");
        }
        else
        {
            // Si no está logueado, mostrar el popUp de aviso
            if (popUpAntesJugar != null)
            {
                popUpAntesJugar.SetActive(true);
            }
        }
    }
    void Ayuda(){ SceneManager.LoadScene("MenuAyuda"); }
    void Salir()
    {
        #if UNITY_EDITOR
            // Si estamos en el editor de Unity
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si estamos en una build del juego
            Application.Quit();
        #endif 
    }

    void OcultarPopUp()
    {
        if (popUpAntesJugar != null)
        {
            popUpAntesJugar.SetActive(false);
        }
    }
}
