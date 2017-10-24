using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
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

        if (!_LevelIsGenerated)
        {
            Debug.LogError("No level generated yet");
            return null;
        }
        List<Node> open = new List<Node>();
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
        print("Creating path");
        if (startNode == null || targetNode == null) {
            return null;
        }
        open.Add(startNode);
        
        while (open.Count > 0) {
            Node currentNode = open[0];
            // Find lowest fCost node in open
            foreach (var n in open) {
                if (n.FCost() <= currentNode.FCost() &&
                    n._HCost < currentNode._HCost) {
                    currentNode = n;
                }
            }

            // Remove current from open
            open.Remove(currentNode);
            closed.Add(currentNode);

            // Return if a path is found
            if (currentNode == targetNode) {
                //print("Found the path dude");
                path = Retrace(startNode, targetNode);
                print("In returning: " + path.Count);
                //print("<color=green>Pathfinding:</color> Time of execution: " + (Time.realtimeSinceStartup - startTime));
                return path;
            }

            foreach (var neighbour in _Grid.GetNeighbours(currentNode)) {
                if (!neighbour._Walkable || closed.Contains(neighbour)) {
                    continue;
                }

                int newCost = currentNode._GCost + ManhattenDistanceInt(currentNode, neighbour);
                if (newCost < neighbour._GCost || !open.Contains(neighbour)) {
                    // Distance to startNode through parent
                    neighbour._GCost = newCost;

                    // Distance to target
                    neighbour._HCost =
                        ManhattenDistanceInt(neighbour, targetNode);

                    neighbour._parentNode = currentNode;

                    if (!open.Contains(neighbour)) {
                        open.Add(neighbour);
                    }
                }
            }
        }
        print("Out returning: " +path.Count);
        return path;
    }
    
    IEnumerator FindPathCoroutine(float WaitTime)
    {
        yield return null;

        /*
        OPEN //the set of nodes to be evaluated
            CLOSED //the set of nodes already evaluated
        add the start node to OPEN


        loop
            current = node in OPEN with the lowest f_cost
            remove current from OPEN
            add current to CLOSED


            if current is the target node //path has been found
                return


            foreach neighbour of the current node
                if neighbour is not traversable or neighbour is in CLOSED
                    skip to the next neighbour


                if new path to neighbour is shorter OR neighbour is not in OPEN
                    set f_cost of neighbour
                    set parent of neighbour to current
                    if neighbour is not in OPEN
                        add neighbour to OPEN
        */
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