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
    public AudioMixerGroup uiSoundEffectAudioMixerGroup;
    public Transform soundEffectAudioSourceObjectPoolParent;
    public Transform uiSoundEffectAudioSourceObjectPoolParent;

    List<GameObject> soundEffectAudioSourceObjectPool = new List<GameObject>();
    List<GameObject> uiSoundEffectAudioSourceObjectPool = new List<GameObject>();

    private void Start()
    {   
        float masterVolume = PlayerPrefs.GetFloat(ISettings.Type.MASTERVOLUME.ToString(), 0.5f);
        float musicVolume = PlayerPrefs.GetFloat(ISettings.Type.MUSICVOLUME.ToString(), 0.5f);
        float soundEffectVolume = PlayerPrefs.GetFloat(ISettings.Type.SOUNDEFFECTVOLUME.ToString(), 0.5f);
        float UIVolume = PlayerPrefs.GetFloat(ISettings.Type.UIVOLUME.ToString(), 0.5f);

        SetVolume(ISettings.Type.MASTERVOLUME, masterVolume);
        SetVolume(ISettings.Type.MUSICVOLUME, musicVolume);
        SetVolume(ISettings.Type.SOUNDEFFECTVOLUME, soundEffectVolume);
        SetVolume(ISettings.Type.UIVOLUME, UIVolume);

        CreateAudioSourceObjectPool();
    }

    public void SetVolume(ISettings.Type type, float volume) // volume = 0f...1f
    {
        // Log10 is used to convert linear 0...1 -> non linear -80dB...20dB
        // Log10(0) -> sets volume high so we prevent it by using Mathf.Max where -80 is the quietest

        Debug.Log("" + type.ToString() + ", volume=" + volume);

        if(type == ISettings.Type.MASTERVOLUME || type == ISettings.Type.SOUNDEFFECTVOLUME 
            || type == ISettings.Type.MUSICVOLUME || type == ISettings.Type.UIVOLUME)
        {
            float convertedVolume = Mathf.Max(Mathf.Log10(volume) * 20f, -80f);
            audioMixer.SetFloat(type.ToString(), convertedVolume);
        }
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

    public void PauseMusic(bool pause)
    {
        if (pause)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.UnPause();
        }
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
            GameObject go1 = CreateSoundEffectNewAudioSourceObject(soundEffectAudioMixerGroup);
            go1.transform.SetParent(soundEffectAudioSourceObjectPoolParent);
            soundEffectAudioSourceObjectPool.Add(go1);

            GameObject go2 = CreateSoundEffectNewAudioSourceObject(uiSoundEffectAudioMixerGroup);
            go2.transform.SetParent(uiSoundEffectAudioSourceObjectPoolParent);
            uiSoundEffectAudioSourceObjectPool.Add(go2);
        }

        
    }

    private GameObject CreateSoundEffectNewAudioSourceObject(AudioMixerGroup audioMixerGroup)
    {
        string objName = audioMixerGroup == uiSoundEffectAudioMixerGroup ? "PooledUISoundEffectAudioSourceObject" : audioMixerGroup == soundEffectAudioMixerGroup ? "PooledSoundEffectAudioSourceObject" : "objNameProblem";
        GameObject go = new GameObject(objName);
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.playOnAwake = false;
        go.SetActive(false);
        return go;
    }

    private AudioSource GetFreeAudioSource(AudioMixerGroup audioMixerGroup)
    {
        // if pooled object is inactive in hierarchy it's available to be used. After the sound effect has played gameObject is set to be inactive.
        
        if(audioMixerGroup == soundEffectAudioMixerGroup)
        {
            foreach (GameObject audioSourceObject in soundEffectAudioSourceObjectPool)
            {
                if (!audioSourceObject.activeInHierarchy)
                {
                    audioSourceObject.SetActive(true);
                    return audioSourceObject.GetComponent<AudioSource>();
                }
            }

            // no free ones so create a new. This makes the pool flexible but may cause stutter when pool is depleted. Needs testing to find good pool size.
            GameObject go = CreateSoundEffectNewAudioSourceObject(soundEffectAudioMixerGroup);
            go.transform.SetParent(soundEffectAudioSourceObjectPoolParent);
            soundEffectAudioSourceObjectPool.Add(go);
            go.SetActive(true);
            return go.GetComponent<AudioSource>();
        }
        else if(audioMixerGroup == uiSoundEffectAudioMixerGroup)
        {
            foreach (GameObject audioSourceObject in uiSoundEffectAudioSourceObjectPool)
            {
                if (!audioSourceObject.activeInHierarchy)
                {
                    audioSourceObject.SetActive(true);
                    return audioSourceObject.GetComponent<AudioSource>();
                }
            }

            // no free ones so create a new. This makes the pool flexible but may cause stutter when pool is depleted. Needs testing to find good pool size.
            GameObject go = CreateSoundEffectNewAudioSourceObject(uiSoundEffectAudioMixerGroup);
            go.transform.SetParent(uiSoundEffectAudioSourceObjectPoolParent);
            uiSoundEffectAudioSourceObjectPool.Add(go);
            go.SetActive(true);
            return go.GetComponent<AudioSource>();
        }
        else
        {
            return null;
        }
        
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        // find free object from pool
        AudioSource audioSource = GetFreeAudioSource(soundEffectAudioMixerGroup);
        audioSource.PlayOneShot(clip);
        // deactivate object after the clip has been played
        StartCoroutine(DeactivateAfterTime(audioSource.gameObject, clip.length));
    }

    public void PlayUISoundEffect(AudioClip clip)
    {
        // find free object from pool
        AudioSource audioSource = GetFreeAudioSource(uiSoundEffectAudioMixerGroup);
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
