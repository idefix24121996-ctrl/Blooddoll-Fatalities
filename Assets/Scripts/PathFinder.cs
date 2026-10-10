using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PathFinder 
{
    // map needs to be a rectangle
    public static List<Vector2Int> FindPath(Vector2Int from, Vector2Int to, List<List<bool>> obstacles)
    {
        // doing a DFS with a manhattan heuristic as the map is a grid so it should be enough
        // paths will not be optimal but that should make the movement look cooler and more chaotic possibly increasing difficulty
        List<List<bool>> explored = new List<List<bool>>();
        List<Node> frontier = new List<Node>();
        frontier.Add(new Node(from, new List<Vector2Int>()));

        List<Vector2Int> neighborDeltas = new List<Vector2Int> { new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1) };

        for(int x = 0; x < obstacles.Count; ++x)
        {
            explored.Add(new List<bool>());
            for (int y = 0; y < obstacles[0].Count; ++y)
            {
                explored[x].Add(false);
            }
        }


        while (frontier.Count > 0)
        {
            var current = frontier[frontier.Count - 1];
            frontier.RemoveAt(frontier.Count - 1);

            if(current.position == to)
            {
                return current.path;
            }

            explored[current.position.x][current.position.y] = true;

            // address neighbors from closest to farthest from target

            List<Vector2Int> neighbors = new List<Vector2Int>();
            foreach(var delta in neighborDeltas)
            {
                neighbors.Add(current.position + delta);
            }
            
            // bubble sort because it's just 4 elements
            bool sorted = false;
            while(!sorted)
            {
                bool changed = false;
                for(int i = 0; i < neighbors.Count - 1; ++i)
                {
                    if(Mathf.Abs(neighbors[i].x - to.x) + Mathf.Abs(neighbors[i].y - to.y) > 
                        Mathf.Abs(neighbors[i+1].x - to.x) + Mathf.Abs(neighbors[i+1].y - to.y))
                    { 
                        Vector2Int temp = neighbors[i];
                        neighbors[i] = neighbors[i+1];
                        neighbors[i+1] = temp;
                        changed = true;
                    }
                }

                if(!changed)
                {
                    sorted = true;
                }
            }

            // now actually add neighbors to frontier
            for(int i = 0; i < neighbors.Count; ++i)
            {
                if(!obstacles[neighbors[i].y][neighbors[i].x] && !explored[neighbors[i].y][neighbors[i].x])
                {
                    List<Vector2Int> newPath = new List<Vector2Int>(current.path);
                    newPath.Add(neighbors[i]);
                    frontier.Add(new Node(neighbors[i], newPath));
                }
            }
        }

        return null;
    }

    struct Node
    {
        public Vector2Int position;
        public List<Vector2Int> path;

        public Node(Vector2Int position, List<Vector2Int> path)
        {
            this.position = position;
            this.path = path;
        }
    }
}
