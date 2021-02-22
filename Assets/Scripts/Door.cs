using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public Action trigger;
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.name == "Player") {
            trigger();
        }
    }
}
