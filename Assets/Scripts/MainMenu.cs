using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Carga la escena de Gameplay
    /// </summary>
    public void GoPlay()
    {
        SceneManager.LoadScene("Gameplay");
    }
    /// <summary>
    /// Utiliza el metodo estatico ExitApp() para cerrar la aplicacion.
    /// </summary>
    public void GoExit()
    {
        ExitApp();
    }
    /// <summary>
    /// Metodo estatico, cierra la aplicacion utilizando Application.Quit() de UnityEngine;
    /// </summary>
    public static void ExitApp()
    {
        Application.Quit();
    }
}
