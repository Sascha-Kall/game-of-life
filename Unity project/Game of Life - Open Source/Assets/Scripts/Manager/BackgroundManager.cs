/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt den Hintergrund im Hauptmenü.        *
 * Das Script sorgt dafür das dass Spielfeld aufgebaut wird und die Zellen      *
 * sich entsprechend bewegen.                                                   *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Automatisch Komponenten auf das Gameobjekt setzen lassen auf das dieses Script gesetzt wird
[RequireComponent(typeof(GridManager))]
[RequireComponent(typeof(CameraManager))]
public class BackgroundManager : MainManager
{

#region Variablen Deklaration

    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    float m_UpdateSpeed = 1f; //In welchem Zeitintervall in Sekunden sollen die Zellen im Hintergrund upgedatet werden

    GridManager m_GridManager = null; //Das GridManager Script damit das Script das Feld mit Zellen gespawnt werden kann
    CameraManager m_CameraManager = null; //Das CameraManager Script damit errechnet werden kann wie viele Zellen bei der aktuellen Spielauflösung maximal angezeigt werden können

#endregion

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
	void Start ()
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        m_GridManager = GetComponent<GridManager>();
        if (!m_GridManager)
        {
            Debug.LogError("Auf dem Gameobjekt: " + name + " wurde kein GridManager Script gefunden! Der BackgroundManager wird ausgeschaltet.");
            enabled = false;
        }

        m_CameraManager = GetComponent<CameraManager>();
        if (!m_CameraManager)
        {
            Debug.LogError("Auf dem Gameobjekt: " + name + " wurde kein CameraManager Script gefunden! Der BackgroundManager wird beendet.");
            enabled = false;
        }

        //Hintergrund Spielfeld Spawnen lassen. Die Spielfeld Größe wird dabei von der Spielauflösung festgelegt. Es Spawnen immer soviel Zellen damit der gesamte Hintergrund
        //mit Zellen belegt ist
        SpwanCellFullScreen(); 

        //Zufällige Zellen Pärchen auf dem Spielfeld als Lebendig markieren lassen
        SelectRandomCellPairs();

        //Das Updaten der Zellen starten und somit die Hintergrund Animation starten
        StartCoroutine(StartCellUpdate());
	}

    /// <summary>Lässt ein Spielfeld spawnen das den kompletten Hintergrund bedeckt</summary>
    /// <remarks>Je nach Spielauflösung werden entsprechend viele Zellen in der X sowie Y Richtung gespawnt, damit der komplette Hintergrund bedeckt ist</remarks>
    void SpwanCellFullScreen()
    {
        //Die maximale Zellenanzahl in der X sowie Y Richtung errechnen lassen
        Vector2 MaxCells = m_CameraManager.CalcMaxCellCount(m_GridManager.GetCellSize());

        //Die Spawnposition des Spielfelderrechnen lassen damit es als Hintergrund in der Kamera zu sehen ist
        Vector2 SpawnPos = m_CameraManager.CalcCellCenterPos(m_GridManager.GetCellSize(), MaxCells);

        //Das Spielfeld Spawnen lassen mit den errechneten Position sowie Anzahl der Zellen die Eigenschaften der Altersgruppen sowie die Farbe der Gestorbenen Zelle
        //stammt von der Basis Klasse MainManager die alle wichtigen Eigenschaften der Hautmanager beinhaltet
        m_GridManager.CreateField(SpawnPos, MaxCells, BabySettings, TeenagerSettings, AdultSettings, GrandparentsSettings, DeadColor);
    }

    /// <summary>Wählt aus dem Spielfeld Zellen aus und markiert diese als Lebendig</summary>
    /// <remarks>Wählt aus dem Spielfeld per Random Zellen aus markiert dies als Lebendig. Dabei werden immer Zellen Paare per Random mit 2 oder 3 Lebenden
    /// Nachbarn gebildet</remarks>
    void SelectRandomCellPairs()
    {
        //Die Anzahl der Durchläufe für das auswähle errechnen. Dabei soll die halbe Anzahl der aktuell sich auf dem Spielfeld befindenden Zellen als Lebendig markiert
        //werden
        int TempTurns = (m_GridManager.Field.GetLength(0) * m_GridManager.Field.GetLength(1) / 2);

        for (int i = 0; i < TempTurns; i++)
        {
            //Sich per Random eine Zelle aus dem Spielfeld suchen
            Vector2 Pos = new Vector2((int)Random.Range(2, m_GridManager.Field.GetLength(0) -2), (int)Random.Range(2, m_GridManager.Field.GetLength(1) -2));

            //Die Zelle als Lebendig markieren
            m_GridManager.Field[(int)Pos.x, (int)Pos.y].SetCellState(Cell.STATES.ALIVE);

            //Per Random entscheiden lassen ob die Zelle 2 oder 3 Lebendige Nachbarszellen bekommt
            if (Random.value <= .5f)
                for (int a = 0; a < 3; a++) //3 Nachbarszellen in y Richtung als Lebendig markieren
                    m_GridManager.Field[(int)Pos.x, (int)Pos.y + a].SetCellState(Cell.STATES.ALIVE);
            else
                for (int a = 0; a < 2; a++) //2 Nachbarszellen in x Richtung als Lebendig markieren
                    m_GridManager.Field[(int)Pos.x + a, (int)Pos.y].SetCellState(Cell.STATES.ALIVE);
        }
    }

    /// <summary>CoRoutine die die Zellen auf dem Spielfeld in einem bestimmten Zeitintervall, der als Variable gespeichert ist, updaten lässt</summary>
    /// <remarks>Updatet in einem bestimmten Zeitintervall die Zellen auf dem Spielfeld</remarks>
    IEnumerator StartCellUpdate()
    {
        //Wartet die Zeit die per Variable deklariert wurde
        yield return new WaitForSeconds(m_UpdateSpeed);

        //Durch das Spielfeld iterieren und die Zellen updaten
        for (int x = 0; x < m_GridManager.FieldSize.x; x++)
        {
            for (int y = 0; y < m_GridManager.FieldSize.y; y++)
                m_GridManager.Field[x, y].CellUpdateOld();
        }

        //Die CoRoutine erneut aufrufen damit die Zellen erneut in dem Zeitintervall upgedatet werden
        StartCoroutine(StartCellUpdate());
    }
}
