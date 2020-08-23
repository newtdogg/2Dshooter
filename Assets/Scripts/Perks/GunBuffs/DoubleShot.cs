using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleShot : Perk {
    public override void applyGunPerk(Gun gun) {
        StartCoroutine(delayedShot(gun));
    }

    private IEnumerator delayedShot(Gun gun) {
        yield return new WaitForSeconds(0.2f);
        gun.fireBullet();
    }
}