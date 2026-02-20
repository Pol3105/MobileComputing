using UnityEngine;
using System.Collections;

public class ControlFondo : MonoBehaviour
{
    [Header("Imágenes del Fondo 🖼️")]
    public Sprite fondoNormal; // El fondo de siempre (puerta cerrada)
    public Sprite fondoAccion; // El fondo con la puerta abierta / luz
    
    [Header("Configuración")]
    public float duracionCambio = 0.5f; // Tiempo que dura el fondo cambiado

    private SpriteRenderer miSprite;
    private Coroutine animacionActual;

    void Start()
    {
        miSprite = GetComponent<SpriteRenderer>();
        
        // Empezamos con el fondo normal
        if (fondoNormal != null) miSprite.sprite = fondoNormal;
    }

    // --- FUNCIÓN PARA LOS BOTONES ---
    public void ActivarFondo()
    {
        // Si ya está activo, reiniciamos el tiempo
        if (animacionActual != null) StopCoroutine(animacionActual);
        
        animacionActual = StartCoroutine(SecuenciaFondo());
    }

    IEnumerator SecuenciaFondo()
    {
        // 1. Ponemos el fondo de ACCIÓN (Puerta abierta)
        if (fondoAccion != null) miSprite.sprite = fondoAccion;

        // 2. Esperamos
        yield return new WaitForSeconds(duracionCambio);

        // 3. Volvemos al fondo NORMAL
        if (fondoNormal != null) miSprite.sprite = fondoNormal;
    }
}