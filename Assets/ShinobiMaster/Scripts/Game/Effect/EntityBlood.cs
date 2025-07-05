using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlood : MonoBehaviour
{

    [SerializeField] private ParticleSystem bloodEffect;


    public void ShowBlood() {
        bloodEffect.Clear();
        bloodEffect.Play();
    }
}
