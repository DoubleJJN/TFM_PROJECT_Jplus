using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JuegoDeLaMochila : MonoBehaviour
{
    public TMP_Text capacidadTexto;
    public static int capacidadMochila;
    public List<TMP_Text> pesoTexto= new List<TMP_Text>();
    static int[] peso = new int[5];
    int numElementos = 5;
    public Button[] elementosBtn = new Button[5];
    public Button mochilaBtn;//no ha sido usado al final
    public Button listoBtn;
    public PopUpGanar popUpGanar;
    int restartCounter = 0;
    public static bool ayudado = false;
    List<int> almacenElementos = new List<int>();//aqui guarda indices
    

    void Start()
    {
        if(ayudado){
            SetValor1(true);
        }else{
            SetValor(true);
        }
        for (int j = 0; j < numElementos; j++)
        {
            int index = j;
            elementosBtn[j].onClick.AddListener(() =>
            {
                AddElemento(index);
            });
        }
        listoBtn.onClick.AddListener(() =>
        {
            if(comprobarMochila()==true)
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            else
                restartCounter++;
        });

        Debug.Log("Peso máximo optimizado: " + pesoMaximoOptimizadoWrapper());
    }

    int pesoMaximoOptimizadoWrapper(){
        List<int> almacenOptimo = new List<int>();
        return pesoMaximoOptimizado(0, capacidadMochila, peso, new List<int>(), ref almacenOptimo, 0);
    }

    int pesoMaximoOptimizado(int indice, int capacidad, int[]pesos, List<int> almacenTemporal, ref List<int> almacenOptimo, int sumaOptimo){
        if (indice == pesos.Length){
            int sumaTemporal = sumaLista(almacenTemporal);
            if ((sumaLista(almacenTemporal) > sumaLista(almacenOptimo))||((sumaLista(almacenTemporal)== sumaLista(almacenOptimo)) && (almacenTemporal.Count > almacenOptimo.Count))){
                almacenOptimo.Clear();
                foreach (int p in almacenTemporal){
                    almacenOptimo.Add(p);
                }
                sumaOptimo = sumaTemporal;
            }
            return sumaOptimo;
        }
        else{
            for(int i=indice; i<pesos.Length; i++){
                if(capacidad-pesos[i]>=0){
                    almacenTemporal.Add(pesos[i]);
                    sumaOptimo = pesoMaximoOptimizado(i+1,capacidad-pesos[i],pesos,almacenTemporal,ref almacenOptimo,sumaOptimo);
                    almacenTemporal.RemoveAt(almacenTemporal.Count-1);
                }else
                    sumaOptimo = pesoMaximoOptimizado(i + 1, capacidad, pesos, almacenTemporal, ref almacenOptimo, sumaOptimo);
            }
            return sumaOptimo;
        }
        
    }
    private int sumaLista(List<int> lista){
        int suma = 0;
        foreach (int i in lista){
            suma += i;
        }
        return suma;
    }
    private int sumaListaElegido(List<int> lista){
        int suma = 0;
        foreach (int i in lista){
            suma += peso[i];
        }
        return suma;
    }
    void AddElemento(int index)
    {
        Debug.Log("Elemento " + index+" peso "+ peso[index]);

        if (!almacenElementos.Contains(index))
        {
            almacenElementos.Add(index);
            elementosBtn[index].GetComponent<Image>().color = Color.green;
        }
        else
        {
            almacenElementos.Remove(index);
            elementosBtn[index].GetComponent<Image>().color = Color.white;
            Debug.Log("Elemento eliminado:" + index);
        }
    }
    IEnumerator ChangeButtonColor(Button button, Color color)
    {
        button.image.color = color;
        yield return new WaitForSeconds(1.0f);
        button.image.color = Color.white;
        almacenElementos.Clear();
    }

    bool comprobarMochila()
    {
        int suma = 0;
        if (almacenElementos.Count == 0 || almacenElementos.Count > numElementos)
            return false;
        else
        {
            suma = sumaListaElegido(almacenElementos);
            if (suma <= capacidadMochila && pesoMaximoOptimizadoWrapper() == suma)
            {
                Debug.Log("¡Correcto!");
                return true;
            }
            else
            {
                Debug.Log("¡Incorrecto! no es optimo" + suma + " " + pesoMaximoOptimizadoWrapper());
                foreach (int index in almacenElementos)
                {
                    StartCoroutine(ChangeButtonColor(elementosBtn[index], Color.red));
                }
                return false;
            }
        }
    }

    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Mochila");
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
        SetValor(true);
    }

    public void SetValor1(bool valor)
    {
        if (valor)
        {
            //capacidadMochila = Random.Range(20, 50);
            capacidadTexto.text = "Capacidad: " + capacidadMochila.ToString("") + " kg";
            for (int i = 0; i < numElementos; i++)
            {
                //peso[i] = (Random.Range(5, 20));
                pesoTexto[i].text = peso[i].ToString("") + " kg";
            }
            for (int i = 0; i < numElementos; i++)
            {
                elementosBtn[i].GetComponent<Image>().color = Color.white;
            }
            almacenElementos.Clear();
        }
    }

    public void SetValor(bool valor)
    {

        if (valor)
        {
            Debug.Log("SetValor:" + valor);
            capacidadMochila = Random.Range(20, 50);
            capacidadTexto.text = "Capacidad: " + capacidadMochila.ToString("") + " kg";
            for (int i = 0; i < numElementos; i++)
            {
                peso[i] = (Random.Range(5, 20));
                pesoTexto[i].text = peso[i].ToString("") + " kg";
            }
            for (int i = 0; i < numElementos; i++)
            {
                elementosBtn[i].GetComponent<Image>().color = Color.white;
            }
            almacenElementos.Clear();
        }
    }

    public static void darComoJugar(){
        ayudado = true;
    }

}
