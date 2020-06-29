using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class DebugMap : MapGenerator
{

    void Start() {
        var intMap = IntroMap.map;
        var h = intMap.GetLength(0);
        var w = intMap.GetLength(1);
        // setCameraBoundary(w, h);
        shop = GameObject.Find("Shop");
        craftingStation = GameObject.Find("CraftingStation");
        var worldTileMap = new WorldTile[h, w];
        var mapObj = GameObject.Find("Map");
        var gTilemap = mapObj.transform.GetChild(3).gameObject.GetComponent<Tilemap>();
        var tTilemap = mapObj.transform.GetChild(0).gameObject.GetComponent<Tilemap>();
        var grass = tTilemap.GetTile(new Vector3Int(1, 0, 0));
        var water = tTilemap.GetTile(new Vector3Int(0, 0, 0));
        for(var y = 0; y < h -1; y++) {
            for(var x = 0; x < w -1; x++) {
                worldTileMap[y, x] = new WorldTile(0, 0);
                switch (intMap[y, x]) {
                    case 0:
                        worldTileMap[y, x].walkable = false;
                        tTilemap.SetTile(new Vector3Int(x, y, 0), water);
                        break;
                    case 1:
                        worldTileMap[y, x] = new WorldTile(1, 0);
                        gTilemap.SetTile(new Vector3Int(x, y, 0), grass);
                        worldTileMap[y, x].walkable = true;
                        break;
                    case 2:
                        worldTileMap[y, x] = new WorldTile(1, 0);
                        gTilemap.SetTile(new Vector3Int(x, y, 0), grass);
                        worldTileMap[y, x].walkable = false;
                        break;
                    case 3:
                        var sh = Instantiate(shop, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
                    case 4:
                        var cS = Instantiate(craftingStation, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
                    default:
                        worldTileMap[y, x] = new WorldTile(1, 0);
                        gTilemap.SetTile(new Vector3Int(x, y, 0), grass);
                        worldTileMap[y, x].walkable = true;
                    break;
                }
                if (intMap[y, x] > 10000) {
                    generateSpawner(intMap[y, x], x, y);
                }
                worldTileMap[y, x].worldPosition = new Vector2(x, y);
            }
        }
        map = worldTileMap;
    }


    private void setCameraBoundary(int width, int height) {
        var cameraBoundary = GameObject.Find("CameraBoundary");
        var cameraBoundaryCollider = cameraBoundary.GetComponent<PolygonCollider2D>();
        cameraBoundary.transform.localScale = new Vector3(width, height, 0);
        cameraBoundaryCollider.offset = new Vector2(width/2, height/2);
    }
}