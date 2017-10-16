using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField]
    private Grid _Grid;

    [SerializeField]
    private Transform _Start, _Target;

    [SerializeField]
    private Items _itemScriptableObject;

    [SerializeField]
    private float _WaitTime = .5f;
    [SerializeField]
    private bool _useDelay = false;

    [SerializeField]
    private bool _EveryFrame = false;

    private List<Node> _Path;
    private bool _coroutineRunning = false;
    private Vector3 _lastFramePos = Vector3.zero;

    private Vector3 _workingNode, _pivotNode;

    public bool IsInGrid(Vector3 nodePos) {
        if (_Path == null) return false;
        return _Path.Contains(_Grid.WorldPosToNode(nodePos));
    }

    void Update()
    {
        if (_EveryFrame && _Target.transform.position != _lastFramePos)
        {
            StartCoroutine(FindPathCoroutine(0));
            _lastFramePos = _Target.transform.position;
        }
    }

    [Button]
    void FindPath()
    {
        if (_useDelay)
        {
            StartCoroutine(FindPathCoroutine(_WaitTime));
        }
        else
        {
            StartCoroutine(FindPathCoroutine(0));
        }
    }
    
    IEnumerator FindPathCoroutine(float WaitTime)
    {
        float startTime = Time.realtimeSinceStartup;

        if (_Start == null || _Target == null)
        {
            Debug.LogError("Start or target is null");
            yield return null;
        }
        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();


        Node startNode = _Grid.WorldPosToNode(_Start.position);
        Node targetNode = _Grid.WorldPosToNode(_Target.position);

        if (startNode == null)
        {
            Debug.LogError("StartNode is null");
        }
        if (targetNode == null)
        {
            Debug.LogError("TargetNode is null");
        }
        if (startNode == null && targetNode == null)
        {
            enabled = false;
        }

        _coroutineRunning = true;
        open.Add(startNode);

     //   int runs = 0;
        while (open.Count > 0)
        {
         //   print(runs++);
            Node currentNode = open[0];
            // Find lowest fCost node in open
            foreach (var n in open)
            {
                if (n.FCost() <= currentNode.FCost() &&
                    n._HCost < currentNode._HCost)
                {
                    currentNode = n;
                }
            }

            // Remove current from open
            open.Remove(currentNode);
            closed.Add(currentNode);

            // Return if a path is found
            if (currentNode == targetNode)
            {
                //print("Found the path dude");
                Retrace(startNode, targetNode);
              //  print("Time of execution: " +(Time.realtimeSinceStartup - startTime));
                _coroutineRunning = false;
                yield return null;
            }

            _pivotNode = currentNode._WorldPos;

            foreach (var neighbour in _Grid.GetNeighbours(currentNode))
            {
                if (!neighbour._Walkable || closed.Contains(neighbour))
                {
                    continue;
                }

                _workingNode = neighbour._WorldPos;

                int newCost = currentNode._GCost + ManhattenDistanceInt(currentNode, neighbour);
                if (newCost < neighbour._GCost || !open.Contains(neighbour))
                {
                    // Distance to startNode through parent
                    neighbour._GCost = newCost;

                    // Distance to target
                    neighbour._HCost =
                        ManhattenDistanceInt(neighbour, targetNode);

                    neighbour._parentNode = currentNode;

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }

                if((int)WaitTime > 0)
                {
                    yield return new WaitForSeconds(WaitTime);
                }
            }
        }

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
        float D = 1;
        float dx = Mathf.Abs(s._WorldPos.x - e._WorldPos.x);
        float dy = Mathf.Abs(s._WorldPos.y - e._WorldPos.y);
        return D * (dx + dy);
    }

    int ManhattenDistanceInt(Node s, Node e)
    {
        return Mathf.RoundToInt(ManhattenDistance(s, e));
    }

    void Retrace(Node startNode, Node targetNode)
    {
        // Get's the parents between target and start, and saves it to the _Path list
        _Path = new List<Node>();
        Node current = targetNode;
        while (current != startNode)
        {
            _Path.Add(current);
            current = current._parentNode;
        }

        // Reverse the list because it's going from target to start
        _Path.Reverse();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_workingNode + Vector3.up, Vector3.one * (_itemScriptableObject._nodeDiameter - .1f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_pivotNode + Vector3.up, Vector3.one * (_itemScriptableObject._nodeDiameter - .1f));

        if (_Path == null || _Path.Count == 0)
        {
            return;
        }

        Gizmos.color = Color.green;
        foreach (var node in _Path)
        {
            Gizmos.DrawCube(node._WorldPos, Vector3.one * (_itemScriptableObject._nodeDiameter - .1f));
        }
    }
}