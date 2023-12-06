using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] float timeBeforeReload = 2;

    public static GameSession Instance { get; private set; }
    int currentPlayerZone = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

    public void ReloadScene()
    {
        StartCoroutine(StartReloadScene());
    }

    IEnumerator StartReloadScene()
    {
        yield return new WaitForSeconds(timeBeforeReload);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
