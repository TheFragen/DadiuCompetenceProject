using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class JuiceController : Singleton<JuiceController>
{
    [SerializeField]
    private Font DefaultFont;
    [SerializeField]
    private GameObject ChoosePlayerPanel;
    [SerializeField]
    private Button NextTurnButton;
    [SerializeField]
    private Text UseItemCanvas;
    [SerializeField]
    private AnimationCurve UseItemCurve;

    [Header("Artifact")]
    [SerializeField]
    private Text NeededArtefactCanvas;
    [SerializeField]
    private Text ArtefactAnnounceCanvas;
    [SerializeField]
    private Text ActionsLeftText;

    #region CombatVars

    [Header("Combat")]
    [SerializeField]
    private Button StartCombatButton;
    [SerializeField]
    private GameObject FightCanvas;
    [SerializeField]
    private Animator FightCanvasAnimator;
    [SerializeField]
    private GameObject FightTextObject;
    [SerializeField]
    private List<string> FightText;
    [SerializeField]
    private List<GameObject> NonFightObjects;
    [SerializeField]
    private GameObject FightOptionsObject;
    [SerializeField]
    private Animator DiceAnimator;
    [SerializeField]
    private Text PlayerHealthObject, OpponentHealthObject;
    [SerializeField]
    private GameObject _WinPanel;
    [SerializeField]
    private Vector3 _SpawnPostion;

    private int enemyLives;
    private int playerLives;
    private int activePlayer;
    private GameObject combatPlayer;
    private GameObject combatOpponent;
    private bool InCombat;

    #endregion

    void Start()
    {
        FightCanvasAnimator = FightCanvas.GetComponent<Animator>();
    }

    public void SetEndTurnButton(GameObject player)
    {
        NextTurnButton.onClick.RemoveAllListeners();

        if (player != null && player.tag == "Player")
        {
            NextTurnButton.interactable = true;
            NextTurnButton.onClick.AddListener(delegate
            {
                GameManager.instance.NextTurn(player);
            });
        }
        else
        {
            NextTurnButton.interactable = false;
        }
    }

    public void SetItemUsedText(string text)
    {
        UseItemCanvas.gameObject.SetActive(true);
        UseItemCanvas.text = text;
        StartCoroutine(TweenAlpha(UseItemCanvas, 0, 1, 1, false));
    }

    public void SetNeededArtefact(AbstractItem item)
    {
        NeededArtefactCanvas.text = "You need to find\n" +item.itemName;
        StartCoroutine(TweenAlpha(NeededArtefactCanvas, 0, 1, 0, true));
    }

    public void AnnounceArtefact(AbstractItem item, string text) {
        ArtefactAnnounceCanvas.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(text))
        {
            ArtefactAnnounceCanvas.text = item.owner.name + " has found his artefact.\nCatch him before he returns to Tristam";
        }
        else
        {
            ArtefactAnnounceCanvas.text = text;
        }
        item.isIdentified = true;
        
        StartCoroutine(TweenAlpha(ArtefactAnnounceCanvas, 0, 1, 1, false));
    }

    IEnumerator TweenAlpha(Text affect, float fromAlpha, float toAlpha, float holdTime, bool keepActive)
    {
        float t = 0f;
        Color orignalColor = affect.color;
        orignalColor.a = fromAlpha;

        Color toColor = affect.color;
        toColor.a = toAlpha;

        while (t < UseItemCurve.keys[UseItemCurve.length - 1].time) {
            t += Time.deltaTime;

            float eval = UseItemCurve.Evaluate(t);
            affect.color = Color.Lerp(orignalColor, toColor, eval);

            yield return null;
        }

        if (holdTime > 0)
        {
            yield return new WaitForSeconds(holdTime);
            StartCoroutine(TweenAlpha(affect, 1, 0, 0, keepActive));
            yield break;
        }
        if(!keepActive)
        { 
            affect.gameObject.SetActive(false);
        }
    }

    public void SelectPlayer(GameObject player)
    {
        ChoosePlayerPanel.transform.parent.gameObject.SetActive(true);

        foreach (Transform child in ChoosePlayerPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<GameObject> otherPlayers = GameObject
            .FindGameObjectsWithTag("Player").Where(o => o != player).Concat(
                GameObject.FindGameObjectsWithTag("AI")
                    .Where(o => o != player)).ToList();
        foreach (var op in otherPlayers)
        {
            GameObject tmp = new GameObject();
            tmp.name = "ChoosePlayer_" + op.name;
            GameObject img = new GameObject();
            img.AddComponent<Image>().color =
                op.GetComponent<Renderer>().material.color;
            GameObject txt = new GameObject();

            Inventory opInventory = op.GetComponent<Inventory>();
            bool hasAnyArtifact = opInventory.HasAnyArtifact();
            Text t = txt.AddComponent<Text>();
            t.text = hasAnyArtifact
                ? op.name
                : op.name + "\n(Has no artifact)";
            t.font = DefaultFont;
            t.alignment = TextAnchor.UpperCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;

            tmp.AddComponent<VerticalLayoutGroup>();
            img.transform.SetParent(tmp.transform);
            txt.transform.SetParent(tmp.transform);

            Button btn = tmp.AddComponent<Button>();
            btn.targetGraphic = img.GetComponent<Image>();
            if (hasAnyArtifact)
            {
                btn.onClick.AddListener(delegate
                {
                    RevealArtefact(op);
                });
            }
            tmp.transform.SetParent(ChoosePlayerPanel.transform);
        }
    }

    public void RevealArtefact(GameObject player)
    {
        ChoosePlayerPanel.transform.parent.gameObject.SetActive(false);
        foreach (Transform child in ChoosePlayerPanel.transform)
        {
            Destroy(child.gameObject);
        }

        Inventory inventoryComponent = player.GetComponent<Inventory>();
        List<AbstractItem> selectedInventory =
            inventoryComponent.GetInventory();

        if (selectedInventory.Where(i => i.isArtefact).ToList().Count > 1)
        {
            ChoosePlayerPanel.transform.parent.gameObject.SetActive(true);
            // Setup select multiple items
            foreach (var item in selectedInventory.Where(i => i.isArtefact).ToList()) {
                GameObject tmp = new GameObject();
                tmp.name = "ChooseArtifact_" + item.itemName;
                GameObject img = new GameObject();
                img.AddComponent<Image>().sprite = item.isIdentified ? item.sprite : new Sprite();
                GameObject txt = new GameObject();

                Text t = txt.AddComponent<Text>();
                t.text = item.isIdentified ? item.itemName : "Unidentified Artifact";
                t.font = DefaultFont;
                t.alignment = TextAnchor.UpperCenter;

                tmp.AddComponent<VerticalLayoutGroup>();
                img.transform.SetParent(tmp.transform);
                txt.transform.SetParent(tmp.transform);

                Button btn = tmp.AddComponent<Button>();
                btn.targetGraphic = img.GetComponent<Image>();

                string s = player.name + " has found <b>" + item.itemName + "</b>.";
                btn.onClick.AddListener(delegate
                {
                    AnnounceArtefact(item, s);
                    ChoosePlayerPanel.transform.parent.gameObject.SetActive(false);
                });
                tmp.transform.SetParent(ChoosePlayerPanel.transform);
            }
        }
        else
        {
            foreach (var item in selectedInventory) {
                if (item.isArtefact) {
                    string s = player.name + " has found <b>" + item.itemName + "</b>.";
                    AnnounceArtefact(item, s);
                    return;
                }
            }
        }
    }

    public void SetActionsLeft(int actions)
    {
        ActionsLeftText.text = actions +" actions left";
    }

    #region Combat

    public void EnableCombat(GameObject player, GameObject opponent) {
        StartCombatButton.onClick.RemoveAllListeners();
        StartCombatButton.gameObject.SetActive(true);
        StartCombatButton.onClick.AddListener(delegate
        {
            StartCombat(player, opponent);
        });
    }

    public void DisableCombat() {
        StartCombatButton.onClick.RemoveAllListeners();
        StartCombatButton.gameObject.SetActive(false);
        EndCombat(true);
    }

    public void StartCombat(GameObject player, GameObject opponent) {
        print(player.name + " wants to fight " + opponent.name);
        combatPlayer = player;
        combatOpponent = opponent;
        InCombat = true;

        _WinPanel.SetActive(false);
        FightCanvas.SetActive(true);
        StartCombatButton.gameObject.SetActive(false);
        activePlayer = -1;
        FightTextObject.GetComponent<ScrollingText>().SetText(FightText,
            delegate
            {
                SetupBattle(true);
            });
        SetStateOnList(NonFightObjects, false);
        DiceAnimator.gameObject.SetActive(false);
        enemyLives = 10;
        playerLives = 10;
        StartCoroutine(CombatUpdate());
    }

    public void SetupBattle(bool optionsState)
    {
        FightOptionsObject.SetActive(optionsState);
        DiceAnimator.SetBool("IsEnemy", false);
    }

    IEnumerator EnemyCombatTurn()
    {
        yield return new WaitForSeconds(2);
        ThrowDice(1);
    }

    IEnumerator CombatUpdate()
    {
        while (true)
        {
            PlayerHealthObject.text = "Health: " + playerLives;
            OpponentHealthObject.text = "Health: " + enemyLives;

            if (enemyLives <= 0) {
                EndCombat(false);
                break;
            }
            if (playerLives <= 0) {
                EndCombat(false);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void CombatSubtract(bool isEnemy)
    {
        //isEnemy is the caller
        if (isEnemy)
        {
            playerLives -= UnityEngine.Random.Range(1, 7);
        }
        else
        {
            enemyLives -= UnityEngine.Random.Range(1, 7);
        }
    }

    public void ThrowDice(int playerNum) {
        print("I threw a dice: " +playerNum);
        FightOptionsObject.SetActive(false);
        DiceAnimator.gameObject.SetActive(true);
        if (playerNum == 0)
        {
            DiceAnimator.SetTrigger("PlayerDice");
            FightCanvasAnimator.SetTrigger("EnemyDamage");
            StartCoroutine(EnemyCombatTurn());
            FightOptionsObject.SetActive(false);
        }
        else
        {
            DiceAnimator.SetTrigger("EnemyDice");
            DiceAnimator.SetBool("IsEnemy", true);
            FightCanvasAnimator.SetTrigger("PlayerDamage");
            
        }
        
    }

    public void EndCombat(bool isRunning)
    {
        // Simply just reset the combat state
        if (combatPlayer == null && !InCombat)
        {
            FightCanvas.SetActive(false);
            return;
        }

        StopCoroutine(CombatUpdate());
        _WinPanel.SetActive(true);
        DiceAnimator.gameObject.SetActive(false);

        if (!isRunning) {
            if (playerLives <= 0) {
                print("Reset player position");
                GameManager.instance.RemoveAllItems(combatPlayer);
                combatPlayer.transform.position = _SpawnPostion;
            }
            if (enemyLives <= 0) {
                print("Reset enemy position");
                GameManager.instance.RemoveAllItems(combatOpponent);
                combatOpponent.transform.position = _SpawnPostion;
                AI ai = combatOpponent.GetComponent<AI>();
                ai.SetState(new SearchingState(ai));
            }
        }
        
        GameManager.Instance.NextTurn(combatPlayer);
        combatPlayer = null;
        combatOpponent = null;
        SetStateOnList(NonFightObjects, true);
        InCombat = false;
    }

    private void SetStateOnList(List<GameObject> list, bool state) {
        foreach (var o in list) {
            o.SetActive(state);
        }
    }

    #endregion

}
