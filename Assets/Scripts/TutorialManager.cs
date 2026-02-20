using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class TutorialManager : MonoBehaviour
{

    [Header("REFERENCIAS GENERALES")]
    public ControladorJuego gameManager;

    [Header("CONFIGURACIÓN DE UI")]
    public GameObject[] uiSigueOculta; 

    [Header("Desbloqueos Fase 1 (Granjero)")]
    public GameObject[] uiDesbloqueoGranjero; 

    [Header("Desbloqueos Fase 2 (Arquero)")]
    public GameObject[] uiDesbloqueoArquero; 

    [Header("Diálogo")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    
    [Header("Textos del Tutorial")]
    [TextArea(2, 3)] public string[] frasesIntro;   
    [TextArea(2, 3)] public string[] frasesGranjero; 
    [TextArea(2, 3)] public string[] frasesArquero;  
    
    // VARIABLES INTERNAS
    private int indiceFrase = 0;
    
    // FASES: 0=Inicio, 1=DialogoIntro, 2=Matar1Zombie, 3=DialogoGranjero, 4=MatarHorda, 5=DialogoArquero, 6=JuegoLibre
    private int faseActual = 0; 
    
    private bool puedePasarFrase = false; 
    private float tiempoCheck = 0f;

    // --- NUEVA VARIABLE CLAVE ---
    // Sirve para no ganar la fase antes de que los enemigos salgan
    private bool enemigosHanAparecido = false; 

    void Start()
    {
        if(panelDialogo) panelDialogo.SetActive(false);
        
        GestionarVisibilidad(uiSigueOculta, false);
        GestionarVisibilidad(uiDesbloqueoGranjero, false);
        GestionarVisibilidad(uiDesbloqueoArquero, false);
    }

    void Update()
    {
        // --- DETECTAR INICIO ---
        if (faseActual == 0 && Time.timeScale == 1f) 
        {
            ComenzarDialogo(1); 
        }

        // --- CHEQUEO DE ENEMIGOS (Fase 2 y Fase 4) ---
        if (faseActual == 2 || faseActual == 4 || faseActual == 6)
        {
            tiempoCheck += Time.deltaTime;
            if (tiempoCheck > 0.5f) 
            {
                tiempoCheck = 0;
                ComprobarEnemigos();
            }
        }

        // --- PASAR DE FRASE (INPUT) ---
        if ((faseActual == 1 || faseActual == 3 || faseActual == 5) && puedePasarFrase)
        {
            bool input = false;
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) input = true;
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) input = true;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) input = true;

            if (input) SiguienteFrase();
        }
        
        // REFUERZO DE UI OCULTA
        if (faseActual < 6)
        {
             GestionarVisibilidad(uiSigueOculta, false);
             if (faseActual < 3) GestionarVisibilidad(uiDesbloqueoGranjero, false);
             if (faseActual < 5) GestionarVisibilidad(uiDesbloqueoArquero, false);
        }
    }

    void ComprobarEnemigos()
    {
        // Buscamos al generador para saber si ya soltó todo
        GeneradorEnemigos generador = FindFirstObjectByType<GeneradorEnemigos>();
        GameObject enemigo = GameObject.FindWithTag("Enemigo");
        
        if (enemigo != null) enemigosHanAparecido = true;

        // Si no hay enemigos y ya habían salido antes...
        if (enemigo == null && enemigosHanAparecido)
        {
            if (faseActual == 2) 
            {
                ComenzarDialogo(3);
            }
            else if (faseActual == 4)
            {
                ComenzarDialogo(5);
            }
            // --- NUEVA LÓGICA DE FINALIZACIÓN ---
            else if (faseActual == 6 && generador != null && generador.generadorFinalizado)
            {
                TerminarTutorial();
            }
        }
    }

    void TerminarTutorial()
    {
        faseActual = 7; // Evitamos que se repita la función
        Debug.Log("🎓 ¡Tutorial Completado con éxito!");

        // 1. Guardamos que el tutorial ya se hizo
        // Suponiendo que en DatosJugador tienes este método del Día 9
        PlayerPrefs.SetInt("TutorialCompletado", 1); 
        
        // 2. Desbloqueamos el Nivel 1 oficialmente para que aparezca en el mapa
        DatosJugador.DesbloquearNivel(0); // El nivel 0 suele ser el tutorial, desbloquea el 1

        // 3. Llamamos a la victoria del GameManager para mostrar el panel y fuegos artificiales
        if (gameManager != null)
        {
            // Llamamos a GanarNivel pasándole un "0" o un número especial para el tutorial
            gameManager.GanarNivel(0); 
        }
    }

    void ComenzarDialogo(int nuevaFase)
    {
        faseActual = nuevaFase;

        if (faseActual == 5) // Fase de los Arqueros
        {
            if (gameManager != null)
            {
                // Si el jugador tiene menos de 30 monedas...
                if (gameManager.monedasActuales < 30)
                {
                    Debug.Log("💰 Tutorial: El jugador es pobre. Regalando monedas para el Arquero.");
                    gameManager.monedasActuales = 30; // Ponemos 30 exactos (o podrías sumar)
                    
                    // IMPORTANTE: Actualizar el texto de la UI de monedas inmediatamente
                    gameManager.ActualizarTextoMonedas(); 
                }
            }
        }
        // -----------------------------------------------


        Time.timeScale = 0f; 
        panelDialogo.SetActive(true);
        indiceFrase = 0;
        MostrarFrase();
        StartCoroutine(HabilitarPaso());
    }

    void MostrarFrase()
    {
        string[] frases = null;
        if (faseActual == 1) frases = frasesIntro;
        else if (faseActual == 3) frases = frasesGranjero;
        else if (faseActual == 5) frases = frasesArquero;

        if (frases != null && indiceFrase < frases.Length)
        {
            textoDialogo.text = frases[indiceFrase];
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
        panelDialogo.SetActive(false);
        Time.timeScale = 1f; 

        // Reseteamos el "detector de aparición" para la siguiente ronda
        enemigosHanAparecido = false; 

        // LÓGICA DE TRANSICIÓN
        if (faseActual == 1)
        {
            faseActual = 2; // Matar zombie 1
            // Nota: Si el primer zombie YA estaba en escena antes del dialogo, forzamos true:
            if(GameObject.FindWithTag("Enemigo") != null) enemigosHanAparecido = true;
        }
        else if (faseActual == 3)
        {
            faseActual = 4; // Matar horda
            GestionarVisibilidad(uiDesbloqueoGranjero, true); 
            // Aquí 'enemigosHanAparecido' empieza en FALSE.
            // El script esperará pacientemente a que el Generador saque los zombies.
            // Cuando salgan, se pondrá a TRUE.
            // Cuando mueran todos, saltará el siguiente dialogo.
        }
        else if (faseActual == 5)
        {
            faseActual = 6; 
            GestionarVisibilidad(uiDesbloqueoArquero, true); 
        }
    }

    void GestionarVisibilidad(GameObject[] lista, bool estado)
    {
        foreach (GameObject obj in lista)
        {
            if(obj != null) obj.SetActive(estado);
        }
    }

    System.Collections.IEnumerator HabilitarPaso()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        puedePasarFrase = true;
    }
}