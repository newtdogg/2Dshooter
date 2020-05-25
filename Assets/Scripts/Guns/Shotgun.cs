using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    // Start is called before the first frame update
    private int buck;
    void Start()
    {
        baseStats = new Dictionary<string, float>() {
            { "ammoCapacity", 5f },
            { "reloadSpeed", 3f },
            { "damage", 3f },
            { "shotDelay", 1.4f },
            { "spread", 300f },
            { "bulletVelocity", 70f },
            { "lifetime", 0.6f },
            { "loudness", 4f }
        };
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("Bullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        ammoQuantity = baseStats["ammoCapacity"];
        buck = 8;
    }


    public override void shootingGunCheck () {
        if(reloadTimer < 0 && shooting < 0) {
            if (Input.GetKey(KeyCode.UpArrow)) {
                shootShotgun(new Vector2(transform.position.x, transform.position.y + 2), new Vector2(0, 1));
                shootGun();
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                shootShotgun(new Vector2(transform.position.x, transform.position.y - 2), new Vector2(0, -1));
                shootGun();
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                shootShotgun(new Vector2(transform.position.x - 2, transform.position.y), new Vector2(-1, 0));
                shootGun();
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                shootShotgun(new Vector2(transform.position.x + 2, transform.position.y), new Vector2(1, 0));
                shootGun();
            }
        }
    }

    private void shootShotgun(Vector2 spawnPoint, Vector2 force) {
        for(var i = 0; i < buck; i++) {
            var spreadSection = (currentStats["spread"]/ buck) * (i + 1);
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            var spread = rand.Next(0, (int)spreadSection);
            float spreadFloat = ((float)spread - (currentStats["spread"]/2))/1000;
            var forceWithSpread = force.x == 0 ? new Vector2(spreadFloat, force.y) : new Vector2(force.x, spreadFloat);
            GameObject bullet = Instantiate(bulletObject, spawnPoint, Quaternion.identity) as GameObject;
            bullet.GetComponent<Bullet>().setLifetime(currentStats["lifetime"]);
            bullet.GetComponent<Rigidbody2D>().AddForce(forceWithSpread * currentStats["bulletVelocity"]);
        }
    }
}
