using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] List<ListWrapper> audioLayers; // List of Audiosources and their variants. Bass/drums needs to always be unique 

    // private with getter?
    [SerializeField] float beatsPerMinute;

    [SerializeField] float fadeDuration = 1;
    [SerializeField] float targetVolume = .25f;

    GameSession gameSession;

    private void Awake()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    public float GetBeatsPerMinute()
    {
        return beatsPerMinute;
    }


    IEnumerator FadeVolume(AudioSource audioSource, float startVolume, float endVolume)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    public void AddLayer(int layerVariant)
    {
        int playerZone = gameSession.GetCurrentPlayerZone();
        AudioSource audioLayer = audioLayers[playerZone + 1].variants[layerVariant];
        StartCoroutine(FadeVolume(audioLayer, audioLayer.volume, targetVolume));
    }

    public void RemoveLayer(int layerVariant)
    {
        int playerZone = gameSession.GetCurrentPlayerZone();
        AudioSource audioLayer = audioLayers[playerZone].variants[layerVariant];
        StartCoroutine(FadeVolume(audioLayer, audioLayer.volume, 0));
       
    }
}

[System.Serializable]
public class ListWrapper
{
    public List<AudioSource> variants;
}
