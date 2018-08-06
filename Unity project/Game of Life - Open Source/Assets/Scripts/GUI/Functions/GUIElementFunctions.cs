/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse enthält allgemeine Funktionen für das GUI   *
 * in dem Spiel. Damit lassen sich z.B einzelne Elemente einfach ein bzw. aus   *
 * schalten                                                                     *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GUIElementFunctions : MonoBehaviour 
{
    public void DisabelElement()
    {
        foreach (Transform _tr in transform)
            _tr.gameObject.SetActive(false);
    }

    public bool IsDisableElement()
    {
        foreach (Transform _tr in transform)
            if (_tr.gameObject.activeSelf)
                return false;

        return true;
    }

    public void EnabelElement()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        foreach (Transform _tr in transform)
            _tr.gameObject.SetActive(true);
    }

    public bool IsEnableElement()
    {
        foreach (Transform _tr in transform)
            if (!_tr.gameObject.activeSelf)
                return false;

        return true;
    }

    public void DisabelThisButton()
    {
        Button TempButton = GetComponent<Button>();
        if (!TempButton)
        {
            Debug.LogError("Auf dem Gameobjekt: " + name + " wurde keine Button Komponente gefunden! Der Button kann nicht deaktiviert werden.");
            return;
        }

        Text TempText = GetComponentInChildren<Text>();
        if (TempText)
            TempText.enabled = false;

        TempButton.image.enabled = false;
        TempButton.enabled = false;
        
    }

    public void DisabelThisButton(Button _button)
    {
        Text TempText = _button.GetComponentInChildren<Text>();
        if (TempText)
            TempText.enabled = false;

        _button.image.enabled = false;
        _button.enabled = false;
    }

    public void EnableThisButton()
    {
        Button TempButton = GetComponent<Button>();
        if (!TempButton)
        {
            Debug.LogError("Auf dem Gameobjekt: " + name + " wurde keine Button Komponente gefunden! Der Button kann nicht aktiviert werden.");
            return;
        }

        Text TempText = GetComponentInChildren<Text>();
        if (TempText)
            TempText.enabled = true;

        TempButton.image.enabled = true;
        TempButton.enabled = true;
    }

    public void EnableThisButton(Button _button)
    {
        Text TempText = _button.GetComponentInChildren<Text>();
        if (TempText)
            TempText.enabled = true;

        _button.image.enabled = true;
        _button.enabled = true;
    }

    public bool GetValueFromToggle()
    {
        Toggle TempToggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
        if (!TempToggle)
        {
            Debug.LogError("Auf dem Gameobjekt: " + EventSystem.current.currentSelectedGameObject.name + " wurde keine Toggle Komponente gefunden!");
            return false;
        }

        return TempToggle.isOn;
    }

    public int GetIndexFromDropDown()
    {
        Dropdown TempDropDown = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Dropdown>();
        if (!TempDropDown)
            return 0;

        return TempDropDown.value;
    }

    public int GetIntValueFromInputField()
    {
        int TempValue = -1;

        InputField TempInputField = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();
        if (!TempInputField)
            return TempValue;

        if (!int.TryParse(TempInputField.text, out TempValue))
        {
            Debug.LogWarning("Der Text in dem Inputfield ist keine Zahl!");
            return TempValue;
        }

        return TempValue;
    }

    public void SetInteractivButtonFromGameobject(GameObject _object, bool _isInteractiv)
    {
        Button TempButton = _object.GetComponent<Button>();
        if (!TempButton)
        {
            Debug.LogError("Auf dem Gameobjekt: " + _object.name + " wurde keine Button Komponente gefunden! Es ist nicht möglich den Button auf interaktiv: " + _isInteractiv + " zu setzen.");
            return;
        }

        TempButton.interactable = _isInteractiv;
    }

    public void SetInteractivButton(Button _button, bool _isInteractiv)
    {
        Image[] TempImages = _button.GetComponentsInChildren<Image>();
        if (TempImages.Length > 0)
        {
            foreach(Image _img in TempImages)
            {
                if (_isInteractiv)
                    _img.color = _button.colors.normalColor;
                else
                    _img.color = _button.colors.disabledColor;
            }
        }

        _button.interactable = _isInteractiv;
    }

    public void LoadScene(int _scene)
    {
        SceneManager.LoadScene(_scene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
