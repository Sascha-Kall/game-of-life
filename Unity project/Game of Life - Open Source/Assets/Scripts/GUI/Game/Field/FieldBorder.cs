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
    Vector2 m_MinOffsetDefault = Vector2.zero;
    [SerializeField]
    Vector2 m_MaxOffsetDefault = Vector2.zero;
    [SerializeField]
    Vector2 m_MinOffsetWebGL = Vector2.zero;
    [SerializeField]
    Vector2 m_MaxOffsetWebGL = Vector2.zero;

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
        float LeftPos = Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[0, 0].transform.position).x;
        float RightPos = (Screen.width - Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[(int)GameManager.Instance.GetGridManager.FieldSize.x - 1, 0].transform.position).x);
        float TopPos = (Screen.height - Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[0, 0].transform.position).y);
        float BottomPos = Camera.main.WorldToScreenPoint(GameManager.Instance.GetGridManager.Field[0, (int)GameManager.Instance.GetGridManager.FieldSize.y - 1].transform.position).y;

        #if UNITY_WEBGL
            m_ImageTransform.offsetMin = new Vector2(LeftPos - m_MinOffsetWebGL.x, BottomPos - m_MinOffsetWebGL.y);
            m_ImageTransform.offsetMax = new Vector2((RightPos - m_MaxOffsetWebGL.x) * -1, (TopPos - m_MaxOffsetWebGL.y) * -1);
        #else
            m_ImageTransform.offsetMin = new Vector2(LeftPos - m_MinOffsetDefault.x, BottomPos - m_MinOffsetDefault.y);
            m_ImageTransform.offsetMax = new Vector2((RightPos - m_MaxOffsetDefault.x) * -1, (TopPos - m_MaxOffsetDefault.y) * -1);
        #endif
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
