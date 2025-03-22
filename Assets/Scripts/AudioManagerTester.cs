using System.Collections;
using UnityEngine;

public class AudioManagerTester : MonoBehaviour
{
    [Header("Persistent volumes, these will be remembered in next session")]
    [Range(0f, 1f)]
    public float MasterVolumeValue;

    [Range(0f, 1f)]
    public float MusicVolumeValue;

    [Range(0f, 1f)]
    public float SoundEffectVolumeValue;

    float currentMaster;
    float currentMusic;
    float currentSoundEffect;

    [Header("Music")]
    public AudioClip musicAudioClip;
    public bool PlayMusic;
    bool currentPlayMusic;

    [Header("Sound Effect")]
    public bool playSoundEffectEveryInterval;
    bool currentPlaySound;
    public AudioClip soundEffectAudioClip;
    public float soundEffectInterval = 1f;


    private void Start()
    {
        // get saved values
        MasterVolumeValue = PlayerPrefs.GetFloat(ISettings.Type.MASTERVOLUME.ToString(), 0f);
        MusicVolumeValue = PlayerPrefs.GetFloat(ISettings.Type.MUSICVOLUME.ToString(), 0f);
        SoundEffectVolumeValue = PlayerPrefs.GetFloat(ISettings.Type.SOUNDEFFECTVOLUME.ToString(), 0f);

        currentPlaySound = playSoundEffectEveryInterval;
        currentPlayMusic = PlayMusic;
    }

    // if there are changes in unity inspector
    private void OnValidate()
    {
        if(currentMaster != MasterVolumeValue) 
        {
            AudioManager.Instance.SetVolume(ISettings.Type.MASTERVOLUME, MasterVolumeValue);
            currentMaster = MasterVolumeValue;
        }

        if (currentMusic != MusicVolumeValue)
        {
            AudioManager.Instance.SetVolume(ISettings.Type.MUSICVOLUME, MusicVolumeValue);
            currentMusic = MusicVolumeValue;
        }

        if (currentSoundEffect != SoundEffectVolumeValue)
        {
            AudioManager.Instance.SetVolume(ISettings.Type.SOUNDEFFECTVOLUME, SoundEffectVolumeValue);
            currentSoundEffect = SoundEffectVolumeValue;
        }

        if(currentPlaySound != playSoundEffectEveryInterval)
        {
            if (playSoundEffectEveryInterval)
            {
                // play sound effect every interval
                StartCoroutine(PlaySoundEffect());
            }
            currentPlaySound = playSoundEffectEveryInterval;
        }

        if(currentPlayMusic != PlayMusic)
        {
            if (PlayMusic)
            {
                // start the music
                AudioManager.Instance.PlayMusic(musicAudioClip);
            }
            else
            {
                AudioManager.Instance.StopMusic();
            }
            currentPlayMusic = PlayMusic;
        }
    }

    private IEnumerator PlaySoundEffect()
    {
        while (playSoundEffectEveryInterval)
        {
            // prevent starting new every 0 seconds
            if (soundEffectInterval <= 0f) soundEffectInterval = 0.01f;
            yield return new WaitForSeconds(soundEffectInterval);
            AudioManager.Instance.PlaySoundEffect(soundEffectAudioClip);
        }
    }
    
}
