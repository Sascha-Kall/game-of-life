/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt das Spiel Game of Life. Es beinhaltet*
 * alle wichtigen Manager und hat alle Informationen die das Spiel betreffen.   *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Automatisch Komponenten auf das Gameobjekt setzen lassen auf das dieses Script gesetzt wird
[RequireComponent(typeof(GridManager))]
[RequireComponent(typeof(SaveManager))]
public class GameManager : MainManager
{

#region Variablen Deklaration

    //Struct mit allen Eigenschaften für den Highscore
    public struct HighscoreSettings
    {
        public uint Highscore;
        public string Date;
    }

    GridManager m_GridManager = null; //Das GridManager Script das dass Feld Managt
    SaveManager m_SaveManager = null; //Der SaveManager Script mit dem es möglich ist wichtige Dinge wie den Highscore zu speichern
    float m_UpdateSpeed = .5f;  //Der Update Intervall der Zellen in Sekunden
    float m_UpdateSpeedMultiplicator = 1f;  //Der Update Intervall Multiplikator mit dem es möglich ist den Update Intervall der Zellen zu erhöhen bzw. zu verringern
    IEnumerator m_UpdateCoRoutine = null; //Die aktuelle Zellupdate CoRoutine in der die Zellen upgedatet werden
    ulong m_Generations = 0UL; //Die Anzahl der Generationen die sich aktuell auf dem Spielfeld befinden
    HighscoreSettings m_Highscore = new HighscoreSettings(); //Struct mit den Eigenschaften des Highscores
    Texture2D m_FieldScreenshot = null; //Der Screenshot der beim Starten des Spieles von dem aktuellen Feld gemacht wurde und gespeichert wird wenn ein neuer Highescore erreicht wird

#endregion

#region Events Deklaration

    //Event das aufgerufen wird, wenn das Spielfeld komplett gelöscht wird
    public delegate void OnClearGameDelegate();
    public static event OnClearGameDelegate OnClearGame;

    //Event das aufgerufen wird, wenn ein neuer Highscore erreicht wird
    public delegate void OnNewHighscoreDelegate(HighscoreSettings _highscore);
    public static event OnNewHighscoreDelegate OnNewHighscore;

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft Instance gibt die aktuelle Instanz des Game Managers zurück.</value>
    public static GameManager Instance
    {
        get;
        private set;
    }

    /// <value>Die Eigenschaft GetGridManager gibt die aktuelle Instanz des Grid Managers zurück.</value>
    public GridManager GetGridManager
    {
        get { return m_GridManager; }
    }

    /// <value>Die Eigenschaft GetSaveManager gibt die aktuelle Instanz des Save Managers zurück.</value>
    public SaveManager GetSaveManager
    {
        get { return m_SaveManager; }
    }

    /// <value>Die Eigenschaft UpdateSpeedMultiplicator gibt den aktuellen Speed Multiplikator zurück bzw. setzt ihn.</value>
    public float UpdateSpeedMultiplicator
    {
        get { return m_UpdateSpeedMultiplicator; }
        set { m_UpdateSpeedMultiplicator = value; }
    }

    /// <value>Die Eigenschaft Generations gibt die aktuelle Anzahl der Generationen auf dem Feld zurück.</value>
    public ulong Generations
    {
        get { return m_Generations; }
    }

