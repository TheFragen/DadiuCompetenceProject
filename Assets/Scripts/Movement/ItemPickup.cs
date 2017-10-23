using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private HumanMotor _hm;

    // Use this for initialization
    void Start()
    {
        _hm = transform.parent.GetComponent<HumanMotor>();
    }

    private void OnTriggerStay(Collider other)
    {
        _hm.InvokeItemPickup(other);
    }
}