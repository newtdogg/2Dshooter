using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapData {
	public List<List<int[]>> grid;
	public int bossRoomLocation;
	public int startingRoomLocation;
	public int itemRoomLocation;
	public int itemRoomKeyLocation;
	public int xLength;
	public int yLength;

	public MapData(int x, int y) {
		xLength = x;
		yLength = y;
	}

	public void setRoomValues(int bossRoomLocationVal, int startingRoomLocationVal, int itemRoomLocationVal, int itemRoomKeyLocationVal) {
		bossRoomLocation = bossRoomLocationVal;
		startingRoomLocation = startingRoomLocationVal;
		itemRoomLocation = itemRoomLocationVal;
		itemRoomKeyLocation = itemRoomKeyLocationVal;
	}

	public void generateRandomMapData () {
		var rand = new System.Random((int)System.DateTime.Now.Ticks);
        grid = createRandomCellGrid(xLength, yLength);
		bossRoomLocation = rand.Next(0, xLength);
		startingRoomLocation = (xLength * (yLength - 1)) + rand.Next(0, xLength);
		itemRoomKeyLocation = xLength + rand.Next(0, xLength * (yLength - 2));
	}

    public new List<List<int[]>> createRandomCellGrid(int gridSizeX, int gridSizeY) {
		var cellGrid = new List<List<int[]>>();
		for(var y = 0; y < gridSizeY; y++) {
			cellGrid.Add(new List<int[]>());
			for(var x = 0; x < gridSizeX; x++) {
				if(y == 0 && x == 0) {
					cellGrid[0].Add(new int[] {0,0,1,1});
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
				var randWallPercent = rand.Next(0, 100);
				var randWall = randWallPercent < 30 ? 1 : 0;
				newCell[currentWallIndex] = newCell.Sum() >= 1 ? randWall : 1;
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
}
