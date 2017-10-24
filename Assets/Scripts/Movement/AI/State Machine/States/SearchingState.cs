using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // Todo: Search first by euclidian distance, then filter by Path distance
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
        if (spawnedItems.Count == 0)
        {
            _ai.SetState(new IdleState(_ai));
            return null;
        }
        return spawnedItems[Random.Range(0, spawnedItems.Count)];
    }
}