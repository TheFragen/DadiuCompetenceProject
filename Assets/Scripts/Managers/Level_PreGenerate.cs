using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Gamelogic.Extensions;
using UnityEngine;

#pragma warning disable 649
public class Level_PreGenerate : Singleton<Level_PreGenerate>
{
    [SerializeField]
    private GameObject _spawningObject;

    [SerializeField]
    private int _numTilesToSpawn;

    [SerializeField]
    private List<GameObject> _typesOfTiles;

    [SerializeField]
    private List<GameObject> _spawnedTiles;

    [SerializeField]
    private GameObject[,] tiles;

    [SerializeField]
    private Dictionary<Vector3, Tile> tileDict;

    private List<GameObject> possibleTilesUp;
    private List<GameObject> possibleTilesRight;
    private List<GameObject> possibleTilesDown;
    private List<GameObject> possibleTilesLeft;

    [Button]
    void ClearMap()
    {
        foreach (var tile in GameObject.FindGameObjectsWithTag("Maze Tile"))
        {
            #if UNITY_EDITOR
            DestroyImmediate(tile);
            #else
            Destroy(tile);
            #endif
        }
        _spawnedTiles.Clear();
    }

    public Dictionary<Vector3, Tile> GetTileDictionary()
    {
        return tileDict;
    }

