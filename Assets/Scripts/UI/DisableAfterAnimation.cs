using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAnimation : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void DisableMe()
    {
        if (GetComponent<Animator>().enabled)
        {
            print("Subtracting");
            bool isEnemy = _animator.GetBool("IsEnemy");
            JuiceController.Instance.SetupBattle(isEnemy);
            JuiceController.Instance.CombatSubtract(isEnemy);
            gameObject.SetActive(false);
        }
        
    }
}