    /// <value>Die Eigenschaft Highscore gibt den aktuellen Highscore zurück.</value>
    public HighscoreSettings Highscore
    {
        get { return m_Highscore; }
    }

#endregion

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
    void Awake()
    {
        //Singleton vom GameManager erstellen, damit sichergestellt ist das es ihn nur einmal in dem Spiel gibt
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
    void Start()
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        m_GridManager = GetComponent<GridManager>();
        if (!m_GridManager)
        {
            Debug.LogError("Es wurde kein GridManager Script auf dem Gameobjekt: " + name + " gefunden! Der GameManager wird beendet.");
            enabled = false;
        }

        m_SaveManager = GetComponent<SaveManager>();
        if (!m_SaveManager)
        {
            Debug.LogError("Es wurde kein SaveManager Script auf dem Gameobjekt: " + name + " gefunden! Der GameManager wird beendet.");
            enabled = false;
        }

        //Das Spielfeld Spawnen lassen mit den Eigenschaften der Altersgruppen sowie die Farbe der Gestorbenen Zelle diese stammt von der 
        //Basis Klasse MainManager die alle wichtigen Eigenschaften der Hautmanager beinhaltet
        m_GridManager.CreateField(BabySettings, TeenagerSettings, AdultSettings, GrandparentsSettings, DeadColor);
        
        //Den letzten Highscore laden lassen
        m_Highscore.Highscore = m_SaveManager.GetHighscore();

        //Überprüfen ob schon ein Highscore erspielt wurde, wenn ja wird das Datum des erspielten Highscore auch geladen
        if (m_Highscore.Highscore != 0)
            m_Highscore.Date = m_SaveManager.GetDate();

        //Der aktuelle Spielzustand wird auf dem Create Mode gesetzt. Damit ist es dann möglich das durch den Benutzer Zellen als "Lebendig" markiert werden können
        GameSettings.Instance.ChangeGameState(GameSettings.GAMESTATE.CREATE);

        //Der Zellen Update CoRoutine wird die Methode für das Update zugewiesen
        m_UpdateCoRoutine = CellUpdateDelay();

        //Überprüfen um welchen gewählten Game-mode es sich aktuell handelt. Wenn es der Mode "Game" ist, werden die Benutzerdefinierten Regeln eingeschaltet.
        //Diese beinhaltet dass die Zellen 5 Generationen nach der Altersgruppe Senior an Altersschwäche sterben sowie das sich die Zellen nur in den
        //Altersgruppen Baby bis Erwachsen Fortpflanzen können
        if (GameSettings.Instance.CurrentGameMode == GameSettings.GAMEMODE.GAME)
        {
            m_CustomRules.UsedCustomRules = true;
            m_CustomRules.AgeWeaknessesOfOld = GrandparentsSettings.Age + 5;
            m_CustomRules.MinAgeReproductions = Cell.AGES.BABY;
            m_CustomRules.MaxAgeReproductions = Cell.AGES.ADULT;
        }
    }

    //Unity Methode die aufgerufen wird, wenn das Script aktiviert wird
    void OnEnable()
    {
        //Das Event abonnieren, das aufgerufen wird, wenn das Spielfeld keine "Lebenden" Zellen mehr aufweist
        GridManager.OnNoLivingCells += OnNoLivingCells;
    }

    //Unity Methode die aufgerufen wird, wenn das Script deaktiviert wird
    void OnDisable()
    {
        //Das Event abonnieren, das aufgerufen wird, wenn das Spielfeld keine "Lebenden" Zellen mehr aufweist
        GridManager.OnNoLivingCells -= OnNoLivingCells;
    }

    /// <summary>CoRoutine die die Zellen auf dem Spielfeld in einem bestimmten Zeitintervall und Speed Multiplikator updaten lässt</summary>
    /// <remarks>Updatet in einem bestimmten Zeitintervall die Zellen auf dem Spielfeld. Diese CoRoutine wird sooft aufgerufen bis sie
    /// explizit beendet wird mit den Methoden "BreakGame" oder "EndGame"</remarks>
    IEnumerator CellUpdateDelay()
    {
        //Endlosschleife damit die Zellen solange in einem bestimmten Zeitintervall upgedatet werden solange das Spiel nicht Pausiert bzw.
        //Beendet wird
        while (true)
        {
            yield return new WaitForSeconds(m_UpdateSpeed * m_UpdateSpeedMultiplicator); //Wartet bis die Update-Intervall zeit * dem Speed-Multiplikator vergangen ist

            CellUpdate(); //Startet das Updaten der einzelnen Zellen auf dem Spielfeld
        }
    }

