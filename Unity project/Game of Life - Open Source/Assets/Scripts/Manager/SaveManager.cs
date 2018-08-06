/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt alles was dauerhaft gespeichert oder *
 * geladen werden muss                                                          *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{

#region Variablen Deklaration

    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    string m_HighscoreSaveName = "Highscore"; //Der Namen der beim Speicherpunkt für die Highscore Punkte verwendet werden soll
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    string m_DateSaveName = "Date"; //Der Namen der beim Speicherpunkt für das Highscore Datum verwendet werden soll
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    string m_HighscoreScreenshotPath = "Save/Screenshot"; //Der Pfad für den Highscore Feld Screenshot

#endregion

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
	void Start () 
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        if (string.IsNullOrEmpty(m_HighscoreSaveName))
        {
            Debug.LogError("Es wurde kein Speichernamen für die Highscore Punkte angegeben! Der Highscore kann nicht geladen bzw. gespeichert werden.");
            enabled = false;
        }

        if (string.IsNullOrEmpty(m_DateSaveName))
        {
            Debug.LogError("Es wurde kein Speichernamen für das Highscore Datum angegeben! Der Highscore kann nicht geladen bzw. gespeichert werden.");
            enabled = false;
        }

        if (string.IsNullOrEmpty(m_HighscoreScreenshotPath))
        {
            Debug.LogError("Es wurde kein Speicherpfad für den Highscore Feld Screenshot angegeben! Der Screenshot kann nicht geladen bzw. gespeichert werden.");
            enabled = false;
        }
	}

    /// <summary>Speichert die Highscorepunkte</summary>
    /// <param name="_score">Die Punkte die gespeichert werden sollen</param>
    /// <remarks>Speichert die Highscorepunkte in den Playerprefs von Unity</remarks>
    public void SaveHighscore(uint _score)
    {
        PlayerPrefs.SetInt(m_HighscoreSaveName, (int)_score); //Speichert die Highscorepunkte in den Playerprefs von Unity
    }

    /// <summary>Lädt die Highscorepunkte</summary>
    /// <returns>Die geladenen Highscorepunkte</returns>
    /// <remarks>Speichert die Highscorepunkte in den Playerprefs von Unity. Sollte es noch keinen Speicherpunkt für die Punkte geben wird 0 zurückgegeben</remarks>
    public uint GetHighscore()
    {
        uint TempScore = 0;

        //Überprüfen ob es einen Speicherpunkt für die Highscore Punkte gibt. Wenn nicht wird eine Fehlermeldung in der Konsole ausgegeben und 0 zurückgegeben
        if (!PlayerPrefs.HasKey(m_HighscoreSaveName))
            Debug.LogWarning("Es wurde noch kein Highscore Speicherplatz angelegt. Es wird der Defaultwert 0 zurückgegeben.");
        else
            TempScore = (uint)PlayerPrefs.GetInt(m_HighscoreSaveName); //Lädt die Highscorepunkte aus den Playerprefs von Unity

        return TempScore;
    }

    /// <summary>Speichert das Highscore Datum</summary>
    /// <param name="_date">Das Datum an dem der Highscore erreicht wurde</param>
    /// <remarks>Speichert das Highscore Datum in den Playerprefs von Unity</remarks>
    public void SaveDate(string _date)
    {
        PlayerPrefs.SetString(m_DateSaveName, _date); //Speichert das Highscore Datum in den Playerprefs von Unity
    }

    /// <summary>Lädt das Highscore Datum</summary>
    /// <returns>Das Datum an dem der Highscore erreicht wurde</returns>
    /// <remarks>Speichert das Highscore Datum in den Playerprefs von Unity. Sollte es noch keinen Speicherpunkt für das Datum geben wird ein Leerer String zurückgegeben</remarks>
    public string GetDate()
    {
        string TempDate = "";

        //Überprüfen ob es einen Speicherpunkt für das Highscore Datum gibt. Wenn nicht wird eine Fehlermeldung in der Konsole ausgegeben und ein leerer String zurückgegeben
        if (!PlayerPrefs.HasKey(m_DateSaveName))
            Debug.LogWarning("Es wurde noch kein Highscore Datum Speicherplatz angelegt. Es wird ein leerer String zurückgegeben.");
        else
            TempDate = PlayerPrefs.GetString(m_DateSaveName); //Lädt Highscore Datum aus den Playerprefs von Unity

        return TempDate;
    }

    /// <summary>Speichert den Screenshot von dem Highscore Feld</summary>
    /// <param name="_screenshot">Der Screenshot von dem Highscore Feld</param>
    /// <remarks>Speichert den Highscore Screenshot als PNG in dem angegeben Pfad innerhalb der Ordnerstruktur des Spiels</remarks>
    public void SaveScreenshot(Texture2D _screenshot)
    {
        //Überprüfen ob ein Screenshot übergeben wurde
        if (_screenshot == null)
        {
            Debug.LogError("Die übergeben Textur ist null! Der Screenshot kann nicht gespeichert werden");
            return;
        }

        //Den Screenshot in ein Byte Array umwandeln
        byte[] TempBytes = _screenshot.EncodeToPNG();

        //Überprüfen um welche Plattform es sich aktuell Handelt. Je nach verwendeter Plattform ist der verwendete Speicherpfad anders
        #if (UNITY_ANDROID)
            string TempFilePath = string.Format(@"{0}/{1}/{2}.png", Application.persistentDataPath, m_HighscoreScreenshotPath, m_HighscoreSaveName);
        #else
            string TempFilePath = string.Format(@"{0}/{1}/{2}.png", Application.dataPath, m_HighscoreScreenshotPath, m_HighscoreSaveName);
        #endif

        //Überprüfen um welche Plattform es sich aktuell Handelt. Je nach verwendeter Plattform ist der Pfad für den Ordner in dem der Screenshot gespeichert
        //wird anders
        #if (UNITY_ANDROID)
            string TempDirectoryPath = string.Format(@"{0}/{1}", Application.persistentDataPath, m_HighscoreScreenshotPath);
        #else
            string TempDirectoryPath = string.Format(@"{0}/{1}", Application.dataPath, m_HighscoreScreenshotPath);
        #endif

        //Überprüfen ob es schon einen Ordner gibt in dem die Feld Screenshots gespeichert werden. Wenn nicht dir dieser Ordner angelegt
        if (!Directory.Exists(TempDirectoryPath))
            Directory.CreateDirectory(TempDirectoryPath);

        //Den Highscore Feld Screenshot speichern
        File.WriteAllBytes(TempFilePath, TempBytes);
    }

    /// <summary>Lädt den Screenshot von dem Highscore Feld</summary>
    /// <returns>Screenshot von dem Highscore Feld</returns>
    /// <remarks>Lädt den Highscore Screenshot in dem angegeben Pfad innerhalb der Ordnerstruktur des Spiels</remarks>
    public Texture2D LoadScreenshot()
    {
        Texture2D TempScreenshot = new Texture2D(0, 0);

        //Überprüfen um welche Plattform es sich aktuell Handelt. Je nach verwendeter Plattform ist der verwendete Speicherpfad anders
        #if (UNITY_ANDROID)
            string TempPath = string.Format(@"{0}/{1}/{2}.png", Application.persistentDataPath, m_HighscoreScreenshotPath, m_HighscoreSaveName);
        #else
            string TempPath = string.Format(@"{0}/{1}/{2}.png", Application.dataPath, m_HighscoreScreenshotPath, m_HighscoreSaveName);
        #endif

        //Überprüfen ob es eine Datei unter dem gewählten Speicherpfad gibt
        if (File.Exists(TempPath))
        {
            //Bytearray mit den geladenen Bytes des Files füllen und überprüfen ob Bytes geladen wurden, wenn nicht wird eine Fehlermeldung in der Konsole 
            //ausgegeben und NULL zurückgegeben
            byte[] TempScreenshotBytes = File.ReadAllBytes(TempPath);
            if (TempScreenshotBytes.Length == 0)
            {
                Debug.LogError("Beim Laden des Screenshots ist ein Fehler aufgetreten!");
                return null;
            }

            //Das Bytearray in ein Texture2D umwandeln, damit das Bild verwendet werden kann. Sollte es zu Fehlern beim Umwandeln kommen wird eine Fehlermeldung
            //in der Konsole ausgegeben und NULL zurückgegeben
            if (!TempScreenshot.LoadImage(TempScreenshotBytes))
            {
                Debug.LogError("Beim Laden des Screenshots ist ein Fehler aufgetreten!");
                return null;
            }
        }
        else
        {
            Debug.LogError("Der Screenshot konnte bei dem Pfad: " + TempPath + " nicht geladen werden!");
            return null;
        }

        return TempScreenshot;
    }
}
