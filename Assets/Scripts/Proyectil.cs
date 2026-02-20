using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float escalaProyectil = 1.5f; // ¡Pon aquí 1.5 o 2 para verlas bien grandes!

    [Header("Combate")]
    public float daño = 10f;
    public float tiempoDeVida = 3f;

    private Rigidbody2D rb;
    private bool haGolpeado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 1. ✅ APLICAR TAMAÑO (Activado de nuevo)
        // Como ahora rotamos la flecha para apuntar, esto no da problemas.
        transform.localScale = new Vector3(escalaProyectil, escalaProyectil, 1);

        // 2. (La velocidad la dejamos quieta para que mande el Arquero/Samurai)

        // 3. Autodestrucción
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        if (haGolpeado) return;

        // FÍSICA VISUAL (Rotación)
        // La flecha gira sola mirando hacia donde viaja
        if (rb.linearVelocity != Vector2.zero)
        {
            float angulo = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (haGolpeado) return; 

        // LÓGICA DE IMPACTO
        if (otro.gameObject.layer == LayerMask.NameToLayer("Enemigo") || otro.CompareTag("Enemigo"))
        {
            // 🔥 EL TRUCO PROFESIONAL (SendMessage) 🔥
            otro.gameObject.SendMessage("RecibirDano", daño, SendMessageOptions.DontRequireReceiver);
            Impactar(); 
        }
        else if (otro.CompareTag("Suelo")) 
        {
            // 🔥 NUEVO: Filtro para atravesar la PARED INVISIBLE 🔥
            // Si el objeto tiene la capa "MuroInvisible", ignoramos el impacto y la flecha sigue
            if (otro.gameObject.layer == LayerMask.NameToLayer("MuroInvisible"))
            {
                return; 
            }
            
            // Si es un suelo normal, chocamos
            Impactar(); 
        }
    }

    void Impactar()
    {
        haGolpeado = true;
        Destroy(gameObject);
    }
}