// MusicButton.cs — ponlo en el botón transparente
using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField] private GameObject iconoReproduciendo;  // imagen nota musical
    [SerializeField] private GameObject iconoMuteado;        // imagen nota tachada

    void Start()
    {
        ActualizarIcono();
    }

    public void AlPresionarBoton()
    {
        MusicManager.Instance.ToggleMute();
        ActualizarIcono();
    }

    private void ActualizarIcono()
    {
        bool muteado = MusicManager.Instance.IsMuted();
        iconoReproduciendo.SetActive(!muteado);
        iconoMuteado.SetActive(muteado);
    }
}
