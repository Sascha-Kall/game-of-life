/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das GUI Elemente einfach von    *
 * einer bestimmten Seite eingefadet werden können                              *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIFader : GUIElementFunctions 
{
    enum FADETO { NONE, IN, OUT };
    enum AXIS { X, Y}

    [SerializeField]
    AXIS m_Axis = AXIS.X;
    [SerializeField]
    protected float m_Size = 480f;
    [SerializeField]
    protected float m_DefaultSize = -10f;
    [SerializeField]
    protected float m_FadeSpeed = 15f;
    [SerializeField]
    protected RectTransform m_PanelTransform = null;

    float m_FadeToValue = -10f;
    FADETO m_FadeState = FADETO.NONE;


	// Use this for initialization
    protected void Start()
    {
        if (!m_PanelTransform)
        {
            Debug.LogError("Es wurde kein Rect Transform Component des Settings Panels angegeben! Das Script FieldSettingsButton wird beendet.");
            enabled = false;
        }
	}
	
	// Update is called once per frame
	protected void Update ()
    {
		 if (m_FadeState != FADETO.NONE)
        {
            if(m_Axis == AXIS.X)
                m_PanelTransform.sizeDelta = Vector2.Lerp(m_PanelTransform.sizeDelta, new Vector2(m_FadeToValue, m_PanelTransform.sizeDelta.y), Time.deltaTime * m_FadeSpeed);
            else
                m_PanelTransform.sizeDelta = Vector2.Lerp(m_PanelTransform.sizeDelta, new Vector2(m_PanelTransform.sizeDelta.x, m_FadeToValue), Time.deltaTime * m_FadeSpeed);
        
            if (Mathf.Abs(m_PanelTransform.sizeDelta.x - m_FadeToValue) < 0.01f)
                m_FadeState = FADETO.NONE;
        }
	}

    public void StartFadeTo(int _fadeTo)
    {
        switch ((FADETO)_fadeTo)
        {
            case FADETO.IN:
                m_FadeToValue = m_Size;
                m_FadeState = FADETO.IN;
                break;

            case FADETO.OUT:
                m_FadeToValue = m_DefaultSize;
                m_FadeState = FADETO.OUT;
                break;
        }
    }
}
