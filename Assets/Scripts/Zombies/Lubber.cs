using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Lubber : ZombieController
{
    // Start is called before the first frame update
    void Start()
    {
        defaultZombieAwake("MobLubber");
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
            onDeath();
        }
    }
}
