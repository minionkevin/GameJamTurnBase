using UnityEngine;

public class BossHandComponent : MonoBehaviour
{
    public ParticleSystem Chop;
    public ParticleSystem Palm;

    public void PlayChop()
    {
        Chop.Play();
    }

    public void PlayPalm()
    {
        Palm.Play();
    }
}
