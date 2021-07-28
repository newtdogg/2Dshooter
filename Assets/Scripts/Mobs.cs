using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mobs {
    public MobStats mobRedJacket;
    public MobStats mobSlugopod;
    public MobStats mobChucker;
    public MobStats mobWebRat;
    public MobStats MobRedJacket { get { return mobRedJacket; } }
    public MobStats MobSlugopod { get { return mobSlugopod; } }
    public MobStats MobChucker { get { return mobChucker; } }
    public MobStats MobWebRat { get { return mobWebRat; } }
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