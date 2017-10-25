using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UnityEngine;
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

    [Header("Combat")]
    [SerializeField, Tooltip("The GameObjects that wont be disabled or enabled once combat is entered or terminated.")]
    private List<GameObject> ObjectsToIgnore;

    private List<Transform> objectsToHide;

    private void OnEnable()
    {
        FightController.OnCombatStarted += CombatStart;
        FightController.OnCombatEnded += CombatEnd;
    }

    private void OnDisable()
    {
        FightController.OnCombatStarted -= CombatStart;
        FightController.OnCombatEnded -= CombatEnd;
    }

    private void Start()
    {
        objectsToHide = transform.GetChildren().Where(o => !ObjectsToIgnore.Contains(o.gameObject)).ToList();
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
            img.transform.SetParent(tmp.transform, false);
            txt.transform.SetParent(tmp.transform, false);

            Button btn = tmp.AddComponent<Button>();
            btn.targetGraphic = img.GetComponent<Image>();
            if (hasAnyArtifact)
            {
                btn.onClick.AddListener(delegate
                {
                    RevealArtefact(op);
                });
            }
            tmp.transform.SetParent(ChoosePlayerPanel.transform, false);
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
                img.transform.SetParent(tmp.transform, false);
                txt.transform.SetParent(tmp.transform, false);

                Button btn = tmp.AddComponent<Button>();
                btn.targetGraphic = img.GetComponent<Image>();

                string s = player.name + " has found <b>" + item.itemName + "</b>.";
                btn.onClick.AddListener(delegate
                {
                    AnnounceArtefact(item, s);
                    ChoosePlayerPanel.transform.parent.gameObject.SetActive(false);
                });
                tmp.transform.SetParent(ChoosePlayerPanel.transform, false);
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

    private void CombatStart()
    {
        foreach (var child in objectsToHide)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void CombatEnd()
    {
        foreach (var child in objectsToHide)
        {
            child.gameObject.SetActive(true);
        }
    }

}
