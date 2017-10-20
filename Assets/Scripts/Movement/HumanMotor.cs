using System;
using EasyButtons;
using Gamelogic.Extensions;
using UnityEngine;

public class HumanMotor : MonoBehaviour
{
    public delegate void PickupItem(AbstractItem item);
    public event PickupItem OnItemPickup;

    private void Start()
    {
        GameManager.Instance.CreatePlayer(gameObject);
    }

    [Button]
    private void EndTurn()
    {
        GameManager.Instance.NextTurn(gameObject);
    }

    public bool Move(Vector2 direction)
    {
        if (!GameManager.Instance.IsPlayerTurn(gameObject))
        {
            return false;
        }

        Vector3 reference = transform.position;
        reference += direction.To3DXZ(0);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;
        bool raycastResult = Physics.Raycast(transform.position,
            direction.To3DXZ(0), out hit,
            1,
            layerMask);

        if (!raycastResult || (hit.transform != null && hit.transform.CompareTag("Item")))
        {
            if (GameManager.Instance.PerformAction(gameObject))
            {
                transform.position = reference;
                return true;
            }
        }
        return false;
    }

    public Vector3 MoveToTarget(Vector3 origin, Vector3 target)
    {
        if (!GameManager.Instance.IsPlayerTurn(gameObject) || !GameManager.Instance.PerformAction(gameObject))        
        {
            return origin;
        }
        else
        {
            return Vector3.MoveTowards(origin, target, 1);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        AbstractItem _tmp = collider.gameObject.GetComponent<AbstractItem>();
        if (_tmp == null) return;

        Debug.DrawLine(transform.position, collider.transform.position, Color.yellow);

        if ((transform.position - collider.transform.position).sqrMagnitude < 1.25f)
        {
            if (OnItemPickup != null)
            {
                OnItemPickup.Invoke(_tmp);
                collider.gameObject.SetActive(false);
            }
        }
    }
}