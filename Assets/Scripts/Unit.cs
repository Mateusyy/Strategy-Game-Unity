using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    private GameManager gameManager;
    private LevelManager levelManager;
    private NavMeshAgent agent;
    public UnitsType type;
    private GamePointsType typeGamePoints;
    public GamePointsType TypeGamePoints {get { return typeGamePoints;}set {typeGamePoints = value;}}

    public GameObject firePoint;

    public int damageAmount;
    public int health;
    public float speed;

    private float timer;

    private GameObject target;
    public GameObject Target {get {return target;}set {target = value;}}


    public GameObject targetToAim;

    public bool attackPoint = false;
    private bool isDead = false;
    private float timerShoot;
    private int index;

    private void Start() {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        agent = GetComponent<NavMeshAgent>();


        //SetStartParams(); ---> gameManager->generateUnits();
    }

    public void SetStartParams(int _damageAmount, int _health, float _speed, UnitsType _type, GamePointsType _typeGamePoints) {
        damageAmount = _damageAmount;
        health = _health;
        speed = _speed;
        type = _type;
        typeGamePoints = _typeGamePoints;
    }

    public void SetTarget(GameObject _target) {
        if(_target != null) {
            target = _target;
        }
    }

    public void UnsetTarget() {
        if (target != null) {
            target = null;
        }
    }

    private void Update() {
        AttackPoint();

        if (!targetToAim && gameManager.enemyUnits.Count > 0) {
            SearchTarget();
        }

        if (!attackPoint) {
            AttackGun();
        }
    }

    

    void SearchTarget() {
        if (type == UnitsType.Green) {
            //szukanie celu dla gracza
            index = gameManager.myUnits.IndexOf(gameObject);
            //SearchTargetToAim(gameManager.enemyUnits, index);
            if(gameManager.enemyUnits.Count > index) {
                targetToAim = gameManager.enemyUnits[index];
            } else {
                targetToAim = gameManager.enemyUnits.Last();
            }
        } else if (type == UnitsType.Red) {
            //sukanie celu dla przeciwnika
            index = gameManager.enemyUnits.IndexOf(gameObject);
            //SearchTargetToAim(gameManager.myUnits, index);
            if (gameManager.myUnits.Count > index) {
                targetToAim = gameManager.myUnits[index];
            } else {
                targetToAim = gameManager.myUnits.Last();
            }
        }
    }

    void AttackGun() {
        //attak
         if(targetToAim) {
            if (Vector3.Distance(gameObject.transform.position, targetToAim.transform.position) <= 20f) {
                //celowanie i strzelanie
                transform.LookAt(new Vector3(targetToAim.transform.position.x, 2f, targetToAim.transform.position.z));

                if (timerShoot > 0f)
                    timerShoot -= Time.deltaTime;

                if (timerShoot <= 0f) {
                    if(type == UnitsType.Green) {
                        if (gameManager.myGamePoints <= gameManager.nextLevelGamePoints) {
                            gameManager.myGamePoints += 50f;
                        }
                    }
                    

                    Shoot();
                    timerShoot = 1f;
                }
            } 
        }
    }

    void SearchTargetToAim(List<GameObject> whichList, int index) {
        
        if (whichList.Count >= index) {
            targetToAim = whichList[index];
        } else if(whichList.Count < index) {
            targetToAim = whichList.Last();
        }
    }

    void UnsetTargetToAim() {
            targetToAim = null;
    }

    void Shoot() {
        GameObject bulletGO = (GameObject)Instantiate(gameManager.bullet, firePoint.transform.position, firePoint.transform.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
            bullet.SetParam(targetToAim.transform);
    }

    void AttackPoint() {
        if (target && 
            target.tag == "Point" &&
            target.GetComponent<GamePoint>().type != typeGamePoints &&
            Vector3.Distance(transform.position, target.transform.position) < 10f) {

            attackPoint = true;
        } else {
            attackPoint = false;
        }



        if (target && target.tag == "Point" &&
            target.GetComponent<GamePoint>().type != typeGamePoints &&
            target.GetComponent<GamePoint>().health > 0 &&
            Vector3.Distance(transform.position, target.transform.position) < 10f) {


            if (timer > 0f)
                timer -= 1f * Time.deltaTime;


            if (type == UnitsType.Green) {
                if (target && target.tag == "Point" && target.GetComponent<GamePoint>().type != typeGamePoints && timer <= 0f) {
                    if (gameManager.myGamePoints <= gameManager.nextLevelGamePoints) {
                        gameManager.myGamePoints += 25f;
                    }
                }
            }
        

            if (timer <= 0f) {
                
                
                target.GetComponent<GamePoint>().TakeDamage(damageAmount);
                //przejęcie punktu
                if (target.GetComponent<GamePoint>().health <= 0) {
                    target.GetComponent<GamePoint>().ChangeTypeUnit(type);
                    target.GetComponent<GamePoint>().AddThisPointToList(type);

                    //do zmiany
                    target.GetComponent<GamePoint>().health = 999;
                }

                timer = 1f;
            }
        } 
    }

    public void TakeDamage(int damageAmount) {
        health -= damageAmount;

        if(health <= 0 && !isDead) {
            Die();
        }
    }

    private void Die() {
        isDead = true;

        GameObject objToDestroy;
        int index;
        var whichList = new List<GameObject>();

        if (type == UnitsType.Green)
            whichList = gameManager.myUnits;
        else if(type == UnitsType.Red)
            whichList = gameManager.enemyUnits;

        index = whichList.IndexOf(gameObject);
        objToDestroy = whichList[index];


        whichList.Remove(objToDestroy);
        Destroy(objToDestroy.gameObject);
    } 
}
