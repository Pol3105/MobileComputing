using UnityEngine;
using System.Collections;

public class ProyectilFestivo : MonoBehaviour
{
    [Header("Ajustes de Vuelo")]
    public float velocidadSubida = 5f;   
    public float tiempoDeVida = 2.5f;    
    public GameObject explosionPrefab; 

    private Vector3 direccionVuelo;

    // Se llama desde el lanzador al instanciar el cohete
    public void Configurar(float inclinacionX)
    {
        // 1. Calculamos la dirección combinando la subida y la inclinación lateral
        direccionVuelo = new Vector3(inclinacionX, velocidadSubida, 0).normalized;

        // 2. CALCULAMOS LA ROTACIÓN: Mirar hacia la dirección de lanzamiento
        // Prueba cambiando el valor final (-90, +90 o 0) hasta que la punta mire al frente
        float angulo = Mathf.Atan2(direccionVuelo.y, direccionVuelo.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo - 30);
    }

    void Start()
    {
        StartCoroutine(SecuenciaExplosion());
    }

    void Update()
    {
        // Movimiento recto constante ignorando la pausa del juego
        transform.Translate(direccionVuelo * velocidadSubida * Time.unscaledDeltaTime, Space.World);
    }

    IEnumerator SecuenciaExplosion()
    {
        yield return new WaitForSecondsRealtime(tiempoDeVida);

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}