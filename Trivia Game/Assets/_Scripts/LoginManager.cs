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
    [SerializeField] GameObject mainMenu;
    [SerializeField] SoundManager soundManager;

    private void OnEnable()
    {
        soundManager.PlayLobbyMusic();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoginToServer();
        }
    }
    public void LoginToServer()
    {
        if(CheckUsername(userNameInput.text))
        {
            StartCoroutine(Login(userNameInput.text));
        }
    }
    public void ExitGame()
    {
        Application.Quit();
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
            else if (userID == -2)
            {
                errorText.gameObject.SetActive(true);
                errorText.text = "This user is already connected";
            }
            else
            {
                OpenMainMenu(userID);
            }
        }
    }
    IEnumerator Register(string username)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/Register?username=" + username);
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
                newUserPopup.SetActive(false);
                errorText.gameObject.SetActive(true);
                errorText.text = "Failed To Connect Server";
            }
            else
            {
                OpenMainMenu(userID);
            }
        }
    }
    public void RegisterNewUser()
    {
        StartCoroutine(Register(userNameInput.text));
    }
    public void CloseNewUserPopup()
    {
        newUserPopup.SetActive(false);
    }
    void OpenMainMenu(int userID)
    {
        MenuManager.UserID = userID;
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
    bool CheckUsername(string username)
    {
        if (username == "")
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "You must enter a username first";
            return false;
        }
        else if (username.Length > 8)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Username too long";
            return false;
        }
        else
        {
            return true;
        }
    }
}
