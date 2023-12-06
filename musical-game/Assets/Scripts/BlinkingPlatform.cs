using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlinkingPlatform : MonoBehaviour
{
    float intervalLength = 0;
    Material tilemapMaterial;
    TilemapRenderer tilemapRenderer;
    TilemapCollider2D tilemapCollider;
    float currentPlatformAlpha = 1.0f;
    Color tilemapColor;

    [Header("Delay")]
    [SerializeField, Tooltip("If true, will start with collider disabled.\nIf Use Blink Sequence is also true, " +
        "will start on Interval 2 of the blink sequence instead")] bool hasDelay;

    [Header("Blink Sequence")]
    [SerializeField] bool useBlinkSequence;
    [SerializeField, Tooltip("On/Off state during each interval.\nOne element represents 1 * this tilemap's " +
        "figure of note (e.g., if 1/1 or whole note, then one interval = 4 beats).")] bool[] blinkSequenceIntervals = new bool[2];

    int sequenceIndex = 0;


    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();

        tilemapMaterial = new Material(tilemapRenderer.material);
        tilemapRenderer.material = tilemapMaterial;
        tilemapColor = tilemapMaterial.color;
    }

    void Start()
    {
        // if sequence is true, delay acts as starting the sequence one interval ahead
        if (hasDelay && useBlinkSequence)
            sequenceIndex++;
        // if a blink sequence has been programmed, trigger toggle to start the sequence (otherwise will need to wait an entire interval to enter sequence)
        if (hasDelay || useBlinkSequence)
            ToggleVisibilityAndCollision();
    }

    void ToggleOpacity()
    {
        currentPlatformAlpha = (currentPlatformAlpha == 1.0f) ? 0.25f : 1.0f;
        tilemapColor.a = currentPlatformAlpha;
        tilemapMaterial.color = tilemapColor;
    }

    public void ToggleVisibilityAndCollision(float intervalLengthParam = -1)
    {
        // Zombie code once used to change tile progressively (e.g., opacity)
        if (intervalLengthParam > 0)
            intervalLength = intervalLengthParam;
        // REMOVE intervalLength IF NOT IMPLEMENTED AFTER FIRST LEVEL IS COMPLETE


        if (useBlinkSequence)
        {
            bool initialTilemapColliderEnabled = tilemapCollider.enabled;
            tilemapCollider.enabled = blinkSequenceIntervals[sequenceIndex];
            if (tilemapCollider.enabled != initialTilemapColliderEnabled)
                ToggleOpacity();

            sequenceIndex = (sequenceIndex + 1) % (blinkSequenceIntervals.Length);
        }
        else
        {
            tilemapCollider.enabled = !tilemapCollider.enabled;
            ToggleOpacity();
        }
    }
}
