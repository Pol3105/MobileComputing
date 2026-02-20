using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.InputSystem; 
using System.Collections;

public class Nivel4_Manager : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public ControladorJuego gameManager;
    public GeneradorEnemigos generador; 

    [Header("NUEVA TROPA: NINJA")]
    public Button botonInvocarNinja; // Arrastra el botón del Ninja del Canvas
    public GameObject candadoNinjaVisual; // El candado sobre el botón

    [Header("UI DIÁLOGO")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    [Header("TEXTOS")]
    [TextArea(2, 4)] public string[] frasesIntro;

    // VARIABLES INTERNAS
    private int indiceFrase = 0;
    private int faseActual = 0; // 0=Inicio, 1=Dialogo, 2=Juego, 3=Victoria
    private bool puedePasarFrase = false;
    private bool enemigosHanAparecido = false;

    void Start()
    {
        // 1. El Ninja empieza bloqueado hasta que termine la charla
        if(botonInvocarNinja != null) botonInvocarNinja.interactable = false;
        if(candadoNinjaVisual != null) candadoNinjaVisual.SetActive(true);

        if(panelDialogo) panelDialogo.SetActive(false);
    }

    void Update()
    {
        // 1. INICIO DEL DIÁLOGO
        if (faseActual == 0 && Time.timeScale == 1f) 
        {
            ComenzarDialogo(); 
        }

        // 2. CONTROL DE FRASES
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
        Time.timeScale = 0f; 
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

        // 🔥 DESBLOQUEO DEL NINJA 🔥
        if(botonInvocarNinja != null) botonInvocarNinja.interactable = true;
        if(candadoNinjaVisual != null) candadoNinjaVisual.SetActive(false);
        
        Debug.Log("💣 ¡Ninja Artificiero disponible! Cuidado con las explosiones.");

        Time.timeScale = 1f; 
        faseActual = 2; 
    }

    void ComprobarVictoria()
    {
        GameObject enemigo = GameObject.FindWithTag("Enemigo");
        
        if (enemigo != null) enemigosHanAparecido = true;

        // Victoria Nivel 4
        if (enemigo == null && enemigosHanAparecido && generador != null && generador.generadorFinalizado)
        {
            Debug.Log("🎉 ¡Nivel 4 Superado!");
            faseActual = 3; 
            if (gameManager != null) gameManager.GanarNivel(4); 
        }
    }

    IEnumerator HabilitarPaso()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        puedePasarFrase = true;
    }
}