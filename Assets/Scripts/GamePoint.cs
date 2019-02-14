using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePoint : MonoBehaviour {

    private LevelManager levelManager;


    public List<Transform> neighbourPoints = new List<Transform>();
    public bool isAttacked = false;

    public int health = 1000;
    public Text healthTxt;
    public GamePointsType type = GamePointsType.NeutralPoint;

    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update() {
        UIUpdate();
    }

    private void UIUpdate() {
        healthTxt.text = "" + health;
    }

    public void TakeDamage(int damageAmount) {
        health -= damageAmount;
    }

    public void ChangeTypeUnit(UnitsType typeChange) {
        if(typeChange == UnitsType.Green) {
            gameObject.GetComponent<MeshRenderer>().material.color = FindObjectOfType<LevelManager>().PlayerColor;
            gameObject.GetComponent<GamePoint>().type = GamePointsType.Green;
        }
        if(typeChange == UnitsType.Red) {
            gameObject.GetComponent<MeshRenderer>().material.color = FindObjectOfType<LevelManager>().EnemyColor;
            gameObject.GetComponent<GamePoint>().type = GamePointsType.Red;
        }
    }

    public void AddThisPointToList(UnitsType typeList) {
        if(typeList == UnitsType.Green) {
            if (levelManager.enemyGamePoints.Contains(gameObject))
                levelManager.enemyGamePoints.Remove(gameObject);

            levelManager.myGamePoints.Add(gameObject);
        }
        if (typeList == UnitsType.Red) {
            if (levelManager.myGamePoints.Contains(gameObject))
                levelManager.myGamePoints.Remove(gameObject);

            levelManager.enemyGamePoints.Add(gameObject);
        }
    }
}
