using UnityEngine;
using System; // Allows us to use the serialisable.
using System.Collections.Generic; // Allows us to use lists.
using Random = UnityEngine.Random; // This needs to be specifed as theres random in both unity and C# namespace.

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int Minimum;
        public int Maximum;

        // Constructor
        public Count(int min, int max)
        {
            Minimum = min;
            Maximum = max;
        }
    }

    // Sets up an 8x8 game board.
    public int Columns = 8;

    public int Rows = 8;

    // Random range of how many walls we want to spawn. Min of 5 walls, max of 9
    public Count WallCount = new Count(5, 9);

    public Count FoodCount = new Count(1, 5);

    // These are where our prefabs will be stored.
    public GameObject Exit;

    public GameObject[] FloorTiles;
    public GameObject[] WallTiles;
    public GameObject[] FoodTiles;
    public GameObject[] EnemyTiles;
    public GameObject[] OuterWallTiles;

    private Transform _boardHolder;

    // Used to track all posiable positions on the game board to check if
    // something has been spwaned in that position or not.
    private List<Vector3> _gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        _gridPositions.Clear();

        // Creates a list of potenial places to put wall, pickups or enemies.
        for (int x = 1; x < Columns - 1; x++)
        {
            for (int y = 1; x < Rows - 1; y++)
            {
                _gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // Sets up the floor and outter walls
    void BoardSetup()
    {
        _boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < Columns + 1; x++)
        {
            for (int y = -1; y < Rows + 1; y++)
            {
                // Randomly gets one of the floor tiles
                GameObject toInstantiate = FloorTiles[Random.Range(0, FloorTiles.Length)];

                // If x/y are on the outer area of board, add outer wall tile.
                if (x == -1 || x == Columns || y == -1 || y == Rows)
                {
                    toInstantiate = OuterWallTiles[Random.Range(0, OuterWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);

                instance.transform.SetParent(_boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, _gridPositions.Count);
        Vector3 randomPosition = _gridPositions[randomIndex];

        // Removes item from list to avoid multiple piling up.
        _gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max)
    {
        // Maximum number of that object to spawn.
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            // gets random position
            Vector3 randomPosition = RandomPosition();
            // gets random tile
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // Instantiates that tile
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(WallTiles, WallCount.Minimum, WallCount.Maximum);
        LayoutObjectAtRandom(FoodTiles, FoodCount.Minimum, FoodCount.Maximum);

        int enemyCount = (int) Mathf.Log(level, 2f);

        LayoutObjectAtRandom(EnemyTiles, enemyCount, enemyCount);

        Instantiate(Exit, new Vector3(Columns - 1, Rows - 1, 0f), Quaternion.identity);
    }
}