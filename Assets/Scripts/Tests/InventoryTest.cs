using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{

    public GameObject Player;
    public GameObject Item;
    public GameObject InventoryCanvas;
    

    public void Test()
    {
        Player.transform.position = new Vector3(0,0.5f, 0);
        Instantiate(Item, new Vector3(1, 0, 0), Quaternion.identity);
        Player.transform.position = new Vector3(1, 0.5f, 0);

        StartCoroutine(Coroutine());
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(2);

        Inventory playerInventory = Player.GetComponent<Inventory>();
        bool inventoryContainsItem = playerInventory.GetInventory().Count > 0;
        bool hasMoreThanZeroItems = InventoryCanvas.transform.childCount > 0;
        
        bool testResult = inventoryContainsItem && hasMoreThanZeroItems;

        Debug.Log(GetType().Name + " " + (testResult ? "succeeded" : "failed"));
    }

}
