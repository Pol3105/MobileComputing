using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para controlar los botones

public class MenuInicial : MonoBehaviour
{
    [Header("Botones del Mapa")]
    // Arrastra aquí tus botones en orden: Boton1, Boton2, Boton3, Boton4, BotonBoss
    public Button[] botonesNiveles; 

    private void Start()
    {
        // 1. LÓGICA DE PRIMERA VEZ
        if (DatosJugador.EsPrimeraVez())
        {
            Debug.Log("Usuario nuevo detectado. Lanzando Tutorial...");
            SceneManager.LoadScene("Tutorial");
            return; // Cortamos aquí para que no cargue el menú
        }

        // 2. Si no es nuevo, configura los CANDADOS
        ActualizarCandados();
    }


    // Esta función la llamarán los botones al hacer clic
    public void CargarNivel(int numeroNivel)
    {
        // Si entramos al nivel 1 y era la primera vez, marcamos que ya no es novato
        if (numeroNivel == 1 && DatosJugador.EsPrimeraVez())
        {
            DatosJugador.MarcarTutorialCompletado();
        }

        // Carga la escena "Nivel_1", "Nivel_2", etc.
        SceneManager.LoadScene("Nivel_" + numeroNivel);
    }

    void ActualizarCandados()
    {
        int nivelDesbloqueado = DatosJugador.ObtenerNivelDesbloqueado();
        Debug.Log("Nivel desbloqueado actual: " + nivelDesbloqueado);

        for (int i = 0; i < botonesNiveles.Length; i++)
        {
            // 1. PROTECCIÓN CONTRA HUECOS VACÍOS
            if (botonesNiveles[i] == null)
            {
                Debug.LogWarning("¡Cuidado! El Elemento " + i + " en la lista de botones está vacío (None). Saltando...");
                continue; // Pasa al siguiente botón sin dar error
            }

            int numeroDeNivel = i + 1;
            
            // 2. BUSCAR EL CANDADO (Con seguridad)
            // IMPORTANTE: Busca un objeto hijo llamado EXACTAMENTE "Candado"
            Transform candadoImagen = botonesNiveles[i].transform.Find("Candado");

            if (numeroDeNivel <= nivelDesbloqueado)
            {
                // --- NIVEL DESBLOQUEADO (Se debe poder jugar) ---
                botonesNiveles[i].interactable = true;
                
                if (candadoImagen != null)
                {
                    candadoImagen.gameObject.SetActive(false); // OCULTAR CANDADO
                }
                else
                {
                    // Si entra aquí, es que el botón no tiene hijo "Candado" o se llama distinto
                   // Debug.Log("El botón del nivel " + numeroDeNivel + " no tiene imagen de Candado.");
                }
            }
            else
            {
                // --- NIVEL BLOQUEADO (No se puede jugar) ---
                botonesNiveles[i].interactable = false;
                
                if (candadoImagen != null)
                {
                    candadoImagen.gameObject.SetActive(true); // MOSTRAR CANDADO
                }
            }
        }
    }

    // Añade esto en MenuInicial.cs
    public void IrAlDojo()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TiendaDojo");
    }

    public void IrAlMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }
    

    public void BorrarProgreso()
    {
        DatosJugador.BorrarDatos();
        Debug.Log("Datos borrados. Vuelve a iniciar para ver el tutorial.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga menú
    }
}