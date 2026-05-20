using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;


public class JuegoDeReinas : MonoBehaviour// hay muchas cosas que arreglar de este ultimo juego
{
    public static int n = 5;
    public GameObject[] reinas = new GameObject[n];
    //public Dictionary<int, Button[]> mapaTablaAjedrez = new Dictionary<int, Button[]>();
    public Button[] Fila0 = new Button[n];
    public Button[] Fila1 = new Button[n];
    public Button[] Fila2 = new Button[n];
    public Button[] Fila3 = new Button[n];
    public Button[] Fila4 = new Button[n];
    public Button btnListo;
    public PopUpGanar popUpGanar;
    public int restartCounter = 0;
    public int[] posicionesReinas = new int[n];//cada indice son columnas y el valor son las filas
    public GameObject[] fuera = new GameObject[n];// fuera del tablero habrá poner en algun lado
    int combinacionesPosibles = 3;
    public Dictionary<int, int[]> mapaDeRepeticiones = new Dictionary<int, int[]>();
    public TMP_Text combosRestantes;

    void Start()
    {
        for (int i = 0; i < posicionesReinas.Length; i++)
        {
            posicionesReinas[i] = -1;
            reinas[i].transform.position = fuera[i].transform.position;
        }
        //reinas[0].transform.position=Fila0[1].transform.position;
        for (int i = 0; i < reinas.Length; i++)
        {
            GameObject reina = GameObject.Find("Reina" + i);
            if (reina != null)
            {
                reinas[i] = reina;
            }
        }

        for(int i=0; i<Fila0.Length; i++)
        {
            Fila0[i] = GameObject.Find("Bloque0" + i).GetComponent<Button>();
            Fila1[i] = GameObject.Find("Bloque1" + i).GetComponent<Button>();
            Fila2[i] = GameObject.Find("Bloque2" + i).GetComponent<Button>();
            Fila3[i] = GameObject.Find("Bloque3" + i).GetComponent<Button>();
            Fila4[i] = GameObject.Find("Bloque4" + i).GetComponent<Button>();
        }
        combosRestantes.text = "Combos restantes: " + combinacionesPosibles;
        for(int i=0; i<reinas.Length; i++)
        {
            int index = i;
            Fila0[i].onClick.AddListener(() => {
                PosicionarReina(0, index);
            });
            Fila1[i].onClick.AddListener(() => {
                PosicionarReina(1, index);
            });
            Fila2[i].onClick.AddListener(() => {
                PosicionarReina(2, index);
            });
            Fila3[i].onClick.AddListener(() => {
                PosicionarReina(3, index);
            });
            Fila4[i].onClick.AddListener(() => {
                PosicionarReina(4, index);
            });
        }
        btnListo.onClick.AddListener(() => {
            if (comprobarTablero())
            {
                if (mapaDeRepeticiones.Count == 0)
                {
                    Debug.Log(posicionesReinas[0] + " " + posicionesReinas[1] + " " + posicionesReinas[2] + " " + posicionesReinas[3] + " " + posicionesReinas[4]);
                    combinacionesPosibles--;
                    mapaDeRepeticiones.Add(combinacionesPosibles, posicionesReinas.Clone() as int[]);
                    Debug.Log("Tablero correcto, quedan combinaciones posibles: " + combinacionesPosibles);
                    combosRestantes.text = "Combos restantes: " + combinacionesPosibles;
                    SetValor(true);
                }
                else
                {
                    Debug.Log(posicionesReinas[0] + " " + posicionesReinas[1] + " " + posicionesReinas[2] + " " + posicionesReinas[3] + " " + posicionesReinas[4]);
                    int vecesRepetidos = 0;

                    foreach (var lista in mapaDeRepeticiones.Values)
                    {
                       for(int i=0; i<lista.Length; i++)
                        {
                            if (lista[i] == posicionesReinas[i])
                            {
                                vecesRepetidos++;
                                Debug.Log(lista[i] + "==" + posicionesReinas[i] + " en la columna " + i);
                            }
                        }
                        Debug.Log("Veces repetidos: " + vecesRepetidos);
                        if(vecesRepetidos == 5)
                        {
                            break;
                        } 
                    }
                    if (vecesRepetidos != 5)
                    {
                        combinacionesPosibles--;
                        if (combinacionesPosibles > 0)
                        {
                            mapaDeRepeticiones.Add(combinacionesPosibles, posicionesReinas.Clone() as int[]);
                            Debug.Log("Tablero correcto, quedan combinaciones posibles: " + combinacionesPosibles);
                            combosRestantes.text = "Combos restantes: " + combinacionesPosibles;
                            SetValor(true);
                        }
                        else
                        {
                            Debug.Log("Tablero correcto");
                            combosRestantes.text = "Combos restantes: " + combinacionesPosibles;
                            Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
                        }
                    }
                    else
                    {
                        Debug.Log("Tablero repetido");
                        restartCounter++;
                        SetValor(true);
                    }
                }
            }
            else
            {
                Debug.Log("Tablero incorrecto");
                restartCounter++;
                SetValor(true);
            }
        });

    }

