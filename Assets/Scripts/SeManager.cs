using UnityEngine;

public class SeManager : MonoBehaviour
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
            case "Charge":
                audioSource.PlayOneShot(audioClips[0]);
                break;
            case "Jump":
                audioSource.PlayOneShot(audioClips[1]);
                break;
            case "Land":
                audioSource.PlayOneShot(audioClips[2]);
                break;
        }
    }

    public void StopSe(){
        audioSource.Stop();
    }

}