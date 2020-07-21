using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Revolver44 : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json"); 
        var weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        baseStats = weaponList.Revolver44.stats;
        title = weaponList.Revolver44.title;
        description = weaponList.Revolver44.description;
        unlocked = weaponList.Revolver44.unlocked;
        cost = weaponList.Revolver44.cost;
        type = weaponList.Revolver44.type;
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
