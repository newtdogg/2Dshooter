using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class RedJacket : MobController
{
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
}
