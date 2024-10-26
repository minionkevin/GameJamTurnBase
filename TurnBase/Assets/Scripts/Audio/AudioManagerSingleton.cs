using UnityEngine;

public class AudioManagerSingleton :BaseSingleton<AudioManagerSingleton>
{
    public AudioSource PlayerAudioSource;
    public AudioScriptableObject PlayerAudioData;

    public AudioSource BossAudioSource;
    public AudioScriptableObject BossAudioData;

    public void PlayPlayerAudio(string name)
    {
        PlayerAudioSource.PlayOneShot(PlayerAudioData.FindAudioClipByName(name));
    }
    
    public void PlayBossAudio(string name)
    {
        BossAudioSource.PlayOneShot(BossAudioData.FindAudioClipByName(name));
    }
}


