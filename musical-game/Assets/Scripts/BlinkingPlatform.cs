using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlinkingPlatform : MonoBehaviour
{
    TilemapRenderer tilemapRenderer;
    Collider2D platformCollider;
    AudioPlayer audioPlayer;

    [SerializeField]

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        float intervalInSeconds = 60f / audioPlayer.GetBeatsPerMinute();
        StartCoroutine(Blink(intervalInSeconds));
    }

    IEnumerator Blink(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            ToggleVisibilityAndCollision();
        }
    }

    void ToggleVisibilityAndCollision()
    {
        tilemapRenderer.enabled = !tilemapRenderer.enabled;
        platformCollider.enabled = !platformCollider.enabled;
    }
}
