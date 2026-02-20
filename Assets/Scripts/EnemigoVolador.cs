using UnityEngine;

public class EnemigoVolador : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaFantasma = 1.5f;

    [Header("Configuración")]
    public float velocidad = 3f; 
    public float vida = 15f;
    public float daño = 20f; 

    [Header("Recompensa")]
    public int monedas = 15;

    [Header("UI")]
    public BarraDeVida barraVidaUI; // <--- ¡AQUÍ ESTÁ LA VARIABLE QUE FALTABA!
    private float vidaMaxima;

    private bool estaMuerto = false;
    private Rigidbody2D miCuerpo;
    private Animator miAnimator;
    private ControladorJuego gameManager;

    void Start()
    {
        miCuerpo = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<ControladorJuego>();
        
        vidaMaxima = vida; // Guardamos la vida inicial
        
        AplicarEscala();
    }

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerto) return;
        
        miCuerpo.linearVelocity = new Vector2(-velocidad, miCuerpo.linearVelocity.y);
    }

    void AplicarEscala()
    {
        float direccionX = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(direccionX * escalaFantasma, escalaFantasma, 1);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (estaMuerto) return;

        if (otro.CompareTag("PuertaSagrada"))
        {
            VidaTorre torre = otro.GetComponent<VidaTorre>();
            if (torre != null)
            {
                torre.RecibirDano(daño);
                Morir(false); 
            }
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;
        vida -= cantidad;

        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        if (vida <= 0)
        {
            Morir(true); 
        }
    }

    void Morir(bool darDinero)
    {
        estaMuerto = true;
        miCuerpo.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false; 

        miAnimator.SetTrigger("Die");

        if (darDinero && gameManager != null)
        {
            gameManager.GanarMonedas(monedas);
        }

        if (barraVidaUI != null) Destroy(barraVidaUI.gameObject);
        Destroy(gameObject, 0.5f); 
    }

    // --- 🚀 FUNCIÓN DE ESCALADO CORREGIDA ---
    public void EscalarEstadisticas(int numeroNivel)
    {
        if (numeroNivel <= 1) return;

        float multiplicadorDificultad = numeroNivel - 1;

        // Para el fantasma, subimos 15 de vida por nivel (en vez de 50, que sería demasiado)
        vida += multiplicadorDificultad * 15f;     
        daño += multiplicadorDificultad * 5f;      
        velocidad += multiplicadorDificultad * 0.2f; 

        vidaMaxima = vida; // Actualizamos el nuevo máximo para la barra
        
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        Debug.Log($"👻 {gameObject.name} nivel {numeroNivel} escalado. Vida: {vida}");
    }
}