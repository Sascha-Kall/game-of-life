/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse beinhaltet alle wichtigen Einstellungen für *
 * das Spieles                                                                  *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{

#region Variablen Deklaration

    public enum GAMESTATE { NONE, CREATE, RUN, BREAK, END } //Die verschiedenen Spielzustände die das Spiel haben kann
    public enum GAMEMODE { NONE, GAME, CREATIVE } //Die verschiedenen Spiel Modi die das Spiel haben kann

    GAMESTATE m_CurrentGameState = GAMESTATE.NONE; //Der aktuelle zustand in dem sich das Spiel befindet
    GAMEMODE m_CurrentGameMode = GAMEMODE.NONE; //Der aktuelle Modus in dem sich das Spiel befindet

#endregion

#region Events Deklaration

    //Das Event das aufgerufen wird, wenn sich der Spielzustand ändert
    public delegate void OnChangeGameStateDelegate(GAMESTATE _currentState);
    public static event OnChangeGameStateDelegate OnChangeGameState;

    //Das Event das aufgerufen wird, wenn sich der Spiel Modus ändert
    public delegate void OnChangeGameModeDelegate(GAMEMODE _currentMode);
    public static event OnChangeGameModeDelegate OnChangeGameMode;

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft Instance gibt die aktuelle Instanz des Game Managers zurück.</value>
    public static GameSettings Instance
    {
        get;
        private set;
    }

    /// <value>Die Eigenschaft CurrentGameState gibt den aktuellen Spielzustand zurück.</value>
    public GAMESTATE CurrentGameState
    {
        get { return m_CurrentGameState; }
    }

    /// <value>Die Eigenschaft CurrentGameMode gibt den aktuellen Spielmodus zurück.</value>
    public GAMEMODE CurrentGameMode
    {
        get { return m_CurrentGameMode; }
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

        //Verhindern das dass Gameobjekt auf dem das Script liegt bei einem Szenen Wechsel zerstört wird
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Ändert den aktuellen Spielzustand</summary>
    /// <param name="_newState">In welchen Spielzustand soll gewechselt werden</param>
    /// <remarks>Ändert den aktuellen Spielzustand und sorgt dafür das dass zuständige Event für den Wechsel gestartet wird</remarks>
    public void ChangeGameState(GAMESTATE _newState)
    {
        //Überprüfen ob in einen anderen Zustand gewechselt werden soll
        if (_newState != m_CurrentGameState)
        {
            m_CurrentGameState = _newState; //Neuen Zustand setzen

            //Überprüfen ob das Event für den Wechsel abonniert wurde, wenn ja wird das Event gestartet
            if (OnChangeGameState != null)
                OnChangeGameState(_newState);
        }
    }

    /// <summary>Ändert den aktuellen Spielmodus</summary>
    /// <param name="_newMode">In welchen Spielmodus soll gewechselt werden</param>
    /// <remarks>Ändert den aktuellen Spielmodus und sorgt dafür das dass zuständige Event für den Wechsel gestartet wird</remarks>
    public void ChangeGameMode(GAMEMODE _newMode)
    {
        //Überprüfen ob in einen anderen Modus gewechselt werden soll
        if (_newMode != m_CurrentGameMode)
        {
            m_CurrentGameMode = _newMode; //Neuen Modus setzen

            //Überprüfen ob das Event für den Wechsel abonniert wurde, wenn ja wird das Event gestartet
            if (OnChangeGameMode != null)
                OnChangeGameMode(_newMode);
        }
    }
}
