using UnityEngine;

public class BossHandComponent : MonoBehaviour
{
    public ParticleSystem Chop;
    public ParticleSystem Palm;
    public ParticleSystem Fist;

    public void PlayChop()
    {
        Chop.Play();
    }

    public void PlayPalm()
    {
        Palm.Play();
    }

    public void PlayFist()
    {
        Fist.Play();
    }
}
