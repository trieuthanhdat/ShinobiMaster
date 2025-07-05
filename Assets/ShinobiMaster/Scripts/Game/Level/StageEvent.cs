using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageEvent : MonoBehaviour
{
    protected Stage stage;

    protected virtual void Awake()
    {
        
    }
    public void LoadStage(Stage stage)
    {
        this.stage = stage;
        CallLoadStage();
    }

    protected virtual void CallLoadStage()
    {
    }

    public void UpdateStage()
    {
        CallUpdateStage();
    }

    protected virtual void CallUpdateStage()
    {
    }

    public void LateUpdateStage()
    {
        CallLateUpdateStage();
    }

    protected virtual void CallLateUpdateStage()
    {
    }

    public void UnLoadStage()
    {
        CallUnLoadStage();
    }

    protected virtual void CallUnLoadStage()
    {
    }
}
