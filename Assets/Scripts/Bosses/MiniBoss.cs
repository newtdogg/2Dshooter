using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : Boss {

    public virtual void startFight() {
    }

    public void onDeath() {
        switch (gameController.mode) {
            // case "normal":
            //     lootController.dropMiniBossLoot(transform.position);
            //     lootController.dropPerk(transform.position);
            //     gameController.spawnDoorToNextLevel(spawnPosition);
            //     break;
            // case "tutorial":
            //     gameController.spawnDoorToNextLevel(new Vector3(spawnPosition.x + 3, spawnPosition.y, 0));
            //     gameController.spawnDoorToIntroArea(new Vector3(spawnPosition.x - 3, spawnPosition.y, 0));
            //     break;
        };
    }
    
    void Update() {
        if(health <= 0) {
            Destroy(gameObject);
            onDeath();
        }
    }
}
