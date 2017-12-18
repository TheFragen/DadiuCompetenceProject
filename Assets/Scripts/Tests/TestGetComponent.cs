using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject tile;
    public void Test()
    {
        for (int i = 0; i < 100000; i++)
        {
            tile.GetComponent<Tile>();
        }
    }


}
