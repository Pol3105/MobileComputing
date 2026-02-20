using UnityEngine;

public static class DatosJugador
{
    // --- CLAVES DE GUARDADO ---
    private const string PREF_DINERO_TOTAL = "DineroTotal"; // (Opcional, por si quieres guardar oro acumulado)
    private const string PREF_PUNTOS_MEJORA = "PuntosMejora"; // <--- NUEVA MONEDA
    
    // --- RESTO DE CLAVES (Niveles, Tutorial...) ---
    private const string PREF_NIVEL_DESBLOQUEADO = "NivelDesbloqueado";
    private const string PREF_TUTORIAL_JUGADO = "TutorialJugado";
    private const string PREF_TIENDA_VISTA = "TiendaVista";

    // --- 💎 SISTEMA DE PUNTOS DE MEJORA (NUEVO) 💎 ---
    
    public static int ObtenerPuntos()
    {
        return PlayerPrefs.GetInt(PREF_PUNTOS_MEJORA, 0); 
    }

    public static void SumarPuntos(int cantidad)
    {
        int actuales = ObtenerPuntos();
        PlayerPrefs.SetInt(PREF_PUNTOS_MEJORA, actuales + cantidad);
        PlayerPrefs.Save();
    }

    // Intenta gastar 1 punto. Devuelve TRUE si se pudo, FALSE si no tenías.
    public static bool GastarPunto()
    {
        int actuales = ObtenerPuntos();
        if (actuales >= 1)
        {
            PlayerPrefs.SetInt(PREF_PUNTOS_MEJORA, actuales - 1);
            PlayerPrefs.Save();
            return true; // Compra realizada
        }
        return false; // No tienes puntos
    }

    // --- SISTEMA DE MEJORAS (Igual que antes) ---
    public static int ObtenerNivelMejora(string tipo) 
    {
        return PlayerPrefs.GetInt("Nivel_" + tipo, 1); 
    }

    public static void SubirNivelMejora(string tipo)
    {
        int nivelActual = ObtenerNivelMejora(tipo);
        PlayerPrefs.SetInt("Nivel_" + tipo, nivelActual + 1);
        PlayerPrefs.Save();
    }

    // --- FUNCIONES EXTRA (Tutorial, Mapa...) ---
    public static int ObtenerNivelDesbloqueado() { return PlayerPrefs.GetInt(PREF_NIVEL_DESBLOQUEADO, 1); }
    public static void DesbloquearNivel(int n) { if (n >= ObtenerNivelDesbloqueado()) { PlayerPrefs.SetInt(PREF_NIVEL_DESBLOQUEADO, n + 1); PlayerPrefs.Save(); } }
    public static bool EsPrimeraVez() { return !PlayerPrefs.HasKey(PREF_TUTORIAL_JUGADO); }
    public static void MarcarTutorialCompletado() { PlayerPrefs.SetInt(PREF_TUTORIAL_JUGADO, 1); PlayerPrefs.Save(); }
    public static bool EsPrimeraVezTienda() { return !PlayerPrefs.HasKey(PREF_TIENDA_VISTA); }
    public static void MarcarTiendaVisitada() { PlayerPrefs.SetInt(PREF_TIENDA_VISTA, 1); PlayerPrefs.Save(); }
    
    public static void BorrarDatos() { PlayerPrefs.DeleteAll(); }

    public static void ReiniciarTienda()
    {
        int puntosADevolver = 0;

        puntosADevolver += (ObtenerNivelMejora("Granjero") - 1);
        puntosADevolver += (ObtenerNivelMejora("ArqueraUnit") - 1);
        puntosADevolver += (ObtenerNivelMejora("Katana") - 1);
        puntosADevolver += (ObtenerNivelMejora("Arco") - 1);
        puntosADevolver += (ObtenerNivelMejora("Vida") - 1);
        puntosADevolver += (ObtenerNivelMejora("Tanque") - 1);
        
        // 🔥 AÑADIMOS EL NINJA A LA CUENTA DE DEVOLUCIÓN 🔥
        puntosADevolver += (ObtenerNivelMejora("Ninja") - 1);

        SumarPuntos(puntosADevolver);

        PlayerPrefs.DeleteKey("Nivel_Granjero");
        PlayerPrefs.DeleteKey("Nivel_ArqueraUnit");
        PlayerPrefs.DeleteKey("Nivel_Katana");
        PlayerPrefs.DeleteKey("Nivel_Arco");
        PlayerPrefs.DeleteKey("Nivel_Vida");
        PlayerPrefs.DeleteKey("Nivel_Tanque");
        
        // 🔥 BORRAMOS LA MEJORA DEL NINJA 🔥
        PlayerPrefs.DeleteKey("Nivel_Ninja");

        PlayerPrefs.Save();
        Debug.Log("🔄 Tienda reseteada. Puntos devueltos: " + puntosADevolver);
    }
}