using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager: MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public AudioMixerGroup soundEffectAudioMixerGroup;
    public Transform soundEffectAudioSourceObjectPoolParent;

    List<GameObject> soundEffectAudioSourceObjectPool = new List<GameObject>();

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat(ISettings.Type.MASTERVOLUME.ToString(), 0);
        float musicVolume = PlayerPrefs.GetFloat(ISettings.Type.MUSICVOLUME.ToString(), 0);
        float soundEffectVolume = PlayerPrefs.GetFloat(ISettings.Type.SOUNDEFFECTVOLUME.ToString(), 0);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSoundEffectVolume(soundEffectVolume);

        CreateAudioSourceObjectPool();
    }

    public void SetMasterVolume(float volume) // volume = 0f...1f
    {
        // Log10 is used to convert linear 0...1 -> non linear -80dB...20dB
        // Log10(0) -> sets volume high so we prevent it by using Mathf.Max where -80 is the quietest
        float convertedVolume = Mathf.Max(Mathf.Log10(volume) * 20f, -80f);

        audioMixer.SetFloat(ISettings.Type.MASTERVOLUME.ToString(), convertedVolume);
        PlayerPrefs.SetFloat(ISettings.Type.MASTERVOLUME.ToString(), volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume) // volume = 0f...1f
    {
        float convertedVolume = Mathf.Max(Mathf.Log10(volume) * 20f, -80f);
        audioMixer.SetFloat(ISettings.Type.MUSICVOLUME.ToString(), convertedVolume);
        PlayerPrefs.SetFloat(ISettings.Type.MUSICVOLUME.ToString(), volume);
        PlayerPrefs.Save();
    }

    public void SetSoundEffectVolume(float volume) // volume = 0f...1f
    {
        float convertedVolume = Mathf.Max(Mathf.Log10(volume) * 20f, -80f);
        audioMixer.SetFloat(ISettings.Type.SOUNDEFFECTVOLUME.ToString(), convertedVolume);
        PlayerPrefs.SetFloat(ISettings.Type.SOUNDEFFECTVOLUME.ToString(), volume);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip)
    {
        // dont play if same clip
        if (musicSource.isPlaying && musicSource.clip == clip)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    private void CreateAudioSourceObjectPool()
    {
        // create pool of objects with audio source component
        for(int i = 0; i < 10; i++)
        {
            GameObject go = CreateSoundEffectNewAudioSourceObject();
            soundEffectAudioSourceObjectPool.Add(go);
        }
    }

    private GameObject CreateSoundEffectNewAudioSourceObject()
    {
        GameObject go = new GameObject("PooledSoundEffectAudioSourceObject");
        go.transform.SetParent(soundEffectAudioSourceObjectPoolParent);
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = soundEffectAudioMixerGroup;
        audioSource.playOnAwake = false;
        go.SetActive(false);
        return go;
    }

    private AudioSource GetFreeAudioSource()
    {
        // if pooled object is inactive in hierarchy it's available to be used. After the sound effect has played gameObject is set to be inactive.
        
        foreach(GameObject audioSourceObject in soundEffectAudioSourceObjectPool)
        {
            if (!audioSourceObject.activeInHierarchy)
            {
                audioSourceObject.SetActive(true);
                return audioSourceObject.GetComponent<AudioSource>();
            }
        }

        // no free ones so create a new. This makes the pool flexible but may cause stutter when pool is depleted. Needs testing to find good pool size.
        GameObject go = CreateSoundEffectNewAudioSourceObject();
        soundEffectAudioSourceObjectPool.Add(go);
        go.SetActive(true);
        return go.GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        // find free object from pool
        AudioSource audioSource = GetFreeAudioSource();
        audioSource.PlayOneShot(clip);
        // deactivate object after the clip has been played
        StartCoroutine(DeactivateAfterTime(audioSource.gameObject, clip.length));
    }

    private IEnumerator DeactivateAfterTime(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }
}
