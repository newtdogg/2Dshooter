using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using System;

public class BuffObject : Pickup {

    void Start()
    {
        value = 1;
        type = "Buff";
    }

    public override void pickupItem(GameObject player) {
        foreach (Transform child in player.transform) {
            if (child.name.Contains(type)) {
                transform.SetParent(child);
                player.transform.GetChild(0).gameObject.GetComponent<GunParent>().perkList.Add(gameObject.GetComponent<Perk>().applyGunPerk);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}
