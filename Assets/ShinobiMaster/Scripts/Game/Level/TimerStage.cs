using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class TimerStage
{
    private static Stopwatch timer = new Stopwatch();

    public static void StartRecord() {
        timer.Stop();
        timer.Reset();
        timer.Start();
    }

    public static float GetRecrodS() {
        return timer.ElapsedMilliseconds / 1000f;
    }
}
