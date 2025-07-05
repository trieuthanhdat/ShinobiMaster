using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pause
{
    public static bool IsPause { get; private set; }

    public static void SetPause()
    {
        IsPause = true;
        Physics.autoSimulation = false;
    }

    public static void OffPause()
    {
        IsPause = false;
        Physics.autoSimulation = true;
    }
}
