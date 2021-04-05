using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GunStatChange : Perk {
    public bool statsUpdated;
    public WeaponStats statsToUpdate;
    void Start() {
        statsUpdated = false;
        var jsonString = File.ReadAllText("./Assets/Scripts/Weapons/weapons.json");
        var weaponsList = JsonUtility.FromJson<Weapons>(jsonString);
        // statsToUpdate = weaponsList.Pistol.attachments[0].stats;
    }

    public override void applyGunPerk(GunParent gun) {
        if(!statsUpdated) {
            Debug.Log("applyStatChange");
            gun.statsBaseState.ammoCapacity += statsToUpdate.ammoCapacity;
            gun.statsBaseState.reloadSpeed += statsToUpdate.reloadSpeed;
            gun.statsBaseState.damage += statsToUpdate.damage;
            gun.statsBaseState.shotDelay += statsToUpdate.shotDelay;
            gun.statsBaseState.spread += statsToUpdate.spread;
            gun.statsBaseState.bulletVelocity += statsToUpdate.bulletVelocity;
            gun.statsBaseState.lifetime += statsToUpdate.lifetime;
            gun.statsBaseState.loudness += statsToUpdate.loudness;
            statsUpdated = true;
        }
    }
}