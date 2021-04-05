using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public abstract class Gun : GunParent {

    void Update() {
        if(playerController == null) {
            playerController = transform.parent.GetComponent<PlayerController>();
        }
        if(reloadTimer > 0f) {
            reloadTimer -= Time.deltaTime;
            if(reloadTimer <= 0f) {
                reloadTimer = -1f;
                reloadMagazine();
            }
        }
        if(shooting > 0f) {
            shooting -= Time.deltaTime;
            if(shooting <= 0f) {
                playerController.resetSneakStats();
                shooting = -1f;
            }
        }
    }

    public virtual void shootingGunCheck () {
        if(reloadTimer < 0 && shooting < 0) {
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            var spread = rand.Next(0, (int)currentStats.spread);
            float spreadFloat = ((float)spread - currentStats.spread/2)/1000;
            
            if (Input.GetKey(KeyCode.UpArrow)) {
                directionallyShootGun(new Vector3(0f, 0.6f, 0f), new Vector2(spreadFloat, 1));
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                directionallyShootGun(new Vector3(0f, -0.6f, 0f), new Vector2(spreadFloat, -1));
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                directionallyShootGun(new Vector3(-0.6f, 0f, 0f), new Vector2(-1, spreadFloat));
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                directionallyShootGun(new Vector3(0.6f, 0f, 0f), new Vector2(1, spreadFloat));
            }
        }
    }

    private void directionallyShootGun(Vector2 bPos, Vector2 bDir) {
        bulletProperties = new Dictionary<string, bool>();
        foreach(var property in bulletObject.GetComponent<Bullet>().defaultProperties) {
            bulletProperties.Add(property.Key, property.Value);
        }
        bulletPosition = bPos;
        bulletDirection = bDir;
        foreach(var perk in perkList) {
            perk(this);
        }
        fireBullet();
        shootGun();
    }


    public void setPlayerController(PlayerController pc) {
        playerController = pc;
    }
}