using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
public class Logging : MonoBehaviour
{
    public Button usuario;
    public TMP_Text usernameTMP; // Componente de texto para mostrar el username
    public GameObject popUp;
    private bool estaLogueado; // Variable de clase para almacenar el estado de login
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        usuario = GameObject.Find("User").GetComponent<Button>();
        usuario.onClick.AddListener(Usuario);
        
        // Buscar el componente UserName y mostrar el nombre del usuario logueado
        usernameTMP = GameObject.Find("UserName").GetComponent<TMP_Text>();
        
        // Verificar si el usuario está logueado usando el booleano
        estaLogueado = PlayerPrefs.GetInt("EstaLogueado", 0) == 1;
        
        if (usernameTMP != null)
        {
            if (estaLogueado)
            {
                string usuarioLogueado = PlayerPrefs.GetString("UsuarioLogueado");
                usernameTMP.text = usuarioLogueado;
            }
            else
            {
                // Si no está logueado, limpiar los datos
                PlayerPrefs.DeleteKey("UsuarioLogueado");
                PlayerPrefs.SetInt("EstaLogueado", 0);
                usernameTMP.text = "Iniciar"; // Mostrar "Iniciar" si no está logueado
            }
        }
    }

    void Usuario(){ 
        if(estaLogueado)
        {
            if(popUp != null)
            {
                popUp.SetActive(true);
            }
        } 
        else 
        {
            SceneManager.LoadScene("MenuLogin"); 
        }
    }
}
