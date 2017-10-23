using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : State
{
    private List<Node> _path;
    private bool _isMoving;
    private int _pathIndex;

    public MovingState(AI ai) : base(ai)
    {
    }

    public override void OnStateEnter()
    {
        _pathIndex = 0;
        GameObject target = _ai._ItemToLookFor;
        _path = _ai._PathFinding.GeneratePath(_ai.transform.position, target.transform.position);
      //  Debug.Assert(_path != null && _path.Count > 0);

        if (_path != null && _path.Count > 0)
        {
            _ai._PathFinding.AddToActivePaths(_ai.gameObject, _path);
        }
        else
        {
            _ai.SetState(new SearchingState(_ai));
            _ai._BlacklsitedNodes.Add(_ai._ItemToLookFor);

        }
    }

    public override void OnStateExit()
    {
        _ai._PathFinding.RemoveFromActivePaths(_ai.gameObject);
    }

    public override void Tick()
    {
        if (!_isMoving && GameManager.Instance.IsPlayerTurn(_ai.gameObject))
        {
            _ai.StartCoroutine(PerformMovement(_ai._WaitTime));
        }
    }

    IEnumerator PerformMovement(float delay)
    {
        _isMoving = true;
        // The item has been picked up - find another one #DJ Khaled
        if (!_ai._ItemToLookFor.activeInHierarchy)
        {
            _ai.SetState(new SearchingState(_ai));
            yield break;
        }
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
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
                if (_ai._Inventory.ArtefactFound)
                {
                    _ai.SetState(new ReturningState(_ai));
                }
                else
                {
                    _ai.SetState(new SearchingState(_ai));
                }
                
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