using UnityEngine;

[System.Serializable]
public class Node
{
    public Vector3 _WorldPos;
    public bool _Walkable;
    public int _HCost = 0;
    public int _GCost = 0;
    public Node _parentNode;

    public Node(bool walkable, Vector3 worldPos)
    {
        _WorldPos = worldPos;
        _Walkable = walkable;
    }

    public int FCost()
    {
        return _GCost + _HCost;
    }
}