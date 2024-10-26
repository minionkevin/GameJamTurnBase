using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource AudioPlayer;
    public AudioSource AudioBoss;
    public AudioSource AudioUI;
    public AudioSource AudioBGM;
    public List<AudioClip> AudioClipList;
    public Dictionary<string, AudioClip> AudioDic = new Dictionary<string, AudioClip>();

    public void init()
    {
        foreach (AudioClip clip in AudioClipList)
        {
            AudioDic.Add(clip.name, clip);
        }
    }
}


[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/AudioData", order = 1)]
public class AudioData : ScriptableObject
{
    public AudioClip audioClip;
    public string audioName;
}
