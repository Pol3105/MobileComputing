using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TiendaDojo : MonoBehaviour
{
    [Header("UI Puntos")]
    public TextMeshProUGUI textoPuntosDisponibles; 

    [Header("--- GRÁFICOS EVOLUTIVOS ---")]
    public Sprite[] iconosGranjero;
    public Sprite[] iconosArquera;
    public Sprite[] iconosKatana;
    public Sprite[] iconosArco;
    public Sprite[] iconosVida;
    public Sprite[] iconosRonin; 
    public Sprite[] iconosNinja;

    [Header("--- BOTONES DE COMPRA ---")]
    public Button btnMejorarGranjero;
    public Button btnMejorarArquera;
    public Button btnMejorarKatana;
    public Button btnMejorarArco;
    public Button btnMejorarVida;

    [Header("--- SECCIÓN RONIN ---")]
    public GameObject contenedorRonin; 
    public Button btnMejorarRonin; 

    [Header("--- SECCIÓN NINJA (NUEVO) ---")]
    public GameObject contenedorNinja; 
    public Button btnMejorarNinja; 

    void Start()
    {
        ActualizarTodaLaUI();
    }

    public void ActualizarTodaLaUI()
    {
        // 1. Mostrar Puntos
        if(textoPuntosDisponibles) 
            textoPuntosDisponibles.text = DatosJugador.ObtenerPuntos().ToString();

        // 2. Actualizar botones básicos
        ActualizarBotonVisual("Granjero", btnMejorarGranjero, iconosGranjero);
        ActualizarBotonVisual("ArqueraUnit", btnMejorarArquera, iconosArquera);
        ActualizarBotonVisual("Katana", btnMejorarKatana, iconosKatana);
        ActualizarBotonVisual("Arco", btnMejorarArco, iconosArco);
        ActualizarBotonVisual("Vida", btnMejorarVida, iconosVida);

        // 3. LÓGICA DE APARICIÓN DEL GRUPO RONIN
        // Si has desbloqueado el Nivel 3 (significa que ganaste el 2)
        if (DatosJugador.ObtenerNivelDesbloqueado() >= 3)
        {
            if (contenedorRonin != null) contenedorRonin.SetActive(true);
            if (btnMejorarRonin != null) ActualizarBotonVisual("Tanque", btnMejorarRonin, iconosRonin);
        }
        else
        {
            if (contenedorRonin != null) contenedorRonin.SetActive(false);
        }

        // 4. 🔥 LÓGICA DE APARICIÓN DEL GRUPO NINJA (CORREGIDA) 🔥
        // Si has desbloqueado el Nivel 5 (significa que ganaste el 4)
        if (DatosJugador.ObtenerNivelDesbloqueado() >= 5)
        {
            // --- YA LO TIENES ---
            if (contenedorNinja != null) 
            {
                contenedorNinja.SetActive(true); 
            }

            if (btnMejorarNinja != null)
            {
                ActualizarBotonVisual("Ninja", btnMejorarNinja, iconosNinja);
            }
        }
        else
        {
            // --- AÚN NO LO TIENES ---
            if (contenedorNinja != null)
            {
                contenedorNinja.SetActive(false); 
            }
        }
    }

    void ActualizarBotonVisual(string id, Button btn, Sprite[] misIconos)
    {
        if (btn == null) return;
        int nivelActual = DatosJugador.ObtenerNivelMejora(id);
        int indiceImagen = nivelActual - 1;
        
        if (misIconos != null && misIconos.Length > 0)
        {
            int indiceSeguro = Mathf.Clamp(indiceImagen, 0, misIconos.Length - 1);
            btn.image.sprite = misIconos[indiceSeguro];
        }

        bool esMaximo = (misIconos != null && indiceImagen >= misIconos.Length - 1);
        btn.interactable = !esMaximo && (DatosJugador.ObtenerPuntos() > 0);
    }

    // --- FUNCIONES DE CLIC DE LOS BOTONES ---
    public void ClickGranjero() { Comprar("Granjero", iconosGranjero); }
    public void ClickArquera()  { Comprar("ArqueraUnit", iconosArquera); }
    public void ClickKatana()   { Comprar("Katana", iconosKatana); }
    public void ClickArco()     { Comprar("Arco", iconosArco); }
    public void ClickVida()     { Comprar("Vida", iconosVida); }
    public void ClickRonin()    { Comprar("Tanque", iconosRonin); } 
    public void ClickNinja()    { Comprar("Ninja", iconosNinja); } 

    void Comprar(string id, Sprite[] iconos)
    {
        int nivel = DatosJugador.ObtenerNivelMejora(id);
        if (iconos != null && nivel - 1 >= iconos.Length - 1) return; 
        
        if (DatosJugador.GastarPunto())
        {
            DatosJugador.SubirNivelMejora(id);
            ActualizarTodaLaUI();
        }
    }
    
    public void VolverMenu() { SceneManager.LoadScene("MenuPrincipal"); }
    public void ReiniciarProgreso() { DatosJugador.ReiniciarTienda(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void TrucoDamePuntos() { DatosJugador.SumarPuntos(5); ActualizarTodaLaUI(); }
}