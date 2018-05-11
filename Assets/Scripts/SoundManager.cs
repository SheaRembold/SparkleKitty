using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class SoundGroup
    {
        public string Name;
        public List<AudioClip> Clips = new List<AudioClip>();
    }
    public List<SoundGroup> Groups = new List<SoundGroup>();
    
    public List<AudioClip> BackgroundAudio = new List<AudioClip>();
    public AudioMixerGroup MusicGroup;
    public AudioMixerGroup FxGroup;
    AudioSource MusicSource;
    AudioSource FxSource;
    public bool FXEnabled = true;

    private void Awake()
    {
        Instance = this;

        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.outputAudioMixerGroup = MusicGroup;
        MusicSource.playOnAwake = false;

        FxSource = gameObject.AddComponent<AudioSource>();
        FxSource.outputAudioMixerGroup = FxGroup;
        FxSource.playOnAwake = false;
    }

    void Start()
    {
        MusicSource.loop = true;
        PlayBackground(true);
    }

    public void PlayBackground(bool flipper)
    {
        if (flipper)
        {
            MusicSource.clip = BackgroundAudio[1];
            MusicSource.Play();
        }
        else
        {
            MusicSource.Stop();
        }
    }


    void Update()
    {

    }

    public void SimpleButtonClick()
    {
        if (FXEnabled)
        {
            PlayGroup("ButtonClick");
        }

    }

    public void SimpleMapClick()
    {
        if (FXEnabled)
        {
            PlayGroup("MapClick");
        }
    }

    public void PlayGroup(string name)
    {
        SoundGroup group = Groups.Find((x) => x.Name == name);
        FxSource.PlayOneShot(group.Clips[Random.Range(0, group.Clips.Count)]);
    }
}