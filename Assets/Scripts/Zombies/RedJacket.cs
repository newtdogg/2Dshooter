using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class RedJacket : MobController {
    // Start is called before the first frame update
    void Start() {
        defaultMobAwake("MobRedJacket");

        var mapObject = GameObject.Find("MapGridObject");
        tilemap = mapObject.transform.GetChild(0).gameObject.GetComponent<Tilemap>();
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        manageBehaviourState();
        defaultUpdate();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name == "Player") {
            StartCoroutine(knockPlayerBack(col, 7.0f));
            playerController.updateHealth(-damage);
            StopCoroutine("CycleRandomAttacks");
        }
        defaultMobCollisionEnter(col);
        defaultBulletCollisionEnter(col);
    }
}
