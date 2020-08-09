using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : Projectile {
    private LineRenderer line;
    private float returnTimer;
    void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, transform.parent.position);
        returnTimer = 0;
    }

    void Update() {
        line.SetPosition(0, transform.parent.position);
        line.SetPosition(1, transform.position);
        if(gameObject.name == "DoCHook(Clone)") {
            returnTimer += Time.deltaTime;
            if(returnTimer > lifetime || (transform.position - transform.parent.position).magnitude > 5) {
                transform.localPosition = Vector3.zero;
                returnTimer = 0;
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.name != parent) {
            if(col.gameObject.name.Contains("Player")){
                transform.parent.gameObject.GetComponent<Slinger>().hookAttached = true;
            }
        }

    }
}
