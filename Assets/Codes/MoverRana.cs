using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Dynamic;

public class MoverRana : MonoBehaviour
{
    public Button[] botonesRanas = new Button[6]; 
    public RectTransform[] objetoDestino = new RectTransform[7];
    public static bool[] ocupado = {true, true, true, false, true, true, true };
    public static int[] posiciones = { 0, 1, 2, 4, 5, 6 };//indice de la lista es un boton y el valor es la posicion
    private int indiceBotonClickeado;
    public PopUpGanar popUpGanar;
    int restartCounter = 0;

    //public static bool[] ocupadoGuardado;
    void Start(){
        //popUpGanar = new PopUpGanar();
        for (int i=0;i<botonesRanas.Length;i++){
            int indice = i;
            botonesRanas[indice].onClick.AddListener(() =>{
                indiceBotonClickeado = indice;
                MoverHaciaDestino(indiceBotonClickeado);
            });
        }
        UnityEngine.Debug.Log("ocupado: " + string.Join(", ", ocupado));
        
    }

    bool Bloqueado()
    {
        bool block = true;
        for(int i = 0; i< posiciones.Length; i++)
        {
            if (i < posiciones.Length / 2)
            {
                if((!ocupado[posiciones[i]+1] && posiciones[i] != 6) || (!ocupado[posiciones[i] + 2] && posiciones[i] != 5 && posiciones[i] != 6))
                {
                    block= false;
                    break;
                }
            }
            else
            {
                if((!ocupado[posiciones[i]-1] && posiciones[i]!=0) || (!ocupado[posiciones[i] - 2]&& posiciones[i]!=0 && posiciones[i]!=1))
                {
                    block= false;
                    break;
                }
            }
        }
        return block;
    }

    void MoverHaciaDestino(int indice)
    {
        int pos,salto;
        if (indice < botonesRanas.Length / 2){
            pos = posiciones[indice] + 1;
            salto = posiciones[indice] + 2;
        }else{
            pos = posiciones[indice] - 1;
            salto = posiciones[indice] - 2;
        }
        if (pos >= 0 && pos < ocupado.Length && !ocupado[pos]){
            ocupado[posiciones[indice]] = false;
            ocupado[pos] = true;
            posiciones[indice] = pos;
        }else if (salto >= 0 && salto < ocupado.Length && !ocupado[salto]){
            ocupado[posiciones[indice]] = false;
            ocupado[salto] = true;
            posiciones[indice] = salto;
        }
        else
            return; 
        UnityEngine.Debug.Log("ocupado: " + string.Join(", ", ocupado));
        UnityEngine.Debug.Log("Posiciones: " + string.Join(", ", posiciones));
        botonesRanas[indice].transform.position = objetoDestino[posiciones[indice]].position;
        VerificarPosiciones();
        if (Bloqueado())
        {
            if(restartCounter>2)
                Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
            SetValor(true);
        }
    }

    void VerificarPosiciones(){
        int[] posicionesDeseadas = { 4, 5, 6, 0, 1, 2 };
        for(int i=0;i<posiciones.Length;i++){
            if(posiciones[i] != posicionesDeseadas[i])
                return;
        }
        UnityEngine.Debug.Log("Las veces repetidas: "+ restartCounter);
        UnityEngine.Debug.Log("¡Felicidades!");
        //Debug.Log("¡Felicidades!");
        Invoke("MostrarPopUpGanadoConRetraso", 0.3f);
    }

    public void SetValor(bool valor){
        if(valor){
            restartCounter++;
            posiciones = new int[] { 0, 1, 2, 4, 5, 6 };
            ocupado = new bool[]{ true, true, true, false, true, true, true };
            for (int i = 0; i < botonesRanas.Length; i++)
                botonesRanas[i].transform.position = objetoDestino[posiciones[i]].position;
        }
        
    }

    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Ranas");
        string mensajeGanado = "";
        if (restartCounter == 0)
        {
            mensajeGanado = "¡Felicidades! Has ganado sin cometer errores.";
        }
        else if (restartCounter == 1)
        {
            mensajeGanado = "¡Bien hecho! Has ganado con solo un error.";
        }
        else if(restartCounter ==2)
        {
            mensajeGanado = "Has ganado, pero cometiste algunos errores. ¡Sigue practicando!";
        }
        else
        {
            mensajeGanado = "Has perdido, piénsate mejor cómo deberías mover.";
        }
        popUpGanar.MostrarPopUpGanado(restartCounter, mensajeGanado);
        SetValor(true);
    }
}
