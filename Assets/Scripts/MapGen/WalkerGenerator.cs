using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int MapWidth = 30;
    public int MapHeight = 30;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tile Floor;
    public Tile Wall;
    public int MaximumWalkers = 10;
    public float FillPercentage = 0.4f;
    public float WaitTime = 0.05f;

    [Header("Enemy Spawning")]
    public List<GameObject> enemyPrefabs;
    public float enemySpacing = 4f;
    public int maxEnemies = 10;
    public float minSpawnCooldown = 0.2f;
    public int poissonRejectionSamples = 30;
    public float enemyDensity = 0.005f;
    public float respawnCooldown = 5f;
    public string enemyTag = "Enemy";

    public enum Grid { FLOOR, WALL, EMPTY }
    public Grid[,] gridHandler;
    public List<WalkerObject> Walkers;
    public Vector3Int mapOffset = Vector3Int.zero;
    public int TileCount = 0;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridHandler = new Grid[MapWidth, MapHeight];
        for (int x = 0; x < MapWidth; x++)
            for (int y = 0; y < MapHeight; y++)
                gridHandler[x, y] = Grid.EMPTY;

        Walkers = new List<WalkerObject>();
        mapOffset = new Vector3Int(-MapWidth / 2, -MapHeight / 2, 0);

        Vector3Int TileCenter = new Vector3Int(MapWidth / 2, MapHeight / 2, 0);
        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), 0.5f);
        gridHandler[TileCenter.x, TileCenter.y] = Grid.FLOOR;
        floorTilemap.SetTile(TileCenter + mapOffset, Floor);

        Walkers.Add(curWalker);
        TileCount = 1;

        StartCoroutine(CreateFloors());
        EnsureTilemapVisibleAndCenterCamera();
    }

    Vector2 GetDirection()
    {
        int choice = Random.Range(0, 4);
        return choice switch
        {
            0 => Vector2.down,
            1 => Vector2.left,
            2 => Vector2.up,
            3 => Vector2.right,
            _ => Vector2.zero
        };
    }

    IEnumerator CreateFloors()
    {
        while ((float)TileCount / gridHandler.Length < FillPercentage)
        {
            bool hasCreatedFloor = false;
            foreach (WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);
                if (gridHandler[curPos.x, curPos.y] != Grid.FLOOR)
                {
                    floorTilemap.SetTile(curPos + mapOffset, Floor);
                    TileCount++;
                    gridHandler[curPos.x, curPos.y] = Grid.FLOOR;
                    hasCreatedFloor = true;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasCreatedFloor)
                yield return new WaitForSeconds(WaitTime);
            else
                yield return null;
        }

        CreateWallsFast();
        StartCoroutine(SpawnEnemies());
    }

    void ChanceToRemove()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (Random.value < Walkers[i].ChanceToChange && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (Random.value < Walkers[i].ChanceToChange)
            {
                WalkerObject curWalker = Walkers[i];
                curWalker.Direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (Random.value < Walkers[i].ChanceToChange && Walkers.Count < MaximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i].Position;
                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.5f);
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            WalkerObject walker = Walkers[i];
            walker.Position += walker.Direction;
            walker.Position.x = Mathf.Clamp(walker.Position.x, 1, MapWidth - 2);
            walker.Position.y = Mathf.Clamp(walker.Position.y, 1, MapHeight - 2);
            Walkers[i] = walker;
        }
    }

    void CreateWallsFast()
    {
        List<Vector3Int> worldWallPositions = new List<Vector3Int>();
        for (int x = 1; x < MapWidth - 1; x++)
        {
            for (int y = 1; y < MapHeight - 1; y++)
            {
                if (gridHandler[x, y] != Grid.FLOOR) continue;

                void PlaceWall(int wx, int wy)
                {
                    if (gridHandler[wx, wy] == Grid.EMPTY)
                    {
                        gridHandler[wx, wy] = Grid.WALL;
                        worldWallPositions.Add(new Vector3Int(wx, wy, 0) + mapOffset);
                    }
                }

                PlaceWall(x + 1, y);
                PlaceWall(x - 1, y);
                PlaceWall(x, y + 1);
                PlaceWall(x, y - 1);
            }
        }

        foreach (var pos in worldWallPositions)
            wallTilemap.SetTile(pos, Wall);
    }

    IEnumerator SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
            yield break;

        while (true)
        {
            int currentEnemies = GameObject.FindGameObjectsWithTag(enemyTag).Length;

            if (currentEnemies < maxEnemies)
            {
                int missing = maxEnemies - currentEnemies;
                List<Vector2Int> spawnPoints = new List<Vector2Int>();

                // Collect all floor tiles
                List<Vector2Int> floorTiles = new List<Vector2Int>();
                for (int x = 0; x < MapWidth; x++)
                    for (int y = 0; y < MapHeight; y++)
                        if (gridHandler[x, y] == Grid.FLOOR)
                            floorTiles.Add(new Vector2Int(x, y));

                if (floorTiles.Count > 0)
                {
                    int seedCount = Mathf.Min(5, missing);
                    for (int i = 0; i < seedCount; i++)
                    {
                        Vector2Int seed = floorTiles[Random.Range(0, floorTiles.Count)];
                        List<Vector2Int> points = PoissonOnGrid.GeneratePoints(
                            gridHandler,
                            MapWidth,
                            MapHeight,
                            enemySpacing,
                            poissonRejectionSamples,
                            1
                        );

                        spawnPoints.AddRange(points);
                    }

                    // Remove duplicates and clamp to missing enemies
                    spawnPoints = new List<Vector2Int>(new HashSet<Vector2Int>(spawnPoints));
                    if (spawnPoints.Count > missing)
                        spawnPoints = spawnPoints.GetRange(0, missing);

                    foreach (var p in spawnPoints)
                    {
                        int px = Mathf.Clamp(p.x, 0, MapWidth - 1);
                        int py = Mathf.Clamp(p.y, 0, MapHeight - 1);
                        Vector3Int cell = new Vector3Int(px, py, 0) + mapOffset;
                        Vector3 worldPos = floorTilemap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0f);

                        GameObject chosenEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                        GameObject enemy = Instantiate(chosenEnemy, worldPos, Quaternion.identity);
                        enemy.tag = enemyTag;

                        yield return new WaitForSeconds(minSpawnCooldown);
                    }
                }
            }

            yield return new WaitForSeconds(respawnCooldown);
        }
    }

    void EnsureTilemapVisibleAndCenterCamera()
    {
        Vector3Int centerCell = new Vector3Int(MapWidth / 2, MapHeight / 2, 0) + mapOffset;
        Vector3 centerWorld = floorTilemap.CellToWorld(centerCell);

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = new Vector3(centerWorld.x + 0.5f, centerWorld.y + 0.5f, -10f);
            if (!mainCam.orthographic) mainCam.orthographic = true;
        }
    }
}
