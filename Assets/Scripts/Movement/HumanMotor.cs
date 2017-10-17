using System;
using EasyButtons;
using Gamelogic.Extensions;
using UnityEngine;

public class HumanMotor : MonoBehaviour
{
    public delegate void PickupItem(Item item);
    public event PickupItem OnItemPickup;

    private void Start()
    {
        GameManager.Instance.CreatePlayer(gameObject);
    }

    [Button]
    private void EndTurn()
    {
        GameManager.Instance.NextTurn();
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
            transform.position = reference;
            return true;
        }
        return false;
    }

    private void OnTriggerStay(Collider collider)
    {
        Item _tmp = collider.gameObject.GetComponent("Item") as Item;
        if (_tmp == null) return;
        if ((transform.position - collider.transform.position).sqrMagnitude <
            .75)
        {
            if (OnItemPickup != null)
            {
                OnItemPickup.Invoke(_tmp);
                collider.gameObject.SetActive(false);
            }
        }
    }
}