    void PosicionarReina(int fila, int columna)
    {
        if (fila < reinas.Length && columna < Fila0.Length) // Asegúrate de que fila y columna están dentro de los límites del array
        {
            if (fila == 0)
                reinas[fila].transform.position = Fila0[columna].transform.position;
            else if (fila == 1)
                reinas[fila].transform.position = Fila1[columna].transform.position;
            else if (fila == 2)
                reinas[fila].transform.position = Fila2[columna].transform.position;
            else if (fila == 3)
                reinas[fila].transform.position = Fila3[columna].transform.position;
            else if (fila == 4)
                reinas[fila].transform.position = Fila4[columna].transform.position;
            posicionesReinas[fila] = columna;
        }
        else
        {
            Debug.Log("Fila o columna fuera de rango");
        }
    }
    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Reina");
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
            mensajeGanado = "¡Has ganado! Pero has cometido varios errores.";
        }
        popUpGanar.MostrarPopUpGanado(restartCounter, mensajeGanado);
    }

    public void SetValor(bool valor)
    {
        if (valor)
        {
            for (int i = 0; i < posicionesReinas.Length; i++)
            {
                posicionesReinas[i] = -1;
                reinas[i].transform.position = fuera[i].transform.position;
            }
        }
    }

    bool comprobarTablero(){
        Debug.Log("comprobando tablero"+ comprobarFila() +"&&" +comprobarColumna()+ "&&"+ comprobarDiagonal1()+ "&&" +comprobarDiagonal2());
        return comprobarFila() && comprobarColumna() && comprobarDiagonal1() && comprobarDiagonal2();
    }


    bool comprobarFila()
    {
        int[] contadorReinas = new int[n];

        for (int i = 0; i < posicionesReinas.Length; i++)
        {
            int fila = posicionesReinas[i]; 
            contadorReinas[fila]++; 
        }

        for (int i = 0; i < contadorReinas.Length; i++) 
        {
            if (contadorReinas[i] > 1) 
            {
                return false;
            }
        }

        return true; 
    }

    bool comprobarColumna(){// en este caso el juego nunca puedes posicionar en la misma columna entonces siempre true
        return true;
    }

    bool comprobarDiagonal1(){
        //formula columna - fila + n - 1 = constante
        int[] constantes = new int[5];
        constantes[0] = 0 - posicionesReinas[0] + n - 1;
        constantes[1] = 1 - posicionesReinas[1] + n - 1;
        constantes[2] = 2 - posicionesReinas[2] + n - 1;
        constantes[3] = 3 - posicionesReinas[3] + n - 1;
        constantes[4] = 4 - posicionesReinas[4] + n - 1;

        for (int i = 0; i < constantes.Length; i++)
        {
            for (int j = i + 1; j < constantes.Length; j++)
            {
                if (constantes[i] == constantes[j])
                {
                    Debug.Log("Las constantes " + i + " y " + j + " son iguales.");
                    return false;
                }
            }
        }
        return true;
    }
    bool comprobarDiagonal2()
    {
        // Fórmula: fila + columna = constante
        int[] constantes = new int[5];
        constantes[0] = posicionesReinas[0] + 0;
        constantes[1] = posicionesReinas[1] + 1;
        constantes[2] = posicionesReinas[2] + 2;
        constantes[3] = posicionesReinas[3] + 3;
        constantes[4] = posicionesReinas[4] + 4;

        for (int i = 0; i < constantes.Length; i++)
        {
            for (int j = i + 1; j < constantes.Length; j++)
            {
                if (constantes[i] == constantes[j])
                {
                    Debug.Log("Las constantes " + i + " y " + j + " son iguales.");
                    return false;
                }
            }
        }
        return true;
    }

}
