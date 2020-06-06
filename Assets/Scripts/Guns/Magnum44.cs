using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Magnum44 : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        baseStats = weaponList.Magnum44.stats;
        title = weaponList.Magnum44.title;
        description = weaponList.Magnum44.description;
        unlocked = weaponList.Magnum44.unlocked;
        attachments = weaponList.Magnum44.attachments;
        cost = weaponList.Magnum44.cost;
        type = weaponList.Magnum44.type;
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
    }
}
