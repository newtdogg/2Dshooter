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

    // Update is called once per frame
    void Update() {
        
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(!spawner.battleStarted) {
            if(col.gameObject.name == "Player" && spawner.transform.GetChild(0).childCount != 0) {
                spawner.startBattle();
            }
        }
    }
}
