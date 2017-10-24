using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDetection : MonoBehaviour
{
    [SerializeField]
    private float maxDistance = 2;

    private bool isShowingButton;
    private GameObject activeOpponent;

    void OnTriggerStay(Collider other)
    {
        float dist = (transform.position - other.transform.position)
            .sqrMagnitude;

        if (other.gameObject == activeOpponent && dist > maxDistance &&
            isShowingButton)
        {
            JuiceController.Instance.DisableCombat();
            isShowingButton = false;
            activeOpponent = null;
        }
        else if ((other.tag == "AI" || other.tag == "Player") &&
                 dist < maxDistance &&
                 GameManager.Instance.IsPlayerTurn(gameObject))
        {
            JuiceController.Instance.EnableCombat(gameObject, other.gameObject);
            isShowingButton = true;
            activeOpponent = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "AI" || other.tag == "Player") {
            JuiceController.Instance.DisableCombat();
            isShowingButton = false;
            activeOpponent = null;
        }
    }
}
