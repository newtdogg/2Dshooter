using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour {

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	
	[Range(0,100)]
	public int primaryGrassFillPercent;
	
	[Range(0,100)]
	public int secondaryGrassFillPercent;
	
	[Range(0,100)]
	public int treeFillPercent;

	[Range(0,10)]
	public int primaryGrassSmoothValue;
	[Range(0,10)]
	public int secondaryGrassSmoothValue;
	private Transform treeParent;

	public WorldTile[,] map;
	private Tilemap groundTextureTilemap;
	private Tilemap groundTilemap;
	private Tilemap pathTilemap;
	private Tilemap grassDetailTilemap;

	private Tilemap primaryGrassTilemap;
	private Tilemap secondaryGrassTilemap;
	public Dictionary<string, Vector3Int> bedrockStartingLocations;
	private Tilemap treesTilemap;
	private Tilemap palette;
	private TileBase grassDefault;
	private Dictionary<string, TileBase> bedrockTypes;
	private TileBase[] textures;
	private TileBase[,] groundTiles;
	private TileBase[,] primaryGrassEdges;
	private TileBase[,] secondaryGrassEdges;
	private TileBase[,] treeGrassEdges;
	public GameObject shop;
	public GameObject craftingStation;
	private List<GameObject> trees;
	public bool mapGenerated;

	// private List<GameObject> shrubs;



	public void generateSpawner(int spawnerInt, int posX, int posY) {
		var spawnerClone = Instantiate(GameObject.Find("Spawner"), new Vector2(posX, posY), Quaternion.identity) as GameObject;
		var spawnerScript = spawnerClone.GetComponent<Spawner>();
		var spawnerIntStr = spawnerInt.ToString();
		spawnerScript.width = Int16.Parse(spawnerIntStr.Substring(0,2));
		spawnerScript.height = Int16.Parse(spawnerIntStr.Substring(2,2));
		spawnerScript.initialZombieSpawn = Int16.Parse(spawnerIntStr.Substring(4,1));
		spawnerScript.zombieType = Int16.Parse(spawnerIntStr.Substring(5,1));
		var col = spawnerClone.GetComponent<BoxCollider2D>();
		col.size = new Vector2(spawnerScript.width, spawnerScript.height);
		col.offset = new Vector2(spawnerScript.width/2, spawnerScript.height/2);
		spawnerScript.spawnInitialZombies();
	}













































	///////////////////////
	// WHAT IS THIS SHIT //
	///////////////////////

	void Start() {
		// useRandomSeed = true;
		width = 101;
		height = 101;
		// primaryGrassFillPercent = 55;
		// secondaryGrassFillPercent = 70;
		// treeFillPercent = 40;

		treesTilemap = transform.GetChild(0).gameObject.GetComponent<Tilemap>();
		grassDetailTilemap = transform.GetChild(1).gameObject.GetComponent<Tilemap>();
		secondaryGrassTilemap = transform.GetChild(2).gameObject.GetComponent<Tilemap>();
		primaryGrassTilemap = transform.GetChild(3).gameObject.GetComponent<Tilemap>();
		groundTextureTilemap = transform.GetChild(4).gameObject.GetComponent<Tilemap>();
		groundTilemap = transform.GetChild(5).gameObject.GetComponent<Tilemap>();
		// pathTilemap = transform.GetChild(8).gameObject.GetComponent<Tilemap>();
		
		palette = transform.GetChild(6).gameObject.GetComponent<Tilemap>();
		
		trees = new List<GameObject>();
		treeParent = transform.GetChild(7);
		foreach (Transform tree in treeParent)
        {
            trees.Add(tree.gameObject);
        }

		// shrubs = new List<GameObject>();
		// // var shrubParent = GameObject.Find("Shrubs");
		// // foreach (Transform shrub in shrubParent.transform) {
            // // shrubs.Add(shrub.gameObject);
        // }

		groundTiles = new TileBase[12,14];
		primaryGrassEdges = new TileBase[6,7];
		secondaryGrassEdges = new TileBase[6,7];
		treeGrassEdges = new TileBase[6,7];

		for (int x = 0; x < 12; x ++) {
			for (int y = 0; y < 14; y ++) {
				groundTiles[x, y] = palette.GetTile(new Vector3Int(x, y, 0));
			}
		}

		for (int x = 0; x < 6; x ++) {
			for (int y = 0; y < 7; y ++) {
				primaryGrassEdges[x, y] = palette.GetTile(new Vector3Int(x, y, 0));
			}
		}

		for (int x = 0; x < 6; x ++) {
			for (int y = 7; y < 14; y ++) {
				secondaryGrassEdges[x, y - 7] = palette.GetTile(new Vector3Int(x, y, 0));
			}
		}

		for (int x = 6; x < 12; x ++) {
			for (int y = 0; y < 7; y ++) {
				treeGrassEdges[x - 6, y] = palette.GetTile(new Vector3Int(x, y, 0));
			}
		}

		bedrockTypes = new Dictionary<string, TileBase> { 
			{"clay", groundTiles[6,13]},
			{"sandstone", groundTiles[7,13]},
			{"limestone", groundTiles[8,13]},
			{"granite", groundTiles[9,13]},
			{"basalt", groundTiles[9,13]}
		};
		textures = new TileBase[] { groundTiles[6, 12], groundTiles[7, 12], groundTiles[8,12], groundTiles[9, 12] };
		grassDefault = groundTiles[1,1];

		GenerateMap();
		mapGenerated = true;
	}
	void GenerateMap() {
		map = new WorldTile[width,height];
		RandomFillMap();

		
		SmoothMap(primaryGrassSmoothValue, "primaryGrass");
		SmoothMap(secondaryGrassSmoothValue, "secondaryGrass");
		SmoothMap(5, "trees");
		SetTiles();
	}


	void RandomFillMap() {
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}
		System.Random pseudoRandomPrimaryGrass = new System.Random(seed.GetHashCode()+ 1);
		System.Random pseudoRandomSecondaryGrass = new System.Random(seed.GetHashCode() + 2);
		System.Random pseudoRandomTrees = new System.Random(seed.GetHashCode() + 3);

		System.Random pseudoRandomBedrock = new System.Random(seed.GetHashCode() + 5);

		List<string> bedrockNameList = new List<string>(bedrockTypes.Keys);
		bedrockStartingLocations = new Dictionary<string, Vector3Int>();

		for(int i = 0; i < bedrockTypes.Count; i++) {
			var x = pseudoRandomBedrock.Next(0, width);

			var y = pseudoRandomBedrock.Next(0, height);
			bedrockStartingLocations.Add(bedrockNameList[i], new Vector3Int(x, y, 0));
		}

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				var primaryGrassVal = (pseudoRandomPrimaryGrass.Next(0,100) < primaryGrassFillPercent)? 1: 0;
				var secondaryGrassVal = (pseudoRandomSecondaryGrass.Next(0,100) < secondaryGrassFillPercent) && primaryGrassVal == 1 ? 1: 0;
				map[x,y] = new WorldTile(primaryGrassVal, secondaryGrassVal);
				map[x,y].worldPosition = new Vector2(x, y);
				map[x,y].tileCover["trees"] = (pseudoRandomTrees.Next(0,100) < treeFillPercent)? 1: 0;
				var lowestDelta = width * height;
				var lowestDeltaRock = "";
				foreach(var point in bedrockStartingLocations) {
					//for each point get the difference in distance between our point and the current cell
					var delta = Mathf.Abs(point.Value.x - x) + Mathf.Abs(point.Value.y - y);
					//store the point as nearest if it's closer than the last one
					if(delta < lowestDelta) {
						lowestDelta = delta;
						lowestDeltaRock = point.Key;
					}
				}
				map[x, y].bedrockName = lowestDeltaRock;
			}
		}
	}

	void SetTiles() {
		// var grassStreak = 0;
		foreach (Vector3Int pos in palette.cellBounds.allPositionsWithin) {
			if(pos.x >= 0 && pos.y >= 0 && pos.x <= width && pos.y <= height) {
				var tile = map[pos.x, pos.y];
				var rand = new System.Random((int)System.DateTime.Now.Ticks);
				var rand1 = new System.Random((int)System.DateTime.Now.Ticks);
				var rand2 = new System.Random((int)System.DateTime.Now.Ticks);
				var rand3 = new System.Random((int)System.DateTime.Now.Ticks);
				groundTilemap.SetTile(pos, bedrockTypes[tile.bedrockName]);
				groundTextureTilemap.SetTile(pos, textures[rand.Next(4)]);
				var vec = new Vector3Int(pos.x, pos.y, 0);
				if(tile.tileCover["primaryGrass"] == 1) {
					primaryGrassTilemap.SetTile(pos, grassDefault);
					if(tile.tileCover["trees"] == 0 && tile.tileCover["secondaryGrass"] == 0) {
						if(rand1.Next(0, 250) == 0) {
							// tile.hasZombie = true;
						}
					}
					// if(rand1.Next(0, 250) == 0) {
						// grassDetailTilemap.SetTile(pos, groundTiles[rand.Next(6, 12), rand.Next(3, 5)]);
					// }
					if(tile.tileCover["secondaryGrass"] == 0) {
						// grassDetailTilemap.SetTile(pos, groundTiles[rand.Next(6, 11), rand.Next(5, 7)]);
					}
				}
				if(tile.tileCover["secondaryGrass"] == 1) {
					secondaryGrassTilemap.SetTile(pos, groundTiles[1,8]);
				}
				if(tile.tileCover["primaryGrass"] == 0) {
					if(rand2.Next(0, 6) == 0) {
						// groundTilemap.SetTile(pos, groundTiles[rand2.Next(6, 12), rand2.Next(3, 5)]);
					}
					
				}
				if(tile.tileCover["trees"] == 1) {
					map[pos.x, pos.y].walkable = false;
					treesTilemap.SetTile(pos, groundTiles[7,1]);
					// treesTilemap.SetTileFlags(pos, TileFlags.None);
					// treesTilemap.SetColor(pos, new Color(0.34f, 0.43f, 0.47f, 1f));
					var rand4 = new System.Random((int)System.DateTime.Now.Ticks);
					var rand5 = new System.Random((int)System.DateTime.Now.Ticks);
					var rand6 = new System.Random((int)System.DateTime.Now.Ticks);
					float localXrand = (float)rand4.Next(0, 10);
					float localYrand = (float)rand5.Next(0, 10);
					// int shrubChance = rand6.Next(0, 10);
					var randomTree = trees[rand5.Next(trees.Count)];
					var treePos = new Vector3(pos.x + (localXrand/10), pos.y + 1.5f + (localYrand/10), 0);
					var newTree = Instantiate(randomTree, new Vector3(0, 0, 0), Quaternion.identity);
					newTree.GetComponent<SpriteRenderer>().sortingOrder = 200 - pos.y;
					newTree.transform.SetParent(treeParent);
					newTree.transform.position = treePos;
					if(localXrand > 7 || localYrand > 7) {
						var treePos2 = new Vector3(pos.x + (1 - localXrand/10), pos.y + 1.5f + (1 - localYrand/10), 0);
						var newTree2 = Instantiate(randomTree, new Vector3(0, 0, 0) , Quaternion.identity);
						newTree2.GetComponent<SpriteRenderer>().sortingOrder = 200 - pos.y;
						newTree2.transform.SetParent(treeParent);
						newTree2.transform.position = treePos2;
					}
					// if(shrubChance == 1) {
						// // // var randomShrub = shrubs[rand6.Next(shrubs.Count)];
						// // var newShrub = Instantiate(randomShrub, new Vector3(pos.x + (1 - (localXrand/10)), pos.y + (1 -(localYrand/10)), 0), Quaternion.identity);
						// newShrub.GetComponent<SpriteRenderer>().sortingOrder = 201 - pos.y;
					// }

				}
				if(rand1.Next(0, 1000) == 0) {
					if(rand1.Next(0, 1000) > 499) {
						// setFourTilesFromBottomLeft(groundTilemap, pos, 8, 12);
					} else {
						// setFourTilesFromBottomLeft(groundTilemap, pos, 6, 12);
					}
				}
				if(!tile.walkable) {
					// pathTilemap.SetTileFlags(pos, TileFlags.None);
					// pathTilemap.SetColor(pos, new Color(1f, 0.65f, 0.0f, 1f));
				}
			}
		}
		foreach (Vector3Int pos in palette.cellBounds.allPositionsWithin) {
			if(pos.x >= 0 && pos.y >= 0 && pos.x <= width && pos.y <= height) {
				var tile = map[pos.x, pos.y];
				if(pos.x != 0 && pos.x != width-1 && pos.y != 0 && pos.y != height -1){
					if(tile.tileCover["primaryGrass"] == 0) {
						SetGrassEdgeTile(pos.x, pos.y, primaryGrassEdges, "primaryGrass", primaryGrassTilemap);
					}
					if(tile.tileCover["secondaryGrass"] == 0) {
						SetGrassEdgeTile(pos.x, pos.y, secondaryGrassEdges, "secondaryGrass", secondaryGrassTilemap);
					}
					if(tile.tileCover["trees"] == 0) {
						SetGrassEdgeTile(pos.x, pos.y, treeGrassEdges, "trees", treesTilemap);
					}
				}
			}
		}
	}

	void SmoothMap(int smoothValue, string tileCover) {
		for (int i = 0; i < smoothValue; i ++) {
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					int neighbourgroundTiles = GetSurroundingTileCount(x,y, tileCover);
					if (neighbourgroundTiles > 4) {
						map[x,y].tileCover[tileCover] = 1;
					} else if (neighbourgroundTiles < 4) {
						map[x,y].tileCover[tileCover] = 0;
					}										
				}
			}
		}
	}

	void setFourTilesFromBottomLeft(Tilemap palette, Vector3Int pos, int x, int y) {
		palette.SetTile(pos, groundTiles[x, y]);
		palette.SetTile(new Vector3Int(pos.x + 1, pos.y, 0), groundTiles[x + 1, y]);
		palette.SetTile(new Vector3Int(pos.x, pos.y + 1, 0), groundTiles[x, y + 1]);
		palette.SetTile(new Vector3Int(pos.x + 1, pos.y + 1, 0), groundTiles[x + 1, y + 1]);
	}

	void SetGrassEdgeTile(int x, int y, TileBase[,] edges, string tileCover, Tilemap edgeTilemap) {
		var surroundingTiles = new int[3,3];
		for (int neighbourX = 0; neighbourX < 3; neighbourX ++) {
			for (int neighbourY = 0; neighbourY < 3; neighbourY ++) {
				surroundingTiles[neighbourX, neighbourY] = map[x + (neighbourX - 1), y + (neighbourY - 1)].tileCover[tileCover];
			}
		}
		var topRow = surroundingTiles[0, 2] + surroundingTiles[1, 2] + surroundingTiles[2, 2];
		var middleRow =  surroundingTiles[0, 1] + surroundingTiles[1, 1] + surroundingTiles[2, 1];
		var bottomRow = surroundingTiles[0, 0] + surroundingTiles[1, 0] + surroundingTiles[2, 0];
		var leftRow = surroundingTiles[0, 0] + surroundingTiles[0, 1] + surroundingTiles[0, 2];
		var centreRow = surroundingTiles[1, 0] + surroundingTiles[1, 1] + surroundingTiles[1, 2];
		var rightRow = surroundingTiles[2, 0] + surroundingTiles[2, 1] + surroundingTiles[2, 2];
		var crossVal = surroundingTiles[0, 1] + surroundingTiles[2, 1] + surroundingTiles[1, 2] + surroundingTiles[1, 0];
		var totalVal = topRow + middleRow + bottomRow;
		if(totalVal != 0 && tileCover == "trees") {
			map[x,y].walkable = false;
			// pathTilemap.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
			// pathTilemap.SetColor(new Vector3Int(x, y, 0), new Color(1f, 0.65f, 0.0f, 1f));
		}
		if(totalVal == 3) {
			if(bottomRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[1,2]);
			} else if (topRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[1,0]);
			} else if (leftRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,1]);
			} else if (rightRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[0,1]);
			}
			if(topRow == 2) {
				if(surroundingTiles[0, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3,5]);
					return;
				}
				if(surroundingTiles[2, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,5]);
					return;
				}
			}
			if(bottomRow == 2) {
				if(surroundingTiles[0, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3,6]);
					return;
				}
				if(surroundingTiles[2, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,6]);
					return;
				}
			}
		}
		if(totalVal == 4) {
			if(topRow == 3) {
				if(surroundingTiles[0, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,4]);
					return;
				}
				if(surroundingTiles[2, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3,4]);
					return;
				}

			}
			if(bottomRow == 3) {
				if(surroundingTiles[0, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,3]);
					return;
				}
				if(surroundingTiles[2, 1] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3,3]);
					return;
				}
			}
			if(leftRow == 3) {
				if(surroundingTiles[1, 2] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[0,4]);
					return;
				}
				if(surroundingTiles[1, 0] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[0,3]);
					return;
				}
			}
			if(rightRow == 3) {
				if(surroundingTiles[1, 2] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[1,4]);
					return;
				}
				if(surroundingTiles[1, 0] == 1) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[1,3]);
					return;
				}
			}
		}
		if(totalVal == 5) {
			if(topRow == 3 && leftRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,0]);
			}
			if(topRow == 3 && rightRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[0,0]);
			}
			if(bottomRow == 3 && leftRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[2,2]);
			}
			if(bottomRow == 3 && rightRow == 3) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[0,2]);
			}
		}
		if(totalVal < 4) {
			if(topRow == 2) {
				if(surroundingTiles[0, 2] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[4,5]);
					return;
				}
				if(surroundingTiles[2, 2] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5,5]);
					return;
				}
			}
			if(bottomRow == 2) {
				if(surroundingTiles[0, 0] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[4,6]);
					return;
				}
				if(surroundingTiles[0, 2] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5,6]);
					return;
				}
			}
			if(leftRow == 2) {
				if(surroundingTiles[0, 0] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5,3]);
					return;
				}
				if(surroundingTiles[0, 2] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5,4]);
					return;
				}
			}
			if(rightRow == 2) {
				if(surroundingTiles[2, 0] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[4,3]);
					return;
				}
				if(surroundingTiles[2, 2] == 0) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[4,4]);
					return;
				}
			}
		}
		if(totalVal == 1) {
			if(surroundingTiles[0, 1] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[4,2]);
				return;	
			}
			if(surroundingTiles[1, 0] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5,2]);
				return;
			}
			if(surroundingTiles[1, 2] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5,1]);
				return;
			}
			if(surroundingTiles[2, 1] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3,2]);
				return;
			}
		}
		if(crossVal == 0) {
			if(surroundingTiles[0, 2] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x - 1, y + 1, 0), edges[0,6]);
			}
			if(surroundingTiles[2, 2] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x + 1, y + 1, 0), edges[1,6]);
			}
			if(surroundingTiles[0, 0] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x - 1, y - 1, 0), edges[0,5]);
			}
			if(surroundingTiles[2, 0] == 1) {
				edgeTilemap.SetTile(new Vector3Int(x + 1, y - 1, 0), edges[1,5]);
			}
		}
		if (totalVal > 4) {
			if(surroundingTiles[1, 0] == 1 && surroundingTiles[1, 2] == 1) {
				if(leftRow == 3) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[4, 0]);
					return;
				}
				if(rightRow == 3) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[5, 0]);
					return;
				}
			}
			if(surroundingTiles[0, 1] == 1 && surroundingTiles[2, 1] == 1) {
				if(topRow == 3) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3, 1]);
					return;
				}
				if(bottomRow == 3) {
					edgeTilemap.SetTile(new Vector3Int(x, y, 0), edges[3, 0]);
					return;
				}
			}
		}
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
}