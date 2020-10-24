using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public int[,] mapToCreate;
	private Map map;
	public bool generateRandomMap;
	public GameObject shop;
	public GameObject craftingStation;
	private Dictionary<string, TileBase> bedrockTypes;
	public bool mapGenerated;
	private TileTools tileTools;
	private Dictionary<int, Spawner> spawners;
	private Dictionary<int, List<List<Vector2Int>>> arenaWalls;
	private GameObject player;

	void Start() {
		shop = GameObject.Find("Shop");
		map = gameObject.GetComponent<Map>();
        craftingStation = GameObject.Find("CraftingStation");
		player = GameObject.Find("Player");
		// mapToCreate = GameObject.Find("GameController").GetComponent<GameController>().activeMap;
		mapToCreate = new DebugMap().map;
		tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
		spawners = new Dictionary<int, Spawner>();
		arenaWalls = new Dictionary<int, List<List<Vector2Int>>>();
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
		spawnerScript.setAttributes(spawnerInt);
		spawners.Add(spawnerScript.id, spawnerScript);

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
						wallCount += map.worldTiles[neighbourX,neighbourY].tileCover[tileCover];
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
					neighbours.Add(map.worldTiles[(int)checkX, (int)checkY]);
				}
			}
		}
		return neighbours;
	}

	private void simpleMapGeneration(int[,] intMap) {
        width = intMap.GetLength(0);
        height = intMap.GetLength(1);
        // setCameraBoundary(w, h);
		map.worldTiles = new WorldTile[width, height];
        tileTools.worldTileArray = map.worldTiles;
        var mapObj = gameObject;
    	tileTools.groundTilemap = mapObj.transform.GetChild(3).gameObject.GetComponent<Tilemap>();
        tileTools.wallTilemap = mapObj.transform.GetChild(1).gameObject.GetComponent<Tilemap>();
        tileTools.groundTile = tileTools.wallTilemap.GetTile(new Vector3Int(1, 0, 0));
        tileTools.wallTile = tileTools.wallTilemap.GetTile(new Vector3Int(0, 0, 0));
        for(var x = 0; x < width - 1; x++) {
            for(var y = 0; y < height - 1; y++) {
                switch (intMap[x, y]) {
                    case 0:
                        tileTools.setWallTile(x, y);
                        break;
                    case 1:
						tileTools.setGroundTile(x, y, true);
                        break;
                    case 2:
						tileTools.setGroundTile(x, y, false);
                        break;
                    case 3:
						tileTools.worldTileArray[x, y] = new WorldTile(1, 0);
                        var sh = Instantiate(shop, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
                    case 4:
						tileTools.worldTileArray[x, y] = new WorldTile(1, 0);
                        var cS = Instantiate(craftingStation, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
					case 5:
						tileTools.setGroundTile(x, y, false);
						player.transform.position = new Vector2(x, y);
						break;
                    default:
						tileTools.setGroundTile(x, y, true);
                    break;
                }
                if (intMap[x, y] > 10000) {
                    generateSpawner(intMap[x, y], x, y);
                } else if (intMap[x, y] > 100) {
                    setArenaWall(intMap[x, y], x, y);
                }
                tileTools.worldTileArray[x, y].worldPosition = new Vector2(x, y);
            }
        }
		foreach (var wall in arenaWalls) {
			spawners[wall.Key].walls = wall.Value;
			spawners[wall.Key].setWallTriggers();
		}

        // map.worldTiles = tileTools.worldTileArray;
    }

	public void setArenaWall(int wallInt, int posX, int posY) {
		var wallList = new List<Vector2Int>();
		var wallIntStr = wallInt.ToString();
        var id = Int16.Parse(wallIntStr.Substring(0,1));
        var direction = Int16.Parse(wallIntStr.Substring(1,1));
		var length = Int16.Parse(wallIntStr.Substring(2, wallIntStr.Length -2));
		if (direction == 0) {
			for(var i = posY; i < posY + length; i++) {
				wallList.Add(new Vector2Int(posX, i));
			}
		} else if (direction == 1) {
			for(var i = posX; i < posX + length; i++) {
				wallList.Add(new Vector2Int(i, posY));
			}
		}
		if(!arenaWalls.ContainsKey(id)) {
			arenaWalls[id] = new List<List<Vector2Int>>();
		}
		arenaWalls[id].Add(wallList);
	}
}