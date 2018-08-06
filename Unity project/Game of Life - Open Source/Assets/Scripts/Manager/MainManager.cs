/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse beinhaltet alle wichtigen Parameter für die *
 * die Manager in dem Spiel                                                     *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AgeSettings //Struct das alle Einstellungen beinhaltet welche Zellen Farbe ab welchem Alter angezeigt werden soll
{
    public int Age; //Alter ab dem die Farbe geändert werden soll
    public Color32 AgeColor; //Die Farbe in die die Zelle geändert werden soll

    //Ctor
    public AgeSettings(int _age, Color _color)
    {
        Age = _age;
        AgeColor = _color;
    }
}

[System.Serializable]
public struct CustomRules //Struct das alle Einstellungen für Benutzerdefinierte Regeln benötigt
{
    public bool UsedCustomRules; //Sollen Benutzerdefinierte Regeln verwendet werden
    public int AgeWeaknessesOfOld; //Ab welchem Alter sterben Zellen an Altersschwäche
    public Cell.AGES MinAgeReproductions; //Welche Mindestaltersgruppe müssen Zellen haben um sich Fortzupflanzen
    public Cell.AGES MaxAgeReproductions; //Welche Maximalaltersgruppe müssen die Zellen haben um sich Fortzupflanzen

    //Ctor
    public CustomRules(bool _usedCustomRules, int _ageWeaknessesOfOld, Cell.AGES _minAgeReproductions, Cell.AGES _maxAgeReproductions)
    {
        UsedCustomRules = _usedCustomRules;
        AgeWeaknessesOfOld = _ageWeaknessesOfOld;
        MinAgeReproductions = _minAgeReproductions;
        MaxAgeReproductions = _maxAgeReproductions;
    }
}

//Klasse von der alle wichtigen Manager ableiten
public abstract class MainManager : MonoBehaviour
{

#region Variablen deklaration

    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    AgeSettings m_Baby = new AgeSettings(5, new Color(255, 209, 0)); //Struct mit den festgelegten Einstellungen für die Altersgruppe: Baby
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    AgeSettings m_Teenager = new AgeSettings(10, new Color(230, 145, 0)); //Struct mit den festgelegten Einstellungen für die Altersgruppe: Jugendlich
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    AgeSettings m_Adult = new AgeSettings(15, new Color(230, 90, 20)); //Struct mit den festgelegten Einstellungen für die Altersgruppe: Erwachsen
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    AgeSettings m_Grandparents = new AgeSettings(20, new Color(150, 10, 10)); //Struct mit den festgelegten Einstellungen für die Altersgruppe: Senior
    [SerializeField] //Lässt private Variablen im Inspector von Unity erscheinen
    Color m_DeadColor = Color.white; //Die Farbe die die Zelle haben soll wenn sie als "Gestorben" markiert ist

    protected CustomRules m_CustomRules = new CustomRules(false, 0, Cell.AGES.BABY, Cell.AGES.GRANDPARENTS); //Die Benutzerdefinierten Regeln die festgelegt wurden

#endregion

#region Getter / Setter

    /// <value>Die Eigenschaft BabySettings gibt die Einstellungen für die Altersgruppe Baby zurück.</value>
    public AgeSettings BabySettings
    {
        get { return m_Baby; }
    }

    /// <value>Die Eigenschaft TeenagerSettings gibt die Einstellungen für die Altersgruppe Jugendlich zurück.</value>
    public AgeSettings TeenagerSettings
    {
        get { return m_Teenager; }
    }

    /// <value>Die Eigenschaft AdultSettings gibt die Einstellungen für die Altersgruppe Erwachsen zurück.</value>
    public AgeSettings AdultSettings
    {
        get { return m_Adult; }
    }

    /// <value>Die Eigenschaft GrandparentsSettings gibt die Einstellungen für die Altersgruppe Senior zurück.</value>
    public AgeSettings GrandparentsSettings
    {
        get { return m_Grandparents; }
    }

    /// <value>Die Eigenschaft DeadColor gibt die Farbe zurück die die Zelle beim Markieren als "Gestoben" haben soll.</value>
    public Color DeadColor
    {
        get { return m_DeadColor; }
    }

    /// <value>Die Eigenschaft AgeWeaknessesOfOld setzt oder gibt das Alter zurück in dem die Zellen an Altersschwäche sterben.</value>
    public int AgeWeaknessesOfOld
    {
        get { return m_CustomRules.AgeWeaknessesOfOld; }
        set { m_CustomRules.AgeWeaknessesOfOld = value; }
    }

    /// <value>Die Eigenschaft MinAgeReproductions setzt oder gibt die Mindestaltersgruppe zurück die Zellen haben müssen um sich Fortpflanzen zu können.</value>
    public Cell.AGES MinAgeReproductions
    {
        get { return m_CustomRules.MinAgeReproductions; }
        set { m_CustomRules.MinAgeReproductions = value; }
    }

    /// <value>Die Eigenschaft MaxAgeReproductions setzt oder gibt die Maximalaltersgruppe zurück die Zellen haben dürfen um sich Fortpflanzen zu können.</value>
    public Cell.AGES MaxAgeReproductions
    {
        get { return m_CustomRules.MaxAgeReproductions; }
        set { m_CustomRules.MaxAgeReproductions = value; }
    }

    /// <value>Die Eigenschaft UsedCustomRules setzt oder gibt zurück ob Benutzerdefinierte Regeln verwendet werden.</value>
    public bool UsedCustomRules
    {
        get { return m_CustomRules.UsedCustomRules; }
        set { m_CustomRules.UsedCustomRules = value; }
    }

#endregion

    /// <summary>Setzt die Benutzerdefinierten Regeln zurück</summary>
    /// <remarks>Setzt die Benutzerdefinierten Regeln zurück auf ihren Ursprungswert</remarks>
    public void ResetCustomRules()
    {
        m_CustomRules.MinAgeReproductions = Cell.AGES.BABY;
        m_CustomRules.MaxAgeReproductions = Cell.AGES.GRANDPARENTS;
        m_CustomRules.AgeWeaknessesOfOld = 0;
    }
}
