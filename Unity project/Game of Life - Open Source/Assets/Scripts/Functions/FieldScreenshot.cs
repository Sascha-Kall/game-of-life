/*==============================================================================*
 * Projekt: Game of life                                                        *
 * Entwickler: Sascha Kall https://www.sascha-kall.de                           *
 * Klassen Beschreibung: Die Klasse macht speichert das Renderbild von der      *
 * Camera auf dem das Script sitzt                                              *
 * =============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FieldScreenshot : MonoBehaviour 
{
    public Camera m_ScreenShotCamera = null;

	// Use this for initialization
	void Start () 
    {
        m_ScreenShotCamera = GetComponent<Camera>();
        if (!m_ScreenShotCamera)
        {
            Debug.LogError("Es wurde auf dem Gameobjekt: " + name + " keine Kamera Komponente gefunden! Das Script FieldScreenshot wird beendet.");
            enabled = false;
        }
	}

    public Texture2D MakeScreenshot()
    {
        RenderTexture TempRenderTexture = new RenderTexture(1920, 1080, 24);
        Texture2D TempScreenshotTexture = new Texture2D(m_ScreenShotCamera.pixelWidth, m_ScreenShotCamera.pixelHeight, TextureFormat.RGB24, false);

        m_ScreenShotCamera.targetTexture = TempRenderTexture;
        m_ScreenShotCamera.Render();

        RenderTexture.active = TempRenderTexture;
        TempScreenshotTexture.ReadPixels(new Rect(0, 0, m_ScreenShotCamera.pixelWidth, m_ScreenShotCamera.pixelHeight), 0, 0);

        m_ScreenShotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(TempRenderTexture);

        return TempScreenshotTexture;
    }
}
