using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RedJacket : MobController {
    // Start is called before the first frame update
    void Start() {
        defaultMobAwake("MobRedJacket");
        var mapObject = GameObject.Find("MapGridObject");
        tilemap = mapObject.transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        attacks = new List<Action>() { lungeAtPlayerInRange };
        attackDelay = 1.5f;
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        manageBehaviourState();
        defaultUpdate();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            StartCoroutine(knockPlayerBack(col, 14.0f));
            playerController.dealMobDamage(-damage);
            playerController.slow(0.2f);
            StopCoroutine("CycleRandomAttacks");
        }
        defaultMobCollisionEnter(col);
        defaultBulletCollisionEnter(col);
    }

    public void lungeAtPlayerInRange() {
        if(distance < 3.5f) {
            StartCoroutine(lunge(1.5f, 9));
        }
    }
}
