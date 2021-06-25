using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;

public class WorldTile : IHeapItem<WorldTile> {
    public string bedrockName;
    public Dictionary<string, int> tileCover;
    public bool hasMob;
    public Vector2 worldPosition;
    public WorldTile parent;
    public bool walkable;
    public int gCost;
    public int hCost;
    private int hI;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int heapIndex {
        get {
            return hI;
        }
        set {
            hI = value;
        }
    }

    public WorldTile(int pg, int sg) {
        walkable = true;
        tileCover = new Dictionary<string, int>(){
            { "primaryGrass", pg },
            { "secondaryGrass", sg },
            { "trees", 0 }
        };
    }

    public int CompareTo(WorldTile tileToCompare) {
		int compare = fCost.CompareTo(tileToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(tileToCompare.hCost);
		}
		return -compare;
	}
}
