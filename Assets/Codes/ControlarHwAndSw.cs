using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Question
{
    public string enunciado;
    public bool respuesta;
    public Question(string enunciado, bool respuesta)
    {
        this.enunciado = enunciado;
        this.respuesta = respuesta;
        //this.foto = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity); 
    }
}

public class ControlarHwAndSw : MonoBehaviour
{
    public Question[] preguntas = new Question[5];
    public RawImage []fotos= new RawImage[5];
    public RawImage foto;
    public bool[] preguntasFallidas = new bool[5];
    public TMP_Text textoPregunta;
    public int i = 0;
    void Start()
    {
        preguntas[0] = new Question("¿Es un hardware el ratón?", true);
        preguntas[1] = new Question("¿Es un software la memoria RAM?", false);
        preguntas[2] = new Question("¿Es un software el sistema operativo?", true);
        preguntas[3] = new Question("¿Es un software un teléfono móvil?", false);
        preguntas[4] = new Question("¿Es un software un ordenador?", false);

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
