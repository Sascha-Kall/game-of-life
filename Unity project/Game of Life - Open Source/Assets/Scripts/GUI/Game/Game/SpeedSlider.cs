/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das man die Zellenupdate einfach*
 * über eine Schieberegler steuern kann                                         *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : GUIElementFunctions 
{
    Slider m_Slider = null;

	// Use this for initialization
	void Start () 
    {
        m_Slider = GetComponentInChildren<Slider>(true);
        if (!m_Slider)
        {
            Debug.LogError("Das Gameobjekt: " + name + " hat keine Slider Component als Kinnobjekt! Das Script SpeedSlider wird deaktiviert.");
            enabled = false;
        }
	}

    void OnEnable()
    {
        GameSettings.OnChangeGameState += OnChangeGameState;
    }

    void OnDisable()
    {
        GameSettings.OnChangeGameState -= OnChangeGameState;
    }

    void OnChangeGameState(GameSettings.GAMESTATE _newState)
    {
        switch (_newState)
        {
            case GameSettings.GAMESTATE.CREATE:
                DisabelElement();
                break;

            case GameSettings.GAMESTATE.RUN:
                EnabelElement();
                break;

            case GameSettings.GAMESTATE.BREAK:
                DisabelElement();
                break;

            case GameSettings.GAMESTATE.END:
                DisabelElement();
                break;
        }
    }

    public void SetValue()
    {
        GameManager.Instance.UpdateSpeedMultiplicator = m_Slider.value;
    }
}
