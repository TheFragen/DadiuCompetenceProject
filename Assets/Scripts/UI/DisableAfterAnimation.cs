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
            bool isEnemy = _animator.GetBool("IsEnemy");
            FightController.Instance.SetupBattleOptions(isEnemy);
            FightController.Instance.CombatSubtract(isEnemy);
            gameObject.SetActive(false);
        }
        
    }
}
