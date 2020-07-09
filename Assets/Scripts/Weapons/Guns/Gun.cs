using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour {

    public string title;
	public string description;
	public bool unlocked;
	public int cost;
	public string type;
    public float shooting;
    public float ammoQuantity;

    public float reloadTimer;
    public WeaponStats baseStats;
    public WeaponStats statsBaseState;
    public WeaponStats currentStats;
    public GameObject bulletDisplay;
    public List<Action<Gun>> perkList;
    private GameObject reloadBulletObject;
    public string ammoType;
    public GameObject ammoClone;
    public GameObject reloadBar;
    public GameObject bulletObject;
    public Dictionary<string, bool> bulletProperties;
    public GameObject activeBullet;
    private PlayerController playerController;
    public Vector2 bulletPosition;
    public Vector2 bulletDirection;

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
    public virtual void reloadMagazine () {
        // Debug.Log("reload");
        for (int i = 0; i < currentStats.ammoCapacity; i++) {
            var ammoBullet = Instantiate(ammoClone, new Vector2(2, 0), Quaternion.identity);
            ammoBullet.transform.SetParent(bulletDisplay.transform);
            ammoBullet.transform.position = new Vector3(bulletDisplay.transform.position.x + 0.7f, bulletDisplay.transform.position.y + i/7f, 0);
        }
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public virtual void shootingGunCheck () {
        if(reloadTimer < 0 && shooting < 0) {
            var rand = new System.Random((int)System.DateTime.Now.Ticks);
            var spread = rand.Next(0, (int)currentStats.spread);
            float spreadFloat = ((float)spread - currentStats.spread/2)/1000;
            
            if (Input.GetKey(KeyCode.UpArrow)) {
                directionallyShootGun(new Vector2(transform.position.x, transform.position.y + 2), new Vector2(spreadFloat, 1));
            } else if (Input.GetKey(KeyCode.DownArrow)) {
                directionallyShootGun(new Vector2(transform.position.x, transform.position.y - 2), new Vector2(spreadFloat, -1));
            } else if (Input.GetKey(KeyCode.LeftArrow)) {
                directionallyShootGun(new Vector2(transform.position.x - 2, transform.position.y), new Vector2(-1, spreadFloat));
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                directionallyShootGun(new Vector2(transform.position.x + 2, transform.position.y), new Vector2(1, spreadFloat));
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

    public virtual void shootGun () {
        playerController.resetSneakStats();
        playerController.setSneakStat("detectionDistance", playerController.getSneakStat("detectionDistance") + (currentStats.loudness * 2));
        playerController.setSneakStat("attackDistance", playerController.getSneakStat("attackDistance") + currentStats.loudness);
        shooting = currentStats.shotDelay;
        ammoQuantity -= 1;
        Destroy(bulletDisplay.transform.GetChild(bulletDisplay.transform.childCount - 1).gameObject);
        if(ammoQuantity == 0) {
            reload();
        }
        currentStats = statsBaseState.duplicateStats();
    }

    public void fireBullet () {
        GameObject bullet = Instantiate(bulletObject, bulletPosition, Quaternion.identity) as GameObject;
        bullet.GetComponent<Bullet>().properties = bulletProperties;
        // Debug.Log(bullet.GetComponent<Bullet>().properties["poison"]);
        bullet.GetComponent<Rigidbody2D>().AddForce(bulletDirection * currentStats.bulletVelocity);
        bullet.GetComponent<Bullet>().setLifetime(currentStats.lifetime);
    }

    public virtual void reload () {
        transform.GetChild(1).gameObject.SetActive(true);
        reloadTimer = currentStats.reloadSpeed;
        ammoQuantity = currentStats.ammoCapacity;
        // transform.localScale = new Vector3(0.001f, 0.1f, 0f);
        // transform.localScale = Vector3.MoveTowards (transform.localScale, new Vector3(1f, 0.1f, 0f), reloadSpeed * Time.deltaTime);
    }

    public void setPlayerController(PlayerController pc) {
        playerController = pc;
    }
}