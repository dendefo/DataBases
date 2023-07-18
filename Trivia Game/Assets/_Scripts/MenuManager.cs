using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static int UserID;
    public static string UserName;
    [SerializeField] TMP_Text userNameText;
    [SerializeField] TMP_Text wins;
    [SerializeField] TMP_Text loses;
    [SerializeField] TMP_Text errorText;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject gameScreen;
    private void OnEnable()
    {
        StartCoroutine(GetUsername(UserID));
    }
    public void PlayButton()
    {
        StartCoroutine(CreateGame(UserID));
    }
    public void BackToLogin()
    {
        StartCoroutine(DissconnectFromServer(UserID));
        gameObject.SetActive(false);
        loginScreen.SetActive(true);
    }
    void StartGame(int gameID, int userID)
    {
        GameHandeler.GameID = gameID;
        GameHandeler.UserID = userID;
        gameScreen.SetActive(true);
        gameObject.SetActive(false);
    }
    void DisconnectFromGame()
    {
        StartCoroutine(DissconnectFromServer(UserID));
        gameObject.SetActive(false);
        loginScreen.SetActive(true);
        errorText.gameObject.SetActive(true);
        errorText.text = "Disconnected From Server";
    }
    IEnumerator GetUsername(int userID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/PlayerData?PlayerID=" + userID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error getting username");
            DisconnectFromGame();
        }
        else
        {
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(www.downloadHandler.text);
            userNameText.text = playerData._name.ToString();
            wins.text = playerData._wins.ToString();
            loses.text = playerData._losses.ToString();
        }
    }
    IEnumerator CreateGame(int userID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/CreateGame/" + userID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Failed to Create Game");
            DisconnectFromGame();
        }
        else
        {
            int gameID = int.Parse(www.downloadHandler.text);
            StartGame(gameID, userID);
        }
    }
    IEnumerator DissconnectFromServer(int userID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/Disconnect?PlayerID=" + userID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            DisconnectFromGame();
        }
        else
        {
            Debug.Log("Dissconnected");
        }
    }
}

public struct PlayerData
{
    public string _name;
    public int _amountOfGames;
    public int _wins;
    public int _losses;
}
