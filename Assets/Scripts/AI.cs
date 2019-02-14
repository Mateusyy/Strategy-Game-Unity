using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour {

    private LevelManager levelManager;
    private GameManager gameManager;
    public GameObject target;
    private int STAGE_ATTACK;       //0 - firstattack;  1 - nextattack;
    


    //na starcie wybieramy jeden punkt i atakujemy go - punkt musi byc neutralny. O ataku decyduje flaga która zostaje zmieniona po przejęciu punktu-celu
    //kolejno wybieramy punkt z dostepnych i zajmujemy nastepny

    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
        STAGE_ATTACK = 0;
    }

    private void Update() {
        if (STAGE_ATTACK == 0) {
            FirstAttack();
        }else if(STAGE_ATTACK == 1) {
            NextAttack();
        }
    }


    private void FirstAttack() {
        GameObject firstTarget = levelManager.enemyGamePoints[0].GetComponent<GamePoint>().neighbourPoints.First().gameObject;

        if (firstTarget.GetComponent<GamePoint>().type == GamePointsType.Red) {
            STAGE_ATTACK = 1;
            return;
        }
            
        if (firstTarget.GetComponent<GamePoint>().type != GamePointsType.NeutralPoint) {
            firstTarget = levelManager.enemyGamePoints[0].GetComponent<GamePoint>().neighbourPoints[1].gameObject;
        }

        SetTarget(firstTarget);
    }

    private void NextAttack() {
        GamePoint myGamePoint = levelManager.enemyGamePoints[levelManager.enemyGamePoints.Count - 1].GetComponent<GamePoint>();

        //szukam odpowiedniego celu
        if(!target || target == null) {
            for (int i = 0; i < myGamePoint.neighbourPoints.Count; i++) {
                if (myGamePoint.neighbourPoints[i].GetComponent<GamePoint>().type != GamePointsType.Red) {
                    target = myGamePoint.neighbourPoints[i].gameObject;
                    SetTarget(target);
                    break;
                }
            }
        }

        if (target.GetComponent<GamePoint>().type == GamePointsType.Red) {
            target = null;
            UnsetTarget();
        }
            
    }

    private void SetTarget(GameObject target) {
        gameManager.EnemyTarget = target;
    }

    private void UnsetTarget() {
        gameManager.UnsetUnitTarget(false);
    }
}
