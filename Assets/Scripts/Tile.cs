using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

#pragma warning disable 649
public class Tile : MonoBehaviour
{
    public bool BlockedUp = false;
    public bool BlockedRight = false;
    public bool BlockedDown = false;
    public bool BlockedLeft = false;
    public GameObject prefab;

    [SerializeField]
    private int _gridSize;
    [SerializeField]
    private Items _itemScriptableObject;
    [SerializeField]
    private bool _disableItemSpawn, _startHidden;

    private List<Vector3> _itemSpawnPositions;
    private Grid _Grid;

    void Start()
    {
        _Grid = GameObject.FindGameObjectWithTag("Generator")
            .GetComponent<Grid>();

        _itemSpawnPositions = GetTilePositions();

        int numItemsToSpawn = Random.Range(0, 9);
        if (!_disableItemSpawn)
        {
            for (int i = 0; i < numItemsToSpawn; i++)
            {
                SpawnObjects(ref _itemSpawnPositions);
            }
        }
        if (gameObject.tag != "Respawn" && _startHidden)
        {
            foreach (var rend in gameObject
                .GetComponentsInChildren<Renderer>())
            {
                rend.enabled = false;
            }
        }
    }

    List<Vector3> GetTilePositions()
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        Renderer rend = GetComponent<Renderer>();
        float width = rend.bounds.size.x;
        float height = rend.bounds.size.z;

        float xSpacing = width / _gridSize;
        float ySpacing = height / _gridSize;
        
        for (int y = 0, tilePos = 0; y < _gridSize; y++)
        {
            for (int x = 0; x < _gridSize; x++, tilePos++)
            {
                Vector3 pos = transform.position;
                pos.x = pos.x - width / 2 + x * xSpacing + xSpacing / 2;
                pos.z = pos.z - height / 2 + y * ySpacing + ySpacing / 2;

                _Grid.AddToGrid(pos, this, tilePos);
                spawnPositions.Add(pos);
            }
        }
        return spawnPositions;
    }

    void SpawnObjects(ref List<Vector3> possibleLocations)
    {
        List<AbstractItem> possibleItems = GameManager.Instance.GetItemList();

        int randomIndex = Random.Range(0, possibleItems.Count);
        int spawnRate = _itemScriptableObject._spawnRate;

        bool shouldSpawn = Random.Range(0, 100) < spawnRate;
        if (shouldSpawn)
        {
            Vector3 position =
                possibleLocations[Random.Range(0, possibleLocations.Count)];
            possibleLocations.Remove(position);
            position.y = 0;

            AbstractItem itemToSpawn = possibleItems[randomIndex];
            GameObject item = Instantiate(itemToSpawn.gameObject, position,
                Quaternion.identity);
            itemToSpawn.sprite = itemToSpawn.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            item.transform.parent = transform;
            GameManager.Instance.AddSpawnedItem(item, itemToSpawn);
        }
    }
}