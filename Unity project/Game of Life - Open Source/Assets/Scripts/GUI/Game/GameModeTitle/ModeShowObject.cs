/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das je nach gewählten Spiele    *
 * modus das entsprechende Objekt angezeigt wird                                *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeShowObject : GUIElementFunctions 
{
    [SerializeField]
    GameSettings.GAMEMODE m_ShowOnGameMode = GameSettings.GAMEMODE.NONE;

	// Use this for initialization
	void Awake()
    {
        if (GameSettings.Instance.CurrentGameMode != m_ShowOnGameMode)
        {
            DisabelElement();
            gameObject.SetActive(false);
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
                EnabelElement();
            break;

            default:
                DisabelElement();
            break;
        }
    }
}
