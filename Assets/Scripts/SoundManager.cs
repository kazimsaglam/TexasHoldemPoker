using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Sounds Clip")]
    public AudioClip dingSound;
    public AudioClip foldSound;
    public AudioClip callAndRaiseSound;
    public AudioClip checkSound;
    public AudioClip cardDealSound;

    private AudioSource audioSrc;


    private void Awake()
    {
        instance = this;

        audioSrc = GetComponent<AudioSource>();
    }

    public void PlayCardDealSound()
    {
        audioSrc.PlayOneShot(cardDealSound);
    }

    public void PlayDingSound()
    {
        audioSrc.PlayOneShot(dingSound);
    }

    public void PlayFoldSound()
    {
        audioSrc.PlayOneShot(foldSound);
    }

    public void PlayCallAndRaiseSound()
    {
        audioSrc.PlayOneShot(callAndRaiseSound);
    }

    public void PlayCheckSound()
    {
        audioSrc.PlayOneShot(checkSound);
    }

}
