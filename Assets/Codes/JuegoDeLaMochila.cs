using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JuegoDeLaMochila : MonoBehaviour
{
    public TMP_Text capacidadTexto;
    public int nivel = 1;
    public TMP_Text niveles;
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
        niveles.text = "Nivel: " + nivel;
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
                if(nivel < 3)
                {
                    AvanzarNivel();
                }
                else
                    Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            else
            {
                restartCounter++;
                if(restartCounter > 3)
                {
                    Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
                }
            }
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
        SetValor(true);
    }

    /// <summary>
    /// Avanza al siguiente nivel regenerando nuevos valores aleatorios.
    /// Mantiene el contador de errores y solo cambia capacidad y pesos.
    /// </summary>
    public void AvanzarNivel()
    {
        if (nivel < 3)
        {
            nivel++;
            niveles.text = "Nivel: " + nivel;
            
            // Regenerar valores según el nuevo nivel
            if(nivel == 2)
            {
                capacidadMochila = Random.Range(30, 50);
            }
            else if(nivel == 3)
            {
                capacidadMochila = Random.Range(50, 70);
            }
            
            capacidadTexto.text = "Capacidad: " + capacidadMochila.ToString("") + " kg";
            for (int i = 0; i < numElementos; i++)
            {
                if(nivel == 2)
                    peso[i] = Random.Range(10, 20);
                else if(nivel == 3)
                    peso[i] = Random.Range(15, 25);
                pesoTexto[i].text = peso[i].ToString("") + " kg";
            }
            for (int i = 0; i < numElementos; i++)
            {
                elementosBtn[i].GetComponent<Image>().color = Color.white;
            }
            almacenElementos.Clear();
        }
    }

    /// <summary>
    /// Mantiene los mismos parámetros sin regenerarlos (se usa cuando aplicas ayuda).
    /// Solo resetea la selección de elementos y actualiza la UI.
    /// </summary>
    public void SetValor1(bool valor)
    {
        if (valor)
        {
            Debug.Log("SetValor1 - Manteniendo parámetros actuales");
            capacidadTexto.text = "Capacidad: " + capacidadMochila.ToString("") + " kg";
            for (int i = 0; i < numElementos; i++)
            {
                pesoTexto[i].text = peso[i].ToString("") + " kg";
            }
            for (int i = 0; i < numElementos; i++)
            {
                elementosBtn[i].GetComponent<Image>().color = Color.white;
            }
            almacenElementos.Clear();
        }
    }

    /// <summary>
    /// Reinicia completamente el juego: vuelve al nivel 1 y resetea el contador de errores.
    /// Se utiliza cuando el usuario presiona el botón "Reiniciar" en el menú de pausa.
    /// </summary>
    public void SetValor(bool valor)
    { 
        if (valor)
        {
            Debug.Log("SetValor - Reiniciando juego completo");
            nivel = 1;
            restartCounter = 0;
            niveles.text = "Nivel: " + nivel;
            
            capacidadMochila = Random.Range(10, 30);
            capacidadTexto.text = "Capacidad: " + capacidadMochila.ToString("") + " kg";
            for (int i = 0; i < numElementos; i++)
            {
                peso[i] = Random.Range(5, 15);
                pesoTexto[i].text = peso[i].ToString("") + " kg";
            }
            for (int i = 0; i < numElementos; i++)
            {
                elementosBtn[i].GetComponent<Image>().color = Color.white;
            }
            almacenElementos.Clear();
        }
    }
}
