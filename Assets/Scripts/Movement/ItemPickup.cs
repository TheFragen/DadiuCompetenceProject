using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private HumanMotor _hm;

    void Start()
    {
        _hm = transform.parent.GetComponent<HumanMotor>();
    }

    private void OnTriggerStay(Collider other)
    {
        _hm.InvokeItemPickup(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Wall")
        {
            print("Went through a wall");
            Debug.Break();
        }
    }


}