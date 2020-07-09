using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ripper : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        title = "Ripper";
        speed = 30;
        maxHealth = 26;
        damage = 15;
        status = "idle";
        scrapDropMin = 1;
        scrapDropMax = 3;
        detectionTimer = -1f;
        scrapObject = GameObject.Find("Scrap");
        recipeObject = GameObject.Find("RecipeObject");
        rbody = gameObject.GetComponent<Rigidbody2D>();
        if(gameObject.name == $"{title}(Clone)") {
            Debug.Log(transform.parent.parent.gameObject);
            lootManager = transform.parent.parent.gameObject.GetComponent<Spawner>().lootManager;
        }
        player = GameObject.Find("Player");
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
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
                status = "attacking";
                break;
            case "idle":
                idleBehaviour();
                break;
        }
        if(health <= 0) {
            Destroy(gameObject);
            lootManager.dropZombieLoot(transform.position);
        }
    }
}
