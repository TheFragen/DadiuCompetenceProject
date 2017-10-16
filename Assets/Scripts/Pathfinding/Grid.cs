using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public PathFinding _pathFinding;

    private Dictionary<Vector3, Node> _Grid;
    private Vector3 _testPos;
    
    private void Start()
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
        _Grid.TryGetValue(node._WorldPos + Vector3.back, out north);
        if (north != null)
        {
            // Raycast this direction to see if there is a wall
            RaycastHit hit;
            Vector3 direction = node._WorldPos - north._WorldPos;
            Physics.Raycast(node._WorldPos, direction, out hit,
                direction.magnitude + .25f, layerMask);

            print("North: " + Physics.CheckSphere(north._WorldPos + new Vector3(0, 0, .5f), .1f));

            if (hit.transform == null || hit.transform.name != "Wall")
            {
                neighbours.Add(north);
            }
        }

        // East
        Node east;
        _Grid.TryGetValue(node._WorldPos + Vector3.right, out east);
        if (east != null)
        {
            // Raycast this direction to see if there is a wall
            RaycastHit hit;
            Vector3 direction = node._WorldPos - east._WorldPos;
            Physics.Raycast(node._WorldPos, direction, out hit,
                direction.magnitude + .25f, layerMask);

            if (hit.transform == null || hit.transform.name != "Wall")
            {
                neighbours.Add(east);
            }
        }

        // South
        Node south;
        _Grid.TryGetValue(node._WorldPos + Vector3.forward, out south);
        if (south != null)
        {
            // Raycast this direction to see if there is a wall
            RaycastHit hit;
            Vector3 direction = node._WorldPos - south._WorldPos;
            Physics.Raycast(node._WorldPos, direction, out hit,
                direction.magnitude + .25f, layerMask);

            if (hit.transform == null || hit.transform.name != "Wall")
            {
                neighbours.Add(south);
            }
        }

        // West
        Node west;
        _Grid.TryGetValue(node._WorldPos + Vector3.left, out west);
        if (west != null)
        {
            // Raycast this direction to see if there is a wall
            RaycastHit hit;
            Vector3 direction = node._WorldPos - west._WorldPos;
            Physics.Raycast(node._WorldPos, direction, out hit,
                direction.magnitude + .25f, layerMask);

            if (hit.transform == null || hit.transform.name != "Wall")
            {
                neighbours.Add(west);
            }
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
        _testPos = nodePos;

        Node n;
        _Grid.TryGetValue(nodePos, out n);

        return n;
    }

    void OnDrawGizmos()
    {
        if (_testPos != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_testPos, .5f);
        }
    }
}