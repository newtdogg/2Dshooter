using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnum44 : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        stats = new Dictionary<string, float>() {
            { "ammoCapacity", 6f },
            { "reloadSpeed", 2.2f },
            { "damage", 14f },
            { "shotDelay", 0.8f },
            { "spread", 90f },
            { "bulletVelocity", 160f },
            { "lifetime", 1.4f },
            { "loudness", 7f }
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
