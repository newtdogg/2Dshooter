using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour {
    void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.name.Contains("DoCPlayerBullet")) {
            Destroy(col.gameObject);
        }
    }
}
