/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt den Inhalt des Rahmens Spiel         *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : GUIElementFunctions 
{
    [SerializeField]
    string m_ButonTextCreateMode = "Spiel Starten";
    [SerializeField]
    string m_ButtonTextPlayMode = "Pausieren";
    [SerializeField]
    string m_ButtonTextBreakMode = "Weiter abspielen";   
    
    Button m_Button = null;
    Text m_ButtonText = null;

	// Use this for initialization
	void Awake () 
    {
        m_Button = GetComponentInChildren<Button>();
        if (!m_Button)
        {
            Debug.LogError("Das Gameobjekt: " + name + " hat kein Button Component. Das PlayButton Script wird ausgeschaltet.");
            enabled = false;
        }

        m_ButtonText = m_Button.GetComponentInChildren<Text>();
        if (!m_ButtonText)
        {
            Debug.LogError("Das Button Gameobjekt: " + name + " hat kein Text Component als Kindobjekt! Das PlayButton Script wird ausgeschaltet.");
            enabled = false;
        }
	}

    void OnEnable()
    {
        GameSettings.OnChangeGameState += OnChangeGameState;
        CreateManager.OnChangeCellAsAlive += OnChangeCellAsAliveCreateManager;
        GameManager.OnClearGame += OnClearGame;
        GridManager.OnChangeCellAsAlive += OnChangeCallAsAliveGridManager;
    }

    void OnDisable()
    {
        GameSettings.OnChangeGameState -= OnChangeGameState;
        CreateManager.OnChangeCellAsAlive -= OnChangeCellAsAliveCreateManager;
        GameManager.OnClearGame -= OnClearGame;
        GridManager.OnChangeCellAsAlive -= OnChangeCallAsAliveGridManager;
    }

    void OnChangeGameState(GameSettings.GAMESTATE _newState)
    {
        switch (_newState)
        {
            case GameSettings.GAMESTATE.CREATE:
                m_Button.interactable = false;
                EnabelElement();
                m_ButtonText.text = m_ButonTextCreateMode;
                m_Button.onClick.RemoveAllListeners();

                if (GameSettings.Instance.CurrentGameMode == GameSettings.GAMEMODE.GAME)
                    m_Button.onClick.AddListener(GameManager.Instance.TakeFieldScreenshot);
                
                m_Button.onClick.AddListener(GameManager.Instance.StartGame);
                break;

            case GameSettings.GAMESTATE.RUN:
                EnabelElement();
                m_ButtonText.text = m_ButtonTextPlayMode;
                m_Button.onClick.RemoveAllListeners();
                m_Button.onClick.AddListener(GameManager.Instance.BreakGame);
                break;

            case GameSettings.GAMESTATE.BREAK:
                EnabelElement();
                m_ButtonText.text = m_ButtonTextBreakMode;
                m_Button.onClick.RemoveAllListeners();
                m_Button.onClick.AddListener(GameManager.Instance.StartGame);
                break;

            case GameSettings.GAMESTATE.END:
                DisabelElement();
                break;
        }
    }

    void OnChangeCellAsAliveCreateManager(int _count)
    {
        if (_count > 0)
            m_Button.interactable = true;
    }

    void OnChangeCallAsAliveGridManager(int _count)
    {
        if (_count == 0 && GameSettings.Instance.CurrentGameState != GameSettings.GAMESTATE.RUN)
            m_Button.interactable = false;
        else
            m_Button.interactable = true;
    }

    void OnClearGame()
    {
        m_Button.interactable = false;
    }
}
