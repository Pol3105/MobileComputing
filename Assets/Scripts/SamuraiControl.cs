using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic; 

public class SamuraiControl : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaPersonaje = 1.5f;

    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Combate: Espada ⚔️")]
    public Transform puntoAtaque;
    public float rangoEspada = 0.8f; 
    public float danoEspada = 10f; // <--- SE MEJORARÁ AUTOMÁTICAMENTE
    public float tiempoRetrasoEspada = 0.3f; 

    [Header("Combate: Arco 🏹")]
    public Transform puntoDisparo; 
    public GameObject flechaPrefab; 
    public float rangoArco = 8f; 
    public float fuerzaDisparo = 15f; 
    public float tiempoRetrasoArco = 0.5f; 
    public float alturaApuntado = 0.8f;

    // 🔥 VARIABLE PRIVADA PARA EL DAÑO DEL ARCO 🔥
    private float danoArco = 10f; // Base 10, sube con mejoras

    [Header("General")]
    public LayerMask capaEnemigos; 
    public float tiempoEntreAtaques = 1.0f;

    private Rigidbody2D miCuerpo;
    private Animator miAnimator;
    private InputSystem_Actions controles;
    private Vector2 entradaMovimiento;
    
    private float tiempoSiguienteAtaque = 0f;
    
    [Header("Audio 🔊")]
    public AudioSource miAudioSource; 
    public AudioClip sonidoEspada;    
    public AudioClip sonidoArco;      

    void Awake()
    {
        miCuerpo = GetComponent<Rigidbody2D>();
        miAnimator = GetComponent<Animator>();
        controles = new InputSystem_Actions();

        controles.Player.Move.performed += ctx => entradaMovimiento = ctx.ReadValue<Vector2>();
        controles.Player.Move.canceled += ctx => entradaMovimiento = Vector2.zero;

        // 🔥 APLICAR MEJORAS AL DESPERTAR 🔥
        AplicarMejorasSamurai();
    }

    // --- 🚀 FUNCIÓN DE MEJORA DEL HÉROE ---
    void AplicarMejorasSamurai()
{
    // 1. MEJORA DE KATANA (Aumentamos el daño significativamente)
    int nivelKatana = DatosJugador.ObtenerNivelMejora("Katana");
    if (nivelKatana > 1)
    {
        // 🔥 Daño: Subimos de 10 a 25. 
        // En Nivel 3 tendrá +50 de daño, ideal para compensar los +200 HP de los zombis.
        danoEspada += (nivelKatana - 1) * 25f;
        
        // 🔥 Velocidad: Subimos a 0.7 para que pueda esquivar y posicionarse rápido.
        velocidad += (nivelKatana - 1) * 0.7f;
        
        Debug.Log("⚔️ Samurái Maestro Lv." + nivelKatana + " | Daño Katana: " + danoEspada);
    }

    // 2. MEJORA DE ARCO (Ahora más letal que la arquera básica)
    int nivelArco = DatosJugador.ObtenerNivelMejora("Arco");
    if (nivelArco > 1)
    {
        // 🔥 Daño: Subimos de 8 a 15.
        danoArco += (nivelArco - 1) * 15f;
        
        // 🔥 Rango: Subimos a 1.0 metro extra para disparar desde lejos.
        rangoArco += (nivelArco - 1) * 1.0f;

        Debug.Log("🏹 Samurái Arquero Lv." + nivelArco + " | Daño Flecha: " + danoArco);
    }
}

    void FixedUpdate()
    {
        // 1. MOVIMIENTO
        miCuerpo.linearVelocity = new Vector2(entradaMovimiento.x * velocidad, miCuerpo.linearVelocity.y);

        if (entradaMovimiento.x != 0)
        {
            miAnimator.SetBool("Walking", true);
            
            if (entradaMovimiento.x > 0) 
                transform.localScale = new Vector3(escalaPersonaje, escalaPersonaje, 1);
            else if (entradaMovimiento.x < 0) 
                transform.localScale = new Vector3(-escalaPersonaje, escalaPersonaje, 1);
        }
        else
        {
            miAnimator.SetBool("Walking", false);
            transform.localScale = new Vector3(escalaPersonaje, escalaPersonaje, 1);
        }

        // 2. BLOQUEO
        if (entradaMovimiento.x != 0 || 
            Time.time < tiempoSiguienteAtaque ||
            miAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || 
            miAnimator.GetCurrentAnimatorStateInfo(0).IsName("Distance"))
        {
            return; 
        }

        // 3. INTENTAR ATACAR
        IntentarAutoAtaque();
    }

    void IntentarAutoAtaque()
    {
        // PRIORIDAD 1: ESPADA
        Collider2D enemigoMuyCerca = Physics2D.OverlapCircle(puntoAtaque.position, rangoEspada, capaEnemigos);

        if (enemigoMuyCerca != null)
        {
            miAnimator.SetTrigger("Attack"); 
            StartCoroutine(LogicaEspada());
            
            miCuerpo.linearVelocity = Vector2.zero; 
            tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
            return; 
        }

        // PRIORIDAD 2: ARCO
        Collider2D[] enemigosLejos = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoArco, capaEnemigos);

        foreach (Collider2D enemigo in enemigosLejos)
        {
            if (enemigo != null && EstaEnFrente(enemigo.transform))
            {
                miAnimator.SetTrigger("Shoot");
                StartCoroutine(LogicaArco(enemigo.transform));
                
                miCuerpo.linearVelocity = Vector2.zero;
                tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                return; 
            }
        }
    }

    bool EstaEnFrente(Transform objetivo)
    {
        float miDireccion = transform.localScale.x;
        float direccionEnemigo = objetivo.position.x - puntoAtaque.position.x; 
        return (miDireccion * direccionEnemigo) > 0;
    }

    IEnumerator LogicaEspada()
    {
        if (miAudioSource != null && sonidoEspada != null)
        {
            miAudioSource.PlayOneShot(sonidoEspada); 
        }

        yield return new WaitForSeconds(tiempoRetrasoEspada);
        
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoEspada, capaEnemigos);
        
        List<GameObject> enemigosYaGolpeados = new List<GameObject>(); 

        foreach (Collider2D enemigo in enemigosGolpeados)
        {
            // Verificamos que no hayamos cortado ya a este enemigo en este mismo espadazo
            if (!enemigosYaGolpeados.Contains(enemigo.gameObject))
            {
                // 🔥 CAMBIO 2: SendMessage universal
                // "Oye tú, seas Zombi Normal, Fantasma o Arquero, cómete este daño"
                enemigo.gameObject.SendMessage("RecibirDano", danoEspada, SendMessageOptions.DontRequireReceiver);
                
                // Lo añadimos a la lista para no volver a dañarlo en este frame
                enemigosYaGolpeados.Add(enemigo.gameObject);
            }
        }
    }

    IEnumerator LogicaArco(Transform objetivo)
    {
        if (miAudioSource != null && sonidoArco != null)
        {
            miAudioSource.PlayOneShot(sonidoArco);
        }

        yield return new WaitForSeconds(tiempoRetrasoArco);

        Vector3 posicionDestino;
        
        if (objetivo != null)
        {
            posicionDestino = objetivo.position + new Vector3(0, alturaApuntado, 0); 
        }
        else
        {
            float dire = transform.localScale.x > 0 ? 1 : -1;
            posicionDestino = puntoDisparo.position + new Vector3(dire * 10, 0, 0);
        }

        Vector2 direccionFinal = (posicionDestino - puntoDisparo.position).normalized;

        GameObject nuevaFlecha = Instantiate(flechaPrefab, puntoDisparo.position, Quaternion.identity);
        
        // --- 🔥 PASAR EL DAÑO MEJORADO A LA FLECHA 🔥 ---
        Proyectil scriptFlecha = nuevaFlecha.GetComponent<Proyectil>();
        if (scriptFlecha != null)
        {
            scriptFlecha.daño = this.danoArco; // Inyectamos el daño calculado
        }
        // ------------------------------------------------

        Rigidbody2D rbFlecha = nuevaFlecha.GetComponent<Rigidbody2D>();
        
        if (rbFlecha != null)
        {
            nuevaFlecha.transform.right = direccionFinal;
            rbFlecha.gravityScale = 0f; 
            rbFlecha.linearVelocity = direccionFinal * fuerzaDisparo;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoAtaque.position, rangoEspada);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(puntoAtaque.position, rangoArco);
    }

    void OnEnable() { controles.Enable(); }
    void OnDisable() { controles?.Disable(); }
}