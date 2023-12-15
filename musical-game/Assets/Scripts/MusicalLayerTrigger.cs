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

    // this logic works assuming the trigger is a vertical block for a left to right movement. 
    // at some point if we want to put horizontal triggers we might need to add a [SerializeField] bool to specify so and then we can adjust the logic (checking the velocity.y instead of x)
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.tag.Contains(LAYER_TRIGGER_PREFIX) && collision.CompareTag(Tags.PLAYER_TAG))
        {
            Rigidbody2D playersRigidBody = collision.GetComponent<Rigidbody2D>();
            if (playersRigidBody)
            {
                Vector2 playersVelocity = playersRigidBody.velocity;
                (int layerIndex, int layerVariant) extractedIndexes = ExtractIndexes(gameObject.tag);
                int playerZone = gameSession.GetCurrentPlayerZone();
                // comments are assuming the scene is linear from left to right 
                if (playerZone == extractedIndexes.layerIndex - 1 && playersVelocity.x > 0)
                // means we're left from the trigger so we should add a music layer
                // checking if player is moving from left to right otherwise if he goes back and forth on the trigger it messes up the playerzone
                {
                    audioPlayer.AddLayer(extractedIndexes.layerVariant);
                    gameSession.IncrementCurrentPlayerZone();
                }
                else if (playerZone == extractedIndexes.layerIndex && playersVelocity.x < 0)
                // if playerzone == triggerIndex ==> means we were in the music layer zone and crossed the trigger (from right to left) so we should remove the music layer
                // checking if player is moving from right to left 
                {
                    audioPlayer.RemoveLayer(extractedIndexes.layerVariant);
                    gameSession.DecrementCurrentPlayerZone();
                }
            }

        }
    }

    private (int layerIndex, int layerVariant) ExtractIndexes(string layerName)
    {
        // Assuming the format is "LayerTriggerXX-YY" with a fixed length of 17 characters
        string numericPart = layerName.Substring(12, 2);  // Extract "XX"
        string variantPart = layerName.Substring(15, 2);  // Extract "YY"

        if (int.TryParse(numericPart, out int layerIndex) && int.TryParse(variantPart, out int layerVariant))
        {
            return (layerIndex, layerVariant);
        }

        throw new InvalidOperationException("Tried parsing " + layerName + " but couldn't. Should be named LayerTriggerXX-YY");
    }
}
