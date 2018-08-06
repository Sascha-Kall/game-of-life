/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt den Inhalt des Rahmens Feld          *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSettings : GUIElementFunctions
{
    [SerializeField]
    Button m_NextStepButton = null;
    [SerializeField]
    GameObject m_ObjectPatternsDropDown = null;
    [SerializeField]
    Button m_ClearButton = null;

    void Start()
    {
        if (!m_NextStepButton)
        {
            Debug.LogError("Es wurde keine Nächster Schritt Button Komponente in dem Gameobjekt: " + name + " angegeben! Die Feld Einstellungen können nicht richtig angezeigt werden.");
            enabled = false;
        }

        if (!m_ObjectPatternsDropDown)
        {
            Debug.LogError("Es wurde kein Vorlagen Drop-down Gameobjekt in dem Gameobjekt: " + name + " angegeben. Das DropDown kann nicht richtig angezeigt werden.");
            enabled = false;
        }

        if (!m_ClearButton)
        {
            Debug.LogError("Es wurde keine Lösche Feld Button Komponente in dem Gameobjekt: " + name + " angegeben! Die Feld Einstellungen können nicht richtig angezeigt werden.");
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
                if (GameSettings.Instance.CurrentGameMode == GameSettings.GAMEMODE.GAME)
                {
                    DisabelElement();
                    break;
                }
                else
                    EnabelElement();

                m_ObjectPatternsDropDown.SetActive(true);
                DisabelThisButton(m_NextStepButton);
                EnableThisButton(m_ClearButton);
                break;

            case GameSettings.GAMESTATE.RUN:
                EnabelElement();
                DisabelThisButton(m_NextStepButton);
                m_ObjectPatternsDropDown.SetActive(false);
                EnableThisButton(m_ClearButton);
                break;

            case GameSettings.GAMESTATE.BREAK:
                EnabelElement();
                EnableThisButton(m_NextStepButton);
                m_ObjectPatternsDropDown.SetActive(false);
                EnableThisButton(m_ClearButton);
                break;

            case GameSettings.GAMESTATE.END:
                DisabelElement();
                DisabelThisButton(m_NextStepButton);
                m_ObjectPatternsDropDown.SetActive(false);
                DisabelThisButton(m_ClearButton);
                break;
        }
    }
}
