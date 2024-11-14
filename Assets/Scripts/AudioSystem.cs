using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class AudioSystem
{
    public static AudioPack instance;

    public static bool UseWaterMixer
    {
        get
        {
            return _useWaterMixer;
        }
        set
        {
            _useWaterMixer = value;
            //instance.UseWaterSound(value);
        }
    }
    private static bool _useWaterMixer;

    public static void PlaySound(AudioClip clip)
    {
        instance.soundSource.PlayOneShot(clip);
    }
}

[Serializable]
public class AudioPack
{

    public AudioSource musicSource;
    public AudioSource soundSource;
    public AudioMixerGroup defaultSondsGroup;
    public AudioMixerGroup waterSondsGroup;


    public AudioClip ShowStarGroupClip;

    public void UseWaterSound(bool value)
    {
        musicSource.outputAudioMixerGroup = soundSource.outputAudioMixerGroup = value ? defaultSondsGroup : waterSondsGroup;
    }
}
