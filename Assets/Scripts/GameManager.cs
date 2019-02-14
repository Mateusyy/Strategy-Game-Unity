using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    private LevelManager levelManager;

    [HideInInspector]
    public GameObject clickGO;

    public int STAGE;
    public float myGamePoints;
    public float nextLevelGamePoints;
    public int numberOfLevelGamePoint;

    [Header("Player's Unit Stats")]
    public int damageAmount;
    public int health;
    public float speed;

    private int countUnitPerPoint = 10;
    private float timer;
    public GameObject unit;
    public GameObject bullet;

    [HideInInspector]
    public Vector3 targetPos;
    public GameObject PlayerTarget;
    public GameObject EnemyTarget;
    
    public List<GameObject> myUnits = new List<GameObject>();
    public List<GameObject> enemyUnits = new List<GameObject>();

    private void Start() {
        STAGE = 0;  //main menu
        SetStartGamePoints_Param();

        levelManager = FindObjectOfType<LevelManager>();
        InvokeRepeating("SetEnemyTarget", 2f, 2f);
    }

    void SetStartGamePoints_Param() {
        numberOfLevelGamePoint = 1;
        nextLevelGamePoints = 1000;
        myGamePoints = 0;
    }


    void Update() {
        if (STAGE == 1) { //Game
            GenerateUnits();
            MoveUnits();
        }

        if(STAGE == 2) { //LOAD SAVE
            string dataPath;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                dataPath = System.IO.Path.Combine(Application.persistentDataPath, "Resources/actors.xml");
            else
                dataPath = System.IO.Path.Combine(Application.dataPath, "Resources/actors.xml");

            SaveData.Load(dataPath);
            levelManager.LoadDataPoints();

            Unit[] units = FindObjectsOfType<Unit>();
            for (int i = 0; i < units.Length; i++) {
                if(units[i].type == UnitsType.Green) {
                    units[i].TypeGamePoints = GamePointsType.Green;
                    units[i].gameObject.GetComponent<MeshRenderer>().material.color = levelManager.PlayerColor;
                    myUnits.Add(units[i].gameObject);
                    units[i].transform.SetParent(GameObject.Find("MyUnits").transform);
                }else if(units[i].type == UnitsType.Red) {
                    units[i].TypeGamePoints = GamePointsType.Red;
                    units[i].gameObject.GetComponent<MeshRenderer>().material.color = levelManager.EnemyColor;
                    enemyUnits.Add(units[i].gameObject);
                    units[i].transform.SetParent(GameObject.Find("EnemyUnits").transform);
                }
            }

            STAGE = 1;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            Time.timeScale = 2;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Time.timeScale = 1;
        }

        if (levelManager.myGamePoints.Count <= 0 || levelManager.enemyGamePoints.Count <= 0)
            EndGame();
    }


    void SetEnemyTarget() {
        if(EnemyTarget && enemyUnits.Count > 0) {
            for (int i = 0; i < enemyUnits.Count; i++) {
                if (enemyUnits[i].GetComponent<Unit>().Target) {
                    return;
                } else {
                    enemyUnits[i].GetComponent<Unit>().SetTarget(EnemyTarget);
                    CalculatePos_Circle(enemyUnits, EnemyTarget);
                }
            }
        }
    }

    void UnsetEnemyTarget() {
        if (EnemyTarget && enemyUnits.Count > 0) {
            for (int i = 0; i < enemyUnits.Count; i++) {
                if (enemyUnits[i].GetComponent<Unit>().Target) {
                    enemyUnits[i].GetComponent<Unit>().UnsetTarget();
                } else {
                    return;
                }
            }
        }
    }

    void GenerateUnits() {
        if (timer > 0f)
            timer -= 1f * Time.deltaTime;

        if(timer <= 0f) {

            if(myUnits.Count < levelManager.myGamePoints.Count * countUnitPerPoint && myUnits.Count < 50) {
                int pivot_x = Random.Range(3, 7);
                int pivot_z = Random.Range(3, 7);
                int numSpawnPoint = Random.Range(0, levelManager.myGamePoints.Count);
                Transform spawnPoint = levelManager.myGamePoints[numSpawnPoint].transform;

                GameObject unitGO = (GameObject)Instantiate(unit, new Vector3(spawnPoint.transform.position.x + pivot_x, 1, spawnPoint.transform.position.z + pivot_z), Quaternion.identity);
                unitGO.GetComponent<Unit>().SetStartParams(damageAmount, health, speed, UnitsType.Green, GamePointsType.Green);

                unitGO.GetComponent<MeshRenderer>().material.color = levelManager.PlayerColor;
                myUnits.Add(unitGO);
                unitGO.transform.SetParent(GameObject.Find("MyUnits").transform);
                if(targetPos != new Vector3(0,0,0)) {
                    if(clickGO.tag == "Point") {
                        SetUnitTarget(true, clickGO);
                        CalculatePos_Circle(myUnits, clickGO);
                    } else {
                        UnsetUnitTarget(true);
                        CalculatePos_Rectangle(myUnits);
                    }
                }

                if (PlayerTarget) {
                    unitGO.GetComponent<Unit>().SetTarget(PlayerTarget);
                    CalculatePos_Circle(myUnits, clickGO);
                }
            }

            if(enemyUnits.Count < levelManager.enemyGamePoints.Count * countUnitPerPoint && enemyUnits.Count < 50) {
                int pivot_x = Random.Range(3, 7);
                int pivot_z = Random.Range(3, 7);
                int numSpawnPoint = Random.Range(0, levelManager.enemyGamePoints.Count);
                Transform spawnPoint = levelManager.enemyGamePoints[numSpawnPoint].transform;

                GameObject unitGO = (GameObject)Instantiate(unit, new Vector3(spawnPoint.transform.position.x + pivot_x, 1, spawnPoint.transform.position.z + pivot_z), Quaternion.identity);
                unitGO.GetComponent<Unit>().SetStartParams(damageAmount, health, speed, UnitsType.Red, GamePointsType.Red);

                unitGO.GetComponent<MeshRenderer>().material.color = levelManager.EnemyColor;
                enemyUnits.Add(unitGO);
                unitGO.transform.SetParent(GameObject.Find("EnemyUnits").transform);

                if (EnemyTarget) {
                    unitGO.GetComponent<Unit>().SetTarget(EnemyTarget);
                    CalculatePos_Circle(enemyUnits, EnemyTarget);
                }
            }

            timer = 1f;
        }
    }


    void MoveUnits() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit)) {

                    clickGO = hit.collider.gameObject;
                    targetPos = hit.point;


                    if (clickGO.tag == "Point") {
                        SetUnitTarget(true, clickGO);
                        CalculatePos_Circle(myUnits, clickGO);
                    } else {
                        UnsetUnitTarget(true);
                        CalculatePos_Rectangle(myUnits);
                    }
                }
            }
        }
    }

    //circle
    public void CalculatePos_Circle(List<GameObject> unitList, GameObject point) {
        float angle = 360 / unitList.Count;
        float r = 7f;

        for (int i = 0; i < unitList.Count; i++) {
            float x = point.transform.position.x + r * Mathf.Cos(angle * i / unitList.Count);
            float z = point.transform.position.z + r * Mathf.Sin(angle * i / unitList.Count);
            unitList[i].GetComponent<NavMeshAgent>().SetDestination(new Vector3(x, targetPos.y, z));
        }
    }

    //ustawia jednostką cel
    public void SetUnitTarget(bool whichUnit, GameObject obj) {
        if (whichUnit) {
            for (int i = 0; i < myUnits.Count; i++) {
                myUnits[i].GetComponent<Unit>().SetTarget(obj);
            }
        } else {
            for (int i = 0; i < enemyUnits.Count; i++) {
                enemyUnits[i].GetComponent<Unit>().SetTarget(obj);
            }
        }
    }

    //czyści cele
    public void UnsetUnitTarget(bool whichUnit) {
        if (whichUnit) {
            PlayerTarget = null;
            for (int i = 0; i < myUnits.Count; i++) {
                myUnits[i].GetComponent<Unit>().UnsetTarget();
            }
        } else {
            EnemyTarget = null;
            for (int i = 0; i < enemyUnits.Count; i++) {
                enemyUnits[i].GetComponent<Unit>().UnsetTarget();
            }
        }
    }

    //rectangle
    void CalculatePos_Rectangle(List<GameObject> unitList) {
        float num_ = Mathf.Sqrt(unitList.Count);
        int num = Mathf.FloorToInt(num_);

        for (int i = 0; i < unitList.Count; i++) {
            unitList[i].GetComponent<NavMeshAgent>().SetDestination(new Vector3(targetPos.x + (i % num) * 2.5f, targetPos.y, targetPos.z + (i / num) * 2.5f));
        }
    }

    

    void EndGame() {
        STAGE = 3;

        if(levelManager.myGamePoints.Count <= 0) {
            FindObjectOfType<UI>().EndGame("lost");
        }else if(levelManager.enemyGamePoints.Count <= 0) {
            FindObjectOfType<UI>().EndGame("win");
        }
    }


    public void ActorEnable() {
        Actor[] actors = FindObjectsOfType<Actor>();
        for (int i = 0; i < actors.Length; i++) {
            actors[i].GetComponent<Actor>().enabled = true;
        }
    }

    public void ActorDisable() {
        Actor[] actors = FindObjectsOfType<Actor>();
        for (int i = 0; i < actors.Length; i++) {
            actors[i].GetComponent<Actor>().enabled = false;
        }
    }
}
