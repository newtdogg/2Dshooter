using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripper : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        speed = 25;
        maxHealth = 40;
        damage = 15;
        status = "idle";
        detectionTimer = -1f;
        rbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        health = maxHealth;
        healthBar = gameObject.transform.GetChild(0).gameObject;
        intents = transform.GetChild(1);

    }

    // Update is called once per frame
}
