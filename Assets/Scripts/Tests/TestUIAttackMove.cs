using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIAttackMove : MonoBehaviour
{
    public GameObject player;
    public GameObject opponent;

    public Button FightStartButton;
    public Button FightText;
    public Button FightOptionFight;
    public Button FightOptionAttack;


    public void Test()
    {
        StartCoroutine(Coroutine());
    }

    IEnumerator Coroutine()
    {
        // We need to wait because there are some tests that run in coroutines
        // TODO: Make each test return when done, before the next is invoked
        yield return new WaitForSeconds(5);
        player.transform.position = Vector3.zero;
        opponent.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1);
        FightStartButton.onClick.Invoke();


        // Play a fight
        int enemyHP = FightController.Instance.GetEnemyHealth();
        yield return new WaitForSeconds(5);
        FightText.onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        FightText.onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        FightText.onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        FightText.onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        FightText.onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        FightOptionFight.onClick.Invoke();
        yield return new WaitForSeconds(.5f);
        FightOptionAttack.onClick.Invoke();
        yield return new WaitForSeconds(2);
        int nextEnemyHP = FightController.Instance.GetEnemyHealth();

        Debug.Assert(nextEnemyHP < enemyHP);

        Debug.Log(GetType().Name + " finished");
    }
}
