using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoronoiCalculator : MonoBehaviour
{
    public static VoronoiCalculator Instance
    {
        get
        {
            return _instance;
        }
    }
    private static VoronoiCalculator _instance = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_instance == null)
            _instance = this;
        else
            throw new System.Exception("VoronoiCalculator is singleton and an instance already exists");
    }

    List<List<bool>> obstacles = null;
    public List<List<bool>> Obstacles
    {
        get { return obstacles; }
        set { obstacles = value; }
    }
    List<Vector2Int> agentPositions = null;
    public List<Vector2Int> AgentPositions
    {
        get { return agentPositions; }
        set { agentPositions = value; }
    }

    List<Vector2> middles;
    public List<Vector2> Middles
    {
        get { return middles; }
    }

    // Update is called once per frame
    void Update()
    {
        // if performance is bad, calculate only once per x frames or on a separate thread

        if(obstacles != null && obstacles.Count > 0 && obstacles[0].Count > 0 &&
            agentPositions!= null && agentPositions.Count > 0)
        {
            // calculate Voronoi diagram via Manhattan distance BFS
            // won't be perfent, but it's good enough and should run fast

            List<HashSet<Vector2Int>> closestCells = new List<HashSet<Vector2Int>>();
            Queue<VoronoiBFSNode> frontier = new Queue<VoronoiBFSNode>();

            for(int i = 0; i < agentPositions.Count; ++i)
            {
                closestCells.Add(new HashSet<Vector2Int>());
                frontier.Enqueue(new VoronoiBFSNode(agentPositions[i], i));
            }

            List<Vector2Int> neighbors = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                closestCells[current.fromAgent].Add(current.position);


                foreach (var neighborOffset in neighbors)
                {
                    Vector2Int neighbor = new Vector2Int(current.position.x + neighborOffset.x, current.position.y + neighborOffset.y);

                    // if neighbor is out of bounds or obstacle, skip it
                    if (neighbor.x < 0 || neighbor.x >= obstacles.Count || neighbor.y < 0 || neighbor.y >= obstacles[neighbor.x].Count ||
                        obstacles[neighbor.x][neighbor.y])
                    {
                        continue;
                    }

                    // if the place is already closer to another agent, skip it
                    bool neighborFound = false;
                    for (int i = 0; i < agentPositions.Count; ++i)
                    {
                        if (closestCells[i].Contains(neighbor))
                        {
                            neighborFound = true;
                            break;
                        }
                    }

                    if (neighborFound)
                    {
                        continue;
                    }

                    frontier.Enqueue(new VoronoiBFSNode(neighbor, current.fromAgent));
                }
            }

            // the entire voronoi diagram is made to calculate the middle of each cell which is used for steering
            for(int i = 0; i< closestCells.Count; i++)
            {
                Vector2 middle = Vector2.zero;

                foreach(var a in closestCells[i])
                {
                    middle += a;
                }
                middle /= closestCells[i].Count;
                middles.Add(middle);
            }
        }
    }
}

struct VoronoiBFSNode
{
    public Vector2Int position;
    public int fromAgent;

    public VoronoiBFSNode(Vector2Int position, int fromAgent)
    {
        this.position = position;
        this.fromAgent = fromAgent;
    }
}