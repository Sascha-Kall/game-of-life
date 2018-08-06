/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das je nachdem in welchen Modus *
 * sich der Create Manager aktuell befindet, sich das entsprechende Symbol      * 
 * einblendet bzw. das andere ausblendet.                                       *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldBorderSideMenu : GUIElementFunctions
{
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    Button m_BrushButton = null; //Der Pinsel Button;
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    Button m_RubberButton = null; //Der Radiergummi Button;
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    CreateManager m_CreateManager = null;

	// Use this for initialization
	void Start ()
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        if (!m_BrushButton)
        {
            Debug.LogError("Es wurde keine Button Komponente für den Pinselbutton in dem Script FieldBorderSideMenu auf dem Gameobjekt: " + name + " gefunden. Das Script wird deaktiviert.");
            enabled = false;
        }

        if (!m_RubberButton)
        {
            Debug.LogError("Es wurde keine Button Komponente für den Pinselbutton in dem Script FieldBorderSideMenu auf dem Gameobjekt: " + name + " gefunden. Das Script wird deaktiviert.");
            enabled = false;
        }

        if (!m_CreateManager)
        {
            Debug.LogError("Es wurde das CreateManager Script in dem Gameobjekt: " + name + " nicht angegeben! Das Script wird deaktiviert.");
            enabled = false;
        }
	}

    void OnEnable()
    {
        //Das Event abonnieren, das aufgerufen wird, wenn sich der aktuelle Zustand des CreateManagers geändert hat
        CreateManager.OnChangeCreateState += OnChangeCreateState;

        //Das Event abonnieren, das aufgerufen wird, wenn sich der Spielzustand ändert
        GameSettings.OnChangeGameState += OnChangeGameState;
    }

    void OnDisable()
    {
        //Das Event deabonnieren, das aufgerufen wird, wenn sich der aktuelle Zustand des CreateManagers geändert hat
        CreateManager.OnChangeCreateState -= OnChangeCreateState;

        //Das Event deabonnieren, das aufgerufen wird, wenn sich der Spielzustand ändert
        GameSettings.OnChangeGameState -= OnChangeGameState;
    }

    void OnChangeCreateState(CreateManager.CREATESTATE _newState)
    {
        switch (_newState)
        {
            case CreateManager.CREATESTATE.NONE:
                SetInteractivButton(m_BrushButton, true);
                SetInteractivButton(m_RubberButton, true);
                break;

            case CreateManager.CREATESTATE.CREATE:
                SetInteractivButton(m_BrushButton, false);
                SetInteractivButton(m_RubberButton, true);
                break;

            case CreateManager.CREATESTATE.ERASE:
                SetInteractivButton(m_RubberButton, false);
                SetInteractivButton(m_BrushButton, true);
                break;
        }
    }

    void OnChangeGameState(GameSettings.GAMESTATE _currentState)
    {
        switch (_currentState)
        {
            case GameSettings.GAMESTATE.CREATE:
                EnabelElement();
            break;

            case GameSettings.GAMESTATE.RUN:
                DisabelElement();
            break;

            case GameSettings.GAMESTATE.BREAK:
                DisabelElement();
            break;

            case GameSettings.GAMESTATE.END:
                DisabelElement();
            break;
        }
    }

    public void ChangeCreateState(int _newState)
    {
        m_CreateManager.CurrentCreateState = (CreateManager.CREATESTATE)_newState;
    }

    public void VisibleSideMenu()
    {
        Image[] TempAllImages = GetComponentsInChildren<Image>();
        if (TempAllImages.Length > 0)
        {
            foreach (Image _img in TempAllImages)
                _img.enabled = true;
        }
    }

    public void InvisibleSideMenu()
    {
        Image[] TempAllImages = GetComponentsInChildren<Image>();
        if (TempAllImages.Length > 0)
        {
            foreach (Image _img in TempAllImages)
                _img.enabled = false;
        }
    }
}
