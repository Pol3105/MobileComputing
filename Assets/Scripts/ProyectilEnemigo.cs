using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    [Header("Configuración")]
    public float daño = 10f; 
    public float tiempoDeVida = 5f; 

    [Header("Efectos")]
    public GameObject efectoImpactoPrefab; 

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. IGNORAR ELEMENTOS QUE NO SON OBJETIVOS
        if (collision.CompareTag("Enemigo")) return; 
        if (collision.isTrigger) return;

        // 🔥 ESTA ES LA LÍNEA NUEVA 🔥
        // Ignora el muro invisible para que la flecha lo atraviese sin borrarse
        if (collision.CompareTag("Suelo") || collision.gameObject.layer == LayerMask.NameToLayer("MuroInvisible")) 
        {
            return; 
        }

        // 2. DETECTAR ALIADOS O LA TORRE
        collision.gameObject.SendMessage("RecibirDano", daño, SendMessageOptions.DontRequireReceiver);

        // 3. EFECTO VISUAL
        if (efectoImpactoPrefab != null)
        {
            GameObject efecto = Instantiate(efectoImpactoPrefab, transform.position, Quaternion.identity);
            Destroy(efecto, 0.5f);
        }

        // 4. DESTRUIR LA FLECHA (Solo si no es el muro)
        Destroy(gameObject);
    }
}