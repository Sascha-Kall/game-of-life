/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt das markieren der Zellen als Lebendig*
 * durch den Benutzer. Außerdem kümmert es sich um die verschiedenen            *
 * Eingabe Möglichkeiten der verschieden Plattformen die von dem Game of Life   *
 * unterstützt werden                                                           *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{

#region Variablen Deklaration

    public enum CREATESTATE { NONE, CREATE, ERASE } //Enum das den aktuellen Zustand anzeigt, was gerade gemacht wird
    enum MOUSEBUTTONS { LEFT, RIGHT, MIDDLE } //Enum mit den verschiedenen Maus Buttons

    int m_CellCounter = 0; //Zähler der die Markierten Zellen zählt
    Cell m_FirstFrameSelectedCell = null; //Die Zelle die im ersten Frame ausgewählt wurde
    Cell m_SecondFrameSelectedCell = null; //Die Zelle die im nächsten Frame ausgewählt wurde
    CREATESTATE m_CreateState = CREATESTATE.NONE; //Der aktuelle Zustand was gerade gemacht wird

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft CurrentCreateState setzt oder gibt den aktuellen Zustand des CreateManagers zurück.</value>
    public CREATESTATE CurrentCreateState
    {
        get { return m_CreateState; }
        set { ChangeCreateState(value); }
    }

#endregion

#region Events Deklaration

    //Event das aufgerufen wird, wenn sich die Anzahl der Markierten Zellen ändert
    public delegate void OnChangeCellAsAliveDelegate(int _count);
    public static event OnChangeCellAsAliveDelegate OnChangeCellAsAlive;

    //Event das aufgerufen wird, wenn sich der aktuelle Zustand des CreateManagers geändert hat
    public delegate void OnChangeCreateStateDelegate(CREATESTATE _newState);
    public static event OnChangeCreateStateDelegate OnChangeCreateState;

#endregion

    //Unity Methode die beim Instanziieren des Scripts aufgerufen, wenn dieses angeschaltet wird
    void Start()
    {
        //Den Create Zustand beim Start auf Zeichnen setzen lassen.
        ChangeCreateState(CREATESTATE.CREATE);
    }

    //Unity Methode die aufgerufen wird, wenn das Script aktiviert wird
    void OnEnable()
    {
        //Das Event abonnieren, das aufgerufen wird, wenn das Spielfeld komplett gelöscht wird
        GameManager.OnClearGame += OnClearGame;
    }

    //Unity Methode die aufgerufen wird, wenn das Script deaktiviert wird
    void OnDisable()
    {
        // Event deabonnieren, das aufgerufen wird, wenn das Spielfeld komplett gelöscht wird
        GameManager.OnClearGame -= OnClearGame;
    }

	//Unity Methode die bei jedem Frame einmal aufgerufen wird
	void Update ()
    {
        //Überprüfen ob aktuell Zellen als "Lebendig" markiert werden dürfen.
        if (GameSettings.Instance.CurrentGameState == GameSettings.GAMESTATE.CREATE || (GameSettings.Instance.CurrentGameState == GameSettings.GAMESTATE.BREAK && GameSettings.Instance.CurrentGameMode != GameSettings.GAMEMODE.GAME))
        {
            //Überprüfen ob es sich aktuell um die Eingabe per Touch oder Maus handelt
            #if (UNITY_ANDROID && !UNITY_EDITOR)
                //Überprüfen ob das Display berührt wird
                if(Input.touchCount > 0)
                {
                     //Durch die verschiedenen Berührungspunkte auf dem Bildschirm iterieren
                     for (int i = 0; i < Input.touchCount; ++i)
                     {
                         //Überprüfen ob sich die Berührungspunkte bewegen
                         if (Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Moved)
                         {
                            //Überprüfen ob eine Zelle selektiert wurde
                            Cell TempCell = GetSelectedCellFromTouch(Input.GetTouch(i).position);
                            if (TempCell)
                            {
                                if(m_CreateState == CREATESTATE.ERASE) //Überprüfen ob es sich um den Löschen Modus handelt
                                {
                                        //Die Routine starten die die Zelle ausgewählte Zelle in den entsprechenden Zustand versetzt
                                        SwitchCellStateRoutine(TempCell, Cell.STATES.DEAD);
                         
                                        //Den Create Zustand auf zeichnen wechseln
                                        ChangeCreateState(CREATESTATE.ERASE);
                                }
                                //Überprüfen um welchen Create Modus es sich aktuell handelt
                                else
                                {
                                    //Die Routine starten die die Zelle ausgewählte Zelle in den entsprechenden Zustand versetzt
                                    SwitchCellStateRoutine(TempCell, Cell.STATES.ALIVE);

                                   //Den Create Zustand auf zeichnen wechseln
                                    ChangeCreateState(CREATESTATE.CREATE);
                                }
                            }
                         }
                         else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                         {
                             //Überprüfen ob noch Zellen gespeichert sind, wenn ja werden diese dann gelöscht
                             if (m_FirstFrameSelectedCell != null || m_SecondFrameSelectedCell != null)
                             {
                                 m_FirstFrameSelectedCell = null;
                                 m_SecondFrameSelectedCell = null;
                             }
                         }
                     }
                }
            #else
            if (Input.GetMouseButton((int)MOUSEBUTTONS.LEFT)) //Überprüfen ob die Linke Maustaste zum markieren gedrückt wurde
            {
                //Überprüfen ob eine Zelle selektiert wurde
                Cell TempCell = GetSelectedCellFromMouse();
                if (TempCell)
                {
                    if(m_CreateState == CREATESTATE.ERASE) //Überprüfen ob es sich um den Löschen Modus handelt
                    {
                        //Die Routine starten die die Zelle ausgewählte Zelle in den entsprechenden Zustand versetzt
                        SwitchCellStateRoutine(TempCell, Cell.STATES.DEAD);
                         
                        //Den Create Zustand auf zeichnen wechseln
                        ChangeCreateState(CREATESTATE.ERASE);
                    }
                     //Überprüfen um welchen Create Modus es sich aktuell handelt
                    else
                    {
                        //Die Routine starten die die Zelle ausgewählte Zelle in den entsprechenden Zustand versetzt
                        SwitchCellStateRoutine(TempCell, Cell.STATES.ALIVE);

                        //Den Create Zustand auf zeichnen wechseln
                        ChangeCreateState(CREATESTATE.CREATE);
                    }
                }
                 
            }
            else if (Input.GetMouseButton((int)MOUSEBUTTONS.RIGHT)) //Überprüfen ob die Rechte Maustaste zum löschen der Markierung gedrückt wurde
            {
                //Überprüfen ob eine Zelle ausgewählt wurde
                Cell TempCell = GetSelectedCellFromMouse();
                if (TempCell)
                {
                    //Die Routine starten die die Zelle ausgewählte Zelle in den entsprechenden Zustand versetzt
                    SwitchCellStateRoutine(TempCell, Cell.STATES.DEAD);

                    //Den Create Zustand auf löschen wechseln
                    ChangeCreateState(CREATESTATE.ERASE);
                }
                
            }
            else if (Input.GetMouseButtonUp((int)MOUSEBUTTONS.LEFT) || Input.GetMouseButtonUp((int)MOUSEBUTTONS.RIGHT)) //Überprüfen ob die Maustasten losgelassen wurden
            {
                //Überprüfen ob noch Zellen gespeichert sind, wenn ja werden diese dann gelöscht
                if (m_FirstFrameSelectedCell != null || m_SecondFrameSelectedCell != null)
                {
                    m_FirstFrameSelectedCell = null;
                    m_SecondFrameSelectedCell = null;

                    //Den Create Zustand auf NONE wechseln
                    ChangeCreateState(CREATESTATE.NONE);
                }
            }
            #endif
        }
	}

    /// <summary>Methode die beim Event aufgerufen wird, wenn das Spielfeld gelöscht wurde</summary>
    /// <remarks>Der Zähler für die als "Lebendig" markierten Zellen wird zurückgesetzt</remarks>
    void OnClearGame()
    {
        m_CellCounter = 0;
    }

    /// <summary>Methode die die per Maus ausgewählte Zelle zurückgibt</summary>
    /// <returns>Die Zelle die ausgewählt wurde</returns>
    /// <remarks>Es wird ein Strahl von der Kamera zu den Weltkoordinaten des Mauszeigers gesendet. Wenn dieser Strahl ein Objekt trifft, wird überprüft ob das Objekt eine Zelle ist.
    /// Wenn ja wird diese zurückgegeben</remarks>
    Cell GetSelectedCellFromMouse()
    {
        Cell TempCell = null;

         //Einen Strahl ausgehend von der Mitte der Kamera in Richtung der Weltkoordinate der Mausposition schicken 
         Vector2 TempRayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
         RaycastHit2D TempHit = Physics2D.Raycast(TempRayPos, Vector2.zero, 0f);

         //Überprüfen ob der Strahl etwas getroffen hat.
         if (TempHit.collider != null)
             //Überprüfen ob es sich bei dem getroffenen Objektes um eine Zelle handelt.
             TempCell = TempHit.transform.GetComponent<Cell>();

        return TempCell;
    }

    /// <summary>Methode die die per Touch ausgewählte Zelle zurückgibt</summary>
    /// <param name="_touchPos">Die Touch Position auf dem Display</param>
    /// <returns>Die Zelle die ausgewählt wurde</returns>
    /// <remarks>Es wird ein Strahl von der Kamera zu den Weltkoordinaten des Touchpunktes gesendet. Wenn dieser Strahl ein Objekt trifft, wird überprüft ob das Objekt eine Zelle ist.
    /// Wenn ja wird diese zurückgegeben</remarks>
    Cell GetSelectedCellFromTouch(Vector2 _touchPos)
    {
        Cell TempCell = null;

        //Einen Strahl ausgehend von der Mitte der Kamera in Richtung der Weltkoordinate des Berührungspunktes schicken 
        Vector2 TempRayPos = new Vector2(Camera.main.ScreenToWorldPoint(_touchPos).x, Camera.main.ScreenToWorldPoint(_touchPos).y);
        RaycastHit2D TempHit = Physics2D.Raycast(TempRayPos, Vector2.zero, 0f);

        //Überprüfen ob der Strahl etwas getroffen hat.
        if (TempHit.collider != null)
            //Überprüfen ob es sich bei dem getroffenen Objektes um eine Zelle handelt.
            TempCell = TempHit.transform.GetComponent<Cell>();

        return TempCell;
    }

    /// <summary>Die Routine die eine ausgewählte Zelle in den entsprechenden Zustand versetzt</summary>
    /// <param name="_cell">Die Zelle deren Zustand geändert werden soll</param>
    /// <param name="_newState">Der Zustand in den die Zelle geändert werden soll</param>
    /// <remarks>Die Methode überprüft ob in einer Routine mehr wie eine Zelle gespeichert ist. Ist dies der Fall, wird automatisch eine Linie mit ausgewählten Zellen zwischen den gespeicherten Zellen
    /// generiert und in den entsprechenden Zustand versetzt</remarks>
    void SwitchCellStateRoutine(Cell _cell, Cell.STATES _newState)
    {
        //Die ausgewählte Zelle in dem aktuellen Frame Speichern
        if (m_FirstFrameSelectedCell == null)
        {
            m_FirstFrameSelectedCell = _cell; //Anfangszelle zwischen speichern
            SetCellStateTo(_cell, _newState); //Den State der Zelle ändern
        }
        else if (m_FirstFrameSelectedCell != null && _cell != m_FirstFrameSelectedCell)
        {
            m_SecondFrameSelectedCell = _cell; //Endzelle zwischen speichern

            Vector2 TempStartPos = m_FirstFrameSelectedCell.GridPos; //Die Startzelle festlegen

            //Festlegen das die Startzellen X Position immer kleiner ist als die X Position der Endzelle. Damit wird sichergestellt das immer mit der Linken ausgewählten Zelle begonnen wird
            if (m_SecondFrameSelectedCell.GridPos.x < m_FirstFrameSelectedCell.GridPos.x)
                TempStartPos.x = (int)m_SecondFrameSelectedCell.GridPos.x;

            //Festlegen das die Startzellen Y Position immer kleiner ist als die Y Position der Endzelle. Damit wird sichergestellt das immer mit der untersten ausgewählten Zelle begonnen wird
            if (m_SecondFrameSelectedCell.GridPos.y < m_FirstFrameSelectedCell.GridPos.y)
                TempStartPos.y = (int)m_SecondFrameSelectedCell.GridPos.y;

            //Die zu überprüfende Spielfeld Größe festlegen. Es wird immer nur das Rechteck geprüft in dem die beiden Zellen die Ecken Oben/Rechts und unten Links bilden
            int TempFieldWitdh = (int)Mathf.Abs(m_FirstFrameSelectedCell.GridPos.x - m_SecondFrameSelectedCell.GridPos.x) + 1;
            int TempFieldHeight = (int)Mathf.Abs(m_FirstFrameSelectedCell.GridPos.y - m_SecondFrameSelectedCell.GridPos.y) + 1;

            //Durch das Spielfeld iterieren
            for (int y = 0; y < TempFieldHeight; y++)
            {
                for (int x = 0; x < TempFieldWitdh; x++)
                {
                    //Den Abstand der Zelle zu dem Vector durch die Start und Endzelle berechnen. Wenn diese unter einem bestimmten Wert ist, befindet sich die geprüfte Zelle auf der Linie durch die Start und Endzelle. Wenn dies der Fall ist, 
                    //wird der Zustand der Zelle geändert  
                    #if (UNITY_ANDROID && !UNITY_EDITOR)
                    if (m_CreateState == CREATESTATE.CREATE)
                    {
                        if (DistanceToLine(m_FirstFrameSelectedCell.transform.position, m_SecondFrameSelectedCell.transform.position, GameManager.Instance.GetGridManager.Field[(int)TempStartPos.x + x, (int)TempStartPos.y + y].transform.position) < .35f)
                        {
                            //Zellenzustand ändern
                            SetCellStateTo(GameManager.Instance.GetGridManager.Field[(int)TempStartPos.x + x, (int)TempStartPos.y + y], _newState);
                        }
                    }
                    else
                    {
                        if (DistanceToLine(m_FirstFrameSelectedCell.transform.position, m_SecondFrameSelectedCell.transform.position, GameManager.Instance.GetGridManager.Field[(int)TempStartPos.x + x, (int)TempStartPos.y + y].transform.position) < 10f)
                        {
                            //Zellenzustand ändern
                            SetCellStateTo(GameManager.Instance.GetGridManager.Field[(int)TempStartPos.x + x, (int)TempStartPos.y + y], _newState);
                        }
                    }
                    #else
                    if (DistanceToLine(m_FirstFrameSelectedCell.transform.position, m_SecondFrameSelectedCell.transform.position, GameManager.Instance.GetGridManager.Field[(int)TempStartPos.x + x, (int)TempStartPos.y + y].transform.position) < .35f)
                        {
                            //Zellenzustand ändern
                            SetCellStateTo(GameManager.Instance.GetGridManager.Field[(int)TempStartPos.x + x, (int)TempStartPos.y + y], _newState);
                        }
                    #endif
                }
            }

            //Gespeicherte Zellen wieder löschen
            m_FirstFrameSelectedCell = m_SecondFrameSelectedCell;
            m_SecondFrameSelectedCell = null;
        }
    }

    /// <summary>Versetzt die übergebene Zelle in den übergebenen Zustand</summary>
    /// <param name="_cell">Die Zelle deren Zustand geändert werden soll</param>
    /// <param name="_newState">Der Zustand in den die Zelle geändert werden soll</param>
    /// <remarks>Die Methode versetzt die übergebene Zelle in den übergebenen Zustand. Hier werden dann auch alle relevanten Events ausgelöst und der Zellen Counter erhöht oder verringert</remarks>
    void SetCellStateTo(Cell _cell, Cell.STATES _newState)
    {
        switch (_newState)
        {
            case Cell.STATES.ALIVE:
            {
                //Überprüfen ob die Zelle schon als "Lebendig" markiert wurde. Wenn nicht wird diese als "Lebendig" markiert und der Zähler der "Lebendigen" Zellen wird um eins erhöht
                if (_cell.CurrentState != Cell.STATES.ALIVE)
                {
                    m_CellCounter++;

                    //Überprüfen ob das Event das sich die Anzahl der Lebendigen Zellen geändert hat abonniert wurde, wenn ja wird das Event aufgerufen
                    if (OnChangeCellAsAlive != null)
                        OnChangeCellAsAlive(m_CellCounter);

                    _cell.SetCellState(Cell.STATES.ALIVE);
                }
            } break;

            case Cell.STATES.DEAD:
            {
                //Überprüfen ob die Zelle schon als "Gestorben" markiert wurde. Wenn nicht wird diese als "Gestorben" markiert und der Zähler der "Lebendigen" Zellen wird um eins verringert
                if (_cell.CurrentState != Cell.STATES.DEAD)
                {
                    m_CellCounter--;

                    //Überprüfen ob das Event das sich die Anzahl der Lebendigen Zellen geändert hat abonniert wurde, wenn ja wird das Event aufgerufen
                    if (OnChangeCellAsAlive != null)
                        OnChangeCellAsAlive(m_CellCounter);

                    _cell.SetCellState(Cell.STATES.DEAD);
                }
            } break;
        }
    }

    /// <summary>Berechnet den Abstand einer Position zu einer Linie</summary>
    /// <param name="_lineStart">Start Position der Linie</param>
    /// <param name="_lineEnd">End Position der Linie</param>
    /// <param name="_checkPos">Die Position von der aus der Abstand gemessen werden soll</param>
    /// <remarks>Berechnet den Abstand einer Position zu einer Linie</remarks>
    float DistanceToLine(Vector3 _lineStart, Vector3 _lineEnd, Vector3 _checkPos)
    {
        //Die Richtung zwischen Start und Ende berechnen
        Vector3 TempDirection = _lineEnd - _lineStart;

        //Die Länge des Richtungsvektors bestimmen
        float TempDirectionLength = TempDirection.sqrMagnitude;

        //Die Richtung des Startpunktes und des zu prüfenden Punktes berechnen
        Vector3 TempDirectionFromStart = _checkPos - _lineStart;

        //Punktprodukt berechnen um herauszufinden welcher Punkt am nächsten ist
        float TempLineProgress = Vector3.Dot(TempDirection, TempDirectionFromStart) / TempDirectionLength;

        //Hier wird die Position gespeichert die am nächsten an dem zu prüfenden Position liegt
        Vector3 TempClosest = Vector3.zero;

        //Überprüfen welche Position an nächsten an dem zu prüfenden Punkt liegt
        if (TempLineProgress < 0f)
            TempClosest = _lineStart;
        else if (TempLineProgress > 1f)
            TempClosest = _lineEnd;
        else
            TempClosest = _lineStart + TempDirection * TempLineProgress;

        //Die Länge des Vectors berechnen der zwischen dem Punkt der am nächsten zum geprüften Punkt liegt
        float TempDistanceToLine = (TempClosest - _checkPos).magnitude;

        return TempDistanceToLine;
    }

    /// <summary>Ändert den aktuellen Zustand des Create Managers</summary>
    /// <param name="_newState">Der neue Zustand in den der Create Manager gewechselt werden soll</param>
    /// <remarks>Ändert den aktuellen Zustand des Create Managers und löst dabei dann das entsprechende Event aus.</remarks>
    void ChangeCreateState(CREATESTATE _newState)
    {
        //Überprüfen ob der aktuelle Zustand geändert wird
        if (_newState != m_CreateState)
        {
            //Den neuen Zustand setzen
            m_CreateState = _newState;

            //Überprüfen ob das Event das angibt das sich der aktuelle Zustand geändert hat abonniert wurde, wenn ja wird das Event aufgerufen
            if (OnChangeCreateState != null)
                OnChangeCreateState(m_CreateState);
        }
    }
}
