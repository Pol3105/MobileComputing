using UnityEngine;

public class BombaNinja : MonoBehaviour
{
    public float velocidad = 8f;
    public float radioExplosion = 2.5f;
    public GameObject efectoExplosion;
    
    private float daño;
    private Vector2 direccion;

    public void Configurar(Transform objetivo, float dañoNinja)
    {
        daño = dañoNinja;
        direccion = (objetivo.position - transform.position).normalized;
        
        // Rotar para que mire al objetivo
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        // Atravesar el muro invisible
        if (otro.gameObject.layer == LayerMask.NameToLayer("MuroInvisible")) return;
        
        // Si toca un enemigo o el suelo, explota
        if (otro.CompareTag("Enemigo") || otro.CompareTag("Suelo"))
        {
            Explotar();
        }
    }

    void Explotar()
    {
        // 1. Efecto visual
        if (efectoExplosion != null)
        {
            GameObject fx = Instantiate(efectoExplosion, transform.position, Quaternion.identity);
            Destroy(fx, 1f);
        }

        // 2. DAÑO EN ÁREA (AoE) 🔥
        Collider2D[] enemigosAfectados = Physics2D.OverlapCircleAll(transform.position, radioExplosion);
        
        foreach (Collider2D col in enemigosAfectados)
        {
            if (col.CompareTag("Enemigo"))
            {
                // Usamos el SendMessage universal para cualquier tipo de enemigo
                col.SendMessage("RecibirDano", daño, SendMessageOptions.DontRequireReceiver);
            }
        }

        Destroy(gameObject);
    }

    // Para ver el radio en el editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}