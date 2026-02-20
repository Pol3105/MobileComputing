using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorEnemigos : MonoBehaviour
{
    public bool generadorFinalizado = false;
    [Header("Ajuste de Dificultad")]
    public int numeroDeNivel = 1;
    
    [System.Serializable]
    public class GrupoEnemigos
    {
        public string nombre = "Zombis";
        public GameObject enemigoPrefab;
        public int cantidad = 5;
        public float ritmoDeSalida = 2f; 
        public float tiempoInicio = 0f; 
    }

    [System.Serializable]
    public class Oleada
    {
        public string nombre = "Ronda Mixta";
        public List<GrupoEnemigos> gruposDeEnemigos;
    }

    [Header("Configuración")]
    public List<Oleada> listaDeOleadas;
    public float tiempoEntreOleadas = 4f; 
    public Transform puntoSalida;
    public ControladorJuego gameManager;

    void Start()
    {
        if (puntoSalida == null) puntoSalida = transform;
        StartCoroutine(RutinaNivelCompleto());
    }

    IEnumerator RutinaNivelCompleto()
    {
        for (int i = 0; i < listaDeOleadas.Count; i++)
        {
            Oleada oleadaActual = listaDeOleadas[i];
            Debug.Log($"📢 INICIANDO {oleadaActual.nombre}");

            foreach (GrupoEnemigos grupo in oleadaActual.gruposDeEnemigos)
            {
                StartCoroutine(GenerarGrupo(grupo));
            }

            yield return new WaitForSeconds(1f);
            
            // 🔥 AQUÍ ESTÁ LA MAGIA: Usamos nuestra nueva función en lugar del Length == 0
            yield return new WaitUntil(() => !QuedanEnemigosNormales());

            Debug.Log("✅ Oleada completada.");

            if (i < listaDeOleadas.Count - 1)
            {
                yield return new WaitForSeconds(tiempoEntreOleadas);
            }
        }

        generadorFinalizado = true; 
        Debug.Log("🏁 El generador ha terminado todas las oleadas.");
    }

    IEnumerator GenerarGrupo(GrupoEnemigos grupo)
    {
        yield return new WaitForSeconds(grupo.tiempoInicio);

        for (int c = 0; c < grupo.cantidad; c++)
        {
            if (grupo.enemigoPrefab != null)
            {
                GameObject nuevoEnemigo = Instantiate(grupo.enemigoPrefab, puntoSalida.position, Quaternion.identity);
                
                // 1. ESCALADO ZOMBI COMÚN
                ZombiIA scriptZombi = nuevoEnemigo.GetComponent<ZombiIA>();
                if (scriptZombi != null) scriptZombi.EscalarEstadisticas(numeroDeNivel);
                
                // 2. ESCALADO FANTASMA
                EnemigoVolador scriptFantasma = nuevoEnemigo.GetComponent<EnemigoVolador>();
                if (scriptFantasma != null) scriptFantasma.EscalarEstadisticas(numeroDeNivel);

                // 3. ESCALADO ARQUERO ZOMBI
                ZombiArqueroIA scriptArquero = nuevoEnemigo.GetComponent<ZombiArqueroIA>();
                if (scriptArquero != null)
                {
                    scriptArquero.EscalarEstadisticas(numeroDeNivel);
                }
            }
            yield return new WaitForSeconds(grupo.ritmoDeSalida);
        }
    }

    // 🔥 NUEVA FUNCIÓN: Cuenta enemigos pero ignora al Boss 🔥
    bool QuedanEnemigosNormales()
    {
        GameObject[] todosLosEnemigos = GameObject.FindGameObjectsWithTag("Enemigo");
        
        foreach (GameObject enemigo in todosLosEnemigos)
        {
            // Si el enemigo actual NO tiene el script del Boss...
            if (enemigo.GetComponent<BossFinalIA>() == null)
            {
                // ¡Significa que es un enemigo normal y sigue vivo!
                return true; 
            }
        }
        
        // Si termina el bucle y todos los que quedan resultaron ser el Boss (o no queda nadie)...
        return false; 
    }
}