using UnityEngine;

public class AutoDestruccion : MonoBehaviour
{
    public float tiempoVida = 0.5f; // Ajusta esto a lo que dure tu animación
    void Start()
    {
        // Se borra solo después de medio segundo
        Destroy(gameObject, tiempoVida); 
    }
}