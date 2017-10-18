using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class JuiceController : Singleton<JuiceController>
{
    [SerializeField]
    private Text UseItemCanvas;
    [SerializeField]
    private AnimationCurve UseItemCurve;

    public void SetItemUsedText(string text)
    {
        UseItemCanvas.text = text;
        StartCoroutine(TweenAlpha(UseItemCanvas, 0, 1, 1));
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
