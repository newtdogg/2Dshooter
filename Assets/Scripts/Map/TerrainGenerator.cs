// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;



/*
        TERRAIN - NON TRAVERSABLE. TREES. BUILDINGS ETC.
*/




// public class TerrainGenerator
// {
//         map.getTiles()[x,y].tileCover["trees"] = (random.Next(0,100) < treeFillPercent)? 1: 0;

				// if(tile.tileCover["trees"] == 1) {
				// 	treesTilemap.SetTile(pos, groundTiles[7,1]);


				// 	float localXrand = (float)random.Next(0, 10);
				// 	float localYrand = (float)random.Next(0, 10);
				// 	// int shrubChance = random.Next(0, 10);
				// 	var randomTree = trees[random.Next(trees.Count)];
				// 	var treePos = new Vector3(pos.x + (localXrand/10), pos.y + 1.5f + (localYrand/10), 0);
				// 	var newTree = Instantiate(randomTree, new Vector3(0, 0, 0), Quaternion.identity);
				// 	newTree.GetComponent<SpriteRenderer>().sortingOrder = 200 - pos.y;
				// 	newTree.transform.SetParent(treeParent);
				// 	newTree.transform.position = treePos;
				// 	if(localXrand > 7 || localYrand > 7) {
				// 		var treePos2 = new Vector3(pos.x + (1 - localXrand/10), pos.y + 1.5f + (1 - localYrand/10), 0);
				// 		var newTree2 = Instantiate(randomTree, new Vector3(0, 0, 0) , Quaternion.identity);
				// 		newTree2.GetComponent<SpriteRenderer>().sortingOrder = 200 - pos.y;
				// 		newTree2.transform.SetParent(treeParent);
				// 		newTree2.transform.position = treePos2;
				// 	}

				// }


                		
		// trees = new List<GameObject>();
		// treeParent = transform.GetChild(7);
		// foreach (Transform tree in treeParent)
        // {
        //     trees.Add(tree.gameObject);
        // }

        
		// for (int x = 6; x < 12; x ++) {
		// 	for (int y = 0; y < 7; y ++) {
		// 		treeGrassEdges[x - 6, y] = palette.GetTile(new Vector3Int(x, y, 0));
		// 	}
		// }



// }
