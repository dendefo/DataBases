using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_InputField userNameInput;
    [SerializeField] TMP_Text errorText;
    [SerializeField] GameObject newUserPopup;
    void Start()
    {
        
    }

    public void LoginToServer()
    {
        if(CheckUsername(userNameInput.text))
        {
            StartCoroutine(Login(userNameInput.text));
        }
    }

    IEnumerator Login(string username)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/Login?username=" + username);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Failed To Connect Server";
        }
        else
        {
            int userID = int.Parse(www.downloadHandler.text);
            if (userID == -1)
            {
                newUserPopup.SetActive(true);
            }
            else
            {
                errorText.gameObject.SetActive(true);
                errorText.text = "UserID = " + userID;
            }
        }
    }
    public void CloseNewUserPopup()
    {
        newUserPopup.SetActive(false);
    }
    bool CheckUsername(string username)
    {
        if (username == "")
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "You must enter a username first";
            return false;
        }
        else
        {
            return true;
        }
    }
}
