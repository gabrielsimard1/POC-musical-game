using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectMask2D mask;

    float maxRightMask;
    float initialRightMask;


    private void Start()
    {
        maxRightMask = rectTransform.rect.width - mask.padding.x - mask.padding.z;
        initialRightMask = mask.padding.z;
    }

    public void SetHealthBarValue(int newValue)
    {
        var targetWidth = newValue * maxRightMask / health.GetMaxHealth();
        var newRightMask = maxRightMask + initialRightMask - targetWidth;
        var padding = mask.padding;
        padding.z = newRightMask;
        mask.padding = padding;

    }

}
