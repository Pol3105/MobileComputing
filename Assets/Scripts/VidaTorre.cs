using UnityEngine;

public class VidaTorre : MonoBehaviour
{
    [Header("Configuración")]
    public float vidaMaxima = 100f;
    public float vidaActual;
    
    [Header("Estado")]
    public bool estaDestruida = false;

    [Header("Visuales Torre")]
    public Sprite spriteIntacta;   
    public Sprite spriteDestruida; 
    private SpriteRenderer miPintor;

    // --- SECCIÓN NUEVA PARA EL FONDO ---
    [Header("Visuales Fondo")]
    public SpriteRenderer rendererDelFondo; // Arrastra aquí el objeto "Fondo" de la Jerarquía
    public Sprite imagenFondoDestruido;     // Arrastra aquí la imagen nueva del proyecto
    // -----------------------------------
    
    [Header("Conexiones")]
    public ControladorJuego miGameManager;


    void Start()
    {
        vidaActual = vidaMaxima;
        miPintor = GetComponent<SpriteRenderer>();
        
        if (spriteIntacta != null) miPintor.sprite = spriteIntacta;
    }

    public void RecibirDano(float cantidad)
    {
        if (estaDestruida) return;

        vidaActual -= cantidad;
        // Debug.Log("⛩️ Torre atacada! Vida restante: " + vidaActual);
        
        StartCoroutine(EfectoDano());

        if (vidaActual <= 0)
        {
            DestruirTorre();
        }
    }

    void DestruirTorre()
    {
        estaDestruida = true;
        vidaActual = 0;

        Debug.Log("🔥 GAME OVER: La Torre ha caído");

        // 1. Cambiar la propia Torre (lo que ya tenías)
        if (spriteDestruida != null)
        {
            miPintor.sprite = spriteDestruida;
        }
        else
        {
            miPintor.color = Color.gray; 
        }

        // --- 2. NUEVO: CAMBIAR EL FONDO ---
        if (rendererDelFondo != null && imagenFondoDestruido != null)
        {
            rendererDelFondo.sprite = imagenFondoDestruido;
        }
        // ----------------------------------

        // 3. Avisar al Manager
        if (miGameManager != null)
        {
            miGameManager.MostrarGameOver();
        }
    }

    System.Collections.IEnumerator EfectoDano()
    {
        miPintor.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (!estaDestruida) miPintor.color = Color.white;
    }
}