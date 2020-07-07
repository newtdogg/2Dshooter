using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

public class TileTools : MonoBehaviour {

    public WorldTile[,] worldTileArray;
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public TileBase groundTile;
    public TileBase wallTile;

    public void setTileColor(Tilemap tilemap, float x, float y, Color color) {
        var pos = new Vector3Int((int)x, (int)y, 0);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }

    public void setWallTile(int x, int y) {
        worldTileArray[x, y] = new WorldTile(0, 0);
        worldTileArray[x, y].walkable = false;
        wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
    }

    public void setGroundTile(int x, int y, bool walkable) {
        worldTileArray[x, y] = new WorldTile(1, 0);
        worldTileArray[x, y].walkable = walkable;
        groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
    }

    public void removeWallTile(int x, int y) {
        worldTileArray[x, y].walkable = true;
        wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
    }
}