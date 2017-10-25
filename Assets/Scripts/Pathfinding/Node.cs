using UnityEngine;

[System.Serializable]
public class Node
{
    public Vector3 _WorldPos;
    public Tile _ParentTile;
    public int _TileIndex;
    public int _HCost = 0;
    public int _GCost = 0;
    public Node _parentNode;

    public Node(Vector3 worldPos, Tile tile, int tileIndex)
    {
        _WorldPos = worldPos;
        _ParentTile = tile;
        _TileIndex = tileIndex;
    }

    public int FCost()
    {
        return _GCost + _HCost;
    }
}