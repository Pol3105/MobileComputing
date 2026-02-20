using UnityEngine;

public class NinjaIA : MonoBehaviour
{
    [Header("Configuración")]
    public float vida = 50f;
    public float dañoExplosion = 40f; 
    public float velocidad = 2.5f;
    public float rangoAtaque = 6f;
    public float tiempoEntreBombas = 3.5f;

    [Header("Referencias")]
    public GameObject bombaPrefab;
    public Transform puntoDisparo;
    public LayerMask capaEnemigos;
    public BarraDeVida barraVidaUI; 

    [Header("Estado")]
    public bool estaMuerto = false;
    private float siguienteBomba = 0f;
    private float vidaMaxima;
    private Transform objetivoActual;

    private Animator miAnimator;
    private Rigidbody2D rb;

    void Start()
    {
        miAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        AplicarMejorasNinja(); 

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima); 

        Destroy(gameObject, 60f); 
    }

    void Update() 
    {
        if (estaMuerto) return;

        BuscarObjetivoCercano();

        if (objetivoActual != null)
        {
            miAnimator.SetBool("Idle", true);

            if (Time.time >= siguienteBomba)
            {
                miAnimator.SetTrigger("Attack");
                LanzarBomba(objetivoActual);
                siguienteBomba = Time.time + tiempoEntreBombas;
            }
        }
        else
        {
            miAnimator.SetBool("Idle", false);
        }
    }

    void FixedUpdate() 
    {
        if (estaMuerto) return;

        if (objetivoActual != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = Vector2.right * velocidad;
        }
    }

    void BuscarObjetivoCercano()
    {
        Collider2D[] enemigos = Physics2D.OverlapCircleAll(transform.position, rangoAtaque, capaEnemigos);
        float distanciaCercana = Mathf.Infinity;
        Transform masCercano = null;

        foreach (Collider2D enemigo in enemigos)
        {
            // 🔥 FILTRO ANTIAÉREO: Ignorar a los fantasmas 🔥
            // Si el enemigo tiene el script "EnemigoVolador", pasamos al siguiente.
            if (enemigo.GetComponent<EnemigoVolador>() != null)
            {
                continue; 
            }

            float dist = Vector2.Distance(transform.position, enemigo.transform.position);
            if (dist < distanciaCercana)
            {
                distanciaCercana = dist;
                masCercano = enemigo.transform;
            }
        }
        objetivoActual = masCercano;
    }

    void LanzarBomba(Transform objetivo)
    {
        GameObject proyectil = Instantiate(bombaPrefab, puntoDisparo.position, Quaternion.identity);
        BombaNinja scriptBomba = proyectil.GetComponent<BombaNinja>();
        if (scriptBomba != null) scriptBomba.Configurar(objetivo, dañoExplosion);
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;
        vida -= cantidad;
        
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima); 

        if (vida <= 0) Morir();
    }

    void Morir()
    {
        estaMuerto = true;
        
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f; 
        
        GetComponent<Collider2D>().enabled = false;
        
        miAnimator.SetTrigger("Die");
        
        if (barraVidaUI != null) Destroy(barraVidaUI.gameObject); 
        Destroy(gameObject, 2f);
    }
    
    public void AplicarMejorasNinja()
    {
        int nivel = DatosJugador.ObtenerNivelMejora("Ninja");
        if (nivel > 1)
        {
            vida += (nivel - 1) * 20f;
            dañoExplosion += (nivel - 1) * 15f;
            vidaMaxima = vida;
        }
    }
}