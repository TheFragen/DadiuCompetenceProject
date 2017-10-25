using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventProxy : MonoBehaviour
{
    [SerializeField]
    private ScrollingText FightText;
    [SerializeField]
    private AudioClip AttackSound;
    [SerializeField]
    private GameObject FightCanvas;
    private AudioSource _as;

    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (var source in sources)
        {
            if (source.clip == null)
            {
                _as = source;
                break;
            }
        }
    }

    public void StartScrolling()
    {
        FightText.AnimationStart();
    }

    public void PlayAttackOneShot()
    {
        _as.PlayOneShot(AttackSound);
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    public void DisableFightCanvas()
    {
        if (FightCanvas != null)
        {
            FightCanvas.SetActive(false);
        }
    }
}
