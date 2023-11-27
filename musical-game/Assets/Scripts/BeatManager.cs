using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    AudioPlayer audioPlayer;
    AudioSource bassLayer;
    float bpm;
    [SerializeField] Intervals[] intervals;

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void Start()
    {
        bassLayer = audioPlayer.GetFirstLayer();
        bpm = audioPlayer.GetBeatsPerMinute();
    }

    void Update()
    {
        foreach (Intervals interval in intervals)
        {
            float intervalLength = interval.GetIntervalLength(bpm);
            float sampledTime = (bassLayer.timeSamples / (bassLayer.clip.frequency * intervalLength));
            interval.CheckForNewInterval(sampledTime, intervalLength);
        }
    }
}

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


[System.Serializable]
public class Intervals
{
    [SerializeField] BeatLength selectedBeatLength;
    [SerializeField] FloatUnityEvent trigger;
    float steps;
    int lastInterval;

    public float GetIntervalLength(float bpm)
    {
        string enumName = Enum.GetName(typeof(BeatLength), selectedBeatLength);
        if (enumName != null)
        {
            steps = (float)typeof(BeatLengthValues).GetField(enumName).GetValue(null);
        }
        return 60 / bpm * steps;
    }

    public void CheckForNewInterval (float interval, float intervalLength)
    {
        if(Mathf.FloorToInt(interval) != lastInterval)
        {
            lastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke(intervalLength);
        }
    }
}

[System.Serializable]
public class FloatUnityEvent : UnityEvent<float> { }