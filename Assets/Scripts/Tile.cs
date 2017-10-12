using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using Gamelogic.Extensions;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField]
	private int _gridSize;
	[SerializeField]
	private Vector2 _offset;

	[SerializeField]
	private Items _itemScriptableObject;

    public bool BlockedUp = false;
    public bool BlockedRight = false;
    public bool BlockedDown = false;
    public bool BlockedLeft = false;

    void Start()
	{
		List<Vector3> itemSpawnPositions = GetSpawnPositions();

		int numItemsToSpawn = Random.Range(0, 9);
		for (int i = 0; i < numItemsToSpawn; i++)
		{
			SpawnObjects(ref itemSpawnPositions);
        }
	    if (gameObject.tag != "Respawn")
	    {
	        foreach (var rend in gameObject
	            .GetComponentsInChildren<Renderer>())
	        {
	            rend.enabled = false;
	        }
	    }	    
    }

	List<Vector3> GetSpawnPositions()
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
				pos.x = pos.x - width / 2 + x * xSpacing;
				pos.z = pos.z - height / 2 + y * ySpacing;
				spawnPositions.Add(pos);
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

			Item itemToSpawn = possibleItems[randomIndex];
			GameObject item = Instantiate(itemToSpawn.itemObject, position, Quaternion.identity);
		    item.transform.parent = transform;
		}
	}
}