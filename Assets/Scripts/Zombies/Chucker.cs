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
        title = "MobChucker";
        speed = 20;
        maxHealth = 26;
        damage = 15;
        status = "idle";
        scrapDropMin = 1;
        scrapDropMax = 3;
        detectionTimer = -1f;
        canMove = true;
        attackDelay = 3f;
        scrapObject = GameObject.Find("Scrap");
        recipeObject = GameObject.Find("RecipeObject");
        rbody = gameObject.GetComponent<Rigidbody2D>();
        bullet = transform.GetChild(2).gameObject;
        if(gameObject.name == $"{title}(Clone)") {
            spawner = transform.parent.parent.gameObject.GetComponent<Spawner>();
            lootController = transform.parent.parent.gameObject.GetComponent<Spawner>().lootController;
        }
        attacks = new List<Action>() { throwProjectile };
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
                StartCoroutine("CycleRandomAttacks");
                StartCoroutine("UpdatePath");
                status = "attacking";
                break;
            case "idle":
                idleBehaviour();
                break;
        }
        if(health <= 0) {
            Destroy(gameObject);
            lootController.dropZombieLoot(transform.position);
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
