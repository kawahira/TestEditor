using System;
using System.Collections;
using System.Collections.Generic;

namespace SpatialAStar
{
    public class Result
    {
        public List<DeenGames.Utils.AStarPathFinder.PathFinderNode> path;
    }
    public class PathFinder
    {
        private int width;
        private int height;
        private byte[,] grid;
        private DeenGames.Utils.AStarPathFinder.PathFinder pf;
        public PathFinder(int mapwidth, int mapheight)
        {
            width = DeenGames.Utils.AStarPathFinder.PathFinderHelper.RoundToNearestPowerOfTwo(mapwidth);
            height = DeenGames.Utils.AStarPathFinder.PathFinderHelper.RoundToNearestPowerOfTwo(mapheight);
            grid = new byte[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = DeenGames.Utils.AStarPathFinder.PathFinderHelper.BLOCKED_TILE;
                }
            }
            pf = new DeenGames.Utils.AStarPathFinder.PathFinder(grid);
        }
        public void SetTile(byte[,] tiles, byte ThresholdIndex)
        {
            if (tiles.GetLength(0) > width) return;
            if (tiles.GetLength(1) > height) return;
            for ( int i = 0 ; i < tiles.GetLength(0) ; i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i, j] != 0 && tiles[i, j] < ThresholdIndex)
                    {
                        grid[i, j] = DeenGames.Utils.AStarPathFinder.PathFinderHelper.EMPTY_TILE;
                    }
                }
            }
        }
        public void Get(out Result result, int fromX, int fromY, int toX, int toY)
        {
            result = new Result();
            DeenGames.Utils.Point f = new DeenGames.Utils.Point(fromX,fromY);
            DeenGames.Utils.Point t = new DeenGames.Utils.Point(toX, toY);
            result.path = pf.FindPath(f, t);
/*
            if (path != null)
            {
                Debug.Log("Found path " + path.Count);

                foreach (PathFinderNode node in path)
                {
                    Debug.Log(node.PX + "x" + node.PY);
                }
            }
            else
            {
                Debug.Log("No path");
            }
 */
        }
    }
}
