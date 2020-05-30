using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStatChange : Perk {
    public bool statsUpdated;
    public Dictionary<string, float> statsToUpdate;
    public Dictionary<string, Dictionary<string, float>> statUpdateTypes;
    void Start() {
        statsUpdated = false;
        statUpdateTypes = new Dictionary<string, Dictionary<string, float>>();
        statUpdateTypes.Add("")

    }
    public override void applyGunPerk(Gun gun) {
        if(!statsUpdated) {
            foreach (var stat in statsToUpdate) {
                gun.baseStats[stat.Key] += statsToUpdate;
            }
            statsUpdated = true;
        }
    }
}