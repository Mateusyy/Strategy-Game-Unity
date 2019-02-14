using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Transform target;
    private float speed = 70f;


    public void SetParam(Transform _target) {
        target = _target;
    }


    private void Update() {
        if(target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = transform.position - target.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(direction.magnitude <= distanceThisFrame) {
            HitTarget();
            return;
        }

        transform.Translate(-direction.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget() {
        Damage(target);
        Destroy(gameObject);
    }

    void Damage(Transform enemy) {
        Unit e = enemy.GetComponent<Unit>();
        if(e != null) {
            e.TakeDamage(e.damageAmount);
        }
    }
}
