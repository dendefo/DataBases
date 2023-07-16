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
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        StartCoroutine(GetUsername(UserID));
    }
    public void PlayButton()
    {

    }
    void ReturnToLogin()
    {

    }
    IEnumerator GetUsername(int userID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/PlayerData?PlayerID=" + userID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error getting username");
            ReturnToLogin();
        }
        else
        {
            PlayerData playerData = new PlayerData();
            playerData = JsonUtility.FromJson<PlayerData>(www.downloadHandler.text);
            userNameText.text = playerData._name.ToString();
            wins.text = playerData._wins.ToString();
            loses.text = playerData._losses.ToString();
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
