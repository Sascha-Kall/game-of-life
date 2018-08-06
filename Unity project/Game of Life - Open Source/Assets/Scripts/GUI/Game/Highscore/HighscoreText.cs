/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das der Highscore in dem Rahmen *
 * Highscore angezeigt wird                                                     *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreText : GUIElementFunctions
{
    [SerializeField]
    Text m_TextField = null;
    [SerializeField]
    Button m_FieldButton = null;

	// Use this for initialization
	void Start ()
    {
        if (!m_TextField)
        {
            Debug.LogError("Es wurde keine Textfeld Komponente angegeben in der Highscore angezeigt werden kann. Das Highscore script wird beendet.");
            enabled = false;
        }

        if (!m_FieldButton)
        {
            Debug.LogError("Es wurde keine Button Komponente angegeben die den Feld Screenshot anzeigen lässt! Das Highscore script wird beendet.");
            enabled = false;
        }

        if(GameSettings.Instance.CurrentGameMode == GameSettings.GAMEMODE.CREATIVE)
        {
            DisabelElement();
            enabled = false;
        }
        else
        {
            uint TempHighscore = GameManager.Instance.Highscore.Highscore;

            if (TempHighscore == 0)
            {
                DisabelElement();
                return; 
            }
            
            ShowText(GameManager.Instance.Highscore.Date, TempHighscore);
        }
	}

    void OnEnable()
    {
        GameManager.OnNewHighscore += OnNewHighscore;
        GameSettings.OnChangeGameState += OnChangeGameState;
    }

    void OnDisable()
    {
        GameManager.OnNewHighscore -= OnNewHighscore;
        GameSettings.OnChangeGameState -= OnChangeGameState;
    }

    void OnChangeGameState(GameSettings.GAMESTATE _newState)
    {
        switch (_newState)
        {
            case GameSettings.GAMESTATE.CREATE:
                if (GameManager.Instance.Highscore.Highscore != 0)
                {
                    EnableThisButton(m_FieldButton);
                    EnabelElement();
                }
                else
                    DisabelElement();
                break;

            case GameSettings.GAMESTATE.RUN:
                if (GameManager.Instance.Highscore.Highscore != 0)
                {
                    DisabelThisButton(m_FieldButton);
                    EnabelElement();
                }
                else
                    DisabelElement();
                break;

            case GameSettings.GAMESTATE.BREAK:
                if (GameManager.Instance.Highscore.Highscore != 0)
                {
                    DisabelThisButton(m_FieldButton);
                    EnabelElement();
                }
                else
                    DisabelElement();
                break;
        }
    }

    void OnNewHighscore(GameManager.HighscoreSettings _highscore)
    {
        if(IsDisableElement())
            EnabelElement();

        ShowText(_highscore.Date, _highscore.Highscore);
    }

    void ShowText(string _date, uint _highscore)
    {
        string TempHighscoreText = string.Format("{0} {1} <b>{2}</b> Generationen", _date, System.Environment.NewLine, _highscore);

        m_TextField.text = TempHighscoreText.Replace("\\n", "\n");
    }

    public void ShowFieldWindow()
    {
        Texture2D TempScreenshot = GameManager.Instance.GetSaveManager.LoadScreenshot();
        if (TempScreenshot == null)
        {
            Debug.LogError("Beim Laden des Feldscreenshots ist ein Fehler aufgetreten. Das Fenster mit dem Feld Highscore konnte nicht angezeigt werden.");
            return;
        }

        HighscoreField HighscoreFieldScript = (HighscoreField)FindObjectOfType(typeof(HighscoreField));
        if (!HighscoreFieldScript)
        {
            Debug.LogError("In der aktuellen Scene konnte kein Script mit dem Namen: HighscoreField gefunden werden! Das Fenster mit dem Feld Highscore konnte nicht angezeigt werden.");
            return;
        }

        HighscoreFieldScript.EnableWindow(TempScreenshot);
    }
}
