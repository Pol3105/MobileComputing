using UnityEngine;

public class ZombiIA : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaZombi = 1.5f;

    [Header("Configuración")]
    public float velocidad = 2f;
    public float vida = 30f;
    public float daño = 10f;
    public float velocidadAtaque = 1f;
    public float rangoDeteccion = 0.8f; 

    [Header("Capas")]
    public LayerMask capaSamurai; 

    [Header("Estado")]
    public bool estaAtacando = false;
    public bool estaMuerto = false;
    private float siguienteAtaque = 0f;

    // --- EL MENÚ DEL ZOMBI (Tipos de Víctima) ---
    private VidaSamurai victimaSamurai; 
    private GranjeroIA victimaGranjero; 
    private ArqueraIA victimaArquera;
    private SamuraiTanqueIA victimaTanque; 
    private NinjaIA victimaNinja; // <--- 🔥 1. AÑADIMOS AL BOMBER (NINJA)
    private bool atacandoTorre = false;

    [Header("Efectos Visuales")]
    public GameObject efectoSangre;

    private Animator miAnimator;
    private Rigidbody2D miCuerpo;

    [Header("UI")]
    public BarraDeVida barraVidaUI; 
    private float vidaMaxima; 

    [Header("Recompensa")]
    public int monedasAlMorir = 10;
    public ControladorJuego gameManager; 

    void Start()
    {
        miAnimator = GetComponent<Animator>();
        miCuerpo = GetComponent<Rigidbody2D>();

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<ControladorJuego>();
        }

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);
        
        AplicarEscala();
    }

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerto) return;

        if (estaAtacando)
        {
            // 1. FRENAR
            miCuerpo.linearVelocity = Vector2.zero;

            // 2. LÓGICA DE DAÑO
            if (Time.time >= siguienteAtaque)
            {
                if (atacandoTorre)
                {
                    Collider2D[] golpes = Physics2D.OverlapCircleAll(transform.position, 1.5f);
                    foreach(Collider2D golpe in golpes)
                    {
                        if (golpe.CompareTag("PuertaSagrada"))
                        {
                            VidaTorre scriptTorre = golpe.GetComponent<VidaTorre>();
                            if (scriptTorre != null) scriptTorre.RecibirDano(daño);
                        }
                    }
                }
                else if (victimaSamurai != null && victimaSamurai.estaVivo) victimaSamurai.RecibirDano(daño);
                else if (victimaGranjero != null) victimaGranjero.RecibirDano(daño);
                else if (victimaArquera != null && !victimaArquera.estaMuerta) victimaArquera.RecibirDano(daño);
                else if (victimaTanque != null && !victimaTanque.estaMuerto) victimaTanque.RecibirDano(daño);
                else if (victimaNinja != null && !victimaNinja.estaMuerto) victimaNinja.RecibirDano(daño); // <--- 🔥 2. EL ZOMBI LE HACE DAÑO

                siguienteAtaque = Time.time + velocidadAtaque;
            }

            // 3. COMPROBACIÓN DE SALIDA (¿Siguen vivos o cerca?)
            if (!atacandoTorre)
            {
                if (victimaSamurai != null && (!victimaSamurai.estaVivo || Vector2.Distance(transform.position, victimaSamurai.transform.position) > rangoDeteccion + 1f))
                    PararAtaque();
                else if (victimaGranjero != null && (victimaGranjero.estaMuerto || Vector2.Distance(transform.position, victimaGranjero.transform.position) > rangoDeteccion + 1f))
                    PararAtaque();
                else if (victimaArquera != null && (victimaArquera.estaMuerta || Vector2.Distance(transform.position, victimaArquera.transform.position) > rangoDeteccion + 1f))
                    PararAtaque();
                else if (victimaTanque != null && (victimaTanque.estaMuerto || Vector2.Distance(transform.position, victimaTanque.transform.position) > rangoDeteccion + 1f)) 
                    PararAtaque();
                else if (victimaNinja != null && (victimaNinja.estaMuerto || Vector2.Distance(transform.position, victimaNinja.transform.position) > rangoDeteccion + 1f)) // <--- 🔥 3. SI EL BOMBER MUERE, EL ZOMBI AVANZA
                    PararAtaque();
                else if (victimaSamurai == null && victimaGranjero == null && victimaArquera == null && victimaTanque == null && victimaNinja == null)
                    PararAtaque();
            }
        }
        else
        {
            // CAMINAR HACIA LA IZQUIERDA
            miCuerpo.linearVelocity = Vector2.left * velocidad;

            // --- RADAR OMNÍVORO ---
            Vector3 centroDeteccion = transform.position + (Vector3.up * 0.5f);
            Collider2D[] cosasCerca = Physics2D.OverlapCircleAll(centroDeteccion, rangoDeteccion, capaSamurai);
            
            foreach (Collider2D cosa in cosasCerca)
            {
                if (cosa.name == "PuntoAtaque" || cosa.isTrigger) continue; 

                // 1. ¿Es un Samurái?
                VidaSamurai scriptVida = cosa.GetComponent<VidaSamurai>();
                if (scriptVida != null && scriptVida.estaVivo)
                {
                    EmpezarAtaque(scriptVida, null, null, null, null, false);
                    break; 
                }

                // 2. ¿Es un Granjero?
                GranjeroIA scriptGranjero = cosa.GetComponent<GranjeroIA>();
                if (scriptGranjero != null && !scriptGranjero.estaMuerto)
                {
                    EmpezarAtaque(null, scriptGranjero, null, null, null, false); 
                    break;
                }
                
                // 3. ¿Es un Tanque (Ronin)?
                SamuraiTanqueIA scriptTanque = cosa.GetComponent<SamuraiTanqueIA>();
                if (scriptTanque != null && !scriptTanque.estaMuerto)
                {
                    EmpezarAtaque(null, null, null, scriptTanque, null, false);
                    break;
                }

                // 4. ¿Es una Arquera?
                ArqueraIA scriptArquera = cosa.GetComponent<ArqueraIA>();
                if (scriptArquera != null && !scriptArquera.estaMuerta)
                {
                    EmpezarAtaque(null, null, scriptArquera, null, null, false);
                    break;
                }

                // 5. ¿Es un Bomber/Ninja? <--- 🔥 4. EL RADAR DETECTA AL BOMBER
                NinjaIA scriptNinja = cosa.GetComponent<NinjaIA>();
                if (scriptNinja != null && !scriptNinja.estaMuerto)
                {
                    EmpezarAtaque(null, null, null, null, scriptNinja, false);
                    break;
                }
            }
        }
    }

    void AplicarEscala()
    {
        float direccionX = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(direccionX * escalaZombi, escalaZombi, 1);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (estaMuerto) return;

        if (otro.CompareTag("PuertaSagrada"))
        {
            EmpezarAtaque(null, null, null, null, null, true); 
        }
    }

    // 🔥 5. ACTUALIZAMOS LA FUNCIÓN PARA QUE RECIBA AL BOMBER
    void EmpezarAtaque(VidaSamurai samurai, GranjeroIA granjero, ArqueraIA arquera, SamuraiTanqueIA tanque, NinjaIA ninja, bool esTorre)
    {
        estaAtacando = true;
        atacandoTorre = esTorre;
        victimaSamurai = samurai;
        victimaGranjero = granjero;
        victimaArquera = arquera;
        victimaTanque = tanque; 
        victimaNinja = ninja; // <--- LO GUARDAMOS AQUÍ

        miAnimator.SetBool("Attack", true);
        miCuerpo.linearVelocity = Vector2.zero;
    }

    void PararAtaque()
    {
        estaAtacando = false;
        atacandoTorre = false;
        victimaSamurai = null;
        victimaGranjero = null;
        victimaArquera = null;
        victimaTanque = null; 
        victimaNinja = null; // <--- LO SOLTAMOS AQUÍ

        miAnimator.SetBool("Attack", false);
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;
        
        vida -= cantidad;
        
        if (efectoSangre != null)
        {
            GameObject sangre = Instantiate(efectoSangre, transform.position, Quaternion.identity);
            Destroy(sangre, 1f); 
        }

        if (barraVidaUI != null)
        {
            barraVidaUI.ActualizarBarra(vida, vidaMaxima);
        }

        if (vida <= 0) Morir();
    }

    void Morir()
    {
        if (estaMuerto) return; 

        estaMuerto = true;
        gameObject.tag = "Untagged";

        if (gameManager != null)
        {
            gameManager.GanarMonedas(monedasAlMorir);
        }
        
        estaAtacando = false; 
        miAnimator.SetBool("Attack", false);
        miAnimator.SetTrigger("Die"); 
        
        foreach(Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        
        miCuerpo.gravityScale = 0; 
        miCuerpo.linearVelocity = Vector2.zero; 
        
        Destroy(gameObject, 3f);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * 0.5f), rangoDeteccion);
    }

    public void EscalarEstadisticas(int numeroNivel)
    {
        if (numeroNivel <= 1) return;

        float multiplicador = numeroNivel - 1;

        vida += multiplicador * 100f; 
        daño += multiplicador * 10f;
        velocidad += multiplicador * 0.15f;

        // 🔥 6. ARREGLADO EL BUG DE LA VIDA MÁXIMA PARA LA UI
        vidaMaxima = vida; 
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);
        
        Debug.Log($"🧟 Zombi Reforzado Lv.{numeroNivel} | HP: {vida}");
    }
}