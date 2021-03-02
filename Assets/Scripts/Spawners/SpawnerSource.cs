using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSource : MonoBehaviour
{
    // Start is called before the first frame update
    public Spawner spawner;
    public float hp;

    // void OnTriggerEnter2D(Collider2D col) {
    //     if(!spawner.battleStarted) {
    //         if(col.gameObject.name == "Player" && spawner.transform.GetChild(0).childCount != 0) {
    //             spawner.startBattle();
    //         }
    //     }
    // }
}
