using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {
    // Start is called before the first frame update
    public string type;

    protected virtual void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.name == "Player") {
           pickupItem(col.gameObject);
        }
    }

    public virtual void pickupItem(GameObject player) {
        foreach (Transform child in player.transform) {
            if (child.name == type) {
                transform.SetParent(child);
                gameObject.SetActive(false);
            }
        }
    }
}
