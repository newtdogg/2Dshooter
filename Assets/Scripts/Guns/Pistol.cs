using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Pistol : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        baseStats = weaponList.Pistol.stats;
        title = weaponList.Pistol.title;
        description = weaponList.Pistol.description;
        unlocked = weaponList.Pistol.unlocked;
        attachments = weaponList.Pistol.attachments;
        cost = weaponList.Pistol.cost;
        type = weaponList.Pistol.type;
        ammoQuantity = baseStats.ammoCapacity;
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("Bullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        gunPerks = new List<Action<Gun>>();
        var perks = GameObject.Find("Perks").transform;
        foreach(Transform perk in perks) {
            gunPerks.Add(perk.gameObject.GetComponent<Perk>().applyGunPerk);
        };
        statsBaseState = baseStats.duplicateStats();
        currentStats = baseStats.duplicateStats();
        reloadMagazine();
    }

}
