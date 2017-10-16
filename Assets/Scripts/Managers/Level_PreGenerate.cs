using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;

#pragma warning disable 649
public class Level_PreGenerate : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawningObject;

    [SerializeField]
    private int _numTilesToSpawn;

    [SerializeField]
    private List<GameObject> _typesOfTiles;

    [SerializeField]
    private List<GameObject> _spawnedTiles;

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

    [Button]
    void RevealMap()
    {
        foreach (var tile in _spawnedTiles)
        {
            foreach (var rend in tile.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
        }
    }

    [Button]
    void GenerateWithDelay()
    {
        _spawnedTiles = new List<GameObject> {_spawningObject};
        StartCoroutine(GenerateMaceDelay(.1f));
    }

    IEnumerator GenerateMaceDelay(float delayTime)
    {
        GameObject lastObject = _spawningObject;
        for (int i = 0; i < _numTilesToSpawn; i++)
        {
            Vector3 position = lastObject.transform.position;
            Vector3 dirVector = Vector3.zero;

            
            // Defines mask such that Physics.CheckSphere ignores the Player collision layer
            int layerMask = 1 << 8; //0b0001_0000_0000
            layerMask = ~layerMask; // 0b1110_1111_1111

            // Create a list of possible directions to go based on which sides of the current tile is blocked
            List<int> randomObjectList =
                GetAllowedDirections(lastObject.transform.position, layerMask, lastObject.GetComponent<Tile>());

            // No where to go - Go back to a previous tile
            if (randomObjectList.Count == 0)
            {
                _spawnedTiles.RemoveAt(_spawnedTiles.Count - 1);
                lastObject = _spawnedTiles.Last();
                --i;
                continue;
            }

            // Pick a direction to generate a tile
            int direction =
                randomObjectList[Random.Range(0, randomObjectList.Count - 1)];

            // Filter out tiles that are blocked in the direction we want to go
            List<GameObject> possibleTiles = new List<GameObject>();
            switch (direction)
            {
                case 0:
                    // Up
                    dirVector = new Vector3(0, 0, 1);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedDown)
                        .ToList();
                    break;
                case 1:
                    // Right
                    dirVector = new Vector3(1, 0, 0);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedLeft)
                        .ToList();
                    break;
                case 2:
                    // Down
                    dirVector = new Vector3(0, 0, -1);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedUp)
                        .ToList();
                    break;
                case 3:
                    // Left
                    dirVector = new Vector3(-1, 0, 0);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedRight)
                        .ToList();
                    break;
            }

            // Pick a tile from the remaining possible tiles
            List<int> nextUpAllowedDirections =
                GetAllowedDirections(lastObject.transform.position + (dirVector*3), layerMask, lastObject.GetComponent<Tile>());
          //  print(PrintHorizontal(nextUpAllowedDirections.ToArray()));
            /*             direction =
                            nextUpAllowedDirections[Random.Range(0, nextUpAllowedDirections.Count - 1)];

                       switch (direction) {
                            case 0:
                                // Up
                                switch (blockedDirection)
                                {
                                    case 0:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown)
                                            .ToList();
                                        break;
                                    case 1:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown && !o.GetComponent<Tile>().BlockedLeft)
                                            .ToList();
                                        break;
                                    case 2:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown && !o.GetComponent<Tile>().BlockedUp)
                                            .ToList();
                                        break;
                                    case 3:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown && !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                }
                                break;
                            case 1:
                                // Right
                                switch (blockedDirection) {
                                    case 0:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown && !o.GetComponent<Tile>().BlockedLeft)
                                            .ToList();
                                        break;
                                    case 1:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedLeft)
                                            .ToList();
                                        break;
                                    case 2:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedLeft && !o.GetComponent<Tile>().BlockedUp)
                                            .ToList();
                                        break;
                                    case 3:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedLeft && !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                }
                                break;
                            case 2:
                                // Down
                                switch (blockedDirection) {
                                    case 0:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown && !o.GetComponent<Tile>().BlockedUp)
                                            .ToList();
                                        break;
                                    case 1:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedUp && !o.GetComponent<Tile>().BlockedLeft)
                                            .ToList();
                                        break;
                                    case 2:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedUp)
                                            .ToList();
                                        break;
                                    case 3:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedUp && !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                }
                                break;
                            case 3:
                                // Left
                                switch (blockedDirection) {
                                    case 0:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedDown && !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                    case 1:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedLeft && !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                    case 2:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedUp && !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                    case 3:
                                        possibleTiles = possibleTiles
                                            .Where(o => !o.GetComponent<Tile>().BlockedRight)
                                            .ToList();
                                        break;
                                }
                                break;
                        }
             */

            GameObject chosenTile =
                possibleTiles[Random.Range(0, possibleTiles.Count)];

            // Spawn the chosen tile
            lastObject = GenerateTile(position, dirVector, chosenTile);
            _spawnedTiles.Add(lastObject);
            yield return new WaitForSeconds(delayTime);
        }
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

    string PrintHorizontal(int[] arr)
    {
        string s = "";
        foreach (var i in arr)
        {
            s += "," + i;
        }
        return s;
    }

   [Button]
    void GenerateMaze()
    {
        _spawnedTiles = new List<GameObject> { _spawningObject };
        GameObject lastObject = _spawningObject;
        for (int i = 0; i < _numTilesToSpawn; i++)
        {
            Vector3 position = lastObject.transform.position;
            Vector3 dirVector = Vector3.zero;

            // Create a list of possible directions to go based on which sides of the current tile is blocked
            Tile lastObjectTile = lastObject.GetComponent<Tile>();
            List<int> randomObjectList = new List<int>();

            // Defines mask such that Physics.CheckSphere ignores the Player collision layer
            int layerMask = 1 << 8; //0b0001_0000_0000
            layerMask = ~layerMask; // 0b1110_1111_1111

            if (!lastObjectTile.BlockedUp && !Physics.CheckSphere(
                    lastObject.transform.position + Vector3.forward * 3, .1f,
                    layerMask))
            {
                randomObjectList.Add(0);
            }
            if (!lastObjectTile.BlockedRight && !Physics.CheckSphere(
                    lastObject.transform.position + Vector3.right * 3, .1f,
                    layerMask))
            {
                randomObjectList.Add(1);
            }
            if (!lastObjectTile.BlockedDown && !Physics.CheckSphere(
                    lastObject.transform.position + Vector3.back * 3, .1f,
                    layerMask))
            {
                randomObjectList.Add(2);
            }
            if (!lastObjectTile.BlockedLeft && !Physics.CheckSphere(
                    lastObject.transform.position + Vector3.left * 3, .1f,
                    layerMask))
            {
                randomObjectList.Add(3);
            }

            // No where to go - Go back to a previous tile
            if (randomObjectList.Count == 0)
            {
                _spawnedTiles.RemoveAt(_spawnedTiles.Count - 1);
                lastObject = _spawnedTiles.Last();
                --i;
                continue;
            }

            // Pick a direction to generate a tile
            int direction =
                randomObjectList[Random.Range(0, randomObjectList.Count - 1)];

            // Filter out tiles that are blocked in the direction we want to go
            List<GameObject> possibleTiles = new List<GameObject>();
            switch (direction)
            {
                case 0:
                    // Up
                    dirVector = new Vector3(0, 0, 1);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedDown)
                        .ToList();
                    break;
                case 1:
                    // Right
                    dirVector = new Vector3(1, 0, 0);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedLeft)
                        .ToList();
                    break;
                case 2:
                    // Down
                    dirVector = new Vector3(0, 0, -1);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedUp)
                        .ToList();
                    break;
                case 3:
                    // Left
                    dirVector = new Vector3(-1, 0, 0);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.GetComponent<Tile>().BlockedRight)
                        .ToList();
                    break;
            }

            // Pick a tile from the remaining possible tiles
            GameObject chosenTile =
                possibleTiles[Random.Range(0, possibleTiles.Count)];

            // Spawn the chosen tile
            lastObject = GenerateTile(position, dirVector, chosenTile);
            _spawnedTiles.Add(lastObject);
        }
    }

    GameObject GenerateTile(Vector3 position, Vector3 direction,
        GameObject chosenTile)
    {
        position.y = 0;
        Vector3 newPos = position + direction * 3;
        return Instantiate(chosenTile, newPos, Quaternion.identity);
    }
}