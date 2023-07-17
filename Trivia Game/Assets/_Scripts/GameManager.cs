using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


}
