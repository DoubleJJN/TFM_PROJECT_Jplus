using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class BotonVolver : MonoBehaviour
{
    public Button botonVolver;

    void Start()
    {
        // Agregar un listener al evento de clic del botón
        botonVolver.onClick.AddListener(Back);
    }

    void Back()
    {
        // Cargar la escena del menú inicial
        SceneManager.LoadScene("MenuInicial");
    }
}
