/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse managt das Spielfeld und sorgt dafür das    *
 * es beim Spielstart erstellt wird                                             *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

#region Variablen Deklaration

    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    GameObject m_CellPrefab = null; //Das Prefab der Zell aus dem das Spielfeld bestehen soll
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    Vector2 m_FieldSize = Vector2.zero; //Die Größe des Spielfeldes Angabe in Anzahl der Zellen in X und Y Richtung
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    Vector3 m_StartPos = Vector3.zero; //Die Position in der das Spielfeld gespawnt werden soll. Ausgangspunkt ist dabei die erste Zelle Oben Links

    Cell[,] m_Field = new Cell[,] { }; //Das Spielfeld mit den Zellen
    int m_LivingCells = 0; //Zähler mit den aktuell "Lebenden" Zellen

#endregion

#region Event Deklaration

    //Event das aufgerufen wird, wenn bei einer Zelle in dem Spielfeld der Zellenzustand geändert wird
    public delegate void OnChangeCellAsAliveDelegate(int _count);
    public static event OnChangeCellAsAliveDelegate OnChangeCellAsAlive;

    //Event das aufgerufen wird, wenn es bei dem Spielfeld keine "Lebende" Zelle mehr gibt
    public delegate void OnNoLivingCellsDelegate();
    public static event OnNoLivingCellsDelegate OnNoLivingCells;

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft FieldSize gibt die aktuelle Größe des Spielfeldes zurück.</value>
    public Vector2 FieldSize
    {
        get { return m_FieldSize; }
    }

    /// <value>Die Eigenschaft Field gibt das aktuelle Spielfeld zurück.</value>
    public Cell[,] Field
    {
        get { return m_Field; }
    }

    /// <value>Die Eigenschaft LivingCells gibt den Zählerstand der aktuell Lebenden Zellen zurück bzw. setzt ihn.</value>
    public int LivingCells
    {
        get { return m_LivingCells; }
        set 
        {
            //Den übergebenen Zählerstand zwischen 0 und Zellenanzahl X * Zellenanzahl Y auf dem Spielfeld begrenzen
            m_LivingCells = Mathf.Clamp(value, 0, (int)m_FieldSize.x * (int)m_FieldSize.y);

            //Überprüfen ob das Event für die Änderung der Anzahl an "Lebenden" Zellen abonniert wurde, wenn ja wird das Event aufgerufen
            if (OnChangeCellAsAlive != null)
                OnChangeCellAsAlive(m_LivingCells);

            //Überprüfen ob das es noch "Lebende" Zellen auf dem Spielfeld gibt und ob das Event dafür abonniert wurde, wenn ja wird das Event aufgerufen
            if (m_LivingCells == 0 && OnNoLivingCells != null)
                OnNoLivingCells();
        }
    }

