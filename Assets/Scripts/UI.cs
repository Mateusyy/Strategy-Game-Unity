using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    private GameManager gameManager;

    public GameObject startMenu;
    public GameObject updateMenu;
    public GameObject endMenu;
    public Image fillImage;

    public Text forceTxt;
    public Text healthTxt;
    public Text speedTxt;
    public Text endText;

    private int startDamageAmount;
    private int startHealth;
    private float startSpeed;

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();

        startMenu.SetActive(true);
        updateMenu.SetActive(false);
        endMenu.SetActive(false);

        startDamageAmount = gameManager.damageAmount;
        startHealth = gameManager.health;
        startSpeed = gameManager.speed;
    }

    private void Update() {
        if(gameManager.nextLevelGamePoints != 0)
            fillImage.GetComponent<Image>().fillAmount = gameManager.myGamePoints / gameManager.nextLevelGamePoints;

        if (gameManager.myGamePoints >= gameManager.nextLevelGamePoints)
            ShowUpdatePanel();


        UpdateUI();
    }

    void UpdateUI() {
        forceTxt.text = "FORCE: " + (gameManager.damageAmount - startDamageAmount);
        healthTxt.text = "HEALTH: " + (gameManager.health - startHealth);
        speedTxt.text = "SPEED: " + (gameManager.speed - startSpeed);
    }

    public void StartMenu_StartButton() {
        FindObjectOfType<GameManager>().STAGE = 1;
        startMenu.SetActive(false);
        gameManager.ActorEnable();
    }

    /*public void StartMenu_LoadAutosave() {
        FindObjectOfType<GameManager>().STAGE = 2;
        startMenu.SetActive(false);
    }*/


    public void StartMenu_ExitButton() {
        Application.Quit();
    }

    public void ShowUpdatePanel() {
        updateMenu.SetActive(true);
        fillImage.gameObject.SetActive(false);
    }


    public void Update_AddForce() {
        if (gameManager.damageAmount - startDamageAmount < 10)
            gameManager.damageAmount += 1;
        updateMenu.SetActive(false);
        fillImage.gameObject.SetActive(true);

        RestartLevelPoint();
    }

    public void Update_AddHealth() {
        if (gameManager.health - startHealth < 10)
            gameManager.health += 1;
        updateMenu.SetActive(false);
        fillImage.gameObject.SetActive(true);

        RestartLevelPoint();
    }

    public void Update_AddSpeed() {
        if(gameManager.speed - startSpeed < 10)
            gameManager.speed += 1;
        updateMenu.SetActive(false);
        fillImage.gameObject.SetActive(true);

        RestartLevelPoint();
    }

    private void RestartLevelPoint() {
        if (gameManager.numberOfLevelGamePoint < 30) {
            gameManager.numberOfLevelGamePoint += 1;

            gameManager.nextLevelGamePoints = gameManager.numberOfLevelGamePoint * 1000;
            gameManager.myGamePoints = 0;
        }
    }

    public void EndGame(string result) {
        endMenu.SetActive(true);

        if(result == "win") {
            endText.text = "YOU WIN!";
        }else if(result == "lost") {
            endText.text = "YOU LOST!";
        } else {
            endText.text = "***";
        }

        gameManager.ActorDisable();
    }

    public void EndMenu_TryAgainButton() {
        Application.LoadLevel(Application.loadedLevelName);
        gameManager.STAGE = 1;
        endMenu.SetActive(false);
    }

    public void EndMenu_MainMenuButton() {
        gameManager.STAGE = 0;
        endMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void SaveButton() {
        FindObjectOfType<LevelManager>().SaveDataPoints();
    }
}
