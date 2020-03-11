using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public float shooting;
    public float ammoQuantity;

    public float reloadTimer;
    private Dictionary<string, float> stats;

    private GameObject bulletDisplay;
    private GameObject reloadBulletObject;
    private GameObject ammoClone;
    private GameObject reloadBar;
    private GameObject bulletObject;
    void Start() {
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("Bullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        // shotDelay = 0.1f;
        shooting = -1f;
        stats = new Dictionary<string, float>() {
            { "ammoCapacity", 6f },
            { "reloadSpeed", 2f },
            { "damage", 8f },
            { "shotDelay", 0.2f }
        };
        ammoQuantity = stats["ammoCapacity"];
    }

    void Update() {
        if(reloadTimer > 0) {
            reloadTimer -= Time.deltaTime;
            if(reloadTimer <= 0) {
                reloadTimer = -1;
                reloadMagazine();
            }
        }
        if(shooting > 0) {
            shooting -= Time.deltaTime;
            if(shooting <= 0) {
                shooting = -1;
            }
        }
    }
    public void reloadMagazine () {
        for (int i = 0; i < stats["ammoCapacity"]; i++) {
            var ammoBullet = Instantiate(ammoClone, new Vector2(2, 0), Quaternion.identity);
            ammoBullet.transform.SetParent(bulletDisplay.transform);
            ammoBullet.transform.position = new Vector3(bulletDisplay.transform.position.x + 0.7f, bulletDisplay.transform.position.y + i/7f, 0);
        }
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void directionallyShootGun () {
        if(reloadTimer < 0 && shooting < 0) {
            if (Input.GetKey(KeyCode.UpArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x, transform.position.y + 2), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1) * 100);
                shootGun();
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x, transform.position.y - 2), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1) * 100);
                shootGun();
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x - 2, transform.position.y), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0) * 100);
                shootGun();
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                GameObject bullet = Instantiate(bulletObject, new Vector2(transform.position.x + 2, transform.position.y), Quaternion.identity) as GameObject;
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * 100);
                shootGun();
            }
        }
    }

    public void shootGun () {
        shooting = stats["shotDelay"];
        ammoQuantity -= 1;
        Destroy(bulletDisplay.transform.GetChild(bulletDisplay.transform.childCount - 1).gameObject);
        if(ammoQuantity == 0) {
            reload();
        }
    }

    public void reload () {
        transform.GetChild(1).gameObject.SetActive(true);
        reloadTimer = stats["reloadSpeed"];
        ammoQuantity = stats["ammoCapacity"];
        // transform.localScale = new Vector3(0.001f, 0.1f, 0f);
        // transform.localScale = Vector3.MoveTowards (transform.localScale, new Vector3(1f, 0.1f, 0f), reloadSpeed * Time.deltaTime);
        // 
    }

    public float getStat(string key) {
        return stats[key];
    }

    public void setStat(string key, float value) {
        stats[key] = value;
    }
}