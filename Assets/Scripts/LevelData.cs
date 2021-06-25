using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Levels {
    public Level temperate;
    
}

public class Level {
    public List<Spawn> spawns;
    public List<string> bosses;

}

public class Spawn {
    public List<string> enemies;
    public List<SpawnerType> spawnerTypes;
}