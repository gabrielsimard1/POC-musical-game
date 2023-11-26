using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    GameSession instance;
    int currentPlayerZone = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

    public int GetCurrentPlayerZone()
    {
        return currentPlayerZone;
    }

    public void IncrementCurrentPlayerZone()
    {
        currentPlayerZone++;
    }

    public void DecrementCurrentPlayerZone()
    {
        currentPlayerZone--;
    }

    
}