#endregion

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
	void Start ()
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        if (!m_CellPrefab)
        {
            Debug.LogError("Es wurde das Zellen-Prefab nicht gesetzt! Das Feld kann nicht aufgebaut werden.");
            enabled = false;
        }

        if (m_FieldSize.x <= 0)
        {
            Debug.LogError("Ungültige X-Achsen Feld Größe. Das Spielfeld muss größer als 0 sein");
            enabled = false;
        }

        if (m_FieldSize.y <= 0)
        {
            Debug.LogError("Ungültige Y-Achsen Feld Größe. Das Spielfeld muss größer als 0 sein");
            enabled = false;
        }
	}

    /// <summary>Lässt das Spielfeld Spawnen</summary>
    /// <param name="_startPos">Start der Spawnposition. Ausgangspunkt ist die erste Zelle in der oberen linke Reihe</param>
    /// <param name="_cellCount">Die Größe des Spielfeldes. Wie viele Zellen soll das Feld in X und Y Richtung haben</param>
    /// <param name="_babySettings">Struct das alle Eigenschaften der Altersgruppe: Baby beinhaltet</param>
    /// <param name="_teenagerSettings">Struct das alle Eigenschaften der Altersgruppe: Jugendlich beinhaltet</param>
    /// <param name="_adultSettings">Struct das alle Eigenschaften der Altersgruppe: Erwachsen beinhaltet</param>
    /// <param name="_grandparents">Struct das alle Eigenschaften der Altersgruppe: Senior beinhaltet</param>
    /// <param name="_deadColor">Farbe die die Zelle haben soll wenn sie gestorben ist</param>
    /// <remarks>Startet das Spawnen des Spielfeldes, die Größe sowie die Position des Spielfeldes werden dabei per Parameter übergeben</remarks>
    public void CreateField(Vector3 _startPos, Vector2 _cellCount, AgeSettings _babySettings, AgeSettings _teenagerSettings, AgeSettings _adultSettings, AgeSettings _grandpartensSettings, Color _deadColor)
    {
        //Werte für die StartPosition sowie Feldgröße zwischenspeichern
        m_StartPos = _startPos;
        m_FieldSize = _cellCount;

        //Das Spielfeld Spawnen Starten
        CreateField(_babySettings, _teenagerSettings, _adultSettings, _grandpartensSettings, _deadColor);
    }

    /// <summary>Lässt das Spielfeld Spawnen</summary>
    /// <param name="_babySettings">Struct das alle Eigenschaften der Altersgruppe: Baby beinhaltet</param>
    /// <param name="_teenagerSettings">Struct das alle Eigenschaften der Altersgruppe: Jugendlich beinhaltet</param>
    /// <param name="_adultSettings">Struct das alle Eigenschaften der Altersgruppe: Erwachsen beinhaltet</param>
    /// <param name="_grandparents">Struct das alle Eigenschaften der Altersgruppe: Senior beinhaltet</param>
    /// <param name="_deadColor">Farbe die die Zelle haben soll wenn sie gestorben ist</param>
    /// <remarks>Startet das Spawnen des Spielfeldes, die Größe sowie die Position des Spielfeldes werden dabei im Unity Inspector bei dem Script eingestellt</remarks>
    public void CreateField(AgeSettings _babySettings, AgeSettings _teenagerSettings, AgeSettings _adultSettings, AgeSettings _grandpartensSettings, Color _deadColor)
    {
        //Werte für die StartPosition sowie Feldgröße zwischenspeichern
        Vector3 TempSpawnPos = m_StartPos;
        m_Field = new Cell[(int)m_FieldSize.x, (int)m_FieldSize.y];
     
        //Das Spawnen des Spielfeldes starten angefangen in der linken oberen Ecke
        for (int x = 0; x < m_FieldSize.x; x++)
        {
            //Alle Zellen in Y Richtung Spawnen lassen
            for (int y = 0; y < m_FieldSize.y; y++)
            {
                //Zelle Spawnen lassen
                GameObject TempCell = Instantiate(m_CellPrefab, TempSpawnPos, Quaternion.identity) as GameObject;
                TempCell.name = string.Format("Cell x:{0}, y:{1}", x, y);
                
                //Sich das Zellen Script von der gespawnten Zelle holen und die Zelle dann gleich Initialisieren und somit Spiel bereit machen
                Cell TempCellScript = TempCell.GetComponent<Cell>();
                if (!TempCellScript)
                {
                    Debug.LogError("Die Zelle mit dem Namen: " + TempCell.name + " hat kein Zellen Script! Das erstellen des Grids wird beendet.");
                    enabled = false;
                }

                TempCellScript.InitCell(this, new Vector2(x, y), _babySettings, _teenagerSettings, _adultSettings, _grandpartensSettings, _deadColor);
                m_Field[x, y] = TempCellScript; //Die Zelle dem Spielfeld zuweisen

                TempSpawnPos.y += (m_Field[0, y].CellHeight) * -1; //Die Position der nächsten Zelle in der Reihe zuweisen. Unter der aktuell gespawnten Zelle
            }

            TempSpawnPos.x += (m_Field[x, 0].CellWidth); //Eine Zelle nach rechts rutschen damit dort als nächstes allen Zellen in Y Richtung gespawnt werden
            TempSpawnPos.y = m_StartPos.y; //Start Richtung für das Spawnen wieder zurücksetzen damit wieder oben begonnen wird mit dem Spawnen
        }
    }

    /// <summary>Resetet alle Zellen des Spielfeldes</summary>
    /// <remarks>Iteriert durch das Spielfeld und Resetet jede Zelle</remarks>
    public void ClearFeld()
    {
        //Durch das Spielfeld iterieren und dabei jede Zelle Reseten lassen
        for (int x = 0; x < m_Field.GetLength(0); x++)
        {
            for (int y = 0; y < m_Field.GetLength(1); y++)
            {
                Field[x, y].ResetCell(); //Zelle Reseten die sich an der X, Y Position des Spielfeldes befindet
            }
        }
    }

    /// <summary>Holt sich die Größe der Zelle von dem Zellen Prefab</summary>
    /// <returns>Die Größe einer Zelle in Pixel</returns>
    /// <remarks>Holt sich die Größe der Zelle von dem Zellen Prefab aus dem das Spielfeld besteht</remarks>
    public Vector2 GetCellSize()
    {
        Vector2 CellSize = Vector2.zero;

        //Das Zellen Script von dem Zellen Prefab holen
        Cell TempCell = m_CellPrefab.GetComponent<Cell>();
        if (!TempCell)
        {
            Debug.LogError("Auf dem Zellen Prefab wurde kein Zellen Script gefunden! Die Zellen Größe konnte nicht ermittelt werden.");
            return CellSize;
        }

        //Die Zellen Größe in Pixel von dem Zellen Script holen
        CellSize.x = TempCell.CellWidth;
        CellSize.y = TempCell.CellHeight;

        return CellSize;
    }

    /// <summary>Sich die Position der mittigen Zelle holen</summary>
    /// <returns>Die Position der mittigen Zelle</returns>
    /// <remarks>Sucht die mittige Zelle in dem Spielfeld und gibt die Position in dem Spielfeld zurück</remarks>
    public Vector2 GetCenterPoint()
    {
        return new Vector2((int)(m_Field.GetLength(0) / 2), (int)(m_Field.GetLength(1) / 2));
    }

    /// <summary>Schaltet das Spielfeld auf unsichtbar</summary>
    /// <remarks>Schaltet alle Zellen des Spielfeldes auf unsichtbar. Außerdem wird ein Spielfeldrahmen in der aktuellen Scene gesucht und
    /// gegebenenfalls auch auf unsichtbar geschallten</remarks>
    public void InvisibleField()
    {
        //Sucht in der Scene das Script "Fieldborder". Sollte es das Script geben wird dieser auch auf unsichtbar gestellt
        FieldBorder TempFieldBorderScript = (FieldBorder)FindObjectOfType(typeof(FieldBorder));
        if (!TempFieldBorderScript)
            Debug.LogWarning("Es wurde kein FieldBorder Script in der aktuellen Scene gefunden! Der Rahmen kann nicht ausgeschaltet werden.");
        else
            TempFieldBorderScript.InvisibleBorder();

        //Sucht in der Scene das Script "FieldBorderSideMenu". Sollte es das Script geben wird dieser auch auf unsichtbar gestellt
        FieldBorderSideMenu TempFieldBorderSideMenuScript = (FieldBorderSideMenu)FindObjectOfType(typeof(FieldBorderSideMenu));
        if (!TempFieldBorderSideMenuScript)
            Debug.LogWarning("Es wurde kein FieldBorderSideMenu Script in der aktuellen Scene gefunden! Das Seiten Menü kann nicht ausgeschaltet werden.");
        else
            TempFieldBorderSideMenuScript.InvisibleSideMenu();

        //Durch das Spielfeld iterieren und alle Zellen darin auf unsichtbar stellen
        foreach (Cell _cell in Field)
            _cell.InVisible();
    }

    /// <summary>Schaltet das Spielfeld auf sichtbar</summary>
    /// <remarks>Schaltet alle Zellen des Spielfeldes auf sichtbar. Außerdem wird ein Spielfeldrahmen in der aktuellen Scene gesucht und
    /// gegebenenfalls auch auf sichtbar geschallten</remarks>
    public void VisibleField()
    {
        //Sucht in der Scene das Script "Fieldborder". Sollte es das Script geben wird dieser auch auf sichtbar gestellt
        FieldBorder TempFieldBorderScript = (FieldBorder)FindObjectOfType(typeof(FieldBorder));
        if (!TempFieldBorderScript)
            Debug.LogWarning("Es wurde kein FieldBorder Script in der aktuellen Scene gefunden! Der Rahmen kann nicht eingeschaltet werden.");
        else
            TempFieldBorderScript.VisibleBorder();

        //Sucht in der Scene das Script "FieldBorderSideMenu". Sollte es das Script geben wird dieser auch auf unsichtbar gestellt
        FieldBorderSideMenu TempFieldBorderSideMenuScript = (FieldBorderSideMenu)FindObjectOfType(typeof(FieldBorderSideMenu));
        if (!TempFieldBorderSideMenuScript)
            Debug.LogWarning("Es wurde kein FieldBorderSideMenu Script in der aktuellen Scene gefunden! Das Seiten Menü kann nicht ausgeschaltet werden.");
        else
            TempFieldBorderSideMenuScript.VisibleSideMenu();

        //Durch das Spielfeld iterieren und alle Zellen darin auf sichtbar stellen
        foreach (Cell _cell in Field)
            _cell.Visible();
    }
}
