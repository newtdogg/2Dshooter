using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System;
using Random=UnityEngine.Random;


public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public int gridSizeX = 4;
	public int gridSizeY = 3;
	public int sideLength = 64;

	private Map map;
	public GameObject shop;
	public GameObject craftingStation;
	private Dictionary<string, TileBase> bedrockTypes;
	private TileTools tileTools;
	public bool mapGenerated;
	public GameController gameController;
	private bool itemRoomSet = false;
	public int[,] mapToCreate;
	private GameObject spawnerClone;
	private GameObject bossSpawnerClone;
	public Dictionary<string, MobSpawner> spawners;
	public BossSpawner bossSpawner;
	private Dictionary<int, List<List<Vector2Int>>> arenaWalls;
	private GameObject player;
	private GameObject shadowBlock;
	private GameObject borderShadow;
	private GameObject shadowParent;
	private List<GameObject> itemRoomEntranceWalls;
	private GameObject entranceWall;
	private readonly Dictionary<string, string> cellNumberToStringMap = new Dictionary<string, string>() {
		{ "0000", "Empty" },
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
		{ "1101", "RightTopLeft" },
		{ "1110", "TopRightBottom" },
		{ "1111", "Cross" },
		{ "1010", "TopBottom" },
		{ "0101", "LeftRight" }
	};

	private List<Vector2Int> entryWallPositions = new List<Vector2Int>()
	{
		new Vector2Int(27, 58),
		new Vector2Int(58, 27),
		new Vector2Int(27, 5),
		new Vector2Int(5, 27),
	};

	private MapData smallestMapData;
	private MapData activeMapData;
	

	void Start() {
		shop = GameObject.Find("Shop");
		map = gameObject.GetComponent<Map>();
        craftingStation = GameObject.Find("CraftingStation");
		shadowBlock = GameObject.Find("Shadow");
		spawnerClone = GameObject.Find("Spawner");
		bossSpawnerClone = GameObject.Find("BossSpawner");
		player = GameObject.Find("Player");
		itemRoomEntranceWalls = new List<GameObject>();
		smallestMapData = new MapData(2, 2);
		smallestMapData.setRoomValues(
			new Vector2Int(0, 0),
			new Vector2Int(0, 1),
			new Vector2Int(1, 0),
			new Vector2Int(1, 1)
		);
		smallestMapData.grid = new List<List<int[]>>() {
			new List<int[]>() {new int[] {1, 1, 0, 0}, new int[] {1, 0, 0, 1}},
			new List<int[]>() {new int[] {0, 1, 1, 0}, new int[] {0, 0, 1, 1}}
		};
		// mapToCreate = GameObject.Find("GameController").GetComponent<GameController>().activeMap;
		// mapToCreate = new DebugMap().map;
		// mapToCreate = new SurvivalMap().map;
		tileTools = GameObject.Find("TileTools").GetComponent<TileTools>();
		spawners = new Dictionary<string, MobSpawner>();
		arenaWalls = new Dictionary<int, List<List<Vector2Int>>>();
		shadowParent = transform.parent.GetChild(1).gameObject;
		borderShadow = transform.parent.GetChild(2).gameObject;
		entranceWall = GameObject.Find("EntranceWall");
	}
	
	public void generateMobDebugMap() {
		var cellInterior = File.ReadAllText($"./Assets/Scripts/Map/Maps/Debug.json");
		var mapJSONString = File.ReadAllText($"./Assets/Scripts/Map/Maps/Primary/Bottom.json");
		var cellInteriorObject = JsonConvert.DeserializeObject<MapCell>(cellInterior);
		var mapPrimaryCell = JsonConvert.DeserializeObject<MapCell>(mapJSONString);
		mapToCreate = assembleCell(mapPrimaryCell.map, cellInteriorObject.map);
		activeMapData = new MapData(1, 1);
		smallestMapData.setRoomValues(
			new Vector2Int(5, 5),
			new Vector2Int(5, 5),
			new Vector2Int(5, 5),
			new Vector2Int(5, 5)
		);
		createMap();
	}

	public void generateLevelMap() {
		mapToCreate = generateMapFromCells("random");
		createMap();
	}

	public void generateSmallMap() {
		mapToCreate = generateMapFromCells("smallestfloor");
		createMap();
	}

	public void generateGunRangeMap() {
		var mapJSONString = File.ReadAllText($"./Assets/Scripts/Map/Maps/Primary/Bottom.json");
		var mapPrimaryCell = JsonConvert.DeserializeObject<MapCell>(mapJSONString);
		mapToCreate = mapPrimaryCell.map;
		activeMapData = new MapData(1, 1);
		createMap();
	}

	private int[,] assembleCell(int[,] primaryCell, int[,] interiorCell) {
		const int borderWidth = 6;
    	var primaryCellSize = 64;
		for(var x = 0; x < primaryCellSize; x++) {
			for(var y = 0; y < primaryCellSize; y++) {
				if(y >= borderWidth && x >= borderWidth && y < primaryCellSize - borderWidth  && x < primaryCellSize - borderWidth) {
					primaryCell[x, y] = interiorCell[x - borderWidth, y - borderWidth];
				}
			}
		}
		return primaryCell;
	}

	public List<List<int[,]>> translateCellGridToMapCells(MapData mapData) {
		var yCount = 0;
		var cellGrid = new List<List<int[,]>>();
		foreach (var row in mapData.grid)
		{
			var xCount = 0;
			var rowList = new List<int[,]>();
			foreach (var cell in row)
			{
				var cellVec = new Vector2Int(xCount, yCount);
				var cellString = string.Join("", cell);
				var cellName = cellNumberToStringMap[cellString];
				var mapJSONString = File.ReadAllText($"./Assets/Scripts/Map/Maps/Primary/{cellName}.json");
				string cellInterior = File.ReadAllText($"./Assets/Scripts/Map/Maps/Interior/Start/1.json");
				var cellRand = new System.Random((int) System.DateTime.Now.Ticks);
				if (cellVec == mapData.startingRoomLocation) {
					cellInterior = File.ReadAllText($"./Assets/Scripts/Map/Maps/Interior/Start/1.json");
				}
				else if (cellVec == mapData.bossRoomLocation) {
					cellInterior = File.ReadAllText($"./Assets/Scripts/Map/Maps/Interior/Boss/1.json");
				}
				else if (cellVec == mapData.itemRoomLocation) {
					cellInterior = File.ReadAllText($"./Assets/Scripts/Map/Maps/Interior/Item/1.json");
				}
				else {
					cellInterior =
						File.ReadAllText($"./Assets/Scripts/Map/Maps/Interior/Generic/{cellRand.Next(1, 4)}.json");
				}

				var cellInteriorObject = JsonConvert.DeserializeObject<MapCell>(cellInterior);
				var mapPrimaryCell = JsonConvert.DeserializeObject<MapCell>(mapJSONString);
				if (cellString != "0000")
				{
					assembleCell(mapPrimaryCell.map, cellInteriorObject.map);
				}

				rowList.Add(mapPrimaryCell.map);
				xCount += 1;
			}

			// rowList.Reverse();
			cellGrid.Add(rowList);
			yCount += 1;
		}

		return cellGrid;
	}

	private int[,] generateMapFromCells(string type) {
		activeMapData = new MapData(gridSizeX, gridSizeY);
		switch (type) {
			case "smallestfloor": 
				activeMapData = smallestMapData;
			break;
			case "random":
			default:
				activeMapData.generateRandomMapData();
				break;
		}
		var listOfCells = translateCellGridToMapCells(activeMapData);
		var width = activeMapData.xLength * sideLength;
		var height = activeMapData.yLength * sideLength;
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

	private void destroyItemRoomWalls() {
		foreach (var wall in itemRoomEntranceWalls) {
			Destroy(wall);
		}
	}

	private void generateBossSpawner(Vector2Int cellVector) {
		var bossSpawnerPos = new Vector2((cellVector.x * 64) + 32, (cellVector.y * 64) + 32);
		var bossSpawnerObject = Instantiate(bossSpawnerClone, bossSpawnerPos, Quaternion.identity) as GameObject;
		var bossSpawnerScript = bossSpawnerObject.GetComponent<BossSpawner>();
		bossSpawnerScript.createWallLists(cellVector);
		bossSpawnerScript.setWallTriggers();
	}

	private void generateMobSpawner(int spawnerInt, int posX, int posY, Vector2Int spawnerVector, int spawnerIndex) {
		Debug.Log("generating mob spawner");
		var spawnerPos = new Vector2(posX, posY); 
		var spawnerObject = Instantiate(spawnerClone, spawnerPos, Quaternion.identity) as GameObject;
		var spawnerScript = spawnerClone.GetComponent<MobSpawner>();
		var id = $"{Random.Range(0,999)}v{spawnerVector.x}:{spawnerVector.y}";
		spawnerScript.cellVector = spawnerVector;
		if ((spawnerVector == activeMapData.itemRoomKeyLocation) && !itemRoomSet) {
			Debug.Log($"{id}, spawns key");
			spawnerScript.holdsItemKey = true;
			itemRoomSet = true;
			spawnerScript.keyPickupCallback = destroyItemRoomWalls;
		}
		// Debug.Log($"{id}, normal");
		spawnerScript.setAttributes(spawnerInt, spawnerVector, player.transform);
		spawners.Add(id, spawnerScript);
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

	

	private void createCellEntranceWalls(Vector2Int cellPos) {
		var wallCount = 0;
		foreach (var position in entryWallPositions) {
			createEntryWall((cellPos.x * sideLength) + position.x, (cellPos.y * sideLength) + position.y, (wallCount + 1) % 2);
			wallCount += 1;
		}
	}

	private void createEntryWall(int xPos, int yPos, int direction) {
		var entranceWallClone = Instantiate(entranceWall, new Vector3(direction == 0 ? xPos : xPos + 5, direction == 0 ? yPos + 5 : yPos, 0), Quaternion.identity);
		entranceWallClone.transform.localScale = direction == 0 ? new Vector3(1, 10, 0) : new Vector3(10, 1, 0);
		itemRoomEntranceWalls.Add(entranceWallClone);
	}

	public void createMap() {
        width = mapToCreate.GetLength(0);
        height = mapToCreate.GetLength(1);
        // setCameraBoundary(w, h);
        var spawnerCount = 0;
		map.worldTiles = new WorldTile[width, height];
        tileTools.worldTileArray = map.worldTiles;
        var mapObj = gameObject;
    	tileTools.groundTilemap = mapObj.transform.GetChild(3).gameObject.GetComponent<Tilemap>();
        tileTools.wallTilemap = mapObj.transform.GetChild(1).gameObject.GetComponent<Tilemap>();
        tileTools.groundTile = tileTools.wallTilemap.GetTile(new Vector3Int(1, 0, 0));
        tileTools.wallTile = tileTools.wallTilemap.GetTile(new Vector3Int(0, 0, 0));
        for (var x = 0; x < width - 1; x++) {
	        for (var y = 0; y < height - 1; y++)
	        {
		        switch (mapToCreate[x, y])
		        {
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

		        if (mapToCreate[x, y] > 10)
		        {
			        if (mapToCreate[x, y].ToString().Substring(0, 2) == "55")
			        {
				        generateShadow(x, y, Int32.Parse(mapToCreate[x, y].ToString().Substring(2, 2)),
					        Int32.Parse(mapToCreate[x, y].ToString().Substring(4, 2)));
				        tileTools.setWallTile(x, y);
			        }
			        else if (mapToCreate[x, y] > 100000)
			        {
				        var cellX = (int)(Mathf.Floor(x / sideLength));
				        var cellY = (int)(Mathf.Floor(y / sideLength));
				        var cellVector = new Vector2Int(cellX, cellY);
				        if (spawners.Count > 0) {
					        if (spawners.Values.Last().cellVector != cellVector) {
						        spawnerCount = 0;
					        }
				        }
						generateMobSpawner(mapToCreate[x, y], x, y, cellVector, spawnerCount);

				        spawnerCount++;
				        tileTools.setGroundTile(x, y, true);
			        }
		        }
		        tileTools.worldTileArray[x, y].worldPosition = new Vector2(x, y);
	        }
        }
		// generateBossSpawner(activeMapData.bossRoomLocation);
        tileTools.intMap = mapToCreate;
		gameController.bossSpawner = bossSpawner;
		createCellEntranceWalls(activeMapData.itemRoomLocation);
		gameController.spawners = new List<MobSpawner>(spawners.Values);
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