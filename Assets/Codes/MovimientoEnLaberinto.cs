using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Conexiones
{
    public int Destino1 { get; set; }
    public int Destino2 { get; set; }
    public int Destino3 { get; set; }
    public int Destino4 { get; set; }
    public Conexiones(int destino1, int destino2, int destino3, int destino4)
    {
        Destino1 = destino1;
        Destino2 = destino2;
        Destino3 = destino3;
        Destino4 = destino4;
    }
}

public class MovimientoEnLaberinto : MonoBehaviour
{
    public GameObject puntoMovimiento;
    public GameObject DestinoObjetivo1;
    public GameObject DestinoObjetivo2;
    int numbCaminos = 13;
    public GameObject[] Caminos;
    static int caminoActual = 0;
    int Destino1, Destino2;
    Dictionary<int, Tuple<int, int>> mapaObjetosIndices;// aquí guardan donde tendrán que caer cada objeto
    Dictionary<int, Conexiones> mapaConexiones;
    public static int paso = 0;
    ControlMovimientoBolaLaberinto control;
    public PopUpGanar popUpGanar;
    public int restartCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.Find("PanelBotones").GetComponent<ControlMovimientoBolaLaberinto>();
        mapaObjetosIndices = new Dictionary<int, Tuple<int, int>>();
        mapaConexiones = new Dictionary<int, Conexiones>();

        //caminos de los objetos
        mapaObjetosIndices.Add(0, new Tuple<int, int>(1, 3));
        mapaObjetosIndices.Add(1, new Tuple<int, int>(6, 4));
        mapaObjetosIndices.Add(2, new Tuple<int, int>(11, 7));
        mapaObjetosIndices.Add(3, new Tuple<int, int>(8, 12));
        mapaObjetosIndices.Add(4, new Tuple<int, int>(0, 2));

        //conexiones de los caminos
        mapaConexiones.Add(0, new Conexiones(0, 1, 0, 0));
        mapaConexiones.Add(1, new Conexiones(0, 2, 3, 1));
        mapaConexiones.Add(2, new Conexiones(1, 2, 2, 2));
        mapaConexiones.Add(3, new Conexiones(3, 3, 4, 1));
        mapaConexiones.Add(4, new Conexiones(6, 5, 7, 3));
        mapaConexiones.Add(5, new Conexiones(4, 5, 5, 5));
        mapaConexiones.Add(6, new Conexiones(6, 4, 8, 6));
        mapaConexiones.Add(7, new Conexiones(8, 7, 10, 4));
        mapaConexiones.Add(8, new Conexiones(9, 7, 12, 6));
        mapaConexiones.Add(9, new Conexiones(9, 8, 9, 9));
        mapaConexiones.Add(10, new Conexiones(12, 11, 10, 7));
        mapaConexiones.Add(11, new Conexiones(10, 11, 11, 11));
        mapaConexiones.Add(12, new Conexiones(12, 10, 12, 8));
        if(paso==0){
            Destino1 = mapaObjetosIndices[0].Item1;
            Destino2 = mapaObjetosIndices[0].Item2;
        }
        Caminos = new GameObject[numbCaminos];//setear los bloques de caminos
        for (int i = 0; i < numbCaminos; i++)
        {
            Caminos[i] = GameObject.Find("Camino" + i);
        }
        colocarObjetivos();
    }

    public void MoverHaciaDestino(List<int> caminos)
    {
        Conexiones conexiones;
        for (int i = 0; i < caminos.Count; i++)
        {
            if (mapaConexiones.TryGetValue(caminoActual, out conexiones))
            {
                switch (caminos[i])
                {
                    case 0:
                        caminoActual = conexiones.Destino1;
                        break;
                    case 1:
                        caminoActual = conexiones.Destino2;
                        break;
                    case 2:
                        caminoActual = conexiones.Destino3;
                        break;
                    case 3:
                        caminoActual = conexiones.Destino4;
                        break;
                }
            }
            if ((caminoActual == Destino1))
            {
                ControlMovimientoBolaLaberinto.pActual += 3; // Reemplaza "valor" con el valor que deseas asignar
                Debug.Log("PASA POR AQUI");
            }
            else if ((caminoActual == Destino2))
            {
                ControlMovimientoBolaLaberinto.pActual -= 2; // Reemplaza "valor" con el valor que deseas asignar
                Debug.Log("PASA POR AQUI2");
            }
            puntoMovimiento.transform.position = Caminos[caminoActual].transform.position;
        }
        if(caminoActual != Destino1 && caminoActual != Destino2){
            restartCounter++;
            ReiniciarJuego();
            Debug.Log("SE REINICIA");
        }   
        else{
            paso++;
            if (paso == 5)
            {
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            }else{
                colocarObjetivos();
            }
            
        }
        caminos.Clear();

    }

    void colocarObjetivos() {
        if (ControlMovimientoBolaLaberinto.pActual == ControlMovimientoBolaLaberinto.pObjetivo)
        {
            Debug.Log("PASO: " + paso);
            mapaObjetosIndices.TryGetValue(paso, out Tuple<int, int> destinos);
            Destino1 = destinos.Item1;
            Destino2 = destinos.Item2;
            control.UpdatePuntuacion();
            puntoMovimiento.transform.position = Caminos[caminoActual].transform.position;
            DestinoObjetivo1.transform.position = Caminos[Destino1].transform.position;
            DestinoObjetivo2.transform.position = Caminos[Destino2].transform.position;
        }
        else{
            ReiniciarJuego();
        }
        
    }

    public void ReiniciarJuego()
    {
        paso = 0;
        ControlMovimientoBolaLaberinto.pActual = 0;
        ControlMovimientoBolaLaberinto.pObjetivo = 3;
        control.VaciarCola();
        control.ConstruirCola();
        control.UpdatePuntuacion();

        mapaObjetosIndices.TryGetValue(paso, out Tuple<int, int> destinos);
        Destino1 = destinos.Item1;
        Destino2 = destinos.Item2;
        caminoActual = 0;

        puntoMovimiento.transform.position = Caminos[caminoActual].transform.position;
        DestinoObjetivo1.transform.position = Caminos[Destino1].transform.position;
        DestinoObjetivo2.transform.position = Caminos[Destino2].transform.position;
    }

    public void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Laberinto");
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

    public void SetValor(bool valor){
        ReiniciarJuego();
    }
}
