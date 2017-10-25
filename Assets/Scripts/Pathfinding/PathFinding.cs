using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Priority_Queue;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField]
    private Grid _Grid;
    private bool _LevelIsGenerated;

    [SerializeField]
    private Items _itemScriptableObject;
    private Dictionary<GameObject, List<Node>> _PathsActive = new Dictionary<GameObject, List<Node>>();

    private void OnEnable()
    {
        GameManager.OnLevelGenerated += SetLevel;
    }
    private void OnDisable()
    {
        GameManager.OnLevelGenerated -= SetLevel;
    }

    public bool IsInGrid(Vector3 nodePos) {
        if (_PathsActive == null) return false;
        foreach (var players in _PathsActive)
        {
            foreach (var node in players.Value)
            {
                if (node._WorldPos == nodePos)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddToActivePaths(GameObject go, List<Node> path)
    {
        _PathsActive.Add(go, path);
    }

    public void RemoveFromActivePaths(GameObject go) {
        _PathsActive.Remove(go);
    }

    void SetLevel()
    {
        _LevelIsGenerated = true;
    }

    public List<Node> GeneratePath(Vector3 start, Vector3 target)
    {
        List<Node> path = new List<Node>();
        float startTime = Time.realtimeSinceStartup;

        if (!_LevelIsGenerated) {
            Debug.LogError("No level generated yet");
            return null;
        }
        SimplePriorityQueue<Node> open = new SimplePriorityQueue<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        Node startNode = _Grid.WorldPosToNode(start);
        Node targetNode = _Grid.WorldPosToNode(target);

        if (startNode == null) {
            Debug.LogError("StartNode is null");
        }
        if (targetNode == null) {
            print("TargetNode is null");
            Debug.LogError("TargetNode is null");
        }
        if (startNode == null || targetNode == null) {
            return null;
        }

        open.Enqueue(startNode, startNode.FCost());
        while (open.Count > 0) {
            Node currentNode = open.Dequeue();

            // Return if a path is found
            if (currentNode == targetNode) {
                path = Retrace(startNode, targetNode);
                //print("<color=green>Pathfinding:</color> Time of execution: " + (Time.realtimeSinceStartup - startTime));
                return path;
            }

            // Add current to closed
            closed.Add(currentNode);

            foreach (var neighbour in _Grid.GetNeighbours(currentNode)) {

                // Distance to startNode through parent
                int newCost = currentNode._GCost + ManhattenDistanceInt(currentNode, neighbour);
                if (!closed.Contains(neighbour) || newCost < neighbour._GCost)
                {
                    neighbour._GCost = newCost;
                    neighbour._HCost = ManhattenDistanceInt(neighbour, targetNode);
                    open.Enqueue(neighbour, neighbour.FCost());
                    neighbour._parentNode = currentNode;
                    closed.Add(neighbour);
                }
            }
        }

        return path;
    }

    float ManhattenDistance(Node s, Node e)
    {
        float dx = Mathf.Abs(s._WorldPos.x - e._WorldPos.x);
        float dz = Mathf.Abs(s._WorldPos.z - e._WorldPos.z);
        return dx + dz;
    }

    int ManhattenDistanceInt(Node s, Node e)
    {
        return Mathf.RoundToInt(ManhattenDistance(s, e));
    }

    List<Node> Retrace(Node startNode, Node targetNode)
    {
        // Get's the parents between target and start, and saves it to the _Path list
        List<Node> path = new List<Node>();
        Node current = targetNode;
        while (current != startNode)
        {
            path.Add(current);
            current = current._parentNode;
        }

        // Reverse the list because it's going from target to start
        path.Reverse();
        return path;
    }
    
    
    private void OnDrawGizmos()
    {
        if (_PathsActive == null || _PathsActive.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var player in _PathsActive)
        {
            foreach (var node in player.Value)
            {
                Gizmos.DrawCube(node._WorldPos, Vector3.one * (_itemScriptableObject._nodeDiameter - .1f));
            }
        }
    }
}