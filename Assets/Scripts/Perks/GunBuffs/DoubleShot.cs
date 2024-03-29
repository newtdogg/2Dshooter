using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleShot : Perk {
    public override void applyGunPerk(GunParent gun) {
        StartCoroutine(delayedShot(gun));
    }

    private IEnumerator delayedShot(GunParent gun) {
        yield return new WaitForSeconds(0.2f);
        gun.fireBullet();
    }
}