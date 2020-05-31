using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStatChange : Perk {
    public bool statsUpdated;
    public WeaponStats statsToUpdate;
    void Start() {
        statsUpdated = false;

    }
    public override void applyGunPerk(Gun gun) {
        if(!statsUpdated) {
            // foreach (var stat in statsToUpdate) {
                // var weaponStat = gun.baseStats.GetType().GetProperty(stat.Key).GetValue(gun.baseStats);
                // weaponStat += statsToUpdate;
            // }
            statsUpdated = true;
        }
    }
}