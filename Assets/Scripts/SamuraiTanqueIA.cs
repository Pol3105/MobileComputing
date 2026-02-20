using UnityEngine;

public class SamuraiTanqueIA : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaTanque = 1.8f;

    [Header("Configuración Base")]
    public float velocidad = 0.8f;   
    public float vida = 250f;        
    public float daño = 5f;          
    public float velocidadAtaque = 1.5f; 
    public float rangoDeteccion = 1.0f; 

    [Header("Objetivos")]
    public LayerMask capaEnemigos; 

    [Header("Estado")]
    private bool estaAtacando = false; 
    public bool estaMuerto = false; 
    private float siguienteAtaque = 0f;
    
    // 🔥 AÑADIMOS A LOS 3 TIPOS DE ENEMIGOS AL MENÚ
    private ZombiIA enemigoZombi; 
    private ZombiArqueroIA enemigoArquero;
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
        
        Destroy(gameObject, 120f); 
        AplicarEscala();
    }

    void AplicarMejoras()
    {
        int nivel = DatosJugador.ObtenerNivelMejora("Tanque");

        if (nivel > 1)
        {
            vida += (nivel - 1) * 150f; 
            daño += (nivel - 1) * 10f;
            vidaMaxima = vida;

            Debug.Log("🛡️ Ronin Reforzado Nivel " + nivel + " | HP: " + vida + " | Daño: " + daño);
        }
    }

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerto) return;

        if (estaAtacando)
        {
            miCuerpo.linearVelocity = Vector2.zero;

            // 🔥 COMPROBAMOS SI EL OBJETIVO ACTUAL HA MUERTO
            bool objetivoMuerto = true;
            if (enemigoZombi != null && !enemigoZombi.estaMuerto) objetivoMuerto = false;
            else if (enemigoArquero != null && !enemigoArquero.estaMuerto) objetivoMuerto = false;
            else if (enemigoBoss != null && !enemigoBoss.estaMuerto) objetivoMuerto = false;

            if (objetivoMuerto)
            {
                DejarDeAtacar();
                return;
            }

            // --- APLICAR DAÑO ---
            if (Time.time >= siguienteAtaque)
            {
                if (enemigoZombi != null) enemigoZombi.RecibirDano(daño);
                else if (enemigoArquero != null) enemigoArquero.RecibirDano(daño);
                else if (enemigoBoss != null) enemigoBoss.RecibirDano(daño);
                
                siguienteAtaque = Time.time + velocidadAtaque;
            }
            
            // --- COMPROBAR DISTANCIA ---
            Transform transformObjetivo = null;
            if (enemigoZombi != null) transformObjetivo = enemigoZombi.transform;
            else if (enemigoArquero != null) transformObjetivo = enemigoArquero.transform;
            else if (enemigoBoss != null) transformObjetivo = enemigoBoss.transform;
            
            if (transformObjetivo != null && Vector2.Distance(transform.position, transformObjetivo.position) > rangoDeteccion + 0.5f)
            {
                DejarDeAtacar();
            }
        }
        else
        {
            // --- MOVIMIENTO ---
            miCuerpo.linearVelocity = Vector2.right * velocidad;
            miAnimator.SetBool("Attack", false);

            // 🔥 DETECCIÓN MEJORADA: BUSCAMOS A TODOS Y ELEGIMOS AL PRIMERO VÁLIDO
            Collider2D[] enemigosCerca = Physics2D.OverlapCircleAll(transform.position, rangoDeteccion, capaEnemigos);
            
            foreach (Collider2D enemigo in enemigosCerca)
            {
                // Ignoramos triggers o cosas raras (como el área de ataque del boss)
                if (enemigo.isTrigger) continue;

                ZombiIA scriptZombi = enemigo.GetComponent<ZombiIA>();
                if (scriptZombi != null && !scriptZombi.estaMuerto) 
                {
                    EmpezarAtaque(scriptZombi, null, null);
                    break; // Dejamos de buscar
                }

                ZombiArqueroIA scriptArquero = enemigo.GetComponent<ZombiArqueroIA>();
                if (scriptArquero != null && !scriptArquero.estaMuerto)
                {
                    EmpezarAtaque(null, scriptArquero, null);
                    break;
                }

                BossFinalIA scriptBoss = enemigo.GetComponent<BossFinalIA>();
                if (scriptBoss != null && !scriptBoss.estaMuerto)
                {
                    EmpezarAtaque(null, null, scriptBoss);
                    break;
                }
            }
        }
    }

    // 🔥 ACTUALIZAMOS LA FUNCIÓN PARA ACEPTAR A LOS 3 TIPOS
    void EmpezarAtaque(ZombiIA zombi, ZombiArqueroIA arquero, BossFinalIA boss)
    {
        estaAtacando = true;
        enemigoZombi = zombi;
        enemigoArquero = arquero;
        enemigoBoss = boss;
        
        miCuerpo.linearVelocity = Vector2.zero;
        miAnimator.SetBool("Attack", true); 
    }

    void DejarDeAtacar()
    {
        estaAtacando = false;
        enemigoZombi = null;
        enemigoArquero = null;
        enemigoBoss = null;

        miAnimator.SetBool("Attack", false);
    }

    void AplicarEscala()
    {
        transform.localScale = new Vector3(escalaTanque, escalaTanque, 1);
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
        miAnimator.SetTrigger("Die"); 
        
        miCuerpo.linearVelocity = Vector2.zero;
        miCuerpo.gravityScale = 0; 

        foreach(Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        
        if (barraVidaUI != null) Destroy(barraVidaUI.gameObject);
        Destroy(gameObject, 2f); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; 
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}