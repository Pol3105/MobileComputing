using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 
using System.Collections;

public class Nivel1_Manager : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public ControladorJuego gameManager;

    [Header("UI DIÁLOGO")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;

    [Header("TEXTOS")]
    [TextArea(2, 4)] public string[] frasesIntro;

    // VARIABLES
    private int indiceFrase = 0;
    private int faseActual = 0; // 0=Inicio, 1=Dialogo, 2=Juego, 3=Fin
    private bool puedePasarFrase = false;
    private bool enemigosHanAparecido = false;
    
    // Timer para no buscar zombis 60 veces por segundo
    private float temporizadorBusqueda = 0f; 

    void Start()
    {
        if(panelDialogo) panelDialogo.SetActive(false);
    }

    void Update()
    {
        // 1. INICIO AUTOMÁTICO
        if (faseActual == 0 && Time.timeScale == 1f) 
        {
            ComenzarDialogo(); 
        }

        // 2. CONTROL DEL DIÁLOGO (Pausado)
        if (faseActual == 1 && puedePasarFrase)
        {
            bool input = false;
            // Detectamos input de cualquier fuente
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) input = true;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) input = true;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) input = true;

            if (input) SiguienteFrase();
        }

        // 3. CONTROL DE LA BATALLA (Tiempo normal)
        if (faseActual == 2)
        {
            // Optimizacion: Solo buscamos cada 0.5 segundos
            temporizadorBusqueda += Time.deltaTime;
            if (temporizadorBusqueda > 0.5f)
            {
                temporizadorBusqueda = 0f;
                ComprobarVictoria();
            }
        }
    }

    void ComenzarDialogo()
    {
        faseActual = 1;
        Time.timeScale = 0f; // Congelamos el mundo ❄️
        if(panelDialogo) panelDialogo.SetActive(true);
        
        indiceFrase = 0;
        MostrarFrase();
        StartCoroutine(HabilitarPaso());
    }

    void MostrarFrase()
    {
        if (indiceFrase < frasesIntro.Length)
        {
            textoDialogo.text = frasesIntro[indiceFrase];
        }
        else
        {
            FinDelDialogo();
        }
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
        
        // ¡ACCIÓN! 🔥
        Time.timeScale = 1f; 
        faseActual = 2; 
    }

    void ComprobarVictoria()
    {
        // Buscamos al generador en la escena
        GeneradorEnemigos generador = FindFirstObjectByType<GeneradorEnemigos>();
        
        GameObject enemigo = GameObject.FindWithTag("Enemigo");
        
        if (enemigo != null) enemigosHanAparecido = true;

        // 🔥 LA CLAVE: Solo ganas si no hay enemigos Y el generador ya terminó todo su trabajo
        if (enemigo == null && enemigosHanAparecido && generador != null && generador.generadorFinalizado)
        {
            Debug.Log("🎉 ¡Nivel 1 Superado de verdad!");
            faseActual = 3; 
            if (gameManager != null) gameManager.GanarNivel(1);
        }
    }

    // Usamos la función nativa de Unity para esperar en tiempo real (incluso en pausa)
    IEnumerator HabilitarPaso()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        puedePasarFrase = true;
    }
}