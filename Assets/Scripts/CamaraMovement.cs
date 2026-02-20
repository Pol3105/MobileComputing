using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;

public class CamaraSeguimiento : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo; 
    public Vector3 desfase = new Vector3(0, 2, 0); 
    public float velocidadSuave = 5f; 

    [Header("Configuración Intro 🎬")]
    public float zoomIntro = 12f; 
    public float zoomJuego = 5f;  
    public Vector3 posicionIntro; 
    public GameObject mensajeTexto; 
    public GameObject interfazJuego; 
    public float velocidadTransicion = 2f;

    [Header("Game Over ☠️")]
    public GameObject panelGameOver; 
    public SpriteRenderer fondoEscenario; 
    public Sprite imagenFondoRoto;        
    public float velocidadGameOver = 2f; 
    public float tiempoEsperaMuerte = 1.5f; 

    [Header("Victoria 🏆")]
    public GameObject panelVictoria;      // El cartel de WIN
    public GameObject fuegosArtificiales; // Tu sistema de partículas (cohetes)
    public float tiempoEsperaVictoria = 1f; // Tiempo para ver los cohetes antes de alejarse

    private Camera miCamara;
    private bool enModoIntro = true;
    private bool secuenciaIniciada = false;
    private bool partidaTerminada = false; // Usamos esto para bloquear todo (Win o Lose)

    void Start()
    {
        miCamara = GetComponent<Camera>();

        // Inicio
        miCamara.orthographicSize = zoomIntro;
        transform.position = new Vector3(posicionIntro.x, posicionIntro.y, -10);

        if (mensajeTexto != null) mensajeTexto.SetActive(true);
        if (interfazJuego != null) interfazJuego.SetActive(false);
        
        // Apagamos paneles y efectos
        if (panelGameOver != null) panelGameOver.SetActive(false); 
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (fuegosArtificiales != null) fuegosArtificiales.SetActive(false);

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (enModoIntro && !secuenciaIniciada && !partidaTerminada) 
        {
            bool tocado = false;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) tocado = true;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) tocado = true;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) tocado = true;

            if (tocado)
            {
                secuenciaIniciada = true; 
                StartCoroutine(SecuenciaEntrada());
            }
        }
    }

    void LateUpdate()
    {
        if (!enModoIntro && !partidaTerminada && objetivo != null)
        {
            Vector3 destino = new Vector3(objetivo.position.x, objetivo.position.y, -10) + desfase;
            transform.position = Vector3.Lerp(transform.position, destino, velocidadSuave * Time.unscaledDeltaTime);
        }
    }

    IEnumerator SecuenciaEntrada()
    {
        if (mensajeTexto != null) mensajeTexto.SetActive(false);
        
        float tiempo = 0f;
        Vector3 posInicial = transform.position;
        float zoomInicial = miCamara.orthographicSize;

        while (tiempo < velocidadTransicion)
        {
            tiempo += Time.unscaledDeltaTime; 
            float t = tiempo / velocidadTransicion;
            float tSuave = t * t * (3f - 2f * t);

            Vector3 destino = new Vector3(objetivo.position.x, objetivo.position.y, -10) + desfase;
            transform.position = Vector3.Lerp(posInicial, destino, tSuave);
            miCamara.orthographicSize = Mathf.Lerp(zoomInicial, zoomJuego, tSuave);

            yield return null;
        }

        if (interfazJuego != null) interfazJuego.SetActive(true);
        Time.timeScale = 1f; 
        enModoIntro = false;
    }

    // --- GAME OVER ---
    public void ActivarGameOver()
    {
        if (partidaTerminada) return; 
        partidaTerminada = true;
        StartCoroutine(SecuenciaGameOver());
    }

    IEnumerator SecuenciaGameOver()
    {
        if (interfazJuego != null) interfazJuego.SetActive(false);
        yield return new WaitForSecondsRealtime(tiempoEsperaMuerte);

        if (fondoEscenario != null && imagenFondoRoto != null)
            fondoEscenario.sprite = imagenFondoRoto;

        Time.timeScale = 0f;
        yield return StartCoroutine(HacerZoomOut()); // Reutilizamos el viaje

        if (panelGameOver != null) panelGameOver.SetActive(true);
    }

    // --- VICTORIA 🏆 ---
    public void ActivarVictoria()
    {
        if (partidaTerminada) return; 
        partidaTerminada = true;
        StartCoroutine(SecuenciaVictoria());
    }

    IEnumerator SecuenciaVictoria()
    {
        // 1. Quitamos botones molestos
        if (interfazJuego != null) interfazJuego.SetActive(false);

        // 2. ¡LANZAR COHETES! 🚀
        if (fuegosArtificiales != null) fuegosArtificiales.SetActive(true);

        // 3. Esperamos un poco viendo la fiesta
        yield return new WaitForSecondsRealtime(tiempoEsperaVictoria);

        // 4. Congelamos personajes (opcional, si quieres que sigan celebrando, quita esta linea)
        Time.timeScale = 0f;

        // 5. Viaje de cámara hacia atrás
        yield return StartCoroutine(HacerZoomOut());

        // 6. ¡WIN!
        if (panelVictoria != null) panelVictoria.SetActive(true);
    }

    // --- FUNCIÓN DE VIAJE COMPARTIDA (Para no repetir código) ---
    IEnumerator HacerZoomOut()
    {
        float tiempo = 0f;
        Vector3 posActual = transform.position;
        float zoomActual = miCamara.orthographicSize;
        Vector3 posDestino = new Vector3(posicionIntro.x, posicionIntro.y, -10);

        // Usamos la velocidad de GameOver para ambos (o crea una variable nueva si quieres)
        while (tiempo < velocidadGameOver)  
        {
            tiempo += Time.unscaledDeltaTime; 
            float t = tiempo / velocidadGameOver;
            float tSuave = t * t * (3f - 2f * t); 

            transform.position = Vector3.Lerp(posActual, posDestino, tSuave);
            miCamara.orthographicSize = Mathf.Lerp(zoomActual, zoomIntro, tSuave);

            yield return null;
        }
    }
}