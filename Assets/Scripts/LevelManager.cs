using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePointsType { Green, Red, NeutralPoint }



public enum UnitsType { Green, Red }

public class LevelManager : MonoBehaviour {


    public Color PlayerColor;
    public Color EnemyColor;

    public Transform[] gamePoints;
    public List<GameObject> myGamePoints = new List<GameObject>();
    public List<GameObject> enemyGamePoints = new List<GameObject>();


    private void Start() {
        GenerateGame();
    }

    void GenerateGame() {
        //random player point
        int playerIndex = Random.Range(0, gamePoints.Length);
        //random enemy point
        int enemyIndex;
        do {
            enemyIndex = Random.Range(0, gamePoints.Length);
        } while (enemyIndex == playerIndex);

        //select point from list
        gamePoints[playerIndex].GetComponent<MeshRenderer>().material.color = PlayerColor;
        gamePoints[playerIndex].GetComponent<GamePoint>().type = GamePointsType.Green;
        myGamePoints.Add(gamePoints[playerIndex].gameObject);

        gamePoints[enemyIndex].GetComponent<MeshRenderer>().material.color = EnemyColor;
        gamePoints[enemyIndex].GetComponent<GamePoint>().type = GamePointsType.Red;
        enemyGamePoints.Add(gamePoints[enemyIndex].gameObject);
    }

    public void SaveDataPoints() {
        Debug.Log("zapisuje");
        for (int i = 0; i < gamePoints.Length; i++) {
            if(gamePoints[i].GetComponent<GamePoint>().type == GamePointsType.Green) {
                PlayerPrefs.SetInt(gamePoints[i].name, 1);
            }else if (gamePoints[i].GetComponent<GamePoint>().type == GamePointsType.Red) {
                PlayerPrefs.SetInt(gamePoints[i].name, 2);
            } else if (gamePoints[i].GetComponent<GamePoint>().type == GamePointsType.NeutralPoint) {
                PlayerPrefs.SetInt(gamePoints[i].name, 0);
            }
        }
    }

    public void LoadDataPoints() {
        Debug.Log("odczytuje");
        for (int i = 0; i < gamePoints.Length; i++) {
            if(PlayerPrefs.GetInt(gamePoints[i].name) == 1) {
                gamePoints[i].GetComponent<MeshRenderer>().material.color = PlayerColor;
                gamePoints[i].GetComponent<GamePoint>().type = GamePointsType.Green;
                myGamePoints.Add(gamePoints[i].gameObject);
            }else if (PlayerPrefs.GetInt(gamePoints[i].name) == 2) {
                gamePoints[i].GetComponent<MeshRenderer>().material.color = EnemyColor;
                gamePoints[i].GetComponent<GamePoint>().type = GamePointsType.Red;
                enemyGamePoints.Add(gamePoints[i].gameObject);
            }else if(PlayerPrefs.GetInt(gamePoints[i].name) == 0) {
                gamePoints[i].GetComponent<MeshRenderer>().material.color = Color.white;
                gamePoints[i].GetComponent<GamePoint>().type = GamePointsType.NeutralPoint;

                if (enemyGamePoints.Contains(gamePoints[i].gameObject)) {
                    enemyGamePoints.Remove(gamePoints[i].gameObject);
                }
                if (myGamePoints.Contains(gamePoints[i].gameObject)) {
                    myGamePoints.Remove(gamePoints[i].gameObject);
                }
            }
        }
    }
}
