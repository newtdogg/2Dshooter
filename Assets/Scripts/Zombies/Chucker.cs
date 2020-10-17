using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class Chucker : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        defaultZombieAwake("MobChucker");
        bullet = transform.GetChild(2).gameObject;
        attacks = new List<Action>() { throwProjectile };
        attackDelay = 3f;
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        switch (status) {
            case "attackNow":
                StopCoroutine("UpdatePath");
                StartCoroutine("CycleRandomAttacks");
                StartCoroutine("UpdatePath");
                status = "attacking";
                break;
            case "idle":
                idleBehaviour();
                break;
        }
        if(health <= 0) {
            onDeath();
        }
    }

    private void throwProjectile() {
        var vectorAngle = (player.transform.position - transform.position).normalized;
        var bulletClone = Instantiate(bullet, transform.position + new Vector3(vectorAngle.x, vectorAngle.y, 0), Quaternion.identity) as GameObject;
        bulletClone.transform.SetParent(transform);
        bulletClone.GetComponent<Rigidbody2D>().AddForce(vectorAngle * 30);
        bulletClone.GetComponent<Bullet>().setLifetime(4);
        bulletClone.GetComponent<Bullet>().parent = gameObject.name;
    }
}
