using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class WebRat : ZombieController
{
    public GameObject hookClone;
    void Start()
    {
        defaultZombieAwake("MobWebRat");
        bullet = GameObject.Find("DoCHook");
        attacks = new List<Action>() { throwHook };
        attackDelay = 3;
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        manageBehaviourState();
        if(hookAttached) {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.1f);
        }
        defaultUpdate();
    }

    private void throwHook() {
        var vectorAngle = (player.transform.position - transform.position).normalized;
        hookClone = Instantiate(bullet, transform.position + (new Vector3(vectorAngle.x, vectorAngle.y, 0) * 0.4f), Quaternion.identity) as GameObject;
        hookClone.transform.SetParent(transform);
        hookClone.GetComponent<Rigidbody2D>().AddForce(vectorAngle * 80f);
        hookClone.GetComponent<Projectile>().setLifetime(5f);
        hookClone.GetComponent<Projectile>().parent = "MobWebRat(Clone)";
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            inContactWithPlayer = true;
            hookAttached = false;
            playerController.canMove = false;
            contactController.updateBrawlStatus(gameObject);
            Destroy(hookClone);
            StopCoroutine("FollowPath");
            StopCoroutine("CycleRandomAttacks");
        }
        if (col.gameObject.name.Contains("Mob")) {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }

    public IEnumerator attachToPlayer() {
        yield return null;
    }
}
