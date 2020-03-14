using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : Gun
{
       // Start is called before the first frame update
    void Start()
    {
        stats = new Dictionary<string, float>() {
            { "ammoCapacity", 1f },
            { "reloadSpeed", 1.7f },
            { "damage", 14f },
            { "shotDelay", 1f },
            { "spread", 70f },
            { "bulletVelocity", 170f },
            { "lifetime", 1.4f },
            { "loudness", 2f }
        };
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("Bolt");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        ammoQuantity = stats["ammoCapacity"];
    }
}