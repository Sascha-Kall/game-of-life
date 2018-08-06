/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt das Verhalten einer einzelnen Zelle. *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

#region Variablen Deklaration

    public enum STATES { NONE, ALIVE, DEAD } //Die verschiedenen möglichen Zustände einer Zelle --> NONE = noch nicht definiert 
    public enum AGES { NONE, BABY, TEENAGER, ADULT, GRANDPARENTS } //Die verschiedenen möglichen Altersgruppen einer Zelle --> NONE = noch nicht definiert

    GridManager m_GridManager = null; //Das GridManager Script damit die Zelle das Feld mit Zellen kennt

    //Structs mit den Spezifischen Eigenschaften der verschiedenen Altersgruppen einer Zelle
    AgeSettings m_Baby = new AgeSettings(5, new Color(255, 209, 0)); 
    AgeSettings m_Teenager = new AgeSettings(10, new Color(230, 145, 0));
    AgeSettings m_Adult = new AgeSettings(15, new Color(230, 90, 20));
    AgeSettings m_Grandparents = new AgeSettings(20, new Color(150, 10, 10));

    Color m_DeadColor = Color.white; //Farbe einer toten Zelle

    STATES m_CurrentState = STATES.NONE; //Welche Zustand hat die Zelle gerade
    STATES m_NewState = STATES.NONE; //Welchen Zustand wird die Zelle in der nächsten Generation einnehmen
    AGES m_AgeGroup = AGES.NONE; //Zu welcher Altersgruppe gehört die Zelle gerade

    Vector2 m_CellPos = Vector2.zero; //Welche Position hat die Zelle in dem aktuellen Spielfeld
    SpriteRenderer m_Renderer = null; //Das SpriteRenderer Component der Zelle. Wird benötigt um die Farbe einer Zelle ändern zu können
    ulong m_Age = 0UL; //Das aktuelle Alter der Zelle

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft CellWidth gibt die aktuelle Breite der Zelle in Pixel umgerechnet in Unity Units zurück.</value>
    public float CellWidth
    {
        get 
        {
            //Überprüfen ob das SpriteRenderer Component schon von der Zelle geholt wurde. Wenn nicht wird es geholt. Sollte die Komponente nicht gefunden
            //werden, wird eine Fehlermeldung in der Konsole ausgegeben und 0 zurückgegeben.
            if (!m_Renderer) 
            {
                m_Renderer = GetComponent<SpriteRenderer>();
                if (!m_Renderer)
                {
                    Debug.LogError("Auf dem Gameobjekt: " + name + " wurde keine SpriteRenderer Komponente gefunden! Die Zellen Breite kann nicht zurück gegeben werden.");
                    return 0f;
                }
            }

            return m_Renderer.sprite.rect.width / 100; //Umrechnen der Zellen Pixelgröße in Unity Units. 100 entspricht dabei den Pixel per Unit in Unity
        }
    }

    /// <value>Die Eigenschaft CellHeight gibt die aktuelle Höhe der Zelle in Pixel umgerechnet in Unity Units zurück.</value>
    public float CellHeight
    {
        get 
        {
            //Überprüfen ob das SpriteRenderer Component schon von der Zelle geholt wurde. Wenn nicht wird es geholt. Sollte die Komponente nicht gefunden
            //werden, wird eine Fehlermeldung in der Konsole ausgegeben und 0 zurückgegeben.
            if (!m_Renderer)
            {
                m_Renderer = GetComponent<SpriteRenderer>();
                if (!m_Renderer)
                {
                    Debug.LogError("Auf dem Gameobjekt: " + name + " wurde keine SpriteRenderer Komponente gefunden! Die Zellen Höhe kann nicht zurück gegeben werden.");
                    return 0f;
                }
            }

            return m_Renderer.sprite.rect.height / 100; //Umrechnen der Zellen Pixelgröße in Unity Units. 100 entspricht dabei den Pixel per Unit in Unity
        }
    }

    /// <value>Die Eigenschaft CurrentState gibt den aktuellen Zustand der Zelle zurück.</value>
    public STATES CurrentState
    {
        get { return m_CurrentState; }
    }

    /// <value>Die Eigenschaft GridPos gibt die aktuelle Position in dem Grid aus Zellen zurück.</value>
    public Vector2 GridPos
    {
        get { return m_CellPos; }
    }

