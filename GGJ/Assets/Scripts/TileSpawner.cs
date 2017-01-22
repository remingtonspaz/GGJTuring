using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

	// Definition
	const int empty = 0;
	const int player = 1;
	const int obstacle = 2;
	const int creature = 3;
	const int inaccessible = 4;
	const int gridWidth = 64;
	const int gridHeight = 64;
	int[][] grid;
	int[][] creatures;
	int[][] grid0;
	int[][] creatures0;
	int[][] grid1;
	int[][] creatures1;
	int[][] emptySpaces;

	// Algo Params
	int maxWallRadius = 4;
	double wallCostCoefficient = 4;
	int maxCreatureRadius = 12;
	double creatureCostCoefficient = 32;

	// Test Params
	int obstacleCount = 64;
	int obstacleMinSide = 2;
	int obstacleMaxSide = 8;
	int creatureCount = 64;

	// Use this for initialization
	void Start () {
		setupGrid ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void setupGrid() {
		print ("making a new grid");
		// new grid
		grid = new int[gridWidth] [];
		grid0 = new int[gridWidth] [];
		grid1 = new int[gridWidth] [];
		for (int i = 0; i < gridWidth; i++) {
			grid [i] = new int[gridHeight];
			grid0 [i] = new int[gridHeight];
			grid1 [i] = new int[gridHeight];
			for (int j = 0; j < gridHeight; j++) {
				grid [i] [j] = inaccessible;
			}
		}
		displayGrid ();
			
		print ("randomly placing player");
		// randomly place player
		int playerX = Random.Range(0, gridWidth);
		int playerY = Random.Range (0, gridHeight);
		grid [playerX] [playerY] = player;
		displayGrid ();

		// randomly place obstacles
		print ("randomly placing obstacles");
		for (int i = 0; i < obstacleCount; i++) {
			bool obstacleContainsPlayer = true;
			int obstacleCenterX;
			int obstacleCenterY;
			int obstacleMinX = 0;
			int obstacleMinY = 0;
			int obstacleWidth = 0;
			int obstacleHeight = 0;
			while (obstacleContainsPlayer) {
				obstacleCenterX = Random.Range (0, gridWidth);
				obstacleCenterY = Random.Range (0, gridHeight);
				obstacleWidth = Random.Range (obstacleMinSide, obstacleMaxSide + 1);
				obstacleHeight = Random.Range (obstacleMinSide, obstacleMaxSide + 1);
				obstacleMinX = (int)(obstacleCenterX - 0.5 * obstacleWidth);
				obstacleMinY = (int)(obstacleCenterY - 0.5 * obstacleHeight);
				obstacleContainsPlayer = obstacleMinX <= playerX && playerX <= obstacleMinX + obstacleWidth && obstacleMinY <= playerY && playerY <= obstacleMinY + obstacleHeight;
			}
			for (int j = obstacleMinX; j < obstacleMinX + obstacleWidth; j++) {
				for (int k = obstacleMinY; k < obstacleMinY + obstacleHeight; k++) {
					if (inBounds(j, k)) {
						grid[j][k] = obstacle;
					}
				}
			}
		}
		displayGrid ();

		// flood filling 
		print("flood filling");
		floodFill (playerX, playerY);
		displayGrid ();

		// placing dudes
		emptySpaces = new int[gridWidth * gridHeight][];
		int emptySpaceCount = 0;
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				if (grid[i][j] == empty) {
					emptySpaces [emptySpaceCount] = new int[2];
					emptySpaces [emptySpaceCount] [0] = i;
					emptySpaces [emptySpaceCount] [1] = j;
					emptySpaceCount += 1;
				}
			}
		}
		if (emptySpaceCount < creatureCount) {
			print ("abort!  not enough empty space.");
			return;
		}
		int[] creatureIndices = randomShuffleAndDeal (creatureCount, emptySpaceCount);
		creatures = new int[creatureCount][];
		creatures0 = new int[creatureCount][];
		creatures1 = new int[creatureCount][];
		for (int i = 0; i < creatureCount; i++) {
			int x = emptySpaces [creatureIndices [i]] [0];
			int y = emptySpaces [creatureIndices [i]] [1];
			grid [x] [y] = creature;
			creatures [i] = new int[2];
			creatures [i] [0] = x;
			creatures [i] [1] = y;
			creatures0 [i] = new int[2];
			creatures0 [i] [0] = x;
			creatures0 [i] [1] = y;
			creatures1 [i] = new int[2];
			creatures1 [i] [0] = x;
			creatures1 [i] [1] = y;
		}

		print ("initial cost");
		print (computeCost(grid, creatures));

		// perturb and step a bunch
		for (int i = 0; i < 32; i++) {
			for (int j = 0; j < 32; j++) {
				perturbAndStep ();
			}
			displayGrid ();
			double c = computeCost (grid, creatures);
			print ("cost");
			print (c);
		}
	}

	void perturbAndStep() {
		for (int i = 0; i < creatureCount; i++) {
			creatures0 [i] [0] = creatures [i] [0];
			creatures0 [i] [1] = creatures [i] [1];
			creatures1 [i] [0] = creatures [i] [0];
			creatures1 [i] [1] = creatures [i] [1];
		}
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridWidth; j++) {
				grid0 [i] [j] = grid [i] [j];
				grid1 [i] [j] = grid [i] [j];
			}
		}
		for (int i = 0; i < creatureCount; i++) {
			int randomWalkP = Random.Range (0, 12);
			int oldX = creatures [i] [0];
			int oldY = creatures [i] [1];
			int newX0 = oldX;
			int newY0 = oldY;
			int newX1 = oldX;
			int newY1 = oldY;
			if (randomWalkP == 0) {
				if (inBounds (oldX + 1, oldY) && grid0 [oldX + 1] [oldY] == empty) {
					newX0 += 1;
				}
				if (inBounds (oldX - 1, oldY) && grid1 [oldX - 1] [oldY] == empty) {
					newX1 -= 1;
				}
			} else if (randomWalkP == 1) {
				if (inBounds (oldX, oldY + 1) && grid0 [oldX] [oldY + 1] == empty) {
					newY0 += 1;
				}
				if (inBounds (oldX, oldY - 1) && grid1 [oldX] [oldY - 1] == empty) {
					newY1 -= 1;
				}
			} else if (randomWalkP == 2) {
				if (inBounds (oldX - 1, oldY) && grid0 [oldX - 1] [oldY] == empty) {
					newX0 -= 1;
				}
				if (inBounds (oldX + 1, oldY) && grid1 [oldX + 1] [oldY] == empty) {
					newX1 += 1;
				}
			} else if (randomWalkP == 3) {
				if (inBounds (oldX, oldY - 1) && grid0 [oldX] [oldY - 1] == empty) {
					newY0 -= 1;
				}
				if (inBounds (oldX, oldY + 1) && grid1 [oldX] [oldY + 1] == empty) {
					newY1 += 1;
				}
			}
			grid0 [oldX] [oldY] = empty;
			grid0 [newX0] [newY0] = creature;
			grid1 [oldX] [oldY] = empty;
			grid1 [newX1] [newY1] = creature;
			creatures0 [i] [0] = newX0;
			creatures0 [i] [1] = newY0;
			creatures1 [i] [0] = newX1;
			creatures1 [i] [1] = newY1;
		}
		double c = computeCost (grid, creatures);
		double c0 = computeCost (grid0, creatures0);
		double c1 = computeCost (grid1, creatures1);
		print ("c, c0, c1");
		print (c);
		print (c0);
		print (c1);
		if (c < c0 && c < c1) {
			return;
		}
		if (c0 < c1) {
			for (int j = 0; j < gridWidth; j++) {
				for (int k = 0; k < gridHeight; k++) {
					grid [j] [k] = grid0 [j] [k];
				}
			}
			for (int j = 0; j < creatureCount; j++) {
				creatures[j][0] = creatures0[j][0];
				creatures[j][1] = creatures0[j][1];
			}
		} else {
			for (int j = 0; j < gridWidth; j++) {
				for (int k = 0; k < gridHeight; k++) {
					grid [j] [k] = grid1 [j] [k];
				}
			}
			for (int j = 0; j < creatureCount; j++) {
				creatures[j][0] = creatures1[j][0];
				creatures[j][1] = creatures1[j][1];
			}
		}
	}

	double computeCost(int[][] aGrid, int[][] theCreatures) {
		return computeWallCost (aGrid, theCreatures) + computeCreatureCost (aGrid, theCreatures);
	}

	double computeWallCost(int[][] aGrid, int[][] theCreatures) {
		double result = 0;
		for (int c = 0; c < creatureCount; c++) {
			int creatureX = theCreatures [c] [0];
			int creatureY = theCreatures [c] [1];
			for (int i = creatureX - maxWallRadius; i <= creatureX + maxWallRadius; i++) {
				for (int j = creatureY - maxWallRadius; j <= creatureY + maxWallRadius; j++) {
					if (inBounds (i, j)) {
						if (aGrid [i] [j] == obstacle) {
							double dx = creatureX - i;
							double dy = creatureY - j;
							double sqDistance = dx * dx + dy * dy;
							if (sqDistance < maxWallRadius * maxWallRadius) {
								result += wallCostCoefficient / sqDistance;
							}
						}
					} else {
						double dx = creatureX - i;
						double dy = creatureY - j;
						double sqDistance = dx * dx + dy * dy;
						if (sqDistance < maxWallRadius * maxWallRadius) {
							result += wallCostCoefficient / sqDistance;
						}
					}
				}
			}
		}
		return result;
	}

	double computeCreatureCost (int[][] aGrid, int[][] theCreatures)
	{
		double result = 0;
		for (int i = 0; i < creatureCount; i++) {
			for (int j = i + 1; j < creatureCount; j++) {
				int x0 = theCreatures [i] [0];
				int y0 = theCreatures [i] [1];
				int x1 = theCreatures [j] [0];
				int y1 = theCreatures [j] [1];
				double dx = x1 - x0;
				double dy = y1 - y0;
				double sqDistance = dx * dx + dy * dy;
				if (sqDistance < maxCreatureRadius * maxCreatureRadius) {
					result += creatureCostCoefficient / sqDistance;
				}
			}
		}
		return result;
	}

	void floodFill(int x, int y) {
		if (!inBounds (x, y)) {
			return;
		} else if (grid [x] [y] == player) {
			floodFill (x - 1, y);
			floodFill (x + 1, y);
			floodFill (x, y - 1);
			floodFill (x, y + 1);
		} else if (grid [x] [y] == inaccessible) {
			grid [x] [y] = empty;
			floodFill (x - 1, y);
			floodFill (x + 1, y);
			floodFill (x, y - 1);
			floodFill (x, y + 1);
		}
	}

	bool inBounds(int x, int y) {
		return 0 <= x && x < gridWidth && 0 <= y && y < gridHeight;
	}

	void displayGrid() {
		string s = "";
		for (int j = 0; j < gridHeight; j++) {
			for (int i = 0; i < gridWidth; i++) {
				if (grid[i][j] == 0) {
					s += " ";
				} else {
					s += grid[i][j].ToString();
				}
			}
			s += "\n";
		}
		print (s);
	}

	int[] randomShuffleAndDeal(int count, int length) {
		int[] result = new int[count];
		List<int> deck = new List<int>();
		for (int i = 0; i < length; i++) {
			deck.Add (i);
		}
		for (int i = 0; i < count; i++) {
			int r = Random.Range (0, length - i);
			result [i] = deck [r];
			deck.RemoveAt (r);
		}
		print ("random shuffle");
		print (result);
		return result;
	}
}
