using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class QuestionHandeler : MonoBehaviour
{
    [SerializeField] GameHandeler gameHandeler;

    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text questionNumText;
    [SerializeField] TMP_Text questionText;
    [SerializeField] GameObject[] answers;
    [SerializeField] TMP_Text[] answersText;
    [SerializeField] GameObject[] currectAnswers;
    int questionNum = 1;
    int CorrectAnswerID;

    [SerializeField] float questionTimer = 10f;
    [SerializeField] float waitTimer = 3f;
    private void OnEnable()
    {
        questionTimer = 10f;
        questionNumText.text = questionNum.ToString();
        questionNum++;
    }
    private void Update()
    {
        if (questionTimer > 0)
        {
            questionTimer -= Time.deltaTime;
            timerText.text = Mathf.RoundToInt(questionTimer).ToString();
        }
        else
        {
            ShowAnswer(CorrectAnswerID);
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            else
            {
                waitTimer = 3f;
                gameHandeler.NextQuestion();
            }
        }
    }
    public void ReceivePlayerAnswer(int answer)
    {
        bool isAnswerCorrect = false;
        float answerTime = 10f - questionTimer;
        if (answer == CorrectAnswerID) isAnswerCorrect = true;
        for (int i = 0; i < answers.Length; i++)
        {
            if (answer != i+1)
            {
                var boxColor = answers[i].GetComponent<Image>().color;
                answers[i].GetComponent<Image>().color = new Color(boxColor.r, boxColor.g, boxColor.b, 0.5f);
            }
        }
        for (int i = 0; i < answersText.Length; i++)
        {
            if (answer != i + 1)
            {
                var textColor = answersText[i].color;
                answersText[i].color = new Color(textColor.r, textColor.g, textColor.b, 0.5f);
            }
        }
        StartCoroutine(SendPlayerAnswer(GameHandeler.GameID, GameHandeler.UserID, answerTime, isAnswerCorrect));
    }
    public void ResetQuestion()
    {
        foreach (var answer in answers)
        {
            var boxColor = answer.GetComponent<Image>().color;
            answer.GetComponent<Image>().color = new Color(boxColor.r, boxColor.g, boxColor.b, 1f);
        }
        foreach (var answerText in answersText)
        {
            var textColor = answerText.color;
            answerText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
        }
        foreach (var currectAnswer in currectAnswers)
        {
            currectAnswer.GetComponent<Image>().enabled = false;
        }
    }
    public void DisplayQuestion(int gameID)
    {
        StartCoroutine(GetQuestion(gameID));
    }
    void ShowAnswer(int currectAnswerID)
    {
        switch (currectAnswerID)
        {
            case 1:
                currectAnswers[0].GetComponent<Image>().enabled = true;
                break;
            case 2:
                currectAnswers[1].GetComponent<Image>().enabled = true;
                break;
            case 3:
                currectAnswers[2].GetComponent<Image>().enabled = true;
                break;
            case 4:
                currectAnswers[3].GetComponent<Image>().enabled = true;
                break;
        }
    }
    IEnumerator GetQuestion(int gameID)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/GetQuestion?GameID=" + gameID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            gameHandeler.DisconnectFromGame();
        }
        else
        {
            Question currentQuestion = JsonUtility.FromJson<Question>(www.downloadHandler.text);
            if (currentQuestion.QuestionId == 0)
            {
                gameHandeler.EndGame();
            }
            GameHandeler.currentQuestionID = currentQuestion.QuestionId;
            questionText.text = currentQuestion.QuestionText;
            answersText[0].text = currentQuestion.Answers[0];
            answersText[1].text = currentQuestion.Answers[1];
            answersText[2].text = currentQuestion.Answers[2];
            answersText[3].text = currentQuestion.Answers[3];
            CorrectAnswerID = currentQuestion.CorrectAnswerIndex;
        }
    }
    IEnumerator SendPlayerAnswer(int gameID, int playerID, float answerTime, bool isAnswerRight)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://localhost:44339/api/UpdatePlayerAnswer?GameID=" + gameID + 
            "&PlayerID=" + playerID + 
            "&AnswerTime=" + answerTime + 
            "&IsAnswerRight=" + isAnswerRight);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            gameHandeler.DisconnectFromGame();
        }
        else
        {
            Debug.Log("Answer Updated Succesfully");
        }
    }
}
public struct Question
{
    public int QuestionId;
    public string QuestionText;
    public string[] Answers;
    public int CorrectAnswerIndex;
}