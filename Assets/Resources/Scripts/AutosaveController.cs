using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutosaveController : MonoBehaviour {

    public Button saveButton;
    public Button loadButton;
    public const string playerPath = "Prefabs/Unit";

    
    static string dataPath = string.Empty;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            dataPath = System.IO.Path.Combine(Application.persistentDataPath, "Resources/actors.xml");
        else
            dataPath = System.IO.Path.Combine(Application.dataPath, "Resources/actors.xml");
    }

    

    public static Actor CreateActor(string path, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
        Actor actor = go.GetComponent<Actor>() ?? go.AddComponent<Actor>();
        return actor;
    }

    public static Actor CreateActor(ActorData data, string path, Vector3 position, UnitsType type, float speed, int health, int damageAmount, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
        Actor actor = go.GetComponent<Actor>() ?? go.AddComponent<Actor>();
        Unit unit = go.GetComponent<Unit>() ?? go.AddComponent<Unit>();
        actor.data = data;
        unit.type = type;
        unit.speed = speed;
        unit.health = health;
        unit.damageAmount = damageAmount;        

        return actor;
    }

    void OnEnable()
    {
        saveButton.onClick.AddListener(
            delegate {
                SaveData.Save(dataPath, SaveData.actorContainer);
            }
        );
        //loadButton.onClick.AddListener(delegate { SaveData.Load(dataPath); });
    }
    void OnDisable()
    {
        saveButton.onClick.RemoveListener(delegate { SaveData.Save(dataPath, SaveData.actorContainer); });
        //loadButton.onClick.RemoveListener(delegate { SaveData.Load(dataPath); });
    }
}
