using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level_Threading
{
    [SerializeField]
    private Dictionary<Vector3,Tile> _tileDictionary;

    public Dictionary<Vector3, Tile> GenerateMazeInThread(Tile spawningTile, Vector3 spawningPosition, int tilesToSpawn, List<Tile> _typesOfTiles) {
        _tileDictionary = new Dictionary<Vector3, Tile>();
        Tile lastObject = spawningTile;
        Vector3 lastPosition = spawningPosition;
        _tileDictionary.Add(spawningPosition, spawningTile);

        System.Random rand = new System.Random(10);

        int checks = 0;
        for (int i = 0; i < tilesToSpawn; i++) {
            Vector3 position = _tileDictionary.Last().Key;
            lastObject = _tileDictionary.Last().Value;
            Vector3 dirVector = Vector3.zero;

            // Create a list of possible directions to go based on which sides of the current tile is blocked
            List<int> randomObjectList = GetAllowedDirectionsThreading(position, lastObject);

            // No where to go - Go back to a previous tile
            if (randomObjectList.Count == 0) {
                KeyValuePair<Vector3, Tile> tmp = _tileDictionary.Last();
                _tileDictionary.Remove(tmp.Key);

                // We've generated a completely closed maze, so generate a new one
                if (_tileDictionary.Count == 0) {
                    _tileDictionary.Add(tmp.Key, _typesOfTiles[0]);
                }

                if (i <= 0) {
                    // Completely closed maze - now lets just return the maze then
                   // print("Returning maze after checking for a new direction " + checks + " times.");
                    break;
                }

                lastObject = _tileDictionary.Last().Value;
                lastPosition = _tileDictionary.Last().Key;
                i -= 2;
                continue;
            }

            // Pick a direction to generate a tile
            
            int direction =
                randomObjectList[rand.Next(randomObjectList.Count)];

            // Filter out tiles that are blocked in the direction we want to go
            List<Tile> possibleTiles = new List<Tile>();
            switch (direction) {
                case 0:
                    // Up
                    dirVector = new Vector3(0, 0, 1);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.BlockedDown)
                        .ToList();
                    break;
                case 1:
                    // Right
                    dirVector = new Vector3(1, 0, 0);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.BlockedLeft)
                        .ToList();
                    break;
                case 2:
                    // Down
                    dirVector = new Vector3(0, 0, -1);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.BlockedUp)
                        .ToList();
                    break;
                case 3:
                    // Left
                    dirVector = new Vector3(-1, 0, 0);
                    possibleTiles = _typesOfTiles
                        .Where(o => !o.BlockedRight)
                        .ToList();
                    break;
            }

            Vector3 nextTilePos = position + dirVector * 3;

            Tile north;
            _tileDictionary.TryGetValue(nextTilePos + Vector3.forward * 3, out north);
            Tile east;
            _tileDictionary.TryGetValue(nextTilePos + Vector3.right * 3, out east);
            Tile south;
            _tileDictionary.TryGetValue(nextTilePos + Vector3.back * 3, out south);
            Tile west;
            _tileDictionary.TryGetValue(nextTilePos + Vector3.left * 3, out west);

            // Up
            if (north != null &&
                north.BlockedDown) {
                possibleTiles.RemoveAll(o => !o.BlockedUp);
            }
            // Right
            if (east != null &&
                east.BlockedLeft) {
                possibleTiles.RemoveAll(o => !o.BlockedRight);
            }
            // Down
            if (south != null &&
                south.BlockedUp) {
                possibleTiles.RemoveAll(o => !o.BlockedDown);
            }
            // Left
            if (west != null &&
                west.BlockedRight) {
                possibleTiles.RemoveAll(o => !o.BlockedLeft);
            }

            // No where to go - Go back to a previous tile
            if (possibleTiles.Count == 0) {
                if (i <= 0) {
                    // Completely closed maze - now lets just return the maze then
                //    print("Returning maze after checking for a new direction " + checks + " times.");
                    break;
                }
                KeyValuePair<Vector3, Tile> tmp = _tileDictionary.Last();
                _tileDictionary.Remove(tmp.Key);
                lastObject = _tileDictionary.Last().Value;
                lastPosition = _tileDictionary.Last().Key;
                i -= 2;
                checks++;
                continue;
            }
            
            Tile chosenTile =
                possibleTiles[rand.Next(possibleTiles.Count)];

            // Spawn the chosen tile
            //lastObject = GenerateTile(position, dirVector, chosenTile);
            Vector3 newPos = position + dirVector * 3;
            _tileDictionary.Add(newPos, chosenTile);
            lastObject = _tileDictionary.Last().Value;
            lastPosition = newPos;
        }

        return _tileDictionary;
        // print("Time of execution: " + (Time.realtimeSinceStartup - startTime));
        // GameManager.instance._LevelIsGenerated = true;
    }

    List<int> GetAllowedDirectionsThreading(Vector3 inputTile, Tile tile) {
        List<int> randomObjectList = new List<int>();
        
        Tile north;
        _tileDictionary.TryGetValue(inputTile + Vector3.forward * 3, out north);
        Tile east;
        _tileDictionary.TryGetValue(inputTile + Vector3.right * 3, out east);
        Tile south;
        _tileDictionary.TryGetValue(inputTile + Vector3.back * 3, out south);
        Tile west;
        _tileDictionary.TryGetValue(inputTile + Vector3.left * 3, out west);


        if (!tile.BlockedUp && north == null) {
            randomObjectList.Add(0);
        }
        if (!tile.BlockedRight && east == null) {
            randomObjectList.Add(1);
        }
        if (!tile.BlockedDown && south == null) {
            randomObjectList.Add(2);
        }
        if (!tile.BlockedLeft && west == null) {
            randomObjectList.Add(3);
        }
        return randomObjectList;
    }
}
