using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(string seName)
    {
        switch (seName)
        {
            case "PlayBGM":
                audioSource.PlayOneShot(audioClips[0]);
                break;
            case "EndBGM":
                audioSource.PlayOneShot(audioClips[1]);
                break;
        }
    }

    public void StopBgm(){
        audioSource.Stop();
    }
}
