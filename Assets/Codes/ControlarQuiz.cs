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
    public Sprite[] imagenes = new Sprite[3]; // Imágenes para cada opción
    public Question(string enunciado, int respuestaCorrecta, string opcion1, string opcion2, string opcion3)
    {
        this.enunciado = enunciado;
        this.respuestaCorrecta = respuestaCorrecta;
        this.opciones[0] = opcion1;
        this.opciones[1] = opcion2;
        this.opciones[2] = opcion3;
    }
    public Question(string enunciado, int respuestaCorrecta, Sprite imagen1, Sprite imagen2, Sprite imagen3)
    {
        this.enunciado = enunciado;
        this.respuestaCorrecta = respuestaCorrecta;
        this.imagenes[0] = imagen1;
        this.imagenes[1] = imagen2;
        this.imagenes[2] = imagen3;
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
        // Cargar imágenes desde Assets/Resources/Photos con formato Quiz_X_Y
        Sprite[] imagenes1 = new Sprite[3] 
        { 
            Resources.Load<Sprite>("Photos/Quiz_1_1"),
            Resources.Load<Sprite>("Photos/Quiz_1_2"),
            Resources.Load<Sprite>("Photos/Quiz_1_3")
        };
        ValidarImagenes("Pregunta 1", imagenes1);
        preguntas[0] = new Question("¿Cuáles son los movimientos que tiene que hacer el coche para llegar al punto de carga?", 3, imagenes1[0], imagenes1[1], imagenes1[2]);

        Sprite[] imagenes2 = new Sprite[3] 
        { 
            Resources.Load<Sprite>("Photos/Quiz_2_1"),
            Resources.Load<Sprite>("Photos/Quiz_2_2"),
            Resources.Load<Sprite>("Photos/Quiz_2_3")
        };
        ValidarImagenes("Pregunta 2", imagenes2);
        preguntas[1] = new Question("¿Qué movimiento debe hacer el astronauta para llegar al cohete?", 1, imagenes2[0], imagenes2[1], imagenes2[2]);

        Sprite[] imagenes3 = new Sprite[3] 
        { 
            Resources.Load<Sprite>("Photos/Quiz_3_1"),
            Resources.Load<Sprite>("Photos/Quiz_3_2"),
            Resources.Load<Sprite>("Photos/Quiz_3_3")
        };
        ValidarImagenes("Pregunta 3", imagenes3);
        preguntas[2] = new Question("¿Que figura debería aparecer en donde está el robot?", 1, imagenes3[0], imagenes3[1], imagenes3[2]);

        Sprite[] imagenes4 = new Sprite[3] 
        { 
            Resources.Load<Sprite>("Photos/Quiz_4_1"),
            Resources.Load<Sprite>("Photos/Quiz_4_2"),
            Resources.Load<Sprite>("Photos/Quiz_4_3")
        };
        ValidarImagenes("Pregunta 4", imagenes4);
        preguntas[3] = new Question("¿Qué tiene que hacer la abeja para llegar a la flor?", 2, imagenes4[0], imagenes4[1], imagenes4[2]);

        Sprite[] imagenes5 = new Sprite[3] 
        { 
            Resources.Load<Sprite>("Photos/Quiz_5_1"),
            Resources.Load<Sprite>("Photos/Quiz_5_2"),
            Resources.Load<Sprite>("Photos/Quiz_5_3")
        };
        ValidarImagenes("Pregunta 5", imagenes5);
        preguntas[4] = new Question("¿Cuál de las tres vías puede pasar el tren sin problema?", 3, imagenes5[0], imagenes5[1], imagenes5[2]);

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

    private void ValidarImagenes(string nombrePregunta, Sprite[] imagenes)
    {
        for (int i = 0; i < imagenes.Length; i++)
        {
            if (imagenes[i] == null)
            {
                Debug.LogError($"❌ Imagen no encontrada: {nombrePregunta} opción {i + 1}");
            }
            else
            {
                Debug.Log($"✓ Imagen cargada: {nombrePregunta} opción {i + 1}");
            }
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
