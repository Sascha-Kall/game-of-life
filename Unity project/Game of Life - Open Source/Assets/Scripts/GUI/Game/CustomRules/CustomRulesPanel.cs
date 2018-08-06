/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt das GUI Panel das für die            *
 * Benutzerdefinierten Regeln zuständig ist.                                    *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomRulesPanel : GUIFader
{
    [SerializeField]
    GameObject m_ShowPanelButton = null;
    [SerializeField]
    GameObject m_ClosePanelButton = null;
    [SerializeField]
    Dropdown m_MinAgeReproductionElement = null;
    [SerializeField]
    Dropdown m_MaxAgeReproductionElement = null;
    [SerializeField]
    InputField m_CellsOldWeaknessesElement = null;
    [SerializeField]
    Text m_WarningField = null;

    new void Start()
    {
        if (!m_ShowPanelButton)
        {
            Debug.LogError("Es wurde kein Button angegeben der das Settings Panel aufruft! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }

        if (!m_ClosePanelButton)
        {
            Debug.LogError("Es wurde kein Button angegeben der das Settings Panel schließt! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }

        if (!m_MinAgeReproductionElement)
        {
            Debug.LogError("Es wurde keine DropDown Komponente angegeben in der die Mindestaltersgruppe für die reproduction angegeben ist! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }

        if (!m_MaxAgeReproductionElement)
        {
            Debug.LogError("Es wurde keine DropDown Komponente angegeben in der die maximal Altersgruppe für die reproduction angegeben ist! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }

        if(!m_CellsOldWeaknessesElement)
        {
            Debug.LogError("Es wurde keine DropDown Komponente angegeben in der die maximal Altersgruppe für die reproduction angegeben ist! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }

        if (!m_WarningField)
        {
            Debug.LogError("Es wurde keine Text Komponente angegeben in dem die Warnungen ausgegeben werden! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }

        base.Start();
    }

    void OnEnable()
    {
        GameSettings.OnChangeGameState += OnChangeGameState;
    }

    void OnDisable()
    {
        GameSettings.OnChangeGameState -= OnChangeGameState;
    }

    new void Update()
    {
        base.Update();
    }

    void OnChangeGameState(GameSettings.GAMESTATE _newState)
    {
        switch (_newState)
        {
            case GameSettings.GAMESTATE.CREATE:
                m_PanelTransform.sizeDelta = new Vector2(-10f, m_PanelTransform.sizeDelta.y);


                if (GameSettings.Instance.CurrentGameMode == GameSettings.GAMEMODE.CREATIVE)
                {
                    m_ShowPanelButton.SetActive(true);

                    EnabelElement();
                }
                else
                {
                    m_ShowPanelButton.SetActive(false);
                    DisabelElement();
                }

                break;

            case GameSettings.GAMESTATE.RUN:
                DisabelElement();
                m_ShowPanelButton.SetActive(false);
                break;

            case GameSettings.GAMESTATE.BREAK:
                m_ShowPanelButton.SetActive(false);
                DisabelElement();
                break;
        }
    }

   

    public void SetAgeWeaknessesOfOld()
    {
        int TempValue = GetIntValueFromInputField();

        if (TempValue < 0 || TempValue > 999)
        {
            m_WarningField.text = "Bitte geben sie eine gültige Zahl zwischen 0 und 999 ein!";
            SetInteractivButtonFromGameobject(m_ClosePanelButton, false);
            return;
        }
        else
        {
            SetInteractivButtonFromGameobject(m_ClosePanelButton, true);
            m_WarningField.text = "";
        }

        GameManager.Instance.AgeWeaknessesOfOld = TempValue;

        if (TempValue <= GameManager.Instance.GrandparentsSettings.Age && TempValue != 0)
            m_WarningField.text = "Warnung: Das eingegeben Zellen Maximalalter ist jünger als das Alter der Senioren!";
        else
            m_WarningField.text = "";
    }

    public void SetUsedCustomRules()
    {
        bool TempIsUsed = GetValueFromToggle();

        GameManager.Instance.UsedCustomRules = TempIsUsed;

        if (TempIsUsed)
        {
            m_MinAgeReproductionElement.interactable = true;
            m_MaxAgeReproductionElement.interactable = true;
            m_CellsOldWeaknessesElement.interactable = true;

            GameManager.Instance.MinAgeReproductions = (Cell.AGES)m_MinAgeReproductionElement.value + 1;
            GameManager.Instance.MaxAgeReproductions = (Cell.AGES)m_MaxAgeReproductionElement.value + 1;
            GameManager.Instance.AgeWeaknessesOfOld = 0;
        }
        else
        {
            GameManager.Instance.ResetCustomRules();
            m_MinAgeReproductionElement.interactable = false;
            m_MaxAgeReproductionElement.interactable = false;
            m_CellsOldWeaknessesElement.interactable = false;
            m_MinAgeReproductionElement.value = (int)GameManager.Instance.MinAgeReproductions - 1;
            m_MaxAgeReproductionElement.value = (int)GameManager.Instance.MaxAgeReproductions - 1;
            m_CellsOldWeaknessesElement.text = string.Empty;
            m_WarningField.text = "";
        }
    }

    public void SetMinAgeReproduction()
    {
        int TempIndex = GetIndexFromDropDown();

        switch (TempIndex)
        {
            case 0:
                GameManager.Instance.MinAgeReproductions = Cell.AGES.BABY;
                break;

            case 1:
                GameManager.Instance.MinAgeReproductions = Cell.AGES.TEENAGER;
                break;

            case 2:
                GameManager.Instance.MinAgeReproductions = Cell.AGES.ADULT;
                break;

            case 3:
                GameManager.Instance.MinAgeReproductions = Cell.AGES.GRANDPARENTS;
                break;
        }
        
        if (GameManager.Instance.MinAgeReproductions >= GameManager.Instance.MaxAgeReproductions)
            m_WarningField.text = "Warnung: Das Mindestalter für die Fortpflanzung ist älter oder gleich dem Maximalalter für die Fortpflanzung!";
        else
            m_WarningField.text = "";
    }

    public void SetMaxAgeReproduction()
    {
        int TempIndex = GetIndexFromDropDown();

        switch (TempIndex)
        {
            case 0:
                GameManager.Instance.MaxAgeReproductions = Cell.AGES.BABY;
                break;

            case 1:
                GameManager.Instance.MaxAgeReproductions = Cell.AGES.TEENAGER;
                break;

            case 2:
                GameManager.Instance.MaxAgeReproductions = Cell.AGES.ADULT;
                break;

            case 3:
                GameManager.Instance.MaxAgeReproductions = Cell.AGES.GRANDPARENTS;
                break;
        }
      
        if (GameManager.Instance.MaxAgeReproductions <= GameManager.Instance.MinAgeReproductions)
            m_WarningField.text = "Warnung: Das Maximalalter für die Fortpflanzung ist jünger oder gleich dem Mindestalter für die Fortpflanzung!";
        else
            m_WarningField.text = "";
    }
}