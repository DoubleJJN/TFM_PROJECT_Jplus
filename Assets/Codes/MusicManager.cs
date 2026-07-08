using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private bool isMuted = false;

    [Header("Configuración")]
    [SerializeField] private float fadeDuration = 0.5f; 
    
    private float maxVolume;
    private Coroutine fadeCoroutine;
    private bool webGLStarted = false; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
            Debug.LogError("AudioSource no tiene clip asignado");

        audioSource.loop = true;
    }

    void Start()
    {
        // Guardamos el volumen máximo, pero ¡OJO!, ya NO le damos a Play() aquí.
        maxVolume = audioSource.volume;
    }

    void Update()
    {
        // PARCHE WEBGL DEFINITIVO: 
        // Esperamos a que el usuario toque CUALQUIER tecla, haga clic o toque la pantalla
        if (!isMuted && !webGLStarted)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 || Input.anyKeyDown)
            {
                webGLStarted = true;
                
                // Ahora sí, arrancamos la música junto con la interacción del usuario
                audioSource.Play();
                
                Debug.Log("🎵 WebGL Autoplay superado: Música iniciada por interacción");
            }
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        webGLStarted = true; // Si usa el botón, ya contamos como que interactuó

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (isMuted)
        {
            fadeCoroutine = StartCoroutine(FadeAudio(0f));
        }
        else
        {
            audioSource.mute = false;
            if (!audioSource.isPlaying) audioSource.Play();
            fadeCoroutine = StartCoroutine(FadeAudio(maxVolume));
        }
    }

    private IEnumerator FadeAudio(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (targetVolume == 0f)
        {
            audioSource.mute = true;
        }
    }

    public bool IsMuted() => isMuted;
}