using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : Pickup
{
    public int value;
    protected override void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.name == "Player") {
            col.gameObject.GetComponent<PlayerController>().updateScrap(value);
            Destroy(gameObject);
        }
    }
}
