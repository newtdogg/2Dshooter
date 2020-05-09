using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    // Start is called before the first frame update
    void Start()
    {
        stats = new Dictionary<string, float>() {
            { "ammoCapacity", 8f },
            { "reloadSpeed", 1.2f },
            { "damage", 7f },
            { "shotDelay", 0.6f },
            { "spread", 110f },
            { "bulletVelocity", 130f },
            { "lifetime", 1.1f },
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

    // Update is called once per frame
}
