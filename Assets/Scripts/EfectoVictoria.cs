using UnityEngine;
using System.Collections;

public class EfectoVictoria : MonoBehaviour
{
    void OnEnable()
    {
        // Aparece con un pequeño "golpe" de escala
        transform.localScale = Vector3.zero;
        StartCoroutine(AparecerSuave());
    }

    IEnumerator AparecerSuave()
    {
        float t = 0;
        // Escala final que tienes configurada (4, 2, 1)
        Vector3 escalaFinal = new Vector3(4, 2, 1); 

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 2f; // Se mueve aunque el juego esté pausado
            // Efecto de crecimiento con rebote
            transform.localScale = Vector3.Lerp(Vector3.zero, escalaFinal, t);
            yield return null;
        }
        transform.localScale = escalaFinal;
    }
}