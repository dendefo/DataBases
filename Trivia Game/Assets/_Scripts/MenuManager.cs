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
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/GetUsername?usernameID=" + userID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            ReturnToLogin();
        }
        else
        {
            UserName = www.downloadHandler.text.Trim('"');
            userNameText.text = UserName;
        }
    }
}
