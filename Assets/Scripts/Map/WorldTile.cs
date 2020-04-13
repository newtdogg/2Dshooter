using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;

public class WorldTile {
    public string bedrockName;

    public Dictionary<string, int> tileCover;
    public WorldTile(int pg, int sg) {
        tileCover = new Dictionary<string, int>(){
            { "primaryGrass", pg },
            { "secondaryGrass", sg },
            { "trees", 0 }
        };
    }
}
