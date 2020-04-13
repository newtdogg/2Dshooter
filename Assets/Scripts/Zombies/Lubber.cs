using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lubber : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        speed = 10;
        maxHealth = 80;
        damage = 25;
        status = "idle";
        detectionTimer = -1f;
        zombieObj = transform.GetChild(0).gameObject;
        rbody = zombieObj.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        health = maxHealth;
        healthBar = gameObject.transform.GetChild(1).gameObject;
        intents = transform.GetChild(2);
    }
}
