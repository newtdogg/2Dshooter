using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class UnlocksController : MonoBehaviour {

    private GameObject treeNodeParents;
    public PlayerController playerController;
    public Weapons weaponList;
    public Text xp;

    void Start() {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        treeNodeParents = transform.GetChild(0).gameObject;
        xp = transform.parent.parent.parent.GetChild(2).GetChild(1).gameObject.GetComponent<Text>();
        xp.text = playerController.experienceSpendable.ToString();
        initialiseWeaponNodes();
    }

    public void initialiseWeaponNodes(int selectGroup = 0) {
        var parentCount = 0;
        var count = 0;
        foreach (var weapon in typeof(Weapons).GetProperties()) {
            if(transform.GetChild(parentCount).GetChild(0).childCount <= count) {
                parentCount++;
                count = 0;
            }     
            var newWeapon = weapon.GetValue(weaponList) as Weapon;
            // Debug.Log($"{transform.GetChild(parentCount).GetChild(0).childCount}, {count}, {newWeapon.title}");
            if(newWeapon.requiredUnlocks != "") {
                setWeaponChildren(newWeapon);
            }
            transform.GetChild(parentCount).GetChild(0).GetChild(count).gameObject.GetComponent<WeaponNode>().setupNode(newWeapon, parentCount + 1);
            count++;
        }
        if(selectGroup != 0) {
            transform.GetChild(selectGroup - 1).GetChild(1).gameObject.GetComponent<UnlocksOverlay>().selectGunGroup();
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

    public void updatePlayerWeapons() {
        playerController.weaponsList = weaponList;
    }

    
}