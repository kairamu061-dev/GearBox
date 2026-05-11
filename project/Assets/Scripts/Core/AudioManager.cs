using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;

    public float MasterVolume { get; private set; } = 1f;
    public float BgmVolume { get; private set; } = 1f;
    public float SeVolume { get; private set; } = 1f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumeSettings();
    }

    public void PlayBgm(AudioClip clip, bool loop = true)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBgm() => bgmSource.Stop();

    public void PlaySe(AudioClip clip)
    {
        if (clip == null) return;
        seSource.PlayOneShot(clip, MasterVolume * SeVolume);
    }

    public void SetMasterVolume(float v) { MasterVolume = v; ApplyVolumes(); SaveVolumeSettings(); }
    public void SetBgmVolume(float v) { BgmVolume = v; ApplyVolumes(); SaveVolumeSettings(); }
    public void SetSeVolume(float v) { SeVolume = v; ApplyVolumes(); SaveVolumeSettings(); }

    void ApplyVolumes()
    {
        bgmSource.volume = MasterVolume * BgmVolume;
        seSource.volume = MasterVolume * SeVolume;
    }

    void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("BgmVolume", BgmVolume);
        PlayerPrefs.SetFloat("SeVolume", SeVolume);
        PlayerPrefs.Save();
    }

    void LoadVolumeSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1f);
        SeVolume = PlayerPrefs.GetFloat("SeVolume", 1f);
        ApplyVolumes();
    }
}