#endregion

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
    void Awake()
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        m_Renderer = GetComponent<SpriteRenderer>();
        if (!m_Renderer)
        {
            Debug.LogError("Auf dem Gameobjekt: " + name + " wurde keine SpriteRenderer Komponente gefunden! Der SpriteRenderer konnte nicht gesetzt werden.");
            enabled = false;
        }
    }

    /// <summary>Initialisiert die Zelle</summary>
    /// <param name="_gridManager">Das GridManager script das die Zelle managt</param>
    /// <param name="_cellPos">Die aktuelle Position(x, y) der Zelle in dem Feld aller Zellen</param>
    /// <param name="_babySettings">Struct das alle Eigenschaften der Altersgruppe: Baby beinhaltet</param>
    /// <param name="_teenagerSettings">Struct das alle Eigenschaften der Altersgruppe: Jugendlich beinhaltet</param>
    /// <param name="_adultSettings">Struct das alle Eigenschaften der Altersgruppe: Erwachsen beinhaltet</param>
    /// <param name="_grandparents">Struct das alle Eigenschaften der Altersgruppe: Senior beinhaltet</param>
    /// <param name="_deadColor">Farbe die die Zelle haben soll wenn sie gestorben ist</param>
    /// <remarks>Gibt der Zelle alle wichtigen Eigenschaften die sie beschreibt und Initialisiert diese dann</remarks>
    public void InitCell(GridManager _gridManager, Vector2 _cellPos, AgeSettings _babySettings, AgeSettings _teenagerSettings, AgeSettings _adultSettings, AgeSettings _grandparents, Color _deadColor)
    {
        //Alle Eigenschaften der Zelle in die Variablen speichern
        m_GridManager = _gridManager;
        m_CellPos = _cellPos;

        m_Baby = _babySettings;
        m_Teenager = _teenagerSettings;
        m_Adult = _adultSettings;
        m_Grandparents = _grandparents;
        m_DeadColor = _deadColor;

        //Den Zellen Zustand auf "gestorben" setzen und somit die Zelle als initialisiert kennzeichnen
        SetCellState(STATES.DEAD);
    }

    /// <summary>Updatet den Zellenzustand nach den gängigen Regeln von Game of Life ändert diesen aber noch nicht</summary>
    /// <remarks>Holt sich die Anzahl der aktuell Lebenden Nachbarszellen. Je nach Anzahl wird die Zelle geboren, überlebt oder stirbt</remarks>
    public void CellUpdate()
    {
        //Sich die aktuelle Anzahl der 8 möglich Lebenden Nachbarszellen holen
        int LifingNeighborsCount = LifingNeighbors();

        //Den aktuellen Zustand der Zelle speichern
        m_NewState = m_CurrentState;

        //Nach den gängigen Regeln von Game of Life den neuen Zustand der Zelle speichern
        if (m_CurrentState == STATES.DEAD && LifingNeighborsCount == 3) //Überprüfen der aktuelle Zustand "Gestorben" ist und ob es 3 Lebende Nachbarszellen gibt
            m_NewState = STATES.ALIVE; //Die Zelle wird neugeboren -> Der neue Zustand der Zelle wird "Lebend" sein
        else if (m_CurrentState == STATES.ALIVE && LifingNeighborsCount < 2) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es weniger als 2 Lebende Nachbarszellen gibt
            m_NewState = STATES.DEAD; //Die Zelle stirbt an Vereinsamung -> Der neue Zustand der Zelle wird  "Gestorben" sein
        else if (m_CurrentState == STATES.ALIVE && (LifingNeighborsCount == 2 || LifingNeighborsCount == 3)) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es genau 2 oder 3 lebende Nachbarszellen gibt
            return; //Der neue Zustand wird gleich dem aktuellen Zustand der Zelle sein
        else if (m_CurrentState == STATES.ALIVE && LifingNeighborsCount > 3) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es mehr wie 3 Lebende Nachbarszellen gibt
            m_NewState = STATES.DEAD; //Die Zelle stirbt an Überbevölkerung -> Der neue Zustand der Zelle wird "Gestorben" sein
    }

    /// <summary>Updatet den Zellenzustand nach selbst festgelegten Regeln ändert diesen aber nicht</summary>
    /// <param name="_rules">Struct mit den selber festgelegten Regeln</param>    
    /// <remarks>Holt sich die Anzahl der aktuell Lebenden Nachbarszellen. Je nach Anzahl wird die Zelle geboren, überlebt oder stirbt</remarks>
    public void CellUpdate(CustomRules _rules)
    {
        //Sich die aktuelle Anzahl der 8 möglich Lebenden Nachbarszellen holen
        int LifingNeighborsCount = LifingNeighbors();

        //Sich die aktuelle Anzahl an Lebenden Nachbarszellen holen die den übergebenen Altersgruppen entsprechen
        int AgeGroupNeighborsCount = AgeGroupNeighbors(_rules.MinAgeReproductions, _rules.MaxAgeReproductions);

        //Den aktuellen Zustand der Zelle speichern
        m_NewState = m_CurrentState;

        //Überprüfen ob das aktuelle Zellenalter älter oder gleich ist als das Alter das in den eigenen Regeln für "Sterben an Altersschwäche" festgelegt wurde
        if (_rules.AgeWeaknessesOfOld != 0 && m_Age >= (ulong)_rules.AgeWeaknessesOfOld)
            m_NewState = STATES.DEAD; //Die Zelle stirbt an Altersschwäche -> Der neue Zustand der Zelle wird "Gestorben" sein
        else if (m_CurrentState == STATES.DEAD && AgeGroupNeighborsCount == 3) //Überprüfen ob der aktuelle Zustand "Gestorben" ist und ob es 3 Lebende Nachbarszellen in der festgelegten Altersgruppe gibt
            m_NewState = STATES.ALIVE; //Die Zelle wird neugeboren -> Der neue Zustand der Zelle wird "Lebend" sein
        else if (m_CurrentState == STATES.ALIVE && LifingNeighborsCount < 2) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es weniger als 2 Lebende Nachbarszellen in der festgelegten Altersgruppe gibt
            m_NewState = STATES.DEAD; //Die Zelle stirbt an Vereinsamung -> Der neue Zustand der Zelle wird  "Gestorben" sein
        else if (m_CurrentState == STATES.ALIVE && (LifingNeighborsCount == 2 || LifingNeighborsCount == 3)) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es genau 2 oder 3 lebende Nachbarszellen in der festgelegten Altersgruppe gibt
            return; //Der neue Zustand wird gleich dem aktuellen Zustand der Zelle sein
        else if (m_CurrentState == STATES.ALIVE && LifingNeighborsCount > 3) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es mehr wie 3 Lebende Nachbarszellen in der festgelegten Altersgruppe gibt
            m_NewState = STATES.DEAD; //Die Zelle stirbt an Überbevölkerung -> Der neue Zustand der Zelle wird "Gestorben" sein
    }

    /// <summary>Updatet den Zellenzustand nach selbst festgelegten Regeln ändert diesen aber sofort</summary>
    /// <remarks>Holt sich die Anzahl der aktuell Lebenden Nachbarszellen. Je nach Anzahl wird die Zelle geboren, überlebt oder stirbt.</remarks>
    public void CellUpdateOld()
    {
        //Sich die aktuelle Anzahl der 8 möglich Lebenden Nachbarszellen holen
        int LifingNeighborsCount = LifingNeighbors();

        //Nach den gängigen Regeln von Game of Life den neuen Zustand der Zelle setzen
        if (m_CurrentState == STATES.DEAD && LifingNeighborsCount == 3) //Überprüfen der aktuelle Zustand "Gestorben" ist und ob es 3 Lebende Nachbarszellen gibt
            SetCellState(STATES.ALIVE); //Den neuen Zustand der Zelle als "Lebendig" setzen
        else if (m_CurrentState == STATES.ALIVE && LifingNeighborsCount < 2) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es weniger als 2 Lebende Nachbarszellen gibt
            SetCellState(STATES.DEAD); //Den neuen Zustand der Zelle als "Gestorben" setzen
        else if (m_CurrentState == STATES.ALIVE && (LifingNeighborsCount == 2 || LifingNeighborsCount == 3)) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es genau 2 oder 3 lebende Nachbarszellen gibt
            return; //Den aktuellen Zustand nicht verändern
        else if (m_CurrentState == STATES.ALIVE && LifingNeighborsCount > 3) //Überprüfen der aktuelle Zustand "Lebend" ist und ob es mehr wie 3 Lebende Nachbarszellen gibt
            SetCellState(STATES.DEAD); //Den neuen Zustand der Zelle als "Gestorben" setzen
    }

    /// <summary>Update routine die ausgeführt werden sollte nachdem alle Zellen upgedatet wurden</summary>
    /// <remarks>Updatet die Zelle und setzt den neuen Zustand der bei dem regulären Update gespeichert wurde und erhöht das Zellenalter</remarks>
    public void CellLateUpdate()
    {
        SetCellState(m_NewState); //Setzt den neuen Zellenzustand

        m_CurrentState = m_NewState; //Den neuen Zellenzustand als aktuelle Zustand speichern

        //Wenn die Zelle noch lebt wird ihr alter um eine Generation erhöht
        if(m_CurrentState != STATES.DEAD)
            m_Age++;
    }

    /// <summary>Zählt wie-viele Nachbarn um die Zelle aktuell Leben</summary>
    /// <returns>Anzahl der Lebenden Zellen um die Zelle</returns>
    /// <remarks>Zählt die Nachbarszellen die eine Reihe um die eigene Zelle deren aktueller Zellenzustand "Lebendig" ist</remarks>
    int LifingNeighbors()
    {
        int TempCounter = 0;

        //Startposition der Zählung festlegen -> oben Links
        int TempX = (int)m_CellPos.x - 1;
        int TempY = (int)m_CellPos.y - 1;

        //Die Nachbarszellen durchlaufen und überprüfen ob deren aktueller Zustand "Lebendig" ist
        for (int i = 0; i < 3; i++)
        {
            for (int a = 0; a < 3; a++)
            {
                //Überprüfen ob es sich um die eigene Zelle handelt
                if (TempX == (int)m_CellPos.x && TempY == (int)m_CellPos.y)
                {
                    TempY++; //Eine Zelle weiter nach rechts rutschen
                    continue; 
                }

                //Überprüfen ob man am oberen oder unteren Spielfeldrand angekommen ist
                if (TempX < 0 || TempX > m_GridManager.FieldSize.x - 1)
                    break;

                //Überprüfen ob man am linken oder rechten Spielfeldrand angekommen ist. Wenn nicht wird die Nachbarszellen die sich an der TempX und TempY Position des Spielfeld
                //befindet überprüft ob diese den Zellenzustand "Lebendig" trägt.
                if (TempY >= 0 && TempY <= m_GridManager.FieldSize.y - 1)
                    if (m_GridManager.Field[TempX, TempY].CurrentState == STATES.ALIVE)
                        TempCounter++; //Anzahl der Lebenden Nachbarszellen Zähler um eins erhöhen

                TempY++; //Eine Zelle nach unten rutschen
            }

            TempX++; //Eine Zelle weiter nach rechts rutschen
            TempY = (int)m_CellPos.y - 1; //Suchlauf wieder an der linken Zelle beginnen lassen
        }

        return TempCounter;
    }

    /// <summary>Zählt wie-viele Nachbarn um die Zelle aktuell Leben. Diese müssen sich aber in dem übergebenen Altersgruppenumfeld befinden</summary>
    /// <param name="_minAgeGroup">In welcher Altersgruppe müssen sich die Nachtbarzellen mindestens befinden damit sie gezählt werden</param>
    /// <param name="_maxAgeGroup">In welcher Altersgruppe dürfen sich die Nachtbarzellen maximal befinden damit sie gezählt werden</param>
    /// <returns>Anzahl der Lebenden Zellen um die Zelle</returns>
    /// <remarks>Zählt die Nachbarszellen die eine Reihe um die eigene Zelle deren aktueller Zellenzustand "Lebendig" ist. Diese müssen sich aber in dem #
    /// übergebenen Altersgruppenumfeld befinden</remarks>
    int AgeGroupNeighbors(AGES _minAgeGroup, AGES _maxAgeGroup)
    {
        int TempCounter = 0;

        //Startposition der Zählung festlegen -> oben Links
        int TempX = (int)m_CellPos.x - 1;
        int TempY = (int)m_CellPos.y - 1;

        //Die Nachbarszellen durchlaufen und überprüfen ob deren aktueller Zustand "Lebendig" ist
        for (int i = 0; i < 3; i++)
        {
            for (int a = 0; a < 3; a++)
            {
                //Überprüfen ob es sich um die eigene Zelle handelt
                if (TempX == (int)m_CellPos.x && TempY == (int)m_CellPos.y)
                {
                    TempY++; //Eine Zelle weiter nach rechts rutschen
                    continue;
                }

                //Überprüfen ob man am oberen oder unteren Spielfeldrand angekommen ist
                if (TempX < 0 || TempX > m_GridManager.FieldSize.x - 1)
                    break;

                //Überprüfen ob man am linken oder rechten Spielfeldrand angekommen ist. Wenn nicht wird die Nachbarszellen die sich an der TempX und TempY Position des Spielfeld
                //befindet überprüft ob diese den Zellenzustand "Lebendig" trägt und ob sie sich in dem entsprechenden Altersumfeld befindet.
                if (TempY >= 0 && TempY <= m_GridManager.FieldSize.y - 1)
                    if (m_GridManager.Field[TempX, TempY].CurrentState == STATES.ALIVE && m_GridManager.Field[TempX, TempY].m_AgeGroup >= _minAgeGroup && m_GridManager.Field[TempX, TempY].m_AgeGroup <= _maxAgeGroup)
                        TempCounter++;

                TempY++; //Eine Zelle nach unten rutschen
            }

            TempX++; //Eine Zelle weiter nach rechts rutschen
            TempY = (int)m_CellPos.y - 1; //Suchlauf wieder an der linken Zelle beginnen lassen
        }

        return TempCounter;
    }

    /// <summary>Setzt den übergeben Zellenzustand und passt alles weiter entsprechend an</summary>
    /// <param name="_state">Der Zustand der gesetzt werden soll</param>
    /// <remarks>Setzt den übergeben Zellenzustand und passt dann die Zellenfarbe entsprechend dem Alter und Zustand an. Außerdem wird hier dann auch die aktuelle
    /// Altersgruppe festgelegt</remarks>
    public void SetCellState(STATES _state)
    {
        //Je nach übergeben Zustand die Zellenfarbe sowie die Altersgruppe anpassen
        switch (_state)
        {
            case STATES.NONE:
                m_Renderer.color = new Color(0, 0, 0, 0);
                break;

            case STATES.ALIVE:
                if (m_Age <= (ulong)m_Baby.Age)
                {
                    m_Renderer.color = m_Baby.AgeColor;
                    m_AgeGroup = AGES.BABY;
                }
                else if (m_Age <= (ulong)m_Teenager.Age)
                {
                    m_Renderer.color = m_Teenager.AgeColor;
                    m_AgeGroup = AGES.TEENAGER;
                }
                else if (m_Age <= (ulong)m_Adult.Age)
                {
                    m_Renderer.color = m_Adult.AgeColor;
                    m_AgeGroup = AGES.ADULT;
                }
                else
                {
                    m_Renderer.color = m_Grandparents.AgeColor;
                    m_AgeGroup = AGES.GRANDPARENTS; 
                }

                //Ist der aktuelle Zellenzustand "Gestorben" und wird er jetzt auf "Lebendig" gesetzt wird dem Grid Manager mitgeteilt das es eine weitere Lebende
                //Zelle gibt.
                if (m_CurrentState == STATES.DEAD)
                    m_GridManager.LivingCells++;

                break;

            case STATES.DEAD:
                m_Renderer.color = m_DeadColor;
                m_Age = 0UL;
                m_AgeGroup = AGES.NONE;

                //Ist der aktuelle Zellenzustand "Lebendig" und wird er jetzt auf "Gestorben" gesetzt wird dem Grid Manager mitgeteilt das es eine Lebende weniger
                //gibt.
                if(m_CurrentState == STATES.ALIVE)
                    m_GridManager.LivingCells--;

                break;
        }

        //Den neuen Zellenzustand speichern, damit die Zelle ihren eigenen Zustand kennt
        m_CurrentState = _state;
    }

    /// <summary>Setzt die Zelle in ihren Ursprungszustand zurück</summary>
    /// <remarks>Setzt die Zelle in ihren Ursprungszustand zurück in dem den aktuellen Zellenzustand auf "Gestorben" setzt sowie den zukünftigen auf "NONE"</remarks>
    public void ResetCell()
    {
        m_NewState = STATES.NONE;
        SetCellState(STATES.DEAD);
    }

    /// <summary>Mach die Zelle unsichtbar</summary>
    /// <remarks>Mach die Zelle unsichtbar indem das Zellen Gameobjekt ausgeschaltet wird</remarks>
    public void InVisible()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Mach die Zelle sichtbar</summary>
    /// <remarks>Mach die Zelle sichtbar indem das Zellen Gameobjekt eingeschaltet wird</remarks>
    public void Visible()
    {
        gameObject.SetActive(true);
    }
}
