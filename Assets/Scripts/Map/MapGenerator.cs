using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public WorldTile[,] map;
	public int[,] mapToCreate;
	public bool generateRandomMap;
	public GameObject shop;
	public GameObject craftingStation;
	private Dictionary<string, TileBase> bedrockTypes;
	public bool mapGenerated;

	// private List<GameObject> shrubs;

	void Start() {
		shop = GameObject.Find("Shop");
        craftingStation = GameObject.Find("CraftingStation");
		mapToCreate = IntroMap.map;
		if(generateRandomMap) {
			// CODE GOES HERE
		} else {
			simpleMapGeneration(mapToCreate);
			mapGenerated = true;
		}
	}


	public void generateSpawner(int spawnerInt, int posX, int posY) {
		var spawnerClone = Instantiate(GameObject.Find("Spawner"), new Vector2(posX, posY), Quaternion.identity) as GameObject;
		var spawnerScript = spawnerClone.GetComponent<Spawner>();
		var spawnerIntStr = spawnerInt.ToString();
		spawnerScript.width = Int16.Parse(spawnerIntStr.Substring(0,2));
		spawnerScript.height = Int16.Parse(spawnerIntStr.Substring(2,2));
		spawnerScript.initialZombieSpawn = Int16.Parse(spawnerIntStr.Substring(4,1));
		spawnerScript.zombieType = Int16.Parse(spawnerIntStr.Substring(5,1));
		// var col = spawnerClone.GetComponent<BoxCollider2D>();
		// col.size = new Vector2(spawnerScript.width, spawnerScript.height);
		// col.offset = new Vector2(spawnerScript.width/2, spawnerScript.height/2);
		spawnerScript.spawnInitialZombies();
	}

	private void setCameraBoundary(int width, int height) {
        var cameraBoundary = GameObject.Find("CameraBoundary");
        var cameraBoundaryCollider = cameraBoundary.GetComponent<PolygonCollider2D>();
        cameraBoundary.transform.localScale = new Vector3(width, height, 0);
        cameraBoundaryCollider.offset = new Vector2(width/2, height/2);
    }

	public int GetSurroundingTileCount(int gridX, int gridY, string tileCover) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY].tileCover[tileCover];
					}
				}
				else {
					wallCount ++;
				}
			}
		}
		return wallCount;
	}

	public List<WorldTile> getNeighbourTiles(Vector2 tilePos) {
		List<WorldTile> neighbours = new List<WorldTile>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if(x == 0 && y == 0) {
					continue;
				}
				var checkX = tilePos.x + x;
				var checkY = tilePos.y + y;

				if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height) {
					neighbours.Add(map[(int)checkX, (int)checkY]);
				}
			}
		}
		return neighbours;

	}

	private void simpleMapGeneration(int[,] intMap) {
        height = intMap.GetLength(0);
        width = intMap.GetLength(1);
        // setCameraBoundary(w, h);
        var worldTileMap = new WorldTile[height, width];
        var mapObj = gameObject;
        var gTilemap = mapObj.transform.GetChild(3).gameObject.GetComponent<Tilemap>();
        var tTilemap = mapObj.transform.GetChild(1).gameObject.GetComponent<Tilemap>();
        var grass = tTilemap.GetTile(new Vector3Int(1, 0, 0));
        var water = tTilemap.GetTile(new Vector3Int(0, 0, 0));
        for(var x = 0; x < height - 1; x++) {
            for(var y = 0; y < width - 1; y++) {
                worldTileMap[x, y] = new WorldTile(0, 0);
                switch (intMap[x, y]) {
                    case 0:
                        worldTileMap[x, y].walkable = false;
                        tTilemap.SetTile(new Vector3Int(x, y, 0), water);
                        break;
                    case 1:
                        worldTileMap[x, y] = new WorldTile(1, 0);
                        gTilemap.SetTile(new Vector3Int(x, y, 0), grass);
                        worldTileMap[x, y].walkable = true;
                        break;
                    case 2:
                        worldTileMap[x, y] = new WorldTile(1, 0);
                        gTilemap.SetTile(new Vector3Int(x, y, 0), grass);
                        worldTileMap[x, y].walkable = false;
                        break;
                    case 3:
                        var sh = Instantiate(shop, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
                    case 4:
                        var cS = Instantiate(craftingStation, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
                    default:
                        worldTileMap[x, y] = new WorldTile(1, 0);
                        gTilemap.SetTile(new Vector3Int(x, y, 0), grass);
                        worldTileMap[x, y].walkable = true;
                    break;
                }
                if (intMap[x, y] > 10000) {
                    generateSpawner(intMap[x, y], x, y);
                }
                worldTileMap[x, y].worldPosition = new Vector2(x, y);
            }
        }
        map = worldTileMap;
    }
}