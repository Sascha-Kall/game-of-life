/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt den Button der dafür sorgt das man   *
 * auch einzelne Schritte weiter gehen kann                                     *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextStepButton : GUIElementFunctions 
{
    public void NextStep()
    {
        GameManager.Instance.CellUpdate();
    }
}
