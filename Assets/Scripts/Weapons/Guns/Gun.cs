using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

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
    public List<Action<Gun>> perkList;
    public string ammoType;
    public GameObject reloadBar;
    public GameObject bulletObject;
    public Dictionary<string, bool> bulletProperties;
    public GameObject activeBullet;
    private PlayerController playerController;
    public Vector2 bulletPosition;
    public Vector2 bulletDirection;
    private Transform gunUI;
    private Text ammoCountUI;
    private Text clipSizeUI;
    private Text gunNameUI;

    public void defaultGunAwake(string weaponName) {
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        var weaponJsonObject = weaponList.GetType().GetProperty(weaponName).GetValue(weaponList, null) as Weapon;
        baseStats = weaponJsonObject.stats;
        title = weaponJsonObject.title;
        description = weaponJsonObject.description;
        unlocked = weaponJsonObject.unlocked;
        cost = weaponJsonObject.cost;
        type = weaponJsonObject.type;
        ammoQuantity = baseStats.ammoCapacity;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("DoCBullet");
        reloadTimer = -1;
        shooting = -1f;
        perkList = new List<Action<Gun>>();
        statsBaseState = baseStats.duplicateStats();
        currentStats = baseStats.duplicateStats();
        gunUI = transform.GetChild(3);
        ammoCountUI = gunUI.GetChild(0).GetChild(1).gameObject.GetComponent<Text>();
        clipSizeUI = gunUI.GetChild(0).GetChild(2).gameObject.GetComponent<Text>();
        gunNameUI = gunUI.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
        ammoCountUI.text = baseStats.ammoCapacity.ToString();
        clipSizeUI.text = baseStats.ammoCapacity.ToString();
        gunNameUI.text = title;
        reloadMagazine();

    }

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
        transform.GetChild(1).gameObject.SetActive(false);
        ammoCountUI.text = ammoQuantity.ToString();
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
        ammoCountUI.text = ammoQuantity.ToString();
        if(ammoQuantity == 0) {
            reload();
        }
        currentStats = statsBaseState.duplicateStats();
    }

    public void fireBullet () {
        GameObject bullet = Instantiate(bulletObject, bulletPosition, Quaternion.identity) as GameObject;
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.properties = bulletProperties;
        // Debug.Log(bulletScript.properties["poison"]);
        bullet.GetComponent<Rigidbody2D>().AddForce(bulletDirection * currentStats.bulletVelocity);
        bulletScript.setLifetime(currentStats.lifetime);
        bulletScript.parent = gameObject.name;
    }

    public virtual void reload () {
        transform.GetChild(1).gameObject.SetActive(true);
        reloadTimer = currentStats.reloadSpeed;
        ammoQuantity = currentStats.ammoCapacity;
    }

    public void setPlayerController(PlayerController pc) {
        playerController = pc;
    }
}