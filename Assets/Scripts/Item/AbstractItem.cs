using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbstractItem : MonoBehaviour
{
    public string itemName;
    public bool isArtefact;
    public bool isIdentified;

    [HideInInspector]
    public GameObject owner;
    [HideInInspector]
    public Sprite sprite;

    void Start()
    {
        if (sprite == null)
        {
            sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
    }

    public virtual void Use()
    {
        if (!isArtefact)
        {
            Debug.Log("Used item: " + itemName);
            owner.GetComponent<Inventory>().RemoveItem(this);
        }
    }
}