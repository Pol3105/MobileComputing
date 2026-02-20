using UnityEngine;
using UnityEngine.UI; // Necesario para tocar la UI

public class BarraDeVida : MonoBehaviour
{
    public Slider slider; // Arrastraremos aquí el Slider
    public Vector3 offset; // Para ajustar la altura si hace falta

    public void ActualizarBarra(float vidaActual, float vidaMaxima)
    {
        // Convertimos la vida en un porcentaje entre 0 y 1
        slider.value = vidaActual / vidaMaxima;
    }

    void Update()
    {
        // OPCIONAL: Esto hace que la barra siempre mire a la cámara 
        // (Útil si rotas los personajes, en 2D plano no suele hacer falta pero queda bien)
        transform.rotation = Quaternion.identity;
    }
}