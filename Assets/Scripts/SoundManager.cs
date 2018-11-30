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
    public float BackgroundLength = 300f;
    int currentBack = -1;
    float lastBackChange;

    private void Awake()
    {
        Instance = this;

        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.outputAudioMixerGroup = MusicGroup;
        MusicSource.playOnAwake = false;
        MusicSource.loop = true;

        FxSource = gameObject.AddComponent<AudioSource>();
        FxSource.outputAudioMixerGroup = FxGroup;
        FxSource.playOnAwake = false;
    }

    void Start()
    {
        PlayBackground();
    }

    void Update()
    {
        if (Time.time - lastBackChange > BackgroundLength)
            PlayBackground();
    }

    public void PlayBackground()
    {
        if (BackgroundAudio.Count > 1)
        {
            int newBack = Random.Range(0, BackgroundAudio.Count);
            while(newBack == currentBack)
                newBack = Random.Range(0, BackgroundAudio.Count);
            currentBack = newBack;
        }
        else
        {
            currentBack = 0;
        }

        MusicSource.clip = BackgroundAudio[currentBack];
        MusicSource.Play();

        lastBackChange = Time.time;
    }
    
    public void PlayGroup(string name)
    {
        if (FXEnabled)
        {
            SoundGroup group = Groups.Find((x) => x.Name == name);
            FxSource.PlayOneShot(group.Clips[Random.Range(0, group.Clips.Count)]);
        }
    }
}