using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OrdenarBolasListas : MonoBehaviour
{
    public Button[] bolas= new Button[6];
    public RectTransform[] destinosDeBolas =new RectTransform[6];
    //public Dictionary<Button, Transform> mapaDestinos = new Dictionary<Button, Transform>();
    public PopUpGanar popUpGanar;
    public int restartCounter=0;
    public TMP_Text pasosRestantes;
    public TMP_Text niveles;
    public int nivel = 1;
    public int pasos = 6;
    public Dictionary<int, int[]> mapaNiveles = new Dictionary<int, int[]>();
    int[] posicionesActuales = new int[6];
    int[] objetivoFinal = { 0, 1, 2, 3, 4, 5 };
    public GameObject imagenBien, imagenMal;
    private bool bloqueado = false;

    void Start()
    {
        mapaNiveles.Add(1, new int[] { 0, 2, 1, 4, 5, 3 });
        mapaNiveles.Add(2, new int[] { 2, 0, 3, 1, 5, 4 });
        mapaNiveles.Add(3, new int[] { 4, 1, 2, 0, 3, 5 });
        for(int i = 0; i < posicionesActuales.Length; i++)
        {
            posicionesActuales[i] = mapaNiveles[nivel][i];
        }
        niveles.text = "Nivel: " + nivel;
        pasosRestantes.text = "Pasos restantes: " + pasos;
        for (int i = 0; i < bolas.Length; i++)
        {
            GameObject bolaGameObject = GameObject.Find("Bola" + (i + 1));
            if (bolaGameObject != null && destinosDeBolas[i] != null)
            {
                bolas[i] = bolaGameObject.GetComponent<Button>();
                destinosDeBolas[i] = GameObject.Find("DestinoBola" + (i + 1)).GetComponent<RectTransform>();
            }
        }
        
        ColocarBolas();

        Button bolaSeleccionada1 = null;
        Button bolaSeleccionada2 = null;

        int indexBola1 = -1, indexBola2 = -1;
        for (int i = 0; i < bolas.Length; i++)
        {
            int index = i;
            bolas[i].onClick.AddListener(() =>
            {
                if (bolaSeleccionada1 == null)
                {
                    // Si no hay ninguna bola seleccionada, selecciona la primera bola
                    indexBola1 = index;
                    bolaSeleccionada1 = bolas[indexBola1];
                    Debug.Log("Bola seleccionada 1: " + indexBola1);    
                }
                else if (bolaSeleccionada2 == null)
                {
                    // Si ya hay una bola seleccionada, selecciona la segunda bola
                    indexBola2 = index;
                    bolaSeleccionada2 = bolas[indexBola2];
                    Debug.Log("Bola seleccionada 2: " + indexBola2);
                    
                    // Intercambia las bolas
                    comprobarPosicionBola(bolaSeleccionada1, bolaSeleccionada2, indexBola1, indexBola2);

                    // Restablece las bolas seleccionadas
                    bolaSeleccionada1 = null;
                    bolaSeleccionada2 = null;
                }
            });
        }

    }

    void ColocarBolas(){
        int[] lista = mapaNiveles[nivel];
        for (int i = 0; i < bolas.Length; i++)
        {
            bolas[i].transform.position = destinosDeBolas[lista[i]].position;
            Debug.Log("Posicion de la bola: "+lista[i]+ "indice bola"+  i);
            posicionesActuales[i] = lista[i];
        }
    }

    void MoverBola(Button bola1, Button bola2, int index1, int index2)
    {
        // Guarda la posición de la primera bola
        Vector3 posicionBola1 = bola1.transform.position;

        // Mueve la primera bola a la posición de la segunda bola
        bola1.transform.position = bola2.transform.position;

        // Mueve la segunda bola a la posición original de la primera bola
        bola2.transform.position = posicionBola1;

        // Intercambia las posiciones en posicionesActuales
        int temp = posicionesActuales[index1];
        posicionesActuales[index1] = posicionesActuales[index2];
        posicionesActuales[index2] = temp;
    }

    void comprobarPosicionBola(Button bola1, Button bola2,int iBola1, int iBola2)//bolas y que número de bola quiero mover
    {
        if (bloqueado) return;

        int indexBola1 = iBola1; 
        int indexBola2 = iBola2;
        
        Debug.Log("Index bola 1: "+indexBola1+" Index bola 2: "+indexBola2);
        if (indexBola1 != -1 && indexBola2 != -1 && Math.Abs(posicionesActuales[indexBola1] - posicionesActuales[indexBola2]) == 1)//si estan al lado
        {
            if (pasos > 0)
            {
                MoverBola(bola1, bola2, iBola1, iBola2);
                pasos--;
                pasosRestantes.text = "Pasos restantes: " + pasos;
            }
            int aux = 0;
            for (int j = 0; j <objetivoFinal.Length ; j++)
            {
                if (posicionesActuales[j] == objetivoFinal[j])
                {
                    aux++;
                }
            }
            Debug.Log("aux: "+aux+ "Posiciones actuales: "+posicionesActuales[0]+" "+posicionesActuales[1]+" "+posicionesActuales[2]+" "+posicionesActuales[3]+" "+posicionesActuales[4]+" "+posicionesActuales[5]);
            if (aux == 6)
            {
                StartCoroutine(RutinaGanar());
            }                
            else if(pasos == 0 && aux != 6)
            {
                StartCoroutine(RutinaPerder());
                return;
            }
        }
        else{
            Debug.Log("No se pueden intercambiar las bolas");
        }
    }

    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Bolas");
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

    public void SetValor(bool valor)
    {
        nivel = 1;
        for (int i = 0; i < posicionesActuales.Length; i++)
        {
            posicionesActuales[i] = mapaNiveles[nivel][i];
        }
        ColocarBolas();
        pasos = 6;
        pasosRestantes.text = "Pasos restantes: " + pasos;
        niveles.text = "Nivel: " + nivel;
    }
    IEnumerator MostrarImagenTiempoLimitado(GameObject imagen, float duracion)
    {
        if (imagen == null) yield break;
        
        imagen.SetActive(true);
        yield return new WaitForSeconds(duracion);
        imagen.SetActive(false);
    }
    IEnumerator RutinaGanar()
    {
        bloqueado = true; // Bloquea los clics
        yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenBien, 1f));
        
        // TU CÓDIGO ORIGINAL INTACTO:
        if (nivel == 3)
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
        }
        else
        {
            nivel++;
            for (int i = 0; i < posicionesActuales.Length; i++)
            {
                posicionesActuales[i] = mapaNiveles[nivel][i];
            }
            ColocarBolas();
            pasos = 6;
            pasosRestantes.text = "Pasos restantes: " + pasos;
            niveles.text = "Nivel: " + nivel;
        }
        
        bloqueado = false; // Desbloquea los clics
    }

    IEnumerator RutinaPerder()
    {
        bloqueado = true; // Bloquea los clics
        yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenMal, 1f));
        
        // TU CÓDIGO ORIGINAL INTACTO:
        restartCounter++;
        nivel = 1;
        for (int i = 0; i < posicionesActuales.Length; i++)
        {
            posicionesActuales[i] = mapaNiveles[nivel][i];
        }
        pasos = 6;
        pasosRestantes.text = "Pasos restantes: " + pasos;
        niveles.text = "Nivel: " + nivel;
        ColocarBolas();
        
        bloqueado = false; // Desbloquea los clics
        if(restartCounter == 3)
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
        }
    }
}
