using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] int _number;
    [SerializeField] GameHandeler gameHandeler;
    private void OnEnable()
    {
        text.text = "3";
    }

    public void ChangeText()
    {
        text.text = _number.ToString();
    }

    public void End()
    {
        gameObject.SetActive(false);
        gameHandeler.StartGame();
    }
}
