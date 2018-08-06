/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse sorgt dafür das bei der Plattform Android   *
 * das Display während dem Spielen nicht auf Standby geht                       *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Android : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
    {
        #if UNITY_ANDROID
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #else
            enabled = false;
        #endif
	}
}
