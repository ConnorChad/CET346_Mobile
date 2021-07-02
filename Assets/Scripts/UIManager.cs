using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Button interactButton;
    public Button attackButton;
    public Image healthBar;
    public Image manaBar;
    public GameObject deathPanel;
    public GameObject pausePanel;
    public Sprite rangedSprite;
    public Sprite meleeSprite;

    private PlayerController player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void Update()
    {
        SetHealth();
        SetMana();
    }

    public void SetHealth()
    {
        healthBar.fillAmount = player.health / player.maxHealth;
    }

    public void SetMana()
    {
        manaBar.fillAmount = player.mana / player.maxMana;
    }
    public void EnableDeathPanel()
    {
        deathPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}
