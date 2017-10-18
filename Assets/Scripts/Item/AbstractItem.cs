using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbstractItem : MonoBehaviour
{
    public string name;
    public bool isArtefact;

    [HideInInspector]
    public GameObject owner;

    public virtual void Use()
    {
        Debug.Log("Used item: " + name);
        owner.GetComponent<Inventory>().RemoveItem(this);
    }
}