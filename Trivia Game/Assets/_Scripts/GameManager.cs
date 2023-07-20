using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject menuScreen;

    void Awake()
    {
        loginScreen.SetActive(true);
        menuScreen.SetActive(false);
        gameScreen.SetActive(false);
    }
    private void OnApplicationFocus(bool focus)
    {
        
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(DissconnectFromServer(GameHandeler.UserID));
    }

    IEnumerator DissconnectFromServer(int userID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/Disconnect?PlayerID=" + userID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Failed");
        }
        else
        {
            Debug.Log("Dissconnected");
        }
    }
}
