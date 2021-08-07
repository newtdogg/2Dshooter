using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mobs {
    public MobStats mobRedJacket;
    public MobStats mobSlugopod;
    public MobStats mobChucker;
    public MobStats mobWebRat;
    public MobStats MobRedJacket => mobRedJacket;
    public MobStats MobSlugopod => mobSlugopod;
    public MobStats MobChucker => mobChucker;
    public MobStats MobWebRat => mobWebRat;
}

[System.Serializable]
public class MobStats {
    public int speed;
    public int maxHealth;
    public int damage;
    public int xpValue;
    public List<SpawnerType> spawnerTypes;
    public Dictionary<string, List<string>> scrapLoot;
}