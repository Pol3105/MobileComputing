using UnityEngine;
using System.Collections; 

public class ArqueraIA : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaArquera = 1.5f;

    [Header("Configuración Movimiento")]
    public float velocidad = 1.2f;
    public float vida = 15f;
    
    [Header("Configuración Combate")]
    public float rangoAtaque = 8f; 
    public float tiempoEntreFlechas = 2f; 
    public float fuerzaDisparo = 20f; 
    public float tiempoRetrasoAnimacion = 0.5f; 
    public float alturaApuntado = 0.8f; 

    // 🔥 ESTADÍSTICAS DINÁMICAS (Mejoras del Dojo)
    [Header("Estadísticas Dinámicas")]
    public float daño = 10f; 

    [Header("Referencias")]
    public GameObject flechaPrefab;
    public Transform puntoDisparo;
    public LayerMask capaEnemigos;
    
    [Header("UI & Audio")]
    public BarraDeVida barraVidaUI;
    public AudioSource miAudioSource; 
    public AudioClip sonidoDisparo;   

    [Header("Estado (Solo lectura)")]
    public bool estaMuerta = false;
    private float siguienteDisparo = 0f;
    private Transform objetivoActual; 
    
    private Rigidbody2D miCuerpo;
    private Animator miAnimator;
    private float vidaMaxima;

    void Start()
    {
        miCuerpo = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();

        // 1. APLICAR MEJORAS DEL DOJO
        AplicarMejoras();

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        Destroy(gameObject, 60f); 
        AplicarEscala();
    }

    void AplicarMejoras()
{
    int nivel = DatosJugador.ObtenerNivelMejora("ArqueraUnit");

    if (nivel > 1)
    {
        // 1. VIDA: Aumentamos a 15 para que aguante al menos 2 flechazos enemigos
        vida += (nivel - 1) * 15f; 
        
        // 2. DAÑO: Subimos a 12 o 15 para que pueda limpiar las hordas
        daño += (nivel - 1) * 12f; 
        
        // 3. RANGO: 0.8 es un buen equilibrio para que disparen desde antes
        rangoAtaque += (nivel - 1) * 0.8f;

        Debug.Log("🏹 Arquera Buffeada Nivel " + nivel + " | HP: " + vida + " | Daño: " + daño);
    }
}

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerta) return;

        // --- BUSCAR OBJETIVO INTELIGENTE (Prioridad Aérea) ---
        BuscarObjetivoPrioritario();

        if (objetivoActual != null)
        {
            // --- MODO COMBATE ---
            miCuerpo.linearVelocity = Vector2.zero; 

            // Si el objetivo muere o desaparece, volvemos a buscar en el siguiente frame
            // (La comprobación de null ya se hace arriba, pero esto es extra seguridad)
            if(objetivoActual == null) return;

            // Orientar visualmente si es necesario (opcional)
            
            miAnimator.SetBool("Idle", true);

            if (Time.time >= siguienteDisparo)
            {
                miAnimator.SetTrigger("Attack"); 
                
                // Pasamos el objetivo actual a la corrutina para que dispare hacia él
                StartCoroutine(LogicaDisparoIA(objetivoActual));
                siguienteDisparo = Time.time + tiempoEntreFlechas;
            }
        }
        else
        {
            // --- MODO CAMINAR ---
            miCuerpo.linearVelocity = Vector2.right * velocidad;
            miAnimator.SetBool("Idle", false);
        }
    }

    // 🔥 NUEVA FUNCIÓN DE INTELIGENCIA ARTIFICIAL 🔥
    void BuscarObjetivoPrioritario()
    {
        Collider2D[] enemigosEnRango = Physics2D.OverlapCircleAll(transform.position, rangoAtaque, capaEnemigos);
        
        Transform mejorObjetivo = null;
        float distanciaMasCorta = Mathf.Infinity;
        bool objetivoEsVolador = false; // Flag para saber si ya encontramos un avión

        foreach (Collider2D enemigo in enemigosEnRango)
        {
            if (enemigo == null) continue;

            // Detectamos si es volador buscando su script específico
            // (Asegúrate de que tus fantasmas tengan el script "EnemigoVolador" o similar)
            EnemigoVolador esVolador = enemigo.GetComponent<EnemigoVolador>();
            float dist = Vector2.Distance(transform.position, enemigo.transform.position);

            if (esVolador != null)
            {
                // ¡ES UN FANTASMA!
                
                // Si antes teníamos un objetivo terrestre, lo descartamos inmediatamente
                if (!objetivoEsVolador)
                {
                    objetivoEsVolador = true;
                    distanciaMasCorta = dist;
                    mejorObjetivo = enemigo.transform;
                }
                else
                {
                    // Si ya teníamos otro fantasma, nos quedamos con el más cercano
                    if (dist < distanciaMasCorta)
                    {
                        distanciaMasCorta = dist;
                        mejorObjetivo = enemigo.transform;
                    }
                }
            }
            else
            {
                // ES UN ZOMBI TERRESTRE
                
                // Solo nos interesa si NO hemos encontrado ningún volador todavía
                if (!objetivoEsVolador)
                {
                    if (dist < distanciaMasCorta)
                    {
                        distanciaMasCorta = dist;
                        mejorObjetivo = enemigo.transform;
                    }
                }
            }
        }

        objetivoActual = mejorObjetivo;
    }

    void AplicarEscala()
    {
        transform.localScale = new Vector3(escalaArquera, escalaArquera, 1);
    }

    IEnumerator LogicaDisparoIA(Transform objetivo)
    {
        // Esperamos al frame de la animación donde suelta la cuerda
        yield return new WaitForSeconds(tiempoRetrasoAnimacion);

        if (estaMuerta) yield break; 

        // Reproducir sonido
        if (miAudioSource != null && sonidoDisparo != null)
        {
            miAudioSource.PlayOneShot(sonidoDisparo);
        }

        // CALCULAR POSICIÓN DE TIRO
        Vector3 posicionDestino;
        
        // Verificamos si el objetivo sigue existiendo (pudo morir durante el retraso)
        if (objetivo != null)
        {
            posicionDestino = objetivo.position + new Vector3(0, alturaApuntado, 0);
        }
        else
        {
            // Si murió, disparamos recto para no cancelar la flecha
            posicionDestino = puntoDisparo.position + new Vector3(5, 0, 0);
        }

        Vector2 direccionTiro = (posicionDestino - puntoDisparo.position).normalized;

        if (flechaPrefab != null && puntoDisparo != null)
        {
            // Instanciar flecha
            GameObject nuevaFlecha = Instantiate(flechaPrefab, puntoDisparo.position, Quaternion.identity);
            
            // --- INYECTAR DAÑO MEJORADO ---
            Proyectil scriptProyectil = nuevaFlecha.GetComponent<Proyectil>();
            if (scriptProyectil != null)
            {
                scriptProyectil.daño = this.daño; 
            }

            // --- FÍSICA DE LA FLECHA ---
            Rigidbody2D rbFlecha = nuevaFlecha.GetComponent<Rigidbody2D>();
            if (rbFlecha != null)
            {
                nuevaFlecha.transform.right = direccionTiro; // Rotar flecha visualmente
                rbFlecha.gravityScale = 0f; // Flecha recta (tipo láser) para asegurar impacto aéreo
                rbFlecha.linearVelocity = direccionTiro * fuerzaDisparo;
            }
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerta) return;
        vida -= cantidad;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        if (vida <= 0) Morir();
    }

    void Morir()
    {
        estaMuerta = true;
        miCuerpo.linearVelocity = Vector2.zero;
        miCuerpo.gravityScale = 0; // Evitar que caiga raro al morir
        GetComponent<Collider2D>().enabled = false;
        
        miAnimator.SetBool("Die", true); 
        
        if (barraVidaUI != null) Destroy(barraVidaUI.gameObject);
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; // Color cian para diferenciar rango visualmente
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}