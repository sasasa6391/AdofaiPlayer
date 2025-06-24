using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public AudioClip SFX_Sizzle;
    public AudioClip SFX_Kick;
    public AudioClip SFX_Chuck;
    public AudioClip SFX_Hat;
    private static SFXManager _instance = null;

    public HitSound hitSoundType;
    public float hitSoundVolume;
    public static SFXManager Instance => _instance;

    public List<AudioSource> audioPool;
    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(_instance.gameObject);
        }

        _instance = this;

        for (int i = 0; i < 50; i++)
        {
            audioPool.Add(Instantiate(Resources.Load<AudioSource>("Prefabs/AudioPoolObject")));
        }
    }


    public void PlaySFX()
    {
        switch(hitSoundType)
        {
            case HitSound.Sizzle:
                PlayOneShot(SFX_Sizzle, hitSoundVolume / 100.0f);
                break;
            case HitSound.Chuck:
                PlayOneShot(SFX_Chuck, hitSoundVolume / 100.0f);
                break;
            case HitSound.Hat:
                PlayOneShot(SFX_Hat, hitSoundVolume / 100.0f);
                break;
            case HitSound.Kick:
                PlayOneShot(SFX_Kick, hitSoundVolume / 100.0f);
                break;
            default:
                PlayOneShot(SFX_Sizzle, hitSoundVolume / 100.0f);
                break;
        }
    }

    public void PlayOneShot(AudioClip clip, float sound)
    {
        for (int i = 0; i < audioPool.Count; i++)
        {
            if (audioPool[i].isPlaying == false)
            {
                audioPool[i].PlayOneShot(clip, sound);
                return;
            }
        }
    }
}