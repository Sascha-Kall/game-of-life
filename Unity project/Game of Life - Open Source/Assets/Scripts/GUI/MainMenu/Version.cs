/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das die Spielversion im Haupt-  *
 * Menü angezeigt wird                                                          *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour 
{
    enum RELEASESTATE { PROTOTYPE, ALPHA, BETA, GOLD };

    [SerializeField]
    string m_Text = "Version";
    [SerializeField]
    RELEASESTATE m_CurrentRelase = RELEASESTATE.BETA;

    Text m_TextField = null;

	// Use this for initialization
	void Awake () 
    {
        m_TextField = GetComponentInChildren<Text>();
        if (!m_TextField)
        {
            Debug.LogError("Es wurde keine Text Komponente in den Kind Gameobjektes des Gameobjektes: " + name + " gefunden! Die Spiel Version konnte nicht angezeigt werden.");
            enabled = false;
        }

        SetVersion();
	}

    void SetVersion()
    {
        string TempText = string.Format("{0}: {1}", m_Text, Application.version);

        switch (m_CurrentRelase)
        {
            case RELEASESTATE.PROTOTYPE:
                TempText += " PROTOTYPE";
                break;

            case RELEASESTATE.ALPHA:
                TempText += " ALPHA";
                break;

            case RELEASESTATE.BETA:
                TempText += " BETA";
                break;
        }

        m_TextField.text = TempText;
    }
}
