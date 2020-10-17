using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Zombies {
    public ZombieStats mobRipper;
    public ZombieStats mobLubber;
    public ZombieStats mobChucker;
    public ZombieStats mobSlinger;
    public ZombieStats MobRipper { get { return mobRipper; } }
    public ZombieStats MobLubber { get { return mobLubber; } }
    public ZombieStats MobChucker { get { return mobChucker; } }
    public ZombieStats MobSlinger { get { return mobSlinger; } }
}

[System.Serializable]
public class ZombieStats {
    public int speed;
    public int maxHealth;
    public int damage;
    public int xpValue;
}