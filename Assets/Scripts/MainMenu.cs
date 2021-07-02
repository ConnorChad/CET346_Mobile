using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class MainMenu : MonoBehaviour
{
    [Header("Screens")]
    public GameObject MainMenuScreen;
    public GameObject optionsScreen;
    public GameObject tutorialScreen;

    [Header("Graphics Quality Presets")]
    public RenderPipelineAsset lowQuality;
    public RenderPipelineAsset highQuality;
    
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GoToMainMenu()
    {
        optionsScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        MainMenuScreen.SetActive(true);    
    }

    public void GoToOptions()
    {
        MainMenuScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void GoToTutorial()
    {
        MainMenuScreen.SetActive(false);
        tutorialScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LowGraphics()
    {
        QualitySettings.SetQualityLevel(0);
        QualitySettings.renderPipeline = lowQuality;
    }

    public void HighGraphics()
    {
        QualitySettings.SetQualityLevel(2);
        QualitySettings.renderPipeline = highQuality;
    }
}
