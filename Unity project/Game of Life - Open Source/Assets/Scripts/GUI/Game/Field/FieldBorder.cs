/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das sich der Rahmen um das      *
 * Spielfeld automatisch an die Feldgröße anpasst.                              *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldBorder : MonoBehaviour 
{
    [SerializeField]
    Vector2 m_Offset = Vector2.zero;

    RectTransform m_ImageTransform = null;

	// Use this for initialization
	void Start () 
    {
        m_ImageTransform = GetComponentInChildren<RectTransform>();
        if (!m_ImageTransform)
        {
            Debug.LogError("Es wurde keine RectTansform Komponente in den Kind Objekten des Gameobjektes: " + name + " gefunden! Der Spielfeldrahmen kann nicht erstellt werden.");
            enabled = false;
        }

        SetBorderSize();
	}

    void SetBorderSize()
    {
        float LeftPos = Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[0, 0].transform.position).x - m_Offset.x;
        float RightPos = (Screen.width - Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[(int)GameManager.Instance.GetGridManager.FieldSize.x - 1, 0].transform.position).x) - m_Offset.x;
        float TopPos = (Screen.height - Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[0, 0].transform.position).y) - m_Offset.y;
        float BottomPos = Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[0, (int)GameManager.Instance.GetGridManager.FieldSize.y - 1].transform.position).y - m_Offset.y;

        m_ImageTransform.offsetMin = new Vector2(LeftPos, BottomPos);
        m_ImageTransform.offsetMax = new Vector2(RightPos * -1, TopPos * -1);
    }

    public void InvisibleBorder()
    {
        Image TempBorderImage = GetComponentInChildren<Image>();
        if (!TempBorderImage)
        {
            Debug.LogError("In den Kinder Objekten des Gameobjektes: " + name + " wurde keine Image Komponente gefunden! Der Rahmen kann nicht unsichtbar gemacht werden.");
            return;
        }

        TempBorderImage.enabled = false;
    }

    public void VisibleBorder()
    {
        Image TempBorderImage = GetComponentInChildren<Image>();
        if (!TempBorderImage)
        {
            Debug.LogError("In den Kinder Objekten des Gameobjektes: " + name + " wurde keine Image Komponente gefunden! Der Rahmen kann nicht sichtbar gemacht werden.");
            return;
        }

        TempBorderImage.enabled = true;
    }
}
