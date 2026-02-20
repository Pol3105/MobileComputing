using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Usamos el nuevo sistema de Input

public class TiendaMentor : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelDialogo;      // El pergamino
    public TextMeshProUGUI textoDialogo; // El texto dentro del pergamino
    public GameObject panelTiendaUI;     // Todo el Canvas de la tienda (Botones, precios...)

    [Header("Historia")]
    [TextArea(2, 4)] public string[] frasesMaestro; // Escribe aquí lo que dice

    // Variables internas
    private int indice = 0;
    private bool hablando = false;

    void Start()
    {
        // 1. Preguntamos a la memoria: ¿Es la primera vez aquí?
        if (DatosJugador.EsPrimeraVezTienda())
        {
            EmpezarCharla();
        }
        else
        {
            // Si ya somos clientes habituales, ocultamos el diálogo y mostramos la tienda
            if(panelDialogo) panelDialogo.SetActive(false);
            if(panelTiendaUI) panelTiendaUI.SetActive(true);
        }
    }

    void Update()
    {
        if (hablando)
        {
            bool click = false;
            // Detectar clic en PC o Móvil
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) click = true;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) click = true;

            if (click) SiguienteFrase();
        }
    }

    void EmpezarCharla()
    {
        hablando = true;
        
        // Activamos pergamino, desactivamos botones de compra para que no molesten
        if(panelDialogo) panelDialogo.SetActive(true);
        if(panelTiendaUI) panelTiendaUI.SetActive(false); 
        
        indice = 0;
        MostrarFrase();
    }

    void MostrarFrase()
    {
        if (textoDialogo != null && indice < frasesMaestro.Length)
        {
            textoDialogo.text = frasesMaestro[indice];
        }
    }

    void SiguienteFrase()
    {
        indice++;
        if (indice < frasesMaestro.Length)
        {
            MostrarFrase();
        }
        else
        {
            TerminarCharla();
        }
    }

    void TerminarCharla()
    {
        hablando = false;
        
        // Ocultamos pergamino, mostramos la tienda
        if(panelDialogo) panelDialogo.SetActive(false);
        if(panelTiendaUI) panelTiendaUI.SetActive(true); 
        
        // ¡IMPORTANTE! Marcamos que ya hemos estado aquí
        DatosJugador.MarcarTiendaVisitada(); 
    }
}