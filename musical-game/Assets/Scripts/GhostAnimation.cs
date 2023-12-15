using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimation : MonoBehaviour
{
    Animator ghostInstanceAnimator;

    void Awake()
    {
        ghostInstanceAnimator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        ChooseGhostAnimation();
    }

    void ChooseGhostAnimation()
    {
        string[] animationNames = GetAnimationNames(ghostInstanceAnimator);
        string randomAnimation = animationNames[Random.Range(0, animationNames.Length)];
        ghostInstanceAnimator.SetBool(randomAnimation, true);
    }

    string[] GetAnimationNames(Animator ghostInstanceAnimator)
    {
        List<string> animationNames = new();
        foreach (var controller in ghostInstanceAnimator.runtimeAnimatorController.animationClips)
            animationNames.Add(controller.name);
        return animationNames.ToArray();
    }
}
