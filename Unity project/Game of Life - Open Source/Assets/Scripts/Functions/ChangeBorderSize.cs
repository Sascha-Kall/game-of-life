/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür die Rahmen der Spiele GUI       *
 * je nach Spielmode bzw. Spielzustand sich mit ihrer Größe an den Inhalt in    *
 * dem Rahmen anpassen.                                                         *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBorderSize : MonoBehaviour
{
    [System.Serializable]
    struct BorderSettings
    {
        public GameSettings.GAMEMODE ChangeInMode;
        public GameSettings.GAMESTATE ChangeInState;
        public Vector2 BorderSize;
    }

    [SerializeField]
    List<BorderSettings> m_BorderSettings = new List<BorderSettings>();

    RectTransform m_Transform = null;

	// Use this for initialization
	void Awake ()
    {
        if (m_BorderSettings.Count == 0)
        {
            Debug.LogWarning("Es wurden keine Einstellungen für den Rahmen: " + name + " angegeben! Das Script wird beendet.");
            enabled = false;
        }

        m_Transform = GetComponent<RectTransform>();
        if (!m_Transform)
        {
            Debug.LogError("Auf dem Gameobjekt: " + name + " wurde keine RectTransform Komponente gefunden! Der Rahmen kann von der Größe nicht angepasst werden.");
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
        foreach (BorderSettings _settings in m_BorderSettings)
        {
            if (_settings.ChangeInState == _newState && GameSettings.Instance.CurrentGameMode == _settings.ChangeInMode)
            {
                ChangeBorder(_settings.BorderSize);
                break;
            }
        }
    }

    void ChangeBorder(Vector2 _toSize)
    {
        m_Transform.sizeDelta = _toSize;
    }
}
