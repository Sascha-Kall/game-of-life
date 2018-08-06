/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das die Legende für den Farbcode*
 * für das Zellenalter angezeigt wird                                           *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : GUIElementFunctions 
{
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
}
