using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
	public string name;
	public GameObject itemObject;
	public bool isArtefact;

	public virtual void Use()
	{
		Debug.Log("Used item: " + name);
	}
}
