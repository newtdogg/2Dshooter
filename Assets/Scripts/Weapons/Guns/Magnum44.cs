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
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        baseStats = weaponList.Magnum44.stats;
        title = weaponList.Magnum44.title;
        description = weaponList.Magnum44.description;
        unlocked = weaponList.Magnum44.unlocked;
        cost = weaponList.Magnum44.cost;
        type = weaponList.Magnum44.type;
        ammoQuantity = baseStats.ammoCapacity;
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("DoCBullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        perkList = new List<Action<Gun>>();
    }
}
