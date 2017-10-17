using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UnityEngine;

[RequireComponent(typeof(HumanMotor))]
public class NaiveMovement : MonoBehaviour
{
    [SerializeField]
    private PathFinding _PathFinding;

    private HumanMotor _motor;
    [SerializeField]
    public List<Node> _path;
    private int _pathIndex = 0;
    [SerializeField]
    private GameObject _itemToLookFor;
    private Vector3 _targetPos = Vector3.negativeInfinity;

    private Vector3 nextNode;
    private Vector3 _lastFrameDirection;

    // Use this for initialization
    void Start ()
    {
        _motor = GetComponent<HumanMotor>();
    }

    void Update()
    {
        if (_path == null || _path.Count == 0)
        {
            InitializePath();
            if (_path != null && _path.Count > 1)
            {
                _targetPos = _path[_path.Count - 1]._WorldPos;
                PerformMovement();
            }
        }
        else if (transform.position == _targetPos)
        {
            print("Finished");
            InitializePath();
            if (_path.Count > 1)
            {
                _targetPos = _path[_path.Count - 1]._WorldPos;
                PerformMovement();
            }
        } else if (_path.Count > 1)
        {
            if (_path.Count > 1 && _pathIndex < _path.Count)
            {
                PerformMovement();
            }
        }
    }

    void InitializePath()
    {
        FindClosestItem();
        if (_itemToLookFor != null)
        {
            _path = _PathFinding.GeneratePath(transform, _itemToLookFor.transform);
        }
    }

    void PerformMovement()
    {
        Vector3 targetPos = _path[_pathIndex]._WorldPos;
        Vector3 direction = transform.position - targetPos;

        Vector2 normalizedDirection = direction.normalized.To2DXY();

        Vector2 relativePoint = transform.InverseTransformPoint(targetPos).To2DXZ().normalized;
        Vector2 directionVec = Vector2.zero;
        nextNode = targetPos;

        print(relativePoint);
        bool didMove = false;
        /* if (relativePoint.y > 0 && Mathf.RoundToInt(relativePoint.x) == 0) {
             print("Trying to move up");
             didMove = _motor.Move(Vector2.up);
             directionVec = Vector2.up;
         }
         else if (relativePoint.y < 0 && Mathf.RoundToInt(relativePoint.x) == 0) {
             print("Trying to move down");
             didMove = _motor.Move(Vector2.down);
             directionVec = Vector2.down;
         }
         else if (relativePoint.x > 0 && Mathf.RoundToInt(relativePoint.y) == 0) {
             print("Trying to move right");
             didMove = _motor.Move(Vector2.right);
             directionVec = Vector2.right;
         }
         else if (relativePoint.x < 0 && Mathf.RoundToInt(relativePoint.y) == 0)
         {
             print("Trying to move left");
             didMove = _motor.Move(Vector2.left);
             directionVec = Vector2.left;
         }
         else
         {
             // Use movement from last frame
             print("Using old direction");
             didMove = _motor.Move(_lastFrameDirection);
         }*/
        targetPos.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 1);
        didMove = true;

        if (didMove)
        {
            
            _lastFrameDirection = directionVec;
            if (transform.position == targetPos)
            {
                print("Moved");
                if (_pathIndex < _path.Count - 1)
                {
                    _pathIndex++;
                }
               
            }
        }
    }

    void FindClosestItem()
    {
        float minDistance = float.MaxValue;
        foreach (var item in GameManager.Instance.GetSpawnedItems())
        {
            if (!item.gameObject.activeSelf) continue;
            Vector3 direction = transform.position - item.transform.position;
            if (direction.sqrMagnitude < minDistance)
            {
                _itemToLookFor = item.gameObject;
                minDistance = direction.sqrMagnitude;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(nextNode, Vector3.one);
    }


}
