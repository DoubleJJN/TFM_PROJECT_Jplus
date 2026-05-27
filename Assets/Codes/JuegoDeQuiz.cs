using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class JuegoDeQuiz : MonoBehaviour
{
    public PopUpGanar popUpGanar;
    public Button btn1, btn2, btn3;
    public Image img1, img2, img3; // Componentes Image para mostrar sprites
    public ControlarQuiz controlarQuiz;
    public int restartCounter = 0;
    void Start()
    {
        controlarQuiz = GameObject.Find("Panel").GetComponent<ControlarQuiz>();
        btn1 = GameObject.Find("Respuesta1").GetComponent<Button>();
        btn2 = GameObject.Find("Respuesta2").GetComponent<Button>();
        btn3 = GameObject.Find("Respuesta3").GetComponent<Button>();
        
        img1 = btn1.GetComponent<Image>();
        img2 = btn2.GetComponent<Image>();
        img3 = btn3.GetComponent<Image>();

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
        
        StartCoroutine(ActualizarImagenesBotonesConDelay());
    }

    IEnumerator ActualizarImagenesBotonesConDelay()
    {
        yield return null; // Espera un frame para asegurar que todo está inicializado
        ActualizarImagenesBotones();
    }

    void ActualizarImagenesBotones()
    {
        if (img1 == null || img2 == null || img3 == null)
        {
            Debug.LogError("Image components no están asignados correctamente");
            return;
        }
        
        if (controlarQuiz != null && controlarQuiz.i < controlarQuiz.preguntas.Length)
        {
            if (controlarQuiz.preguntas[controlarQuiz.i].imagenes[0] != null)
                img1.sprite = controlarQuiz.preguntas[controlarQuiz.i].imagenes[0];
            if (controlarQuiz.preguntas[controlarQuiz.i].imagenes[1] != null)
                img2.sprite = controlarQuiz.preguntas[controlarQuiz.i].imagenes[1];
            if (controlarQuiz.preguntas[controlarQuiz.i].imagenes[2] != null)
                img3.sprite = controlarQuiz.preguntas[controlarQuiz.i].imagenes[2];
            
            // Asegurar que las imágenes sean visibles
            img1.color = Color.white;
            img2.color = Color.white;
            img3.color = Color.white;
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
        ActualizarImagenesBotones();
        while (controlarQuiz.i < controlarQuiz.preguntasFallidas.Length && controlarQuiz.preguntasFallidas[controlarQuiz.i] == false)
        {
            controlarQuiz.i++;
            controlarQuiz.CambiarPreguntas();
            ActualizarImagenesBotones();
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
            ActualizarImagenesBotones();
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
        ActualizarImagenesBotones();
    }
}