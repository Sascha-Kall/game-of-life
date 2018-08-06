/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das die überlegte Generationen- *
 * zahl angezeigt wird                                                          *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationsText : GUIElementFunctions
{
    [SerializeField]
    string m_Text = "Generation: <b>{0}</b>";
    [SerializeField]
    Text m_TextField = null;

	// Use this for initialization
	void Start () 
    {
        if (!m_TextField)
        {
            Debug.LogError("Es wurde keine Textfeld Komponente für das Anzeigen der Generationen in dem Gameobjekt: " + name + " angegeben!");
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
                EnabelElement();
                break;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (GameSettings.Instance.CurrentGameState == GameSettings.GAMESTATE.RUN || GameSettings.Instance.CurrentGameState == GameSettings.GAMESTATE.BREAK)
        {
            string TempString = m_Text.Replace("\\n", "\n");
            m_TextField.text = string.Format(TempString, GameManager.Instance.Generations);
        }
	}
}
