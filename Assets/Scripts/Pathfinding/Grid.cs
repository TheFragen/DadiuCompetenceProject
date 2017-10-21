using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public PathFinding _pathFinding;

    [SerializeField]
    private Items _itemScriptableObject;

    private Dictionary<Vector3, Node> _Grid;
    private Vector3 _testPos;

    private void Awake()
    {
        _Grid = new Dictionary<Vector3, Node>();
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        Dictionary<Vector3, Tile> tileDict =
            Level_PreGenerate.Instance.GetTileDictionary();
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // North
        Node north;
        _Grid.TryGetValue(node._WorldPos + Vector3.forward, out north);
        if (north != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(0, .5f, .5f), .1f,
                layerMask))
        {
            Tile nTile;
            tileDict.TryGetValue(north._WorldPos + new Vector3(0, 0, 1),
                out nTile);
            if (nTile != null)
            {
                if (!nTile.BlockedDown)
                {
                    neighbours.Add(north);
                }
            }
            else
            {
                neighbours.Add(north);
            }
        }

        // East
        Node east;
        _Grid.TryGetValue(node._WorldPos + Vector3.right, out east);
        if (east != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(.5f, .5f, 0), .1f,
                layerMask))
        {
            Tile nTile;
            tileDict.TryGetValue(east._WorldPos + new Vector3(1, 0, 0),
                out nTile);
            if (nTile != null)
            {
                if (!nTile.BlockedLeft)
                {
                    neighbours.Add(east);
                }
            }
            else
            {
                neighbours.Add(east);
            }
        }

        // South
        Node south;
        _Grid.TryGetValue(node._WorldPos + Vector3.back, out south);
        if (south != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(0, .5f, -.5f),
                .1f, layerMask))
        {
            Tile nTile;
            tileDict.TryGetValue(south._WorldPos + new Vector3(0, 0, -1),
                out nTile);
            if (nTile != null)
            {
                if (!nTile.BlockedUp)
                {
                    neighbours.Add(south);
                }
            }
            else
            {
                neighbours.Add(south);
            }
        }

        // West
        Node west;
        _Grid.TryGetValue(node._WorldPos + Vector3.left, out west);
        if (west != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(-.5f, .5f, 0),
                .1f, layerMask))
        {
            Tile nTile;
            tileDict.TryGetValue(west._WorldPos + new Vector3(-1, 0, 0),
                out nTile);
            if (nTile != null)
            {
                if (!nTile.BlockedRight)
                {
                    neighbours.Add(west);
                }
            }
            else
            {
                neighbours.Add(west);
            }
        }

        return neighbours;
    }

    public void AddToGrid(Vector3 worldPos)
    {
        worldPos.y = 0;
        Node node = new Node(true, worldPos);
        _Grid.Add(worldPos, node);
    }

    public Node WorldPosToNode(Vector3 worldPos)
    {
        worldPos.y = 0;

        Vector3 tilePos = Vector3.zero;
        // Get which Tile the position is on
        RaycastHit hit;
        Ray ray = new Ray
        {
            origin = worldPos + Vector3.up,
            direction = Vector3.down
        };
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        Physics.Raycast(ray, out hit, 1, layerMask);
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Respawn" ||
                hit.transform.tag == "Maze Tile")
            {
                tilePos = hit.transform.position;
               // print("Tile: " + tilePos);
            }
        }
        else
        {
            return FindClosestNode(worldPos);
        }

       // print("World pos: " + worldPos);

        // Snap worldPos to nearest nodePos
        Vector3 normalizedPos = worldPos - tilePos;
       // print("Normalized pos (before round): " + normalizedPos);
        normalizedPos.x = Mathf.Round(normalizedPos.x);
        normalizedPos.z = Mathf.Round(normalizedPos.z);
      //  print("Normalized pos (after round): " + normalizedPos);

        Vector3 nodePos = tilePos + normalizedPos;
        nodePos.y = 0;
        //  print("Node pos: " + nodePos);

        Node n;
        _Grid.TryGetValue(nodePos, out n);
        //print("Found node: " +n);

        // No node found, find the closest node to this position
        if (n == null)
        {
            n = FindClosestNode(worldPos);
        }

        return n;
    }

    private Node FindClosestNode(Vector3 worldPos)
    {
        float minDist = float.MaxValue;
        Node closestNode = null;
        
        foreach (KeyValuePair<Vector3, Node> pos in _Grid) {
            if ((worldPos - pos.Key).sqrMagnitude < minDist) {
                minDist = (worldPos - pos.Key).sqrMagnitude;
                closestNode = pos.Value;
            }
        }
        return closestNode;
    }

    void OnDrawGizmos()
    {
        if (_Grid == null)
        {
            return;
        }

        foreach (var n in _Grid.Values)
        {
            if (!_pathFinding.IsInGrid(n._WorldPos))
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(n._WorldPos,
                    Vector3.one * (_itemScriptableObject._nodeDiameter - .1f));
            }
        }
    }
}