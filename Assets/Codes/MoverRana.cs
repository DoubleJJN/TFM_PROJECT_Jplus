using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Dynamic;
using TMPro;

public class MoverRana : MonoBehaviour
{
    public Button[] botonesRanas = new Button[6]; 
    public RectTransform[] objetoDestino = new RectTransform[7];
    public static bool[] ocupado;
    public static int[] posiciones;
    private int indiceBotonClickeado;
    public PopUpGanar popUpGanar;
    public TextMeshProUGUI textoNivel;
    int restartCounter = 0;
    public GameObject imagenBien, imagenMal;
    
    // Sistema de niveles
    public int nivelActual = 1;
    private bool[] ocupadoNivel1 = { true, true, true, false, true, true, true };
    private int[] posicionesNivel1 = { 2, 4 }; // índices de botones (rana 2 y rana 3)
    private int[] posicionesDesadasNivel1 = { 4, 2 }; // posiciones finales
    
    private bool[] ocupadoNivel2 = { true, true, true, false, true, true, true };
    private int[] posicionesNivel2 = { 1, 2, 4, 5 }; // índices de botones (ranas 1,2,3,4)
    private int[] posicionesDesadasNivel2 = { 4, 5, 1, 2 }; // posiciones finales
    
    private bool[] ocupadoNivel3 = { true, true, true, false, true, true, true };
    private int[] posicionesNivel3 = { 0, 1, 2, 4, 5, 6 };
    private int[] posicionesDesadasNivel3 = { 4, 5, 6, 0, 1, 2 };

    void Start(){
        InicializarNivel(nivelActual);
        
        for (int i=0; i<botonesRanas.Length; i++){
            int indice = i;
            botonesRanas[indice].onClick.AddListener(() =>{
                indiceBotonClickeado = indice;
                MoverHaciaDestino(indiceBotonClickeado);
            });
        }
        UnityEngine.Debug.Log("ocupado: " + string.Join(", ", ocupado));
    }
    
    void InicializarNivel(int nivel)
    {
        // Solo resetear el contador al iniciar una nueva partida (nivel 1)
        if (nivel == 1)
        {
            restartCounter = 0;
        }
        
        nivelActual = nivel;
        
        // Actualizar texto del nivel
        if (textoNivel != null)
        {
            textoNivel.text = "Nivel: " + nivel;
        }
        
        // Inicializar posiciones como array de tamaño completo (siempre 6 ranas)
        posiciones = new int[6];
        for (int i = 0; i < 6; i++) posiciones[i] = -1;  // -1 = no activo
        
        switch(nivel)
        {
            case 1:
                ocupado = new bool[] { true, true, true, false, true, true, true };
                posiciones[2] = 2;   // Rana 2 en posición 2
                posiciones[3] = 4;   // Rana 3 en posición 4
                MostrarRanasNivel(new int[] {2, 3});
                break;
            case 2:
                ocupado = new bool[] { true, true, true, false, true, true, true };
                posiciones[1] = 1;   
                posiciones[2] = 2;   
                posiciones[3] = 4;   // Rana 4 en posición 4
                posiciones[4] = 5;   // Rana 5 en posición 5
                MostrarRanasNivel(new int[] {1, 2, 3, 4});
                break;
            case 3:
                ocupado = new bool[] { true, true, true, false, true, true, true };
                posiciones[0] = 0;   // Rana 0 en posición 0
                posiciones[1] = 1;   // Rana 1 en posición 1
                posiciones[2] = 2;   // Rana 2 en posición 2
                posiciones[3] = 4;   // Rana 3 en posición 4
                posiciones[4] = 5;   // Rana 4 en posición 5
                posiciones[5] = 6;   // Rana 5 en posición 6
                MostrarRanasNivel(new int[] {0, 1, 2, 3, 4, 5});
                break;
        }
        
        // Posicionar ranas inicialmente
        for (int i = 0; i < botonesRanas.Length; i++)
        {
            if (posiciones[i] >= 0)
            {
                botonesRanas[i].transform.position = objetoDestino[posiciones[i]].position;
            }
        }
    }
    
    void MostrarRanasNivel(int[] ranasVisibles)
    {
        for (int i = 0; i < botonesRanas.Length; i++)
        {
            bool estaEnNivel = System.Array.Exists(ranasVisibles, element => element == i);
            botonesRanas[i].gameObject.SetActive(estaEnNivel);
        }
    }

    bool Bloqueado()
    {
        bool block = true;
        for(int i = 0; i< posiciones.Length; i++)
        {
            // Solo verificar ranas activas
            if (posiciones[i] < 0) continue;
            
            int posRana = posiciones[i];
            
            if (i < posiciones.Length / 2)
            {
                // Ranas que se mueven hacia la derecha
                bool puedeMoverDerecha1 = (posRana + 1 < ocupado.Length) && !ocupado[posRana + 1];
                bool puedeMoverDerecha2 = (posRana + 2 < ocupado.Length) && !ocupado[posRana + 2];
                
                if(puedeMoverDerecha1 || puedeMoverDerecha2)
                {
                    block = false;
                    break;
                }
            }
            else
            {
                // Ranas que se mueven hacia la izquierda
                bool puedeMoverIzquierda1 = (posRana - 1 >= 0) && !ocupado[posRana - 1];
                bool puedeMoverIzquierda2 = (posRana - 2 >= 0) && !ocupado[posRana - 2];
                
                if(puedeMoverIzquierda1 || puedeMoverIzquierda2)
                {
                    block = false;
                    break;
                }
            }
        }
        return block;
    }

