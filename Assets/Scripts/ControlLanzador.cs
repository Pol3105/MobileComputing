using UnityEngine;
using System.Collections;

public class ControlLanzador : MonoBehaviour
{
    public GameObject prefabProyectil; 
    public float tiempoEntreCohetes = 0.8f; 
    
    [Header("Rango de Ángulo (Inclinación)")]
    public float minInclinacion = 2f; 
    public float maxInclinacion = 6f;

    void OnEnable()
    {
        StartCoroutine(SecuenciaInfinita());
    }

    IEnumerator SecuenciaInfinita()
    {
        while (true) 
        {
            LanzarUno();
            yield return new WaitForSecondsRealtime(tiempoEntreCohetes);
        }
    }

    void LanzarUno()
    {
        if (prefabProyectil == null) return;

        GameObject nuevoCohete = Instantiate(prefabProyectil, transform.position, Quaternion.identity);
        
        ProyectilFestivo script = nuevoCohete.GetComponent<ProyectilFestivo>();
        if(script != null)
        {
            // Le damos una inclinación distinta a cada uno entre tus valores
            script.Configurar(Random.Range(minInclinacion, maxInclinacion));
        }
    }
}