using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

    [SerializeField] AudioSource firstLayer;
    [SerializeField] bool isOnFirstLayer;

    [SerializeField] AudioSource secondLayer;
    [SerializeField] bool isOnSecondLayer;

    [SerializeField] AudioSource thirdLayer;
    [SerializeField] bool isOnthirdLayer;

    [SerializeField] float fadeDuration = 1;
    [SerializeField] float targetVolume = .25f;

    public bool isFading = false;



    void Start()
    {
        
    }

    void Update()
    {
        if (isOnSecondLayer && !isFading)
        {
            Debug.Log("Entering coroutine for second layer");
            StartCoroutine(FadeVolume(secondLayer, secondLayer.volume, targetVolume));
        }

        if (isOnthirdLayer && !isFading)
        {
            StartCoroutine(FadeVolume(thirdLayer, thirdLayer.volume, targetVolume));
        }


        //fade out
        //StartCoroutine(FadeVolume(secondLayer, secondLayer.volume, 0));
    }

    IEnumerator FadeVolume(AudioSource audioSource, float startVolume, float endVolume)
    {
        float elapsedTime = 0f;
        isFading = true;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = endVolume;
        Debug.Log("Setting isfading false");
        isFading = false;

    }
}
