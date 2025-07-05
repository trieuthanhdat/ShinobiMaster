using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class DiagnosticUI : MonoBehaviour
{
    [SerializeField] private Text fpsText;


    Stopwatch timer;

    int count;

    private void Awake()
    {
        timer = new Stopwatch();
        timer.Start();
    }

    private void Update()
    {
        count++;
        if (timer.ElapsedMilliseconds > 1000) {
            fpsText.text = "FPS: " + count.ToString();
            count = 0;
            timer.Restart();
        }
    }
}
