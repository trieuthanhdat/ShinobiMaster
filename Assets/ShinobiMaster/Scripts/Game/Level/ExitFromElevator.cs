using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ExitFromElevator : StageEvent
{
    [SerializeField] private StartElevator startElevatorEvent;

    bool show;

    Stopwatch timer;

    protected override void CallLoadStage()
    {
        timer = new Stopwatch();
        startElevatorEvent.PlayerExitFromElevatorEvent += StartShow;        
    }

    protected override void CallUpdateStage()
    {
        if (show && timer.ElapsedMilliseconds > 3000f)
        {
            timer.Stop();
            timer.Reset();
            show = false;
        }
    }

    private void StartShow()
    {
        show = true;;
        timer.Start();
    }

    protected override void CallUnLoadStage()
    {
    }
}
