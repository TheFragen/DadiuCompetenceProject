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

    [SerializeField]
	private int _gridSize;
    [SerializeField]
	private Items _itemScriptableObject;

    private List<Vector3> _itemSpawnPositions;
    private Grid _Grid;

    void Start()
    {
        _Grid = GameObject.FindGameObjectWithTag("Generator").GetComponent<Grid>();

		_itemSpawnPositions = GetTilePositions();

		int numItemsToSpawn = Random.Range(0, 9);
		for (int i = 0; i < numItemsToSpawn; i++)
		{
			SpawnObjects(ref _itemSpawnPositions);
        }
	    if (gameObject.tag != "Respawn")
	    {
	        foreach (var rend in gameObject
	            .GetComponentsInChildren<Renderer>())
	        {
	         //   rend.enabled = false;
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

		for (int y = 0; y < _gridSize; y++)
		{
			for (int x = 0; x < _gridSize; x++)
			{
				Vector3 pos = transform.position;
				pos.x = pos.x - width / 2 + x * xSpacing + xSpacing/2;
				pos.z = pos.z - height / 2 + y * ySpacing + ySpacing/2;
                
			    _Grid.AddToGrid(pos);
				spawnPositions.Add(pos - new Vector2(xSpacing / 2, ySpacing / 2).To3DXZ(0));
			}
		}
		return spawnPositions;
	}

    void SpawnObjects(ref List<Vector3> posibleLocations)
	{
		List<Item> possibleItems = _itemScriptableObject._itemList;

		int randomIndex = Random.Range(0, possibleItems.Count);
		int spawnRate = _itemScriptableObject._spawnRate;

		bool shouldSpawn = Random.Range(0, 100) < spawnRate;
		if (shouldSpawn)
		{
			Vector3 position =
				posibleLocations[Random.Range(0, posibleLocations.Count)];
			posibleLocations.Remove(position);

		    AbstractItem itemToSpawn = possibleItems[randomIndex];
			GameObject item = Instantiate(itemToSpawn.gameObject, position, Quaternion.identity);
		    item.transform.parent = transform;
            GameManager.Instance.AddSpawnedItem(item);
		}
	}
}