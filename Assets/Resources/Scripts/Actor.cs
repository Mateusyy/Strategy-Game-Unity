using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class Actor : MonoBehaviour {

    public ActorData data = new ActorData();
    public UnitsType type;
    public float speed;
    public int health;
    public int damageAmount;

    private void Start() {
        type = gameObject.GetComponent<Unit>().type;
        speed = gameObject.GetComponent<Unit>().speed;
        health = gameObject.GetComponent<Unit>().health;
        damageAmount = gameObject.GetComponent<Unit>().damageAmount;
    }

    public void StoreData()
    {
        data.type = type;
        data.speed = speed;
        data.health = health;
        data.damageAmount = damageAmount;
        Vector3 pos = transform.position;
        data.posX = pos.x;
        data.posY = pos.y;
        data.posZ = pos.z;
    }

    public void LoadData()
    {
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        type = data.type;
        speed = data.speed;
        health = data.health;
        damageAmount = data.damageAmount;
    }

    void OnEnable()
    {
        SaveData.OnLoaded += delegate { LoadData(); };
        SaveData.OnBeforeSave += delegate { StoreData(); };
        SaveData.OnBeforeSave += delegate { SaveData.AddActorData(data); };
    }

    void OnDisable()
    {
        SaveData.OnLoaded -= delegate { LoadData(); };
        SaveData.OnBeforeSave -= delegate { StoreData(); };
        SaveData.OnBeforeSave -= delegate { SaveData.AddActorData(data); };
    }

}

public class ActorData 
{
    [XmlElement("Type")]
    public UnitsType type;

    [XmlElement("Speed")]
    public float speed;

    [XmlElement("Health")]
    public int health;

    [XmlElement("DamageAmount")]
    public int damageAmount;

    [XmlElement("PosX")]
    public float posX;

    [XmlElement("PosY")]
    public float posY;

    [XmlElement("PosZ")]
    public float posZ;
}
