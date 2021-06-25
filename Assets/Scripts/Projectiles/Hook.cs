using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : Projectile {
    private LineRenderer line;
    private float returnTimer;
    private bool hookAttached;
    private GameObject hookAttachmentObject;
    void Start () {
        hookAttached = false;
        line = gameObject.GetComponent<LineRenderer>();
        // if(transform.parent.gameObject.name != "Bullets") {
        //     line.SetPosition(0, transform.parent.position);
        // }
        returnTimer = 0;
    }

    void Update() {
        if(transform.parent.gameObject.name != "Bullets" && gameObject.name == "DoCHook(Clone)") {
            line.SetPosition(0, transform.parent.position);
            line.SetPosition(1, transform.position);
            returnTimer += Time.deltaTime;
            if(returnTimer > lifetime || (transform.position - transform.parent.position).magnitude > 10) {
                transform.localPosition = Vector3.zero;
                returnTimer = 0;
                Destroy(gameObject);
            }
            if(hookAttached) {
                transform.position = hookAttachmentObject.transform.position;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        var colName = col.gameObject.name;
        if(colName != parent) {
            if(colName.Contains("Player")){
                transform.parent.gameObject.GetComponent<WebRat>().hookAttached = true;
                col.gameObject.GetComponent<PlayerController>().slow(2f);
                hookAttached = true;
                hookAttachmentObject = col.gameObject;
            }
            if(colName.Contains("Obstacle") || colName.Contains("Mob") || colName.Contains("DoC")){
                Destroy(gameObject);
            }
        }
    }
}
