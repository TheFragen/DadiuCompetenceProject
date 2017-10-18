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
        if (_inventory != null && _inventory.Count > 0)
        {
            if (_inventory[0] != null)
            {
                _inventory[0].Use();
            }
            
        }	
	}
}
