/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das dass Highscore Feld         *
 * angezeigt wird                                                               *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreField : GUIElementFunctions 
{
    [SerializeField]
    Image m_FieldPlace = null;

	// Use this for initialization
	void Start () 
    {
        if (!m_FieldPlace)
        {
            Debug.LogError("Es wurde keine Image Komponente bei dem Gameobjekt: "+ name + " angegeben wo der Feld Highscore angezeigt werden kann!");
            enabled = false;
        }
	}

    public void EnableWindow(Texture2D _screenshot)
    {
        GameManager.Instance.GetGridManager.InvisibleField();

        m_FieldPlace.sprite = Sprite.Create(_screenshot, new Rect(0, 0, _screenshot.width, _screenshot.height), new Vector2(0.5f, 0.5f), 100f);
        EnabelElement();
    }

    public void DisableWindow()
    {
        GameManager.Instance.GetGridManager.VisibleField();

        m_FieldPlace.sprite = null;
        DisabelElement();
    }
}
