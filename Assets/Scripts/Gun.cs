using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour {
    public float shooting;
    public float ammoQuantity;

    public float reloadTimer;
    public Dictionary<string, float> stats;

    public GameObject bulletDisplay;
    private GameObject reloadBulletObject;
    public string ammoType;
    public GameObject ammoClone;
    public GameObject reloadBar;
    public GameObject bulletObject;
    private PlayerController playerController;
    private GameObject boltObject;
    void Start() {
        reloadTimer = -1f;
        shooting = -1f;
        ammoQuantity = stats["ammoCapacity"];
    }

    void Update() {
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
    public virtual void reloadMagazine () {
        for (int i = 0; i < stats["ammoCapacity"]; i++) {
            var ammoBullet = Instantiate(ammoClone, new Vector2(2, 0), Quaternion.identity);
            ammoBullet.transform.SetParent(bulletDisplay.transform);
            ammoBullet.transform.position = new Vector3(bulletDisplay.transform.position.x + 0.7f, bulletDisplay.transform.position.y + i/7f, 0);
        }
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public virtual void directionallyShootGun () {
        if(reloadTimer < 0 && shooting < 0) {
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            var spread = rand.Next(0, (int)stats["spread"]);
            float spreadFloat = ((float)spread - stats["spread"]/2)/1000;
            if (Input.GetKey(KeyCode.UpArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x, transform.position.y + 2), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(spreadFloat, 1) * stats["bulletVelocity"]);
                bullet.GetComponent<Bullet>().setLifetime(stats["lifetime"]);
                shootGun();
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x, transform.position.y - 2), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(spreadFloat, -1) * stats["bulletVelocity"]);
                bullet.GetComponent<Bullet>().setLifetime(stats["lifetime"]);
                shootGun();
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x - 2, transform.position.y), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, spreadFloat) * stats["bulletVelocity"]);
                bullet.GetComponent<Bullet>().setLifetime(stats["lifetime"]);
                shootGun();
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x + 2, transform.position.y), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, spreadFloat) * stats["bulletVelocity"]);
                bullet.GetComponent<Bullet>().setLifetime(stats["lifetime"]);
                shootGun();
            }
        }
    }

    public virtual void shootGun () {
        playerController.resetSneakStats();
        playerController.setSneakStat("detectionDistance", playerController.getSneakStat("detectionDistance") + (stats["loudness"] * 2));
        playerController.setSneakStat("attackDistance", playerController.getSneakStat("attackDistance") + stats["loudness"]);
        shooting = stats["shotDelay"];
        ammoQuantity -= 1;
        Destroy(bulletDisplay.transform.GetChild(bulletDisplay.transform.childCount - 1).gameObject);
        if(ammoQuantity == 0) {
            reload();
        }
    }

    public virtual void reload () {
        transform.GetChild(1).gameObject.SetActive(true);
        reloadTimer = stats["reloadSpeed"];
        ammoQuantity = stats["ammoCapacity"];
        // transform.localScale = new Vector3(0.001f, 0.1f, 0f);
        // transform.localScale = Vector3.MoveTowards (transform.localScale, new Vector3(1f, 0.1f, 0f), reloadSpeed * Time.deltaTime);
        // 
    }

    public virtual float getStat(string key) {
        return stats[key];
    }

    public virtual void setStat(string key, float value) {
        stats[key] = value;
    }

    public void setPlayerController(PlayerController pc) {
        playerController = pc;
    }
}