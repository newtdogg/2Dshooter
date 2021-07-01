using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;


public class GunParent : MonoBehaviour {

    public string title;
	public string description;
	public bool unlocked;
	public Dictionary<string, int> cost;
	public string group;
    public int xpCost;
    public float shooting;
    public float ammoQuantity;
    public float reloadTimer;
    public WeaponStats baseStats;
    public WeaponStats statsBaseState;
    public WeaponStats currentStats;
    public List<Action<GunParent>> perkList;
    public string ammoType;
    public GameObject reloadBar;
    public GameObject bulletObject;
    public Dictionary<string, bool> bulletProperties;
    public GameObject activeBullet;
    public PlayerController playerController;
    public Vector3 bulletPosition;
    public Vector2 bulletDirection;
    public Transform gunUI;
    public Text ammoCountUI;
    public Text clipSizeUI;
    public Text gunNameUI;
    public void defaultGunAwake(string weaponName) {
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json"); 
        var weaponsList = JsonConvert.DeserializeObject<Weapons>(jsonString);
        var weaponJsonObject = weaponsList.GetType().GetProperty(weaponName).GetValue(weaponsList, null) as Weapon;
        baseStats = weaponJsonObject.stats;
        title = weaponJsonObject.title;
        description = weaponJsonObject.description;
        unlocked = weaponJsonObject.unlocked;
        cost = weaponJsonObject.cost;
        xpCost = weaponJsonObject.xpCost;
        ammoQuantity = baseStats.ammoCapacity;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("DoCPlayerBullet");
        group = weaponJsonObject.group;
        reloadTimer = -1;
        shooting = -1f;
        perkList = new List<Action<GunParent>>();
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

    public virtual void reloadMagazine () {
        transform.GetChild(1).gameObject.SetActive(false);
        ammoCountUI.text = ammoQuantity.ToString();
    }

    public void directionallyShootGun(Vector2 bPos, Vector2 bDir) {
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
        playerController.setSneakStat("detectionDistance", playerController.getSneakStat("detectionDistance") + (currentStats.loudness ));
        playerController.setSneakStat("attackDistance", playerController.getSneakStat("attackDistance") + currentStats.loudness);
        shooting = currentStats.shotDelay;
        ammoQuantity -= 1;
        ammoCountUI.text = ammoQuantity.ToString();
        if(ammoQuantity == 0) {
            reload();
        }
        currentStats = statsBaseState.duplicateStats();
    }

    public virtual void fireBullet () {
        GameObject bullet = Instantiate(bulletObject, playerController.transform.position + bulletPosition, Quaternion.identity) as GameObject;
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.properties = bulletProperties;
        bulletScript.damage = currentStats.damage;
        // Debug.Log(bulletScript.properties["poison"]);
        bullet.GetComponent<Rigidbody2D>().AddForce(bulletDirection * currentStats.bulletVelocity * playerController.gameController.globalSpeed);
        bulletScript.setLifetime(currentStats.lifetime);
        bulletScript.parent = gameObject.name;
    }

    public virtual void reload () {
        transform.GetChild(1).gameObject.SetActive(true);
        reloadTimer = currentStats.reloadSpeed;
        ammoQuantity = currentStats.ammoCapacity;
    }

}