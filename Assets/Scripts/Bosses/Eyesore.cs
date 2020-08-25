using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Eyesore : MiniBoss {
    private int sprayCount;
    private bool inBattle;

    void Start() {
        attacks = new List<Action>() { attackShockwave, attackDash, attackSprayBullets };
        sprayCount = 9;
        attackDelay = 4.2f;
        speed = 25;
        maxHealth = 100;
        health = maxHealth;
        canMove = true;
        inBattle = false;
        title = "MobEyeSore";
        type = "boss";
        rbody = gameObject.GetComponent<Rigidbody2D>();
        bullet = transform.GetChild(1).gameObject;
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        if(gameObject.name == $"{title}(Clone)") {
            // Debug.Log(transform.parent.parent.gameObject);
            spawner = transform.parent.parent.gameObject.GetComponent<Spawner>();
            lootController = transform.parent.parent.gameObject.GetComponent<Spawner>().lootController;
        }
    }

    public override void startFight() {
        StartCoroutine("UpdatePath");
        StartCoroutine("CycleRandomAttacks");
    }

    void Update() {
        if(gameObject.name == $"{title}(Clone)") {
            distance = Vector3.Distance(transform.position, player.transform.position);
            if(distance < spawner.height - 4 && !inBattle) {
                startFight();
                inBattle = true;
            }
        }
        if(health <= 0) {
            Destroy(gameObject);
            lootController.dropZombieLoot(transform.position);
        }
    }

    public IEnumerator attackShockwaveCoroutine() {
        foreach(Transform child in transform.GetChild(0)) {
            StartCoroutine(createShockwave(child));
            yield return new WaitForSeconds (1.4f);
        }
        yield return null;
    }

    public void attackShockwave() {
        StartCoroutine(attackShockwaveCoroutine());
    }
    public IEnumerator createShockwave(Transform shockwave) {
        while(shockwave.localScale.x < 9) {
            // shockwave.gameObject.GetComponent<Bullet>();
            yield return new WaitForSeconds (0.012f);
            shockwave.localScale = new Vector3(shockwave.localScale.x + 0.1f, shockwave.localScale.y + 0.08f, 0);
        }
        shockwave.localScale = new Vector3(1f, 0.8f, 0);
        yield return null;
    }

    public IEnumerator attackSprayBulletsCoroutine(){
        for(var i = 0; i < sprayCount; i++) {
            var angle = (360 / sprayCount) * (i + 1);
            var vectorAngle = calculateDirectionFromAngle((float)angle);
            var bulletClone = Instantiate(bullet, transform.position + new Vector3(vectorAngle.x, vectorAngle.y, 0), Quaternion.identity) as GameObject;
            bulletClone.transform.SetParent(transform);
            bulletClone.GetComponent<Rigidbody2D>().AddForce(vectorAngle * 30);
            bulletClone.GetComponent<Bullet>().setLifetime(4);
            bulletClone.GetComponent<Bullet>().parent = gameObject.name;
        }
        sprayCount += 1;
        yield return null;
    }

    public void attackSprayBullets() {
        StartCoroutine(attackSprayBulletsCoroutine());
    }

    public Vector2 calculateDirectionFromAngle(float angle) {
        if(angle > 315 || angle <= 45) {
            var a = angle > 315 ? angle - 360 : angle;
            return new Vector2(Mathf.Tan((a * Mathf.PI) / 180), 1f);
        } else if(angle > 45 && angle <= 135) {
            return new Vector2(1f, Mathf.Tan((90 - angle) * Mathf.PI / 180));
        } else if(angle > 135 && angle <= 225) {
            return new Vector2(Mathf.Tan((180 - angle) * Mathf.PI / 180), -1f);
        } else if(angle > 225 && angle <= 315) {
            return new Vector2(-1, Mathf.Tan((angle - 270) * Mathf.PI / 180));
        }
        return Vector2.zero;
    }

    public IEnumerator attackDashCoroutine() {
        canMove = false;
        Debug.Log("dash");
        var distanceVector = player.transform.position - transform.position;
        rbody.AddForce(distanceVector.normalized * 600 * rbody.mass);
        yield return new WaitForSeconds (1.3f);
        rbody.velocity = Vector3.zero;
        rbody.angularVelocity = 0f; 
        canMove = true;
        yield return null;
    }

    public void attackDash() {
        StartCoroutine(attackDashCoroutine());
    }

}
