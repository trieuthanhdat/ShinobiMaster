using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpExplosion : MonoBehaviour
{
    public ParticleSystem JumpEffect;
    public void Play()
    {
        JumpEffect.Play();
    }
}
