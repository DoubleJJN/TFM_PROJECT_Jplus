using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aumentar : MonoBehaviour
{
    private bool movimientoPermitido = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Solo permite el movimiento si la variable es verdadera
        if (movimientoPermitido)
        {
            // Tu lógica de movimiento aquí
            float movimientoHorizontal = Input.GetAxis("Horizontal");
            float movimientoVertical = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(movimientoHorizontal, movimientoVertical, 0) * Time.deltaTime);
        }
    }

    // Método para detener el movimiento
    public void DetenerMovimiento()
    {
        movimientoPermitido = false;
    }
}
