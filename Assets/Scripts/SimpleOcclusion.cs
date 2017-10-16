using UnityEngine;

public class SimpleOcclusion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        foreach (var rend in other.gameObject
            .GetComponentsInChildren<Renderer>())
        {
            rend.enabled = true;
        }
    }
}