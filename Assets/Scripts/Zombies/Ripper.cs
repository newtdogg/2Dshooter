using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Ripper : ZombieController
{
    // Start is called before the first frame update
    void Start() {
        defaultZombieAwake("MobRipper");

        var mapObject = GameObject.Find("MapGridObject");
        tilemap = mapObject.transform.GetChild(0).gameObject.GetComponent<Tilemap>();
    }

    void Update() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        manageBehaviourState();
        if(health <= 0) {
            onDeath();
        }
        if(damageParent.GetChild(0).gameObject.activeSelf) {
            damageIndicatorTimer += Time.deltaTime;
        }
        if(damageIndicatorTimer > 1.5f) {
            damageParent.GetChild(0).gameObject.SetActive(false);
            damageIndicatorTimer = 0;
        }
    }
}
