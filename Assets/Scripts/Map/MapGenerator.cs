using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
	public int spawnerCount;
	public GameController gameController;
	public Dictionary<int, Spawner> spawners;
	private Dictionary<int, List<List<Vector2Int>>> arenaWalls;
	private GameObject player;
	private GameObject shadowBlock;
	private GameObject borderShadow;
	private GameObject shadowParent;

	void Start() {
		shop = GameObject.Find("Shop");
		map = gameObject.GetComponent<Map>();
        craftingStation = GameObject.Find("CraftingStation");
		shadowBlock = GameObject.Find("Shadow");
		player = GameObject.Find("Player");
		// mapToCreate = GameObject.Find("GameController").GetComponent<GameController>().activeMap;
		// mapToCreate = new DebugMap().map;
		// mapToCreate = new SurvivalMap().map;
		mapToCreate = generateMapFromCells();
		tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
		spawners = new Dictionary<int, Spawner>();
		arenaWalls = new Dictionary<int, List<List<Vector2Int>>>();
		spawnerCount = 0;
		shadowParent = transform.parent.GetChild(1).gameObject;
		borderShadow = transform.parent.GetChild(2).gameObject;
		if(generateRandomMap) {
			// CODE GOES HERE
		} else {
			simpleMapGeneration(mapToCreate);
			mapGenerated = true;
		}
	}

	public new List<List<int[]>> createCellGrid() {
		var gridSizeX = 3;
		var gridSizeY = 2;
		var cellGrid = new List<List<int[]>>();
		for(var y = 0; y < gridSizeY; y++) {
			cellGrid.Add(new List<int[]>());
			for(var x = 0; x < gridSizeX; x++) {
				if(y == 0 && x == 0) {
					cellGrid[0].Add(new int[] {1,1,0,0});
				} else {
					cellGrid[y].Add(generateRandomFittingCell(cellGrid, x, y, gridSizeX, gridSizeY));
				}
			}
		}
		return cellGrid;
	}

	private int[] generateRandomFittingCell(List<List<int[]>> grid, int x, int y, int maxSizeX, int maxSizeY) {
		var directions = new List<Vector2Int>() { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
		var newCell = new int[] { 0, 0, 0, 0 };
		var count = 0;
		foreach (var direction in directions) {
			var newX = x + direction.x;
      		var newY = y + direction.y;
			var currentWallIndex = count > 1 ? count - 2 : count + 2;
			var rand = new System.Random((int)System.DateTime.Now.Ticks);
			if(newX >= maxSizeX || newX < 0 || newY >= maxSizeY || newY < 0 ) {
				newCell[currentWallIndex] = 0;
			} else {
				newCell[currentWallIndex] = newCell.Sum() > 1 ? rand.Next(0, 1) : 1;
			}
			if (newY >= 0 && newY < grid.Count) {
				if(newX >= 0 && newX < grid[newY].Count) {
					newCell[currentWallIndex] = grid[newY][newX][count];
				}
			}
			count += 1;	
		}
		return newCell;
	}

	public List<List<int[,]>> translateCellGridToMapCells(List<List<int[]>> cellTypeGrid) {
		var cellGrid = new List<List<int[,]>>();
		var cellNumberToStringMap = new Dictionary<string, string>() {
			{ "1000", "Top" },
			{ "0100", "Right" },
			{ "0010", "Bottom" },
			{ "0001", "Left" },
			{ "1100", "TopRight" },
			{ "0110", "BottomRight" },
			{ "0011", "BottomLeft" },
			{ "1001", "TopLeft" },
			{ "0111", "RightBottomLeft" },
			{ "1011", "TopLeftBottom" },
			{ "1101", "TopRightLeft" },
			{ "1110", "TopRightBottom" },
			{ "1111", "Cross" },
			{ "1010", "TopBottom" },
			{ "0101", "LeftRight" }
		};
		var rand = new System.Random((int)System.DateTime.Now.Ticks);
		// var randCell = rand.Next(0, cellTypeGrid.Count * cellTypeGrid[0].Count);
		var randCell = 0;
		var count = 0;
		foreach (var row in cellTypeGrid) {
			var rowList = new List<int[,]>();
			foreach (var cell in row) {
				var cellString = string.Join("", cell);
				var cellName = cellNumberToStringMap[cellString];
				string mapJSONString;
				if(randCell == count) {
					mapJSONString = File.ReadAllText($"./Assets/Scripts/Map/Maps/{cellName}/Start.json");
				} else {
					mapJSONString = File.ReadAllText($"./Assets/Scripts/Map/Maps/{cellName}/1.json");
				}
				var testmap = JsonConvert.DeserializeObject<MapCell>(mapJSONString);
				rowList.Add(testmap.map);
				count += 1;
			}
			cellGrid.Add(rowList);
		}
		return cellGrid;
	}

	public int[,] generateMapFromCells() {
		// var test = createCellGrid();
		var test = new List<List<int[]>>() {
			new List<int[]>() {new int[] {1, 1, 0, 0},new int[] {1, 0, 0, 1}},
			new List<int[]>() {new int[] {0, 1, 1, 0},new int[] {0, 0, 1, 1}}
		};
		var listOfCells = translateCellGridToMapCells(test);
		var width = 128;
		var height = width;
		var sideLength = 64;
		var mainArray = new int[height, width];
		for(var yCount = 0; yCount < listOfCells.Count; yCount++) {
			for(var xCount = 0; xCount < listOfCells[yCount].Count; xCount++) {
				var currentCell = listOfCells[xCount][yCount];
				for(var a = 0; a < currentCell.GetLength(0); a++) {
					for(var b = 0; b < currentCell.GetLength(0); b++) {
						mainArray[(sideLength * yCount) + a, (sideLength * xCount) + b] = currentCell[a, b];
					}
				}
			}
		}
		return mainArray;
	}

	public void generateSpawner(int spawnerInt, int posX, int posY, int spawnerIndex) {
		var spawnerClone = Instantiate(GameObject.Find("Spawner"), new Vector2(posX, posY), Quaternion.identity) as GameObject;
		var spawnerScript = spawnerClone.GetComponent<Spawner>();
		spawnerScript.setAttributes(spawnerInt, spawnerIndex, player.transform);
		spawners.Add(spawnerCount, spawnerScript);

		// var col = spawnerClone.GetComponent<BoxCollider2D>();
		// col.size = new Vector2(spawnerScript.width, spawnerScript.height);
		// col.offset = new Vector2(spawnerScript.width/2, spawnerScript.height/2);
		// spawnerScript.spawnInitialZombies();
	}

	private void generateShadow(int x, int y, int width, int height) {
		var shadowObject = Instantiate(borderShadow, new Vector2(x, y), Quaternion.identity) as GameObject;
		shadowObject.transform.SetParent(shadowParent.transform);
		shadowObject.transform.position = new Vector3(shadowObject.transform.position.x + (float)width/2f, shadowObject.transform.position.y + (float)height/2f, 0);
		shadowObject.transform.localScale = new Vector3(width, height, 0);
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

	public List<int> getNeighbourTileValues(Vector3Int tilePos) {
		List<int> neighbours = new List<int>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if(x == 0 && y == 0) {
					continue;
				}
				var checkX = tilePos.x + x;
				var checkY = tilePos.y + y;

				if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height) {
					neighbours.Add(mapToCreate[(int)checkX, (int)checkY]);
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
						tileTools.setGroundTile(x, y, true);
						tileTools.worldTileArray[x, y] = new WorldTile(1, 0);
                        var sh = Instantiate(shop, new Vector2(x, y), Quaternion.identity) as GameObject;
                        break;
                    case 4:
						tileTools.setGroundTile(x, y, true);
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
				if(intMap[x, y] > 10) {
					if(intMap[x, y].ToString().Substring(0,2) == "55") {
						generateShadow(x, y, Int32.Parse(intMap[x, y].ToString().Substring(2,2)), Int32.Parse(intMap[x, y].ToString().Substring(4,2)));
						tileTools.setWallTile(x, y);
					} else if (intMap[x, y] > 10000) {
						spawnerCount++;
						tileTools.setWallTile(x, y);
						generateSpawner(intMap[x, y], x, y, spawnerCount);
					}
				}
                tileTools.worldTileArray[x, y].worldPosition = new Vector2(x, y);
            }
        }
		foreach (var wall in arenaWalls) {
			spawners[wall.Key].walls = wall.Value;
			spawners[wall.Key].setWallTriggers();
		}
		tileTools.intMap = intMap;
		gameController.spawners = new List<Spawner>(spawners.Values);
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