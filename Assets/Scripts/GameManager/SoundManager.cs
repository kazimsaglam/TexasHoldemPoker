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

    private AudioSource audioSource;


    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCardDealSound()
    {
        audioSource.PlayOneShot(cardDealSound);
    }

    public void PlayDingSound()
    {
        audioSource.PlayOneShot(dingSound);
    }

    public void PlayFoldSound()
    {
        audioSource.PlayOneShot(foldSound);
    }

    public void PlayCallAndRaiseSound()
    {
        audioSource.PlayOneShot(callAndRaiseSound);
    }

    public void PlayCheckSound()
    {
        audioSource.PlayOneShot(checkSound);
    }

}
