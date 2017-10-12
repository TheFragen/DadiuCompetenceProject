using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;

public class Level_PreGenerate : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawningObject;

    [SerializeField]
    private int numTilesToSpawn;

    [SerializeField]
    private List<GameObject> _typesOfTiles;

    [SerializeField]
    private List<GameObject> _spawnedTiles = new List<GameObject>();

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
        _spawnedTiles.Add(_spawningObject);
        StartCoroutine(GenerateMaceDelay(.1f));
    }

    IEnumerator GenerateMaceDelay(float delayTime)
    {
        int checks = 0;
        GameObject lastObject = _spawningObject;
        for (int i = 0; i < numTilesToSpawn; i++)
        {
            Vector3 position = lastObject.transform.position;
            Vector3 dirVector = Vector3.zero;

            // Create a list of possible directions to go based on which sides of the current tile is blocked
            Tile lastObjectTile = lastObject.GetComponent<Tile>();
            List<int> randomObjectList = new List<int>();

            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            if (!lastObjectTile.BlockedUp && !Physics.CheckSphere(lastObject.transform.position + Vector3.forward * 3, .1f, layerMask))
            {
                randomObjectList.Add(0);
            }
            if (!lastObjectTile.BlockedRight && !Physics.CheckSphere(lastObject.transform.position + Vector3.right * 3, .1f, layerMask))
            {
                randomObjectList.Add(1);
            }
            if (!lastObjectTile.BlockedDown && !Physics.CheckSphere(lastObject.transform.position + Vector3.back * 3, .1f, layerMask))
            {
                randomObjectList.Add(2);
            }
            if (!lastObjectTile.BlockedLeft && !Physics.CheckSphere(lastObject.transform.position + Vector3.left * 3, .1f, layerMask))
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
            int direction = randomObjectList[Random.Range(0, randomObjectList.Count-1)];
            
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
            // print("Direction: " + direction);
            GameObject chosenTile = possibleTiles[Random.Range(0, possibleTiles.Count)];

            // Spawn the chosen tile
            GameObject go = GenerateTile(position, dirVector, chosenTile);
            if (go == null)
            {
                print("GO was null!");
                if (checks == 4)
                {
                    _spawnedTiles.RemoveAt(_spawnedTiles.Count - 1);
                    if (_spawnedTiles.Count == 0)
                    {
                        _spawnedTiles.Add(_spawningObject);
                    }
                    lastObject = _spawnedTiles.Last();
                    checks = 0;
                }
                else
                {
                    --i;
                    checks++;
                }
            }
            else
            {
                lastObject = go;
                _spawnedTiles.Add(lastObject);
                checks = 0;
            }
            yield return new WaitForSeconds(delayTime);
        }
    }

    [Button]
    void GenerateMaze()
    {
        int checks = 0;
        GameObject lastObject = _spawningObject;
        for (int i = 0; i < numTilesToSpawn; i++) {
            Vector3 position = lastObject.transform.position;
            Vector3 dirVector = Vector3.zero;

            // Create a list of possible directions to go based on which sides of the current tile is blocked
            Tile lastObjectTile = lastObject.GetComponent<Tile>();
            List<int> randomObjectList = new List<int>();

            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            if (!lastObjectTile.BlockedUp && !Physics.CheckSphere(lastObject.transform.position + Vector3.forward * 3, .1f, layerMask)) {
                randomObjectList.Add(0);
            }
            if (!lastObjectTile.BlockedRight && !Physics.CheckSphere(lastObject.transform.position + Vector3.right * 3, .1f, layerMask)) {
                randomObjectList.Add(1);
            }
            if (!lastObjectTile.BlockedDown && !Physics.CheckSphere(lastObject.transform.position + Vector3.back * 3, .1f, layerMask)) {
                randomObjectList.Add(2);
            }
            if (!lastObjectTile.BlockedLeft && !Physics.CheckSphere(lastObject.transform.position + Vector3.left * 3, .1f, layerMask)) {
                randomObjectList.Add(3);
            }

            // No where to go - Go back to a previous tile
            if (randomObjectList.Count == 0) {
                _spawnedTiles.RemoveAt(_spawnedTiles.Count - 1);
                lastObject = _spawnedTiles.Last();
                --i;
                continue;
            }

            // Pick a direction to generate a tile
            int direction = randomObjectList[Random.Range(0, randomObjectList.Count - 1)];

            // Filter out tiles that are blocked in the direction we want to go
            List<GameObject> possibleTiles = new List<GameObject>();
            switch (direction) {
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
            // print("Direction: " + direction);
            GameObject chosenTile = possibleTiles[Random.Range(0, possibleTiles.Count)];

            // Spawn the chosen tile
            GameObject go = GenerateTile(position, dirVector, chosenTile);
            if (go == null) {
                print("GO was null!");
                if (checks == 4) {
                    _spawnedTiles.RemoveAt(_spawnedTiles.Count - 1);
                    if (_spawnedTiles.Count == 0) {
                        _spawnedTiles.Add(_spawningObject);
                    }
                    lastObject = _spawnedTiles.Last();
                    checks = 0;
                }
                else {
                    --i;
                    checks++;
                }
            }
            else {
                lastObject = go;
                _spawnedTiles.Add(lastObject);
                checks = 0;
            }
        }
    }

    GameObject GenerateTile(Vector3 position, Vector3 direction, GameObject chosenTile)
    {
        position.y = 0;
        Vector3 newPos = position + direction * 3;
        return Instantiate(chosenTile, newPos, Quaternion.identity);
    }
}