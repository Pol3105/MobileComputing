using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 
using System.Collections;

public class Nivel5_Manager : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public ControladorJuego gameManager;
    public GeneradorEnemigos generador; 

    [Header("UI DIÁLOGO")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    [Header("TEXTOS (Nivel 5 - Final)")]
    [TextArea(2, 4)] public string[] frasesIntro;

    // VARIABLES INTERNAS
    private int indiceFrase = 0;
    private int faseActual = 0; // 0=Inicio, 1=Dialogo, 2=Juego, 3=Victoria
    private bool puedePasarFrase = false;
    private bool enemigosHanAparecido = false;

    void Start()
    {
        // Ocultamos diálogo al inicio
        if(panelDialogo) panelDialogo.SetActive(false);
    }

    void Update()
    {
        // 1. INICIO AUTOMÁTICO DEL DIÁLOGO
        if (faseActual == 0 && Time.timeScale == 1f) 
        {
            ComenzarDialogo(); 
        }

        // 2. PASAR FRASES (Input)
        if (faseActual == 1 && puedePasarFrase)
        {
            bool input = false;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) input = true;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) input = true;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) input = true;

            if (input) SiguienteFrase();
        }

        // 3. CHEQUEO DE VICTORIA
        if (faseActual == 2)
        {
            ComprobarVictoria();
        }
    }

    void ComenzarDialogo()
    {
        faseActual = 1;
        Time.timeScale = 0f; // Pausamos el juego para leer la tensión final
        if(panelDialogo) panelDialogo.SetActive(true);
        
        indiceFrase = 0;
        MostrarFrase();
        StartCoroutine(HabilitarPaso());
    }

    void MostrarFrase()
    {
        if (indiceFrase < frasesIntro.Length)
            textoDialogo.text = frasesIntro[indiceFrase];
        else
            FinDelDialogo();
    }

    public void SiguienteFrase()
    {
        indiceFrase++;
        MostrarFrase();
    }

    void FinDelDialogo()
    {
        puedePasarFrase = false; 
        if(panelDialogo) panelDialogo.SetActive(false);

        Debug.Log("🔥 ¡La Batalla Final comienza! Nivel 5 en marcha.");

        // Empieza la acción
        Time.timeScale = 1f; 
        faseActual = 2; 
    }

    void ComprobarVictoria()
{
    // Buscamos si queda algún enemigo con la etiqueta
    GameObject enemigo = GameObject.FindWithTag("Enemigo");
    
    if (enemigo != null) enemigosHanAparecido = true;

    // Victoria definitiva: 
    // 1. No hay enemigos (el Boss y sus súbditos han muerto)
    // 2. Ya han salido todos los enemigos programados
    if (enemigo == null && enemigosHanAparecido && generador != null && generador.generadorFinalizado)
    {
        Debug.Log("🏆 ¡REY HELADO DERROTADO! Iniciando escena de despedida.");
        faseActual = 3; 
        
        if (gameManager != null) 
        {
            // Llamamos directamente a la función de Fin de Juego
            gameManager.FinJuego(); 
        }
    }
}

    IEnumerator HabilitarPaso()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        puedePasarFrase = true;
    }
}