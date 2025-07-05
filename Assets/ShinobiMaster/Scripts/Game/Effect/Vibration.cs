using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
    private void Start()
    {
        EnemyControll.Singleton.EnemyDead += StartVibration;
    }

    private void StartVibration() {
        Development.Vibration.Vibrate(30L);
    }
}
