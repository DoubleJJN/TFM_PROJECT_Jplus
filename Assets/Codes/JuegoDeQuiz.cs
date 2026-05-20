using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class JuegoDeQuiz : MonoBehaviour
{
    public PopUpGanar popUpGanar;
    public Button btn1, btn2, btn3;
    public TextMeshProUGUI txt1, txt2, txt3;
    public ControlarQuiz controlarQuiz;
    public int restartCounter = 0;
    void Start()
    {
        controlarQuiz = GameObject.Find("Panel").GetComponent<ControlarQuiz>();
        btn1 = GameObject.Find("Respuesta1").GetComponent<Button>();
        btn2 = GameObject.Find("Respuesta2").GetComponent<Button>();
        btn3 = GameObject.Find("Respuesta3").GetComponent<Button>();
        
        txt1 = btn1.GetComponentInChildren<TextMeshProUGUI>();
        txt2 = btn2.GetComponentInChildren<TextMeshProUGUI>();
        txt3 = btn3.GetComponentInChildren<TextMeshProUGUI>();

        btn1.onClick.AddListener(() =>
        {
            comprobar(1, btn1);
        });

        btn2.onClick.AddListener(() =>
        {
            comprobar(2, btn2);
        });

        btn3.onClick.AddListener(() =>
        {
            comprobar(3, btn3);
        });
        
        StartCoroutine(ActualizarTextosBotonesConDelay());
    }

    IEnumerator ActualizarTextosBotonesConDelay()
    {
        yield return null; // Espera un frame para asegurar que todo está inicializado
        ActualizarTextosBotones();
    }

    void ActualizarTextosBotones()
    {
        if (txt1 == null || txt2 == null || txt3 == null)
        {
            Debug.LogError("TextMeshProUGUI no están asignados correctamente");
            return;
        }
        
        if (controlarQuiz != null && controlarQuiz.i < controlarQuiz.preguntas.Length)
        {
            txt1.text = controlarQuiz.preguntas[controlarQuiz.i].opciones[0];
            txt2.text = controlarQuiz.preguntas[controlarQuiz.i].opciones[1];
            txt3.text = controlarQuiz.preguntas[controlarQuiz.i].opciones[2];
            
            // Asegurar que el texto sea visible
            txt1.color = Color.black;
            txt2.color = Color.black;
            txt3.color = Color.black;
        }
        else
        {
            Debug.LogError("controlarQuiz no está inicializado o índice fuera de rango");
        }
    }

    public void comprobar(int respuesta, Button botonPresionado)
    {
        bool esCorrecto = respuesta == controlarQuiz.preguntas[controlarQuiz.i].respuestaCorrecta;

        if (esCorrecto)
        {
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.green));
            controlarQuiz.preguntasFallidas[controlarQuiz.i] = false;
        }
        else
        {
            restartCounter++;
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.red));
            controlarQuiz.preguntasFallidas[controlarQuiz.i] = true;
        }
        controlarQuiz.i++;
        controlarQuiz.CambiarPreguntas();
        ActualizarTextosBotones();
        while (controlarQuiz.i < controlarQuiz.preguntasFallidas.Length && controlarQuiz.preguntasFallidas[controlarQuiz.i] == false)
        {
            controlarQuiz.i++;
            controlarQuiz.CambiarPreguntas();
            ActualizarTextosBotones();
        }

        Debug.Log(controlarQuiz.i);

        if (controlarQuiz.NoHayPreguntasFallidas())
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
        }else if (controlarQuiz.i == controlarQuiz.preguntas.Length)
        {
            for(int j = 0; j<controlarQuiz.preguntasFallidas.Length; j++)
            {
                if(controlarQuiz.preguntasFallidas[j] == true)
                {
                    controlarQuiz.i = j;
                    break;
                }
            }
            controlarQuiz.CambiarPreguntas();
            ActualizarTextosBotones();
        }
    }

    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Quiz");
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
        controlarQuiz.i = 0;
        for(int k = 0; k<controlarQuiz.preguntasFallidas.Length; k++)
        {
            controlarQuiz.preguntasFallidas[k] = true;
        }
        controlarQuiz.CambiarPreguntas();
        ActualizarTextosBotones();
    }
}