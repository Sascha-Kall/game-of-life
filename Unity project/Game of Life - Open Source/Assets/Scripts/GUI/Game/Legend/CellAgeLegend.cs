/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das die Legende für den Farbcode*
 * des Zellenalters richtig angezeigt wird                                      *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAgeLegend : GUIFader 
{
    [System.Serializable]
    struct LegendSettings
    {
        public Text TextField;
        public Image ColorField;
        public string Text;

        public bool IsSetAllComponents()
        {
            return (!TextField || !ColorField) ? false : true;
        }
    }

    [SerializeField]
    LegendSettings m_BabyComponents = new LegendSettings();
    [SerializeField]
    LegendSettings m_TeenagerComponents = new LegendSettings();
    [SerializeField]
    LegendSettings m_AdultComponents = new LegendSettings();
    [SerializeField]
    LegendSettings m_GrandparentsComponents = new LegendSettings();

	// Use this for initialization
	new void Start ()
    {
        if (!m_BabyComponents.IsSetAllComponents())
            Debug.LogError("Es wurden nicht alle Komponenten des Baby Elements in der Zellen Legende gesetzt. Das Element kann nicht richtig dargestellt werden.");

        if (!m_TeenagerComponents.IsSetAllComponents())
            Debug.LogError("Es wurden nicht alle Komponenten des Teenager Elements in der Zellen Legende gesetzt. Das Element kann nicht richtig dargestellt werden.");

        if (!m_AdultComponents.IsSetAllComponents())
            Debug.LogError("Es wurden nicht alle Komponenten des Erwachsenen Elements in der Zellen Legende gesetzt. Das Element kann nicht richtig dargestellt werden.");

        if (!m_GrandparentsComponents.IsSetAllComponents())
            Debug.LogError("Es wurden nicht alle Komponenten des Großeltern Elements in der Zellen Legende gesetzt. Das Baby Element kann nicht richtig dargestellt werden.");

        SetLegend();

        base.Start();
	}

    new void Update()
    {
        base.Update();
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
                m_PanelTransform.sizeDelta = new Vector2(m_PanelTransform.sizeDelta.x, -190);
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

    void SetLegend()
    {
        m_BabyComponents.ColorField.color = GameManager.Instance.BabySettings.AgeColor;
        m_BabyComponents.TextField.text = string.Format(m_BabyComponents.Text, GameManager.Instance.BabySettings.Age);

        m_TeenagerComponents.ColorField.color = GameManager.Instance.TeenagerSettings.AgeColor;
        m_TeenagerComponents.TextField.text = string.Format(m_TeenagerComponents.Text, GameManager.Instance.TeenagerSettings.Age);

        m_AdultComponents.ColorField.color = GameManager.Instance.AdultSettings.AgeColor;
        m_AdultComponents.TextField.text = string.Format(m_AdultComponents.Text, GameManager.Instance.AdultSettings.Age);

        m_GrandparentsComponents.ColorField.color = GameManager.Instance.GrandparentsSettings.AgeColor;
        m_GrandparentsComponents.TextField.text = string.Format(m_GrandparentsComponents.Text, GameManager.Instance.GrandparentsSettings.Age);
    }
}
