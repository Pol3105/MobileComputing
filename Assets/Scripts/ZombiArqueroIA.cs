using UnityEngine;
using System.Collections; 

public class ZombiArqueroIA : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaArquero = 1.5f;

    [Header("Configuración Movimiento")]
    public float velocidad = 1.0f; // Un poco más lento que los zombis normales
    public float vida = 15f;       // Muy frágil
    
    [Header("Configuración Combate")]
    public float dañoFlecha = 15f; // Pega muy duro
    public float rangoAtaque = 8f; 
    public float tiempoEntreFlechas = 2.5f; 
    public float fuerzaDisparo = 18f; 
    public float tiempoRetrasoAnimacion = 0.5f; 
    public float alturaApuntado = 0.8f; 

    [Header("Referencias")]
    public GameObject flechaEnemigaPrefab; // ⚠️ Necesitas una flecha especial para enemigos
    public Transform puntoDisparo;
    public LayerMask capaAliados; // ⚠️ Asigna aquí la capa de tus tropas y la torre
    
    [Header("UI, Audio y FX")]
    public BarraDeVida barraVidaUI;
    public AudioSource miAudioSource; 
    public AudioClip sonidoDisparo;
    public GameObject efectoSangre;

    [Header("Recompensa")]
    public int monedasAlMorir = 15;

    [Header("Estado (Solo lectura)")]
    public bool estaMuerto = false;
    private float siguienteDisparo = 0f;
    private Transform objetivoActual; 
    
    private Rigidbody2D miCuerpo;
    private Animator miAnimator;
    private float vidaMaxima;
    private ControladorJuego gameManager;

    void Start()
    {
        miCuerpo = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<ControladorJuego>();

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        Destroy(gameObject, 60f); // Limpieza de seguridad
        AplicarEscala();
    }

    void FixedUpdate()
    {
        AplicarEscala();

        if (estaMuerto) return;

        // --- BUSCAR OBJETIVO ---
        Collider2D enemigoCerca = Physics2D.OverlapCircle(transform.position, rangoAtaque, capaAliados);

        if (enemigoCerca != null)
        {
            // --- MODO COMBATE (ESPERANDO/DISPARANDO) ---
            objetivoActual = enemigoCerca.transform;
            miCuerpo.linearVelocity = Vector2.zero; // Frenar

            // 1. Le decimos que se ponga en posición de guardia (quieto)
            miAnimator.SetBool("Idle", true); 

            if (Time.time >= siguienteDisparo)
            {
                // 2. ¡Pum! Ejecutamos el ataque UNA sola vez
                miAnimator.SetTrigger("Attack"); 
                
                StartCoroutine(LogicaDisparoIA(objetivoActual));
                siguienteDisparo = Time.time + tiempoEntreFlechas;
            }
        }
        else
        {
            // --- MODO CAMINAR ---
            objetivoActual = null;
            miCuerpo.linearVelocity = Vector2.left * velocidad;
            
            // Apagamos el Idle para que vuelva a la animación de Run
            miAnimator.SetBool("Idle", false);
        }
    }
    
    void AplicarEscala()
    {
        // Forzamos la escala X a ser negativa para que mire hacia la torre (izquierda)
        transform.localScale = new Vector3(escalaArquero, escalaArquero, 1);
    }

    IEnumerator LogicaDisparoIA(Transform objetivo)
    {
        yield return new WaitForSeconds(tiempoRetrasoAnimacion);

        if (estaMuerto) yield break; 

        if (miAudioSource != null && sonidoDisparo != null)
        {
            miAudioSource.PlayOneShot(sonidoDisparo);
        }

        Vector3 posicionDestino;
        
        if (objetivo != null)
        {
            posicionDestino = objetivo.position + new Vector3(0, alturaApuntado, 0);
        }
        else
        {
            // Si el aliado murió mientras el arquero tensaba el arco, dispara recto hacia la izquierda
            posicionDestino = puntoDisparo.position + new Vector3(-5, 0, 0);
        }

        Vector2 direccionTiro = (posicionDestino - puntoDisparo.position).normalized;

        if (flechaEnemigaPrefab != null && puntoDisparo != null)
        {
            GameObject nuevaFlecha = Instantiate(flechaEnemigaPrefab, puntoDisparo.position, Quaternion.identity);
            
            // --- INYECTAR DAÑO EN LA FLECHA ENEMIGA ---
            ProyectilEnemigo scriptProyectil = nuevaFlecha.GetComponent<ProyectilEnemigo>();
            if (scriptProyectil != null)
            {
                scriptProyectil.daño = this.dañoFlecha; 
            }

            Rigidbody2D rbFlecha = nuevaFlecha.GetComponent<Rigidbody2D>();
            if (rbFlecha != null)
            {
                nuevaFlecha.transform.right = -direccionTiro; 

                nuevaFlecha.transform.Rotate(0, 0, -20f);

                rbFlecha.gravityScale = 0f; 
                rbFlecha.linearVelocity = direccionTiro * fuerzaDisparo;
            }
        }
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

        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);

        if (vida <= 0) Morir();
    }

    void Morir()
    {
        if (estaMuerto) return; // Evita que la función se ejecute dos veces

        estaMuerto = true;
        gameObject.tag = "Untagged"; 
        
        if (gameManager != null) gameManager.GanarMonedas(monedasAlMorir);

        miCuerpo.linearVelocity = Vector2.zero;
        miCuerpo.gravityScale = 0; 
        GetComponent<Collider2D>().enabled = false;
        
        // 🔥 RESET TOTAL DE PARÁMETROS 🔥
        miAnimator.SetBool("Attack", false); 
        miAnimator.SetBool("Idle", false);
        miAnimator.SetBool("Die", true); // Activamos la muerte
        
        if (barraVidaUI != null) Destroy(barraVidaUI.gameObject);

        // Destruimos el objeto para limpiar la memoria
        Destroy(gameObject, 1.5f); 
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta; // Color diferente para distinguir su rango
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }

    // --- ESCALADO PARA OLEADAS AVANZADAS ---
    public void EscalarEstadisticas(int numeroNivel)
    {
        if (numeroNivel <= 1) return;

        float multiplicador = numeroNivel - 1;

        // Sube vida y daño, pero sigue siendo un "cañón de cristal"
        vida += multiplicador * 10f; 
        dañoFlecha += multiplicador * 5f;

        vidaMaxima = vida;
        if (barraVidaUI != null) barraVidaUI.ActualizarBarra(vida, vidaMaxima);
    }


}