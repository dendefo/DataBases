using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GameHandeler : MonoBehaviour
{
    public static int UserID;
    public static int GameID;
    public static int currentQuestionID;
    public int QuestionIndicator = 1;
    public int winnerUserID;
    public string winnerUsername;

    [SerializeField] GameObject waitForPlayerScreen;
    [SerializeField] GameObject counter;
    [SerializeField] GameObject questionScreen;
    [SerializeField] GameObject winnerScreen;
    [SerializeField] TMP_Text winnerText;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject menuScreen;
    [SerializeField] TMP_Text errorText;
    [SerializeField] QuestionHandeler questionHandeler;
    [SerializeField] SoundManager soundManager;

    [SerializeField] float checkForGameDelay;
    bool secondPlayerConnected = false;
    bool bothPlayersAnswered = false;
    bool waitingForPlayerAnswer = false;
    float nextActionTime = 0.0f;
    
    private void OnEnable()
    {
        counter.SetActive(false);
        questionScreen.SetActive(false);
        winnerScreen.SetActive(false);
        waitForPlayerScreen.SetActive(true);
    }
    private void Update()
    {
        if (!secondPlayerConnected)
            CheckForGame();
        else
        {
            if (waitingForPlayerAnswer)
            {
                if (!bothPlayersAnswered)
                    CheckForPlayer();
            }
        }
    }
    void CheckForGame()
    {
        if(Time.time > nextActionTime)
        {
            nextActionTime += checkForGameDelay;
            StartCoroutine(WaitForGame(GameID));
        }
    }
    void CheckForPlayer()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += checkForGameDelay;
            StartCoroutine(CheckQuestion(GameID));
        }
    }
    public void DisconnectFromGame()
    {
        StartCoroutine(DissconnectFromServer(UserID));
        gameObject.SetActive(false);
        loginScreen.SetActive(true);
        errorText.gameObject.SetActive(true);
        errorText.text = "Disconnected From Server";
    }
    void StartCounter()
    {
        waitForPlayerScreen.SetActive(false);
        counter.SetActive(true);
        soundManager.PlayTimerMusic();
    }
    public void StartGame()
    {
        bothPlayersAnswered = false;
        questionScreen.SetActive(true);
        questionHandeler.DisplayQuestion(GameID);
        soundManager.PlayQuestionMusic();
    }
    public void EndGame()
    {
        questionScreen.SetActive(false);
        StartCoroutine(GetGameResults(GameID));
        waitForPlayerScreen.SetActive(false);
        winnerScreen.SetActive(true);
        soundManager.PlayWinnerMusic();
    }
    public void BackToMenu()
    {
        menuScreen.SetActive(true);
        soundManager.PlayLobbyMusic();
        gameObject.SetActive(false);
    }
    public void NextQuestion()
    {
        questionHandeler.ResetQuestion();
        questionScreen.SetActive(false);
        if (bothPlayersAnswered)
        {
            if (QuestionIndicator > 5)
            {
                EndGame();
            }
            else
            {
                StartCounter();
            }
        }
        else
        {
            waitingForPlayerAnswer = true;
            waitForPlayerScreen.SetActive(true);
        }
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
                StartCounter();
            }
            else if (www.downloadHandler.text == "false")
            {
                Debug.Log("false");
            }
        }
    }
    IEnumerator CheckQuestion(int gameID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/GetQuestion?GameID=" + gameID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            DisconnectFromGame();
        }
        else
        {
            Question currentQuestion = JsonUtility.FromJson<Question>(www.downloadHandler.text);
            if (currentQuestionID != currentQuestion.QuestionId)
            {
                bothPlayersAnswered = true;
                waitingForPlayerAnswer = false;
                NextQuestion();
            }
            else
            {
                bothPlayersAnswered = false;
            }
        }
    }
    IEnumerator GetGameResults(int gameID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/GameResults?GameID=" + gameID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            DisconnectFromGame();
        }
        else
        {
            GameData gameResults = JsonUtility.FromJson<GameData>(www.downloadHandler.text);
            winnerUserID = gameResults.WinnerID;
            StartCoroutine(GetUsername(winnerUserID));
        }
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
            winnerText.text = playerData._name;
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
public struct GameData
{
    public int WinnerID;
    public int LoserID;
}

