using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropChanceList {
	public DropChance arena;
    public DropChance miniboss;
    public DropChance mobRedJacket;
    public DropChance mobLubber;
    public DropChance mobChucker;
    public DropChance mobWebRat;
	public DropChance Arena { get { return arena; } }
    public DropChance Miniboss { get { return miniboss; } }
    public DropChance MobRedJacket { get { return mobRedJacket; } }
    public DropChance MobLubber { get { return mobLubber; } }
    public DropChance MobChucker { get { return mobChucker; } }
    public DropChance MobWebRat { get { return mobWebRat; } }
}

[System.Serializable]
public class DropChance {
    public int scrapMin;
    public int scrapMax;
    public int scrapValue;
    public int recipeDropChance;

    public int ScrapMin { get { return scrapMin; } }
    public int ScrapMax { get { return scrapMax; } }
    public int ScrapValue { get { return scrapValue; } }
    public int RecipeDropChance  { get { return recipeDropChance; } }
}