    /// <summary>Updatet die Zellen auf dem Spielfeld</summary>
    /// <remarks>Startet die Update Methode der einzelnen Zellen auf dem Spielfeld. Dabei wird unterschieden ob diese mit den gängigen Regeln
    /// von Game of Life oder mit den Benutzerdefinierten Regeln upgedatet werden sollen</remarks>
    public void CellUpdate()
    {
        //Überprüfen ob mit den gängigen Game of Life Regeln upgedatet werden soll oder mit den Benutzerdefinierten. Damit werden die Zelle mit dem aktuellen
        //Zellenzustand upgedatet und festgelegt was mit ihnen in der nächsten Generation passiert
        if (UsedCustomRules)
        {
            for (int x = 0; x < m_GridManager.FieldSize.x; x++)
            {
                for (int y = 0; y < m_GridManager.FieldSize.y; y++)
                    m_GridManager.Field[x, y].CellUpdate(m_CustomRules);
            }
        }
        else
        {
            for (int x = 0; x < m_GridManager.FieldSize.x; x++)
            {
                for (int y = 0; y < m_GridManager.FieldSize.y; y++)
                    m_GridManager.Field[x, y].CellUpdate();
            }
        }
       
        //Das "LateUpdate" der einzelnen Zellen auf dem Feld ausführen, damit deren neuer Zellzustand für die nächste Generation gesetzt wird
        for (int x = 0; x < m_GridManager.FieldSize.x; x++)
        {
            for (int y = 0; y < m_GridManager.FieldSize.y; y++)
                m_GridManager.Field[x, y].CellLateUpdate();
        }

        //Generationen Zähler um eins erhöhen
        m_Generations++;
    }

    /// <summary>Macht einen Screenshot von dem Spielfeld</summary>
    /// <remarks>Es wird das Script "FieldScreenshot" in der aktuellen Scene gesucht und mit dessen Hilfe ein Screenshot von dem aktuellen
    /// Spielfeld erstellt. Dieses wird dann in der Variable "m_FieldScreenshot" zwischengespeichert</remarks>
    public void TakeFieldScreenshot()
    {
        //Das Script "FieldScreenshot" in der aktuellen Scene suchen. Dies wird benötigt um einen Screenshot des Spielfeldes zu erstellen
        FieldScreenshot TempFieldScreenshot = FindObjectOfType<FieldScreenshot>();
        if (!TempFieldScreenshot)
        {
            Debug.LogError("Es wurde kein FieldScreenshot Script in der aktuellen Scene gefunden!");
            return;
        }

        //Screenshot erstellen und zwischenspeichern bis es endgültig bei einem neuen Highscore gespeichert wird
        m_FieldScreenshot = TempFieldScreenshot.MakeScreenshot();
    }

    /// <summary>Startet das Updaten der Zellen und somit das Spiel</summary>
    /// <remarks>Der aktuelle Spielzustand wird auf "Spielen" gesetzt und das Updaten der Zellen gestartet</remarks>
    public void StartGame()
    {
        GameSettings.Instance.ChangeGameState(GameSettings.GAMESTATE.RUN); //Setzt den aktuellen Spielzustand auf "Spielen"

        //Startet das Updaten der einzelnen Zellen auf dem Spielfeld
        StartCoroutine(m_UpdateCoRoutine);
    }

    /// <summary>Pausiert das Updaten der Zellen und somit das Spiel</summary>
    /// <remarks>Der aktuelle Spielzustand wird auf "Pause" gesetzt und das Updaten der Zellen gestoppt</remarks>
    public void BreakGame()
    {
        GameSettings.Instance.ChangeGameState(GameSettings.GAMESTATE.BREAK); //Setzt den aktuellen Spielzustand auf "Pause"

        //Stoppt das Updaten der einzelnen Zellen auf dem Spielfeld
        StopCoroutine(m_UpdateCoRoutine);
    }

    /// <summary>Beendet das Updaten der Zellen und somit das Spiel</summary>
    /// <remarks>Der aktuelle Spielzustand wird auf "Ende" gesetzt und das Updaten der Zellen gestoppt</remarks>
    public void EndGame()
    {
        GameSettings.Instance.ChangeGameState(GameSettings.GAMESTATE.END); //Setzt den aktuellen Spielzustand auf "Ende"

        //Stoppt das Updaten der einzelnen Zellen auf dem Spielfeld
        StopCoroutine(m_UpdateCoRoutine);
    }

