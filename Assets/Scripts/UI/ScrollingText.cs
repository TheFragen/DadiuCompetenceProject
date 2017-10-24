using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    private Text _textComponent;
    private List<string> _scrollingText;
    private UnityAction _scrollingCallback;
    
    private bool _componentClicked;

    // Gets called from the PokemonAnimation as an animator event
    public void AnimationStart()
    {
        StartCoroutine(StartScrolling(_scrollingText, _scrollingCallback));
    }

    IEnumerator StartScrolling(List<string> textList, UnityAction callback)
    {
        for (int i = 0; i < textList.Count; i++)
        {
            string[] words = textList[i].Split(' ');
            string final = "";
            for (int j = 0; j < words.Length; j++)
            {
                // Fix for Coroutines not beeing called properly when in an animation
                if (words[j].Equals("ø"))
                {
                    words[j] = "";
                }
                final += " " + words[j];
                _textComponent.text = final.Trim();
                yield return new WaitForSeconds(1/24f);
            }
            while (!_componentClicked)
            {
                yield return null;
            }
            _componentClicked = false;
        }
        _textComponent.text = "";
        if (callback != null)
        {
            callback.Invoke();
        }
    }

    public void TriggerClicked()
    {
        _componentClicked = true;
    }

    public void SetText(List<string> text, UnityAction callback)
    {
        _scrollingText = text;
        _scrollingCallback = callback;
        _textComponent = GetComponent<Text>();
        if (_scrollingText != null) {
            StartCoroutine(StartScrolling(_scrollingText, _scrollingCallback));
        }
        _scrollingCallback = callback;
        _scrollingText = text;
    }
}