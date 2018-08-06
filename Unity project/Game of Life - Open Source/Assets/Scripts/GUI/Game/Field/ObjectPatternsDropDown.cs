/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das man Vorlagen von Objekten   *
 * auf das Feld setzen kann                                                     *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPatternsDropDown : GUIElementFunctions 
{
    [System.Serializable]
    struct Pattern
    {
        public string Name;
        public List<Vector2> Points;
    }

    [SerializeField]
    List<Pattern> m_ObjectPatterns = new List<Pattern>();

    Dropdown m_DropDown = null;
    int m_SelectedPatternIndex = -1;

	// Use this for initialization
    void Awake()
    {
        if (m_ObjectPatterns.Count == 0)
        {
            Debug.LogWarning("Es wurden keine Object Vorlagen angegeben!. Das Drop-down mit den Vorlagen wird nicht erstellt.");
            enabled = false;
        }

        m_DropDown = GetComponentInChildren<Dropdown>();
        if (!m_DropDown)
        {
            Debug.LogError("Es wurde in den Kind Objekten des Gameobjektes: " + name + " keine DropDown Komponente gefunden!. Das Objektpatern Script wird deaktiviert.");
            enabled = false;
        }

        SetDropDownValues();
    }

    void OnEnable()
    {
        GameManager.OnClearGame += OnClearGame;
    }

    void OnDisable()
    {
        GameManager.OnClearGame -= OnClearGame;
    }

    void SetDropDownValues()
    {
        m_DropDown.ClearOptions();

        List<string> TempOptions = new List<string>();

        foreach (Pattern _p in m_ObjectPatterns)
            TempOptions.Add(_p.Name);

        m_DropDown.AddOptions(TempOptions);
    }

    public void SetObjectPatterns()
    {
        int TempSelectedPattern = GetIndexFromDropDown();

        if (TempSelectedPattern == 0)
        {
            DeletePatterns();
            return;
        }

        SetPatterns(m_ObjectPatterns[TempSelectedPattern].Points, Cell.STATES.ALIVE, GameManager.Instance.GetGridManager.GetCenterPoint());

        m_SelectedPatternIndex = TempSelectedPattern;
    }

    void DeletePatterns()
    {
        if (m_SelectedPatternIndex <= 0)
            return;

        SetPatterns(m_ObjectPatterns[m_SelectedPatternIndex].Points, Cell.STATES.DEAD, GameManager.Instance.GetGridManager.GetCenterPoint());

        m_SelectedPatternIndex = -1;
    }

    void SetPatterns(List<Vector2> _patternPoints, Cell.STATES _cellState, Vector2 _centerPoint)
    {
        for (int i = 0; i < _patternPoints.Count; i++)
        {
            GameManager.Instance.GetGridManager.Field[(int)(_centerPoint.x + _patternPoints[i].x), (int)(_centerPoint.y + _patternPoints[i].y)].SetCellState(_cellState);
        }
    }

    void OnClearGame()
    {
        DeletePatterns();
        m_DropDown.value = 0;
    }
}

