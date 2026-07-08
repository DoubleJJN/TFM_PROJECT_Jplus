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
    public GameObject imagenBien, imagenMal;
    private bool estaComprobando = false;
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

    /*public void comprobar(int respuesta, Button botonPresionado)
    {
        bool esCorrecto = respuesta == controlarQuiz.preguntas[controlarQuiz.i].respuestaCorrecta;

        if (esCorrecto)
        {
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.green));
            controlarQuiz.preguntasFallidas[controlarQuiz.i] = false;
        }
        else
        {
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
            restartCounter++;
            if(restartCounter > 3)
            {
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }
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
    */
    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Quiz");
        string mensajeGanado = "";
        if (restartCounter == 0)
        {
            mensajeGanado = "¡Felicidades! ¡Has completado todos los niveles sin errores!";
        }
        else if (restartCounter == 1)
        {
            mensajeGanado = "¡Bien hecho! ¡Has completado todos los niveles con solo 1 error!";
        }
        else if (restartCounter == 2)
        {
            mensajeGanado = "¡Lo lograste! Completaste todos los niveles con " + restartCounter + " errores.";
        }
        else
        {
            mensajeGanado = "Cometiste " + restartCounter + " errores. ¡Intenta mejorar!";
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
    public void comprobar(int respuesta, Button botonPresionado)
    {
        // Si ya estamos mostrando una imagen, ignoramos el clic
        if (estaComprobando) return; 

        StartCoroutine(RutinaComprobar(respuesta, botonPresionado));
    }

    IEnumerator RutinaComprobar(int respuesta, Button botonPresionado)
    {
        estaComprobando = true; // Cerramos el candado
        
        bool esCorrecto = respuesta == controlarQuiz.preguntas[controlarQuiz.i].respuestaCorrecta;

        if (esCorrecto)
        {
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.green));
            controlarQuiz.preguntasFallidas[controlarQuiz.i] = false;
            
            // Esperamos 1 segundo mostrando la imagen Bien ANTES de cambiar la pregunta
            yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenBien, 1f));
        }
        else
        {
            StartCoroutine(ChangeButtonColor(botonPresionado, Color.red));
            controlarQuiz.preguntasFallidas[controlarQuiz.i] = true;
            
            // Esperamos 1 segundo mostrando la imagen Mal ANTES de cambiar la pregunta
            yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenMal, 1f));
        }

        // --- A PARTIR DE AQUÍ CAMBIAMOS A LA SIGUIENTE PREGUNTA ---
        controlarQuiz.i++;
        
        if (controlarQuiz.i < controlarQuiz.preguntas.Length)
        {
            controlarQuiz.CambiarPreguntas();
            ActualizarImagenesBotones();
        }

        // Saltamos las preguntas que ya estaban correctas
        while (controlarQuiz.i < controlarQuiz.preguntasFallidas.Length && controlarQuiz.preguntasFallidas[controlarQuiz.i] == false)
        {
            controlarQuiz.i++;
            if (controlarQuiz.i < controlarQuiz.preguntas.Length)
            {
                controlarQuiz.CambiarPreguntas();
                ActualizarImagenesBotones();
            }
        }

        Debug.Log(controlarQuiz.i);

        if (controlarQuiz.NoHayPreguntasFallidas())
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
        }
        else if (controlarQuiz.i >= controlarQuiz.preguntas.Length)
        {
            restartCounter++;
            if(restartCounter == 3)
            {
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }
            else
            {
                // Buscamos la primera fallida para repetirla
                for(int j = 0; j < controlarQuiz.preguntasFallidas.Length; j++)
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

        estaComprobando = false; // Abrimos el candado para la siguiente pregunta
    }

    IEnumerator MostrarImagenTiempoLimitado(GameObject imagen, float duracion)
    {
        if (imagen != null)
        {
            imagen.SetActive(true);
            yield return new WaitForSeconds(duracion);
            imagen.SetActive(false);
        }
    }
}