using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AjustePopUp : MonoBehaviour
{

    public Button ajuste; // Quita la instanciación del botón aquí

    public GameSetting gameSetting;

    // Start is called before the first frame update
    void Start()
    {
        // Asegúrate de que el botón esté asignado en el Inspector de Unity
        if (ajuste != null)
        {
            // Agrega un listener al evento de clic del botón, pasando la función MostrarPopUp sin paréntesis
            ajuste.onClick.AddListener(gameSetting.MostrarPopUp);
        }
        else
        {
            Debug.LogError("El botón no está asignado en el Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
