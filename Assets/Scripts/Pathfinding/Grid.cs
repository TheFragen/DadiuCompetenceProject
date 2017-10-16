using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public PathFinding _pathFinding;

    private Dictionary<Vector3, Node> _Grid;
    private Vector3 _testPos;

    private void Awake()
    {
        _Grid = new Dictionary<Vector3, Node>();
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // North
        Node north;
        _Grid.TryGetValue(node._WorldPos + Vector3.forward, out north);
        if (north != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(0, .5f, .5f), .1f,
                layerMask))
        {
            neighbours.Add(north);
        }

        // East
        Node east;
        _Grid.TryGetValue(node._WorldPos + Vector3.right, out east);
        if (east != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(.5f, .5f, 0), .1f,
                layerMask))
        {
            neighbours.Add(east);
        }

        // South
        Node south;
        _Grid.TryGetValue(node._WorldPos + Vector3.back, out south);
        if (south != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(0, .5f, -.5f),
                .1f, layerMask))
        {
            neighbours.Add(south);
        }

        // West
        Node west;
        _Grid.TryGetValue(node._WorldPos + Vector3.left, out west);
        if (west != null &&
            !Physics.CheckSphere(node._WorldPos + new Vector3(-.5f, .5f, 0),
                .1f, layerMask))
        {
            neighbours.Add(west);
        }

        return neighbours;
    }

    public void AddToGrid(Vector3 worldPos)
    {
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
            }
        }
        else
        {
            return null;
        }

        // Snap worldPos to nearest nodePos
        Vector3 normalizedPos = worldPos - tilePos;
        normalizedPos.x = Mathf.Round(normalizedPos.x);
        normalizedPos.z = Mathf.Round(normalizedPos.z);

        Vector3 nodePos = tilePos + normalizedPos;

        Node n;
        _Grid.TryGetValue(nodePos, out n);

        return n;
    }

    void OnDrawGizmos()
    {
        if (_testPos != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_testPos, .1f);
        }
    }
}