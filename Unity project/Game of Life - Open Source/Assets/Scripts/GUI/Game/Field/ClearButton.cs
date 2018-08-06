/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt den Button zum löschen des Spielfelds*
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButton : GUIElementFunctions 
{
    public void Clear()
    {
        GameManager.Instance.ClearGame();
    }
}
