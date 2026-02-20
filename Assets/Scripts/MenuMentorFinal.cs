using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 🔥 NECESARIO PARA EL NUEVO SISTEMA

public class MenuMentorFinal : MonoBehaviour
{
    [Header("UI del Diálogo")]
    public TextMeshProUGUI textoDialogo;
    public GameObject panelPergamino;

    [Header("Botones Finales")]
    public GameObject botonReinicio; 

    [Header("Contenido")]
    [TextArea(3, 5)]
    public string[] frasesVictoria; 

    private int indiceFrase = 0;
    private bool dialogoTerminado = false;

    void Start()
    {
        if (botonReinicio != null) botonReinicio.SetActive(false);
        MostrarSiguienteFrase();
    }

    void Update()
    {
        // 🔥 CAMBIO AQUÍ: Detectamos el clic/toque con el Nuevo Sistema 🔥
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame && !dialogoTerminado)
        {
            MostrarSiguienteFrase();
        }
    }

    void MostrarSiguienteFrase()
    {
        if (indiceFrase < frasesVictoria.Length)
        {
            textoDialogo.text = frasesVictoria[indiceFrase];
            indiceFrase++;
        }
        else
        {
            dialogoTerminado = true;

            if (panelPergamino != null) panelPergamino.SetActive(false);
            if (botonReinicio != null) botonReinicio.SetActive(true);
        }
    }

    public void ReiniciarJuegoCompleto()
    {
        PlayerPrefs.DeleteAll();
        DatosJugador.ReiniciarTienda(); 
        SceneManager.LoadScene("MenuPrincipal");
    }
}