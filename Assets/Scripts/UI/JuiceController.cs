using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private Text NeededArtefactCanvas;
    [SerializeField]
    private Text ArtefactAnnounceCanvas;

    public void SetEndTurnButton(GameObject player)
    {
        if (player != null && player.name.ToUpper().Equals("PLAYER"))
        {
            NextTurnButton.interactable = true;
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
        NeededArtefactCanvas.text = "You need to find\n" +item.name;
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
            .FindGameObjectsWithTag("Player").Where(o => o != player).ToList();
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
            img.transform.parent = tmp.transform;
            txt.transform.parent = tmp.transform;

            Button btn = tmp.AddComponent<Button>();
            btn.targetGraphic = img.GetComponent<Image>();
            if (hasAnyArtifact)
            {
                btn.onClick.AddListener(delegate
                {
                    RevealArtefact(op);
                });
            }
            tmp.transform.parent = ChoosePlayerPanel.transform;
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
                tmp.name = "ChooseArtifact_" + item.name;
                GameObject img = new GameObject();
                img.AddComponent<Image>().sprite = item.isIdentified ? item.sprite : new Sprite();
                GameObject txt = new GameObject();

                Text t = txt.AddComponent<Text>();
                t.text = item.isIdentified ? item.name : "Unidentified Artifact";
                t.font = DefaultFont;
                t.alignment = TextAnchor.UpperCenter;

                tmp.AddComponent<VerticalLayoutGroup>();
                img.transform.parent = tmp.transform;
                txt.transform.parent = tmp.transform;

                Button btn = tmp.AddComponent<Button>();
                btn.targetGraphic = img.GetComponent<Image>();

                string s = player.name + " has found <b>" + item.name + "</b>.";
                btn.onClick.AddListener(delegate
                {
                    AnnounceArtefact(item, s);
                    ChoosePlayerPanel.transform.parent.gameObject.SetActive(false);
                });
                tmp.transform.parent = ChoosePlayerPanel.transform;
            }
        }
        else
        {
            foreach (var item in selectedInventory) {
                if (item.isArtefact) {
                    string s = player.name + " has found <b>" + item.name + "</b>.";
                    AnnounceArtefact(item, s);
                    return;
                }
            }
        }
    }
}
