using UnityEngine;

public class VueloFantasma : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float alturaDeseada = 2.5f; // A qué altura quieres que flote sobre el suelo
    public float velocidadAjuste = 5f; // Qué tan rápido corrige su altura (más alto = más reactivo)
    public LayerMask capaSuelo;        // Para que el láser solo detecte el suelo (y no a otros enemigos)

    private Rigidbody2D miCuerpo;

    void Start()
    {
        miCuerpo = GetComponent<Rigidbody2D>();
        // IMPORTANTE: Quitamos la gravedad para que este script controle la altura 100%
        miCuerpo.gravityScale = 0; 
    }

    void FixedUpdate()
    {
        FlotarSobreTerreno();
    }

    void FlotarSobreTerreno()
    {
        // 1. Lanzamos un rayo láser hacia abajo desde el centro del fantasma
        RaycastHit2D golpe = Physics2D.Raycast(transform.position, Vector2.down, 10f, capaSuelo);

        // 2. ¿Hemos detectado suelo debajo?
        if (golpe.collider != null)
        {
            // Calculamos la distancia actual al suelo
            float distanciaAlSuelo = golpe.distance;

            // Calculamos la diferencia: ¿Estamos muy bajos o muy altos?
            // Si el suelo sube (escalera), la diferencia será positiva y nos empujará arriba.
            float errorAltura = alturaDeseada - distanciaAlSuelo;

            // 3. Aplicamos velocidad vertical suave para corregir
            // Mantenemos la velocidad X que tenga por su otro script (movement), solo tocamos la Y
            Vector2 nuevaVelocidad = miCuerpo.linearVelocity;
            
            // Lerp hace que el cambio sea suave y no a tirones
            float velocidadY = Mathf.Lerp(nuevaVelocidad.y, errorAltura * velocidadAjuste, Time.fixedDeltaTime * 5f);
            
            nuevaVelocidad.y = velocidadY;
            miCuerpo.linearVelocity = nuevaVelocidad;
        }
    }

    // Dibujamos el láser en el editor para que veas lo que hace
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector2.down * alturaDeseada);
    }
}