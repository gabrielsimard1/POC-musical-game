using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlinkingPlatform : MonoBehaviour
{
    float intervalLength = 0;

    [Header("Delay")]
    [SerializeField] bool hasDelay;

    [Header("Skipping Beats")]
    [SerializeField] bool isSkippingBeats;
    [SerializeField] float skippingBeatsCount = 0;
    float skippingBeatsCountAcc = 0;


    Material tilemapMaterial;
    TilemapRenderer tilemapRenderer;
    TilemapCollider2D tilemapCollider;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();

        tilemapMaterial = new Material(tilemapRenderer.material);
        tilemapRenderer.material = tilemapMaterial;
    }

    void Start()
    {
        if (hasDelay)
        {
            tilemapRenderer.enabled = false;
            tilemapCollider.enabled = false;
        }
    }

    public IEnumerator ToggleOpacity(float intervalLength)
    {
        float startTime = Time.time;
        float startAlpha = 1;

        while (Time.time - startTime < intervalLength)
        {
            float normalizedTime = (Time.time - startTime) / intervalLength;
            Color newColor = tilemapMaterial.color;
            newColor.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            tilemapMaterial.color = newColor;

            yield return new WaitForEndOfFrame();
        }
    }

    public void ToggleVisibilityAndCollision(float intervalLengthParam)
    {
        if (intervalLength == 0)
            intervalLength = intervalLengthParam;
        if (!isSkippingBeats || 
            (isSkippingBeats && skippingBeatsCountAcc == skippingBeatsCount - 1))
        {
            tilemapRenderer.enabled = !tilemapRenderer.enabled;
            tilemapCollider.enabled = !tilemapCollider.enabled;

            if (tilemapRenderer.enabled)
                //StartCoroutine(ToggleOpacity(intervalLength));

            skippingBeatsCountAcc = 0;
        }
        else if(isSkippingBeats)
        {
            skippingBeatsCountAcc++;
        }
    }
}
