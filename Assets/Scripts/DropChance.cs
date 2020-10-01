using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropChanceList {
	public DropChance arena;
    public DropChance miniboss;
    public DropChance mobRipper;
    public DropChance mobLubber;
    public DropChance mobChucker;
    public DropChance mobSlinger;
	public DropChance Arena { get { return arena; } }
    public DropChance Miniboss { get { return miniboss; } }
    public DropChance MobRipper { get { return mobRipper; } }
    public DropChance MobLubber { get { return mobLubber; } }
    public DropChance MobChucker { get { return mobChucker; } }
    public DropChance MobSlinger { get { return mobSlinger; } }
}

[System.Serializable]
public class DropChance {
    public int scrapMin;
    public int scrapMax;
    public int scrapValue;
    public int recipeDropChance;
}