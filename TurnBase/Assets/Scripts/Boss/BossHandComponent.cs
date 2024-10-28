using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BossHandComponent : MonoBehaviour
{
    public ParticleSystem Chop;
    public ParticleSystem Palm;
    public ParticleSystem Fist;

    public List<SpriteRenderer> HandSprites;

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

    public async void HandleAttack(Material damageMaterial)
    {
        foreach (var sprite in HandSprites)
        {
            if (!sprite.gameObject.activeSelf) continue;
            var tmp = sprite.material;
            sprite.material = damageMaterial;
            await Task.Delay(500);
            sprite.material = tmp;   
        }
    }
}