    void MoverHaciaDestino(int indice)
    {
        // Verificar que la rana está activa en este nivel
        if (posiciones[indice] < 0) return;
        
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
        
        // Verificar si completó el nivel primero
        bool nivelCompletado = VerificarPosicionesYRetorna();
        
        // Solo chequear bloqueo si NO completó el nivel
        if (!nivelCompletado && Bloqueado())
        {
            UnityEngine.Debug.Log("¡Bloqueado! Reiniciando nivel...");
            Invoke("ReiniciarNivelActual", 0.5f);
        }
    }
    
    bool VerificarPosicionesYRetorna(){
        int[] posicionesDeseadas;
        
        switch(nivelActual)
        {
            case 1:
                posicionesDeseadas = posicionesDesadasNivel1;
                break;
            case 2:
                posicionesDeseadas = posicionesDesadasNivel2;
                break;
            case 3:
                posicionesDeseadas = posicionesDesadasNivel3;
                break;
            default:
                posicionesDeseadas = posicionesDesadasNivel3;
                break;
        }
        
        // Comparar solo las ranas activas
        int indiceDeseado = 0;
        for(int i=0; i<posiciones.Length; i++){
            if(posiciones[i] >= 0){  // Solo ranas activas
                if(posiciones[i] != posicionesDeseadas[indiceDeseado])
                    return false;
                indiceDeseado++;
            }
        }
        
        // Nivel completado
        UnityEngine.Debug.Log("Las veces repetidas: "+ restartCounter);
        UnityEngine.Debug.Log("¡Nivel " + nivelActual + " completado!");
        
        // Progresión de niveles
        if (nivelActual < 3)
        {
            // Avanzar al siguiente nivel
            Invoke("AvanzarNivel", 1.0f);
        }
        else
        {
            // Nivel 3 completado - mostrar popup de victoria
            Invoke("MostrarPopUpGanadoConRetraso", 1.0f);
        }
        
        return true;
    }
    
    void ReiniciarNivelActual()
    {
        restartCounter++;  // Incrementar contador de errores
        UnityEngine.Debug.Log("Errores acumulados: " + restartCounter);
        StartCoroutine(MostrarImagenTiempoLimitado(imagenMal, 1f));
        // Reinicializar posiciones sin resetear el contador
        posiciones = new int[6];
        for (int i = 0; i < 6; i++) posiciones[i] = -1;
        
        switch(nivelActual)
        {
            case 1:
                ocupado = new bool[] { true, true, true, false, true, true, true };
                posiciones[2] = 2;
                posiciones[3] = 4;
                break;
            case 2:
                ocupado = new bool[] { true, true, true, false, true, true, true };
                posiciones[1] = 1;   
                posiciones[2] = 2;   
                posiciones[3] = 4;
                posiciones[4] = 5;
                break;
            case 3:
                ocupado = new bool[] { true, true, true, false, true, true, true };
                posiciones[0] = 0;
                posiciones[1] = 1;
                posiciones[2] = 2;
                posiciones[3] = 4;
                posiciones[4] = 5;
                posiciones[5] = 6;
                break;
        }
        
        // Posicionar ranas inicialmente
        for (int i = 0; i < botonesRanas.Length; i++)
        {
            if (posiciones[i] >= 0)
            {
                botonesRanas[i].transform.position = objetoDestino[posiciones[i]].position;
            }
        }

        if(restartCounter == 3)
        {
            Invoke("MostrarPopUpGanadoConRetraso", 0.5f);
        }
    }

    
    void AvanzarNivel()
    {
        // Método void normal: Usamos StartCoroutine para llamar al IEnumerator
        StartCoroutine(RutinaAvanzarNivel());
    }

    IEnumerator RutinaAvanzarNivel()
    {
        // Al ser un IEnumerator, aquí SÍ podemos usar yield return
        yield return StartCoroutine(MostrarImagenTiempoLimitado(imagenBien, 1f));
        NivelSiguiente();
    }

    public void SetValor(bool valor){
        if(valor){
            // Resetear completamente el juego
            ResetearJuegoCompleto();
        }
    }
    
    void ResetearJuegoCompleto()
    {
        // Reiniciar contador de errores
        restartCounter = 0;
        
        // Reiniciar al nivel 1
        nivelActual = 1;
        
        // Actualizar texto del nivel
        if (textoNivel != null)
        {
            textoNivel.text = "Nivel: 1";
        }
        
        // Reinicializar posiciones del nivel 1
        posiciones = new int[6];
        for (int i = 0; i < 6; i++) posiciones[i] = -1;
        
        ocupado = new bool[] { true, true, true, false, true, true, true };
        posiciones[2] = 2;
        posiciones[3] = 4;
        
        // Mostrar solo las ranas del nivel 1
        MostrarRanasNivel(new int[] {2, 3});
        
        // Posicionar ranas inicialmente
        for (int i = 0; i < botonesRanas.Length; i++)
        {
            if (posiciones[i] >= 0)
            {
                botonesRanas[i].transform.position = objetoDestino[posiciones[i]].position;
            }
        }
        
        UnityEngine.Debug.Log("Juego reiniciado completamente");
    }

    void MostrarPopUpGanadoConRetraso()
    {
        popUpGanar.SetNombreJuego("Ranas");
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
    
    // Métodos para cambiar de nivel
    public void CambiarNivel(int nuevoNivel)
    {
        if (nuevoNivel >= 1 && nuevoNivel <= 3)
        {
            InicializarNivel(nuevoNivel);
        }
    }
    
    public void NivelSiguiente()
    {
        if (nivelActual < 3)
        {
            InicializarNivel(nivelActual + 1);
        }
    }
    IEnumerator MostrarImagenTiempoLimitado(GameObject imagen, float duracion)
    {
        if (imagen == null) yield break;
        
        imagen.SetActive(true);
        yield return new WaitForSeconds(duracion);
        imagen.SetActive(false);
    }
}
