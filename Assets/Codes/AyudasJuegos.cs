using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AyudasJuegos : MonoBehaviour
{
    public Button volver;
    // Start is called before the first frame update

    public GameSetting gameSetting;
    void Start()
    {
        volver.onClick.AddListener(VolverAlJuego);
    }

    // Update is called once per frame
    void VolverAlJuego()
    {
        int id = PlayerPrefs.GetInt("ID");
        if (id == 0)
        {
            SceneManager.LoadScene("GameRanas");
        }
        else if (id == 1)
        {
            SceneManager.LoadScene("GameMochila");
        }
        else if (id == 2)
        {
            SceneManager.LoadScene("GameLaberinto");
        }
        else if (id == 3)
        {
            SceneManager.LoadScene("GameQuiz");
        }
        else if (id == 4)
        {
            SceneManager.LoadScene("GameOrdenar");
        }
        else if (id == 5)
        {
            SceneManager.LoadScene("GameAjedrez");
        }
    }
}
