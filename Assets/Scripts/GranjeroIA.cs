using UnityEngine;

public class GranjeroIA : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaGranjero = 1.5f;

    [Header("Configuración Base (Nivel 1)")]
    public float velocidad = 1.5f; 
    public float vida = 20f;
    public float daño = 10f;
    public float velocidadAtaque = 1f; 
    public float rangoDeteccion = 0.8f;

    [Header("Objetivos")]
    public LayerMask capaEnemigos; 

    [Header("Estado")]
    private bool estaAtacando = false;
    public bool estaMuerto = false; 
    private float siguienteAtaque = 0f;
    
    // 🔥 1. AÑADIMOS AL BOSS AL MENÚ DE VÍCTIMAS
    private ZombiIA enemigoZombi; 
    private BossFinalIA enemigoBoss; 

    private Rigidbody2D miCuerpo;
    private Animator miAnimator;

    [Header("UI")]
    public BarraDeVida barraVidaUI; 
    private float vidaMaxima;      

    void Start()
    {
        miCuerpo = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();
        
        AplicarMejoras();

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);
        
        Destroy(gameObject, 60f); 
        AplicarEscala();
    }

    void AplicarMejoras()
    {
        int nivel = DatosJugador.ObtenerNivelMejora("Granjero");

        if (nivel > 1)
        {
            vida += (nivel - 1) * 30f; 
            daño += (nivel - 1) * 10f;
            velocidad += (nivel - 1) * 0.4f;
            vidaMaxima = vida;

            Debug.Log("👨‍🌾 Granjero Veterano Nivel " + nivel + " | HP: " + vida + " | Daño: " + daño + " | Vel: " + velocidad);
        }
    }

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerto) return;

        if (estaAtacando)
        {
            miCuerpo.linearVelocity = Vector2.zero;

            // 🔥 2. COMPROBAMOS SI ALGUNO DE LOS DOS OBJETIVOS MURIÓ
            bool objetivoMuerto = (enemigoZombi == null || enemigoZombi.estaMuerto) && 
                                  (enemigoBoss == null || enemigoBoss.estaMuerto);

            if (objetivoMuerto)
            {
                DejarDeAtacar();
                return;
            }

            if (Time.time >= siguienteAtaque)
            {
                // 🔥 3. DAÑAMOS AL QUE TENGAMOS DELANTE
                if (enemigoZombi != null) enemigoZombi.RecibirDano(daño);
                else if (enemigoBoss != null) enemigoBoss.RecibirDano(daño);
                
                siguienteAtaque = Time.time + velocidadAtaque;
            }
            
            // Calculamos la distancia al objetivo que esté vivo
            Transform transformObjetivo = enemigoZombi != null ? enemigoZombi.transform : enemigoBoss.transform;

            if (Vector2.Distance(transform.position, transformObjetivo.position) > rangoDeteccion + 0.5f)
            {
                DejarDeAtacar();
            }
        }
        else
        {
            miCuerpo.linearVelocity = Vector2.right * velocidad;
            miAnimator.SetBool("Attack", false);

            Collider2D enemigoCerca = Physics2D.OverlapCircle(transform.position, rangoDeteccion, capaEnemigos);
            
            if (enemigoCerca != null)
            {
                // ¿Es un zombi normal?
                ZombiIA scriptZombi = enemigoCerca.GetComponent<ZombiIA>();
                if (scriptZombi != null && !scriptZombi.estaMuerto) 
                {
                    EmpezarAtaque(scriptZombi, null);
                }
                else
                {
                    // 🔥 4. ¿Es el Boss Final?
                    BossFinalIA scriptBoss = enemigoCerca.GetComponent<BossFinalIA>();
                    if (scriptBoss != null && !scriptBoss.estaMuerto)
                    {
                        EmpezarAtaque(null, scriptBoss);
                    }
                }
            }
        }
    }

    void AplicarEscala()
    {
        transform.localScale = new Vector3(escalaGranjero, escalaGranjero, 1);
    }

    // 🔥 5. ACTUALIZAMOS LA FUNCIÓN PARA ACEPTAR AMBOS
    void EmpezarAtaque(ZombiIA zombi, BossFinalIA boss)
    {
        estaAtacando = true;
        enemigoZombi = zombi;
        enemigoBoss = boss;

        miAnimator.SetBool("Attack", true); 
    }

    void DejarDeAtacar()
    {
        estaAtacando = false;
        enemigoZombi = null;
        enemigoBoss = null;

        miAnimator.SetBool("Attack", false);
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
        estaAtacando = false; 
        
        miAnimator.SetBool("Attack", false); 
        miAnimator.SetBool("Die", true);
        
        miCuerpo.linearVelocity = Vector2.zero;
        miCuerpo.gravityScale = 0; 

        foreach(Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        
        if (barraVidaUI != null) Destroy(barraVidaUI.gameObject);

        Destroy(gameObject, 2f); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}