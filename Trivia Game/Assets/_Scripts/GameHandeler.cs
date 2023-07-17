using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GameHandeler : MonoBehaviour
{
    public static int UserID;
    public static int GameID;
    [SerializeField] GameObject waitForPlayerScreen;
    [SerializeField] GameObject loginScreen;
    [SerializeField] TMP_Text errorText;
    bool secondPlayerConnected;

    [SerializeField] float checkForGameDelay;
    float nextActionTime = 0.0f;
    private void OnEnable()
    {
        waitForPlayerScreen.SetActive(true);
    }
    private void Update()
    {
            CheckForGame();
            
    }
    void CheckForGame()
    {
        if(Time.time > nextActionTime)
        {
            nextActionTime += checkForGameDelay;
            StartCoroutine(WaitForGame(GameID));
        }
    }
    void DisconnectFromGame()
    {
        gameObject.SetActive(false);
        loginScreen.SetActive(true);
        errorText.gameObject.SetActive(true);
        errorText.text = "Disconnected From Server";
    }

    IEnumerator WaitForGame(int gameID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/WaitForGame/" + gameID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            DisconnectFromGame();
        }
        else
        {
            if(www.downloadHandler.text == "true")
            {
                secondPlayerConnected = true;
                Debug.Log("true");
            }
            else if (www.downloadHandler.text == "false")
            {
                Debug.Log("false");
            }
        }
    }
}