    /// <summary>Löscht das aktuelle Spielfeld und setzt alle Zellen wieder auf ihren Ursprungszustand</summary>
    /// <remarks>Setzt das Spielfeld auf den Ursprungszustand zurück und setzt alle relevanten Werte wieder zurück</remarks>
    public void ClearGame()
    {
        StopCoroutine(m_UpdateCoRoutine); // Das Zellupdate der Zellen auf dem Spielfeld wird gestoppt

        //Alle relevanten Einstellungen werden wieder zurückgesetzt
        GameSettings.Instance.ChangeGameState(GameSettings.GAMESTATE.CREATE); 
        m_Generations = 0UL; 
        m_GridManager.ClearFeld(); 

        //Wenn das Event abonniert wurde, dass aufgerufen wird wenn das Spiel zurückgesetzt wird, wird gestartet
        if (OnClearGame != null)
            OnClearGame();
    }

    /// <summary>Methode die beim Event aufgerufen wird, wenn es keine "Lebende" Zellen mehr auf dem Spielfeld gibt</summary>
    /// <remarks>Je nach Spielmodus wird überprüft ob es einen neuen Highscore gibt oder es wird eine Meldung mit den überlebten Generationen angezeigt</remarks>
    void OnNoLivingCells()
    {
        //Überprüfen ob das Spielzustand aktuell auf "Spielen" steht
        if (GameSettings.Instance.CurrentGameState == GameSettings.GAMESTATE.RUN)
        {
            EndGame(); //Das Spiel beenden

            bool TempNewHighscore = false;

            //Überprüfen ob der Spielmodus "Game" gewählt wurde und ob es einen neuen Highscore an überlebten Zellen gibt. Sollte dies der Fall sein
            //wird die Anzahl, das aktuelle Datum sowie der Screenshot des Spielfeldes gespeichert.
            if (GameSettings.Instance.CurrentGameMode == GameSettings.GAMEMODE.GAME)
            {
                if (m_Generations > m_Highscore.Highscore)
                {
                    HighscoreSettings TempSetting = new HighscoreSettings();
                    TempSetting.Highscore = (uint)m_Generations;
                    TempSetting.Date = string.Format("{0}.{1}.{2}", System.DateTime.Now.Day, System.DateTime.Now.Month, System.DateTime.Now.Year);

                    //Überprüfen ob das Event für einen neuen Highscore abonniert wurde, wenn ja wird das Event aufgerufen
                    if (OnNewHighscore != null)
                        OnNewHighscore(TempSetting);

                    m_Highscore = TempSetting;
                    m_SaveManager.SaveHighscore(TempSetting.Highscore);
                    m_SaveManager.SaveDate(TempSetting.Date);

                    m_SaveManager.SaveScreenshot(m_FieldScreenshot);

                    TempNewHighscore = true;
                }
                else
                    m_FieldScreenshot = null;
            }

            //Starten der CoRoutine für das anzeigen der Message box
            StartCoroutine(ShowMessageBox(TempNewHighscore));
        }
    }

    /// <summary>CoRoutine die eine Message box anzeigt</summary>
    /// <param name="_showHighscore">Soll eine neuer Highscore angezeigt werden</param>
    /// <remarks>Zeigt eine Message box die entweder den neuen Highscore oder die Anzahl an überlebten Generationen anzeigt</remarks>
    IEnumerator ShowMessageBox(bool _showHighscore)
    {
        string TempTitle = "";
        string TempText = "";

        //Überprüfen ob ein neuer Highscore anzeigt werden soll oder nur die Anzahl an überlebten Generationen. Danach richtet sich dann der angezeigt
        //Text bzw. Titel der Message box
        if (_showHighscore)
        {
            TempTitle = "Neuer Highscore";
            TempText = string.Format("Glückwunsch! Du hast einen neuen Highscore mit <b>{0}</b> überlebten Generationen erstellt.", Generations);
        }
        else
        {
            TempTitle = "Keine überlebende Zellen mehr";
            TempText = string.Format("Simulation ist beendet. Deine Zellen haben: <b>{0}</b> Generationen überlebt.", Generations);
        }

        //Message box anzeigen
        MessageBox TempMessageBox = MessageBox.Show(TempTitle, TempText);

        //Warten bis bei der Message box der OK Button geklickt wurde
        yield return new WaitUntil(() => TempMessageBox.PressedButton == MessageBox.BUTTONPRESSED.OK);
      
        //Spiel zurücksetzen lassen
        ClearGame();
    }
}