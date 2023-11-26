using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    AudioPlayer audioPlayer;

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    public float GetQuarterNoteDuration()
    {
        return 60 / audioPlayer.GetBeatsPerMinute();
    }
}
