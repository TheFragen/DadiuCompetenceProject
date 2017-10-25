using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;
using UnityEngine.Events;

public class TestMain : MonoBehaviour
{
    [SerializeField]
    private List<UnityEvent> Tests;
    

    [Button]
    void StartTests()
    {
        foreach (var test in Tests)
        {
            test.Invoke();
        }
    }
}
