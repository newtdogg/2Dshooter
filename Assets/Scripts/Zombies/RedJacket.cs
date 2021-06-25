using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class RedJacket : ZombieController
{
    // Start is called before the first frame update
    void Start() {
        defaultZombieAwake("MobRedJacket");

        var mapObject = GameObject.Find("MapGridObject");
        tilemap = mapObject.transform.GetChild(0).gameObject.GetComponent<Tilemap>();
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        manageBehaviourState();
        defaultUpdate();
    }
}
