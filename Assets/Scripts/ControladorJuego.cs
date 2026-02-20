using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections; 

public class ControladorJuego : MonoBehaviour
{
    [Header("Pantallas UI")]
    public GameObject panelGameOver; 
    public GameObject panelVictoria;
    public GameObject panelPausa;    
    public GameObject interfazJuego; 

    [Header("Economía 💰")]
    public int monedasActuales = 0;
    public TextMeshProUGUI textoMonedas;

    [Header("Audio 🎵")]
    public AudioSource musicaFondo;

    [Header("Configuración")]
    public float segundosDeDramatismo = 2.5f;

    // Estados internos
    private bool juegoTerminado = false;
    private bool juegoPausado = false;

    // ---------------------------------------------------------
    // ¡HEMOS BORRADO LAS VARIABLES DE CÁMARA QUE DABAN ERROR! 🗑️
    // (puntoVistaFinal, camaraPrincipal, etc. ya no hacen falta aquí)
    // ---------------------------------------------------------

    void Start()
    {
        ActualizarTextoMonedas();
        
        // Aseguramos que los paneles estén apagados al empezar
        if (panelGameOver) panelGameOver.SetActive(false);
        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelPausa) panelPausa.SetActive(false);
        
        if (interfazJuego) interfazJuego.SetActive(false);
    }

    void Update()
    {
        // Si el juego ya acabó, no dejamos pausar
        if (!juegoTerminado && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePausa();
        }
    }

    // --- PAUSA ---
    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;

        if (juegoPausado)
        {
            if (panelPausa) panelPausa.SetActive(true);
            if (interfazJuego) interfazJuego.SetActive(false);
            Time.timeScale = 0f; 
            if (musicaFondo) musicaFondo.Pause(); 
        }
        else
        {
            if (panelPausa) panelPausa.SetActive(false);
            if (interfazJuego) interfazJuego.SetActive(true);
            Time.timeScale = 1f; 
            if (musicaFondo) musicaFondo.UnPause();
        }
    }

    // --- VICTORIA / DERROTA ---

    public void GanarJuego()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log("🏆 ¡VICTORIA! Llamando a la cámara...");

        // Paramos música si quieres
        if (musicaFondo != null) musicaFondo.Stop();

        // DELEGAMOS EN LA CÁMARA
        CamaraSeguimiento camara = FindAnyObjectByType<CamaraSeguimiento>();
        if (camara != null)
        {
            camara.ActivarVictoria();
        }
        else
        {
            // Plan B por si falla
            if (panelVictoria != null) panelVictoria.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void GanarNivel(int nivelQueHeGanado)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log("🏆 ¡VICTORIA! Nivel " + nivelQueHeGanado + " completado.");

        // 💎 LÓGICA DE RECOMPENSA ÚNICA 💎
        // Solo damos el punto si el nivel ganado coincide con el nivel que el jugador tiene por desbloquear.
        // Ejemplo: Si el jugador debe pasarse el 1 para ir al 2, y gana el 1 -> Recibe premio.
        // Si ya tiene el 2 abierto y repite el 1 -> NO recibe premio.
        if (nivelQueHeGanado == DatosJugador.ObtenerNivelDesbloqueado())
        {
            DatosJugador.SumarPuntos(3);
            Debug.Log("💎 ¡Primera victoria! Recompensa guardada: +1 Punto de Mejora");
        }
        else
        {
            Debug.Log("🏁 Nivel repetido: No hay puntos de mejora esta vez.");
        }

        // Desbloqueamos el siguiente nivel (la función interna ya evita errores si ya estaba abierto)
        DatosJugador.DesbloquearNivel(nivelQueHeGanado);

        // 🎬 Cámara y Panel (Igual que antes)
        if (musicaFondo != null) musicaFondo.Stop();
        CamaraSeguimiento camara = FindFirstObjectByType<CamaraSeguimiento>();
        if (camara != null)
        {
            camara.ActivarVictoria();
        }
        else
        {
            if (panelVictoria != null) panelVictoria.SetActive(true);
            Time.timeScale = 0f;
        }
    }

   public void FinJuego()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log("🏆 ¡SAMURAI DEFENSE COMPLETADO! El Rey Helado ha caído.");

        // Desbloqueamos el nivel 5 por si acaso (aunque ya estemos en él)
        DatosJugador.DesbloquearNivel(5);

        if (musicaFondo != null) musicaFondo.Stop();

        // Efecto de cámara lenta para el golpe final
        Time.timeScale = 0.5f; 

        // Iniciamos la transición a la escena de despedida
        StartCoroutine(TransicionACreditos());
    }
    
    System.Collections.IEnumerator TransicionACreditos()
    {
        // Esperamos 2 segundos a cámara lenta para que el jugador vea caer al Boss
        yield return new WaitForSecondsRealtime(2.5f); 
        
        // Devolvemos el tiempo a la normalidad
        Time.timeScale = 1f;
        
        // Cargamos la escena de créditos (Asegúrate de llamarla exactamente así en Unity)
        SceneManager.LoadScene("Creditos");
    }

    // Esta función la llaman la Torre o el Jugador al morir
    public void MostrarGameOver()
    {
        if (juegoTerminado) return; 
        juegoTerminado = true;

        Debug.Log("🎬 Game Over: Llamando a la cámara...");

        // 1. Paramos la música si quieres
        if (musicaFondo != null) musicaFondo.Stop();

        // 2. BUSCAMOS A LA CÁMARA Y LE DECIMOS QUE ACTIVE EL FINAL
        CamaraSeguimiento camara = FindAnyObjectByType<CamaraSeguimiento>();
        
        if (camara != null)
        {
            camara.ActivarGameOver(); // <--- ¡AQUÍ ESTÁ LA CLAVE! 🗝️
        }
        else
        {
            // Si por lo que sea no encuentra la cámara, mostramos el panel de golpe (plan B)
            if (panelGameOver) panelGameOver.SetActive(true);
        }
    }

    // (Aquí abajo estaba la corrutina vieja "SecuenciaCamaraFinal", LA HE BORRADO ENTERA 🗑️)

    // --- NAVEGACIÓN ---

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

        public void TerminarTutorial()
    {
        DatosJugador.MarcarTutorialCompletado(); // Descomenta si tienes este script
        DatosJugador.DesbloquearNivel(0); 

        if (DatosJugador.ObtenerPuntos() == 0) 
        {
            Debug.Log("💎 Tutorial Completado: ¡Has ganado 3 Puntos de Mejora!");
            DatosJugador.SumarPuntos(3);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    // --- ECONOMÍA ---

    public void GanarMonedas(int cantidad)
    {
        monedasActuales += cantidad;
        ActualizarTextoMonedas();
    }

    public bool GastarMonedas(int cantidad)
    {
        if (monedasActuales >= cantidad)
        {
            monedasActuales -= cantidad;
            ActualizarTextoMonedas();
            return true;
        }
        return false;
    }

    public void ActualizarTextoMonedas()
    {
        if (textoMonedas != null)
        {
            textoMonedas.text = monedasActuales.ToString();
        }
    }

    public void CargarSiguienteNivel()
    {
        Time.timeScale = 1f; 
        
        int indiceUnity = SceneManager.GetActiveScene().buildIndex;
        // DatosJugador.DesbloquearNivel(indiceUnity); // Descomenta si usas guardado

        SceneManager.LoadScene("MenuPrincipal");
    }
}