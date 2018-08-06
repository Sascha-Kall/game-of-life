/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt und erstellt die MessageBox          *
 * in dem Spiel                                                                 *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public enum BUTTONPRESSED { NONE, OK };

    [SerializeField]
    Text m_TitleField = null;
    [SerializeField]
    Text m_TextField = null;
    [SerializeField]
    Button m_OKButton = null;

    static BUTTONPRESSED m_ButtonPressed = BUTTONPRESSED.NONE;

    static GameObject m_MessageBoxObject = null;

    public string Title
    {
        get { return m_TitleField.text; }
        set { m_TitleField.text = value; }
    }

    public string Text
    {
        get { return m_TextField.text; }
        set { m_TextField.text = value; }
    }

    public Button OKButton
    {
        get { return m_OKButton; }
    }

    public BUTTONPRESSED PressedButton
    {
        get { return m_ButtonPressed; }
    }

    public static MessageBox Show(string _title, string _text)
    {
        m_ButtonPressed = BUTTONPRESSED.NONE;

        GameObject m_MessageBoxObjectPrefab = (GameObject)Resources.Load("Prefabs/MessageBox", typeof(GameObject));
        if (!m_MessageBoxObjectPrefab)
        {
            Debug.Log("Das MessageBox Prefab konnte nicht aus dem Resources Ordner geladen werden");
            return null;
        }

        m_MessageBoxObject = Instantiate(m_MessageBoxObjectPrefab, new Vector2(0, 0), Quaternion.identity) as GameObject;

        MessageBox TempMessaboxScript = m_MessageBoxObject.GetComponent<MessageBox>();
        if (!TempMessaboxScript)
        {
            Debug.LogError("Die MessageBox hat kein MessageBox Script, der Titel und der Text kann nicht angezeigt werden!");
            return null;
        }

        TempMessaboxScript.Title = _title;
        TempMessaboxScript.Text = _text;

        TempMessaboxScript.OKButton.onClick.AddListener(Close);

        return TempMessaboxScript;
    }

    public static void Close()
    {
        m_ButtonPressed = BUTTONPRESSED.OK;
        Destroy(m_MessageBoxObject);
    }
}
