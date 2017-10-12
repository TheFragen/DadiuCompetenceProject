using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;
#pragma warning disable 649

public class Level : MonoBehaviour
{
	[SerializeField]
	private GameObject _tilePrefab;

	private GameObject[] _playersAndAi;

	private Dictionary<GameObject, KeyValuePair<Vector3, GameObject>>
		_playersAndAiPrevFrame;

	// Use this for initialization
	void Start()
	{
		_playersAndAi = GameObject.FindGameObjectsWithTag("Player");
		_playersAndAiPrevFrame =
			new Dictionary<GameObject, KeyValuePair<Vector3, GameObject>>();
	}

	// Update is called once per frame
	void Update()
	{
		Dictionary<GameObject, KeyValuePair<Vector3, GameObject>> thisFrameGameObjects
			= new Dictionary<GameObject, KeyValuePair<Vector3, GameObject>>();
		foreach (GameObject go in _playersAndAi)
		{
			GameObject currentTile = null;
			if (_playersAndAiPrevFrame.Count > 0)
			{
				// Check if a player has left a tile without a new tile
				KeyValuePair<GameObject, KeyValuePair<Vector3, GameObject>> prevFrame =
					_playersAndAiPrevFrame.SingleOrDefault(i => i.Key.name == go.name);

				RaycastHit hit;
				if (!Physics.Raycast(go.transform.position, -go.transform.up, out hit))
				{
					if (!prevFrame.Equals(
						default(KeyValuePair<GameObject, KeyValuePair<Vector3, GameObject>>)))
					{
						Vector3 difference = prevFrame.Value.Key - go.transform.position;
						GenerateTile(go.transform.position, difference, prevFrame.Value.Value);
					}
				}
				else
				{
					currentTile = hit.transform.gameObject;
				}
			}

			thisFrameGameObjects.Add(go,
				new KeyValuePair<Vector3, GameObject>(go.transform.position, currentTile));
		}

		// Keep a Dictionry of previous frame position, and previous frame tile
		_playersAndAiPrevFrame.Clear();
		_playersAndAiPrevFrame.AddRange(thisFrameGameObjects);
	}

	void GenerateTile(Vector3 position, Vector3 direction, GameObject tileLeaving)
	{
		// Moving up or down
		if ((int)direction.x == 0 && (int)direction.z != 0)
		{
			position.x = tileLeaving.transform.position.x;
		}
		// Moving left or right
		if ((int)direction.x != 0 && (int)direction.z == 0)
		{
			position.z = tileLeaving.transform.position.z;
		}
		position.y = 0;
		Instantiate(_tilePrefab, position - direction, Quaternion.identity);
	}
}