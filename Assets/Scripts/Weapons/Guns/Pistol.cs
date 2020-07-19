using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Pistol : Gun
{
    // Start is called before the first frame update
    void Awake()
    {
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        baseStats = weaponList.Pistol.stats;
        title = weaponList.Pistol.title;
        description = weaponList.Pistol.description;
        unlocked = weaponList.Pistol.unlocked;
        cost = weaponList.Pistol.cost;
        type = weaponList.Pistol.type;
        ammoQuantity = baseStats.ammoCapacity;
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("DoCBullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        perkList = new List<Action<Gun>>();
        statsBaseState = baseStats.duplicateStats();
        currentStats = baseStats.duplicateStats();
        reloadMagazine();
    }

}
