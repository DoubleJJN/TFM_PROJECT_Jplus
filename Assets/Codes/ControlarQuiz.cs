using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Question
{
    public string enunciado;
    public int respuestaCorrecta; // 1, 2 o 3
    public string[] opciones = new string[3];
    public Question(string enunciado, int respuestaCorrecta, string opcion1, string opcion2, string opcion3)
    {
        this.enunciado = enunciado;
        this.respuestaCorrecta = respuestaCorrecta;
        this.opciones[0] = opcion1;
        this.opciones[1] = opcion2;
        this.opciones[2] = opcion3;
    }
}

public class ControlarQuiz : MonoBehaviour
{
    public Question[] preguntas = new Question[5];
    public RawImage []fotos= new RawImage[5];
    public RawImage foto;
    public bool[] preguntasFallidas = new bool[5];
    public TMP_Text textoPregunta;
    public int i = 0;
    void Start()
    {
        preguntas[0] = new Question("¿Cuáles son los movimientos que tiene que hacer el coche para llegar al punto de carga?", 3, "Arriba y derecha", "Abajo e izquierda", "Derecha dos veces");
        preguntas[1] = new Question("¿Qué movimiento debe hacer el astronauta para llegar al cohete?", 1, "Arriba dos veces y Derecha", "Derecha y dos veces arriba", "Tres veces arriba");
        preguntas[2] = new Question("¿Que figura debería aparecer en donde está el robot?", 1, "Estrella", "Triángulo", "Cuadrado");
        preguntas[3] = new Question("¿Qué tiene que hacer la abeja para llegar a la flor?", 2, "Girar 90º a la derecha", "Girar 90º a la izquierda", "Girar 180º");
        preguntas[4] = new Question("¿Cuál de las tres vías puede pasar el tren sin problema?", 3, "Izquierda", "Centro", "Derecha");

        for(int j = 0; j < preguntasFallidas.Length; j++)
        {
            preguntasFallidas[j] = true;
        }

        if (fotos[i] != null)
        {
            textoPregunta.text = preguntas[i].enunciado;
            Instantiate(fotos[i], foto.transform.position, Quaternion.identity, foto.transform);//instancia de foto
        }

    }

    public void CambiarPreguntas(){
        if (i < preguntas.Length)
        {
            if (fotos[i] != null && preguntasFallidas[i] == true)
            {
                textoPregunta.text = preguntas[i].enunciado;
                Instantiate(fotos[i], foto.transform.position, Quaternion.identity, foto.transform);//instancia de foto
            }
        }
    }

    public bool NoHayPreguntasFallidas(){
        for(int k = 0; k < preguntasFallidas.Length; k++)
        {
            if (preguntasFallidas[k] == true){
                return false;
            }
        }
        return true;
    }
}
