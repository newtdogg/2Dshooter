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
        title = "MobSlinger";
        speed = 20;
        maxHealth = 26;
        damage = 15;
        status = "idle";
        scrapDropMin = 1;
        scrapDropMax = 3;
        detectionTimer = -1f;
        canMove = true;
        scrapObject = GameObject.Find("Scrap");
        recipeObject = GameObject.Find("RecipeObject");
        rbody = gameObject.GetComponent<Rigidbody2D>();
        bullet = transform.GetChild(2).gameObject;
        if(gameObject.name == $"{title}(Clone)") {
            spawner = transform.parent.parent.gameObject.GetComponent<Spawner>();
            lootController = transform.parent.parent.gameObject.GetComponent<Spawner>().lootController;
        }
        attacks = new List<Action>() { throwHook };
        attackDelay = 4;
        player = GameObject.Find("Player");
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        contactController = playerController.transform.GetChild(3).GetChild(2).gameObject.GetComponent<ContactController>();
        health = maxHealth;
        // remove when not needed for debugging
        tilemap = GameObject.Find("MapGridObject").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        // healthBar = gameObject.transform.GetChild(0).gameObject;
        intents = transform.GetChild(1);
        scrap = 15;
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
                idleBehaviour();
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
