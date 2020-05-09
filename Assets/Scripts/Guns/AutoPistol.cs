using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPistol : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        stats = new Dictionary<string, float>() {
            { "ammoCapacity", 17f },
            { "reloadSpeed", 1.5f },
            { "damage", 5.8f },
            { "shotDelay", 0.15f },
            { "spread", 110f },
            { "bulletVelocity", 110f },
            { "lifetime", 1.2f },
            { "loudness", 5f }
        };
        bulletDisplay = transform.GetChild(0).gameObject;
        reloadBar = transform.GetChild(1).gameObject;
        bulletObject = GameObject.Find("Bullet");
        ammoClone = GameObject.Find("Ammo");
        reloadTimer = -1;
        shooting = -1f;
        ammoQuantity = stats["ammoCapacity"];
    }
}
