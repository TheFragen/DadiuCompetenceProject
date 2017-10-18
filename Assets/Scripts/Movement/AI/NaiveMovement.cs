using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
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

    [SerializeField]
    private int _pathIndex = 0;
    [SerializeField]
    private GameObject _itemToLookFor;

    [SerializeField]
    private float _waitTime;

    private Vector3 _targetPos = Vector3.negativeInfinity;

    private Vector3 nextNode;
    private Vector3 _lastFrameDirection;

    private List<Vector3> blacklistedNodes;
    private bool _isMoving;

    // Use this for initialization
    void Start ()
    {
        _motor = GetComponent<HumanMotor>();
        blacklistedNodes = new List<Vector3>();
    }

    void Update()
    {
        // Check if the AI has a path
            // If it has a path, do movement
            // If not find a path

        if (_path == null || _path.Count == 0)
        {
            InitializePath();
        }
        if (_path != null &&  _path.Count > 0 && !_isMoving)
        {
            StartCoroutine(PerformMovement(_waitTime));
        }
    }

    void InitializePath()
    {
        FindClosestItem();
        _pathIndex = 0;
        if (_itemToLookFor != null)
        {
            print("Initalizing new target");
            Vector3 target = _itemToLookFor.transform.position;
            target.y = -1.34f;
            _path = _PathFinding.GeneratePath(transform.position, target);
            if (_path != null && _path.Count > 0)
            {
                print("We have the technology");
                _targetPos = _path[_path.Count - 1]._WorldPos;
                _PathFinding.AddToActivePaths(gameObject, _path);
            } else
            {
                print("We don't have the technology");
                blacklistedNodes.Add(_itemToLookFor.transform.position);
                _itemToLookFor = null;
            }
        }
    }

    IEnumerator PerformMovement(float delay)
    {
        _isMoving = true;
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        Vector3 targetPos = _path[_pathIndex]._WorldPos;
        nextNode = targetPos;

        bool didMove = false;
        targetPos.y = transform.position.y;
        Vector3 tmp = transform.position;
        transform.position = _motor.MoveToTarget(transform.position, targetPos);
        didMove = tmp != transform.position;

        if (didMove)
        {
            //  print("Moved");
            if (_pathIndex < _path.Count - 1)
            {
                _pathIndex++;
            }
            else
            {
                //    print("At end pos");
                _path = null;
                _PathFinding.RemoveFromActivePaths(gameObject);
            }
        }
        else
        {
            GameManager.Instance.NextTurn(gameObject);
        }
        
        yield return null;
        _isMoving = false;
    }

    [Button]
    void UseHaste()
    {
        GameManager.Instance.IncreaseAction(gameObject, 2);
    }

    void FindClosestItem()
    {
        float minDistance = float.MaxValue;
        foreach (var item in GameManager.Instance.GetSpawnedItems())
        {
            if (!item.gameObject.activeInHierarchy) continue;
            if (blacklistedNodes.Contains(item.gameObject.transform.position)) continue;

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
