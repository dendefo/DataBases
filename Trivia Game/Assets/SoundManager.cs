using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip lobbyMusic;
    [SerializeField] AudioClip tenSecMusic_1;
    [SerializeField] AudioClip tenSecMusic_2;

    public void PlayLobbyMusic()
    {
        musicSource.clip = lobbyMusic;
        musicSource.Play();
    }
}
