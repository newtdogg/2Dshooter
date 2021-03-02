using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    public Spawner spawner;

    void Start() {
        spawner = transform.parent.gameObject.GetComponent<Spawner>();
    }


    void OnTriggerEnter2D(Collider2D col) {
        if(!spawner.battleStarted && (spawner.type == SpawnerType.Walled || spawner.type == SpawnerType.Trap)) {
            if(col.gameObject.name == "Player" && !spawner.battleCompleted) {
                spawner.triggerWalls();
            }
        }
    }
}
