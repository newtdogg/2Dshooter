using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : Pickup {

    public string scrapType;

    void Start() {
        type = "Scrap";
    }
    protected override void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.name == "Player") {
            var player = col.gameObject.GetComponent<PlayerController>();
            player.updateScrap(new Dictionary<string, int>(){ { scrapType, value } });
            player.pickupItemUI(this);
            Destroy(gameObject);
        }
    }
}
