using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class MusicalLayerTrigger : MonoBehaviour
{
    private static string LAYER_TRIGGER_PREFIX = "LayerTrigger";
    
    GameSession gameSession;
    AudioPlayer audioPlayer;

    private void Awake()
    {
        gameSession = FindObjectOfType<GameSession>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (gameObject.tag.Contains(LAYER_TRIGGER_PREFIX) && collision.CompareTag("Player"))
        {
            (int layerIndex, int layerVariant) extractedIndexes = ExtractIndexes(gameObject.tag);
            int playerZone = gameSession.GetCurrentPlayerZone();
            // comments are assuming the scene is linear from left to right 
            if (playerZone == extractedIndexes.layerIndex - 1) // means we're left from the trigger so we should add a music layer
            {
                audioPlayer.AddLayer(extractedIndexes.layerVariant);
                gameSession.IncrementCurrentPlayerZone();
            }
            else // if playerzone == triggerIndex ==> means we were in the music layer zone and crossed the trigger (from right to left) so we should remove the music layer
            {
                audioPlayer.RemoveLayer(extractedIndexes.layerVariant);
                gameSession.DecrementCurrentPlayerZone();
            }
        }
    }

    private (int layerIndex, int layerVariant) ExtractIndexes(string layerName)
    {
        // Assuming the format is "LayerTriggerXX-YY" with a fixed length of 14 characters
        string numericPart = layerName.Substring(12, 2);  // Extract "XX"
        string variantPart = layerName.Substring(15, 2);  // Extract "YY"

        if (int.TryParse(numericPart, out int layerIndex) && int.TryParse(variantPart, out int layerVariant))
        {
            return (layerIndex, layerVariant);
        }

        throw new InvalidOperationException("Tried parsing " + layerName + " but couldn't. Should be named LayerTriggerXX-YY");
    }
}
