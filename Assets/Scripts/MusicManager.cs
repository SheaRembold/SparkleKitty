using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [System.Serializable]
    public class SoundGroup
    {
        public string Name;
        public List<AudioClip> Clips = new List<AudioClip>();
    }
    public List<SoundGroup> Groups = new List<SoundGroup>();

    public List<AudioClip> ButtonClicks = new List<AudioClip>();
    public List<AudioClip> BackgroundAudio = new List<AudioClip>();
    public AudioMixerGroup MusicGroup;
    public AudioMixerGroup FxGroup;
    AudioSource MusicSource;
    AudioSource FxSource;
    public  bool FXEnabled = true;

    private void Awake()
    {
        instance = this;
        MusicSource = GetComponent<AudioSource>();
        FxSource = GetComponent<AudioSource>();
        
        MusicSource.outputAudioMixerGroup = MusicGroup;
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
        FxSource.loop = false;
        if (FXEnabled)
        {
            MusicSource.outputAudioMixerGroup = FxGroup;
            MusicSource.PlayOneShot(ButtonClicks[0]);
        }

    }

    public void SimpleMapClick()
    {
        FxSource.loop = false;
        if (FXEnabled)
        {
            MusicSource.outputAudioMixerGroup = FxGroup;
            MusicSource.PlayOneShot(ButtonClicks[1]);
        }
    }

    public void PlayGroup(string name)
    {
        SoundGroup group = Groups.Find((x) => x.Name == name);
        FxSource.PlayOneShot(group.Clips[Random.Range(0, group.Clips.Count)]);
    }
}