using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TestFindNearestItem : MonoBehaviour
{
    public AI _ai;
    

    public void Test()
    {
        var spawnedItems = GameManager.Instance.GetSpawnedItems()
            .Where(o => o.gameObject.activeInHierarchy)
            .Where(o => !_ai._BlacklsitedNodes.Contains(o.gameObject))
            .OrderBy(o =>
                (o.transform.position - _ai.transform.position).sqrMagnitude)
            .Take(10)
            .OrderBy(o =>
                _ai._PathFinding.GeneratePath(_ai.transform.position,
                    o.transform.position).Count)
            .Take(5).ToList();

        bool testResult = spawnedItems.Count == 5;
        Debug.Log(GetType().Name + " " + (testResult ? "succeeded" : "failed"));
    }
}
