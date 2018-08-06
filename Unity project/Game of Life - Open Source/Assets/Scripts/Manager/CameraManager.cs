/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Klasse die die Kamerafunktionen beinhaltet, die von    *
 * dem Spiel für den Spielfeldaufbau benötigt werden                            *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

#region Variablen Deklaration

    //Struct das die Weltkoordinaten der Kameraecken beinhaltet
    public struct CameraEdges
    {
        public Vector2 LeftTop;
        public Vector2 LeftBottom;
        public Vector2 RightTop;
        public Vector2 RightBottom;
    }

    Camera m_MainCamera = null; //Variable die die Hauptkamera der Scene Speichert
    CameraEdges m_CameraEdgesWorldPos = new CameraEdges(); //Struct mit den Weltkoordinaten der einzelnen Kameraecken

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft CameraEdgesWorldPos gibt das Struct mit den Weltkoordinaten der Kameraecken zurück.</value>
    public CameraEdges CameraEdgesWorldPos
    {
        get { return m_CameraEdgesWorldPos; }
    }

#endregion

    /// <summary>Unity Methode die einmal beim Laden der Script-Instanz aufgerufen wird. Komponenten von dem Gameobjekt holen und überprüfen ob alle Variablen richtig gesetzt wurden</summary>
    /// <remarks>Alle Komponenten von dem Gameobjekt holen die sich das Script von alleine holt. Außerdem wird hier überprüft ob alle Variablen gesetzt bzw. gültig gesetzt 
    /// wurden.</remarks>
	void Awake () 
    {
        //Sich die benötigten Komponenten holen und überprüfen ob die gefunden wurden. Außerdem werde hier die Variablen auf ihre Gültigkeit überprüft.
        //Sollte eine Komponente nicht gefunden werden bzw. eine Variable ungültig sein, wird das Script beendet und eine Fehlermeldung in der Konsole ausgegeben.
        m_MainCamera = Camera.main;
        if (!m_MainCamera)
        {
            Debug.LogError("In der Scene wurde keine Hauptkamera gefunden! Der CameraManager wird ausgeschaltet.");
            enabled = true;
        }

        //Die Weltkoordinaten der Kameraecken berechnen und in das Struct speichern lassen
        ReadCameraEdges();
	}

    /// <summary>Berechnet die Weltkoordinaten der Kameraecken</summary>
    /// <remarks>Berechnet die Weltkoordinaten der Kameraecken und speichert sie in das Struct</remarks>
    void ReadCameraEdges()
    {
        m_CameraEdgesWorldPos.LeftTop = m_MainCamera.ScreenToWorldPoint(new Vector2(0, Screen.height)); //Weltkoordinaten für Kameraecke Links Oben berechnen
        m_CameraEdgesWorldPos.LeftBottom = m_MainCamera.ScreenToWorldPoint(new Vector2(0, 0)); //Weltkoordinaten für Kameraecke Links Unten berechnen
        m_CameraEdgesWorldPos.RightTop = m_MainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)); //Weltkoordinaten für Kameraecke Rechts Oben berechnen
        m_CameraEdgesWorldPos.RightBottom = m_MainCamera.ScreenToWorldPoint(new Vector2(Screen.width, 0)); //Weltkoordinaten für Kameraecke Links Oben berechnen
    }

    /// <summary>Berechnet die SpawnPosition des Feldes damit dieses Mittig in dem Kamera Feld sitzt</summary>
    /// <param name="_cellSize">Die Größe einer einzelnen Zelle</param>
    /// <param name="_cellCount">Die Anzahl der Zellen auf dem Spielfeld in X und Y Richtung</param>
    /// <returns>Die Spawn Positions des Feldes damit das Komplette Feld Mittig in dem Kamera Feld sitzt</returns>
    /// <remarks>Berechnet anhand der Zellengröße und Anzahl die SpawnPosition des Feldes damit dieses Mittig in dem Kamera Feld sitzt</remarks>
    public Vector2 CalcCellCenterPos(Vector2 _cellSize, Vector2 _cellCount)
    {
        //Den Abstand der Ecken zueinander berechnen lassen
        float XDistance = Vector2.Distance(m_CameraEdgesWorldPos.LeftTop, m_CameraEdgesWorldPos.RightTop);
        float YDistance = Vector2.Distance(m_CameraEdgesWorldPos.LeftTop, m_CameraEdgesWorldPos.LeftBottom);
        
        //Die gesamt Breit und Höhe des kompletten Spielfeldes in Pixel berechnen lassen 
        float CompletteCellWidth = _cellCount.x * _cellSize.x;
        float CompletteCellHeight = _cellCount.y * _cellSize.y;

        //Die Spawnposition ausgehen der oberen Linken Ecke berechnen lassen
        Vector2 SpawnPos = m_CameraEdgesWorldPos.LeftTop;

        //Spawnposition berechnen der Pivotpunkt der Zelle dient dabei nicht als Ausgangspunkt sondern die Position wird so berechnet das der Zellenrand immer am
        //Rand ist
        SpawnPos.x += (_cellSize.x / 2) - ((CompletteCellWidth - XDistance) / 2); 
        SpawnPos.y -= (_cellSize.y / 2) - ((CompletteCellHeight - YDistance) / 2);

        return SpawnPos;
    }

    /// <summary>Berechnet die maximale Anzahl an Zellen die auf den Monitor passen</summary>
    /// <param name="_cellSize">Die Größe einer einzelnen Zelle</param>
    /// <returns>Die Anzahl der Zellen die maximal in X sowie Y Richtung auf den Monitor passen</returns>
    /// <remarks>Berechnet anhand der Zellengröße und der aktuellen Auflösung die Anzahl der Zellen die Maximal auf den Monitor passen</remarks>
    public Vector2 CalcMaxCellCount(Vector2 _cellSize)
    {
        Vector2 MaxCellCount = Vector2.zero;

        //Den Abstand der Ecken zueinander durch die Größe der Zelle teilen. Dabei wird auf den nächst größten Int Wert gerundet. Das Stellt sicher dass immer der 
        //komplette Bildschirm ausgefüllt wird
        MaxCellCount.x = Mathf.Ceil(Vector2.Distance(m_CameraEdgesWorldPos.LeftTop, m_CameraEdgesWorldPos.RightTop) / _cellSize.x);
        MaxCellCount.y = Mathf.Ceil(Vector2.Distance(m_CameraEdgesWorldPos.LeftTop, m_CameraEdgesWorldPos.LeftBottom) / _cellSize.y);

        return MaxCellCount;
    }
}
