using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AutoPistol : Gun
{
    // Start is called before the first frame update
    void Start()
    {
         var jsonString = File.ReadAllText("./Assets/Scripts/Weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        baseStats = weaponList.AutoPistol.stats;
        title = weaponList.AutoPistol.title;
        description = weaponList.AutoPistol.description;
        unlocked = weaponList.AutoPistol.unlocked;
        cost = weaponList.AutoPistol.cost;
        type = weaponList.AutoPistol.type;
        ammoQuantity = baseStats.ammoCapacity;
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("Bullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        perkList = new List<Action<Gun>>();
        statsBaseState = baseStats.duplicateStats();
        currentStats = baseStats.duplicateStats();
        reloadMagazine();
    }
}
