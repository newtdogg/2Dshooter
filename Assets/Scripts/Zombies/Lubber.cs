using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Lubber : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        speed = 18;
        maxHealth = 80;
        damage = 25;
        status = "idle";
        detectionTimer = -1f;
        zombieObj = transform.GetChild(0).gameObject;
        rbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        health = maxHealth;
        tilemap = GameObject.Find("Map").transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        // healthBar = gameObject.transform.GetChild(0).gameObject;
        intents = transform.GetChild(1);
        // StartCoroutine(UpdatePath());
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
        }
        // test
    }
}
