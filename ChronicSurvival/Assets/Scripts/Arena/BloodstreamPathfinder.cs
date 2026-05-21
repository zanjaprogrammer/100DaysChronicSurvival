using System.Collections.Generic;
using UnityEngine;

namespace ChronicSurvival.Arena
{
    /// <summary>
    /// A grid-based A* pathfinding system that works in tandem with ArenaWalkableMask.
    /// Discretizes the arena into a grid and computes paths within the blood vessels.
    /// </summary>
    public class BloodstreamPathfinder : MonoBehaviour
    {
        private static BloodstreamPathfinder instance;
        public static BloodstreamPathfinder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<BloodstreamPathfinder>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("BloodstreamPathfinder");
                        instance = go.AddComponent<BloodstreamPathfinder>();
                    }
                }
                return instance;
            }
        }

        [Header("Grid Settings")]
        [Tooltip("Number of columns in the pathfinding grid")]
        [SerializeField] private int gridWidth = 64;
        [Tooltip("Number of rows in the pathfinding grid")]
        [SerializeField] private int gridHeight = 36;

        [Header("Debug")]
        [SerializeField] private bool drawGridGizmos = false;
        [SerializeField] private bool drawPathGizmos = true;

        private Node[,] grid;
        private Vector2 arenaMin;
        private Vector2 arenaMax;
        private float nodeWidth;
        private float nodeHeight;
        private bool isInitialized = false;

        private List<Vector2> lastDebugPath;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            InitializeGrid();
        }

        /// <summary>
        /// Initializes the pathfinding grid based on the ArenaWalkableMask.
        /// </summary>
        public void InitializeGrid()
        {
            if (ArenaWalkableMask.Instance == null)
            {
                Debug.LogError("[BloodstreamPathfinder] ArenaWalkableMask.Instance is null! Cannot initialize grid.");
                return;
            }

            Bounds bounds = ArenaWalkableMask.Instance.GetArenaBounds();
            arenaMin = bounds.min;
            arenaMax = bounds.max;

            float totalWidth = bounds.size.x;
            float totalHeight = bounds.size.y;

            nodeWidth = totalWidth / gridWidth;
            nodeHeight = totalHeight / gridHeight;

            grid = new Node[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector2 worldPos = GetWorldPosition(x, y);
                    bool isWalkable = ArenaWalkableMask.Instance.IsWalkable(worldPos);

                    grid[x, y] = new Node(x, y, worldPos, isWalkable);
                }
            }

            isInitialized = true;
            Debug.Log($"[BloodstreamPathfinder] Grid initialized: {gridWidth}x{gridHeight} nodes.");
        }

        /// <summary>
        /// Converts grid coordinates to world space coordinates at the node's center.
        /// </summary>
        public Vector2 GetWorldPosition(int x, int y)
        {
            float posX = arenaMin.x + (x * nodeWidth) + (nodeWidth * 0.5f);
            float posY = arenaMin.y + (y * nodeHeight) + (nodeHeight * 0.5f);
            return new Vector2(posX, posY);
        }

        /// <summary>
        /// Converts a world space position to grid coordinates.
        /// </summary>
        public Vector2Int GetGridPosition(Vector2 worldPosition)
        {
            float u = Mathf.InverseLerp(arenaMin.x, arenaMax.x, worldPosition.x);
            float v = Mathf.InverseLerp(arenaMin.y, arenaMax.y, worldPosition.y);

            int x = Mathf.Clamp(Mathf.FloorToInt(u * gridWidth), 0, gridWidth - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(v * gridHeight), 0, gridHeight - 1);

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Finds a path from a start world position to an end world position using A*.
        /// </summary>
        public List<Vector2> FindPath(Vector2 startPos, Vector2 endPos)
        {
            if (!isInitialized)
            {
                // If not initialized, fallback to straight line
                return new List<Vector2> { endPos };
            }

            Vector2Int startGrid = GetGridPosition(startPos);
            Vector2Int endGrid = GetGridPosition(endPos);

            Node startNode = grid[startGrid.x, startGrid.y];
            Node endNode = grid[endGrid.x, endGrid.y];

            // If start node is not walkable, try to find the nearest walkable node
            if (!startNode.isWalkable)
            {
                startNode = GetNearestWalkableNode(startGrid);
            }

            // If end node is not walkable, find the nearest walkable node
            if (!endNode.isWalkable)
            {
                endNode = GetNearestWalkableNode(endGrid);
            }

            if (startNode == null || endNode == null)
            {
                return new List<Vector2> { endPos };
            }

            List<Node> openSet = new List<Node> { startNode };
            HashSet<Node> closedSet = new HashSet<Node>();

            // Reset grid costs
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y].gCost = int.MaxValue;
                    grid[x, y].hCost = 0;
                    grid[x, y].parent = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = GetDistance(startNode, endNode);

            while (openSet.Count > 0)
            {
                // Find node in openSet with lowest fCost
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || 
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // Found the path
                if (currentNode == endNode)
                {
                    List<Vector2> path = RetracePath(startNode, endNode);
                    if (drawPathGizmos)
                    {
                        lastDebugPath = path;
                    }
                    return path;
                }

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.gCost)
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            // Path not found, fallback to straight line target
            return new List<Vector2> { endPos };
        }

        private List<Vector2> RetracePath(Node startNode, Node endNode)
        {
            List<Vector2> path = new List<Vector2>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.worldPosition);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            // Perform path smoothing (simplifying nodes that have clear line of sight)
            return SmoothPath(path);
        }

        private List<Vector2> SmoothPath(List<Vector2> originalPath)
        {
            if (originalPath.Count <= 2) return originalPath;

            List<Vector2> smoothed = new List<Vector2>();
            smoothed.Add(originalPath[0]);

            int currentIndex = 0;
            while (currentIndex < originalPath.Count - 1)
            {
                int nextIndex = currentIndex + 1;
                
                // Look ahead to find the furthest node we have line-of-sight to
                for (int i = originalPath.Count - 1; i > currentIndex + 1; i--)
                {
                    if (HasLineOfSight(originalPath[currentIndex], originalPath[i]))
                    {
                        nextIndex = i;
                        break;
                    }
                }

                smoothed.Add(originalPath[nextIndex]);
                currentIndex = nextIndex;
            }

            return smoothed;
        }

        private bool HasLineOfSight(Vector2 start, Vector2 end)
        {
            if (ArenaWalkableMask.Instance == null) return true;

            // Simple raycast check using walkability samples along the vector
            float distance = Vector2.Distance(start, end);
            int steps = Mathf.CeilToInt(distance / 0.3f); // Sample every 0.3 units

            for (int i = 1; i < steps; i++)
            {
                float t = (float)i / steps;
                Vector2 samplePoint = Vector2.Lerp(start, end, t);
                if (!ArenaWalkableMask.Instance.IsWalkable(samplePoint))
                {
                    return false;
                }
            }

            return true;
        }

        private List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                    {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        private Node GetNearestWalkableNode(Vector2Int gridPos)
        {
            Node bestNode = null;
            int bestDistance = int.MaxValue;

            // Spiral search out
            for (int radius = 1; radius < Mathf.Max(gridWidth, gridHeight); radius++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius) continue;

                        int checkX = gridPos.x + x;
                        int checkY = gridPos.y + y;

                        if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                        {
                            Node node = grid[checkX, checkY];
                            if (node.isWalkable)
                            {
                                int dist = Mathf.Abs(x) + Mathf.Abs(y);
                                if (dist < bestDistance)
                                {
                                    bestDistance = dist;
                                    bestNode = node;
                                }
                            }
                        }
                    }
                }

                if (bestNode != null)
                {
                    return bestNode;
                }
            }

            return null;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            // Diagonal cost is 14, orthogonal cost is 10 (octile distance * 10)
            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private void OnDrawGizmos()
        {
            if (drawGridGizmos && grid != null)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    for (int y = 0; y < gridHeight; y++)
                    {
                        Node n = grid[x, y];
                        Gizmos.color = n.isWalkable ? new Color(0, 1, 0, 0.1f) : new Color(1, 0, 0, 0.1f);
                        Gizmos.DrawCube(n.worldPosition, new Vector3(nodeWidth * 0.9f, nodeHeight * 0.9f, 0.1f));
                    }
                }
            }

            if (drawPathGizmos && lastDebugPath != null && lastDebugPath.Count > 0)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < lastDebugPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(lastDebugPath[i], lastDebugPath[i + 1]);
                    Gizmos.DrawSphere(lastDebugPath[i], 0.15f);
                }
                Gizmos.DrawSphere(lastDebugPath[lastDebugPath.Count - 1], 0.15f);
            }
        }

        private class Node
        {
            public int gridX;
            public int gridY;
            public Vector2 worldPosition;
            public bool isWalkable;

            public int gCost;
            public int hCost;
            public Node parent;

            public int fCost => gCost + hCost;

            public Node(int gridX, int gridY, Vector2 worldPosition, bool isWalkable)
            {
                this.gridX = gridX;
                this.gridY = gridY;
                this.worldPosition = worldPosition;
                this.isWalkable = isWalkable;
            }
        }
    }
}
