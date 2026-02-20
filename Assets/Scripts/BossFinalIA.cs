using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class BossFinalIA : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaBoss = 2f; 

    [Header("Estadísticas de Jefe")]
    public float vida = 3000f; 
    public float daño = 80f;   
    public float velocidad = 1f; 
    public float velocidadAtaque = 2f;
    public float rangoDeteccion = 1.5f;

    [Header("Mecánica Especial (El Despertar)")]
    public float tiempoDeEspera = 40f; 
    public bool haDespertado = false;
    private float tiempoAparicion;

    [Header("Capas y Efectos")]
    public LayerMask capaSamurai;
    public GameObject efectoSangre;
    public BarraDeVida barraVidaUI; 
    private float vidaMaxima;

    [Header("Recompensa")]
    public int monedasAlMorir = 200; 
    private ControladorJuego gameManager;

    // --- ESTADO INTERNO ---
    private bool estaAtacando = false;
    public bool estaMuerto = false;
    private float siguienteAtaque = 0f;

    // Víctimas
    private VidaSamurai victimaSamurai; 
    private GranjeroIA victimaGranjero; 
    private ArqueraIA victimaArquera;
    private SamuraiTanqueIA victimaTanque; 
    private NinjaIA victimaNinja; 
    private bool atacandoTorre = false;

    private Animator miAnimator;
    private Rigidbody2D miCuerpo;

    void Start()
    {
        miAnimator = GetComponent<Animator>();
        miCuerpo = GetComponent<Rigidbody2D>(); 
        gameManager = FindAnyObjectByType<ControladorJuego>();

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);
        
        tiempoAparicion = Time.time; 
        
        AplicarEscala();
    }

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerto) return;

        // 🔥 NUEVO: CLAVAR AL SUELO 🔥
        // Si está dormido o está atacando, bloqueamos sus físicas horizontales para que nadie lo empuje.
        // Si está despierto y caminando, lo desbloqueamos.
        if (!haDespertado || estaAtacando)
        {
            miCuerpo.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            miCuerpo.linearVelocity = Vector2.zero; // Por si acaso
        }
        else
        {
            miCuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 🔥 MECÁNICA DE ESPERA DE 40 SEGUNDOS 🔥
        if (!haDespertado && Time.time >= tiempoAparicion + tiempoDeEspera)
        {
            haDespertado = true;
            Debug.Log("❄️ ¡El Boss ha despertado y comienza a avanzar!");
        }

        // --- LÓGICA DE COMBATE ---
        if (estaAtacando)
        {
            if (Time.time >= siguienteAtaque)
            {
                miAnimator.SetBool("Attack", true); 
                StartCoroutine(GolpeConRetraso()); 
                siguienteAtaque = Time.time + velocidadAtaque;
            }

            // Chequeo de salida
            if (!atacandoTorre)
            {
                if (victimaSamurai != null && (!victimaSamurai.estaVivo || Vector2.Distance(transform.position, victimaSamurai.transform.position) > rangoDeteccion + 1f)) PararAtaque();
                else if (victimaGranjero != null && (victimaGranjero.estaMuerto || Vector2.Distance(transform.position, victimaGranjero.transform.position) > rangoDeteccion + 1f)) PararAtaque();
                else if (victimaArquera != null && (victimaArquera.estaMuerta || Vector2.Distance(transform.position, victimaArquera.transform.position) > rangoDeteccion + 1f)) PararAtaque();
                else if (victimaTanque != null && (victimaTanque.estaMuerto || Vector2.Distance(transform.position, victimaTanque.transform.position) > rangoDeteccion + 1f)) PararAtaque();
                else if (victimaNinja != null && (victimaNinja.estaMuerto || Vector2.Distance(transform.position, victimaNinja.transform.position) > rangoDeteccion + 1f)) PararAtaque();
                else if (victimaSamurai == null && victimaGranjero == null && victimaArquera == null && victimaTanque == null && victimaNinja == null) PararAtaque();
            }
        }
        else
        {
            // RADAR SIEMPRE ACTIVO
            bool objetivoEncontrado = false;
            Vector3 centroDeteccion = transform.position + (Vector3.up * 0.5f);
            Collider2D[] cosasCerca = Physics2D.OverlapCircleAll(centroDeteccion, rangoDeteccion, capaSamurai);
            
            foreach (Collider2D cosa in cosasCerca)
            {
                if (cosa.name == "PuntoAtaque" || cosa.isTrigger) continue; 

                VidaSamurai vSam = cosa.GetComponent<VidaSamurai>();
                if (vSam != null && vSam.estaVivo) { EmpezarAtaque(vSam, null, null, null, null, false); objetivoEncontrado = true; break; }

                GranjeroIA vGra = cosa.GetComponent<GranjeroIA>();
                if (vGra != null && !vGra.estaMuerto) { EmpezarAtaque(null, vGra, null, null, null, false); objetivoEncontrado = true; break; }
                
                SamuraiTanqueIA vTan = cosa.GetComponent<SamuraiTanqueIA>();
                if (vTan != null && !vTan.estaMuerto) { EmpezarAtaque(null, null, null, vTan, null, false); objetivoEncontrado = true; break; }

                ArqueraIA vArq = cosa.GetComponent<ArqueraIA>();
                if (vArq != null && !vArq.estaMuerta) { EmpezarAtaque(null, null, vArq, null, null, false); objetivoEncontrado = true; break; }

                NinjaIA vNin = cosa.GetComponent<NinjaIA>();
                if (vNin != null && !vNin.estaMuerto) { EmpezarAtaque(null, null, null, null, vNin, false); objetivoEncontrado = true; break; }
            }

            // MOVIMIENTO SI ESTÁ DESPIERTO Y NO HAY OBJETIVO
            if (!objetivoEncontrado && haDespertado)
            {
                miCuerpo.linearVelocity = Vector2.left * velocidad;
            }
        }
    }

    void AplicarEscala()
    {
        float direccionX = Mathf.Sign(transform.localScale.x);
        transform.localScale = new Vector3(direccionX * escalaBoss, escalaBoss, 1);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (estaMuerto) return;
        if (otro.CompareTag("PuertaSagrada")) EmpezarAtaque(null, null, null, null, null, true); 
    }

    void EmpezarAtaque(VidaSamurai samurai, GranjeroIA granjero, ArqueraIA arquera, SamuraiTanqueIA tanque, NinjaIA ninja, bool esTorre)
    {
        if (estaAtacando) return; 

        estaAtacando = true;
        atacandoTorre = esTorre;
        victimaSamurai = samurai; victimaGranjero = granjero; victimaArquera = arquera; victimaTanque = tanque; victimaNinja = ninja; 
    }

    void PararAtaque()
    {
        estaAtacando = false;
        atacandoTorre = false;
        victimaSamurai = null; victimaGranjero = null; victimaArquera = null; victimaTanque = null; victimaNinja = null; 

        miAnimator.SetBool("Attack", false); 
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;
        vida -= cantidad;
        
        if (efectoSangre != null) { GameObject sangre = Instantiate(efectoSangre, transform.position, Quaternion.identity); Destroy(sangre, 1f); }
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        if (vida <= 0) Morir();
    }

    void Morir()
    {
        if (estaMuerto) return; 
        estaMuerto = true;
        gameObject.tag = "Untagged"; 

        if (gameManager != null) gameManager.GanarMonedas(monedasAlMorir);
        
        estaAtacando = false; 
        miAnimator.SetBool("Attack", false);
        miAnimator.SetTrigger("Die"); 
        
        foreach(Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        miCuerpo.gravityScale = 0; 
        miCuerpo.linearVelocity = Vector2.zero; 
        Destroy(gameObject, 5f); 
    }

    IEnumerator GolpeConRetraso()
    {
        yield return new WaitForSeconds(0.1f);
        miAnimator.SetBool("Attack", false); 

        yield return new WaitForSeconds(0.4f); 

        if (estaMuerto) yield break;

        // APLICAMOS EL DAÑO
        if (atacandoTorre)
        {
            Collider2D[] golpes = Physics2D.OverlapCircleAll(transform.position, 2f);
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
        else if (victimaGranjero != null && !victimaGranjero.estaMuerto) victimaGranjero.RecibirDano(daño);
        else if (victimaArquera != null && !victimaArquera.estaMuerta) victimaArquera.RecibirDano(daño);
        else if (victimaTanque != null && !victimaTanque.estaMuerto) victimaTanque.RecibirDano(daño);
        else if (victimaNinja != null && !victimaNinja.estaMuerto) victimaNinja.RecibirDano(daño); 
    }
}