using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class Slinger : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        defaultZombieAwake("MobSlinger");
        bullet = transform.GetChild(2).gameObject;
        attacks = new List<Action>() { throwHook };
        attackDelay = 4;
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        switch (status) {
            case "attackNow":
                StopCoroutine("UpdatePath");
                StartCoroutine("UpdatePath");
                StartCoroutine("CycleRandomAttacks");
                status = "attacking";
                break;
            case "idle":
                manageBehaviourState();
                break;
        }
        if(health <= 0) {
            onDeath();
        }
        if(hookAttached) {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.1f);
        }
    }

    private void throwHook() {
        var vectorAngle = (player.transform.position - transform.position).normalized;
        var bulletClone = Instantiate(bullet, transform.position + (new Vector3(vectorAngle.x, vectorAngle.y, 0) * 0.4f), Quaternion.identity) as GameObject;
        bulletClone.transform.SetParent(transform);
        bulletClone.GetComponent<Rigidbody2D>().AddForce(vectorAngle * 25f);
        bulletClone.GetComponent<Projectile>().setLifetime(1.5f);
        bulletClone.GetComponent<Projectile>().parent = "MobSlinger(Clone)";
    }

    public IEnumerator attachToPlayer() {
        yield return null;
    }
}
