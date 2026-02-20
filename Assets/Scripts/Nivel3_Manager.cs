using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 
using System.Collections;

public class Nivel3_Manager : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public ControladorJuego gameManager;
    public GeneradorEnemigos generador; 

    [Header("UI DIÁLOGO")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    [Header("TEXTOS (Nivel 3)")]
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
        Time.timeScale = 0f; // Pausamos el juego para leer
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

        Debug.Log("🏹 ¡Empieza la lluvia de flechas! Nivel 3 en marcha.");

        // Empieza la acción
        Time.timeScale = 1f; 
        faseActual = 2; 
    }

    void ComprobarVictoria()
    {
        GameObject enemigo = GameObject.FindWithTag("Enemigo");
        
        if (enemigo != null) enemigosHanAparecido = true;

        // Victoria: No hay enemigos Y el generador ya terminó
        if (enemigo == null && enemigosHanAparecido && generador != null && generador.generadorFinalizado)
        {
            Debug.Log("🎉 ¡Nivel 3 Superado!");
            faseActual = 3; 
            
            // Le pasamos el Nivel 3 al GameManager
            if (gameManager != null) gameManager.GanarNivel(3); 
        }
    }

    IEnumerator HabilitarPaso()
    {
        // Pequeño retraso para no saltar la primera frase por accidente
        yield return new WaitForSecondsRealtime(0.5f);
        puedePasarFrase = true;
    }
}