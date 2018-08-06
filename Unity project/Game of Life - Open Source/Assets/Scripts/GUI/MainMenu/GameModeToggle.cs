/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das der entsprechende Infotext  *
 * für den gewählten Spielmodus angezeigt wird                                  *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeToggle : MonoBehaviour
{
    [SerializeField]
    string m_GameModeInfoText = "";
    [SerializeField]
    string m_CreativeModeInfoText = "";
    [SerializeField]
    Text m_InfoTextField = null;
    [SerializeField]
    Toggle m_DefaultToggle = null;

    // Use this for initialization
    void Start()
    {
        if(!m_InfoTextField)
        {
            Debug.LogError("Es wurde keine Text Komponente für die Game-mode Infos in dem Gameobjekt: " + name + " angegeben! Das Script GameModeToggle wird deaktiviert.");
            enabled = false;
        }

        if (!m_DefaultToggle)
        {
            Debug.LogError("Es wurde keine Toggle Komponente die als Standard ausgewählt ist in dem Gameobjekt: " + name + " angegeben! Das Script GameModeToggle wird deaktiviert.");
            enabled = false;
        }

        m_DefaultToggle.isOn = true;
    }

    void OnEnable()
    {
        GameSettings.OnChangeGameMode += OnChangeGameMode;
    }

    void OnDisable()
    {
        GameSettings.OnChangeGameMode -= OnChangeGameMode;
    }

    public void ChangeGameMode(int _mode)
    {
        _mode = Mathf.Clamp(_mode, 1, 2);

        GameSettings.Instance.ChangeGameMode((GameSettings.GAMEMODE)_mode);
    }

    void OnChangeGameMode(GameSettings.GAMEMODE _newMode)
    {
        switch (_newMode)
        {
            case GameSettings.GAMEMODE.CREATIVE:
                m_InfoTextField.text = m_CreativeModeInfoText;
                break;

            case GameSettings.GAMEMODE.GAME:
                m_InfoTextField.text = m_GameModeInfoText;
                break;
        }
    }
}