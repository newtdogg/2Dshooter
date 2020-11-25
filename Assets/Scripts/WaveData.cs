using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave {
    public int index;
    public List<ZombieGroup> groups;
    
}

[System.Serializable]
public class WaveData {
    public List<Wave> waves;
}

[System.Serializable]
public class ZombieGroup {
    public string type;
    public int quantity;

}