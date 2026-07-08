using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PopUpGanar : MonoBehaviour
{
    public GameObject popUpGanado;
    public Button btnSalir;
    static int totalStars=3;
    public GameObject[] stars = new GameObject[totalStars];
    public TextMeshProUGUI mensaje;
    private UserManager userManager;
    private string nombreJuegoActual;
    
    //public MoverRana resetear;
    void Start(){
        btnSalir.onClick.AddListener(Salir);
        userManager = FindFirstObjectByType<UserManager>();
        for(int i=0; i<totalStars; i++)
            stars[i] = GameObject.Find("Star"+(i+1));
    }

    public void SetNombreJuego(string nombre)
    {
        nombreJuegoActual = nombre;
    }

    // Update is called once per frame
    void Salir(){
        //resetear.SetValor(true);
        SceneManager.LoadScene("MenuJuego");
    }

    public void MostrarPopUpGanado(int restartCounter, string mensajeGanado)
    {
        popUpGanado.SetActive(true);
        mensaje.text = mensajeGanado;
        // Buscar las estrellas en el momento de mostrar el popup
        for(int i=0; i<totalStars; i++)
        {
            stars[i] = GameObject.Find("Star"+(i+1));
            UnityEngine.Debug.Log("Buscando Star"+(i+1)+": " + (stars[i] != null ? "Encontrada" : "NO encontrada"));
        }
        
        // Calcular estrellas conseguidas
        int estrellasConseguidas = CalcularEstrellas(restartCounter);
        
        // Activar todas las estrellas primero
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(true);
        }
        
        // Luego ocultar según el restartCounter
        if (restartCounter == 0)
        {
            // No oculte nada - 3 estrellas
            UnityEngine.Debug.Log("Mostrando 3 estrellas");
        }
        else if (restartCounter == 1)
        {
            // Oculte la última estrella - 2 estrellas
            if (stars[2] != null)
            {
                stars[2].SetActive(false);
                UnityEngine.Debug.Log("Ocultando Star3, mostrando 2 estrellas");
            }
        }
        else if(restartCounter == 2)
        {
            // Oculte la penúltima y la última - 1 estrella
            if (stars[1] != null)
                stars[1].SetActive(false);
            if (stars[2] != null)
                stars[2].SetActive(false);
            UnityEngine.Debug.Log("Ocultando Star2 y Star3, mostrando 1 estrella");
        }
        else
        {
            if (stars[1] != null)
                stars[1].SetActive(false);
            if (stars[2] != null)
                stars[2].SetActive(false);
            if (stars[0] != null)
                stars[0].SetActive(false);
            UnityEngine.Debug.Log("Ocultando todas");
        }

        // Guardar las estrellas directamente en el usuario actual
        // (Solo si no es TresEnRaya, que ya lo hace desde BotonPrueba())
        if (nombreJuegoActual != "TresEnRaya")
        {
            if (userManager == null)
                userManager = FindFirstObjectByType<UserManager>();

            if (userManager != null)
            {
                string usuarioActual = userManager.GetCurrentUser();
                if (!string.IsNullOrEmpty(usuarioActual))
                {
                    userManager.UpdateGameScore(usuarioActual, nombreJuegoActual, estrellasConseguidas);
                }
            }

            if (TotalStarsCounter.instance != null)
                TotalStarsCounter.instance.SincronizarPuntuacionUsuario();
        }
    }

    private int CalcularEstrellas(int restartCounter)
    {
        if (restartCounter == 0)
            return 3;
        else if (restartCounter == 1)
            return 2;
        else if (restartCounter ==2)
            return 1;
        else
            return 0;
    }

    void OcultarPopUpGanado()
    {
        popUpGanado.SetActive(false);
    }
}