    public void RevealMap()
    {
        foreach (var tile in _spawnedTiles)
        {
            foreach (var rend in tile.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
        }
    }

    public List<GameObject> GetTileTypes()
    {
        return _typesOfTiles;
    }

    public void GenerateMaze() {
        _spawnedTiles = new List<GameObject> { _spawningObject };
        StartCoroutine(GenerateMaceDelay(0));
        //GenerateMaceDelay(0);
    }

    public void GenerateWithDelay()
    {
        _spawnedTiles = new List<GameObject> {_spawningObject};
       StartCoroutine(GenerateMaceDelay(.5f));
    }

    IEnumerator GenerateMaceDelay(float delayTime)
    {
        tileDict = new Dictionary<Vector3, Tile>
        {
            { _spawningObject.transform.position, _spawningObject.GetComponent<Tile>() }
        };
        // tiles = new GameObject[_numTilesToSpawn+1, _numTilesToSpawn+1];
        // tiles[_numTilesToSpawn+1/2, _numTilesToSpawn+1/2] = _spawningObject;

        float startTime = Time.realtimeSinceStartup;

        // Up
        possibleTilesUp = _typesOfTiles
            .Where(o => !o.GetComponent<Tile>().BlockedDown)
            .ToList();
        // Right
        possibleTilesRight = _typesOfTiles
            .Where(o => !o.GetComponent<Tile>().BlockedLeft)
            .ToList();
        // Down
        possibleTilesDown = _typesOfTiles
            .Where(o => !o.GetComponent<Tile>().BlockedUp)
            .ToList();
        // Left
        possibleTilesLeft = _typesOfTiles
            .Where(o => !o.GetComponent<Tile>().BlockedRight)
            .ToList();
        Vector3 lastObject = _spawningObject.transform.position;
        Tile lastTile = _spawningObject.GetComponent<Tile>();

        int checks = 0;
        for (int i = 0; i < _numTilesToSpawn; i++)
        {
            Vector3 position = lastObject;
            Vector3 dirVector = Vector3.zero;
            // Create a list of possible directions to go based on which sides of the current tile is blocked
            List<int> allowedDirections = GetAllowedDirectionsSansPhysics(position, tileDict, lastTile);

            // No where to go - Go back to a previous tile
            if (allowedDirections.Count == 0)
            {
                if (tileDict.Count - 1 - checks < 0)
                {
                    print("Breaking: Something went wrong");
                    break;
                }
                lastObject = tileDict.ElementAt(tileDict.Count - 1 - checks).Key;
                lastTile = tileDict.ElementAt(tileDict.Count - 1 - checks).Value;
                i--;
                checks++;
              //  print("Directions = 0");
                continue;
            }
            checks = 0;

            // Pick a direction to generate a tile
            int direction =
                allowedDirections[Random.Range(0, allowedDirections.Count)];

            // Filter out tiles that are blocked in the direction we want to go
            List<GameObject> possibleTiles = new List<GameObject>();
            switch (direction)
            {
                case 0:
                    // Up
                    dirVector = new Vector3(0, 0, 1);
                    possibleTiles = new List<GameObject>(possibleTilesUp);
                    break;
                case 1:
                    // Right
                    dirVector = new Vector3(1, 0, 0);
                    possibleTiles = new List<GameObject>(possibleTilesRight);
                    break;
                case 2:
                    // Down
                    dirVector = new Vector3(0, 0, -1);
                    possibleTiles = new List<GameObject>(possibleTilesDown);
                    break;
                case 3:
                    // Left
                    dirVector = new Vector3(-1, 0, 0);
                    possibleTiles = new List<GameObject>(possibleTilesLeft);
                    break;
            }

            Vector3 nextTilePos = position + dirVector * 3;
            // Up
            Tile northTile;
            tileDict.TryGetValue(nextTilePos + Vector3.forward * 3, out northTile);
            if (northTile != null && northTile.BlockedDown)
            {
            //    print("Removed");
                possibleTiles.RemoveAll(o => !o.GetComponent<Tile>().BlockedUp);
            }
            // Right
            Tile eastTile;
            tileDict.TryGetValue(nextTilePos + Vector3.right * 3, out eastTile);
            if (eastTile != null && eastTile.BlockedLeft)
            {
             //   print("Removed");
                possibleTiles.RemoveAll(o => !o.GetComponent<Tile>().BlockedRight);
            }
            // Down
            Tile southTile;
            tileDict.TryGetValue(nextTilePos + Vector3.back * 3, out southTile);
            if (southTile != null && southTile.BlockedUp)
            {
          //      print("Removed");
                possibleTiles.RemoveAll(o => !o.GetComponent<Tile>().BlockedDown);
            }
            // Left
            Tile westTile;
            tileDict.TryGetValue(nextTilePos + Vector3.left * 3, out westTile);
            if (westTile != null && westTile.BlockedRight)
            {
           //     print("Removed");
                possibleTiles.RemoveAll(o => !o.GetComponent<Tile>().BlockedLeft);
            }

            // No where to go - Go back to a previous tile
            if (possibleTiles.Count == 0) {
                if (i <= 0)
                {
                    // Completely closed maze - now lets just return the maze then
                    print("Returning maze after checking for a new direction " +checks +" times.");
                    break;
                }
                lastObject = tileDict.ElementAt(tileDict.Count - 1 - checks).Key;
                lastTile = tileDict.ElementAt(tileDict.Count - 1 - checks).Value;
                checks--;
                i--;
             //   print("Possibletiles = 0");
                checks++;
              //  yield return null;
                continue;
            }
            checks = 0;
          //  print("Num possible tiles: " +possibleTiles.Count);
            GameObject chosenTile =
                possibleTiles[Random.Range(0, possibleTiles.Count)];
            Vector3 newPos = position + dirVector * 3;
            tileDict.Add(newPos, chosenTile.GetComponent<Tile>());
            lastObject = newPos;
            lastTile = chosenTile.GetComponent<Tile>();

            if (delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }
        }
        foreach (var element in tileDict)
        {
            if (element.Value.prefab == null)
            {
                continue;
            }
            Instantiate(element.Value.prefab, element.Key, Quaternion.identity);
        }
        // Yields while all tiles get set up
        yield return new WaitForEndOfFrame();
        print("<color=blue>Time of execution:</color> " + (Time.realtimeSinceStartup - startTime));
        GameManager.Instance.SetLevelGenerated();
        yield return null;  
    }

    

    List<int> GetAllowedDirections(Vector3 inputTile, int layerMask, Tile tile)
    {
      //  Tile tile = inputTile.GetComponent<Tile>();
        List<int> randomObjectList = new List<int>();
        if (!tile.BlockedUp && !Physics.CheckSphere(
                inputTile + Vector3.forward * 3, .1f,
                layerMask)) {
            randomObjectList.Add(0);
        }
        if (!tile.BlockedRight && !Physics.CheckSphere(
                inputTile + Vector3.right * 3, .1f,
                layerMask)) {
            randomObjectList.Add(1);
        }
        if (!tile.BlockedDown && !Physics.CheckSphere(
                inputTile + Vector3.back * 3, .1f,
                layerMask)) {
            randomObjectList.Add(2);
        }
        if (!tile.BlockedLeft && !Physics.CheckSphere(
                inputTile + Vector3.left * 3, .1f,
                layerMask)) {
            randomObjectList.Add(3);
        }
        return randomObjectList;
    }

    List<int> GetAllowedDirectionsSansPhysics(Vector3 inputTile, Dictionary<Vector3, Tile> dict, Tile tile) {
        List<int> randomObjectList = new List<int>();

        // Checks if the previous tile is open in any of the four directions, and checks if there already is something there
        Tile northTile;
        dict.TryGetValue(inputTile + Vector3.forward * 3, out northTile);
        if (!tile.BlockedUp && northTile == null) {
            randomObjectList.Add(0);
        }
        // Right
        Tile eastTile;
        dict.TryGetValue(inputTile + Vector3.right * 3, out eastTile);
        if (!tile.BlockedRight && eastTile == null) {
            randomObjectList.Add(1);
        }
        // Down
        Tile southTile;
        dict.TryGetValue(inputTile + Vector3.back * 3, out southTile);
        if (!tile.BlockedDown && southTile == null) {
            randomObjectList.Add(2);
        }
        // Left
        Tile westTile;
        dict.TryGetValue(inputTile + Vector3.left * 3, out westTile);
        if (!tile.BlockedLeft && westTile == null) {
            randomObjectList.Add(3);
        }
        
        return randomObjectList;
    }



    string PrintHorizontal(int[] arr)
    {
        string s = "";
        foreach (var i in arr)
        {
            s += "," + i;
        }
        return s;
    }

    GameObject GenerateTile(Vector3 position, Vector3 direction,
        GameObject chosenTile)
    {
        position.y = 0;
        Vector3 newPos = position + direction * 3;
        return Instantiate(chosenTile, newPos, Quaternion.identity);
    }
}