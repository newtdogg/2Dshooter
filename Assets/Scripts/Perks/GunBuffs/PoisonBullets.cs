using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBullets : Perk {
    public int chance;

    void Start() {
        chance = 5;
    }
    public override void applyGunPerk(Gun gun) {
        var rand = new System.Random((int)System.DateTime.Now.Ticks);
        if(rand.Next(0, chance) == 0) {
            gun.bulletProperties["poison"] = true;
        }
    }
}