using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class ControlMovimientoBolaLaberinto : MonoBehaviour
{
    public GameObject[] listaCaminos;
    public Button[] botonesControl;
    public Button btnListo;
    public GameObject puntuacionAcual, puntuacionObjetivo;
    public static int pActual { get; set; }
    public static int pObjetivo { get; set; }
    TextMeshProUGUI tAcual, tObjetivo;
    private int numCamninos = 7;
    private int numbBtns = 4;
    int position;
    static List<int> caminos;
    Queue<int> colaObjetivos;
    MovimientoEnLaberinto movimientoEnLaberinto;
    

    // Start is called before the first frame update
    void Start(){
        //inicializamos las variables
        colaObjetivos = new Queue<int>();
        caminos = new List<int>();
        listaCaminos = new GameObject[8];
        botonesControl = new Button[4];
        btnListo = GameObject.Find("BotonListo").GetComponent<Button>();

        puntuacionAcual = GameObject.Find("PuntosActuales");
        puntuacionObjetivo = GameObject.Find("PuntosObjetivo");

        movimientoEnLaberinto = GameObject.Find("PanelCamino").GetComponent<MovimientoEnLaberinto>();
        //Valor que tiene que dar en cada ronda
        ConstruirCola();

        pObjetivo = 3;

        for (int i = 0; i < numCamninos; i++)
        {
            listaCaminos[i] = GameObject.Find("Paso" + i);
        }
        
        botonesControl[0] = GameObject.Find("Arriba").GetComponent<Button>();
        botonesControl[1] = GameObject.Find("Abajo").GetComponent<Button>();
        botonesControl[2] = GameObject.Find("Derecha").GetComponent<Button>();
        botonesControl[3] = GameObject.Find("Izquierda").GetComponent<Button>();
        
        for (int i = 0; i < numbBtns; i++)
        {
            int temp = i;//hay que ponerlo asi, porque si no c# no lo coge, es una trampita
            botonesControl[i].onClick.AddListener(() =>
            {
                SeleccionarControlBoton(temp);
            });
        }

        btnListo.onClick.AddListener(() => {
            position = 0;
            //Debug.Log("Boton Listo");
            if (movimientoEnLaberinto != null)
            {
                //Debug.Log("MovimientoEnLaberinto encontrado");
                movimientoEnLaberinto.MoverHaciaDestino(caminos);
            }
            else
            {
                Debug.LogError("MovimientoEnLaberinto no inicializado correctamente");
            }
            VaciarPasosListaCaminos();
        });
    }
    public void ConstruirCola(){
        colaObjetivos.Enqueue(3);
        colaObjetivos.Enqueue(1);
        colaObjetivos.Enqueue(4);
        colaObjetivos.Enqueue(5);
        colaObjetivos.Enqueue(6);
    }
    void SeleccionarControlBoton(int i)
    {
        if(position >= 7)
        {
            Debug.Log("Ya no se pueden seleccionar mas caminos");
            return;
        }
        if(i==0)
            listaCaminos[position].GetComponent<Image>().color = Color.red;
        else if(i==1)
            listaCaminos[position].GetComponent<Image>().color = Color.blue;
        else if(i==2)     
            listaCaminos[position].GetComponent<Image>().color = Color.green;
        else if(i==3)
            listaCaminos[position].GetComponent<Image>().color = Color.yellow;
        Debug.Log("Boton: " + i);
        caminos.Add(i);
        position++;
    }

    void VaciarPasosListaCaminos(){
        for (int i = 0; i < numCamninos; i++){
            listaCaminos[i].GetComponent<Image>().color = Color.white;
        }
    }

    public void UpdatePuntuacion(){
        pObjetivo = colaObjetivos.Dequeue();
        tAcual = puntuacionAcual.GetComponent<TextMeshProUGUI>();
        tObjetivo = puntuacionObjetivo.GetComponent<TextMeshProUGUI>();
        tAcual.text = "Actual: "+pActual.ToString();
        tObjetivo.text = "Objetivo: "+pObjetivo.ToString();
    }

    public void VaciarCola()
    {
        colaObjetivos.Clear();
    }


}
