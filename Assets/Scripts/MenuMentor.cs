using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem; // ✅ IMPORTANTE: Añadimos esta librería

public class MenuMentor : MonoBehaviour
{
    [Header("UI del Diálogo")]
    public GameObject panelDialogo;      
    public TextMeshProUGUI textoDialogo; 
    
    [Header("Botón a Desbloquear")]
    public GameObject botonMejoras;      

    [Header("Contenido")]
    [TextArea(2, 4)] public string[] frasesSamurai; 

    private const string PREF_CHARLA_MENU_VISTA = "CharlaMenuVista";

    private int indiceFrase = 0;
    private bool dialogoActivo = false;

    void Start()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);

        // Usamos DatosJugador para saber si ya pasó el tutorial
        bool tutorialCompletado = !DatosJugador.EsPrimeraVez(); 
        bool charlaVista = PlayerPrefs.GetInt(PREF_CHARLA_MENU_VISTA, 0) == 1;

        if (tutorialCompletado && !charlaVista)
        {
            EmpezarCharla();
        }
        else
        {
            if(botonMejoras != null) botonMejoras.SetActive(tutorialCompletado);
        }
    }

    void Update()
    {
        // ✅ CORRECCIÓN: Detección de Input compatible con el Nuevo Sistema
        if (dialogoActivo)
        {
            bool haHechoClic = false;

            // 1. Ratón (PC)
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) haHechoClic = true;
            
            // 2. Pantalla Táctil (Móvil)
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) haHechoClic = true;
            
            // 3. Teclado (Opcional)
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) haHechoClic = true;

            if (haHechoClic)
            {
                SiguienteFrase();
            }
        }
    }

    void EmpezarCharla()
    {
        dialogoActivo = true;
        panelDialogo.SetActive(true);
        if(botonMejoras != null) botonMejoras.SetActive(false);
        indiceFrase = 0;
        ActualizarTexto();
    }

    void SiguienteFrase()
    {
        indiceFrase++;
        if (indiceFrase < frasesSamurai.Length)
        {
            ActualizarTexto();
        }
        else
        {
            TerminarCharla();
        }
    }

    void ActualizarTexto()
    {
        if (textoDialogo != null && indiceFrase < frasesSamurai.Length)
            textoDialogo.text = frasesSamurai[indiceFrase];
    }

    void TerminarCharla()
    {
        dialogoActivo = false;
        panelDialogo.SetActive(false);

        if (botonMejoras != null) botonMejoras.SetActive(true);

        PlayerPrefs.SetInt(PREF_CHARLA_MENU_VISTA, 1);
        PlayerPrefs.Save();
        
        Debug.Log("Charla del menú completada y guardada.");
    }
    
    public void ResetearCharlaMenu()
    {
        PlayerPrefs.DeleteKey(PREF_CHARLA_MENU_VISTA);
        Debug.Log("Memoria de charla borrada.");
    }
}