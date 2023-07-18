using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip lobbyMusic;
    [SerializeField] AudioClip tenSecMusic_1;
    [SerializeField] AudioClip tenSecMusic_2;
    [SerializeField] AudioClip threeSecTimer;

    public void PlayLobbyMusic()
    {
        musicSource.clip = lobbyMusic;
        musicSource.Play();
    }
    public void PlayTimerMusic()
    {
        musicSource.PlayOneShot(threeSecTimer);
    }

    public void PlayQuestionMusic()
    {
        musicSource.Stop();
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0:
                musicSource.PlayOneShot(tenSecMusic_1);
                break;
            case 1:
                musicSource.PlayOneShot(tenSecMusic_2);
                break;
        }
    }
}
