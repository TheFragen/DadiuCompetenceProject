using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningState : State
{
    private List<Node> _path;
    private bool _isMoving;
    private int _pathIndex;

    public ReturningState(AI ai) : base(ai)
    {
    }

    public override void OnStateEnter()
    {
        _path = _ai._PathFinding.GeneratePath(_ai.transform.position, new Vector3(-1,0,1));
    }

    public override void Tick()
    {
        if (!_ai._Inventory.ArtefactFound)
        {
            _ai.SetState(new SearchingState(_ai));
        }
        if (!_isMoving && GameManager.Instance.IsPlayerTurn(_ai.gameObject)) {
            _ai.StartCoroutine(PerformMovement(_ai._WaitTime));
        }
    }

    IEnumerator PerformMovement(float delay) {
        _isMoving = true;
       
        Vector3 targetPos = _path[_pathIndex]._WorldPos;

        targetPos.y = _ai.transform.position.y;
        Vector3 tmp = _ai.transform.position;
        _ai.transform.position =
            _ai._HumanMotor.MoveToTarget(_ai.transform.position, targetPos);
        bool didMove = tmp != _ai.transform.position;

        if (didMove)
        {
            if (_pathIndex < _path.Count - 1)
            {
                _pathIndex++;
            }
            else
            {
                _ai.SetState(new IdleState(_ai));
            }
        }
        else
        {
            GameManager.Instance.NextTurn(_ai.gameObject);
        }

        yield return null;
        _isMoving = false;
    }
}