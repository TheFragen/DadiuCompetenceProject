using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

public class FightController : Singleton<FightController>
{
    public delegate void CombatStarted();
    public static event CombatStarted OnCombatStarted;
    public delegate void CombatEnded();
    public static event CombatEnded OnCombatEnded;

    [Header("Combat")]
    [SerializeField]
    private Button _startCombatButton;
    [SerializeField]
    private GameObject _fightPanel, _fightOptionsObject, _fightTextObject, _winPanel;
    [SerializeField]
    private Animator _fightCanvasAnimator, _diceAnimator;
    [SerializeField]
    private List<string> _fightText;
    [SerializeField]
    private Slider _playerHealthObject, _opponentHealthObject;
    [SerializeField]
    private Vector3 _spawnPostion;

    private int enemyLives;
    private int playerLives;
    private GameObject combatPlayer;
    private GameObject combatOpponent;
    private bool InCombat;

    void Start()
    {
        _fightCanvasAnimator = _fightPanel.GetComponent<Animator>();
    }

    // Show the "Start Combat" button
    public void CombatInit(GameObject player, GameObject opponent)
    {
        _startCombatButton.onClick.RemoveAllListeners();
        _startCombatButton.gameObject.SetActive(true);
        _startCombatButton.onClick.AddListener(delegate
        {
            CombatStart(player, opponent);
        });
    }

    // Hide the "Start Combat" button
    public void CombatTerminate()
    {
        _startCombatButton.onClick.RemoveAllListeners();
        _startCombatButton.gameObject.SetActive(false);
        EndCombat(true);
    }

    // Actually start the combat
    public void CombatStart(GameObject player, GameObject opponent)
    {
        print(player.name + " wants to fight " + opponent.name);
        combatPlayer = player;
        combatOpponent = opponent;
        InCombat = true;

        _winPanel.SetActive(false);
        _fightCanvasAnimator.enabled = true;
        _fightPanel.SetActive(true);
        _startCombatButton.gameObject.SetActive(false);
        _fightTextObject.GetComponent<ScrollingText>().SetText(_fightText,
            delegate
            {
                SetupBattleOptions(true);
            });

        _diceAnimator.gameObject.SetActive(false);

        _diceAnimator.enabled = true;
        _diceAnimator.gameObject.GetComponent<Image>().enabled = true;

        enemyLives = 10;
        playerLives = 10;
        StartCoroutine(CombatUpdate());
        if (OnCombatStarted != null)
        {
            OnCombatStarted.Invoke();
        }
    }

    // Shows or hides the Battle options
    // Is both called after an enemy has attacked and after a player has attacked
    // If enemy has attacked optionsState is true
    // If player has attacked optionsState is false
    // This bool is gotten from an animator property
    public void SetupBattleOptions(bool optionsState)
    {
        _fightOptionsObject.SetActive(optionsState);
        _diceAnimator.SetBool("IsEnemy", false);
    }

    // Runs when it is the opponents turn
    IEnumerator EnemyCombatTurn() {
        yield return new WaitForSeconds(2);
        ThrowDice(1);
    }

    // Internal update routine for when in combat
    IEnumerator CombatUpdate() {
        while (true) {
            _playerHealthObject.value = playerLives;
            _opponentHealthObject.value = enemyLives;

            if (enemyLives <= 0) {
                EndCombatWithText();
                break;
            }
            if (playerLives <= 0) {
                EndCombatWithText();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void CombatSubtract(bool isEnemy) {
        //isEnemy is the caller
        if (isEnemy) {
            playerLives -= Math.Max(UnityEngine.Random.Range(1, 7), 0); // TODO: FIx this max usage
        }
        else {
            enemyLives -= Math.Max(UnityEngine.Random.Range(1, 7), 0);
        }

        if (enemyLives < 4 && enemyLives > 0) {
            _fightCanvasAnimator.SetTrigger("EnemyBlink");
        }
        if (playerLives < 4 && playerLives > 0) {
            _fightCanvasAnimator.SetTrigger("PlayerBlink");
        }
    }

    // The function that attacks the other player or opponent
    // Will start an animation, when in turn calls "CombatSubstract" once it has played out
    // PlayerNum: 0 is the player
    // PlayerNum: 1 is the opponent
    public void ThrowDice(int playerNum)
    {
        _fightOptionsObject.SetActive(false);
        _diceAnimator.gameObject.SetActive(true);
        if (playerNum == 0)
        {
            _diceAnimator.SetTrigger("PlayerDice");
            _fightCanvasAnimator.SetTrigger("EnemyDamage");
            StartCoroutine(EnemyCombatTurn());
            _fightOptionsObject.SetActive(false);
        }
        else {
            _diceAnimator.SetTrigger("EnemyDice");
            _diceAnimator.SetBool("IsEnemy", true);
            _fightCanvasAnimator.SetTrigger("PlayerDamage");

        }
    }

    // Animates some text telling the player whether he won or not
    private void EndCombatWithText()
    {
        StopCoroutine(CombatUpdate());
        _diceAnimator.enabled = false;
        _diceAnimator.StopPlayback();
        _diceAnimator.gameObject.SetActive(false);
        _diceAnimator.gameObject.GetComponent<Image>().enabled = false;
        _fightCanvasAnimator.enabled = false;
        _fightOptionsObject.SetActive(false);

        // Play sound

        List<string> endText = new List<string>();
        if (playerLives <= 0)
        {
            endText.Add("The opponent has won the fight.");
            endText.Add(
                "You have been returned to Tristam, and lost all your items.");
        }
        else if (enemyLives <= 0)
        {
            endText.Add("Congratulations, you've won the fight.");
            endText.Add(
                "Your opponent has been stripped of his items and cast back to Tristam.");
        }
        _fightTextObject.GetComponent<ScrollingText>().SetText(endText,
            delegate { EndCombat(false); });
    }

    public void EndCombat(bool isRunning)
    {
        // Simply just reset the combat state
        if (combatPlayer == null && !InCombat)
        {
            _fightPanel.SetActive(false);
            return;
        }

        StopCoroutine(CombatUpdate());
        _winPanel.SetActive(true);
        _diceAnimator.gameObject.SetActive(false);

        if (!isRunning)
        {
            if (playerLives <= 0)
            {
                GameManager.Instance.RemoveAllItems(combatPlayer);
                combatPlayer.transform.position = _spawnPostion;
            }
            if (enemyLives <= 0)
            {
                GameManager.Instance.RemoveAllItems(combatOpponent);
                combatOpponent.transform.position = _spawnPostion;
                AI ai = combatOpponent.GetComponent<AI>();
                ai.SetState(new SearchingState(ai));
            }
        }

        GameManager.Instance.NextTurn(combatPlayer);
        combatPlayer = null;
        combatOpponent = null;

        if (OnCombatEnded != null)
        {
            OnCombatEnded.Invoke();
        }
        InCombat = false;
    }

    // Used for testing purposes
    public int GetEnemyHealth()
    {
        return enemyLives;
    }
}
