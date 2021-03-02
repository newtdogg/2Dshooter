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
