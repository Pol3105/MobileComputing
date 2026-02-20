using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class GeneradorAliados : MonoBehaviour
{
    [Header("Referencias Generales")]
    public Transform puntoSalida;     
    private ControladorJuego gameManager;

    [Header("Configuración Tiempos ⏳")]
    public float tiempoEsperaPuerta = 0.5f; 

    [Header("--- EL GRANJERO ---")]
    public GameObject granjeroPrefab; 
    public int costeGranjero = 10;    

    [Header("--- LA ARQUERA ---")]
    public GameObject arqueroPrefab;  
    public int costeArquero = 15;     

    [Header("--- EL TANQUE (RONIN) ---")]
    public GameObject tanquePrefab;  
    public int costeTanque = 40; 

    // 🔥 NUEVA SECCIÓN: EL NINJA ARTIFICIERO 🔥
    [Header("--- EL NINJA (NUEVO) ---")]
    public GameObject ninjaPrefab;  
    public int costeNinja = 60; // El Ninja es una unidad de élite, sugerimos 60 monedas

    void Start()
    {
        gameManager = FindAnyObjectByType<ControladorJuego>();
    }

    // --- BOTONES DE INVOCACIÓN ---

    public void InvocarGranjero() => IntentarInvocacion(granjeroPrefab, costeGranjero, "👨‍🌾 Granjero");
    public void InvocarArquero() => IntentarInvocacion(arqueroPrefab, costeArquero, "🏹 Arquera");
    public void InvocarTanque() => IntentarInvocacion(tanquePrefab, costeTanque, "🛡️ Ronin");
    
    // 🔥 NUEVO MÉTODO PARA EL NINJA 🔥
    public void InvocarNinja() => IntentarInvocacion(ninjaPrefab, costeNinja, "💣 Ninja");

    // Método genérico para no repetir código en cada botón
    void IntentarInvocacion(GameObject prefab, int coste, string nombre)
    {
        if (gameManager == null) return;

        if (gameManager.GastarMonedas(coste))
        {
            StartCoroutine(GenerarConRetraso(prefab));
            Debug.Log($"{nombre} pagado. ¡Refuerzos en camino!");
        }
        else
        {
            Debug.Log($"🚫 No tienes suficiente dinero para: {nombre}");
        }
    }

    // --- LA MAGIA DEL RETRASO Y EL ESCALADO ---
    IEnumerator GenerarConRetraso(GameObject aliadoPrefab)
    {
        yield return new WaitForSeconds(tiempoEsperaPuerta);

        if (aliadoPrefab != null && puntoSalida != null)
        {
            GameObject nuevoAliado = Instantiate(aliadoPrefab, puntoSalida.position, Quaternion.identity);
            
            // ⚔️ APLICAR MEJORAS SEGÚN EL NIVEL ACTUAL ⚔️
            // Buscamos el nivel del generador para saber cuánto bufar a la tropa
            GeneradorEnemigos genEnemigos = FindAnyObjectByType<GeneradorEnemigos>();
            int nivelActual = (genEnemigos != null) ? genEnemigos.numeroDeNivel : 1;

            // Llamamos a las funciones de mejora que creamos en los scripts de cada tropa
            nuevoAliado.SendMessage("AplicarMejoras", SendMessageOptions.DontRequireReceiver);
            
            // Si es el Ninja, podrías tener una función específica o usar la genérica
            nuevoAliado.SendMessage("AplicarMejorasNinja", SendMessageOptions.DontRequireReceiver);
        }
    }
}