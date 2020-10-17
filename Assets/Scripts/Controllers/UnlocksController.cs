using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class UnlocksController : MonoBehaviour {

    private GameObject treeNodeParents;
    private Weapons weaponList;
    void Start() {
        treeNodeParents = transform.GetChild(0).gameObject;
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json");
        weaponList = JsonUtility.FromJson<Weapons>(jsonString);
        var parentCount = 0;
        var count = 0;
        foreach (var weapon in typeof(Weapons).GetProperties()) {
            if(transform.GetChild(parentCount).GetChild(0).childCount <= count) {
                parentCount += 1;
                count = 0;
            }     
            var newWeapon = weapon.GetValue(weaponList) as Weapon;
            Debug.Log($"{transform.GetChild(parentCount).GetChild(0).childCount}, {count}, {newWeapon.title}");
            if(newWeapon.requiredUnlocks != "") {
                setWeaponChildren(newWeapon);
            }
            transform.GetChild(parentCount).GetChild(0).GetChild(count).gameObject.GetComponent<WeaponNode>().setupNode(newWeapon);
            count += 1;
        }
    }

    public void setWeaponChildren(Weapon weapon) {
        weapon.requiredUnlocksList = new List<Weapon>();
        string[] gunStrings = weapon.requiredUnlocks.Split(',');
        foreach (var gunStr in gunStrings) {
            var capGunStr = char.ToUpper(gunStr[0]) + gunStr.Substring(1);
            var gunObject = weaponList.GetType().GetProperty(capGunStr).GetValue(weaponList, null) as Weapon;
            weapon.requiredUnlocksList.Add(gunObject);
        }
    }
    
}