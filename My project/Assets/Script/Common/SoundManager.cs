using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    GameManager gameManager;

    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource bgmSource;

    public void SfxSound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void BgmSound(AudioClip clip)
    {
        bgmSource.PlayOneShot(clip);
        bgmSource.loop = true;
        
    }

    public void BgmStop()
    {
        bgmSource.Stop();
    }
}
