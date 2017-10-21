using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class JuiceController : Singleton<JuiceController>
{
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
        UseItemCanvas.text = text;
        StartCoroutine(TweenAlpha(UseItemCanvas, 0, 1, 1));
    }

    public void SetNeededArtefact(AbstractItem item)
    {
        NeededArtefactCanvas.text = "You need to find\n" +item.name;
        StartCoroutine(TweenAlpha(NeededArtefactCanvas, 0, 1, 0));
    }

    public void AnnounceArtefact(AbstractItem item) {
        ArtefactAnnounceCanvas.text = item.owner.name +" has found his artefact.\nCatch him before he returns to Tristam";
        StartCoroutine(TweenAlpha(ArtefactAnnounceCanvas, 0, 1, 0));
    }

    IEnumerator TweenAlpha(Text affect, float fromAlpha, float toAlpha, float holdTime)
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
            StartCoroutine(TweenAlpha(affect, 1, 0, 0));
        }
        
        
    }
}
