using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BeatLength
{
    WholeNote,
    HalfNote,
    QuarterNote,
    EighthNote,
    Triplet
}

public static class BeatLengthValues
{
    public const float WholeNote = 4f;
    public const float HalfNote = 2f;
    public const float QuarterNote = 1f;
    public const float EighthNote = 0.5f;
    public const float Triplet = 0.333f;
}

public class BlinkingPlatform : MonoBehaviour
{
    BeatManager beatManager;
    float blinkDelay;
    [SerializeField] BeatLength selectedBeatLength;

    TilemapRenderer tilemapRenderer;
    TilemapCollider2D tilemapCollider;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
        beatManager = FindObjectOfType<BeatManager>();
    }

    void Start()
    {
        GetBlinkDelay();
        StartCoroutine(Blink());
    }

    void GetBlinkDelay()
    {
        string enumName = Enum.GetName(typeof(BeatLength), selectedBeatLength);
        if (enumName != null)
        {
            float beatLengthValue = (float)typeof(BeatLengthValues).GetField(enumName).GetValue(null);
            blinkDelay = beatLengthValue * beatManager.GetQuarterNoteDuration();
        }
    }

    IEnumerator Blink()
    {
        while (true)
        {
            ToggleVisibilityAndCollision();
            yield return new WaitForSecondsRealtime(blinkDelay);
        }
    }

    void ToggleVisibilityAndCollision()
    {
        tilemapRenderer.enabled = !tilemapRenderer.enabled;
        tilemapCollider.enabled = !tilemapCollider.enabled;
    }
}
