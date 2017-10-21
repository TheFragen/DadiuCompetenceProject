using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NaiveItemUse : MonoBehaviour
{
    [SerializeField]
    private List<AbstractItem> _inventory;

	// Use this for initialization
	void Start ()
	{
	    _inventory = GetComponent<Inventory>().GetInventory();
	}
	
	// Update is called once per frame
	void Update ()
    {
        List<AbstractItem> itemCopy = new List<AbstractItem>(_inventory);
        foreach (var item in itemCopy)
        {
            if (item != null && !item.isArtefact && _inventory.Contains(item))
            {
                item.Use();
            }
        }
	}
}
