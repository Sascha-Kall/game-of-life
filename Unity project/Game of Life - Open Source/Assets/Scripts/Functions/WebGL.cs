/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt das Verhalten des Spiels wenn es für *
 * WebGL gebuildet wird.                                                        *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGL : MonoBehaviour 
{
    [SerializeField]
    string m_OpenURLByExit = "https://sascha-kall.de/#/portfolio/game-of-life/";

    string m_DefaultURL = "https://sascha-kall.de/#/portfolio/game-of-life/";

    public void OpenURL()
    {
        //Überprüfen ob eine URL angegeben wurde
        if (string.IsNullOrEmpty(m_OpenURLByExit))
        {
            Debug.LogWarning("Es wurde keine URL angegeben die geöffnet werden soll, wenn das Spiel beendet wird. Die Standard URL (" + m_DefaultURL + ") wird nun benutzt.");
            m_OpenURLByExit = m_DefaultURL;
        }

        Application.OpenURL(m_OpenURLByExit);
    }
}
