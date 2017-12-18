using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : ScriptableObject
{
	public List<AbstractItem> _itemList;

	[Tooltip("Defines how often an item should be spawned." +
	         " 0 is items will never spawn, 100 is items will always spawn"),
	 Range(0, 100)]
	public int _spawnChance;

    public float _nodeDiameter;
}