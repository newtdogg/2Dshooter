using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ripper : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        speed = 30;
        maxHealth = 26;
        damage = 15;
        status = "idle";
        detectionTimer = -1f;
        scrapObject = GameObject.Find("Scrap");
        recipeObject = GameObject.Find("RecipeObject");
        rbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        health = maxHealth;
        tilemap = GameObject.Find("Map").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
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
            dropScrap();
        }
    }
}
