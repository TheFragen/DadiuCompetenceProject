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

    [SerializeField] private string _compatibleUp;
    [SerializeField] private string _compatibleRight;
    [SerializeField] private string _compatibleDown;
    [SerializeField] private string _compatibleLeft;

    public List<GameObject> _compatibleUpTypes;
    public List<GameObject> _compatibleRightTypes;
    public List<GameObject> _compatibleDownTypes;
    public List<GameObject> _compatibleLeftTypes;

    [SerializeField]
    private List<GameObject> allTypes;

    public bool BlockedUp = false;
    public bool BlockedRight = false;
    public bool BlockedDown = false;
    public bool BlockedLeft = false;

    [Button]
    void SetTypes()
    {
        _compatibleUpTypes = new List<GameObject>();
        foreach (var ups in _compatibleUp.Split(','))
        {
            _compatibleUpTypes.Add(allTypes[int.Parse(ups)]);
        }
        _compatibleRightTypes = new List<GameObject>();
        foreach (var rights in _compatibleRight.Split(',')) {
            _compatibleRightTypes.Add(allTypes[int.Parse(rights)]);
        }
        _compatibleDownTypes = new List<GameObject>();
        foreach (var downs in _compatibleDown.Split(',')) {
            _compatibleDownTypes.Add(allTypes[int.Parse(downs)]);
        }
        _compatibleLeftTypes = new List<GameObject>();
        foreach (var downs in _compatibleLeft.Split(',')) {
            _compatibleLeftTypes.Add(allTypes[int.Parse(downs)]);
        }
    }

    void Start()
	{
		List<Vector3> itemSpawnPositions = GetSpawnPositions();

		int numItemsToSpawn = Random.Range(0, 9);
		for (int i = 0; i < numItemsToSpawn; i++)
		{
			SpawnObjects(ref itemSpawnPositions);
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
			Instantiate(itemToSpawn.itemObject, position, Quaternion.identity);
		}
	}
}