using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ripper : ZombieController
{
    // Start is called before the first frame update
    void Start() {
        defaultZombieAwake("MobRipper");
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
            case "dormant":
                break;
        }
        if(health <= 0) {
            onDeath();
        }
    }
}
