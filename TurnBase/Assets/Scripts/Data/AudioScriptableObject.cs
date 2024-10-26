using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio",menuName = "ScriptableObjects/AudioData",order=2)]
public class AudioScriptableObject : ScriptableObject
{
    public List<Sound> audioData = new List<Sound>();
    
    private HashSet<string> itemName;

    private void OnEnable()
    {
        itemName = new HashSet<string>();
        foreach (var sound in audioData)
        {
            itemName.Add(sound.name);
        }
    }

    public AudioClip FindAudioClipByName(string name)
    {
        if (itemName.Contains(name))
        {
            foreach (var sound in audioData)
            {
                if (sound.name == name)
                {
                    return sound.audio;
                }
            }
        }
        return null;
    }
}


[Serializable]
public class Sound
{
    public string name;
    public AudioClip audio;
}