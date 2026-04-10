using UnityEngine;
using System.Collections.Generic;

public static class PoissonOnGrid
{
    /// <summary>
    /// Generate Poisson disk points over a tilemap, only on FLOOR tiles.
    /// Supports multiple seeds for even spreading.
    /// </summary>
    public static List<Vector2Int> GeneratePoints(
        WalkerGenerator.Grid[,] grid,
        int width,
        int height,
        float radius,
        int rejectionSamples = 30,
        int seedCount = 3   // number of different seed points
    )
    {
        List<Vector2Int> allPoints = new List<Vector2Int>();
        int cellSize = Mathf.CeilToInt(radius / Mathf.Sqrt(2));

        int gridW = Mathf.CeilToInt(width / (float)cellSize);
        int gridH = Mathf.CeilToInt(height / (float)cellSize);

        for (int seedIndex = 0; seedIndex < seedCount; seedIndex++)
        {
            int[,] cellGrid = new int[gridW, gridH];
            for (int x = 0; x < gridW; x++)
                for (int y = 0; y < gridH; y++)
                    cellGrid[x, y] = -1;

            List<Vector2Int> seedPoints = new List<Vector2Int>();
            List<Vector2> spawnPoints = new List<Vector2>();

            // Pick a random FLOOR tile as seed
            Vector2Int seed = FindRandomFloor(grid, width, height);
            spawnPoints.Add(seed);
            seedPoints.Add(seed);

            int cx = Mathf.Clamp(seed.x / cellSize, 0, gridW - 1);
            int cy = Mathf.Clamp(seed.y / cellSize, 0, gridH - 1);
            cellGrid[cx, cy] = 0;

            while (spawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Count);
                Vector2 spawnCenter = spawnPoints[spawnIndex];
                bool accepted = false;

                for (int i = 0; i < rejectionSamples; i++)
                {
                    float angle = Random.value * Mathf.PI * 2f;
                    float dist = Random.Range(radius, radius * 2f);
                    Vector2 candidateF = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
                    Vector2Int candidate = new Vector2Int(Mathf.RoundToInt(candidateF.x), Mathf.RoundToInt(candidateF.y));

                    if (!InsideMap(candidate, width, height)) continue;
                    if (grid[candidate.x, candidate.y] != WalkerGenerator.Grid.FLOOR) continue;

                    if (IsValid(candidate, cellSize, radius, seedPoints, cellGrid))
                    {
                        seedPoints.Add(candidate);
                        spawnPoints.Add(candidate);

                        int ccx = Mathf.Clamp(candidate.x / cellSize, 0, gridW - 1);
                        int ccy = Mathf.Clamp(candidate.y / cellSize, 0, gridH - 1);
                        cellGrid[ccx, ccy] = seedPoints.Count - 1;

                        accepted = true;
                        break;
                    }
                }

                if (!accepted)
                    spawnPoints.RemoveAt(spawnIndex);
            }

            allPoints.AddRange(seedPoints);
        }

        // Remove duplicates
        return new List<Vector2Int>(new HashSet<Vector2Int>(allPoints));
    }

    private static bool InsideMap(Vector2Int p, int w, int h)
        => p.x >= 0 && p.x < w && p.y >= 0 && p.y < h;

    private static Vector2Int FindRandomFloor(WalkerGenerator.Grid[,] grid, int w, int h)
    {
        List<Vector2Int> floors = new List<Vector2Int>();
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                if (grid[x, y] == WalkerGenerator.Grid.FLOOR)
                    floors.Add(new Vector2Int(x, y));

        if (floors.Count == 0)
            return new Vector2Int(w / 2, h / 2);

        return floors[Random.Range(0, floors.Count)];
    }

    private static bool IsValid(Vector2Int candidate, int cellSize, float radius,
                                List<Vector2Int> points, int[,] cellGrid)
    {
        int cx = Mathf.Clamp(candidate.x / cellSize, 0, cellGrid.GetLength(0) - 1);
        int cy = Mathf.Clamp(candidate.y / cellSize, 0, cellGrid.GetLength(1) - 1);

        int searchX0 = Mathf.Max(0, cx - 2);
        int searchX1 = Mathf.Min(cellGrid.GetLength(0) - 1, cx + 2);
        int searchY0 = Mathf.Max(0, cy - 2);
        int searchY1 = Mathf.Min(cellGrid.GetLength(1) - 1, cy + 2);

        for (int x = searchX0; x <= searchX1; x++)
            for (int y = searchY0; y <= searchY1; y++)
            {
                int idx = cellGrid[x, y];
                if (idx == -1) continue;
                if (Vector2Int.Distance(points[idx], candidate) < radius)
                    return false;
            }

        return true;
    }
}
