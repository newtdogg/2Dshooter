using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastShot : Perk {
    
    public override void applyGunPerk(GunParent gun) {
        if(gun.ammoQuantity == 1) {
            gun.currentStats.damage *= 2;
        }
    }


}
