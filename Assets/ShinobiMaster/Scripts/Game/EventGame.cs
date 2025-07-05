using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EventDelet(EventGame eventGame);
public abstract class EventGame
{
    public event EventDelet EndEvent;

    public abstract void StartEvent();

    public abstract void Update();

    public void CallEndEvent()
    {
        if (EndEvent != null)
        {
            EndEvent(this);
            End();
        }
    }

    public abstract void End();
}
