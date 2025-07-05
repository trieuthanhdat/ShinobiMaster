using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDiffcultry
{
    [Header("RandomOnStartShoot")]
    public float Min = 1.3f;
    public float Max = 3.2f;

    public virtual float GetRandomTimeToStart() {
        return Random.Range(Min, Max);
    }
}
