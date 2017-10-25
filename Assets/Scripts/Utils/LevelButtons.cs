using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using EasyButtons;
using UnityEditor.VersionControl;

public class LevelButtons : MonoBehaviour {
    [SerializeField]
    Tile spawningObject;
    private List<GameObject> types;
    [SerializeField]
    private List<Tile> tileTypes;

    [SerializeField]
    private int tilesToSpawn = 10;

    private Vector3 spawnPos;

    Dictionary<Vector3, Tile> dict = new Dictionary<Vector3, Tile>();

    [Button]
    void GenerateMaze()
    {
        Level_PreGenerate.Instance.GenerateMaze();
    }

    [Button]
    void GenerateWithDelay()
    {
        Level_PreGenerate.Instance.GenerateWithDelay();
    }

    [Button]
    void RevealMap()
    {
        Level_PreGenerate.Instance.RevealMap();
    }
}