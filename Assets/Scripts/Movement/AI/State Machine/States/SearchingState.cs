using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;

public class SearchingState : State
{
    public SearchingState(AI ai) : base(ai)
    {
    }

    public override void OnStateEnter()
    {
        _ai._ItemToLookFor = null;
        InitializePath();
    }

    public override void Tick()
    {
        if (_ai._ItemToLookFor != null)
        {
            _ai.SetState(new MovingState(_ai));
        }
    }

    void InitializePath()
    {
        _ai._ItemToLookFor = FindClosestItem();
    }

    private GameObject FindClosestItem()
    {
       /* var nearestItems = GameManager.Instance.GetSpawnedItems()
            .Where(o => o.gameObject.activeInHierarchy)
            .Where(o => !_ai._BlacklsitedNodes.Contains(o.gameObject))
            .OrderBy(o =>
                (o.transform.position - _ai.transform.position).sqrMagnitude)
            .Take(10)
            .OrderBy(o =>
                _ai._PathFinding.GeneratePath(_ai.transform.position,
                    o.transform.position).Count)
            .Take(5).ToArray();*/
        int n = 10;
        int smallN = 5;
        List<GameObject> spawnedObejcts =
            GameManager.Instance.GetSpawnedItems();
        GameObject[] closestItems = new GameObject[n];
        GameObject[] closestPaths = new GameObject[smallN];

        float[] closestDistances = new float[n];
        float[] closestDistancePaths = new float[smallN];

        for (int i = 0; i < n; i++)
        {
            closestDistances[i] = float.MaxValue;
            closestItems[i] = new GameObject();
            if (i < smallN)
            {
                closestPaths[i] = new GameObject();
                closestDistancePaths[i] = float.MaxValue;
            }
            
        }
        foreach (var elem in spawnedObejcts)
        {
            if (elem.activeInHierarchy && !_ai._BlacklsitedNodes.Contains(elem))
            {
                float d = (elem.transform.position - _ai.transform.position)
                    .sqrMagnitude;
                for(int i = 0; i < n; i++)
                {
                    if (d < closestDistances[i])
                    {
                        float max = 0;
                        int index = 0;
                        for (int o = 0; o < n; o++)
                        {
                            if (closestDistances[o] > max)
                            {
                                max = closestDistances[o];
                                index = o;
                            }
                            closestDistances[index] = d;
                            closestItems[index] = elem;
                        }
                    }
                }
            }
        }

        foreach (var elem in closestItems)
        {
            int d = _ai._PathFinding.GeneratePath(_ai.transform.position, elem.transform.position).Count;
            for (int i = 0; i < smallN; i++) {
                if (d < closestDistancePaths[i]) {
                    float max = 0;
                    int index = 0;
                    for (int o = 0; o < smallN; o++) {
                        if (closestDistancePaths[o] > max) {
                            max = closestDistancePaths[o];
                            index = o;
                        }
                        closestDistancePaths[index] = d;
                        closestPaths[index] = elem;
                    }
                }
            }
        }

      /*  GameObject chosenItem =
            nearestItems[Random.Range(0, nearestItems.Length)];*/
        GameObject chosenItem = closestItems[Random.Range(0, closestItems.Length)];

        if (chosenItem == null)
        {
            _ai.SetState(new IdleState(_ai));
            return null;
        }
        return chosenItem;
    }
}