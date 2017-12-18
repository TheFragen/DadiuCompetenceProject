using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UnityEngine;

public class Grid : Singleton<Grid>
{
    public PathFinding _PathFinding;

    [SerializeField]
    private Items _itemScriptableObject;

    private Dictionary<Vector3, Node> _Grid;

    public Dictionary<Vector3, Node> GetGrid()
    {
        return _Grid;
    }

    public void ClearGrid()
    {
        _Grid.Clear();
    }

    private void Awake()
    {
        _Grid = new Dictionary<Vector3, Node>(_customComparer);
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // North
        Node north;
        _Grid.TryGetValue(node._WorldPos + new Vector3(0, 0, 1), out north);
        if (north != null && node._TileIndex != 6 && node._TileIndex != 8)
        {
            if (node._TileIndex == 7)
            {
                if (!north._ParentTile.BlockedDown &&
                    !node._ParentTile.BlockedUp)
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
        if (east != null && node._TileIndex != 8 && node._TileIndex != 2)
        {
            if (node._TileIndex == 5)
            {
                if (!node._ParentTile.BlockedRight &&
                    !east._ParentTile.BlockedLeft)
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
        if (south != null && node._TileIndex != 0 && node._TileIndex != 2)
        {
            if (node._TileIndex == 1)
            {
                if (!node._ParentTile.BlockedDown && !south._ParentTile.BlockedUp)
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
        if (west != null && node._TileIndex != 6 && node._TileIndex != 0)
        {
            if (node._TileIndex == 3)
            {
                if (!node._ParentTile.BlockedLeft && !west._ParentTile.BlockedRight)
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

    public Dictionary<Vector3, Node> GetNodes()
    {
        return _Grid;
    }

    public void AddToGrid(Vector3 worldPos, Tile tile, int tileIndex)
    {
        worldPos.y = 0;
        Node node = new Node(worldPos, tile, tileIndex);
        _Grid.Add(worldPos, node);
    }
    public static Vector3CoordComparer _customComparer = new Vector3CoordComparer();

    public Node WorldPosToNode(Vector3 worldPos)
    {
        worldPos.y = 0;
        Node n;
        _Grid.TryGetValue(worldPos, out n);
        // No node found, find the closest node to this position
        if (n == null) {
            Debug.Log("No node at pos: " +worldPos);
            return FindClosestNode(worldPos, worldPos);
        }

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
            Debug.Log("No MazeTile");
            return FindClosestNode(worldPos, Vector3.positiveInfinity);
        }
        
        Vector3 normalizedPos = worldPos - tilePos;
        normalizedPos.x = Mathf.Round(normalizedPos.x);
        normalizedPos.z = Mathf.Round(normalizedPos.z);

        Vector3 nodePos = tilePos + normalizedPos;
        nodePos.y = 0;

        

        return n;
    }

    private Node FindClosestNode(Vector3 worldPos, Vector3 nodePos)
    {
        float minDist = Mathf.Infinity;
        Node closestNode = null;
        
        foreach (KeyValuePair<Vector3, Node> pos in _Grid)
        {
            float dist = (worldPos - pos.Value._WorldPos).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
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
            if (!_PathFinding.IsInGrid(n._WorldPos))
            {
                Gizmos.color = Color.white;
             //   Gizmos.DrawCube(n._WorldPos, Vector3.one * (_itemScriptableObject._nodeDiameter - .1f));
                
            }
            drawString(n._WorldPos.ToString(), n._WorldPos, Color.black);
        }
    }

    static void drawString(string text, Vector3 worldPos, Color? colour = null) {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0) {
            UnityEditor.Handles.EndGUI();
            return;
        }

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }
}

public class Vector3CoordComparer : IEqualityComparer<Vector3>
{
    public bool Equals(Vector3 a, Vector3 b)
    {
        if (Mathf.Abs(a.x - b.x) > 0.001) return false;
        if (Mathf.Abs(a.y - b.y) > 0.001) return false;
        if (Mathf.Abs(a.z - b.z) > 0.001) return false;

        return true; //indeed, very close
    }

    public int GetHashCode(Vector3 obj)
    {
        //a cruder than default comparison, allows to compare very close-vector3's into same hash-code.
        return Math.Round(obj.x, 3).GetHashCode()
               ^ Math.Round(obj.y, 3).GetHashCode() << 2
               ^ Math.Round(obj.z, 3).GetHashCode() >> 2;
    }
}