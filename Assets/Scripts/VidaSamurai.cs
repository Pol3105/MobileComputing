using UnityEngine;
using UnityEngine.UI;

public class VidaSamurai : MonoBehaviour
{
    [Header("Configuración Base")]
    public float vidaMaxima = 100f; // <--- ESTO SUBIRÁ CON MEJORAS
    public bool estaVivo = true;

    [Header("Referencias UI")]
    public Slider barraVidaUI;

    private float vidaActual;
    
    private Animator miAnimator;
    private SpriteRenderer miSprite;
    private Rigidbody2D miCuerpo;
    
    [Header("Conexiones")]
    public ControladorJuego miGameManager;

    void Start()
    {
        miAnimator = GetComponent<Animator>();
        miSprite = GetComponent<SpriteRenderer>();
        miCuerpo = GetComponent<Rigidbody2D>();

        // 🔥 1. APLICAR MEJORA DE SALUD 🔥
        AplicarMejoraVida();
        
        // 2. Inicializar vida
        vidaActual = vidaMaxima;
        
        // 3. Configurar UI
        if (barraVidaUI != null)
        {
            barraVidaUI.maxValue = vidaMaxima;
            barraVidaUI.value = vidaActual;
        }
    }

    // --- 🚀 FUNCIÓN DE MEJORA DE SALUD ---
    void AplicarMejoraVida()
    {
        int nivelVida = DatosJugador.ObtenerNivelMejora("Vida");
        
        if (nivelVida > 1)
        {
            // 🔥 BUFO MASIVO: +150 de vida extra por cada nivel 🔥
            // Nivel 1 = 100
            // Nivel 2 = 250
            // Nivel 3 = 400
            // Nivel 4 = 550...
            float vidaExtra = (nivelVida - 1) * 250f;
            
            vidaMaxima += vidaExtra;
            
            Debug.Log("❤️ Samurái Titánico Lv." + nivelVida + " (Max HP: " + vidaMaxima + ")");
        }
    }


    // -------------------------------------

    public void RecibirDano(float cantidad)
    {
        if (!estaVivo) return;

        vidaActual -= cantidad;
        
        if (barraVidaUI != null)
        {
            barraVidaUI.value = vidaActual;
        }

        StartCoroutine(EfectoDanoVisual());

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        estaVivo = false;
        vidaActual = 0;
        
        if (barraVidaUI != null) barraVidaUI.value = 0;

        Debug.Log("💀 ¡El Samurái ha caído!");

        miAnimator.SetBool("Die", true);
        miCuerpo.linearVelocity = Vector2.zero; 
        
        // Desactivamos el control para que no pueda atacar muerto
        GetComponent<SamuraiControl>().enabled = false;
        
        if (miGameManager != null)
        {
            miGameManager.MostrarGameOver();
        }
    }

    System.Collections.IEnumerator EfectoDanoVisual()
    {
        miSprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        miSprite.color = Color.white;
    }
}