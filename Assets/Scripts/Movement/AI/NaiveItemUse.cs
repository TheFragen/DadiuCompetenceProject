using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NaiveItemUse : MonoBehaviour
{
    [SerializeField]
    private List<AbstractItem> _Inventory;

	// Use this for initialization
	void Start ()
	{
	    _Inventory = GetComponent<Inventory>().GetInventory();
	}
	
	// Update is called once per frame
	void Update ()
    {
        List<AbstractItem> itemCopy = new List<AbstractItem>(_Inventory);
        foreach (var item in itemCopy)
        {
            if (item != null && !item.isArtefact && _Inventory.Contains(item))
            {
                item.Use();
            }
        }
	}
}
