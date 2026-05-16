using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JuegoDeHwYSw : MonoBehaviour
{
    public PopUpGanar popUpGanar;
    public Button btnSi, btnNo;
    public ControlarHwAndSw controlarHwAndSw;
    public int restartCounter = 0;
    void Start()
    {
        controlarHwAndSw = GameObject.Find("Panel HW&SW").GetComponent<ControlarHwAndSw>();
        btnSi = GameObject.Find("Si").GetComponent<Button>();
        btnNo = GameObject.Find("No").GetComponent<Button>();

        btnSi.onClick.AddListener(() =>
        {
            comprobar(true,btnSi);
        });

        btnNo.onClick.AddListener(() =>
        {
            comprobar(false,btnNo);
        });
    }

    public void comprobar(bool respuesta, Button botonPresionado)
    {
        bool esCorrecto = respuesta == controlarHwAndSw.preguntas[controlarHwAndSw.i].respuesta;

        if (esCorrecto)
        {
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.green));
            controlarHwAndSw.preguntasFallidas[controlarHwAndSw.i] = false;
        }
        else
        {
            restartCounter++;
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.red));
            controlarHwAndSw.preguntasFallidas[controlarHwAndSw.i] = true;
        }
        controlarHwAndSw.i++;
        controlarHwAndSw.CambiarPreguntas();
        while (controlarHwAndSw.i < controlarHwAndSw.preguntasFallidas.Length && controlarHwAndSw.preguntasFallidas[controlarHwAndSw.i] == false)
        {
            controlarHwAndSw.i++;
            controlarHwAndSw.CambiarPreguntas();
        }

        Debug.Log(controlarHwAndSw.i);

        if (controlarHwAndSw.NoHayPreguntasFallidas())
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
        }else if (controlarHwAndSw.i == controlarHwAndSw.preguntas.Length)
        {
            for(int j = 0; j<controlarHwAndSw.preguntasFallidas.Length; j++)
            {
                if(controlarHwAndSw.preguntasFallidas[j] == true)
                {
                    controlarHwAndSw.i = j;
                    break;
                }
            }
            controlarHwAndSw.CambiarPreguntas();
        }
    }

    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("HardwareSoftware");
        string mensajeGanado = "";
        if (restartCounter == 0)
        {
            mensajeGanado = "¡Felicidades! Has ganado sin cometer errores.";
        }
        else if (restartCounter == 1)
        {
            mensajeGanado = "¡Bien hecho! Has ganado con solo un error.";
        }
        else
        {
            mensajeGanado = "Has ganado, pero cometiste algunos errores. ¡Sigue practicando!";
        }
        popUpGanar.MostrarPopUpGanado(restartCounter, mensajeGanado);
    }

    IEnumerator ChangeButtonColor(Button button, Color color)
    {
        button.image.color = color;
        yield return new WaitForSeconds(0.1f);
        button.image.color = Color.white;
    }

    public void SetValor(bool valor)
    {
        controlarHwAndSw.i = 0;
        for(int k = 0; k<controlarHwAndSw.preguntasFallidas.Length; k++)
        {
            controlarHwAndSw.preguntasFallidas[k] = true;
        }
        controlarHwAndSw.CambiarPreguntas();
    }
}