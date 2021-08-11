using System.Collections.Generic;
using UnityEngine;
using System;

public class Slugopod : MobController {
    private Transform weakSpot;
    // Start is called before the first frame update
    void Start() {
        weakSpot = transform.GetChild(3);
        defaultMobAwake("MobSlugopod");
        attacks = new List<Action>() { lungeAtPlayer };
        attackDelay = 2.2f;
    }

    void FixedUpdate() {
        distance = Vector3.Distance(transform.position, player.transform.position);
        manageBehaviourState();
        defaultUpdate();
        positionWeakSpot();
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.name == "Player") {
            inContactWithPlayer = true;
            playerController.canMove = false;
            contactController.updateBrawlStatus(gameObject);
            StopCoroutine("FollowPath");
            StopCoroutine("CycleRandomAttacks");
        }
        if (col.gameObject.name.Contains("Mob")) {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
        if (col.gameObject.name.Contains("DoCPlayerBullet")) {
            var projectileScript = col.gameObject.GetComponent<Projectile>();
            if(!projectileScript.hitTarget) {
                updateDamage(projectileScript.damage/2, "shielded");
            }
        }
    }

    private void positionWeakSpot() {
        weakSpot.localPosition = new Vector3(compassFacingDirection.x * 0.45f, compassFacingDirection.y * 0.45f, 0);
    }

    private void lungeAtPlayer() {
        StartCoroutine(lunge(2.1f, 6));
    }
}
